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

	public enum ThrottleNotch
	{
		FORWARD,
		NEUTRAL,
		REVERSE,
	}

	// Asteroid landing procedures
	public Asteroid closestAsteroid;
	private State state = State.LANDED;
	private Vector3 preLandingPos = Vector3.zero;
	private Quaternion preLandingRotation = Quaternion.identity;
	private float landingProgress = 0.0f;

	Vector3 motion;

	public float maxThrottle = 3.0f;
	public float maxReverseThrottle = -1.0f;
	public float turnSpeed = 0.2f;
	public float moveSpeed = 1.0f;


	public float throttle = 0.0f;
	public ThrottleNotch throttleNotch = ThrottleNotch.NEUTRAL;
	private float yaw = -90.0f;
	private float pitch;
	private float leftPitch, rightPitch;
	private Vector3 moveInput;
	private float helmPitch, helmYaw;

	public Transform leftEngine, rightEngine;

	public Transform helm;
	public ParticleSystem particles;

	private Transform shipTransform;
	private Rigidbody shipRigidbody;
	private Throttle throttleAnim;

	// Brake engaged voice snippet
	private float timeSinceLastBrakeSnippet = 1.0f;

	private PlayerInputs shipInputs;

	// Interactable/Seat interface
	public override string GetHoverText(Player player)
	{
		Player pilot = GetPlayer();
		if (pilot != null)
		{
			if(pilot == player)
			{
				return "Press Space to take-off, W/s to accelerate, A/D to turn";
			}
			else
			{
				return "This seat is occupied by " + pilot.name;
			}
		}
		else if(CanInteract(player))
		{
			return "Press F to pilot the ship";
		}
		else
		{
			return "Ship controls locked. Please perform pre-flight checks.";
		}
	}

	public override bool CanInteract(Player player)
	{
		return base.CanInteract(player);
	}



	private void BeginTakeoff()
	{
		Story.inst.CheckTakeoffTrigger();
		state = State.FREE_FLIGHT;
		if (particles != null)
			particles.Play();

		motion.y = 100.0f;
	}

	private void BeginLanding()
	{
		Story.inst.CheckLandingTrigger(closestAsteroid.gameObject);
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


	// Seat interface
	public override void UpdateControlledByLocalPlayer(PlayerInputs inputs)
	{
		shipInputs = inputs;
		return;
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

	private void ProcessControls()
	{
		// Rotation
		shipRigidbody.AddRelativeTorque(Vector3.up * shipInputs.rotate.y * turnSpeed * shipRigidbody.mass);
		shipRigidbody.AddRelativeTorque(Vector3.forward * shipInputs.rotate.x * turnSpeed * shipRigidbody.mass);
		shipRigidbody.AddRelativeTorque(Vector3.right * shipInputs.rotate.z * turnSpeed * shipRigidbody.mass);

		if (shipInputs.jump)
		{
			shipRigidbody.AddForce(Vector3.up * moveSpeed * shipRigidbody.mass);
		}

		float throttleDelta = shipInputs.throttle * Time.deltaTime;

		// Reset to neutral when not sending input
		if (Mathf.Approximately(throttleDelta, 0.0f))
		{
			if (throttleNotch != ThrottleNotch.NEUTRAL)
				Debug.LogWarning("Oops" + shipInputs.throttle);
			throttleNotch = ThrottleNotch.NEUTRAL;
		}
		else if(throttleNotch == ThrottleNotch.NEUTRAL)
		{
			// We have some non-zero input. After moving, set our throttle notch so we can't go back past 0.
			throttle += throttleDelta;
			throttle = Mathf.Clamp(throttle, maxReverseThrottle, maxThrottle);

			if (throttle > 0.0f)
				throttleNotch = ThrottleNotch.FORWARD;
			else if (throttle < 0.0f)
				throttleNotch = ThrottleNotch.REVERSE;
		}
		else
		{
			// Our throttle has notched already, we are limited to a certain range
			throttle += throttleDelta;
			switch(throttleNotch)
			{
				case ThrottleNotch.FORWARD:
					throttle = Mathf.Clamp(throttle, 0.0f, maxThrottle);
					break;
				case ThrottleNotch.REVERSE:
					throttle = Mathf.Clamp(throttle, maxReverseThrottle, 0.0f);
					break;
				default:
					Debug.Log("Invalid throttle notch");
					break;
			}
		}

		shipRigidbody.AddForce(throttle * moveSpeed * shipTransform.forward);
	}

	private void UpdateAnims()
	{
		throttleAnim.SetValues(throttle, maxReverseThrottle, maxThrottle);
	}

	protected override void Awake()
	{
		base.Awake();

		if (particles != null)
			particles.Stop();
		shipTransform = FindObjectOfType<ShipExterior>().transform;
		shipRigidbody = shipTransform.GetComponent<Rigidbody>();
		throttleAnim = FindObjectOfType<Throttle>();
	}

	protected override void Update()
	{
		base.Update();

		UpdateAnims();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		ProcessControls();

		// Rotation



		/*
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
		*/

		// Animations
		// Controller Stick Animation
		//helmYaw += shipInputs.yaw;
		//helmYaw *= 0.9f;
		//helmPitch *= 0.9f;
		//helm.localEulerAngles = new Vector3(-helmYaw, 0.0f, helmPitch) * 3.0f;
	}
}