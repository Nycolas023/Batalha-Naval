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

    [SerializeField] private Button testButton;
    [SerializeField] TextMeshProUGUI playerBoatsPlacedText;

    public static SpawnBoatManagerUI Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one SpawnBoatManagerUI instance!");
        }
        Instance = this;
    }

    void Update() {
        playerBoatsPlacedText.text = GameManager.Instance.localPlayerBoats.Count.ToString() + "/" + GameManager.MAX_BOATS_SPAWNED.ToString();
    }

    private void Start() {
        buttonSpawnCircularBoat.onClick.AddListener(() => OnClickSpawnBoat(circularBoatPrefab));
        buttonSpawnLineBoat.onClick.AddListener(() => OnClickSpawnBoat(lineBoatPrefab));
        testButton.onClick.AddListener(BoatDraggerManager.Instance.SetLoopActive);

        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
        GameManager.Instance.OnRematch += GameManager_OnRematch;

        circularBoatText.text = circularBoatPrefab.GetComponent<IBoat>().placementLimit.ToString();
        lineBoatText.text = lineBoatPrefab.GetComponent<IBoat>().placementLimit.ToString();
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

    public void UpdateNumberOfBoatsToBePlaced(IBoat boat) {
        string boatName = boat.name.Split('(')[0].Trim(); // Remove "(Clone)" from the name
        switch (boatName) {
            case "CircularBoat":
                circularBoatText.text = (int.Parse(circularBoatText.text) - 1).ToString();
                break;
            case "LineBoat":
                lineBoatText.text = (int.Parse(lineBoatText.text) - 1).ToString();
                break;
            default:
                Debug.LogError("Unknown boat type: " + boat.name);
                break;
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    
    private void Hide() {
        gameObject.SetActive(false);
    }
}
