using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Cinemachine;
using System.Linq;

public class CarMultiplayer : NetworkBehaviour
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

    private Vector3 otherPos;

    private CinemachineCamera cinemachineCam;
    public List<Transform> spawnPoints;  // Danh sách các điểm spawn cho player
    private static List<Transform> usedSpawnPoints = new List<Transform>();

    void Start()
    {
        if (IsOwner || IsHost) // 🔥 Dành cho cả Host và Client
        {
            cinemachineCam = FindFirstObjectByType<CinemachineCamera>();
            if (cinemachineCam != null)
            {
                cinemachineCam.Follow = transform; // Camera follow Player của Host/Client hiện tại
                cinemachineCam.LookAt = transform;
            }
            if (spawnPoints.Count > 0)
            {
                // Chọn điểm spawn ngẫu nhiên từ danh sách
                Transform selectedSpawnPoint = GetUniqueSpawnPoint();

                // Spawn player tại vị trí của điểm spawn đã chọn
                transform.position = selectedSpawnPoint.position;
                transform.rotation = Quaternion.Euler(0, 0, 90);  // Đảm bảo rotation chính xác

                // Lưu lại điểm spawn đã sử dụng
                usedSpawnPoints.Add(selectedSpawnPoint);
            }
            else
            {
                Debug.LogWarning("No spawn points assigned!");
            }
        }

        rb = GetComponent<Rigidbody2D>();
    }
    Transform GetUniqueSpawnPoint()
    {
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

        // Lọc các điểm spawn chưa được sử dụng
        foreach (Transform usedSpawnPoint in usedSpawnPoints)
        {
            availableSpawnPoints.Remove(usedSpawnPoint);
        }

        // Nếu còn điểm spawn thì chọn ngẫu nhiên 1 điểm từ danh sách
        if (availableSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            return availableSpawnPoints[randomIndex];
        }
        else
        {
            Debug.LogWarning("No available spawn points left.");
            return spawnPoints[0]; // Nếu không còn điểm spawn, spawn lại ở điểm đầu tiên
        }
    }
    //Transform GetRandomSpawnPoint()
    //{
    //    if (spawnPoints.Length == 0)
    //    {
    //        Debug.LogError("No spawn points assigned!");
    //        return null;
    //    }

    //    return spawnPoints[Random.Range(0, spawnPoints.Length)];
    //}
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Nếu Player là Owner, thay đổi vị trí spawn
        if (IsOwner)
        {
            // Đảm bảo rằng khi Player spawn, camera sẽ theo dõi Player này
            cinemachineCam.Follow = transform;
        }
    }
    void Update()
    {
        if (IsOwner)
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
            //if (NetworkManager.Singleton.IsClient) 
            //{
                SyncPlayerPosServerRpc(transform.position);
            //}

        }
        //else
        //{
        //    transform.position = otherPos;
        //}
    }

    [ServerRpc]
    void SyncPlayerPosServerRpc(Vector3 pos, ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return;

        // 🔥 Lưu vị trí nhận được (nếu không phải Host)
        otherPos = pos;

        // 🔥 Gửi vị trí cập nhật đến tất cả Client
        UpdateClientPosClientRpc(pos, rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    void UpdateClientPosClientRpc(Vector3 pos, ulong senderClientId)
    {
        if (IsOwner) return; // Không cần update chính mình

        if (NetworkManager.Singleton.LocalClientId != senderClientId)
        {
            transform.position = pos; // Cập nhật vị trí từ Server
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