using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class StartScreen : MonoBehaviour
{
    public GameObject settings;
    public Slider musicSlider;
    public Slider effectSlider;
    public Image startButton;
    public Text qualityText;
    public GameObject endlessUnlockText;
    public Button endlessBtn;

    bool sliderPressed = false;
    float sliderValue = 0;

    void Start()
    {
        SaveData data = GameData.saveData();
        GVar.quality = (GVar.Quality)data.quality;
        if (GVar.newSave && SystemInfo.systemMemorySize > 5900) //6G RAM -> Set high quality
        {
            GVar.quality = 0;
        }
        SetQuality(0);
        if (GVar.quality != GVar.Quality.high)
        {
            Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing = false;
        }

        if (GVar.completedGoals.Count < 14)
        {
            endlessBtn.interactable = false;
            endlessUnlockText.SetActive(true);
        }
    }

    private void OnDisable()
    {
        GameData.Save(new SaveData());
    }

    void Update()
    {
        if (sliderPressed && Input.GetKeyUp(KeyCode.Mouse0))
        {
            SoundManager.instance.PlaySound(SoundManager.instance.buttonBasic, sliderValue);
            sliderPressed = false;
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Garden");
    }

    public void SetMusicVolume()
    {
        SoundManager.instance.SetMusicVolume(musicSlider.value);
        sliderValue = musicSlider.value;
        sliderPressed = true;
    }

    public void SetEffectVolume()
    {
        SoundManager.instance.SetEffectVolume(effectSlider.value);
        sliderValue = effectSlider.value;
        sliderPressed = true;
    }

    public void PlayClick()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.buttonBasic);
    }

    public void StartEndless()
    {
        //TODO:
        //  1. Load levels from savedata
        //  2. Check if above level x
        //  4. Return to start screen after playing

        if (GVar.completedGoals.Count > 13) // = level 15
        {
            GVar.currentPlant = null;
            SceneManager.LoadScene("Tree");
        }

    }

    public void SetQuality(int toChange) //0 = high
    {
        int quality = (int)GVar.quality - toChange;
        quality  = quality > 2 ? 0 : quality;
        quality = quality < 0 ? 2 : quality;
        switch (quality)
        {
            case 0:
                qualityText.text = "High";
                break;
            case 1:
                qualityText.text = "Medium";
                break;
            case 2:
                qualityText.text = "Low";
                break;
            default:
                break;
        }

        GVar.quality = (GVar.Quality)quality;
        if (GVar.quality == GVar.Quality.high)
        {
            Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing = true;
        }
        else
        {
            Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing = false;

        }
    }
}