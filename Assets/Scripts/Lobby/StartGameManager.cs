using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class StartGameManager : MonoBehaviour {
    private void Start() {
        LobbyManager.Instance.OnLobbyStartGame += LobbyManager_OnLobbyStartGame;
    }

    private void LobbyManager_OnLobbyStartGame(object sender, LobbyManager.LobbyEventArgs e) {
        //Start Game!
        if (LobbyManager.Instance.IsHost) {
            CreateRelay();
        } else {
            JoinRelay(LobbyManager.Instance.RelayJoinCode);
        }
    }

    public void StartHost() {
        Debug.Log("Starting Host...");
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host Started.");
    }

    public void StartClient() {
        Debug.Log("Starting Client...");
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client Started.");
    }

    public async void CreateRelay() {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Allocated Relay JoinCode: " + joinCode);


            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            StartHost();

            LobbyManager.Instance.SetRelayJoinCode(joinCode);
        } catch (RelayServiceException e) {
            Debug.Log(e);
        }
    }

    public async void JoinRelay(string JoinCode) {
        try {
            Debug.Log("Joining Relay with: " + JoinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(JoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            StartClient();
        } catch (RelayServiceException e) {
            Debug.Log(e);
        }
    }
}
