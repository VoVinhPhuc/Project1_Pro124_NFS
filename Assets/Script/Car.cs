using System.Collections;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float acceleration = 5f; // 5
    public float maxSpeed = 7f; // 7 
    public float turnSpeed = 120f; // 120 
    public float driftFactor = 120f; // 120 
    public float nitroBoost = 10f; // 10 
    public float brakeForce = 1f; // 1 
    public float skillCooldown = 3f; // 3
    public float stopThreshold = 10f; // 10 

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
        {
            rb.linearVelocity *= brakeForce;
        }

        HandleDrift();

        if (Input.GetKeyDown(KeyCode.F) && canUseSkill)
        {
            StartCoroutine(UseSkill());
        }
            
    }

    void FixedUpdate()
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
        Debug.Log("Skill Used!");
        yield return new WaitForSeconds(skillCooldown);
        canUseSkill = true;
    }
}