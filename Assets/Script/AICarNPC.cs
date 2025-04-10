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
    public float boostedMaxSpeed = 8f;
    private float defaultMaxSpeed;
    private bool isSlowed = false;
    public float slowDuration = 3f;
    
    private bool isWaitingAtStart = true;
    private Rigidbody2D rb;
    private bool isColliding = false;

    [Header("Skill Settings")]
    public GameObject trapBananaPrefab;
    public float skillCooldown = 15f;
    private float skillTimer = 0f;
    private bool canUseSkill = true;
    private float timeSinceStart = 0f;
    public float skillUsageDelay = 30f; // Không xài skill trong 30 giây đầu

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
            GameObject trapbanana = Instantiate(trapBananaPrefab, spawnPosition, Quaternion.identity);
            TrapBanana trapScript = trapbanana.GetComponent<TrapBanana>();
            if (trapScript != null)
            {
                trapScript.owner = this.gameObject;
            }
            StartCoroutine(DestroyTrapAfterTime(trapbanana, 60f));
        }

        canUseSkill = false;
    }

    IEnumerator DestroyTrapAfterTime(GameObject trapbanana, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (trapbanana != null) Destroy(trapbanana);
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.linearVelocity = Vector2.zero; // Dừng di chuyển ngay lập tức
            rb.angularVelocity = 0f; // Ngăn quay vòng
            StartCoroutine(RecoverFromCollision(0.3f)); // Đứng yên khi va chạm Player hoặc NPC
        }
        else if (collision.gameObject.CompareTag("NPC"))
        {
            rb.linearVelocity = Vector2.zero; // Dừng di chuyển ngay lập tức
            rb.angularVelocity = 0f; // Ngăn quay vòng
            StartCoroutine(RecoverFromCollision(0.15f)); // Đứng yên khi va chạm Player hoặc NPC
        }
    }


    IEnumerator RecoverFromCollision(float waitTime)
    {
        isColliding = true;
        yield return new WaitForSeconds(waitTime);
        isColliding = false;
    }
    IEnumerator WaitBeforeStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isWaitingAtStart = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("TrapBanana") && !isStunned)
        {
            TrapBanana trapScript = other.GetComponent<TrapBanana>();
            if (trapScript != null && trapScript.owner == this.gameObject)
            {
                return;
            }

            StartCoroutine(SpinAndStun());
        }
        else if (other.CompareTag("TrapSlow"))
        {
            if (!isSlowed)
            StartCoroutine(SlowDownByTrapSlow());
        }
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
    IEnumerator SlowDownByTrapSlow()
    {
        if (isSlowed) yield break;

        isSlowed = true;

        float originalSpeed = defaultMaxSpeed;
        float originalBoost = boostedMaxSpeed;

        defaultMaxSpeed *= 2f / 3f;
        boostedMaxSpeed *= 2f / 3f;

        yield return new WaitForSeconds(slowDuration);

        defaultMaxSpeed = originalSpeed;
        boostedMaxSpeed = originalBoost;

        isSlowed = false;
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