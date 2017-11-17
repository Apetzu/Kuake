using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class playerController : NetworkBehaviour {

    public float mouseSensitivity = 5;
    public float movSpeed = 4;
    public float sprintSpeedAdd = 4;
    public float smoothSpeed = 0.6f;
    public float jumpForce = 300;
    public float fireRate = 0.5f;
    public float groundDetectionDist = 0.2f;
    public bool isThisLocalPlayer = false;
    public CursorLockMode cursorState = CursorLockMode.None;

    double lastShot = 0.0;
    Vector3 currentVelocity;
    bool jumping = false;

    public Camera playerCamera;
    public GameObject rocketPrefab;
    public Transform rocketLauncher;
    public GameObject HUD;
    public GameObject shades;
    Rigidbody rb;
    MeshRenderer playerModel;
    CapsuleCollider playerCollider;

    void Awake()
    {
        Cursor.lockState = cursorState;
        rb = GetComponent<Rigidbody>();
        playerModel = GetComponent<MeshRenderer>();
        playerCollider = GetComponent<CapsuleCollider>();
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            if (Input.GetButton("Fire"))
            {
                CmdFire();
            }

            if (Input.GetButton("Menu"))
            {
                transform.position = new Vector3(0, 5, 0);
            }

            Vector2 mouseDelta = mouseMovement() * mouseSensitivity;
            Vector3 deltaPos = Vector3.Normalize(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"))) * (movSpeed + Input.GetAxisRaw("Sprint") * sprintSpeedAdd);

            rb.MovePosition(Vector3.SmoothDamp(rb.position, rb.position + transform.TransformDirection(deltaPos), ref currentVelocity, smoothSpeed, Mathf.Infinity, Time.deltaTime));

            if (Input.GetButton("Jump") && jumping == false)
            {
                if (playerGrounded())
                {
                    jumping = true;
                    rb.velocity = new Vector3 (rb.velocity.x, 0f, rb.velocity.z);
                    rb.AddForce(Vector3.up * jumpForce);
                }
            }
            else if (!Input.GetButton("Jump"))
            {
                jumping = false;
            }

            rb.MoveRotation(Quaternion.Euler(0, rb.rotation.eulerAngles.y + mouseDelta.x, 0));
            playerCamera.transform.localRotation = Quaternion.Euler(strangeAxisClamp(-mouseDelta.y + playerCamera.transform.localRotation.eulerAngles.x, 90, 270), 0, 0);
        }
    }

    public override void OnStartLocalPlayer()
    {
        playerCamera.gameObject.SetActive(true);
        HUD.SetActive(true);
        shades.SetActive(false);
        playerModel.enabled = false;
        isThisLocalPlayer = true;
    }

    [Command]
    public void CmdFire()
    {
        if (Time.time >= fireRate + lastShot)
        {
            // Create the Bullet from the Bullet Prefab
            GameObject rocket = Instantiate(rocketPrefab, rocketLauncher.position, rocketLauncher.rotation);

            // Spawn rocket
            NetworkServer.Spawn(rocket);
            lastShot = Time.time;

            // Destroy the bullet after 2 seconds
            Destroy(rocket, 2.0f);
        }
    }

    public float strangeAxisClamp(float value, float limit1, float limit2)
    {
        if (value > limit1 && value < 180)
            value = limit1;
        else if (value > 180 && value < limit2)
            value = limit2;
        return value;
    }

    public Vector2 mouseMovement()
    {
        return new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));
    }

    public bool playerGrounded()
    {
        Vector3 rayPos = new Vector3(transform.position.x, transform.position.y - playerCollider.height / 2 + 0.2f, transform.position.z);

        if (Physics.Raycast(rayPos, Vector3.down, groundDetectionDist + 0.2f, ~(1 << LayerMask.NameToLayer("Player"))))
            return true;
        else
            return false;
    }
}
