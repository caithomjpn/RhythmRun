using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float baseSpeed = 5f;
    public float maxEscapeSpeed = 20f;
    public float triggerDistance = 15f;
    public float fireIntervalInBeats = 4f;

    private float beatCounter = 0f;
    private bool gameStarted = false;

    private float lastLogTime = 0f;
    private float logInterval = 1f; // log once per second

    private float escapeSpeedDuration = 2f;
    private float escapeSpeedTimer = 0f;
    private bool isEscaping = false;
    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("Player not found in scene.");
            }
        }
    }

    void OnEnable()
    {
        Metronome.OnBeat += HandleBeat;
    }

    void OnDisable()
    {
        Metronome.OnBeat -= HandleBeat;
    }

    void HandleBeat()
    {
        if (!gameStarted && Metronome.GameHasStarted)
        {
            gameStarted = true;
            Debug.Log("Game has started — Boss is active.");
        }

        if (!gameStarted) return;

        beatCounter++;

        if (Metronome.IsActionBeat && beatCounter % fireIntervalInBeats == 0)
        {
            Fire();
        }
    }

    void Update()
    {
        if (!gameStarted || player == null) return;

        FollowPlayer();

        float distanceZ = player.position.z - transform.position.z;
        float speed = baseSpeed;

        // Escape logic: player is within 10 units behind or ahead
        if (distanceZ > -15f && !isEscaping)
        {
            isEscaping = true;
            escapeSpeedTimer = escapeSpeedDuration;
        }
        if (isEscaping)
        {
            speed = maxEscapeSpeed;
            escapeSpeedTimer -= Time.deltaTime;

            if (escapeSpeedTimer <= 0f)
            {
                isEscaping = false;
            }
        }


        transform.position += new Vector3(0, 0, speed * Time.deltaTime);

        // Log once per second
        if (Time.time - lastLogTime >= 1f)
        {
            Debug.Log($"[Boss Log] PlayerZ={player.position.z:F2}, BossZ={transform.position.z:F2}, DistanceZ={distanceZ:F2}, Speed={speed:F2}");
            lastLogTime = Time.time;
        }
    }

    void FollowPlayer()
    {
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * baseSpeed);
    }

    void Fire()
    {
        if (bulletPrefab != null && firePoint != null && player != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            HomingBullet homing = bullet.GetComponent<HomingBullet>();
            if (homing != null)
            {
                homing.target = player;
            }

            Debug.Log("Boss fired a bullet.");
        }
    }
}
