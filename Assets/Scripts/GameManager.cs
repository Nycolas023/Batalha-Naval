using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

    public const int GRID_SIZE = 10;
    public const float CELL_SIZE = 1.0f;
    public const float GRIDS_DISTANCE = 7.24f;
    public static GameManager Instance { get; private set; }

    [SerializeField] private Grid gridPlayer1;
    [SerializeField] private Grid gridPlayer2;

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

    public GamePosition[,] gridArrayPlayer1 = new GamePosition[GRID_SIZE, GRID_SIZE];
    public bool[,] gridArrayPlayer1Occupied = new bool[GRID_SIZE, GRID_SIZE];
    public List<IBoat> boatsPlayer1 = new List<IBoat>();

    public GamePosition[,] gridArrayPlayer2 = new GamePosition[GRID_SIZE, GRID_SIZE];
    public bool[,] gridArrayPlayer2Occupied = new bool[GRID_SIZE, GRID_SIZE];
    public List<IBoat> boatsPlayer2 = new List<IBoat>();

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one GameManager instance!");
        }
        Instance = this;
    }

    private void Start() {
        var gridXPosition = GRID_SIZE * CELL_SIZE / 2f + GRIDS_DISTANCE / 2f;
        InitializeGrid(gridArrayPlayer1, new Vector3(gridXPosition, 0, 0), gridPlayer1);
        InitializeGrid(gridArrayPlayer2, new Vector3(-gridXPosition, 0, 0), gridPlayer2);
        localPlayerType = Player.Player1;
        currentPlayablePlayerType = Player.Player1;
    }

    private void Update() {
        for (int x = 0; x < GRID_SIZE; x++) {
            for (int z = 0; z < GRID_SIZE; z++) {
                if (gridArrayPlayer1Occupied[x, z]) {
                    gridArrayPlayer1[x, z].GetComponent<Renderer>().material.color = Color.red;
                } else {
                    gridArrayPlayer1[x, z].GetComponent<Renderer>().material.color = Color.white;
                }
            }
        }
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

    public void InitializeGrid(GamePosition[,] gridArray, Vector3 initialPosition, Grid grid) {
        initialPosition = findInitialPositionToRender(initialPosition);
        grid.transform.position = initialPosition;
        for (int x = 0; x < GRID_SIZE; x++) {
            for (int y = 0; y < GRID_SIZE; y++) {
                GameObject cell = Instantiate(cellPrefab);
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
        for (int r = 0; r < boat.rotation / 90; r++) {
            boatGrid = new Utils().RotateMatrix(boatGrid);
            var temp = xCenter;
            xCenter = zCenter;
            zCenter = temp;
        }
        var testex = boatGrid.GetLength(0);
        var testez = boatGrid.GetLength(1);
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
                if (gridArrayPlayer1Occupied[gridPosition.x, gridPosition.z]) {
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
        for (int r = 0; r < boat.rotation / 90; r++) {
            boatGrid = new Utils().RotateMatrix(boatGrid);
            var temp = xCenter;
            xCenter = zCenter;
            zCenter = temp;
        }
        for (int x = 0; x < boatGrid.GetLength(1); x++) {
            for (int z = 0; z < boatGrid.GetLength(0); z++) {
                if (boatGrid[z, x] == 0) continue;
                Vector3Int gridPosition = new Vector3Int(
                    x - xCenter + boat.positonOnGrid.x,
                    0,
                    z - zCenter + boat.positonOnGrid.z
                );
                if (localPlayerType == Player.Player1) {
                    gridArrayPlayer1Occupied[gridPosition.x, gridPosition.z] = true;
                } else {
                    gridArrayPlayer2Occupied[gridPosition.x, gridPosition.z] = true;
                }
            }
        }
    }

    public void RemoveBoatFromGrid(IBoat boat) {
        int[,] boatGrid = boat.componetsGrid;
        int xCenter = boat.xCenter;
        int zCenter = boat.zCenter;
        for (int r = 0; r < boat.rotation / 90; r++) {
            boatGrid = new Utils().RotateMatrix(boatGrid);
            var temp = xCenter;
            xCenter = zCenter;
            zCenter = temp;
        }
        for (int x = 0; x < boatGrid.GetLength(1); x++) {
            for (int z = 0; z < boatGrid.GetLength(0); z++) {
                if (boatGrid[z, x] == 0) continue;
                Vector3Int gridPosition = new Vector3Int(
                    x - xCenter + boat.positonOnGrid.x,
                    0,
                    z - zCenter + boat.positonOnGrid.z
                );
                if (localPlayerType == Player.Player1) {
                    gridArrayPlayer1Occupied[gridPosition.x, gridPosition.z] = false;
                } else {
                    gridArrayPlayer2Occupied[gridPosition.x, gridPosition.z] = false;
                }
            }
        }
    }
}
