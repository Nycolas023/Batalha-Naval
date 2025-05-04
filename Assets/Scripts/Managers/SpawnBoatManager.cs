using UnityEngine;
using UnityEngine.UI;

public class SpawnBoatManager : MonoBehaviour {

    [SerializeField] private GameObject circularBoatPrefab;
    [SerializeField] private GameObject lineBoatPrefab;
    [SerializeField] private Button buttonSpawnCircularBoat;
    [SerializeField] private Button buttonSpawnLineBoat;
    [SerializeField] private Button testButton;

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
        buttonSpawnCircularBoat.onClick.AddListener(() => SpawnBoat(circularBoatPrefab));
        buttonSpawnLineBoat.onClick.AddListener(() => SpawnBoat(lineBoatPrefab));
        testButton.onClick.AddListener(BoatDraggerManager.Instance.SetLoopActive);

        initialBoatPositionPlayer1 = new Vector3(8.62f, 0.6f, -6.3f);
        initialBoatPositionPlayer2 = new Vector3(-8.62f, 0.6f, -6.3f);
    }

    private void SpawnBoat(GameObject boatPrefab) {
        Vector3 initialPosition = GameManager.Instance.GetLocalPlayerType() ==
            GameManager.PlayerType.Player1 ? initialBoatPositionPlayer1 : initialBoatPositionPlayer2;

        GameObject boat = Instantiate(boatPrefab, initialPosition, Quaternion.identity);
        var boatComponent = boat.GetComponent<IBoat>();

        var boatPoints = GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Player1 ?
            GameManager.Instance.boatPointsPlayer1 : GameManager.Instance.boatPointsPlayer2;

        if (boatPoints + boatComponent.points > GameManager.MAX_BOAT_POINTS) {
            Debug.Log("Max boat points reached!");
            Destroy(boat);
            return;
        }

        if (GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Player1) {
            GameManager.Instance.localPlayerBoats.Add(boat.GetComponent<IBoat>());
        } else {
            GameManager.Instance.localPlayerBoats.Add(boat.GetComponent<IBoat>());
        }

        BoatDraggerManager.Instance.SetBoatInSpawn(boatComponent);
    }
}
