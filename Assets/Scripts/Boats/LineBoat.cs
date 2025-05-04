using UnityEngine;

public class LineBoat : MonoBehaviour, IBoat {
    [SerializeField] public int xlength { get; set; }
    [SerializeField] public int zlength { get; set; }
    public int[,] componetsGrid { get; set; }
    public int xCenter { get; set; }
    public int zCenter { get; set; }
    GameObject IBoat.gameObject { get => gameObject; }
    public Vector3Int positonOnGrid { get; set; }
    public float rotation { get; set; }
    public int points { get; set; } = 1;

    [SerializeField] private GameObject invalidPositionIndicator;

    private void Awake() {
        xlength = 3;
        zlength = 1;

        xCenter = 0;
        zCenter = 1;

        rotation = 0f;

        componetsGrid = new int[,] {
            { 1 },
            { 1 },
            { 1 }
        };

        HideInvalidPosition();

        GameManager.Instance.boatPointsPlayer1 += points;
    }

    public void ShowInvalidPosition() {
        invalidPositionIndicator.SetActive(true);
    }

    public void HideInvalidPosition() {
        invalidPositionIndicator.SetActive(false);
    }

    public void RemoveBoatFromGrid() {
        GameManager.Instance.RemoveBoatFromGrid(this);
    }

    public void DestroyBoat() {
        GameManager.Instance.boatPointsPlayer1 -= points;
        GameManager.Instance.localPlayerBoats.Remove(this);
        Destroy(gameObject);
    }
}
