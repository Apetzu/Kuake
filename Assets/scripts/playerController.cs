﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class playerController : NetworkBehaviour {

    Camera playerCamera;
    public float mouseSensitivity = 5;
    public CursorLockMode cursorState = CursorLockMode.None;
    public float movAccel = 10;
    public float jumpAccel = 300;
    public GameObject rocketPrefab;
    Transform rocketSpawn;
    Rigidbody rb;
    int jump = 0;


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

        if (axis2Bool("Fire"))
        {
            Fire();
        }

        rb.AddRelativeForce (Vector3.Scale (new Vector3 (Input.GetAxisRaw ("Horizontal"), jump, Input.GetAxisRaw ("Vertical")).normalized, new Vector3 (movAccel * rb.mass, jumpAccel, movAccel * rb.mass)));

		Vector2 mouseDelta = mouseMovement () * mouseSensitivity;

		rb.MoveRotation (Quaternion.Euler (0, rb.rotation.eulerAngles.y + mouseDelta.x, 0));
		//mainCamera.transform.Rotate(new Vector3(-mouseDelta.y, 0, 0)); This doesn't have clamping capability
		playerCamera.transform.localRotation = Quaternion.Euler (strangeAxisClamp (-mouseDelta.y + playerCamera.transform.localRotation.eulerAngles.x, 90, 270), 0, 0);

		jump = 0;
    }

    void Fire()
    {
        // Create the Bullet from the Bullet Prefab
        var rocket = (GameObject)Instantiate(
            rocketPrefab,
            rocketSpawn.position,
            rocketSpawn.rotation);

        // Add velocity to the bullet
        rocket.GetComponent<Rigidbody>().velocity = rocket.transform.forward * 6;

        // Destroy the bullet after 2 seconds
        Destroy(rocket, 2.0f);
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
