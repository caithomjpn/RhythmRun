using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;
    public float rotationSpeed = 5f;

    void Update()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, rotationSpeed * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
