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
        Destroy(gameObject);
        GameManager.Instance.boatsPlayer1.Remove(this);
    }
}
