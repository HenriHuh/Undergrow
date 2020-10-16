using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public GameObject settings;
    public Slider musicSlider;
    public Slider effectSlider;

    bool sliderPressed = false;
    float sliderValue = 0;

    void Start()
    {
        
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
}