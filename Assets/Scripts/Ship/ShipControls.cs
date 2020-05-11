using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControls : Seat
{
	private enum FlightMode
	{
		FREE_FLIGHT,
		PARKING,
		PARKED,
	}

	private FlightMode flightMode = FlightMode.PARKED;
	private bool controlsLockedAfterModeSwitch = false; // Require player to take hands off all controls before the ship responds to new mode

	[Header("Free Flight Mode")]
	public float maxThrottle = 3.0f;
	public float maxReverseThrottle = -1.0f;
	public float turnSpeed = 0.2f;
	public float moveSpeed = 1.0f;

	private enum ThrottleNotch
	{
		FORWARD,
		NEUTRAL,
		REVERSE,
	}

	private ThrottleNotch throttleNotch = ThrottleNotch.NEUTRAL;
	private float throttle = 0.0f;

	[Header("Parking Mode")]
	public float parkModeMoveSpeed = 1.0f;
	public float parkModeTurnSpeed = 1.0f;
	public float parkModeVerticalSpeed = 1.0f;
	public float parkModeVerticalSensitivity = 1.0f;

	private float parkModeVerticalTarget = 0.0f;

	[Header("Anims and Particles")]
	public Transform leftEngine, rightEngine;
	public Transform helm;
	public ParticleSystem particles;

	private float yaw = -90.0f;
	private float pitch;
	private float leftPitch, rightPitch;
	private float helmPitch, helmYaw;

	// Automatically established links
	private ShipExterior shipExterior;
	private Transform shipTransform;
	private Rigidbody shipRigidbody;
	private Throttle throttleAnim;

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

	private void RequestFlightMode(FlightMode mode)
	{
		if (mode != flightMode)
		{
			Debug.Log($"[Ship] RequestFlightMode transitioning from {flightMode} to {mode}");
			ExitFlightMode(mode);
			EnterFlightMode(mode);
			flightMode = mode;

			// Clear inputs and lock until we get fresh data
			controlsLockedAfterModeSwitch = true;
			shipInputs = new PlayerInputs();
		}
	}

	private void ExitFlightMode(FlightMode nextMode)
	{
		switch(flightMode)
		{
			case FlightMode.PARKED:
			{
				Story.inst.CheckTakeoffTrigger();
				// When taking off, give us a boost
				//motion.y = 100.0f;
				break;
			}
			case FlightMode.FREE_FLIGHT:
			{
				if (shipExterior.closestAsteroid != null)
				{
					Story.inst.CheckLandingTrigger(shipExterior.closestAsteroid.gameObject);

					if (particles != null)
						particles.Stop();
				}
				else
				{
					Debug.LogError("Can't leave free flight mode without somewhere to stop");
				}
				break;
			}
		}
	}

	private void EnterFlightMode(FlightMode nextMode)
	{
		switch(nextMode)
		{
			case FlightMode.FREE_FLIGHT:
			{
				if (particles != null)
					particles.Play();
				break;
			}
			case FlightMode.PARKING:
			{
				
				break;
			}
		}
	}

	public string GetOverlayText()
	{
		switch (flightMode)
		{
			case FlightMode.PARKED:
				return "Parking brake engaged. Press SPACE to take off. Press F to leave the helm";
			case FlightMode.PARKING:
				return "Parking mode engaged. Please wait for landing.";
			case FlightMode.FREE_FLIGHT:
				if (shipExterior.closestAsteroid != null)
				{
					return "Press 1 to enter parking mode";
				}
				else
				{
					return "Press W to accelerate, A/D to turn";
				}
		}
		return "";
	}

	// Seat interface
	public override void UpdateControlledByLocalPlayer(PlayerInputs inputs)
	{
		if (controlsLockedAfterModeSwitch)
		{
			if (!inputs.interact && !inputs.jump && !inputs.select1 && !inputs.select2 &&
				Mathf.Approximately(inputs.throttle, 0.0f) &&
				Mathf.Approximately(inputs.rotate.sqrMagnitude, 0.0f) &&
				Mathf.Approximately(inputs.move.sqrMagnitude, 0.0f))
			{
				controlsLockedAfterModeSwitch = false;
			}
		}
		else
		{
			shipInputs = inputs;
		}
	}

	private void ProcessControls()
	{
		switch (flightMode)
		{
			case FlightMode.FREE_FLIGHT:
			{
				// Rotation
				shipRigidbody.AddRelativeTorque(Vector3.up * shipInputs.rotate.y * turnSpeed * shipRigidbody.mass);
				shipRigidbody.AddRelativeTorque(Vector3.forward * shipInputs.rotate.x * turnSpeed * shipRigidbody.mass);
				shipRigidbody.AddRelativeTorque(Vector3.right * shipInputs.rotate.z * turnSpeed * shipRigidbody.mass);

				//if (shipInputs.jump)
				//{
				//	shipRigidbody.AddForce(Vector3.up * moveSpeed * shipRigidbody.mass);
				//}

				float throttleDelta = shipInputs.throttle * Time.deltaTime;

				// Reset to neutral when not sending input
				if (Mathf.Approximately(throttleDelta, 0.0f))
				{
					if (throttleNotch != ThrottleNotch.NEUTRAL)
						Debug.LogWarning("Oops" + shipInputs.throttle);
					throttleNotch = ThrottleNotch.NEUTRAL;
				}
				else if (throttleNotch == ThrottleNotch.NEUTRAL)
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
					switch (throttleNotch)
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

				shipRigidbody.AddForce(throttle * moveSpeed * shipTransform.forward * shipRigidbody.mass);
				break;
			}
			case FlightMode.PARKING:
			{
				// If we leave range of our asteroid, we need to leave parking mode
				//if(closestAsteroid == null)
				{
				//	RequestFlightMode(FlightMode.FREE_FLIGHT);
				}
				//else
				{
					// Behaviour in park mode is to latch on to the surface orientation of the asteroid and move laterally across the surface

					Vector3 lateralMovement = shipTransform.forward * shipInputs.move.z + shipTransform.right * shipInputs.move.x;
					parkModeVerticalTarget += shipInputs.throttle * parkModeVerticalSensitivity * Time.deltaTime;
					parkModeVerticalTarget = Mathf.Clamp(parkModeVerticalTarget, 0.0f, 100.0f);


					shipRigidbody.AddForce(lateralMovement * parkModeMoveSpeed * shipRigidbody.mass);

					// Raycast from each landing gear and work out average distance to asteroid and average surface normal
					int numSuccessfulRays = 0;
					Ray ray;
					RaycastHit hit;
					float minimalDistance = float.MaxValue;
					Vector3 normal = Vector3.zero;

					foreach (Transform rayOrigin in shipExterior.parkModeStabilisationOrigins)
					{
						ray = new Ray(shipTransform.position, -shipTransform.up);
						if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Asteroid")))
						{
							numSuccessfulRays++;

							if (hit.distance < minimalDistance)
								minimalDistance = hit.distance;

							normal += hit.normal;
						}
					}

					// With the sums successful, orient ourselves
					if(numSuccessfulRays > 0)
					{
						normal /= numSuccessfulRays;

						Quaternion deltaAngle = Quaternion.FromToRotation(shipTransform.up, normal);
						float magnitude;
						Vector3 axis;
						deltaAngle.ToAngleAxis(out magnitude, out axis);
						shipRigidbody.AddTorque(axis * magnitude * parkModeTurnSpeed * shipRigidbody.mass);

						float deltaHeight = parkModeVerticalTarget - minimalDistance;
						shipRigidbody.AddForce(shipTransform.up * deltaHeight * parkModeVerticalSpeed * shipRigidbody.mass);
					}
					else
					{
						RequestFlightMode(FlightMode.FREE_FLIGHT);
					}
				}

				break;
			}
		}

		if(shipInputs.select1)
		{
			RequestFlightMode(FlightMode.FREE_FLIGHT);
		}
		if(shipInputs.select2)
		{
			RequestFlightMode(FlightMode.PARKING);
		}
	}

	private void UpdateAnims()
	{
		throttleAnim.SetValues(throttle, maxReverseThrottle, maxThrottle);

		// Controller Stick Animation
		helmYaw += shipInputs.rotate.y;
		helmYaw *= 0.9f;
		helmPitch *= 0.9f;
		helm.localEulerAngles = new Vector3(-helmYaw, 0.0f, helmPitch) * 3.0f;
	}

	protected override void Awake()
	{
		base.Awake();

		if (particles != null)
			particles.Stop();
		shipExterior = FindObjectOfType<ShipExterior>();
		shipTransform = shipExterior.transform;
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
	}
}