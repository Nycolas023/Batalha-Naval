using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnBoatManager : MonoBehaviour {

    [SerializeField] private GameObject boatPrefab;
    [SerializeField] private Button buttonSpawnBoat;
    [SerializeField] private Button testButton;

    private Boat currentBoat;
    private bool isDragging;
    private GameObject currentDraggedObject;

    private bool loopActive = false;

    private void Start() {
        buttonSpawnBoat.onClick.AddListener(SpawnBoat);
        testButton.onClick.AddListener(TestButtonClicked);
    }

    private void Update() {
        RaycastHit hit;
        Boat boat = InputManager.Instance.GetBoatBeaingIntercepted(out hit);

        RaycastHit gridHit;
        Grid grid = InputManager.Instance.GetGridBeaingIntercepted(out gridHit);

        RaycastHit defaultHit;
        InputManager.Instance.GetGameObjectBeaingIntercepted(out defaultHit);

        if (loopActive) {
            Debug.DrawRay(hit.point, Vector3.up * 10, Color.red);
            Debug.Log("Loop is active! " + defaultHit.point.x + " " + 0.25f + " " + defaultHit.point.z);
        }

        if (currentBoat != null && currentBoat != null && boat != null) {
            if (Input.GetMouseButtonDown(0)) {
                isDragging = true;
                currentDraggedObject = boat.gameObject;
            }
        }

        if (isDragging && currentDraggedObject != null) {
            if (grid != null) {
                Vector3Int gridPosition = grid.WorldToCell(gridHit.point);
                Vector3 worldPosition = grid.GetCellCenterWorld(gridPosition);
                if(worldPosition.x == 0f) {
                    Debug.Log("Grid position is zero, not moving the object.");
                }
                currentDraggedObject.transform.position = new Vector3(worldPosition.x, 0.25f, worldPosition.z);
            } else {
                currentDraggedObject.transform.position = new Vector3(defaultHit.point.x, 0.25f, defaultHit.point.z);
            }

            if (Input.GetMouseButtonUp(0)) {
                isDragging = false;
                currentDraggedObject = null;
            }

            if (Input.GetMouseButtonDown(1)) {
                int rotation = currentDraggedObject.transform.rotation.y == 0 ? 90 : 0;
                currentDraggedObject.transform.rotation = Quaternion.Euler(
                    0, 
                    rotation, 
                    0
                );
            }
        }
    }

    private void SpawnBoat() {
        Vector3 origin = new Vector3(17.5f, 0, -9);
        GameObject boat = Instantiate(boatPrefab, origin, Quaternion.identity);

        currentBoat = boat.GetComponent<Boat>();
    }

    private void TestButtonClicked() {
        Debug.Log("Test button clicked!");
        // Add your test logic here
        loopActive = !loopActive; // Toggle the loopActive state
    }
}
