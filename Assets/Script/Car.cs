using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Car : MonoBehaviour
{
    [Header("Movement Settings")]
    public float acceleration = 5f;
    public float maxSpeed = 7f;
    public float boostedMaxSpeed = 8f;
    public float turnSpeed = 120f;
    public float driftFactor = 120f;
    public float nitroBoost = 10f;
    public float brakeForce = 1f;
    public float stopThreshold = 10f;

    [Header("Skill Settings")]
    public float skillCooldown = 3f;
    private bool canUseSkill = true;
    public GameObject trapBananaPrefab;
    public Image skillImage;
    public TMP_Text cooldownText;

    [Header("Skill Slider")]
    public Slider skillSlider;
    public float sliderDecreaseSpeed = 20f;
    public float sliderRecoverRate = 5f;
    public Image sliderFillImage;
    public Color normalColor = Color.green;
    public Color warningColor = Color.red;
    public float warningThreshold = 20f;
    private bool isFlashing = false;
    private Coroutine flashCoroutine;

    [Header("Audio Settings")]
    private AudioSource engineAudioSource;
    public AudioClip engineIdleClip;
    public AudioClip engineAccelerateClip;

    [Header("Hit Settings")]
    public GameObject hitEffectPrefab;
    public Slider healthSlider;
    private float health = 1f;
    private Coroutine hideHealthBarCoroutine;
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioClip explosionSound;

    [Header("Checkpoint Settings")]
    public float checkpointDistance = 10f;  // Khoảng cách tối thiểu để tạo một checkpoint mới
    private Vector2 lastCheckpointPosition;  // Vị trí checkpoint trước đó
    private List<Transform> checkpointsList = new List<Transform>();

    public float rotationSpeed = 360f; // tốc độ xoay
    private bool isSpinning = false;
    private bool isStunned = false;
    private Car movementScript;

    private Rigidbody2D rb;
    private float moveInput;
    private float turnInput;
    private bool isDrifting;
    private bool isNitro;
    private float defaultMaxSpeed;
    private bool isWaitingAtStart = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        engineAudioSource = GetComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        movementScript = GetComponent<Car>();
        defaultMaxSpeed = maxSpeed;

        lastCheckpointPosition = transform.position;

        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(true);
            healthSlider.value = health;
        }
        StartCoroutine(WaitBeforeStart(4f));
    }

    void Update()
    {
        if (!TrafficLightController.canStartRace || isWaitingAtStart)
            return;

        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
        isNitro = Input.GetKey(KeyCode.LeftShift);

        HandleEngineSound();
        HandleDrift();
        HandleSkillSlider();

        if (Vector2.Distance(transform.position, lastCheckpointPosition) >= checkpointDistance)
        {
            CreateCheckpoint();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity *= brakeForce;
        }

        if (Input.GetKeyDown(KeyCode.F) && canUseSkill)
        {
            StartCoroutine(UseSkill());
            SpawnTrapBanana();
        }

        maxSpeed = isNitro ? boostedMaxSpeed : defaultMaxSpeed;
    }

    void FixedUpdate()
    {
        if (isWaitingAtStart) return;

        ApplyEngineForce();
        ApplySteering();
        ApplyStop();
    }

    void ApplyEngineForce()
    {
        float currentSpeed = rb.linearVelocity.magnitude;
        if (currentSpeed < maxSpeed || moveInput < 0)
        {
            float speedFactor = isNitro ? nitroBoost : 1f;
            rb.AddForce(transform.up * moveInput * acceleration * speedFactor);
        }
    }

    void ApplySteering()
    {
        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
        float steeringFactor = Mathf.Lerp(1f, 0.5f, speedFactor);
        rb.angularVelocity = -turnInput * turnSpeed * steeringFactor;
    }

    void HandleDrift()
    {
        if (moveInput > 0 && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            isDrifting = true;
        }
        else if (moveInput < 0)
        {
            if (Input.GetKey(KeyCode.A))
            {
                isDrifting = true;
                rb.angularVelocity = turnSpeed * 1.5f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                isDrifting = true;
                rb.angularVelocity = -turnSpeed * 1.5f;
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

        if (isDrifting)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, transform.up * rb.linearVelocity.magnitude, driftFactor * Time.deltaTime);
        }
    }

    void ApplyStop()
    {
        if (moveInput == 0 && rb.linearVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    IEnumerator UseSkill()
    {
        canUseSkill = false;

        if (skillImage != null)
        {
            Color color = skillImage.color;
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
            Color color = skillImage.color;
            color.a = 1f;
            skillImage.color = color;
        }

        canUseSkill = true;
    }

    void SpawnTrapBanana()
    {
        if (trapBananaPrefab != null)
        {
            Vector3 spawnPosition = transform.position - transform.up * 1.5f;
            GameObject trap = Instantiate(trapBananaPrefab, spawnPosition, Quaternion.identity);
            StartCoroutine(DestroyTrapAfterTime(trap, 30f));
        }
    }

    IEnumerator DestroyTrapAfterTime(GameObject trap, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (trap != null) Destroy(trap);
    }

    void HandleSkillSlider()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            skillSlider.value -= sliderDecreaseSpeed * Time.deltaTime;
        }
        else
        {
            if (skillSlider.value < skillSlider.maxValue)
            {
                skillSlider.value += sliderRecoverRate * Time.deltaTime;
                skillSlider.value = Mathf.Min(skillSlider.value, skillSlider.maxValue);
            }
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
            else if (isFlashing)
            {
                StopCoroutine(flashCoroutine);
                sliderFillImage.color = normalColor;
                isFlashing = false;
            }
        }
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

    void HandleEngineSound()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (engineAudioSource.clip != engineAccelerateClip)
            {
                engineAudioSource.clip = engineAccelerateClip;
                engineAudioSource.loop = true;
                engineAudioSource.Play();
            }
        }
        else
        {
            if (engineAudioSource.clip != engineIdleClip)
            {
                engineAudioSource.clip = engineIdleClip;
                engineAudioSource.loop = true;
                engineAudioSource.Play();
            }
        }
    }

    IEnumerator WaitBeforeStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isWaitingAtStart = false;
    }

    void OnTriggerEnter2D(Collider2D other)
{
    Bullet bullet = other.GetComponent<Bullet>();
    if (bullet != null)
    {
        if (bullet.shooter == gameObject) return; // Né đạn của chính mình

        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        if (hitSound != null && audioSource != null)
            audioSource.PlayOneShot(hitSound);

        TakeDamage(0.1f);
        Destroy(other.gameObject);
        return;
    }

    if (other.CompareTag("TrapBanana") && !isStunned)
    {
        StartCoroutine(SpinAndStun());
    }
}
    void CreateCheckpoint()
    {
        // Tạo một checkpoint mới tại vị trí hiện tại của xe
        GameObject checkpoint = new GameObject("Checkpoint");
        checkpoint.transform.position = transform.position;

        // Thêm checkpoint mới vào danh sách
        checkpointsList.Add(checkpoint.transform);

        // Cập nhật vị trí checkpoint cuối cùng
        lastCheckpointPosition = transform.position;

        // Debug log để kiểm tra các checkpoint đã tạo
        Debug.Log("Checkpoint created at: " + transform.position);
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
                StopCoroutine(hideHealthBarCoroutine);

            //hideHealthBarCoroutine = StartCoroutine(HideHealthBarAfterDelay());
        }

        if (health <= 0f)
        {
            if (explosionSound != null)
                audioSource.PlayOneShot(explosionSound);

            if (explosionEffect != null)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            RespawnAtCheckpoint();
        }
    }
    void RespawnAtCheckpoint()
    {
        if (checkpointsList.Count > 0)
        {
            Transform lastCheckpoint = checkpointsList[checkpointsList.Count - 1];
            transform.position = lastCheckpoint.position;  // Di chuyển xe đến checkpoint gần nhất
            health = 1f;  // Đặt lại sức khỏe
            if (healthSlider != null)
                healthSlider.value = health;  // Cập nhật lại thanh máu
        }
    }

    // IEnumerator HideHealthBarAfterDelay()
    // {
    //     yield return new WaitForSeconds(3f);
    //     if (healthSlider != null)
    //     {
    //         healthSlider.gameObject.SetActive(false);
    //     }
    // }
    IEnumerator SpinAndStun()
    {
        isStunned = true;
        movementScript.enabled = false; // Tắt điều khiển xe
        rb.linearVelocity = Vector2.zero;
        float timer = 0f;

        while (timer < 3f)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        movementScript.enabled = true;
        isStunned = false;
    }
}
