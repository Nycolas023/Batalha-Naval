using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shooterOrigin;
    public float projectileSpeed = 10f;
    public float arcHeight = 5f;

    private bool hasFired = false; // ✅ NOVA VARIÁVEL



    private void Update()
    {
        if (!GameManager.Instance.ArePlayersReady()) return;

        // Bloqueia se já atirou no turno
        if (hasFired) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                LaunchProjectile(hit.point);
                hasFired = true;
            }
        }
    }


    void LaunchProjectile(Vector3 targetPosition)
    {
        GameObject projectileGO = Instantiate(projectilePrefab, shooterOrigin.position, Quaternion.identity);
        ProjectileArc projectile = projectileGO.AddComponent<ProjectileArc>();
        projectile.Initialize(shooterOrigin.position, targetPosition, projectileSpeed, arcHeight);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnChangePlayablePlayerType += GameManager_OnChangePlayablePlayerType;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnChangePlayablePlayerType -= GameManager_OnChangePlayablePlayerType;
    }

    private void GameManager_OnChangePlayablePlayerType(object sender, GameManager.PlayerTypeEventArgs e)
    {
        hasFired = false;
    }


}
