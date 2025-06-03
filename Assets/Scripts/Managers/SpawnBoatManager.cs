using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnBoatManager : MonoBehaviour {

    public static SpawnBoatManager Instance { get; private set; }

    public Vector3 initialBoatPositionPlayer1;
    public Vector3 initialBoatPositionPlayer2;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one SpawnBoatManager instance!");
        }
        Instance = this;
    }

    private void Start() {
        initialBoatPositionPlayer1 = new Vector3(8.62f, 0.6f, -6.3f);
        initialBoatPositionPlayer2 = new Vector3(-8.62f, 0.6f, -6.3f);
    }

    public void SpawnBoat(GameObject boatPrefab) {
        bool isLocalPlayerReady = GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Player1 ?
            GameManager.Instance.isPlayer1Ready.Value : GameManager.Instance.isPlayer2Ready.Value;
        if (isLocalPlayerReady) {
            Debug.Log("Player is ready, cannot spawn more boats!");
            return;
        }

        Vector3 initialPosition = GameManager.Instance.GetLocalPlayerType() ==
            GameManager.PlayerType.Player1 ? initialBoatPositionPlayer1 : initialBoatPositionPlayer2;

        GameObject boat = Instantiate(boatPrefab, initialPosition, Quaternion.identity);
        var boatComponent = boat.GetComponent<IBoat>();

        List<IBoat> boats = GameManager.Instance.localPlayerBoats;

        if (IsBoatPlacementLimitReached(boatComponent, boats)) {
            Debug.Log("Cant Spawn more of this type of boat!");
            Destroy(boat);
            return;
        }

        if (GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Player1) {
            GameManager.Instance.localPlayerBoats.Add(boat.GetComponent<IBoat>());
        } else {
            GameManager.Instance.localPlayerBoats.Add(boat.GetComponent<IBoat>());
        }

        BoatDraggerManager.Instance.SetBoatInSpawn(boatComponent);
        SpawnBoatManagerUI.Instance.UpdateNumberOfBoatsToBePlaced();
    }

    private bool IsBoatPlacementLimitReached(IBoat boat, List<IBoat> localPlayerBoats) {
        int numberOfThisBoatSpawned = localPlayerBoats.FindAll(b => b.name == boat.name).Count;
        return numberOfThisBoatSpawned >= boat.placementLimit;
    }
}
