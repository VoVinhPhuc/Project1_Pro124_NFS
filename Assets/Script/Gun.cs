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
    private float timeSinceStart = 0f; // Thời gian kể từ khi bắt đầu
    private float noShootDelay = 10f;  // Không bắn trong 10 giây đầu

    [SerializeField] private float bulletOffset = 1.5f; // Độ lệch của viên đạn so với vị trí bắn

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        currentBullet = maxBullet;
    }

    void Update()
    {
        RotateGun();
        Shoot();
        timeSinceStart += Time.deltaTime;
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
        if (timeSinceStart < noShootDelay) return; // Không bắn trong 10 giây đầu
        if (Input.GetMouseButtonDown(0) && currentBullet > 0 && Time.time > nextShot)
        {
            // Tạo viên đạn, thêm độ lệch vào vị trí bắn
            Vector3 firePointPosition = firePos.position + transform.up * bulletOffset;

            // Instantiate viên đạn tại vị trí mới
            GameObject bullet = Instantiate(bulletPrefab, firePointPosition, firePos.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();

            // Kiểm tra và gán shooter cho viên đạn
            if (bulletScript != null)
            {
                bulletScript.shooter = gameObject; // GÁN NGƯỜI BẮN LÀ PLAYER
            }

            nextShot = Time.time + shotDelay; // Cập nhật thời gian giữa các phát bắn
            currentBullet--; // Giảm số viên đạn sau mỗi phát bắn

            // Phát âm thanh khi bắn
            if (shootClip != null)
            {
                audioSource.PlayOneShot(shootClip);
            }
        }
    }
}
