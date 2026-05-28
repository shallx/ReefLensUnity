using UnityEngine;

public class CrabMovement : MonoBehaviour
{
    public float speed = 0.5f;

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}