using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PhotoCameraSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject cameraOverlay;
    [SerializeField] private Image photoThumbnail;
    [SerializeField] private float thumbnailShowTime = 3f;

    [Header("Mission")]
    [SerializeField] private BoatMissionController missionController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float photoDetectionDistance = 50f;

    [Header("Audio")]
    [SerializeField] private AudioSource cameraAudioSource;
    [SerializeField] private AudioClip cameraClickSound;

    [Header("Screenshot Settings")]
    [SerializeField] private string folderName = "ReefLensPhotos";
    [SerializeField] private int screenshotScale = 1;

    [SerializeField] private GameHUDController hudController;

    private bool cameraMode = false;
    public bool IsCameraModeActive => cameraMode;

    private string saveFolderPath;

    private void Start()
    {
        saveFolderPath = Path.Combine(Application.persistentDataPath, folderName);

        if (!Directory.Exists(saveFolderPath))
            Directory.CreateDirectory(saveFolderPath);

        if (cameraOverlay != null)
            cameraOverlay.SetActive(false);

        if (photoThumbnail != null)
            photoThumbnail.gameObject.SetActive(false);

        if (playerCamera == null)
            playerCamera = Camera.main;

        Debug.Log("Photos will save here: " + saveFolderPath);
    }

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
            ToggleCameraMode();

        if (cameraMode && Mouse.current.leftButton.wasPressedThisFrame)
            StartCoroutine(TakePhoto());
    }

    private void ToggleCameraMode()
    {
        cameraMode = !cameraMode;

        if (cameraOverlay != null)
            cameraOverlay.SetActive(cameraMode);

        if (cameraMode && hudController != null)
            hudController.Objective_TakePhoto();
    }

    private IEnumerator TakePhoto()
    {
        if (cameraAudioSource != null && cameraClickSound != null)
            cameraAudioSource.PlayOneShot(cameraClickSound);

        string capturedFishID = DetectFishInPhoto();

        if (cameraOverlay != null)
            cameraOverlay.SetActive(false);

        yield return new WaitForEndOfFrame();

        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();

        string fileName = "ReefLens_Photo_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string fullPath = Path.Combine(saveFolderPath, fileName);

        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(fullPath, bytes);

        Sprite photoSprite = CreateSpriteFromTexture(screenshot);

        ShowThumbnail(photoSprite);

        if (missionController != null && !string.IsNullOrEmpty(capturedFishID))
        {
            missionController.SetLastCapturedFish(capturedFishID, photoSprite);

            if (hudController != null)
                hudController.Objective_ReturnToBoat();
        }

        Debug.Log("Photo saved: " + fullPath);
        Debug.Log("Captured Fish ID: " + capturedFishID);

        yield return new WaitForSeconds(0.15f);

        if (cameraOverlay != null && cameraMode)
            cameraOverlay.SetActive(true);
    }

    private string DetectFishInPhoto()
    {
        if (playerCamera == null)
            return "";

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, photoDetectionDistance))
        {
            FishIdentity fish = hit.collider.GetComponentInParent<FishIdentity>();

            if (fish != null)
                return fish.fishID;
        }

        return "";
    }

    private Sprite CreateSpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }

    private void ShowThumbnail(Sprite sprite)
    {
        if (photoThumbnail == null)
            return;

        photoThumbnail.sprite = sprite;
        photoThumbnail.gameObject.SetActive(true);

        StopCoroutine(nameof(HideThumbnailAfterDelay));
        StartCoroutine(nameof(HideThumbnailAfterDelay));
    }

    private IEnumerator HideThumbnailAfterDelay()
    {
        yield return new WaitForSeconds(thumbnailShowTime);

        if (photoThumbnail != null)
            photoThumbnail.gameObject.SetActive(false);
    }
}