using UnityEngine;

public class CircularBoat : MonoBehaviour, IBoat {
    [SerializeField] public int xlength { get; set; }
    [SerializeField] public int zlength { get; set; }
    public int[,] componetsGrid { get; set; }
    public int xCenter { get; set; }
    public int zCenter { get; set; }
    GameObject IBoat.gameObject { get => gameObject; }
    public Vector3Int positonOnGrid { get; set; }

    [SerializeField] private GameObject invalidPositionIndicator;

    private void Awake() {
        xlength = 3;
        zlength = 3;

        xCenter = 1;
        zCenter = 1;

        componetsGrid = new int[,] {
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 1, 1 }
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
        GamaManager.Instance.RemoveBoatFromGrid(this);
    }
}
