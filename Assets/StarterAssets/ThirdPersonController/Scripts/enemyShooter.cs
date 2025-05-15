using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;      // The bullet prefab (must be assigned)
    public Transform firePoint;          // The firePoint from which bullets are spawned
    public AudioSource shootSound;       // Audio source that plays when shooting

    [Header("Rhythm Settings")]
    [Tooltip("Shoot on every Nth beat (e.g. 4 = every 4th beat)")]
    public int shootEveryNthBeat = 4;
    [Tooltip("Bullet speed in units per second")]
    public float bulletSpeed = 10f;

    private Transform player;
    private int beatCounter = 0;
    // When true, shooting will stop permanently.
    private bool shootingDisabledPermanently = false;

    void Start()
    {
        // Find the player by tag ("Player")
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        // Subscribe to the metronome's beat event
        Metronome.OnBeat += HandleBeat;
    }

    void HandleBeat()
    {
        if (player == null || firePoint == null || bulletPrefab == null) return;

        // Round Z position to avoid float errors
        float playerZ = Mathf.Round(player.position.z * 10f) / 10f;
        float enemyZ = Mathf.Round(transform.position.z * 10f) / 10f;

        if (!shootingDisabledPermanently && Mathf.Approximately(playerZ, enemyZ))
        {
            shootingDisabledPermanently = true;
            Debug.Log(" Enemy stopped shooting — Z position matched!");
            return;
        }

        if (shootingDisabledPermanently) return;

        beatCounter++;
        if (beatCounter % shootEveryNthBeat == 0)
        {
            Debug.Log($"Bullet instantiated at beat {beatCounter}");
            Shoot();
        }
    }



    void Shoot()
    {
        Debug.Log($"Bullet instantiated at beat {beatCounter}");

        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Bullet prefab or firePoint is missing. Cannot shoot.");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (bullet == null)
        {
            Debug.LogError("Bullet failed to instantiate!");
            return;
        }

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * bulletSpeed;
        }

        if (shootSound != null)
        {
            shootSound.Play();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the metronome event on destroy to prevent errors.
        Metronome.OnBeat -= HandleBeat;
    }
}
