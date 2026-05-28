using UnityEngine;
using UnityEngine.InputSystem;

public class BoatEnterTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform seatPoint;
    [SerializeField] private DiveController diveController;
    [SerializeField] private CharacterController playerController;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameHUDController hudController;

    private bool playerNearby = false;

    private void Update()
    {
        if (!playerNearby) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            EnterBoat();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            playerNearby = true;

            if (hudController != null)
                hudController.ShowHint("Press E to return to the boat.", 10f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            playerNearby = false;

            if (hudController != null)
                hudController.HideHint();
        }
    }

    private void EnterBoat()
    {
        if (seatPoint == null || playerController == null) return;

        playerController.enabled = false;

        playerController.transform.position = seatPoint.position;
        playerController.transform.rotation = seatPoint.rotation;

        playerController.enabled = true;

        if (diveController != null)
            diveController.ExitWater();

        if (hudController != null)
            hudController.HideHint();

        playerNearby = false;
    }
}