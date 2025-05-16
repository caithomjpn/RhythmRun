using UnityEngine;
using System.Collections;

public class enemyMovement : MonoBehaviour
{
    public enum MovementPattern { None, Forward, LaneShift }

    [Header("Movement Settings")]
    public MovementPattern movementPattern = MovementPattern.Forward;
    public float moveSpeed = 2f;
    public float laneOffset = 1.2f;
    public float moveDuration = 0.5f;

    private int currentLane = 1; 
    private bool isShifting = false;
    void Update()
    {
        switch (movementPattern)
        {
            case MovementPattern.Forward:
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                break;

            case MovementPattern.LaneShift:
                if (!isShifting)
                {
                    int nextLane = GetRandomLane();
                    StartCoroutine(ShiftLane(nextLane));
                }
                break;

            case MovementPattern.None:
                // No movement
                break;
        }
    }
    private int GetRandomLane()
    {
        int nextLane;
        do
        {
            nextLane = Random.Range(0, 3);
        } while (nextLane == currentLane);

        return nextLane;
    }
    private IEnumerator ShiftLane(int targetLane)
    {
        isShifting = true;
        float elapsed = 0f;
        Vector3 start = transform.position;
        float[] laneX = { -laneOffset, 0f, laneOffset };
        Vector3 end = new Vector3(laneX[targetLane], start.y, start.z);

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            float newX = Mathf.Lerp(start.x, end.x, t);
            transform.position = new Vector3(newX, start.y, transform.position.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(end.x, start.y, transform.position.z);
        currentLane = targetLane;
        isShifting = false;
    }
}
