using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawHitbox : MonoBehaviour
{
    public void EnableHitbox()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;
    }

    public void DisableHitbox()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DodgeCat.Instance.OnPlayerHit();
        }
    }
}