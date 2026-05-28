using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    private void Start()
    {
        float volume = PlayerPrefs.GetFloat("Volume", 1f);
        float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 0.03f);
        int quality = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());

        volumeSlider.value = volume;
        sensitivitySlider.value = sensitivity;
        qualityDropdown.value = quality;

        SetVolume(volume);
        SetSensitivity(sensitivity);
        SetQuality(quality);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("Quality", qualityIndex);
    }
}