using System.Collections;
using UnityEngine;

public class CarNPC : MonoBehaviour
{
    public float acceleration = 5f;
    public float maxSpeed = 7f;
    public float turnSpeed = 120f;
    public float driftFactor = 120f;
    public float nitroBoost = 10f;
    public float brakeForce = 1f;
    public float skillCooldown = 3f;
    public float stopThreshold = 10f; // Ngưỡng để xe dừng hẳn

    protected Rigidbody2D rb;
    protected float moveInput;
    protected float turnInput;
    protected bool isDrifting;
    protected bool isNitro;
    private bool canUseSkill = true;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        HandleDrift();
    }

    protected virtual void FixedUpdate()
    {
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
        float minSpeedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
        rb.angularVelocity = -turnInput * turnSpeed * minSpeedFactor;
    }

    void HandleDrift()
    {
        if (moveInput > 0 && Mathf.Abs(turnInput) > 0.5f)
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

    void ApplyStop()
    {
        if (moveInput == 0 && rb.linearVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}

