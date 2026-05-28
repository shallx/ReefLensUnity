using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class BoatInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public CharacterController playerController;
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour mouseLookScript;

    public Transform seatPoint;
    public Transform destinationPoint;

    [Header("UI")]
    public GameObject promptUI; // assign a UI panel/text
    public TextMeshProUGUI promptText;

    [Header("Boat Travel")]
    public float travelDuration = 5f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Optional Camera Look")]
    public Camera playerCamera;
    public Transform lookTargetDuringRide;

    private bool playerInRange = false;
    private bool isTravelling = false;

    private void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);

        if (promptText != null)
            promptText.text = "Press E to enter boat";
    }

    private void Update()
    {
        if (playerInRange && !isTravelling)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                StartCoroutine(StartBoatRide());
            }
        }
    }

    private IEnumerator StartBoatRide()
    {
        isTravelling = true;

        if (promptUI != null)
            promptUI.SetActive(false);

        // Disable player control
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (mouseLookScript != null) mouseLookScript.enabled = false;
        if (playerController != null) playerController.enabled = false;

        // Move player into boat seat
        player.position = seatPoint.position;
        player.rotation = seatPoint.rotation;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = destinationPoint.position;
        Quaternion endRot = destinationPoint.rotation;

        float elapsed = 0f;

        while (elapsed < travelDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / travelDuration);
            float easedT = easeCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, endPos, easedT);
            transform.rotation = Quaternion.Slerp(startRot, endRot, easedT);

            // Keep player attached to seat
            player.position = seatPoint.position;
            player.rotation = seatPoint.rotation;

            // Optional: camera faces look target during ride
            if (playerCamera != null && lookTargetDuringRide != null)
            {
                Vector3 dir = lookTargetDuringRide.position - playerCamera.transform.position;
                Quaternion targetRot = Quaternion.LookRotation(dir);
                playerCamera.transform.rotation = Quaternion.Slerp(
                    playerCamera.transform.rotation,
                    targetRot,
                    Time.deltaTime * 2f
                );
            }

            yield return null;
        }

        // Re-enable player control
        if (playerController != null) playerController.enabled = true;
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (mouseLookScript != null) mouseLookScript.enabled = true;

        isTravelling = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptUI != null && !isTravelling)
                promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }
}