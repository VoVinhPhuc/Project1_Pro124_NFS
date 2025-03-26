using System.Collections;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float acceleration = 10f;
    public float maxSpeed = 15f;
    public float turnSpeed = 200f;
    public float driftFactor = 0.95f;
    public float nitroBoost = 1.5f;
    public float brakeForce = 0.5f;
    public float skillCooldown = 3f;

    private Rigidbody2D rb;
    private float moveInput;
    private float turnInput;
    private bool isDrifting;
    private bool isNitro;
    private bool canUseSkill = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
        isNitro = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKey(KeyCode.Space))
            rb.linearVelocity *= brakeForce;

        HandleDrift();

        if (Input.GetKeyDown(KeyCode.F) && canUseSkill)
            StartCoroutine(UseSkill());
    }

    void FixedUpdate()
    {
        ApplyEngineForce();
        ApplySteering();
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

    IEnumerator UseSkill()
    {
        canUseSkill = false;
        Debug.Log("Skill Used!");
        yield return new WaitForSeconds(skillCooldown);
        canUseSkill = true;
    }
}