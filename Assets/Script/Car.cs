using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

    // void ApplySteering()
    // {
    //     float minSpeedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
    //     rb.angularVelocity = -turnInput * turnSpeed * minSpeedFactor;
    // }

    void ApplySteering()
{
    // Tạo một hệ số giảm tốc độ khi xe di chuyển chậm
    float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / maxSpeed);
    // Thêm một hệ số ma sát để xe quay mượt mà hơn
    float steeringFactor = Mathf.Lerp(1f, 0.5f, speedFactor); // Tốc độ càng thấp thì quay càng dễ

    rb.angularVelocity = -turnInput * turnSpeed * steeringFactor;
}


  void HandleDrift()
{
    if (moveInput > 0) // Khi di chuyển về phía trước (W)
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) // Nhấn A hoặc D để drift
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }
    }
    else if (moveInput < 0) // Khi di chuyển lùi (S)
    {
        if (Input.GetKey(KeyCode.A)) // Nhấn A để drift sang trái khi lùi
        {
            isDrifting = true;
            rb.angularVelocity = turnSpeed * 1.5f; // Quay trái nhanh hơn khi lùi
        }
        else if (Input.GetKey(KeyCode.D)) // Nhấn D để drift sang phải khi lùi
        {
            isDrifting = true;
            rb.angularVelocity = -turnSpeed * 1.5f; // Quay phải nhanh hơn khi lùi
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
        // Điều chỉnh tốc độ drift
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
        Debug.Log("Skill Used!");
        yield return new WaitForSeconds(skillCooldown);
        canUseSkill = true;
    }
}