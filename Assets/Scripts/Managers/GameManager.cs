using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour {

    public const int GRID_SIZE = 10;
    public const float CELL_SIZE = 1.0f;
    public const float GRIDS_DISTANCE = 7.24f;
    public const int MAX_BOATS_SPAWNED = 5;
    public static GameManager Instance { get; private set; }

    [SerializeField] private Grid gridPlayer1;
    [SerializeField] private Grid gridPlayer2;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private ParticleSystem explosionEffectPrefab;
    public enum PlayerType {
        None,
        Player1,
        Player2
    }

    private PlayerType localPlayerType;
    public List<IBoat> localPlayerBoats = new List<IBoat>();
    private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>(PlayerType.None);

    public GamePosition[,] gridArrayPlayer1 = new GamePosition[GRID_SIZE, GRID_SIZE];
    public bool[,] gridArrayPlayer1Occupied = new bool[GRID_SIZE, GRID_SIZE];
    private bool isPlayer1Ready = false;

    public GamePosition[,] gridArrayPlayer2 = new GamePosition[GRID_SIZE, GRID_SIZE];
    public bool[,] gridArrayPlayer2Occupied = new bool[GRID_SIZE, GRID_SIZE];
    private bool isPlayer2Ready = false;

    //----------------------------------------- Events --------------------------------------------------------

    public event EventHandler<PlayerTypeEventArgs> OnNetworkSpawned;
    public event EventHandler<PlayerTypeEventArgs> OnChangePlayablePlayerType;
    public class PlayerTypeEventArgs : EventArgs {
        public PlayerType playerType;
    }

    //----------------------------------------- Events --------------------------------------------------------

    public bool IsGameStarted = false;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one GameManager instance!");
        }
        Instance = this;
    }

    public override void OnNetworkSpawn() {
        if (NetworkManager.Singleton.LocalClientId == 0) {
            localPlayerType = PlayerType.Player1;
        } else {
            localPlayerType = PlayerType.Player2;
        }
        OnNetworkSpawned?.Invoke(this, new PlayerTypeEventArgs {
            playerType = localPlayerType
        });

        var gridXPosition = GRID_SIZE * CELL_SIZE / 2f + GRIDS_DISTANCE / 2f;
        InitializeGrid(gridArrayPlayer1, new Vector3(gridXPosition, 0, 0), gridPlayer1);
        InitializeGrid(gridArrayPlayer2, new Vector3(-gridXPosition, 0, 0), gridPlayer2);

        IsGameStarted = true;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {

        }
        //Mostra quais posições estão ocupadas pelo player local
        // if (!IsGameStarted) return;
        // bool[,] gridArrayPlayerOccupied = localPlayerType == PlayerType.Player1 ? gridArrayPlayer1Occupied : gridArrayPlayer2Occupied;
        // GamePosition[,] gridArrayPlayer = localPlayerType == PlayerType.Player1 ? gridArrayPlayer1 : gridArrayPlayer2;
        // for (int x = 0; x < GRID_SIZE; x++) {
        //     for (int z = 0; z < GRID_SIZE; z++) {
        //         if (gridArrayPlayerOccupied[x, z]) {
        //             gridArrayPlayer[x, z].GetComponent<Renderer>().material.color = Color.red;
        //         } else {
        //             gridArrayPlayer[x, z].GetComponent<Renderer>().material.color = Color.white;
        //         }
        //     }
        // }
    }

    [Rpc(SendTo.Server)]
    public void OnChangePlayersReadyRpc() {
        if (isPlayer1Ready && isPlayer2Ready) {
            Debug.Log("Game started!");
            currentPlayablePlayerType.Value = PlayerType.Player1;
            TriggerChangePlayablePlayerTypeRpc(currentPlayablePlayerType.Value);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerChangePlayablePlayerTypeRpc(PlayerType playerType) {
        OnChangePlayablePlayerType?.Invoke(this, new PlayerTypeEventArgs {
            playerType = playerType
        });
    }

    [Rpc(SendTo.Server)]
    public void OnClickGamePositionRpc(int x, int z, PlayerType playerType) {
        if (!isPlayer1Ready || !isPlayer2Ready) {
            Debug.Log("Game not started yet!");
            return;
        }

        if (playerType != currentPlayablePlayerType.Value) {
            Debug.Log("Not your turn!");
            return;
        }

        GameObject gamePosition = playerType == PlayerType.Player1 ?
            gridArrayPlayer2[x, z].gameObject :
            gridArrayPlayer1[x, z].gameObject;

        if (gamePosition.GetComponent<GamePosition>().hasBeenShot) {
            Debug.Log("Position was already shot!");
            return;
        } else {
            TriggerChangeGamePositionColorRpc(x, z, playerType);
        }

        currentPlayablePlayerType.Value = playerType == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;
        Invoke(nameof(ChangeCameraPositionRpc), 0.5f);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerChangeGamePositionColorRpc(int x, int z, PlayerType playerType) {
        GameObject gamePosition = playerType == PlayerType.Player1 ?
            gridArrayPlayer2[x, z].gameObject :
            gridArrayPlayer1[x, z].gameObject;
        bool[,] gridArrayPlayerOccupied = playerType == PlayerType.Player1 ? gridArrayPlayer2Occupied : gridArrayPlayer1Occupied;

        gamePosition.GetComponent<GamePosition>().SetHasBeenShot(true);
        if (gridArrayPlayerOccupied[x, z]) {
            gamePosition.GetComponent<Renderer>().material.color = Color.red;

            var gameObject = Instantiate(explosionEffectPrefab, gamePosition.transform.position, Quaternion.identity);
            gameObject.Play();
            Destroy(gameObject.gameObject, 2f);

            CheckIfBoatIsDestroyedRpc(x, z, playerType);
        } else {
            gamePosition.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ChangeCameraPositionRpc() {
        OnChangePlayablePlayerType?.Invoke(this, new PlayerTypeEventArgs {
            playerType = currentPlayablePlayerType.Value
        });
    }

    public void InitializeGrid(GamePosition[,] gridArray, Vector3 initialPosition, Grid grid) {
        initialPosition = findInitialPositionToRender(initialPosition);
        grid.transform.position = initialPosition;
        for (int x = 0; x < GRID_SIZE; x++) {
            for (int y = 0; y < GRID_SIZE; y++) {
                GameObject cell = Instantiate(cellPrefab);
                cell.GetComponent<GamePosition>().SetPosition(x, y);
                cell.transform.SetParent(grid.transform);
                cell.transform.localPosition = new Vector3(
                    x * CELL_SIZE + (CELL_SIZE / 2f), 0, y * CELL_SIZE + (CELL_SIZE / 2f)
                );
                gridArray[x, y] = cell.GetComponent<GamePosition>();
            }
        }
    }

    private Vector3 findInitialPositionToRender(Vector3 initialPosition) {
        Vector3 offset = new Vector3(GRID_SIZE / 2f * CELL_SIZE, 0, GRID_SIZE / 2f * CELL_SIZE);
        Vector3 newInitialPosition = new Vector3(
            initialPosition.x - offset.x,
            0,
            initialPosition.z - offset.z
        );
        return newInitialPosition;
    }

    public bool IsBoatPositionValid(IBoat boat) {
        if (boat.positonOnGrid == Vector3Int.zero) return false;
        int[,] boatGrid = boat.componetsGrid;
        int xCenter = boat.xCenter;
        int zCenter = boat.zCenter;
        bool[,] gridArrayOccupied = localPlayerType == PlayerType.Player1 ? gridArrayPlayer1Occupied : gridArrayPlayer2Occupied;

        for (int x = 0; x < boatGrid.GetLength(1); x++) {
            for (int z = 0; z < boatGrid.GetLength(0); z++) {
                if (boatGrid[z, x] == 0) continue;
                Vector3Int gridPosition = new Vector3Int(
                    x - xCenter + boat.positonOnGrid.x,
                    0,
                    z - zCenter + boat.positonOnGrid.z
                );
                if (gridPosition.x < 0 || gridPosition.x >= GRID_SIZE || gridPosition.z < 0 || gridPosition.z >= GRID_SIZE) {
                    Debug.Log("Boat is out of grid: " + gridPosition);
                    return false;
                }
                if (gridArrayOccupied[gridPosition.x, gridPosition.z]) {
                    Debug.Log("Boat position is occupied: " + gridPosition);
                    return false;
                }
            }
        }
        Debug.Log("Boat position is valid: " + boat.positonOnGrid);
        return true;
    }

    public void AddBoatToGrid(IBoat boat) {
        int[,] boatGrid = boat.componetsGrid;
        int xCenter = boat.xCenter;
        int zCenter = boat.zCenter;

        for (int x = 0; x < boatGrid.GetLength(1); x++) {
            for (int z = 0; z < boatGrid.GetLength(0); z++) {
                if (boatGrid[z, x] == 0) continue;
                int gridXPosition = x - xCenter + boat.positonOnGrid.x;
                int gridZPosition = z - zCenter + boat.positonOnGrid.z;
                TriggerAddBoatFromGridRpc(
                    gridXPosition,
                    gridZPosition,
                    localPlayerType
                );
                if (localPlayerType == PlayerType.Player1) {
                    gridArrayPlayer1[gridXPosition, gridZPosition].boatOnPosition = boat;
                } else {
                    gridArrayPlayer2[gridXPosition, gridZPosition].boatOnPosition = boat;
                }
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerAddBoatFromGridRpc(int x, int z, PlayerType playerType) {
        if (playerType == PlayerType.Player1) {
            gridArrayPlayer1Occupied[x, z] = true;
        } else {
            gridArrayPlayer2Occupied[x, z] = true;
        }
    }

    public void RemoveBoatFromGrid(IBoat boat) {
        int[,] boatGrid = boat.componetsGrid;
        int xCenter = boat.xCenter;
        int zCenter = boat.zCenter;

        for (int x = 0; x < boatGrid.GetLength(1); x++) {
            for (int z = 0; z < boatGrid.GetLength(0); z++) {
                if (boatGrid[z, x] == 0) continue;
                TriggerRemoveBoatFromGridRpc(
                    x - xCenter + boat.positonOnGrid.x,
                    z - zCenter + boat.positonOnGrid.z,
                    localPlayerType
                );
                if (localPlayerType == PlayerType.Player1) {
                    gridArrayPlayer1[x, z].boatOnPosition = null;
                } else {
                    gridArrayPlayer2[x, z].boatOnPosition = null;
                }
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerRemoveBoatFromGridRpc(int x, int z, PlayerType playerType) {
        if (playerType == PlayerType.Player1) {
            gridArrayPlayer1Occupied[x, z] = false;
        } else {
            gridArrayPlayer2Occupied[x, z] = false;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void CheckIfBoatIsDestroyedRpc(int xIndex, int zIndex, PlayerType playerType) {
        GamePosition[,] gridArrayPlayer = playerType == PlayerType.Player1 ? gridArrayPlayer2 : gridArrayPlayer1;
        GamePosition gamePosition = gridArrayPlayer[xIndex, zIndex];
        if (gamePosition.boatOnPosition != null) {
            IBoat boat = gamePosition.boatOnPosition;
            int[,] boatGrid = boat.componetsGrid;
            int xCenter = boat.xCenter;
            int zCenter = boat.zCenter;

            for (int x = 0; x < boatGrid.GetLength(1); x++) {
                for (int z = 0; z < boatGrid.GetLength(0); z++) {
                    if (boatGrid[z, x] == 0) continue;
                    var gridXPosition = x - xCenter + boat.positonOnGrid.x;
                    var gridZPosition = z - zCenter + boat.positonOnGrid.z;
                    if (!gridArrayPlayer[gridXPosition, gridZPosition].hasBeenShot) {
                        Debug.Log("Boat not destroyed!");
                        return;
                    }
                }
            }
            Debug.Log("Boat destroyed!");
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetIsPlayer1ReadyRpc(bool isReady) {
        isPlayer1Ready = isReady;
        OnChangePlayersReadyRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetIsPlayer2ReadyRpc(bool isReady) {
        isPlayer2Ready = isReady;
        OnChangePlayersReadyRpc();
    }

    public PlayerType GetLocalPlayerType() { return localPlayerType; }
}
