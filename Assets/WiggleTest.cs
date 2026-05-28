using UnityEngine;

public class FishRigWiggleTest : MonoBehaviour
{
    [Header("Bones")]
    public Transform bodyBone;
    public Transform midBone;
    public Transform tailBase;
    public Transform tailTip;

    [Header("Extra Parts")]
    public Transform leftEye;
    public Transform rightEye;
    public Transform upperFin;
    public Transform lowerFin;
    public Transform sideFin;

    [Header("Wiggle Settings")]
    public float speed = 18f;
    public float bodyUpDownAmount = 0.3f;
    public float midSideAmount = 1.5f;
    public float tailSideAmount = 10f;
    public float tipSideAmount = 18f;
    public float eyeFollowAmount = 0.2f;
    public float finAmount = 3f;

    void Update()
    {
        float time = Time.time;

        float waveBody = Mathf.Sin(time * speed);
        float waveMid = Mathf.Sin(time * speed - 0.4f);
        float waveTail = Mathf.Sin(time * speed - 0.8f);
        float waveTip = Mathf.Sin(time * speed - 1.2f);

        // Body: tiny up/down only
        if (bodyBone != null)
            bodyBone.localRotation = Quaternion.Euler(waveBody * bodyUpDownAmount, 0, 0);

        // Mid body: slight sideways movement
        if (midBone != null)
            midBone.localRotation = Quaternion.Euler(0, waveMid * midSideAmount, 0);

        // Tail: strong fast sideways movement
        if (tailBase != null)
            tailBase.localRotation = Quaternion.Euler(0, 0, waveTail * tailSideAmount);

        // Tail tip: strongest movement
        if (tailTip != null)
            tailTip.localRotation = Quaternion.Euler(0, 0, waveTip * tipSideAmount);

        // Eyes follow slightly
        if (leftEye != null)
            leftEye.localRotation = Quaternion.Euler(waveBody * eyeFollowAmount, 0, 0);

        if (rightEye != null)
            rightEye.localRotation = Quaternion.Euler(waveBody * eyeFollowAmount, 0, 0);

        // Fins flap subtly
        if (upperFin != null)
            upperFin.localRotation = Quaternion.Euler(waveBody * finAmount, 0, 0);

        if (lowerFin != null)
            lowerFin.localRotation = Quaternion.Euler(-waveBody * finAmount, 0, 0);

        if (sideFin != null)
            sideFin.localRotation = Quaternion.Euler(0, waveBody * finAmount, 0);
    }
}