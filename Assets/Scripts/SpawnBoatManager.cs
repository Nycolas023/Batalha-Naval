using UnityEngine;
using UnityEngine.UI;

public class SpawnBoatManager : MonoBehaviour {

    [SerializeField] private GameObject boatPrefab;
    [SerializeField] private Button buttonSpawnBoat;
    [SerializeField] private Button testButton;

    private bool isDragging;
    private IBoat currentDraggedObject;

    private Vector3 initialBoatPosition;

    private bool loopActive = false;

    private void Awake() {
        initialBoatPosition = new Vector3(17.5f, 0, -9);
    }

    private void Start() {
        buttonSpawnBoat.onClick.AddListener(SpawnBoat);
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
            Debug.Log("Loop is active! " + boat?.positonOnGrid.x + " " + boat?.positonOnGrid.z);
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
                currentDraggedObject.gameObject.transform.position = new Vector3(worldPosition.x, 0.25f, worldPosition.z);
            } else {
                //If not clipping, set free dragging
                currentDraggedObject.gameObject.transform.position = new Vector3(defaultHit.point.x, 0.25f, defaultHit.point.z);
                currentDraggedObject.positonOnGrid = new Vector3Int(-1, 0, -1);
            }

            //Check if letting go of the mouse button
            if (Input.GetMouseButtonUp(0)) {
                //If position is invalid, return to initial position
                if (!GamaManager.Instance.IsBoatPositionValid(currentDraggedObject)) {
                    currentDraggedObject.gameObject.transform.position = initialBoatPosition;
                    currentDraggedObject.ShowInvalidPosition();
                } else {
                    //If position is valid, set the boat to the grid, hide invalid position notification and set the boat to initial position
                    currentDraggedObject.HideInvalidPosition();
                    currentDraggedObject.positonOnGrid = currentDraggedObject.positonOnGrid;
                    GamaManager.Instance.AddBoatToGrid(currentDraggedObject);
                }

                isDragging = false;
                currentDraggedObject = null;
            }

            //Right click to rotate the boat
            if (Input.GetMouseButtonDown(1)) {
                int rotation = currentDraggedObject.gameObject.transform.rotation.y == 0 ? 90 : 0;
                currentDraggedObject.gameObject.transform.rotation = Quaternion.Euler(
                    0,
                    rotation,
                    0
                );
            }
        }
    }

    private void SpawnBoat() {
        GameObject boat = Instantiate(boatPrefab, initialBoatPosition, Quaternion.identity);
    }

    private void TestButtonClicked() {
        loopActive = !loopActive;
    }
}
