using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnBoatManagerUI : MonoBehaviour {
    [SerializeField] private GameObject circularBoatPrefab;
    [SerializeField] private Button buttonSpawnCircularBoat;
    [SerializeField] private TextMeshProUGUI circularBoatText;

    [SerializeField] private GameObject lineBoatPrefab;
    [SerializeField] private Button buttonSpawnLineBoat;
    [SerializeField] private TextMeshProUGUI lineBoatText;

    [SerializeField] private GameObject lineBoat4Prefab;
    [SerializeField] private Button buttonSpawnLineBoat4;
    [SerializeField] private TextMeshProUGUI lineBoat4Text;

    [SerializeField] private Button testButton;

    public static SpawnBoatManagerUI Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one SpawnBoatManagerUI instance!");
        }
        Instance = this;
    }

    private void Start() {
        buttonSpawnCircularBoat.onClick.AddListener(() => OnClickSpawnBoat(circularBoatPrefab));
        buttonSpawnLineBoat.onClick.AddListener(() => OnClickSpawnBoat(lineBoatPrefab));
        buttonSpawnLineBoat4.onClick.AddListener(() => OnClickSpawnBoat(lineBoat4Prefab));
        testButton.onClick.AddListener(BoatDraggerManager.Instance.SetLoopActive);

        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
        GameManager.Instance.OnRematch += GameManager_OnRematch;

        circularBoatText.text = circularBoatPrefab.GetComponent<IBoat>().placementLimit.ToString();
        lineBoatText.text = lineBoatPrefab.GetComponent<IBoat>().placementLimit.ToString();
        lineBoat4Text.text = lineBoat4Prefab.GetComponent<IBoat>().placementLimit.ToString();
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
        var circularBoat = circularBoatPrefab.GetComponent<IBoat>();
        circularBoatText.text = circularBoat.placementLimit - GetHowManyBoatsWerePlaced(circularBoat.name) + "";

        var lineBoat = lineBoatPrefab.GetComponent<IBoat>();
        lineBoatText.text = lineBoat.placementLimit - GetHowManyBoatsWerePlaced(lineBoat.name) + "";

        var lineBoat4 = lineBoat4Prefab.GetComponent<IBoat>();
        lineBoat4Text.text = lineBoat4.placementLimit - GetHowManyBoatsWerePlaced(lineBoat4.name) + "";
    }

    public int GetHowManyBoatsWerePlaced(string boatName) {
        var localBoatsArray = GameManager.Instance.localPlayerBoats;
        var boatCount = localBoatsArray.FindAll(b => b.name.Split('(')[0].Trim() == boatName).Count;
        return boatCount;
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
