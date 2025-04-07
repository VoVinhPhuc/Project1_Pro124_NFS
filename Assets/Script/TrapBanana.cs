using UnityEngine;
using System.Collections;


public class TrapBanana : MonoBehaviour
{
    [SerializeField] private Transform trapBananaPos;
    [SerializeField] private GameObject bananaPrefab;

   private bool isActivated = false;

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.F) && !isActivated)
    //     {
    //         isActivated = true;
    //         StartCoroutine(DestroyAfterTime(20f));
    //     }
    // }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            Destroy(gameObject);
        }
    }

    // IEnumerator DestroyAfterTime(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     Destroy(gameObject);
    // }
}
