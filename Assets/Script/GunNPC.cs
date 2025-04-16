using UnityEngine;
using System.Collections.Generic;

public class GunNPC : MonoBehaviour
{
    private float rotateOffset = -90f;
    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shotDelay = 0.2f;
    private float nextShot;
    [SerializeField] private int maxBullet = 5;
    public int currentBullet;
    [SerializeField] private AudioClip shootClip;
    private AudioSource audioSource;

    [Header("NPC Shooting")]
    public bool isNPC = false;
    public Transform playerTarget;
    public float shootingRange = 10f;

    private List<GameObject> allNPCs = new List<GameObject>();

    private float timeSinceStart = 0f; // Thời gian kể từ khi bắt đầu
    private float noShootDelay = 10f;  // Không bắn trong 10 giây đầu

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentBullet = maxBullet;

        // Lưu tất cả NPC trong scene để tối ưu hóa việc tìm kiếm
        allNPCs.AddRange(GameObject.FindGameObjectsWithTag("NPC"));
    }

    void Update()
    {
        if (isNPC)
        {
            AutoShootAtTarget();
        }
        else
        {
            RotateGun();
            Shoot();
        }
        timeSinceStart += Time.deltaTime;
    }

    void RotateGun()
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
            return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        Vector3 displacement = mousePos - transform.position;

        float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + rotateOffset);
    }

    void Shoot()
    {
        if (timeSinceStart < noShootDelay) return;
        if (Input.GetMouseButtonDown(0) && currentBullet > 0 && Time.time > nextShot)
        {
            nextShot = Time.time + shotDelay;

            // Tính toán vị trí firePos
            Vector3 firePointPosition = firePos.position + transform.up * 1.5f;

            // Tạo đạn
            FireBullet(firePointPosition);
        }
    }

    void AutoShootAtTarget()
    {
        if (timeSinceStart < noShootDelay) return;
        
        Transform target = GetClosestTarget();
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + rotateOffset);

        if (currentBullet > 0 && Time.time > nextShot)
        {
            nextShot = Time.time + shotDelay;

            // Tính toán vị trí firePos
            Vector3 firePointPosition = firePos.position + transform.up * 1.5f;

            // Tạo đạn
            FireBullet(firePointPosition);
        }
    }

    void FireBullet(Vector3 firePointPosition)
    {
        // Tạo đạn
        GameObject bullet = Instantiate(bulletPrefab, firePointPosition, firePos.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
            bulletScript.shooter = gameObject;

        // Cập nhật va chạm
        Collider2D bulletCol = bullet.GetComponent<Collider2D>();
        Collider2D npcCol = GetComponent<Collider2D>();
        if (bulletCol != null && npcCol != null)
            Physics2D.IgnoreCollision(bulletCol, npcCol);

        // Giảm số viên đạn
        currentBullet--;

        // Phát âm thanh bắn
        if (shootClip != null)
            audioSource.PlayOneShot(shootClip);
    }

    Transform GetClosestTarget()
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;

        // Kiểm tra Player nếu có
        if (playerTarget != null)
        {
            float dist = Vector2.Distance(transform.position, playerTarget.position);
            if (dist <= shootingRange)
            {
                closest = playerTarget;
                minDist = dist;
            }
        }

        // Tìm NPC khác trong danh sách đã lưu
        foreach (GameObject npc in allNPCs)
        {
            if (npc.transform == transform.root) continue; // bỏ qua chính mình

            float dist = Vector2.Distance(transform.position, npc.transform.position);
            if (dist <= shootingRange && dist < minDist)
            {
                closest = npc.transform;
                minDist = dist;
            }
        }

        return closest;
    }
}
