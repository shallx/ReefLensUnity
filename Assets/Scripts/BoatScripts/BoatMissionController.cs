using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class FishMissionTarget
{
    public string fishID;
    public string fishName;
    public Sprite fishImage;
    [TextArea] public string fishFact;
}

public class BoatMissionController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject missionPanel;
    [SerializeField] private GameObject gameHUD;
    [SerializeField] private Image targetFishImage;
    [SerializeField] private Image capturedFishImage;
    [SerializeField] private Sprite capturedPlaceholderSprite;
    [SerializeField] private TMP_Text fishNameText;
    [SerializeField] private TMP_Text fishFactText;
    [SerializeField] private TMP_Text resultText;

    [Header("Mission Targets")]
    [SerializeField] private FishMissionTarget[] targets;

    [Header("References")]
    [SerializeField] private GameHUDController hudController;

    private bool firstMissionViewed = false;
    private int currentTargetIndex = 0;
    private int capturedFishCount = 0;
    private string lastCapturedFishID;
    private Sprite lastCapturedSprite;

    private void Start()
    {
        if (missionPanel != null)
            missionPanel.SetActive(false);

        if (resultText != null)
            resultText.text = "";

        UpdateHUDProgress();
        RefreshMissionUI();
    }

    public void OpenMissionPanel()
    {
        RefreshMissionUI();

        if (missionPanel != null)
            missionPanel.SetActive(true);

        if (gameHUD != null)
            gameHUD.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseMissionPanel()
    {
        if (missionPanel != null)
            missionPanel.SetActive(false);

        if (gameHUD != null)
            gameHUD.SetActive(true);

        if (resultText != null)
            resultText.text = "";

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!firstMissionViewed)
        {
            firstMissionViewed = true;

            if (hudController != null)
                hudController.Objective_DiveIntoReef();
        }
    }

    public void SetLastCapturedFish(string fishID, Sprite capturedSprite)
    {
        lastCapturedFishID = fishID;
        lastCapturedSprite = capturedSprite;

        RefreshMissionUI();
    }

    public void SubmitPhoto()
    {
        if (targets == null || targets.Length == 0)
            return;

        FishMissionTarget target = targets[currentTargetIndex];

        if (string.IsNullOrEmpty(lastCapturedFishID))
        {
            ShowResult("Take a photo first.", "#FFD447");
            return;
        }

        if (lastCapturedFishID == target.fishID)
        {
            capturedFishCount++;
            UpdateHUDProgress();

            ShowResult("Correct photo! New fish target unlocked.", "#00FF88");

            if (hudController != null)
                hudController.Objective_FindAllFish();

            currentTargetIndex++;

            if (currentTargetIndex >= targets.Length)
            {
                currentTargetIndex = targets.Length - 1;
                ShowResult("Mission complete. All target fish photographed.", "#00FF88");
            }

            lastCapturedFishID = "";
            lastCapturedSprite = null;

            RefreshMissionUI();
        }
        else
        {
            ShowResult("Wrong fish. Try again.", "#FF4A4A");
        }
    }

    private void RefreshMissionUI()
    {
        if (targets == null || targets.Length == 0)
            return;

        FishMissionTarget target = targets[currentTargetIndex];

        if (targetFishImage != null)
            targetFishImage.sprite = target.fishImage;

        if (fishNameText != null)
            fishNameText.text = target.fishName;

        if (fishFactText != null)
            fishFactText.text = target.fishFact;

        if (capturedFishImage != null)
        {
            capturedFishImage.enabled = true;
            capturedFishImage.sprite = lastCapturedSprite != null
                ? lastCapturedSprite
                : capturedPlaceholderSprite;
        }
    }

    private void UpdateHUDProgress()
    {
        if (hudController != null)
        {
            int total = targets != null ? targets.Length : 0;
            hudController.UpdateFishProgress(capturedFishCount, total);
        }
    }

    private void ShowResult(string message, string hexColor)
    {
        if (resultText != null)
            resultText.text = "<b><color=" + hexColor + ">" + message + "</color></b>";
    }
}