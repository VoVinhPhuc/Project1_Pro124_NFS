using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera targetCamera;

    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    void LateUpdate()
    {
        transform.forward = targetCamera.transform.forward;
    }
}
