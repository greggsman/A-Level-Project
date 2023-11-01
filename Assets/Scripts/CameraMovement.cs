using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;
    private void Update()
    {
        Vector3 movementVector = new Vector3(0f, 0f, 0f);

        if (Input.GetKey(KeyCode.W)) { movementVector += transform.forward; }
        if (Input.GetKey(KeyCode.A)) { movementVector += -transform.right; }
        if (Input.GetKey(KeyCode.S)) { movementVector += -transform.forward; }
        if (Input.GetKey(KeyCode.D)) { movementVector += transform.right; }

        transform.position += movementVector * movementSpeed * Time.deltaTime;

        float yRotation = 0;
        float xRotation = 0;

        if (Input.GetKey(KeyCode.DownArrow)) { yRotation = rotationSpeed; }
        if (Input.GetKey(KeyCode.UpArrow)) { yRotation = -rotationSpeed; }
        if (Input.GetKey(KeyCode.RightArrow)) { xRotation = rotationSpeed; }
        if (Input.GetKey(KeyCode.LeftArrow)) { xRotation = -rotationSpeed; }

        transform.localEulerAngles += new Vector3(Mathf.Clamp(yRotation, -90, 90), xRotation, 0f) * Time.deltaTime;
    }
}
    