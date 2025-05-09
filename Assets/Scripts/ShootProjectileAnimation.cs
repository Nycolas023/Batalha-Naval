using UnityEngine;

public class ShootProjectileAnimation : MonoBehaviour
{
    public static ShootProjectileAnimation Instance { get; private set; }

    [Header("Configurações do projétil")]
    [SerializeField] private GameObject projectilePrefab;
    public Transform shooterOriginPlayer1;
    public Transform shooterOriginPlayer2;

    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private float arcHeight = 5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void SpawnProjectileAnimation(Vector3 targetPosition)
    {
        Transform origin = GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Player1
        ? shooterOriginPlayer1
        : shooterOriginPlayer2;

        GameObject projectileGO = Instantiate(projectilePrefab, origin.position, Quaternion.identity);
        ProjectileArc projectile = projectileGO.AddComponent<ProjectileArc>();
        projectile.Initialize(origin.position, targetPosition, projectileSpeed, arcHeight);

    }
}
