using System;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpawnBoatManagerUI : MonoBehaviour {


    [SerializeField] private GameObject Boat2x2Prefab;
    [SerializeField] private GameObject Boat1x1Prefab;
    [SerializeField] private GameObject Boat1x2Prefab;
    [SerializeField] private GameObject Boat1x3Prefab;
    [SerializeField] private GameObject Boat1x4Prefab;

    [SerializeField] private Button buttonSpawnBoat2x2;
    [SerializeField] private Button buttonSpawnBoat1x1;
    [SerializeField] private Button buttonSpawnBoat1x2;
    [SerializeField] private Button buttonSpawnBoat1x3;
    [SerializeField] private Button buttonSpawnBoat1x4;

    [SerializeField] private TextMeshProUGUI boat2x2Text;
    [SerializeField] private TextMeshProUGUI boat1x1Text;
    [SerializeField] private TextMeshProUGUI boat1x2Text;
    [SerializeField] private TextMeshProUGUI boat1x3Text;
    [SerializeField] private TextMeshProUGUI boat1x4Text;

    [SerializeField] private Button testButton;

    public static SpawnBoatManagerUI Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one SpawnBoatManagerUI instance!");
        }
        Instance = this;
    }

    private void Start() {
        var api = new Api();

        buttonSpawnBoat2x2.onClick.AddListener(() => OnClickSpawnBoat(Boat2x2Prefab));
        buttonSpawnBoat1x1.onClick.AddListener(() => OnClickSpawnBoat(Boat1x1Prefab));
        buttonSpawnBoat1x2.onClick.AddListener(() => OnClickSpawnBoat(Boat1x2Prefab));
        buttonSpawnBoat1x3.onClick.AddListener(() => OnClickSpawnBoat(Boat1x3Prefab));
        buttonSpawnBoat1x4.onClick.AddListener(() => OnClickSpawnBoat(Boat1x4Prefab));
        testButton.onClick.AddListener(BoatDraggerManager.Instance.SetLoopActive);

        GameManager.Instance.OnNetworkSpawned += GameManager_OnNetworkSpawn;
        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
        GameManager.Instance.OnRematch += GameManager_OnRematch;


        boat2x2Text.text = Boat2x2Prefab.GetComponent<IBoat>().placementLimit.ToString();
        boat1x1Text.text = Boat1x1Prefab.GetComponent<IBoat>().placementLimit.ToString();
        boat1x2Text.text = Boat1x2Prefab.GetComponent<IBoat>().placementLimit.ToString();
        boat1x3Text.text = Boat1x3Prefab.GetComponent<IBoat>().placementLimit.ToString();
        boat1x4Text.text = Boat1x4Prefab.GetComponent<IBoat>().placementLimit.ToString();

        GetSpritesForButtonsAsync();

        Hide();
    }

    private void GameManager_OnNetworkSpawn(object sender, GameManager.PlayerTypeEventArgs e) {
        Show();
    }

    private void GameManager_OnGameStart(object sender, EventArgs e) {
        Hide();
    }

    private void GameManager_OnRematch(object sender, EventArgs e) {
        Show();
    }

    private void OnClickSpawnBoat(GameObject boatPrefab) {
        SpawnBoatManager.Instance.SpawnBoat(boatPrefab);
    }

    public void UpdateNumberOfBoatsToBePlaced() {
        var boat2x2 = Boat2x2Prefab.GetComponent<IBoat>();
        boat2x2Text.text = boat2x2.placementLimit - GetHowManyBoatsWerePlaced(boat2x2.name) + "";

        var boat1x1 = Boat1x1Prefab.GetComponent<IBoat>();
        boat1x1Text.text = boat1x1.placementLimit - GetHowManyBoatsWerePlaced(boat1x1.name) + "";

        var boat1x2 = Boat1x2Prefab.GetComponent<IBoat>();
        boat1x2Text.text = boat1x2.placementLimit - GetHowManyBoatsWerePlaced(boat1x2.name) + "";

        var boat1x3 = Boat1x3Prefab.GetComponent<IBoat>();
        boat1x3Text.text = boat1x3.placementLimit - GetHowManyBoatsWerePlaced(boat1x3.name) + "";

        var boat1x4 = Boat1x4Prefab.GetComponent<IBoat>();
        boat1x4Text.text = boat1x4.placementLimit - GetHowManyBoatsWerePlaced(boat1x4.name) + "";
    }

    public int GetHowManyBoatsWerePlaced(string boatName) {
        var localBoatsArray = GameManager.Instance.localPlayerBoats;
        var boatCount = localBoatsArray.FindAll(b => b.name.Split('(')[0].Trim() == boatName).Count;
        return boatCount;
    }

    public async void GetSpritesForButtonsAsync() {
        await GetSpriteForBoatAsync("1x1", GameManager.Instance.localThemeSelected, buttonSpawnBoat1x1, 3.7f, 1.7f);
        await GetSpriteForBoatAsync("1x2", GameManager.Instance.localThemeSelected, buttonSpawnBoat1x2, 3.7f, 1.7f);
        await GetSpriteForBoatAsync("1x3", GameManager.Instance.localThemeSelected, buttonSpawnBoat1x3, 3.7f, 1.7f);
        await GetSpriteForBoatAsync("1x4", GameManager.Instance.localThemeSelected, buttonSpawnBoat1x4, 3.7f, 1.7f);
        await GetSpriteForBoatAsync("2x2", GameManager.Instance.localThemeSelected, buttonSpawnBoat2x2, 3.7f, 1.7f);
    }

    public async Task GetSpriteForBoatAsync(string size, string themeName, Button button, float xSize, float zSize) {
        var api = new Api();
        Sprite sprite = await api.GetSpriteForShipAsync(size, themeName ?? "Piscina");
        button.GetComponent<Image>().sprite = sprite;
        button.GetComponent<Image>().preserveAspect = true;
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
