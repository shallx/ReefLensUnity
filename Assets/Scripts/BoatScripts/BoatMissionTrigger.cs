using UnityEngine;
using UnityEngine.InputSystem;

public class BoatMissionTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoatMissionController missionController;
    [SerializeField] private GameHUDController hudController;

    private bool playerInside;

    private void Update()
    {
        if (!playerInside)
            return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (missionController != null)
                missionController.OpenMissionPanel();

            if (hudController != null)
                hudController.HideHint();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            playerInside = true;

            if (hudController != null)
                hudController.ShowHint("Press F to open mission control.", 10f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            playerInside = false;

            if (hudController != null)
                hudController.HideHint();
        }
    }
}