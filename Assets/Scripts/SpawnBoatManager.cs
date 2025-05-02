using UnityEngine;
using UnityEngine.UI;

public class SpawnBoatManager : MonoBehaviour {

    [SerializeField] private GameObject circularBoatPrefab;
    [SerializeField] private GameObject lineBoatPrefab;
    [SerializeField] private Button buttonSpawnCircularBoat;
    [SerializeField] private Button buttonSpawnLineBoat;
    [SerializeField] private Button testButton;

    private bool isDragging;
    private IBoat currentDraggedObject;
    private IBoat boatInSpawn;

    private Vector3 initialBoatPosition;

    private bool loopActive = false;

    private void Awake() {
        initialBoatPosition = new Vector3(8.62f, 0.6f, -6.3f);
    }

    private void Start() {
        buttonSpawnCircularBoat.onClick.AddListener(() => SpawnBoat(circularBoatPrefab));
        buttonSpawnLineBoat.onClick.AddListener(() => SpawnBoat(lineBoatPrefab));
        testButton.onClick.AddListener(TestButtonClicked);
    }

    private void Update() {
        RaycastHit boatHit;
        IBoat boat = InputManager.Instance.GetBoatBeaingIntercepted(out boatHit);

        RaycastHit gridHit;
        Grid grid = InputManager.Instance.GetGridBeaingIntercepted(out gridHit);

        RaycastHit defaultHit;
        InputManager.Instance.GetGameObjectBeaingIntercepted(out defaultHit);

        if (loopActive) {
            Debug.Log("Loop is active! " + GameManager.Instance.boatsPlayer1.Count);
        }

        //Is valid Dragging
        if (Input.GetMouseButtonDown(0)) {
            if (boat != null) {
                currentDraggedObject = boat;

                //Check if is the start of dragging
                if (!isDragging && grid != null) {
                    currentDraggedObject.RemoveBoatFromGrid();
                }

                isDragging = true;
            }
        }

        //Move Boat
        if (isDragging && currentDraggedObject != null) {
            //Check if is clipping Grid
            if (grid != null) {
                currentDraggedObject.positonOnGrid = grid.WorldToCell(gridHit.point);
                Vector3 worldPosition = grid.GetCellCenterWorld(currentDraggedObject.positonOnGrid);
                currentDraggedObject.gameObject.transform.position = new Vector3(worldPosition.x, 0.6f, worldPosition.z);
            } else {
                //If not clipping, set free dragging
                currentDraggedObject.gameObject.transform.position = new Vector3(defaultHit.point.x, 0.6f, defaultHit.point.z);
                currentDraggedObject.positonOnGrid = new Vector3Int(-1, 0, -1);
            }

            //Check if letting go of the mouse button
            if (Input.GetMouseButtonUp(0)) {
                //If position is invalid, return to initial position
                if (!GameManager.Instance.IsBoatPositionValid(currentDraggedObject)) {
                    currentDraggedObject.gameObject.transform.position = initialBoatPosition;
                    currentDraggedObject.ShowInvalidPosition();
                    if(boatInSpawn != null && boatInSpawn != currentDraggedObject) {
                        boatInSpawn.DestroyBoat();
                    }
                    boatInSpawn = currentDraggedObject;
                } else {
                    //If position is valid, set the boat to the grid, hide invalid position notification and set the boat to initial position
                    currentDraggedObject.HideInvalidPosition();
                    currentDraggedObject.positonOnGrid = currentDraggedObject.positonOnGrid;
                    GameManager.Instance.AddBoatToGrid(currentDraggedObject);
                    boatInSpawn = null;
                }

                isDragging = false;
                currentDraggedObject = null;
            }

            //Right click to rotate the boat
            if (Input.GetMouseButtonDown(1)) {
                int currentRotarion = (int)currentDraggedObject.gameObject.transform.rotation.eulerAngles.y;
                int rotation = currentRotarion == 270 ? 0 : currentRotarion + 90;
                currentDraggedObject.gameObject.transform.rotation = Quaternion.Euler(
                    0,
                    rotation,
                    0
                );
                currentDraggedObject.rotation = rotation;
            }
        }
    }

    private void SpawnBoat(GameObject boatPrefab) {
        if(boatInSpawn != null) {
            boatInSpawn.DestroyBoat();
        }

        GameObject boat = Instantiate(boatPrefab, initialBoatPosition, Quaternion.identity);
        var boatComponent = boat.GetComponent<IBoat>();

        if (GameManager.Instance.boatPointsPlayer1 + boatComponent.points > GameManager.MAX_BOAT_POINTS) {
            Debug.Log("Max boat points reached!");
            Destroy(boat);
            return;
        }

        boatInSpawn = boatComponent;
        GameManager.Instance.boatsPlayer1.Add(boat.GetComponent<IBoat>());
        GameManager.Instance.boatPointsPlayer1 += boatInSpawn.points;
    }

    private void TestButtonClicked() {
        loopActive = !loopActive;
    }
}
