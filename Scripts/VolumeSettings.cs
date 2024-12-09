using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public Slider musicSlider; // Reference to your UI Slider
    public Slider effectsSlider;

    private void Start()
    {
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found!");
            return;
        }

        // Load saved volume setting
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume");
        //float savedFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        musicSlider.SetValueWithoutNotify(savedVolume);
        //effectsSlider.SetValueWithoutNotify(savedFXVolume);
        
        audioManager.SetMusicVolume(savedVolume);
        //audioManager.SetSfxVolume(savedFXVolume);

        // Add listener to handle slider value changes
        musicSlider.onValueChanged.AddListener(SetVolume);
        //effectsSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float sliderValue)
    {
        AudioManager.instance.SetMusicVolume(sliderValue);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        PlayerPrefs.Save();
    }

    public void SetEffectsVolume(float sliderValue)
    {
        AudioManager.instance.SetSfxVolume(sliderValue);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        musicSlider.onValueChanged.RemoveListener(SetVolume);
    }
}
