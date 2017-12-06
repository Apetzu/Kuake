using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class playerController : NetworkBehaviour {

    // Possible to edit in Unity editor
	public float mouseSensitivity = 5;
    public float movSpeed = 4;									// = max player speed
    //public float sprintSpeedAdd = 4;							// = added sprint speed when sprinting. sprinting is now disabled
    public float jumpAcceleration = 5;
    public float clampVelocity = 500;
    public float fireRate = 0.5f;								// = how fast player is able to shoot
    public float groundDetectionDist = 0.2f;					// = lenght of the ground check raycast
    public bool isThisLocalPlayer = false;						// = tells to other scripts if this player is the local player
    public CursorLockMode cursorState = CursorLockMode.None;	// = cursor mode: is it visible and locked or not

	// Link these to right gameobjects/components in Unity editor
	public GameObject rocketPrefab;
	public GameObject rocketLauncher;
	public GameObject HUD;
	public GameObject shades;
	public GameObject FPW;
    public Camera playerCamera;


	double lastShot = 0.0;
    Vector3 currentVelocity;
    bool jumpKey = false;
    bool shootingHold = false;
    Vector3 deltaPos = Vector3.zero;
    //Vector3 lastPos = Vector3.zero;

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
		// Movement is done in this update
        if (isLocalPlayer)	// If this player is the local player
        {
            // Next mouse movement change
			Vector2 mouseDelta = mouseMovement() * mouseSensitivity;

            deltaPos = Vector3.Normalize(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"))) * (movSpeed/* + Input.GetAxisRaw("Sprint") * sprintSpeedAdd */); // sprint disabled

            rb.AddRelativeForce(deltaPos);

            // Jumping (Space key)
			if (Input.GetButton("Jump"))
            {
                if (playerGrounded() && jumpKey == false)
                {
                    jumpKey = true;
                    rb.velocity = new Vector3 (rb.velocity.x, 0f, rb.velocity.z);
                    rb.AddForce(Vector3.up * rb.mass * (-Physics.gravity.y + jumpAcceleration / Time.deltaTime));
                }
            }
            else
            {
                jumpKey = false;
            }

			// Applying mouse movement (rotation)
			rb.MoveRotation(Quaternion.Euler(0, rb.rotation.eulerAngles.y + mouseDelta.x, 0));
			playerCamera.transform.localRotation = Quaternion.Euler(strangeAxisClamp(-mouseDelta.y + playerCamera.transform.localRotation.eulerAngles.x, 90, 270), 0, 0);

            //Debug.Log(rb.position - lastPos);

            //lastPos = rb.position;
        }
    }

    void Update()
    {
        // All the non movement things are done in this update
        if (isLocalPlayer)  // If this player is the local player
        {
            // Shooting (Left click)
            if (Input.GetButton("Fire"))
            {
                if (shootingHold == false)
                {
                    shootingHold = true;
                    CmdFire();
                }
            }
            else
            {
                shootingHold = false;
            }

            // Respawn (temporary, ESC key)
            if (Input.GetButton("Menu"))
            {
                transform.position = new Vector3(0, 5, 0);
            }
        }
    }

    // If this player is the local player, this disables not needed gameobjects/components
	public override void OnStartLocalPlayer()
    {
        playerCamera.gameObject.SetActive(true);
        FPW.gameObject.SetActive(true);
        HUD.SetActive(true);
        shades.SetActive(false);
        playerModel.enabled = false;
        rocketLauncher.SetActive(false);
        isThisLocalPlayer = true;
    }

    // Spawns a rocket
	[Command]
    public void CmdFire()
    {
        if (Time.time >= fireRate + lastShot)
        {
            GameObject rocket = Instantiate(rocketPrefab, rocketLauncher.transform.position, Quaternion.Euler(playerCamera.transform.eulerAngles.x + 90, transform.eulerAngles.y, 0));

            NetworkServer.Spawn(rocket);
			rocket.GetComponent<rocket>().spawner = this.gameObject;
            lastShot = Time.time;

            Destroy(rocket, 2.0f);
        }
    }

    // Clamps angles when other angle limit is negative
	public float strangeAxisClamp(float value, float limit1, float limit2)
    {
        if (value > limit1 && value < 180)
            value = limit1;
        else if (value > 180 && value < limit2)
            value = limit2;
        return value;
    }

    // Gets mouse movement in Vector2 type
	public Vector2 mouseMovement()
    {
        return new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));
    }

    // Checks if player is on the ground
	public bool playerGrounded()
    {
        Vector3 rayPos = new Vector3(transform.position.x, transform.position.y - playerCollider.height / 2 + 0.2f, transform.position.z);

        if (Physics.Raycast(rayPos, Vector3.down, groundDetectionDist + 0.2f))
            return true;
        else
            return false;
    }
}
