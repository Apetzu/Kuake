﻿using UnityEngine;
using UnityEngine.Networking;

public class playerController : NetworkBehaviour {

	// Possible to edit in Unity editor
	[SerializeField]
	private float mouseSensitivity = 5f;
	[SerializeField]
	private float movSpeed = 4f;								// = max player speed
	//[SerializeField]
	//private float sprintSpeedAdd = 4f;						// = added sprint speed when sprinting. disabled for now
    [SerializeField]
    private float smoothSpeed = 0.6f;							// = how fast player gets to max speed
    [SerializeField]
    private float jumpAcceleration = 250f;
    [SerializeField]
    private float fireRate = 0.2f;								// = how fast player is able to shoot
    [SerializeField]
    private float groundDetectionDist = 0.2f;					// = lenght of the ground check raycast
    [SerializeField]
    private CursorLockMode cursorState = CursorLockMode.None;   // = cursor mode: is it visible and locked or not

    // Link these to right gameobjects/components in Unity editor
    [SerializeField]
    private GameObject rocketPrefab;
    [SerializeField]
    private GameObject rocketLauncher;
    [SerializeField]
    private GameObject hud;
    [SerializeField]
    private GameObject shades;
	[SerializeField]
	private GameObject pekker;
	[SerializeField]
	private GameObject leftTesticle;
	[SerializeField]
	private GameObject rightTesticle;
    [SerializeField]
    private GameObject fpw;
    [SerializeField]
    private Camera playerCamera;

    public bool IsThisLocalPlayer = false;                      // = tells to other scripts if this player is the local player

    double lastShot = 0.0d;
	Vector3 currentVelocity;
	bool jumpKey = false;
	bool playerOnGround = true;
	bool shootingHold = false;
	Vector3 deltaPos = Vector3.zero;

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
		if (isLocalPlayer) // If this player is the local player
		{
			// Next mouse movement change
			Vector2 mouseDelta = mouseMovement() * mouseSensitivity;

			// Checking if player is on the ground
			playerOnGround = playerGrounded();

			// Movement change
			deltaPos = Vector3.Normalize(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"))) * (movSpeed /* + Input.GetAxisRaw("Sprint") * sprintSpeedAdd */); // sprint disabled

			// Applying player ground movement (position)
			rb.MovePosition(Vector3.SmoothDamp(rb.position, rb.position + transform.TransformDirection(deltaPos), ref currentVelocity, smoothSpeed, Mathf.Infinity, Time.deltaTime));

			// Jumping (Space key)
			if (Input.GetButton("Jump"))
			{
				if (playerOnGround && jumpKey == false)
				{
					jumpKey = true;
					rb.velocity = new Vector3 (rb.velocity.x, 0f, rb.velocity.z);
					rb.AddForce(Vector3.up * rb.mass * (Physics.gravity.y + jumpAcceleration));
				}
			}
			else
			{
				jumpKey = false;
			}

			// Applying mouse movement (rotation)
			rb.MoveRotation(Quaternion.Euler(0f, rb.rotation.eulerAngles.y + mouseDelta.x, 0f));
			playerCamera.transform.localRotation = Quaternion.Euler(strangeAxisClamp((-mouseDelta.y + playerCamera.transform.localRotation.eulerAngles.x), 90f, 270f), 0f, 0f);
        }
	}

	void Update()
	{
		// All non-movement actions are done in this update
		if (isLocalPlayer) 
		{
			// Shooting (Left click)
			if (Input.GetButton("Fire"))
			{
				if (shootingHold == false)
				{
					shootingHold = true;
					CmdFire(rocketLauncher.transform.position, transform.rotation, playerCamera.transform.rotation);
				}
			}
			else
			{
				shootingHold = false;
			}

			// Respawn (temporary, ESC key)
			if (Input.GetButton("Menu"))
			{
				transform.position = new Vector3(0f, 5f, 0f);
                rb.velocity = Vector3.zero;
			}
		}
	}

	// If this player is the local player, this disables not needed gameobjects/components
	public override void OnStartLocalPlayer()
	{
		playerCamera.gameObject.SetActive(true);
		fpw.gameObject.SetActive(true);
		hud.SetActive(true);
		shades.SetActive(false);
		pekker.SetActive (false);
		leftTesticle.SetActive (false);
		rightTesticle.SetActive (false);
		playerModel.enabled = false;
		rocketLauncher.SetActive(false);
	}

	// Spawns a rocket
	[Command]
	void CmdFire(Vector3 SpawnPosition, Quaternion PlayerDirection, Quaternion CameraDirection)
	{
		if (Time.time >= fireRate + lastShot)
		{
            GameObject rocket = Instantiate(rocketPrefab, SpawnPosition, Quaternion.Euler(CameraDirection.eulerAngles.x + 90f, PlayerDirection.eulerAngles.y, 0f));

			NetworkServer.Spawn(rocket);
			rocket.GetComponent<rocket>().Spawner = this.gameObject;
			lastShot = Time.time;

			Destroy(rocket, 2.0f);
		}
	}

    public void Die()
    {
        rb.velocity = Vector3.zero;
    }

	// Clamps angles when other angle limit is negative
	public float strangeAxisClamp(float value, float limit1, float limit2)
	{
		if (value > limit1 && value < 180f)
			value = limit1;
		else if (value > 180f && value < limit2)
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
		Vector3 rayPos = new Vector3(transform.position.x, transform.position.y - playerCollider.height / 2f + 0.2f, transform.position.z);

		if (Physics.Raycast(rayPos, Vector3.down, groundDetectionDist + 0.2f))
			return true;
		else
			return false;
	}
}