using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public DialogueLine[] dialogueLines;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        Destroy(gameObject);

        if (dialogueLines != null && dialogueLines.Length > 0)
        {
            DialogueManager.Instance.StartCoroutine(
                DialogueManager.Instance.DelayDialogue(dialogueLines)
            );
        }

        // 通知收集系统
        CollectManager.Instance.AddItem();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}