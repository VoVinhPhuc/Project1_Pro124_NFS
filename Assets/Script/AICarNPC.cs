using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NPCRacerAI : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    public float speed = 5f;
    public float maxTurnSpeed = 200f;
    public float brakeFactor = 0.5f;
    public float acceleration = 5f;
    public float turnSensitivity = 5f;
    public float collisionForce = 5f;
    public float avoidanceForce = 3f;
    public float rotationSpeed = 360f;
    private float timeSinceStart = 0f;
    public float skillUsageDelay = 30f; // Không xài skill trong 30 giây đầu
    [Header("Skill Settings")]
    public GameObject trapBananaPrefab;
    public float skillCooldown = 15f;
    private float skillTimer = 0f;
    private bool canUseSkill = true;
    private bool isDead = false;
    private Vector2 spawnPoint;
    public float respawnDelay = 3f;

    private Rigidbody2D rb;
    private bool isColliding = false;
    private bool isWaitingAtStart = true;
    private bool isStunned = false;

    public GameObject hitEffectPrefab;
    public Slider healthSlider;
    private float health = 1f;
    private Coroutine hideHealthBarCoroutine;

    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioClip explosionSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        skillTimer = skillCooldown;
        spawnPoint = transform.position;

        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false);
            healthSlider.value = health;
        }

        StartCoroutine(WaitBeforeStart(4f)); // Đứng yên tại vạch xuất phát 4 giây
    }
    void Update()
    {
        if (isWaitingAtStart || isDead) return;

        timeSinceStart += Time.deltaTime;

        HandleSkillAutoUse();
    }

    void FixedUpdate()
    {
        if (!TrafficLightController.canStartRace || waypoints.Length == 0 || isColliding || isWaitingAtStart || isStunned)
            return;

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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("NPC"))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            Vector2 pushDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(pushDirection * collisionForce, ForceMode2D.Impulse);
            StartCoroutine(RecoverFromCollision(0.5f));
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
        if (other.CompareTag("PlayerBullet"))
        {
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            if (hitSound != null)
                audioSource.PlayOneShot(hitSound);

            TakeDamage(0.1f);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("TrapBanana") && !isStunned)
        {
            StartCoroutine(SpinAndStun());
        }
    }

    IEnumerator SpinAndStun()
    {
        isStunned = true;
        rb.linearVelocity = Vector2.zero;

        float timer = 0f;
        while (timer < 3f)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isStunned = false;
    }

    void TakeDamage(float damagePercent)
    {
        health -= damagePercent;
        health = Mathf.Clamp01(health);

        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(true);
            healthSlider.value = health;

            if (hideHealthBarCoroutine != null)
            {
                StopCoroutine(hideHealthBarCoroutine);
            }

            hideHealthBarCoroutine = StartCoroutine(HideHealthBarAfterDelay());
        }

        if (health <= 0f)
        {
            if (explosionSound != null)
                audioSource.PlayOneShot(explosionSound);

            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    IEnumerator HideHealthBarAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false);
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
            StartCoroutine(DestroyTrapAfterTime(trap, 60f));
        }

        canUseSkill = false;
    }
    IEnumerator DestroyTrapAfterTime(GameObject trap, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (trap != null) Destroy(trap);
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
            Gizmos.DrawSphere(waypoints[i].position, 0.3f);
        }
    }
}
