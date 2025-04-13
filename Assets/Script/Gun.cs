using UnityEngine;

public class Gun : MonoBehaviour
{
    private float rotateOffset = -90f; // Chỉnh offset để súng xoay đúng hướng
    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shotDelay = 0.2f;
    private float nextShot;
    [SerializeField] private int maxBullet = 5;
    public int currentBullet;
    [SerializeField] private AudioClip shootClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        currentBullet = maxBullet;
    }

    void Update()
    {
        RotateGun();
        Shoot();
    }

    void RotateGun()
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
        {
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        Vector3 displacement = mousePos - transform.position;

        float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + rotateOffset);
    }

    void Shoot()
    {
        if (Input.GetMouseButtonDown(0) && currentBullet > 0 && Time.time > nextShot)
        {
            nextShot = Time.time + shotDelay;
            Instantiate(bulletPrefab, firePos.position, firePos.rotation);
            currentBullet--;

            if (shootClip != null)
                audioSource.PlayOneShot(shootClip);
        }
    }
}
