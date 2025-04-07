using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Car : MonoBehaviour
{
    [Header("Move Settings")]
    public float acceleration = 5f;
    public float maxSpeed = 7f;
    public float turnSpeed = 120f;
    public float driftFactor = 120f;
    public float brakeForce = 1f;
    public float stopThreshold = 10f;

    [Header("Trap")]
    private bool isSlowed = false;
    public float slowDuration = 3f;

    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    public TMP_Text healthText;
    public float respawnDelay = 2f;
    private bool isKnockedBack = false;


    [Header("Checkpoint Settings")]
    public GameObject checkpointPrefab;
    public float checkpointSpacing = 10f;
    public float checkpointCooldown = 10f;

    private Transform lastCheckpoint;
    private Vector2 lastCheckpointPos;
    private float lastCheckpointTime = 0f;
    private float distanceSinceLastCheckpoint = 0f;


    [Header("Skill Settings")]
    public float skillCooldown = 20f;
    public Image skillImage;
    public TMP_Text cooldownText;
    public GameObject trapBananaPrefab;
    private bool canUseSkill = true;

    [Header("Boost Settings")]
    public float boostedMaxSpeed = 8f;
    private float defaultMaxSpeed;

    [Header("Skill Slider")]
    public Slider skillSlider;
    public float sliderDecreaseSpeed = 20f;
    public float sliderRecoverRate = 5f;
    public Image sliderFillImage;
    public Color normalColor = Color.green;
    public Color warningColor = Color.red;
    public float warningThreshold = 20f;

    // Internal Variables
    private Rigidbody2D rb;
    private float moveInput;
    private float turnInput;
    private bool isDrifting;
    private bool isWaitingAtStart = true;
    private bool isFlashing = false;
    private Coroutine flashCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultMaxSpeed = maxSpeed;
        currentHealth = maxHealth;
        UpdateHealthUI();

        SpawnCheckpoint(transform.position);
        lastCheckpointPos = transform.position;

        StartCoroutine(WaitBeforeStart(4f));
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity *= brakeForce;
        }

        HandleDrift();

        if (Input.GetKeyDown(KeyCode.F) && canUseSkill)
        {
            StartCoroutine(UseSkill());
            SpawnTrapBanana();
        }

        HandleSkillSlider();

        maxSpeed = Input.GetKey(KeyCode.LeftShift) ? boostedMaxSpeed : defaultMaxSpeed;
    }


    void FixedUpdate()
    {
        if (isWaitingAtStart) return;

        ApplyEngineForce();
        ApplySteering();
        ApplyStop();

        HandleAutoCheckpoint();
    }

    void ApplyEngineForce()
    {
        float currentSpeed = rb.linearVelocity.magnitude;
        if (currentSpeed < maxSpeed || moveInput < 0)
        {
            rb.AddForce(transform.up * moveInput * acceleration);
        }
    }

    void ApplySteering()
    {
<<<<<<< Updated upstream
        float minSpeedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
        rb.angularVelocity = -turnInput * turnSpeed * minSpeedFactor;
    }

    void HandleDrift()
    {
        if (Input.GetKey(KeyCode.W) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }

        if (isDrifting)
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, transform.up * rb.linearVelocity.magnitude, driftFactor * Time.deltaTime);
    }
=======
        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
        float steeringFactor = Mathf.Lerp(1f, 0.5f, speedFactor);
        rb.angularVelocity = -turnInput * turnSpeed * steeringFactor;
    }

