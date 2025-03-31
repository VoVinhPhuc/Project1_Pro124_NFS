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
    
    private Rigidbody2D rb;
    private bool isColliding = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (waypoints.Length == 0 || isColliding) return;
        
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
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Player"))
        {
            isColliding = true;
            Vector2 pushDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(pushDirection * collisionForce, ForceMode2D.Impulse);
            StartCoroutine(RecoverFromCollision());
        }
    }

    IEnumerator RecoverFromCollision()
    {
        yield return new WaitForSeconds(0.5f);
        isColliding = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            Destroy(gameObject);
        }
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

