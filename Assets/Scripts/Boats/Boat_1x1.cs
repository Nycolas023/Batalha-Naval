using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class Boat_1x1 : MonoBehaviour, IBoat {
    [SerializeField] public int xlength { get; set; }
    [SerializeField] public int zlength { get; set; }
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
        xlength = 1;
        zlength = 1;

        xCenter = 0;
        zCenter = 0;

        rotation = 0f;

        componetsGrid = new int[,] {
            { 1 }
        };

        HideInvalidPosition();
    }

    private void Start() {
        _ = GetImageSurface();
    }

    private async Task GetImageSurface() {
        var api = new Api();
        Sprite sprite = await api.GetSpriteForShipAsync("1x1", GameManager.Instance.localThemeSelected ?? "Piscina");
        imageSurface.GetComponent<SpriteRenderer>().sprite = sprite;
        imageSurface.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
        imageSurface.GetComponent<SpriteRenderer>().size = new Vector2(3.7f, 1.7f);
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
