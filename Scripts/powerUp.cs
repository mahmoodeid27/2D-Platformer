using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public NPCDialogue npcDialogue; // Reference to the NPCDialogue script
    private bool abilityGranted = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !abilityGranted)
        {
            abilityGranted = true;

            // Grant the player the ability to toggle platforms
            PlayerAbility playerAbility = collision.GetComponent<PlayerAbility>();
            if (playerAbility != null)
            {
                playerAbility.EnablePlatformToggle();
            }

            // Trigger the NPC to force dialogue
            npcDialogue.ActivateDialogueFromPowerUp();

            // Destroy the power-up object
            Destroy(gameObject);
        }
    }
}
