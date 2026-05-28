using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHUDController : MonoBehaviour
{

    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("References")]
    [SerializeField] private DiveController diveController;
    [SerializeField] private PhotoCameraSystem photoCameraSystem;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("HUD Boxes")]
    [SerializeField] private GameObject gogglesOverlay;
    [SerializeField] private GameObject objectiveBox;
    [SerializeField] private GameObject hintBox;

    [Header("HUD Text")]
    [SerializeField] private TMP_Text fishProgressText;
    [SerializeField] private TMP_Text oxygenText;

    [Header("Control Labels")]
    [SerializeField] private string moveForwardKey = "W";
    [SerializeField] private string diveKey = "X";
    [SerializeField] private string cameraKey = "P";
    [SerializeField] private string missionKey = "F";

    [Header("HUD Values")]
    [SerializeField] private int totalFish = 3;
    [SerializeField] private float oxygen = 100f;

    private bool shownOpenMissionObjective = false;
    private bool shownDiveObjective = false;
    private bool shownSearchObjective = false;
    private bool shownCameraObjective = false;
    private bool shownTakePhotoObjective = false;
    private bool shownReturnToBoatObjective = false;
    private bool shownFindAllFishObjective = false;

    private TMP_Text objectiveText;
    private TMP_Text hintText;

    private int capturedFish = 0;
    private Coroutine hintCoroutine;

    [Header("Oxygen System")]
    [SerializeField] private Image oxygenBarFill;
    [SerializeField] private float maxOxygen = 100f;
    [SerializeField] private float oxygenDrainRate = 5f;
    [SerializeField] private float oxygenRecoverRate = 12f;

    private float currentOxygen;

    private void Awake()
    {
        if (objectiveBox != null)
            objectiveText = objectiveBox.GetComponentInChildren<TMP_Text>(true);

        if (hintBox != null)
            hintText = hintBox.GetComponentInChildren<TMP_Text>(true);
    }

    private void Start()
    {
        if (gogglesOverlay != null)
            gogglesOverlay.SetActive(false);

        if (objectiveBox != null)
            objectiveBox.SetActive(true);

        if (hintBox != null)
            hintBox.SetActive(false);

        Objective_OpenMissionController();
        UpdateFishProgress(0, totalFish);
        currentOxygen = maxOxygen;
        UpdateOxygen(currentOxygen);


    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }

        UpdateGogglesOverlay();
        //UpdateUnderwaterIdleHint();
        UpdateOxygenSystem();
    }

    private void UpdateOxygenSystem()
    {
        if (diveController == null)
            return;

        bool fullyUnderwater = diveController.inWater && !diveController.isAtSurface;

        if (fullyUnderwater)
            currentOxygen -= oxygenDrainRate * Time.deltaTime;
        else
            currentOxygen += oxygenRecoverRate * Time.deltaTime;

        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

        UpdateOxygen(currentOxygen);
    }

    private void UpdateGogglesOverlay()
    {
        if (gogglesOverlay == null)
            return;

        if (photoCameraSystem != null && photoCameraSystem.IsCameraModeActive)
        {
            gogglesOverlay.SetActive(false);
            return;
        }

        if (diveController == null)
        {
            gogglesOverlay.SetActive(false);
            return;
        }

        bool fullyUnderwater = diveController.inWater && !diveController.isAtSurface;
        gogglesOverlay.SetActive(fullyUnderwater);
    }

    private void UpdateUnderwaterIdleHint()
    {
        if (diveController == null || playerMovement == null)
            return;

        if (photoCameraSystem != null && photoCameraSystem.IsCameraModeActive)
            return;

        bool underwater = diveController.inWater && !diveController.isAtSurface;
        bool notMoving = !playerMovement.IsMoving;

        if (underwater && notMoving)
        {
            ShowHint("Hold " + moveForwardKey + " to swim forward.", 2f);
        }
    }

    public void SetObjective(string message)
    {
        if (objectiveBox != null)
            objectiveBox.SetActive(true);

        if (objectiveText != null)
            objectiveText.text = message;
    }

    public void UpdateFishProgress(int captured, int total)
    {
        capturedFish = captured;
        totalFish = total;

        if (fishProgressText != null)
            fishProgressText.text = "Fish Captured: " + capturedFish + " / " + totalFish;
    }

    public void UpdateOxygen(float value)
    {
        oxygen = Mathf.Clamp(value, 0f, maxOxygen);

        if (oxygenText != null)
            oxygenText.text = "Oxygen: " + Mathf.RoundToInt(oxygen) + "%";

        if (oxygenBarFill != null)
            oxygenBarFill.fillAmount = oxygen / maxOxygen;
    }

    public void ShowHint(string message, float duration = 3f)
    {
        if (hintBox == null || hintText == null)
            return;

        if (hintCoroutine != null)
            StopCoroutine(hintCoroutine);

        hintCoroutine = StartCoroutine(ShowHintRoutine(message, duration));
    }

    public void HideHint()
    {
        if (hintCoroutine != null)
            StopCoroutine(hintCoroutine);

        if (hintBox != null)
            hintBox.SetActive(false);
    }

    private IEnumerator ShowHintRoutine(string message, float duration)
    {
        hintBox.SetActive(true);
        hintText.text = message;

        yield return new WaitForSeconds(duration);

        hintBox.SetActive(false);
    }

    public void Objective_OpenMissionController()
    {
        if (shownOpenMissionObjective) return;

        shownOpenMissionObjective = true;
        SetObjective("Open the boat controller to view your first marine life target.");
    }

    public void Objective_DiveIntoReef()
    {
        if (shownDiveObjective) return;

        shownDiveObjective = true;
        SetObjective("Dive into the reef and begin your search.");
    }

    public void Objective_SearchMatchingFish()
    {
        if (shownSearchObjective) return;

        shownSearchObjective = true;
        StartCoroutine(SearchThenCameraHintRoutine());
    }

    private IEnumerator SearchThenCameraHintRoutine()
    {
        SetObjective("Search the reef for the matching fish.");

        yield return new WaitForSeconds(10f);

        Objective_OpenCamera();
    }

    public void Objective_OpenCamera()
    {
        if (shownCameraObjective) return;

        shownCameraObjective = true;
        SetObjective("Press P to open the camera.");
    }

    public void Objective_TakePhoto()
    {
        if (shownTakePhotoObjective) return;

        shownTakePhotoObjective = true;
        SetObjective("Aim at the fish and left-click to take a photo.");
    }

    public void Objective_ReturnToBoat()
    {
        if (shownReturnToBoatObjective) return;

        shownReturnToBoatObjective = true;
        SetObjective("Photo captured. Return to the boat and submit it.");
    }

    public void Objective_FindAllFish()
    {
        if (shownFindAllFishObjective) return;

        shownFindAllFishObjective = true;
        SetObjective("Great match. Keep exploring and complete your marine life collection.");
    }
}