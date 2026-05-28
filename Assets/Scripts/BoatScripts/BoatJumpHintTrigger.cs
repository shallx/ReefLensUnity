using UnityEngine;
using UnityEngine.InputSystem;

public class BoatJumpHintTrigger : MonoBehaviour
{
    [SerializeField] private GameHUDController hudController;
    [SerializeField] private float jumpForce = 6f;

    private PlayerMovement playerMovement;
    private bool playerInside;

    private void Update()
    {
        if (!playerInside || playerMovement == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            playerMovement.JumpFromBoat(jumpForce);

            if (hudController != null)
            {
                hudController.Objective_SearchMatchingFish();
                hudController.HideHint();
                
            }  

            DiveController diveController = playerMovement.GetComponent<DiveController>();
            if (diveController != null)
                diveController.MarkJumpedFromBoat();
                
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement movement = other.GetComponentInParent<PlayerMovement>();

        if (movement != null)
        {
            playerMovement = movement;
            playerInside = true;

            if (hudController != null)
                hudController.ShowHint("Press SPACE to jump into the reef.", 10f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            playerInside = false;
            playerMovement = null;

            if (hudController != null)
                hudController.HideHint();
        }
    }
}