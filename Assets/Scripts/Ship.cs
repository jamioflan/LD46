using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
	public static Ship theShip;

	public MeshRenderer aFace;
	private Material faceMaterial;
	public Emotion currentEmotion;

	public Transform standingPos;

	public float timeToNext = 1.0f;

	public enum Emotion
	{
		CROSS,
		TICK,
		EXCLAIMATION,
		QUESTION,
		SLEEP,
		NO,
		LOVE,
		DIZZY,
		CRY,
		DEAD,
		ANGRY,
		CUTE,
		NEUTRAL_HAPPY,
		HAPPY,
		POINT_LEFT,
		SAD,
	}

    // Start is called before the first frame update
    void Start()
    {
		theShip = this;
		faceMaterial = aFace.sharedMaterial;

	}

    // Update is called once per frame
    void Update()
    {
		timeToNext -= Time.deltaTime;
		if(timeToNext <= 0.0f)
		{
			timeToNext = 1.0f;
			SetEmotion((Emotion)(((int)currentEmotion + 1) % 16));
		}

		// Apply rotation
		transform.eulerAngles = new Vector3(-pitch, yaw, 0.0f);
		leftEngine.localEulerAngles = new Vector3(Mathf.LerpAngle(leftEngine.localEulerAngles.x, -leftPitch, Time.deltaTime), 0.0f, 0.0f);
		rightEngine.localEulerAngles = new Vector3(Mathf.LerpAngle(rightEngine.localEulerAngles.x, -rightPitch, Time.deltaTime), 0.0f, 180.0f);


		// Apply motion
		motion += moveInput;
		motion *= 0.9f;
		

		transform.position += transform.rotation * motion * Time.deltaTime;

		// Controller Stick
		helmYaw *= 0.9f;
		helmPitch *= 0.9f;
		helm.localEulerAngles = new Vector3(-helmYaw, 0.0f, helmPitch) * 3.0f;
	}

	public void SetEmotion(Emotion emotion)
	{
		currentEmotion = emotion;
		faceMaterial.SetInt("_Face", (int)emotion);
	}



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

	public void NotControlling()
	{
		moveInput = Vector3.zero;
		leftPitch = 0.0f;
		rightPitch = 0.0f;
	}

	public void UpdateMovementControls()
	{
		// Engine pitch, update as we go
		leftPitch = 0.0f;
		rightPitch = 0.0f;

		// Get inputs
		yaw += Input.GetAxis("Horizontal") * turnSpeed;
		helmYaw += Input.GetAxis("Horizontal");
		leftPitch += Input.GetAxis("Horizontal") * 30.0f;
		rightPitch -= Input.GetAxis("Horizontal") * 30.0f;
		moveInput = new Vector3(0.0f, 0.0f, Input.GetAxis("Vertical"));
		if (moveInput.sqrMagnitude > 1.0f)
		{
			moveInput.Normalize();
		}
		moveInput *= moveSpeed;

		// Pitch controls
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
	}
}
