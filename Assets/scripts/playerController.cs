using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerController : MonoBehaviour {

    Camera mainCamera;
    public float mouseSensitivity = 1f;
    public CursorLockMode cursorState = CursorLockMode.None;
    public float movAccel = 5f;
    public float jumpAccel = 50f;
    Rigidbody rb;

    void Awake()
    {
        mainCamera = transform.Find("playerCamera").GetComponent<Camera>();
        Cursor.lockState = cursorState;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        /*float cameraXRot = mainCamera.transform.localRotation.eulerAngles.x;
        float mainCameraEulerFix;

        if (cameraXRot >= 270)
            mainCameraEulerFix = cameraXRot - 360;
        else
            mainCameraEulerFix = cameraXRot;

        transform.rotation = Quaternion.Euler (0, transform.rotation.eulerAngles.y + mouseDelta.x, 0);
        mainCamera.transform.localRotation = Quaternion.Euler(Mathf.Clamp((-mainCameraEulerFix + mouseDelta.y) * -1, -90, 90), 0, 0);*/
    }

    void FixedUpdate()
    {
       /* int jump = 0;

        if (axis2Bool("Jump")) 
        {
            if (Physics.Raycast (new Vector3 (transform.position.x, Mathf.Round(transform.position.y * 100) / 100 - GetComponent<CapsuleCollider> ().height / 2, transform.position.z), Vector3.down, 0.01f, ~(1 << LayerMask.NameToLayer("Player"))))
                jump = 1;
        }

        rb.AddRelativeForce(Vector3.Scale(new Vector3 (Input.GetAxisRaw ("Horizontal"), jump, Input.GetAxisRaw ("Vertical")).normalized, new Vector3(movAccel * rb.mass, jumpAccel, movAccel * rb.mass)));*/

        float cameraXRot = mainCamera.transform.localRotation.eulerAngles.x;
        float mainCameraEulerFix;

        if (cameraXRot >= 270)
            mainCameraEulerFix = cameraXRot - 360;
        else
            mainCameraEulerFix = cameraXRot;

        Vector2 mouseDelta = mouseMovement() * mouseSensitivity;

        rb.MoveRotation(Quaternion.Euler(0, rb.rotation.eulerAngles.y + mouseDelta.x, 0));
        mainCamera.transform.localRotation = Quaternion.Euler(Mathf.Clamp((-mainCameraEulerFix + mouseDelta.y) * -1, -90, 90), 0, 0);
    }

    public Vector2 mouseMovement()
    {
        return new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));
    }

    public bool axis2Bool(string axisName)
    {
        if (Input.GetAxisRaw (axisName) == 1)
            return true;
        else
            return false;
    }
}
