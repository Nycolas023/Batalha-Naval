using UnityEngine;

public class BoatDraggerManager : MonoBehaviour {
    private bool isDragging;
    private IBoat currentDraggedObject;

    private bool loopActive = false;

    public static BoatDraggerManager Instance { get; private set; }

    private IBoat boatInSpawn;

    private bool isDraggingBoatActive = true;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one BoatDraggerManager instance!");
        }
        Instance = this;
    }

    private void Update() {
        RaycastHit boatHit;
        IBoat boat = InputManager.Instance.GetBoatBeaingIntercepted(out boatHit);

        RaycastHit gridHit;
        Grid grid = InputManager.Instance.GetGridBeaingIntercepted(out gridHit);

        RaycastHit defaultHit;
        InputManager.Instance.GetGameObjectBeaingIntercepted(out defaultHit);

        Vector3 initialBoatPosition = SpawnBoatManager.Instance.initialBoatPositionPlayer1;

        if (loopActive) {
            Debug.Log("Loop is active! " + GameManager.Instance.localPlayerBoats.Count);
        }

        if (!isDraggingBoatActive) {
            if (Input.GetMouseButtonDown(0) && boat != null)
                Debug.Log("Boat dragging is not active!");
            return;
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
                    if (boatInSpawn != null && boatInSpawn != currentDraggedObject) {
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

                currentDraggedObject.componetsGrid = new Utils().RotateMatrix(currentDraggedObject.componetsGrid);
                var temp = currentDraggedObject.xCenter;
                currentDraggedObject.xCenter = currentDraggedObject.zCenter;
                currentDraggedObject.zCenter = temp;
            }
        }
    }

    public void SetLoopActive() {
        loopActive = !loopActive;
    }

    public void SetBoatInSpawn(IBoat boat) {
        if (boatInSpawn != null) {
            boatInSpawn.DestroyBoat();
        }
        boatInSpawn = boat;
    }

    public void SetDraggingBoatActive(bool isActive) {
        isDraggingBoatActive = isActive;
    }
}
