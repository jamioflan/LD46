using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayer : PortalTraveller
{
	private CharacterController cc;
	private Vector3 motion;
	private float yaw;
	private float pitch;

	public float mouseSensitivity = 1.0f;
	public float moveSpeed = 1.0f;
	public Transform head, holdPos;

	public Seat currentSeat;

	void Awake()
    {
		cc = GetComponent<CharacterController>();
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
	}

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

		// Not in a seat, do first person controls
		if (currentSeat == null)
		{
			if (cc.isGrounded)
				motion.y = 0.0f;
			motion *= 0.5f;

			// Get inputs
			yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
			pitch += Input.GetAxis("Mouse Y") * mouseSensitivity;

			Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
			if (moveInput.sqrMagnitude > 1.0f)
			{
				moveInput.Normalize();
			}
			moveInput *= moveSpeed;

			// Cap rotations
			pitch = Mathf.Clamp(pitch, -90.0f, 90.0f);
			// Apply rotations
			transform.localEulerAngles = new Vector3(0.0f, yaw, 0.0f);
			head.localEulerAngles = new Vector3(-pitch, 0.0f, 0.0f);


			// Apply motion
			motion += transform.rotation * moveInput;
			motion.y -= 3.0f;
			cc.Move(motion * Time.deltaTime);
		}
		// If we are in a seat, let them handle the inputs
		else
		{
			currentSeat.UpdateControlledByLocalPlayer();
		}
	}

	public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
	{
		base.Teleport(fromPortal, toPortal, pos, rot);

		yaw = rot.eulerAngles.y;
	}
}
