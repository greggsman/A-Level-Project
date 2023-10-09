using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;
    private void Update()
    {
        Vector3 movementVector = new Vector3(0f, 0f, 0f);

        if (Input.GetKey(KeyCode.W)) { movementVector += Vector3.forward; }
        if (Input.GetKey(KeyCode.A)) { movementVector += Vector3.left; }
        if (Input.GetKey(KeyCode.S)) { movementVector += Vector3.back; }
        if (Input.GetKey(KeyCode.D)) { movementVector += Vector3.right; }

        transform.position += movementVector * movementSpeed * Time.deltaTime;

        float rotation = 0;
        if (Input.GetKey(KeyCode.DownArrow)) { rotation += rotationSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.UpArrow)) { rotation += -1 * rotationSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.RightArrow)) { rotation += rotationSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.UpArrow)) { rotation += -1 * rotationSpeed * Time.deltaTime; }

        transform.localEulerAngles += new Vector3(Mathf.Clamp(rotation, -90, 90), 0f);
    }
}
