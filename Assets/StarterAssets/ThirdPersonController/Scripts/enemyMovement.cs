using UnityEngine;

public class EnemyLaneMover : MonoBehaviour
{
    public float laneOffset = 1.2f;
    private int currentLane = 1; // 0 = Left, 1 = Center, 2 = Right
    private bool isMoving = false;
    public float moveDuration = 0.5f;
    void Update()
    {
        if (!isMoving)
        {
            StartCoroutine(ShiftLane(Random.Range(0, 3)));
        }
    }
}
