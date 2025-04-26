using System;
using Unity.VisualScripting;
using UnityEngine;

public class GamaManager : MonoBehaviour {

    public const int GRID_SIZE = 15;
    public const float CELL_SIZE = 1.0f;
    public const float GRIDS_DISTANCE = 20.0f;
    public static GamaManager Instance { get; private set; }

    public event EventHandler<OnChangePlayeblePlayerTypeEventArgs> OnChangePlayeblePlayerType;
    public class OnChangePlayeblePlayerTypeEventArgs : EventArgs {
        public Player playerType;
    }

    [SerializeField] private GameObject cellPrefab; // Prefab for the cell

    public enum Player {
        None,
        Player1,
        Player2
    }

    private Player localPlayerType;
    private Player currentPlayablePlayerType;
    public GamePosition[,] gridPlayer1 = new GamePosition[GRID_SIZE, GRID_SIZE];
    public GamePosition[,] gridPlayer2 = new GamePosition[GRID_SIZE, GRID_SIZE];


    // Good Camara positions
    // 17.5, 15, -5 - Player 1
    // -17.5, 15, -5 - Player 2
    // 0, 35, -5 - Middle

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one GameManager instance!");
        }
        Instance = this;
    }

    private void Start() {
        var gridXPosition = GRID_SIZE * CELL_SIZE / 2f + GRIDS_DISTANCE / 2f;
        InitializeGrid(gridPlayer1, new Vector3(gridXPosition, 0, 0));
        InitializeGrid(gridPlayer2, new Vector3(-gridXPosition, 0, 0));
        localPlayerType = Player.Player1;
        currentPlayablePlayerType = Player.Player1;
    }

    public void ChangeColor(GamePosition gamePosition) {
        gamePosition.GetComponent<Renderer>().material.color =
            gamePosition.GetComponent<Renderer>().material.color == Color.red ? Color.blue : Color.red;
        localPlayerType = localPlayerType == Player.Player1 ? Player.Player2 : Player.Player1;
        currentPlayablePlayerType = localPlayerType == Player.Player1 ? Player.Player2 : Player.Player1;
        Invoke("ChangeCameraPosition", 0.5f);
    }

    public void ChangeCameraPosition() {
        OnChangePlayeblePlayerType?.Invoke(this, new OnChangePlayeblePlayerTypeEventArgs {
            playerType = currentPlayablePlayerType
        });
    }

    public void InitializeGrid(GamePosition[,] grid, Vector3 initialPosition) {
        initialPosition = findInitialPositionToRender(initialPosition);
        for (int x = 0; x < GRID_SIZE; x++) {
            for (int y = 0; y < GRID_SIZE; y++) {
                GameObject cell = Instantiate(cellPrefab, new Vector3(
                        x * CELL_SIZE + initialPosition.x, 0, y * CELL_SIZE + initialPosition.z
                    ), Quaternion.identity);
                cell.GetComponent<GamePosition>().SetPosition(x, y);
                grid[x, y] = cell.GetComponent<GamePosition>();
            }
        }
    }

    private Vector3 findInitialPositionToRender(Vector3 initialPosition) {
        Vector3 offset = new Vector3(GRID_SIZE / 2f * CELL_SIZE, 0, GRID_SIZE / 2f * CELL_SIZE);
        Vector3 newInitialPosition = new Vector3(initialPosition.x - offset.x + (CELL_SIZE / 2), 0, initialPosition.z - offset.z + +(CELL_SIZE / 2));
        return newInitialPosition;
    }
}