>>>>>>> Stashed changes
    void ApplyStop()
    {
        if (moveInput == 0 && rb.linearVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    void HandleDrift()
    {
        if (moveInput != 0)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                isDrifting = true;
            }
            else
            {
                isDrifting = false;
            }
        }
        else
        {
            isDrifting = false;
        }

        if (moveInput < 0)
        {
            if (Input.GetKey(KeyCode.A))
            {
                rb.angularVelocity = turnSpeed * 1.5f;
                isDrifting = true;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rb.angularVelocity = -turnSpeed * 1.5f;
                isDrifting = true;
            }
        }

        if (isDrifting)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, transform.up * rb.linearVelocity.magnitude, driftFactor * Time.deltaTime);
        }
    }

    void HandleSkillSlider()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            skillSlider.value -= sliderDecreaseSpeed * Time.deltaTime;
        }
        else if (skillSlider.value < skillSlider.maxValue)
        {
            skillSlider.value += sliderRecoverRate * Time.deltaTime;
            skillSlider.value = Mathf.Min(skillSlider.value, skillSlider.maxValue);
        }

        skillSlider.value = Mathf.Max(skillSlider.value, skillSlider.minValue);

        if (sliderFillImage != null)
        {
            if (skillSlider.value <= warningThreshold)
            {
                if (!isFlashing)
                {
                    flashCoroutine = StartCoroutine(FlashWarningColor());
                    isFlashing = true;
                }
            }
            else
            {
                if (isFlashing)
                {
                    StopCoroutine(flashCoroutine);
                    sliderFillImage.color = normalColor;
                    isFlashing = false;
                }
            }
        }
    }
    void HandleAutoCheckpoint()
    {
        float distanceMoved = Vector2.Distance(rb.position, lastCheckpointPos);

    // Nếu di chuyển đủ xa và đã qua thời gian cooldown
        if (distanceMoved >= checkpointSpacing && Time.time - lastCheckpointTime >= checkpointCooldown)
        {
            SpawnCheckpoint(rb.position);
            lastCheckpointPos = rb.position;
            lastCheckpointTime = Time.time;
        }
    }


    void SpawnCheckpoint(Vector2 position)
    {
        if (checkpointPrefab != null)
        {
            GameObject checkpoint = Instantiate(checkpointPrefab, position, Quaternion.identity);
            checkpoint.tag = "Checkpoint";

            // Đảm bảo có collider và trigger
            Collider2D col = checkpoint.GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }
    }



    void UpdateHealthUI()
    {
        if (healthText != null)
        healthText.text = currentHealth.ToString();
    }

    void TakeDamage()
    {
        if (isKnockedBack || isWaitingAtStart) 
            return;

        currentHealth--;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            StartCoroutine(HandleRespawn());
        }
    }
    IEnumerator HandleRespawn()
    {
        isKnockedBack = true;

        if (lastCheckpoint != null)
        {
            rb.position = lastCheckpoint.position;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        yield return new WaitForSeconds(respawnDelay);

        currentHealth = maxHealth;
        UpdateHealthUI();
        isKnockedBack = false;
    }


    IEnumerator FlashWarningColor()
    {
        while (true)
        {
            sliderFillImage.color = warningColor;
            yield return new WaitForSeconds(0.2f);
            sliderFillImage.color = normalColor;
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator UseSkill()
    {
        canUseSkill = false;
        Debug.Log("Skill Used!");

        if (skillImage != null)
        {
            var color = skillImage.color;
            color.a = 0.3f;
            skillImage.color = color;
        }

        float remaining = skillCooldown;
        while (remaining > 0f)
        {
            if (cooldownText != null)
                cooldownText.text = Mathf.CeilToInt(remaining).ToString();
            remaining -= Time.deltaTime;
            yield return null;
        }

        if (cooldownText != null)
            cooldownText.text = "";

        if (skillImage != null)
        {
            var color = skillImage.color;
            color.a = 1f;
            skillImage.color = color;
        }

        canUseSkill = true;
    }

    IEnumerator SlowDownTemporarily()
    {
        isSlowed = true;

        float originalSpeed = defaultMaxSpeed;
        defaultMaxSpeed = originalSpeed / 2f;

        // Nếu đang boost, giới hạn luôn cả tốc độ boost
        float originalBoost = boostedMaxSpeed;
        boostedMaxSpeed = boostedMaxSpeed / 2f;

        yield return new WaitForSeconds(slowDuration);

        // Khôi phục tốc độ
        defaultMaxSpeed = originalSpeed;
        boostedMaxSpeed = originalBoost;
        isSlowed = false;
    }

    void SpawnTrapBanana()
    {
        if (trapBananaPrefab != null)
        {
            Vector3 spawnPosition = transform.position - transform.up * 1.5f;
            GameObject trap = Instantiate(trapBananaPrefab, spawnPosition, Quaternion.identity);
            StartCoroutine(DestroyTrapAfterTime(trap, 90f));
        }
    }

    IEnumerator DestroyTrapAfterTime(GameObject trap, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (trap != null) Destroy(trap);
    }

    IEnumerator WaitBeforeStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isWaitingAtStart = false;
    }

    



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            TakeDamage();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            lastCheckpoint = other.transform;
        }
        else if (other.CompareTag("Trap"))
        {
            if (!isSlowed)
                StartCoroutine(SlowDownTemporarily());
        }
    }


}
