using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int lives = 3;

    public void TakeDamage(int damage)
    {
        lives -= damage;
        if (lives <= 0)
        {
            Destroy(gameObject); // Xóa object khi hết mạng
        }
    }
}