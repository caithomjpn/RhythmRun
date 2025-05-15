using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int shootEveryNthBeat = 2;
    [SerializeField] private float bulletSpeed = 10f;

    private Transform player;
    private int beatCounter = 0;
    private bool shootingDisabledPermanently = false;
    public AudioSource shootSound;

    void Start()
    {
        // Initial player lookup
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Listen to metronome beat events
        Metronome.OnBeat += HandleBeat;
    }

    void Update()
    {
        // Optional: auto-recover if player reference gets lost mid-game
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    void HandleBeat()
    {
        // Detailed null checks to identify what’s missing
        if (player == null)
        {
            Debug.LogWarning("Player reference is null.");
            return;
        }
        if (firePoint == null)
        {
            Debug.LogWarning("FirePoint is null.");
            return;
        }
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet prefab is null.");
            return;
        }

        // Compare Z positions, rounded to avoid floating point issues
        float playerZ = Mathf.Round(player.position.z * 10f) / 10f;
        float enemyZ = Mathf.Round(transform.position.z * 10f) / 10f;

        Debug.Log($"[BEAT {beatCounter}] PlayerZ={playerZ} | EnemyZ={enemyZ} | Stopped={shootingDisabledPermanently}");

        // Optional permanent stop condition
        if (!shootingDisabledPermanently && Mathf.Approximately(playerZ, enemyZ))
        {
            shootingDisabledPermanently = true;
            Debug.Log("Enemy permanently stopped shooting due to Z-position match.");
            return;
        }

        if (shootingDisabledPermanently) return;

        beatCounter++;

        if (beatCounter % shootEveryNthBeat == 0)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = firePoint.forward * bulletSpeed;

        if (shootSound != null)
            shootSound.PlayOneShot(shootSound.clip);  // <--- Fix: allows overlap
    }


    private void OnDestroy()
    {
        Metronome.OnBeat -= HandleBeat;
    }
}
