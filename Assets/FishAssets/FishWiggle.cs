using UnityEngine;

public class FishWiggle : MonoBehaviour
{
    public enum WiggleAxis
    {
        X,
        Y,
        Z
    }

    public Transform bodyBone;
    public Transform midBone;
    public Transform tailBase;
    public Transform tailTip;

    public WiggleAxis wiggleAxis = WiggleAxis.Z;

    public float speed = 18f;
    public float bodyAmount = 0.2f;
    public float midAmount = 1.2f;
    public float tailAmount = 8f;
    public float tipAmount = 14f;

    void Update()
    {
        float t = Time.time * speed;

        RotateBone(bodyBone, Mathf.Sin(t) * bodyAmount);
        RotateBone(midBone, Mathf.Sin(t - 0.4f) * midAmount);
        RotateBone(tailBase, Mathf.Sin(t - 0.8f) * tailAmount);
        RotateBone(tailTip, Mathf.Sin(t - 1.2f) * tipAmount);
    }

    void RotateBone(Transform bone, float amount)
    {
        if (bone == null) return;

        if (wiggleAxis == WiggleAxis.X)
            bone.localRotation = Quaternion.Euler(amount, 0, 0);
        else if (wiggleAxis == WiggleAxis.Y)
            bone.localRotation = Quaternion.Euler(0, amount, 0);
        else
            bone.localRotation = Quaternion.Euler(0, 0, amount);
    }
}