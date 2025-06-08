using System.Threading.Tasks;
using UnityEngine;

public class Boat_2x2 : MonoBehaviour, IBoat {
    public int[,] componetsGrid { get; set; }
    public int xCenter { get; set; }
    public int zCenter { get; set; }
    GameObject IBoat.gameObject { get => gameObject; }
    public Vector3Int positonOnGrid { get; set; }
    public float rotation { get; set; }
    public int placementLimit { get; set; } = 1;

    [SerializeField] private GameObject invalidPositionIndicator;
    [SerializeField] private GameObject imageSurface;

    private void Awake() {
        xCenter = 1;
        zCenter = 1;

        componetsGrid = new int[,] {
            { 1, 1 },
            { 1, 1 },
        };

        HideInvalidPosition();
    }

    private void Start() {
        _ = GetImageSurface();
    }

    private async Task GetImageSurface() {
        var api = new Api();
        Sprite sprite = await api.GetSpriteForShipAsync("2x2", GameManager.Instance.localThemeSelected ?? "Piscina");
        imageSurface.GetComponent<SpriteRenderer>().sprite = sprite;
        imageSurface.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
        imageSurface.GetComponent<SpriteRenderer>().size = new Vector2(4.5f, 4.8f);
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
