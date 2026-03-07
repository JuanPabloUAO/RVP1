using UnityEngine;

public class MenuCameraLook : MonoBehaviour
{
    public float mouseSensitivity = 2f;
    public float verticalLimit = 80f;

    float rotationX = 0f;
    float rotationY = 0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        rotationY += mouseX;
        rotationX -= mouseY;

        rotationX = Mathf.Clamp(rotationX, -verticalLimit, verticalLimit);

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}