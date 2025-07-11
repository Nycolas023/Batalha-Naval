using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameManager : NetworkBehaviour {

    public const int GRID_WIDTH = 10;
    public const int GRID_HEIGHT = 10;
    public const float CELL_SIZE = 1.0f;
    public const float GRIDS_DISTANCE = 2.8f;
    public const int MAX_BOATS = 5;
    public static GameManager Instance { get; private set; }

    [SerializeField] private Grid gridPlayer1;
    [SerializeField] private Grid gridPlayer2;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private ParticleSystem explosionEffectPrefab;
    [SerializeField] private Timer timer;
    [SerializeField] public PlayerModelSO playerModel;
    [SerializeField] private Material defaultPositionMaterial;
    [SerializeField] private Transform floor;
    [SerializeField] private FlashImage flashImage;

    public enum PlayerType {
        None,
        Player1,
        Player2
    }

    private PlayerType localPlayerType;
    public List<IBoat> localPlayerBoats = new List<IBoat>();
    public NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>(PlayerType.None);
    public NetworkVariable<FixedString64Bytes> Player1Name = new NetworkVariable<FixedString64Bytes>("Player 1 teste");
    public NetworkVariable<FixedString64Bytes> Player2Name = new NetworkVariable<FixedString64Bytes>("Player 2 teste");
    public string localThemeSelected = "";
    public int localNumberOfBoastsOnGrid = 0;

    public GamePosition[,] gridArrayPlayer1 = new GamePosition[GRID_WIDTH, GRID_HEIGHT];
    public NetworkVariable<bool> isPlayer1Ready = new NetworkVariable<bool>(false);

    public GamePosition[,] gridArrayPlayer2 = new GamePosition[GRID_WIDTH, GRID_HEIGHT];
    public NetworkVariable<bool> isPlayer2Ready = new NetworkVariable<bool>(false);

    //----------------------------------------- Events --------------------------------------------------------

    public event EventHandler<PlayerTypeEventArgs> OnNetworkSpawned;
    public event EventHandler<PlayerTypeEventArgs> OnChangePlayablePlayerType;
    public class PlayerTypeEventArgs : EventArgs {
        public PlayerType playerType;
    }
    public event EventHandler OnGameStart;
    public event EventHandler<PlayerTypeEventArgs> OnGameWin;
    public event EventHandler OnRematch;
    public event EventHandler OnLostTurn;
    public event EventHandler<EmojiCallEventArgs> OnEmojiCall;
    public class EmojiCallEventArgs : EventArgs {
        public int spriteIndex;
        public PlayerType playerType;
    }

    //----------------------------------------- Events --------------------------------------------------------
    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one GameManager instance!");
        }
        Instance = this;
        localThemeSelected = playerModel.Value?.User_Present_Theme;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void OnClientConnectRpc() {
        SetPlayerModel();
    }

    private async void SetPlayerModel() {
        var api = new Api();

        if (localThemeSelected == null || localThemeSelected == "") {
            PlayerModel player;
            if (localPlayerType == PlayerType.Player1)
                player = await api.UpdatePlayerModel(2);
            else
                player = await api.UpdatePlayerModel(3);
            playerModel.Value = player;
            localThemeSelected = player.User_Present_Theme;
        }

        SetPlayerNameRpc(playerModel.Value.User_Nickname, localPlayerType);
        _ = SetThemeURL();
    }

    private async Task SetThemeURL() {
        var api = new Api();
        var url = await api.GetURLForThemeAsync(localThemeSelected);
        VideoPlayer videoPlayer = floor.GetComponent<VideoPlayer>();
        videoPlayer.url = url;
        videoPlayer.Play();
        SoundManager.Instance.PlayBackgroundThemeMusic(localThemeSelected);
    }

    [Rpc(SendTo.Server)]
    public void SetPlayerNameRpc(string playerName, PlayerType playerType) {
        if (playerType == PlayerType.Player1) {
            Player1Name.Value = playerName;
        } else if (playerType == PlayerType.Player2) {
            Player2Name.Value = playerName;
        }
    }

    public GamePosition[,] GetLocalGridArray() {
        return localPlayerType == PlayerType.Player1 ? gridArrayPlayer2 : gridArrayPlayer1;
    }

    public override void OnNetworkSpawn() {
        Debug.Log("GameManager OnNetworkSpawn called");
        if (NetworkManager.Singleton.LocalClientId == 0) {
            localPlayerType = PlayerType.Player1;
        } else {
            localPlayerType = PlayerType.Player2;
        }

        if (IsServer) {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj) {
        Debug.Log("Client Connected");
        OnClientConnectRpc();
        if (NetworkManager.Singleton.ConnectedClients.Count == 2) {
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerOnGameStartedRpc() {
        OnNetworkSpawned?.Invoke(this, new PlayerTypeEventArgs {
            playerType = localPlayerType
        });

        var gridXPosition = GRID_WIDTH * CELL_SIZE / 2f + GRIDS_DISTANCE / 2f;
        InitializeGrid(gridArrayPlayer1, new Vector3(gridXPosition, 0, 0), gridPlayer1);
        InitializeGrid(gridArrayPlayer2, new Vector3(-gridXPosition, 0, 0), gridPlayer2);
    }

    private void Update() {
        PaintShotPositions(gridArrayPlayer1);
        // PaintOccupiedPositions(gridArrayPlayer1);
        PaintShotPositions(gridArrayPlayer2);
        // PaintOccupiedPositions(gridArrayPlayer2);
    }

    private void PaintShotPositions(GamePosition[,] grid) {
        if (grid[0, 0] == null) return;
        for (int x = 0; x < GRID_WIDTH; x++) {
            for (int z = 0; z < GRID_HEIGHT; z++) {
                if (grid[x, z].hasBeenShot) {
                    if (grid[x, z].isOccupied) {
                        grid[x, z].GetComponent<Renderer>().material.color = Color.red;
                    } else {
                        grid[x, z].GetComponent<Renderer>().material.color = Color.blue;
                    }
                } else {
                    grid[x, z].GetComponent<Renderer>().material = defaultPositionMaterial;
                }
            }
        }
    }

    private void PaintOccupiedPositions(GamePosition[,] grid) {
        if (grid[0, 0] == null) return;
        for (int x = 0; x < GRID_WIDTH; x++) {
            for (int z = 0; z < GRID_HEIGHT; z++) {
                if (grid[x, z].isOccupied) {
                    grid[x, z].GetComponent<Renderer>().material.color = Color.yellow;
                } else {
                    grid[x, z].GetComponent<Renderer>().material = defaultPositionMaterial;
                }
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void OnChangePlayersReadyRpc() {
        if (isPlayer1Ready.Value && isPlayer2Ready.Value) {
            Debug.Log("Game started!");
            currentPlayablePlayerType.Value = PlayerType.Player1;
            TriggerOnStartGameRpc();
            TriggerChangePlayablePlayerTypeRpc(currentPlayablePlayerType.Value);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerChangePlayablePlayerTypeRpc(PlayerType playerType) {
        OnChangePlayablePlayerType?.Invoke(this, new PlayerTypeEventArgs {
            playerType = playerType
        });
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnStartGameRpc() {
        // 👉 No início de cada player, já desativa o próprio grid permanentemente
        if (IsLocalPlayerPlayer1()) {
            SetGridCollidersActive(gridPlayer1, false);  // Player1 não interage com o Grid1
            SetGridCollidersActive(gridPlayer2, true);   // Player1 pode interagir com o Grid2
        } else if (IsLocalPlayerPlayer2()) {
            SetGridCollidersActive(gridPlayer2, false);  // Player2 não interage com o Grid2
            SetGridCollidersActive(gridPlayer1, true);   // Player2 pode interagir com o Grid1
        }

        OnGameStart?.Invoke(this, EventArgs.Empty);
    }

    private bool IsLocalPlayerPlayer1() {
        return GetLocalPlayerType() == PlayerType.Player1;
    }

    private bool IsLocalPlayerPlayer2() {
        return GetLocalPlayerType() == PlayerType.Player2;
    }



    [Rpc(SendTo.Server)]
    public void OnClickGamePositionRpc(int x, int z, PlayerType playerType) {
        if (!isPlayer1Ready.Value || !isPlayer2Ready.Value) {
            Debug.Log("Game not started yet!");
            return;
        }

        if (currentPlayablePlayerType.Value == PlayerType.None) {
            Debug.Log("Cant make a play!");
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
            if (DidProjectileHit(x, z, playerType)) SoundManager.Instance.PlayAcertoSound(localThemeSelected);
            else SoundManager.Instance.PlayErroSound(localThemeSelected);
        }


        currentPlayablePlayerType.Value = playerType == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;

        Invoke(nameof(ChangeCameraPositionRpc), 1.1f);
    }

    [Rpc(SendTo.Server)]
    public void OnClickArea2x2Rpc(int x, int z, PlayerType playerType) {
        if (!isPlayer1Ready.Value || !isPlayer2Ready.Value) return;
        if (currentPlayablePlayerType.Value != playerType) return;

        bool didProjectileHit = false;

        // Aplica o ataque em cada uma das 4 células do quadrado 2x2
        for (int dx = 0; dx < 2; dx++) {
            for (int dz = 0; dz < 2; dz++) {
                int targetX = x + dx;
                int targetZ = z + dz;

                if (targetX >= 0 && targetX < GRID_WIDTH && targetZ >= 0 && targetZ < GRID_HEIGHT) {
                    TriggerChangeGamePositionColorRpc(targetX, targetZ, playerType);
                    if (DidProjectileHit(targetX, targetZ, playerType)) didProjectileHit = true;
                }
            }
        }

        if (didProjectileHit) SoundManager.Instance.PlayAcertoSound(localThemeSelected);
        else SoundManager.Instance.PlayErroSound(localThemeSelected);

        // Troca o turno
        currentPlayablePlayerType.Value = playerType == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;
        Invoke(nameof(ChangeCameraPositionRpc), 1.1f);
    }

    [Rpc(SendTo.Server)]
    public void OnClickDiagonalXRpc(int x, int z, PlayerType playerType) {
        if (!isPlayer1Ready.Value || !isPlayer2Ready.Value) return;
        if (currentPlayablePlayerType.Value != playerType) return;
        
        Vector2Int[] offsets = {
            new Vector2Int(0, 0),     // centro
            new Vector2Int(-1, -1),   // diagonal superior esquerda
            new Vector2Int(1, -1),    // diagonal superior direita
            new Vector2Int(-1, 1),    // diagonal inferior esquerda
            new Vector2Int(1, 1)      // diagonal inferior direita
        };

        bool didProjectileHit = false;

        foreach (var offset in offsets) {
            int targetX = x + offset.x;
            int targetZ = z + offset.y;

            if (targetX >= 0 && targetX < GRID_WIDTH && targetZ >= 0 && targetZ < GRID_HEIGHT) {
                TriggerChangeGamePositionColorRpc(targetX, targetZ, playerType);
                if (DidProjectileHit(targetX, targetZ, playerType)) didProjectileHit = true;
            }
        }

        if (didProjectileHit) SoundManager.Instance.PlayAcertoSound(localThemeSelected);
        else SoundManager.Instance.PlayErroSound(localThemeSelected);

        currentPlayablePlayerType.Value = playerType == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;
        Invoke(nameof(ChangeCameraPositionRpc), 1.1f);
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerChangeGamePositionColorRpc(int x, int z, PlayerType playerType) {
        GamePosition[,] gridArrayPlayer = playerType == PlayerType.Player1 ? gridArrayPlayer2 : gridArrayPlayer1;
        GamePosition gamePosition = gridArrayPlayer[x, z];

        gamePosition.GetComponent<GamePosition>().SetHasBeenShot(true);
        if (localPlayerType != playerType) gamePosition.GetComponent<GamePosition>().ShowShotIndicator();

        Vector3 targetPosition = gamePosition.transform.position;
        ShootProjectileAnimation.Instance.SpawnProjectileAnimation(targetPosition, playerType);

        if (gridArrayPlayer[x, z].isOccupied) {
            var gameObject = Instantiate(explosionEffectPrefab, gamePosition.transform.position, Quaternion.identity);
            gameObject.Play();
            Destroy(gameObject.gameObject, 2f);

            CheckIfBoatIsDestroyedRpc(x, z, playerType);
            CheckWinnerRpc();
        }
    }

    public bool DidProjectileHit(int x, int z, PlayerType playerType) {
        GamePosition[,] gridArrayPlayer = playerType == PlayerType.Player1 ? gridArrayPlayer2 : gridArrayPlayer1;
        return gridArrayPlayer[x, z].isOccupied;
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void ChangeCameraPositionRpc() {
        OnChangePlayablePlayerType?.Invoke(this, new PlayerTypeEventArgs {
            playerType = currentPlayablePlayerType.Value
        });
    }

    [Rpc(SendTo.Server)]
    public void LostTurnRpc() {
        Debug.Log("Lost turn called!");
        if (currentPlayablePlayerType.Value == PlayerType.Player1) {
            currentPlayablePlayerType.Value = PlayerType.Player2;
        } else if (currentPlayablePlayerType.Value == PlayerType.Player2) {
            currentPlayablePlayerType.Value = PlayerType.Player1;
        }
        ChangeCameraPositionRpc();
        TriggerLostTurnAnimationRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerLostTurnAnimationRpc() {
        OnLostTurn?.Invoke(this, EventArgs.Empty);
    }

    public void InitializeGrid(GamePosition[,] gridArray, Vector3 initialPosition, Grid grid) {
        initialPosition = findInitialPositionToRender(initialPosition);
        grid.transform.position = initialPosition;
        grid.GetComponent<BoxCollider>().size = new Vector3(
            GRID_WIDTH * CELL_SIZE,
            grid.GetComponent<BoxCollider>().size.y,
            GRID_HEIGHT * CELL_SIZE
        );
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
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
        Vector3 offset = new Vector3(GRID_WIDTH / 2f * CELL_SIZE, 0, GRID_HEIGHT / 2f * CELL_SIZE);
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
        GamePosition[,] gridArray = localPlayerType == PlayerType.Player1 ? gridArrayPlayer1 : gridArrayPlayer2;

        for (int x = 0; x < boatGrid.GetLength(1); x++) {
            for (int z = 0; z < boatGrid.GetLength(0); z++) {
                if (boatGrid[z, x] == 0) continue;
                Vector3Int gridPosition = new Vector3Int(
                    x - xCenter + boat.positonOnGrid.x,
                    0,
                    z - zCenter + boat.positonOnGrid.z
                );
                if (gridPosition.x < 0 || gridPosition.x >= GRID_WIDTH || gridPosition.z < 0 || gridPosition.z >= GRID_HEIGHT) {
                    Debug.Log("Boat is out of grid: " + gridPosition);
                    return false;
                }
                if (gridArray[gridPosition.x, gridPosition.z].isOccupied) {
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

        localNumberOfBoastsOnGrid++;
        SoundManager.Instance.PlayPosicionarSound(localThemeSelected);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerAddBoatFromGridRpc(int x, int z, PlayerType playerType) {
        if (playerType == PlayerType.Player1) {
            gridArrayPlayer1[x, z].isOccupied = true;
        } else {
            gridArrayPlayer2[x, z].isOccupied = true;
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

        localNumberOfBoastsOnGrid--;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerRemoveBoatFromGridRpc(int x, int z, PlayerType playerType) {
        if (playerType == PlayerType.Player1) {
            gridArrayPlayer1[x, z].isOccupied = false;
        } else {
            gridArrayPlayer2[x, z].isOccupied = false;
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
                        return;
                    }
                }
            }
            Debug.Log("Boat destroyed!");
        }
    }

    [Rpc(SendTo.Server)]
    public void SetIsPlayer1ReadyRpc(bool isReady) {
        isPlayer1Ready.Value = isReady;
        OnChangePlayersReadyRpc();
    }

    [Rpc(SendTo.Server)]
    public void SetIsPlayer2ReadyRpc(bool isReady) {
        isPlayer2Ready.Value = isReady;
        OnChangePlayersReadyRpc();
    }

    [Rpc(SendTo.Server)]
    public void CheckWinnerRpc() {
        if (CheckGridForWinner(gridArrayPlayer1)) {
            OnMatchWinnerRpc(PlayerType.Player2);
        } else if (CheckGridForWinner(gridArrayPlayer2)) {
            OnMatchWinnerRpc(PlayerType.Player1);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void OnMatchWinnerRpc(PlayerType playerType) {
        if (IsHost) currentPlayablePlayerType.Value = PlayerType.None;
        Debug.Log("Winner: " + playerType);
        OnGameWin?.Invoke(this, new PlayerTypeEventArgs {
            playerType = playerType
        });
        TriggerChangePlayablePlayerTypeRpc(playerType);
    }

    private bool CheckGridForWinner(GamePosition[,] gridArray) {
        for (int x = 0; x < GRID_WIDTH; x++) {
            for (int z = 0; z < GRID_HEIGHT; z++) {
                if (gridArray[x, z].isOccupied && !gridArray[x, z].hasBeenShot) return false;
            }
        }
        return true;
    }

    public PlayerType GetLocalPlayerType() { return localPlayerType; }

    public bool ArePlayersReady() {
        return isPlayer1Ready.Value && isPlayer2Ready.Value;
    }


    [Rpc(SendTo.Server)]
    public void RematchRpc() {
        isPlayer1Ready.Value = false;
        isPlayer2Ready.Value = false;
        currentPlayablePlayerType.Value = PlayerType.None;
        TriggerRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerRematchRpc() {
        DestroyLocalPlayerBoats();
        ClearGrid(gridArrayPlayer1);
        ClearGrid(gridArrayPlayer2);
        OnRematch?.Invoke(this, EventArgs.Empty);
        OnNetworkSpawned?.Invoke(this, new PlayerTypeEventArgs {
            playerType = localPlayerType
        });
    }

    private void DestroyLocalPlayerBoats() {
        foreach (IBoat boat in localPlayerBoats) {
            Destroy(boat.gameObject);
        }
        localPlayerBoats.Clear();
    }

    private void ClearGrid(GamePosition[,] gridArray) {
        for (int x = 0; x < GRID_WIDTH; x++) {
            for (int z = 0; z < GRID_HEIGHT; z++) {
                gridArray[x, z].isOccupied = false;
                gridArray[x, z].hasBeenShot = false;
                gridArray[x, z].boatOnPosition = null;
                gridArray[x, z].GetComponent<GamePosition>().HideShotIndicator();
            }
        }
    }

    public PlayerType GetCurrentPlayablePlayerType() {
        return currentPlayablePlayerType.Value;
    }

    public Vector3 GetGridPlayer1Position() {
        return gridPlayer1.transform.position;
    }

    public Vector3 GetGridPlayer2Position() {
        return gridPlayer2.transform.position;
    }

    public void SetGridCollidersActive(Grid grid, bool active) {
        foreach (Transform cell in grid.transform) {
            Collider collider = cell.GetComponent<Collider>();
            if (collider != null) {
                collider.enabled = active;
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void QuitGameRpc() {
        SoundManager.Instance.PlayMenuBackgroundMusic();
        if (NetworkManager.Singleton.IsServer)
            ShutdownAndDisconnectPlayersRpc();
        SceneManager.LoadScene("TelaInicialScene");
    }

    [Rpc(SendTo.Server)]
    public void ShutdownAndDisconnectPlayersRpc() {
        NetworkManager.Singleton.DisconnectClient(1);
        NetworkManager.Singleton.Shutdown();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void TriggerEmojiAnimationRpc(int spriteIndex, PlayerType playerType) {
        OnEmojiCall?.Invoke(this, new EmojiCallEventArgs {
            spriteIndex = spriteIndex,
            playerType = playerType
        });
    }
}
