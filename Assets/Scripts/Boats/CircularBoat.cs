using UnityEngine;

public class CircularBoat : MonoBehaviour, IBoat {
    public int[,] componetsGrid { get; set; }
    public int xCenter { get; set; }
    public int zCenter { get; set; }
    GameObject IBoat.gameObject { get => gameObject; }
    public Vector3Int positonOnGrid { get; set; }
    public float rotation { get; set; }
    public int placementLimit { get; set; } = 1;

    [SerializeField] private GameObject invalidPositionIndicator;

    private void Awake() {
        xCenter = 1;
        zCenter = 1;

        componetsGrid = new int[,] {
            { 1, 1 },
            { 1, 1 },
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
        GameManager.Instance.localPlayerBoats.Remove(this);
        Destroy(gameObject);
    }
}
