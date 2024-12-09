using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    private bool canTogglePlatforms = false;

    public void EnablePlatformToggle()
    {
        canTogglePlatforms = true;
    }

    void Update()
    {
        if (canTogglePlatforms && Input.GetKeyDown(KeyCode.W))
        {
            TogglePlatforms();
        }
    }

    void TogglePlatforms()
    {
        // Find all platforms in the scene
        PlatformToggle[] platforms = FindObjectsOfType<PlatformToggle>();
        foreach (PlatformToggle platform in platforms)
        {
            platform.ToggleState();
        }
    }
}
