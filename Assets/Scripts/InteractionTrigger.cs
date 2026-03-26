using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    private bool inRange = false;

    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            Level2EventManager.Instance.StartInteraction();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
        }
    }
}