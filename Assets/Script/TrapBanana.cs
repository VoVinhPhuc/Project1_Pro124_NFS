using System.Collections;
using UnityEngine;

public class TrapBanana : MonoBehaviour
{
    public GameObject owner;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC") || other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        
    }
}
