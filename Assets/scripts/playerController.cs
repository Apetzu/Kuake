using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class playerController : NetworkBehaviour {

    Camera playerCamera;
    public float mouseSensitivity = 5;
    public float movSpeed = 10;
    public float smoothSpeed = 5;
    public float jumpForce = 300;
    public float fireRate = 0.5f;
    public CursorLockMode cursorState = CursorLockMode.None;
    double lastShot = 0.0;
    Vector3 currentVelocity;

    public GameObject rocketPrefab;
    Transform rocketSpawn;
    Rigidbody rb;

    void Awake()
    {
		playerCamera = transform.Find("playerCamera").GetComponent<Camera>();
        Cursor.lockState = cursorState;
        rb = GetComponent<Rigidbody>();
        rocketSpawn = transform.Find("rocketSpawn");
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            playerCamera.enabled = false;
            return;
        }

        transform.Find("shades").gameObject.SetActive(false);

        if (axis2Bool("Fire"))
        {
            CmdFire();
        }

        if (axis2Bool("Menu"))
        {
            transform.position = new Vector3(0, 5, 0);
        }

        Vector2 mouseDelta = mouseMovement () * mouseSensitivity;
        Vector3 deltaPos = Vector3.Normalize(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));

        rb.MovePosition(Vector3.SmoothDamp(rb.position, rb.position + transform.TransformDirection(deltaPos * movSpeed), ref currentVelocity, smoothSpeed, Mathf.Infinity, Time.deltaTime));

		rb.MoveRotation (Quaternion.Euler (0, rb.rotation.eulerAngles.y + mouseDelta.x, 0));
		playerCamera.transform.localRotation = Quaternion.Euler (strangeAxisClamp (-mouseDelta.y + playerCamera.transform.localRotation.eulerAngles.x, 90, 270), 0, 0);

    }

    [Command]
    public void CmdFire()
    {
        if (Time.time >= fireRate + lastShot)
        {
            // Create the Bullet from the Bullet Prefab
            var rocket = (GameObject)Instantiate(
                rocketPrefab,
                rocketSpawn.position,
                rocketSpawn.rotation);

            // Add velocity to the bullet
            rocket.GetComponent<Rigidbody>().velocity = rocket.transform.forward * 40;

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

    public bool axis2Bool(string axisName)
    {
        if (Input.GetAxisRaw (axisName) == 1)
            return true;
        else
            return false;
    }
}
