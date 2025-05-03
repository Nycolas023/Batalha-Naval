using UnityEngine;
using UnityEngine.UI;

public class SpawnBoatManager : MonoBehaviour {

    [SerializeField] private GameObject circularBoatPrefab;
    [SerializeField] private GameObject lineBoatPrefab;
    [SerializeField] private Button buttonSpawnCircularBoat;
    [SerializeField] private Button buttonSpawnLineBoat;
    [SerializeField] private Button testButton;

    public static SpawnBoatManager Instance { get; private set; }

    

    public Vector3 initialBoatPosition = new Vector3(8.62f, 0.6f, -6.3f);

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one SpawnBoatManager instance!");
        }
        Instance = this;
    }

    private void Start() {
        buttonSpawnCircularBoat.onClick.AddListener(() => SpawnBoat(circularBoatPrefab));
        buttonSpawnLineBoat.onClick.AddListener(() => SpawnBoat(lineBoatPrefab));
        testButton.onClick.AddListener(BoatDraggerManager.Instance.SetLoopActive);
    }

    private void SpawnBoat(GameObject boatPrefab) {
        GameObject boat = Instantiate(boatPrefab, initialBoatPosition, Quaternion.identity);
        var boatComponent = boat.GetComponent<IBoat>();

        if (GameManager.Instance.boatPointsPlayer1 + boatComponent.points > GameManager.MAX_BOAT_POINTS) {
            Debug.Log("Max boat points reached!");
            Destroy(boat);
            return;
        }

        BoatDraggerManager.Instance.SetBoatInSpawn(boatComponent);
        GameManager.Instance.boatsPlayer1.Add(boat.GetComponent<IBoat>());
    }
}
