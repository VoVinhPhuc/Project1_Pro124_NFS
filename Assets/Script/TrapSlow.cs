using UnityEngine;

public class TrapSlow : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(1);
            }
        }
    }
}
