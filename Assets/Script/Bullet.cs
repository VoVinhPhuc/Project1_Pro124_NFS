using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject shooter;
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float timeDestroy = 0.5f;

    void Start()
    {
        Destroy(gameObject, timeDestroy);
    }

    void Update()
    {
        MoveBullet();
    }

    void MoveBullet()
    {
        transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
    }
}
