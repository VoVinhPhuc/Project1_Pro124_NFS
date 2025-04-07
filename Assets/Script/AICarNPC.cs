// using UnityEngine;
// using System.Collections;

// public class NPCRacerAI : MonoBehaviour
// {
//     public Transform[] waypoints; // Các điểm waypoint trên đường đua
//     private int currentWaypointIndex = 0;
//     public float speed = 5f;
//     public float maxTurnSpeed = 200f;
//     public float brakeFactor = 0.5f; // Hệ số giảm tốc khi vào cua
//     public float acceleration = 5f; // Gia tốc xe
//     public float turnSensitivity = 5f; // Độ nhạy khi vào cua
    
//     private Rigidbody2D rb;
    
//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//     }

//     void FixedUpdate()
//     {
//         if (waypoints.Length == 0) return;
        
//         Transform targetWaypoint = waypoints[currentWaypointIndex];
//         Vector2 direction = (targetWaypoint.position - transform.position).normalized;
        
//         float angle = Vector2.SignedAngle(transform.up, direction);
        
//         // Giảm tốc khi vào cua
//         float currentSpeed = speed * Mathf.Lerp(1f, brakeFactor, Mathf.Abs(angle) / 90f);
        
//         // Điều chỉnh vận tốc mượt mà
//         rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, transform.up * currentSpeed, Time.deltaTime * acceleration);
        
//         // Xoay xe hướng về waypoint
//         float rotationAmount = Mathf.Clamp(angle * turnSensitivity * Time.deltaTime, -maxTurnSpeed, maxTurnSpeed);
//         rb.MoveRotation(rb.rotation + rotationAmount);
        
//         // Kiểm tra nếu đã đến gần waypoint thì chuyển sang waypoint tiếp theo
//         if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.5f)
//         {
//             currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
//         }
//     }

//     void OnDrawGizmos()
//     {
//         if (waypoints == null || waypoints.Length == 0) return;

//         // Vẽ đường nối giữa các waypoints
//         Gizmos.color = Color.green;
//         for (int i = 0; i < waypoints.Length - 1; i++)
//         {
//             Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
//         }
//         Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);

//         // Vẽ sphere tại các waypoints
//         for (int i = 0; i < waypoints.Length; i++)
//         {
//             Gizmos.color = (i == currentWaypointIndex) ? Color.red : Color.yellow;
//             Gizmos.DrawSphere(waypoints[i].position, 0.3f);
//         }
//     }
//     void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("PlayerBullet"))
//         {
//             Destroy(gameObject);
//         }
//     }
// }

using UnityEngine;
using System.Collections;

public class NPCRacerAI : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    public float speed = 5f;
    public float maxTurnSpeed = 200f;
    public float brakeFactor = 0.5f;
    public float acceleration = 5f;
    public float turnSensitivity = 5f;
    public float collisionForce = 5f; // Lực đẩy lùi khi va chạm
    public float avoidanceForce = 3f; // Lực tránh va chạm
    public float rotationSpeed = 360f;
    private bool isSpinning = false;
    private bool isStunned = false;
    
    private bool isWaitingAtStart = true;
    private Rigidbody2D rb;
    private bool isColliding = false;

    [Header("Skill Settings")]
    public GameObject trapBananaPrefab;
    private float timeSinceStart = 0f;
    public float skillUsageDelay = 30f; // Không xài skill trong 30 giây đầu

    public float skillCooldown = 15f;
    private float skillTimer = 0f;
    private bool canUseSkill = true;

    [Header("NPC Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;
    public float respawnDelay = 3f;
    private Vector2 spawnPoint;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(WaitBeforeStart(4f)); // NPC đứng yên 4 giây

        skillTimer = skillCooldown;
        currentHealth = maxHealth;
        spawnPoint = transform.position;
    }
    void Update()
    {
        if (isWaitingAtStart || isDead) return;
        timeSinceStart += Time.deltaTime;

        HandleSkillAutoUse();
    }

    void FixedUpdate()
    {
        if (waypoints.Length == 0 || isColliding || isWaitingAtStart) return;
        
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector2 direction = (targetWaypoint.position - transform.position).normalized;
        
        float angle = Vector2.SignedAngle(transform.up, direction);
        float currentSpeed = speed * Mathf.Lerp(1f, brakeFactor, Mathf.Abs(angle) / 90f);
        
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, transform.up * currentSpeed, Time.deltaTime * acceleration);
        
        float rotationAmount = Mathf.Clamp(angle * turnSensitivity * Time.deltaTime, -maxTurnSpeed, maxTurnSpeed);
        rb.MoveRotation(rb.rotation + rotationAmount);
        
        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
    void HandleSkillAutoUse()
    {
        if (timeSinceStart < skillUsageDelay) return;

        if (canUseSkill)
        {
            UseSkill();
        }
        else
        {
            skillTimer -= Time.deltaTime;
            if (skillTimer <= 0f)
            {
                canUseSkill = true;
                skillTimer = skillCooldown;
            }
        }
    }
    void UseSkill()
    {
        if (trapBananaPrefab != null)
        {
            Vector3 spawnPosition = transform.position - transform.up * 1.5f;
            GameObject trap = Instantiate(trapBananaPrefab, spawnPosition, Quaternion.identity);
            StartCoroutine(DestroyTrapAfterTime(trap, 90f));
        }

        canUseSkill = false;
    }

    IEnumerator DestroyTrapAfterTime(GameObject trap, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (trap != null) Destroy(trap);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
<<<<<<< Updated upstream
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Player"))
        {
            isColliding = true;
            Vector2 pushDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(pushDirection * collisionForce, ForceMode2D.Impulse);
            StartCoroutine(RecoverFromCollision());
=======
        
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("NPC"))
        {
            rb.linearVelocity = Vector2.zero; // Dừng di chuyển ngay lập tức
            rb.angularVelocity = 0f; // Ngăn quay vòng
            StartCoroutine(RecoverFromCollision(0.3f)); // Đứng yên khi va chạm Player hoặc NPC
>>>>>>> Stashed changes
        }
    }

    IEnumerator RecoverFromCollision()
    {
        yield return new WaitForSeconds(0.5f);
        isColliding = false;
    }
    IEnumerator WaitBeforeStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isWaitingAtStart = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("TrapBanana") && !isStunned)
        {
            // rb.linearVelocity = Vector2.zero;
            // StartCoroutine(RecoverFromCollision(2f)); // Đứng yên 2s khi va chạm "Trap"
            StartCoroutine(SpinAndStun());
        }
    }
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            StartCoroutine(RespawnNPC());
        }
    }
    IEnumerator RespawnNPC()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnDelay);

        transform.position = spawnPoint;
        currentHealth = maxHealth;
        gameObject.SetActive(true);
        isDead = false;
    }
    IEnumerator SpinAndStun()
    {
        isStunned = true;
        //movementScript.enabled = false; // Tắt điều khiển xe
        rb.linearVelocity = Vector2.zero;
        float timer = 0f;

        while (timer < 3f)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        //movementScript.enabled = true;
        isStunned = false;
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
        Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);

        for (int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.color = (i == currentWaypointIndex) ? Color.red : Color.yellow;
            Gizmos.DrawSphere(waypoints[i].position, 0.5f);
        }
    }
}

