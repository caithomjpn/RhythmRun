using UnityEngine;
using System.Collections;

public class enemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public bool enableMovement = true;
    public float moveSpeed = 2f;
    public float laneOffset = 1.2f;
    public float moveDuration = 0.5f;

    private int currentLane = 1; // 0 = left, 1 = center, 2 = right
    private bool isMoving = false;

    void Update()
    {
        if (!enableMovement) return;

        // Example movement: always move forward
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private IEnumerator ShiftLane(int targetLane)
    {
        if (targetLane == currentLane) yield break;

        isMoving = true;
        float elapsed = 0f;
        Vector3 start = transform.position;
        float[] laneX = { -laneOffset, 0f, laneOffset };
        Vector3 end = new Vector3(laneX[targetLane], start.y, start.z);

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            transform.position = Vector3.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        currentLane = targetLane;
        isMoving = false;
    }
}
