using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour
{
    private AudioManager audioManager;

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in the scene!");
        }
    }
    public void LoadOnClick(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        if (audioManager != null)
        {
            // Change music based on the scene being loaded
            if (sceneIndex == 1) // Assuming index 1 is Level 1
            {
                audioManager.ChangeBackgroundMusic(audioManager.level1Music);
            }
            else if (sceneIndex == 0) // Assuming index 0 is Main Menu
            {
                audioManager.ChangeBackgroundMusic(audioManager.mainMenuMusic);
            }
        }
    }

    public void Exit()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in the editor
        #endif
    }
}
