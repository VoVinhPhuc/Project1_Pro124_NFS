using UnityEngine;

public class Cartest : MonoBehaviour
{
    // Tốc độ di chuyển về phía trước và lùi
    public float moveSpeed = 10f;
    // Tốc độ quay của xe (độ/phút hoặc đơn vị tùy chỉnh)
    public float turnSpeed = 100f;

    void Update()
    {
        // Lấy giá trị đầu vào từ phím (hoặc bộ điều khiển mặc định)
        float moveInput = Input.GetAxis("Vertical");     // Nhấn Up (hoặc W) tăng, Down (hoặc S) giảm
        float turnInput = Input.GetAxis("Horizontal");     // Nhấn Right (hoặc D) tăng, Left (hoặc A) giảm

        // Di chuyển xe theo hướng 'up' của đối tượng (tức là hướng đang nhìn)
        transform.Translate(Vector3.up * moveInput * moveSpeed * Time.deltaTime, Space.Self);

        // Quay xe quanh trục Z (trong game 2D top-down, xe quay quanh trục Z)
        transform.Rotate(0, 0, -turnInput * turnSpeed * Time.deltaTime);
    }
}
