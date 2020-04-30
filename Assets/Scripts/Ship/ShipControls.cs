using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControls : Seat
{
	private enum State
	{
		FREE_FLIGHT,
		LANDING,
		LANDED,
	}

	// Asteroid landing procedures
	public Asteroid closestAsteroid;
	private State state = State.LANDED;
	private Vector3 preLandingPos = Vector3.zero;
	private Quaternion preLandingRotation = Quaternion.identity;
	private float landingProgress = 0.0f;

	Vector3 motion;

	public float turnSpeed = 0.2f;
	public float moveSpeed = 1.0f;
	public float yaw = 90.0f;
	public float pitch;
	private float leftPitch, rightPitch;
	private Vector3 moveInput;
	private float helmPitch, helmYaw;

	public Transform leftEngine, rightEngine;

	public Transform helm;
	public ParticleSystem particles;

	private Transform shipTransform;

	// Interactable/Seat interface
	public override bool CanInteract()
	{
		return base.CanInteract();
	}

	protected override void Awake()
	{
		base.Awake();

		if (particles != null)
			particles.Stop();
		shipTransform = FindObjectOfType<ShipExterior>().transform;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		// Apply rotation
		if (leftEngine && rightEngine)
		{
			leftEngine.localEulerAngles = new Vector3(Mathf.LerpAngle(leftEngine.localEulerAngles.x, -leftPitch, Time.deltaTime), 0.0f, 0.0f);
			rightEngine.localEulerAngles = new Vector3(Mathf.LerpAngle(rightEngine.localEulerAngles.x, -rightPitch, Time.deltaTime), 0.0f, 180.0f);
		}

		// Apply motion
		motion += moveInput;
		motion *= 0.9f;

		switch (state)
		{
			case State.FREE_FLIGHT:
			{
				// Free flight, use motion and inputs
				shipTransform.eulerAngles = new Vector3(-pitch, yaw, 0.0f);
				shipTransform.position += shipTransform.rotation * motion * Time.deltaTime;

				break;
			}
			case State.LANDED:
			{
				// No movement
				break;
			}
			case State.LANDING:
			{
				// Landing mode, lerp to target
				landingProgress += Time.deltaTime / 5.0f;
				if (landingProgress >= 1.0f)
				{
					landingProgress = 1.0f;
					state = State.LANDED;
				}
				shipTransform.position = Vector3.Lerp(preLandingPos, closestAsteroid.landingPos.position, landingProgress);
				shipTransform.rotation = Quaternion.Lerp(preLandingRotation, Quaternion.identity, landingProgress);
				break;
			}
		}

		// Controller Stick Animation
		helmYaw *= 0.9f;
		helmPitch *= 0.9f;
		helm.localEulerAngles = new Vector3(-helmYaw, 0.0f, helmPitch) * 3.0f;
	}

	private void BeginTakeoff()
	{
		Ship.CheckTakeoffTrigger();
		state = State.FREE_FLIGHT;
		if (particles != null)
			particles.Play();

		motion.y = 100.0f;
	}

	private void BeginLanding()
	{
		Ship.CheckLandingTrigger(closestAsteroid.gameObject);
		preLandingPos = shipTransform.position;
		preLandingRotation = shipTransform.rotation;
		landingProgress = 0.0f;
		if (particles != null)
			particles.Stop();

		state = State.LANDING;
	}

	public string GetOverlayText()
	{
		switch (state)
		{
			case State.LANDED:
				return "Parking brake engaged. Press SPACE to take off. Press F to leave the helm";
			case State.LANDING:
				return "Parking mode engaged. Please wait for landing.";
			case State.FREE_FLIGHT:
				if (closestAsteroid != null)
				{
					return "Press SPACE to land on asteroid";
				}
				else
				{
					return "Press W to accelerate, A/D to turn";
				}
		}
		return "";
	}

	public void NotControlling()
	{
		moveInput = Vector3.zero;
		leftPitch = 0.0f;
		rightPitch = 0.0f;
	}

	// Brake engaged voice snippet
	private float timeSinceLastBrakeSnippet = 1.0f;


	// Seat interface
	public override void UpdateControlledByLocalPlayer(PlayerInputs inputs)
	{
		// Engine pitch, update as we go
		leftPitch = 0.0f;
		rightPitch = 0.0f;
		moveInput = Vector3.zero;

		switch (state)
		{
			case State.FREE_FLIGHT:
			{
				yaw += inputs.move.x * turnSpeed;
				helmYaw += inputs.move.x;
				leftPitch += inputs.move.x * 30.0f;
				rightPitch -= inputs.move.x * 30.0f;
				moveInput = new Vector3(0.0f, 0.0f, -inputs.move.z);
				if (moveInput.sqrMagnitude > 1.0f)
				{
					moveInput.Normalize();
				}
				moveInput *= moveSpeed;

				timeSinceLastBrakeSnippet += Time.deltaTime;
				if (Mathf.Abs(inputs.move.x) > 0.0f || Mathf.Abs(inputs.move.z) > 0.0f && timeSinceLastBrakeSnippet >= 5.0f)
				{
					timeSinceLastBrakeSnippet = 0.0f;
				}

				if (closestAsteroid != null)
				{
					if (Input.GetKeyDown(KeyCode.Space))
					{

						BeginLanding();

					}
				}
				// Pitch controls
				/*
				pitch = Mathf.Lerp(pitch, 0.0f, turnSpeed * Time.deltaTime);
				if (Input.GetKey(KeyCode.Space))
				{
					pitch = Mathf.Lerp(pitch, 30.0f, turnSpeed * Time.deltaTime);
					leftPitch += 30.0f;
					rightPitch += 30.0f;
					helmPitch += 1.0f;
				}
				if (Input.GetKey(KeyCode.LeftShift))
				{
					pitch = Mathf.Lerp(pitch, -30.0f, turnSpeed * Time.deltaTime);
					leftPitch -= 30.0f;
					rightPitch -= 30.0f;
					helmPitch -= 1.0f;
				}

				moveInput.y = pitch / 60.0f;
				*/

				break;
			}
			case State.LANDING:
			{
				break;
			}
			case State.LANDED:
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					BeginTakeoff();
				}
				break;
			}
		}
	}
}