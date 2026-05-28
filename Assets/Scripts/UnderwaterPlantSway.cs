using UnityEngine;

public class UnderwaterPlantSway : MonoBehaviour
{
    public float swayAmount = 5f;
    public float swaySpeed = 1.5f;
    public float positionSwayAmount = 0.03f;

    private Quaternion startRotation;
    private Vector3 startPosition;
    private float randomOffset;

    void Start()
    {
        startRotation = transform.localRotation;
        startPosition = transform.localPosition;
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float sway = Mathf.Sin(Time.time * swaySpeed + randomOffset);

        transform.localRotation = startRotation * Quaternion.Euler(
            sway * swayAmount,
            0f,
            sway * swayAmount * 0.5f
        );

        transform.localPosition = startPosition + new Vector3(
            sway * positionSwayAmount,
            0f,
            0f
        );
    }
}