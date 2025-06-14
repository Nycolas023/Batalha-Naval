using System;
using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{

    public enum AttackMode
    {
        Single,
        Area2x2,
        X
    }


    public static InputManager Instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private LayerMask defaultLayerMask;
    [SerializeField] private LayerMask gridLayerMask;
    [SerializeField] private LayerMask boatLayerMask;

    private Vector3 lastMousePosition;

    [SerializeField] private Material previewMaterial;
    public AttackMode currentAttackMode;

    private List<GamePosition> lastHoveredCells = new();
    private List<Material> lastHoveredOriginalMaterials = new();

    private bool isGameStarted = false;



    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one InputManager instance!");
        }
        Instance = this;
        GameManager.Instance.OnGameStart += OnGameStart;
    }

    private void Start()
    {
        //currentAttackMode = AttackMode.X;
    }


    private void OnGameStart(object sender, EventArgs e)
    {
        isGameStarted = true;
    }

    private void Update() {
        if (!isGameStarted) return;

        Ray ray = GetRaycastHit();
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, defaultLayerMask)) {
            if (hit.collider.CompareTag("GridCell")) {
                GamePosition baseCell = hit.collider.GetComponent<GamePosition>();
                if (baseCell == null) return;

                string gridName = baseCell.transform.parent.name;
                var currentPlayer = GameManager.Instance.GetCurrentPlayablePlayerType();

                Debug.Log($"👀 Mouse sobre a célula do grid: {gridName} e player {currentPlayer}");


                Vector2Int basePos = baseCell.GetGridPosition();
                List<GamePosition> targetCells = currentAttackMode switch {
                    AttackMode.Single => GetCellsSingle(basePos),
                    AttackMode.Area2x2 => GetCellsInArea2x2(basePos),
                    AttackMode.X => GetCellsInXShape(basePos),
                    _ => new List<GamePosition>()
                };

                // ❗ Remover preview das anteriores
                //ClearLastPreview();

                // ❗ Aplicar preview em todas as novas
                foreach (var cell in targetCells) {
                    Renderer renderer = cell.GetComponent<Renderer>();
                    if (renderer != null) {
                        lastHoveredOriginalMaterials.Add(renderer.sharedMaterial);
                        renderer.sharedMaterial = previewMaterial;
                        //Debug.Log($"✅ Aplicando preview em {cell.name} na posição {cell.transform.position}");
                    }
                }

                lastHoveredCells = targetCells;
            }
        }
        //GameManager.Instance.UpdateGridCollidersPerTurn();
}


    private List<GamePosition> GetCellsSingle(Vector2Int origin)
    {
        var result = new List<GamePosition>();
        if (IsWithinGridBounds(origin.x, origin.y))
        {
            var cell = GameManager.Instance.GetLocalGridArray()[origin.x, origin.y];
            if (cell != null) result.Add(cell);
        }
        return result;
    }

    private List<GamePosition> GetCellsInArea2x2(Vector2Int origin)
    {
        List<GamePosition> result = new();
        for (int dx = 0; dx < 2; dx++)
        {
            for (int dz = 0; dz < 2; dz++)
            {
                int x = origin.x + dx;
                int z = origin.y + dz;
                if (IsWithinGridBounds(x, z))
                {
                    var cell = GameManager.Instance.GetLocalGridArray()[x, z];
                    if (cell != null) result.Add(cell);
                }
            }
        }
        return result;
    }

    private List<GamePosition> GetCellsInXShape(Vector2Int center)
    {
        List<GamePosition> result = new();

        Vector2Int[] offsets = {
    new Vector2Int(0, 0),
    new Vector2Int(-1, -1),
    new Vector2Int(1, -1),
    new Vector2Int(-1, 1),
    new Vector2Int(1, 1)
};

        foreach (var offset in offsets)
        {
            int x = center.x + offset.x;
            int z = center.y + offset.y;
            if (IsWithinGridBounds(x, z))
            {
                var cell = GameManager.Instance.GetLocalGridArray()[x, z];
                if (cell != null) result.Add(cell);
            }
        }

        return result;
    }


    private bool IsWithinGridBounds(int x, int z)
    {
        return x >= 0 && x < GameManager.GRID_WIDTH &&
               z >= 0 && z < GameManager.GRID_HEIGHT;
    }

    private bool AreSameCells(List<GamePosition> a, List<GamePosition> b)
    {
        if (a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }


    public Ray GetRaycastHit()
    {
        return mainCamera.ScreenPointToRay(Input.mousePosition);
    }

    public Grid GetGridBeaingIntercepted(out RaycastHit hit)
    {
        Ray ray = GetRaycastHit();
        bool didRaycastHitSomething = Physics.Raycast(ray, out hit, Mathf.Infinity, gridLayerMask);
        if (didRaycastHitSomething)
        {
            return hit.collider.GetComponent<Grid>();
        }
        return null;
    }

    public IBoat GetBoatBeaingIntercepted(out RaycastHit hit)
    {
        Ray ray = GetRaycastHit();
        bool didRaycastHitSomething = Physics.Raycast(ray, out hit, Mathf.Infinity, boatLayerMask);
        if (didRaycastHitSomething)
        {
            return hit.collider.GetComponent(typeof(IBoat)) as IBoat;
        }
        return null;
    }

    public GameObject GetGameObjectBeaingIntercepted(out RaycastHit hit)
    {
        Ray ray = GetRaycastHit();
        bool didRaycastHitSomething = Physics.Raycast(ray, out hit, Mathf.Infinity, defaultLayerMask);
        if (didRaycastHitSomething)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public bool IsRayHittingBoat(out RaycastHit hit)
    {
        Ray ray = GetRaycastHit();
        return Physics.Raycast(ray, out hit, Mathf.Infinity, boatLayerMask);
    }
}
