using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ShootProjectileAnimation : MonoBehaviour {
    public static ShootProjectileAnimation Instance { get; private set; }

    [Header("Configurações do projétil")]
    [SerializeField] private GameObject projectilePrefab;
    public Transform shooterOriginPlayer1;
    public Transform shooterOriginPlayer2;

    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private float arcHeight = 5f;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void SpawnProjectileAnimation(Vector3 targetPosition, GameManager.PlayerType playerType) {
        Vector3 gridPosition = playerType == GameManager.PlayerType.Player1 
            ? GameManager.Instance.GetGridPlayer1Position()
            : GameManager.Instance.GetGridPlayer2Position();
        
        Vector3 startPosition = GetProjectileStartPosition(gridPosition, playerType);

        GameObject projectileGO = Instantiate(projectilePrefab, startPosition, Quaternion.Euler(90, 0, 0));
        ProjectileArc projectile = projectileGO.AddComponent<ProjectileArc>();
        projectile.Initialize(startPosition, targetPosition, projectileSpeed, arcHeight);
    }

    private Vector3 GetProjectileStartPosition(Vector3 gridPosition, GameManager.PlayerType playerType) {
        Vector3 startPosition = gridPosition;
        if (playerType == GameManager.PlayerType.Player1)
            startPosition.x += GameManager.CELL_SIZE * GameManager.GRID_WIDTH;

        startPosition.z -= 1f;

        return startPosition;
    }
}
