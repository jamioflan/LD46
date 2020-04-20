using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
	public static Ship theShip;

	private AudioSource source;
	public MeshRenderer aFace;
	private Material faceMaterial;
	public Emotion currentEmotion;
	public Image fadeToBlack;
	public float fadeInTimeRemaining = 10.0f;

	public Transform standingPos;

	public float timeToNext = 1.0f;

	// Asteroid landing procedures
	public Asteroid closestAsteroid;
	private State state = State.LANDED;
	private Vector3 preLandingPos = Vector3.zero;
	private Quaternion preLandingRotation = Quaternion.identity;
	private float landingProgress = 0.0f;

	public TMPro.TextMeshProUGUI questLog1, questLog2;

	public bool CanPickChest() { return phase >= StoryPhase.PICKUP_CHEST; }
	public bool CanFly() { return phase >= StoryPhase.READY_TO_FLY; }

	private enum State
	{
		FREE_FLIGHT,
		LANDING,
		LANDED,
	}

	public enum Emotion
	{
		CROSS,
		TICK,
		EXCLAIMATION,
		QUESTION,
		DOTS,
		SCAN,
		LIGHTBULB,
		EYES_SHUT,
		SLEEP,
		NO,
		LOVE,
		DIZZY,
		NEAR_DEATH_EXPERIENCE,
		PERFECT10,
		BLANK7,
		BLANK8,
		CRY,
		DEAD,
		ANGRY,
		CUTE,
		BLANK9,
		BLANK10,
		BLANK11,
		BLANK12,
		NEUTRAL_HAPPY,
		HAPPY,
		THINKING,
		SAD,
		BLANK13,
		BLANK14,
		BLANK15,
		BLANK16,
	}

	public AudioClip[] clips;

	public enum VoiceClip
	{
		WATCH_OUT,
		REBOOTING,
		CAPTAIN_OKAY,
		SCANNING_SHIP,
		ENGINE_DAMAGED,
		AM_I_GONNA_DIE,
		WE_NEED_A_NEW_X,
		MAYBE_ON_ASTEROID,

		CHAT1,
		CHAT2,

		FOUND_IT,
		FIX_ME,
		FIXED_GRATITUDE,
		LETS_GO_DO_MISSION,

		HOW_TO_HELM,
		SPACE_TO_TAKEOFF,
		OTHER_CONTROLS,
		MISSION_QUEST_MARKER,

		CHAT3,
		CHAT4,

		IM_THIRSTY,
		CAN_GET_FUEL,
		FUELING_STATION_OVER_THERE,
		WRONG_WAY,

		PERFECT_LANDING,
		GRAB_A_FUEL,
		PUT_IT_IN_THE_FUEL_HATCH,
		BACK_TO_THE_MISSION,
	
		GETTING_CLOSE,

		CHAT6,
		CHAT7, 

		LOOK_AT_THAT_LOVELY_THING,

		GRAB_CHEST,
		LIFT,
		PUT_IN_POSTBOX,
		DELIVERY_SUCCESSFUL,
		NEW_COORDINATES,
		YOU_HAVE_TO_KILL_ME,
		TIME_IS_UP,
		
		CHAT8,
		CHAT9,

		GOODBYE,
		WE_ESCAPED,
	}

	public enum Priority
	{
		STORY,
		CONTEXT,
		CHAT,
		
	}

	public enum StoryPhase
	{
		INTRO,
		FETCH_COMPONENT,
		HAVE_COMPONENT,
		READY_TO_PLACE,
		READY_TO_FLY,
		FLY_TO_MISSION1,

		FLY_TO_REFUEL,
		GET_FUEL_CAN,
		PLACE_FUEL_CAN,

		FLY_TO_MISSION2,
		PICKUP_CHEST,
		LOOK_AT_CHEST,
		DROPOFF_CHEST,
		FLY_TO_DOOM,

		DEATH,
		LIFE,
		PLAYER_WAS_AN_ID,
	}

	public void TriggerStoryPhase(StoryPhase storyPhase)
	{
		if (storyPhase > phase)
		{
			phase = storyPhase;

			questLog1.text = "";
			questLog1.fontStyle = TMPro.FontStyles.Normal;
			questLog2.text = "";

			switch (storyPhase)
			{
				case StoryPhase.INTRO:
					QueueVoiceLine(Emotion.CROSS, VoiceClip.WATCH_OUT, Priority.STORY);
					QueueVoiceLine(Emotion.DOTS, VoiceClip.REBOOTING, Priority.STORY);
					QueueVoiceLine(Emotion.NEUTRAL_HAPPY, VoiceClip.CAPTAIN_OKAY, Priority.STORY);
					QueueVoiceLine(Emotion.SCAN, VoiceClip.SCANNING_SHIP, Priority.STORY);
					QueueVoiceLine(Emotion.CRY, VoiceClip.ENGINE_DAMAGED, Priority.STORY);
					questLog1.text = "Check Robin's engine";
					ObjectiveMarker.the.target = objectiveEngine;
					break;
				case StoryPhase.FETCH_COMPONENT:
					QueueVoiceLine(Emotion.DEAD, VoiceClip.AM_I_GONNA_DIE, Priority.STORY);
					QueueVoiceLine(Emotion.LIGHTBULB, VoiceClip.WE_NEED_A_NEW_X, Priority.STORY);
					QueueVoiceLine(Emotion.THINKING, VoiceClip.MAYBE_ON_ASTEROID, Priority.STORY);
					QueueVoiceLine(Emotion.HAPPY, VoiceClip.CHAT1, Priority.CHAT);
					questLog1.text = "Find the component";
					music.Play();
					ObjectiveMarker.the.target = objectiveDoor;
					break;
				case StoryPhase.HAVE_COMPONENT:
					QueueVoiceLine(Emotion.HAPPY, VoiceClip.FOUND_IT, Priority.STORY);
					QueueVoiceLine(Emotion.CUTE, VoiceClip.CHAT2, Priority.CHAT);
					questLog1.text = "Fit the component into the engine";
					ObjectiveMarker.the.target = objectiveEngine;
					break;
				case StoryPhase.READY_TO_PLACE:
					QueueVoiceLine(Emotion.EYES_SHUT, VoiceClip.FIX_ME, Priority.STORY);
					ObjectiveMarker.the.target = null;
					break;
				case StoryPhase.READY_TO_FLY:
					QueueVoiceLine(Emotion.NEAR_DEATH_EXPERIENCE, VoiceClip.FIXED_GRATITUDE, Priority.STORY);
					QueueVoiceLine(Emotion.NEAR_DEATH_EXPERIENCE, VoiceClip.LETS_GO_DO_MISSION, Priority.STORY);
					QueueVoiceLine(Emotion.LIGHTBULB, VoiceClip.HOW_TO_HELM, Priority.STORY);
					QueueVoiceLine(Emotion.HAPPY, VoiceClip.SPACE_TO_TAKEOFF, Priority.STORY);
					questLog1.text = "Go to the helm and takeoff";
					ObjectiveMarker.the.target = objectiveHelm;
					break;
				case StoryPhase.FLY_TO_MISSION1:
					QueueVoiceLine(Emotion.THINKING, VoiceClip.OTHER_CONTROLS, Priority.STORY);
					QueueVoiceLine(Emotion.SCAN, VoiceClip.MISSION_QUEST_MARKER, Priority.STORY);

					QueueVoiceLine(Emotion.HAPPY, VoiceClip.CHAT3, Priority.CHAT);
					QueueVoiceLine(Emotion.CUTE, VoiceClip.CHAT4, Priority.CHAT);
					questLog1.text = "Fly to the postbox";
					ObjectiveMarker.the.target = objectiveMission;
					break;
				case StoryPhase.FLY_TO_REFUEL:
					QueueVoiceLine(Emotion.LIGHTBULB, VoiceClip.IM_THIRSTY, Priority.STORY);
					QueueVoiceLine(Emotion.DIZZY, VoiceClip.CAN_GET_FUEL, Priority.STORY);
					QueueVoiceLine(Emotion.THINKING, VoiceClip.FUELING_STATION_OVER_THERE, Priority.STORY);
					questLog1.text = "Fly to the postbox";
					questLog1.fontStyle = TMPro.FontStyles.Strikethrough;
					questLog2.text = "Fly to the fuel station";
					ObjectiveMarker.the.target = objectiveFuelAsteroid;
					break;
				case StoryPhase.GET_FUEL_CAN:
					QueueVoiceLine(Emotion.PERFECT10, VoiceClip.PERFECT_LANDING, Priority.STORY);
					QueueVoiceLine(Emotion.HAPPY, VoiceClip.GRAB_A_FUEL, Priority.STORY);
					//QueueVoiceLine(Emotion.HAPPY, VoiceClip.CHAT5, Priority.CHAT);
					questLog1.text = "Fly to the postbox";
					questLog1.fontStyle = TMPro.FontStyles.Strikethrough;
					questLog2.text = "Pick up the fuel can";
					ObjectiveMarker.the.target = objectiveFuelCan;
					break;
				case StoryPhase.PLACE_FUEL_CAN:
					QueueVoiceLine(Emotion.HAPPY, VoiceClip.PUT_IT_IN_THE_FUEL_HATCH, Priority.STORY);
					questLog1.text = "Fly to the postbox";
					questLog1.fontStyle = TMPro.FontStyles.Strikethrough;
					questLog2.text = "Fill the fuel intake";
					ObjectiveMarker.the.target = objectiveFuelDropoff;
					break;
				case StoryPhase.FLY_TO_MISSION2:
					QueueVoiceLine(Emotion.TICK, VoiceClip.BACK_TO_THE_MISSION, Priority.STORY);
					QueueVoiceLine(Emotion.PERFECT10, VoiceClip.GETTING_CLOSE, Priority.STORY);
					QueueVoiceLine(Emotion.CUTE, VoiceClip.LOOK_AT_THAT_LOVELY_THING, Priority.STORY);
					QueueVoiceLine(Emotion.NEAR_DEATH_EXPERIENCE, VoiceClip.CHAT6, Priority.CHAT);
					QueueVoiceLine(Emotion.HAPPY, VoiceClip.CHAT7, Priority.CHAT);
					questLog1.text = "Fly to the postbox";
					ObjectiveMarker.the.target = objectiveMission;
					break;
				case StoryPhase.PICKUP_CHEST:
					QueueVoiceLine(Emotion.HAPPY, VoiceClip.GRAB_CHEST, Priority.STORY);
					ObjectiveMarker.the.target = objectiveChest;
					questLog1.text = "Find the chest in the hold";
					break;
				case StoryPhase.LOOK_AT_CHEST:
					QueueVoiceLine(Emotion.HAPPY, VoiceClip.LIFT, Priority.STORY);
					questLog1.text = "Pick up the chest";
					ObjectiveMarker.the.target = objectiveChest;
					break;
				case StoryPhase.DROPOFF_CHEST:
					QueueVoiceLine(Emotion.THINKING, VoiceClip.PUT_IN_POSTBOX, Priority.STORY);
					questLog1.text = "Put the chest in the postbox";
					ObjectiveMarker.the.target = objectivePostbox;
					break;
				case StoryPhase.FLY_TO_DOOM:
					QueueVoiceLine(Emotion.PERFECT10, VoiceClip.DELIVERY_SUCCESSFUL, Priority.STORY);
					QueueVoiceLine(Emotion.THINKING, VoiceClip.NEW_COORDINATES, Priority.STORY);
					QueueVoiceLine(Emotion.CRY, VoiceClip.YOU_HAVE_TO_KILL_ME, Priority.STORY);
					QueueVoiceLine(Emotion.SAD, VoiceClip.TIME_IS_UP, Priority.STORY);
					QueueVoiceLine(Emotion.CRY, VoiceClip.CHAT8, Priority.CHAT);
					QueueVoiceLine(Emotion.SAD, VoiceClip.CHAT9, Priority.CHAT);
					questLog1.text = "Fly to the postbox";
					questLog2.text = "Visit the Rainbow Rocks";
					questLog2.fontStyle = TMPro.FontStyles.Strikethrough;
					ObjectiveMarker.the.target = objectiveDeath;
					break;
				case StoryPhase.DEATH:
					shouldFadeOut = true;
					endText.enabled = true;
					endText.text = @"You succesfully delivered Robin to the SpaceCo decomissioning facility.
Well done employee!";
					ObjectiveMarker.the.target = null;
					QueueVoiceLine(Emotion.HAPPY, VoiceClip.GOODBYE, Priority.STORY);
					break;
				case StoryPhase.LIFE:
					shouldFadeOut = true;
					endText.enabled = true;
					endText.text = @"You did not successfully deliver Robing to the SpaceCo decommisioning facility.

A requisitioning team will be dispatched to your quadrant shortly. 

This will not look good on your employee record.";
					QueueVoiceLine(Emotion.HAPPY, VoiceClip.WE_ESCAPED, Priority.STORY);
					ObjectiveMarker.the.target = null;
					break;
				case StoryPhase.PLAYER_WAS_AN_ID:
					shouldFadeOut = true;
					endText.enabled = true;
					endText.text = "You were lost to the inky void";
					QueueVoiceLine(Emotion.SAD, VoiceClip.GOODBYE, Priority.STORY);
					ObjectiveMarker.the.target = null;
					break;
			}
		}

	
	}

	public ParticleSystem particles;
	public TMPro.TextMeshProUGUI endText;

	private void InformGoingWrongWay()
	{
		QueueVoiceLine(Emotion.ANGRY, VoiceClip.WRONG_WAY, Priority.STORY);
	}

	private class NextClip
	{
		public VoiceClip clip;
		public Emotion face;
		public Priority priority;
		public float timeLeft = float.PositiveInfinity;
		public float pause = -1;
	}

	public Transform objectiveEngine, objectiveDoor, objectiveHelm, objectiveMission, objectivePostbox, objectiveFuelAsteroid, objectiveFuelCan, objectiveFuelDropoff, objectiveDeath, objectiveChest;

	private StoryPhase phase = (StoryPhase)(-1);
	private List<NextClip> clipsToPlay = new List<NextClip>();
	private NextClip currentClip;
	private float waitTimer = 0.0f;
	public AudioSource explosion;

	public void QueuePause(float time, Priority prio)
	{

	}

	public void QueueVoiceLine(Emotion face, VoiceClip clip, Priority prio, float time = float.PositiveInfinity)
	{
		clipsToPlay.Add(new NextClip()
		{
			face = face,
			clip = clip,
			priority = prio,
			timeLeft = time,
		});
	}

	private NextClip GetNextClip()
	{
		for (int i = 0; i < 3; i++)
		{
			if(i > 0 && waitTimer > 0.0f)
			{
				// When waiting, skip all non-story lines
				return null;
			}
			for (int j = 0; j < clipsToPlay.Count; j++)
			{
				NextClip clip = clipsToPlay[j];
				if (clip.priority == (Priority)i)
				{
					Debug.Log($"Playing clip {clip.clip} at priority {clip.priority}");
					clipsToPlay.RemoveAt(j);
					return clip;
				}
			}
		}
		return null;
	}

	private void TriggerNextClip()
	{
		NextClip next = GetNextClip();
		if(next != null)
		{
			source.Stop();
			source.clip = clips[(int)next.clip];
			source.Play();

			SetEmotion(next.face);

			if(next.priority == Priority.STORY)
			{
				waitTimer = Random.Range(6.0f, 8.0f);
			}
		}
		else
		{
			SetEmotion(Emotion.NEUTRAL_HAPPY);
		}
		currentClip = next;
	}

	public AudioSource music;

    // Start is called before the first frame update
    void Start()
    {
		theShip = this;
		faceMaterial = aFace.sharedMaterial;
		source = GetComponent<AudioSource>();

		fadeInTimeRemaining = 10.0f;

		TriggerStoryPhase(StoryPhase.INTRO);
		particles.Stop();
	}

	bool shouldFadeOut = false;

    // Update is called once per frame
    void Update()
    {
		if (shouldFadeOut)
		{
			fadeInTimeRemaining += Time.deltaTime;
			fadeToBlack.color = new Color(0f, 0f, 0f, Mathf.Clamp01(fadeInTimeRemaining / 5.0f));
		}
		else
		{
			if (fadeInTimeRemaining > 0.0f)
			{
				if (fadeInTimeRemaining >= 6.5f
					&& fadeInTimeRemaining - Time.deltaTime < 6.5f)
				{
					explosion.Play();
				}


				fadeInTimeRemaining -= Time.deltaTime;
				if (fadeInTimeRemaining < 0.0f)
					fadeInTimeRemaining = 0.0f;



				fadeToBlack.color = new Color(0f, 0f, 0f, Mathf.Clamp01(fadeInTimeRemaining / 5.0f));
			}
		}

		for (int i = clipsToPlay.Count - 1; i >= 0; i--)
		{
			clipsToPlay[i].timeLeft -= Time.deltaTime;
			if(clipsToPlay[i].timeLeft <= 0.0f)
			{
				clipsToPlay.RemoveAt(i);
			}
		}

		if(!source.isPlaying) // We finished the clip
		{
			waitTimer -= Time.deltaTime;
			TriggerNextClip();
		}

		// Apply rotation
		leftEngine.localEulerAngles = new Vector3(Mathf.LerpAngle(leftEngine.localEulerAngles.x, -leftPitch, Time.deltaTime), 0.0f, 0.0f);
		rightEngine.localEulerAngles = new Vector3(Mathf.LerpAngle(rightEngine.localEulerAngles.x, -rightPitch, Time.deltaTime), 0.0f, 180.0f);


		// Apply motion
		motion += moveInput;
		motion *= 0.9f;

		switch(state)
		{
			case State.FREE_FLIGHT:
			{
				// Free flight, use motion and inputs
				transform.eulerAngles = new Vector3(-pitch, yaw, 0.0f);
				transform.position += transform.rotation * motion * Time.deltaTime;
				
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
				transform.position = Vector3.Lerp(preLandingPos, closestAsteroid.landingPos.position, landingProgress);
				transform.rotation = Quaternion.Lerp(preLandingRotation, Quaternion.identity, landingProgress);
				break;
			}
		}		

		// Controller Stick
		helmYaw *= 0.9f;
		helmPitch *= 0.9f;
		helm.localEulerAngles = new Vector3(-helmYaw, 0.0f, helmPitch) * 3.0f;
	}

	private void BeginTakeoff()
	{
		if(phase == StoryPhase.READY_TO_FLY)
			TriggerStoryPhase(StoryPhase.FLY_TO_MISSION1);
		state = State.FREE_FLIGHT;
		particles.Play();
		motion.y = 100.0f;
	}

	private void BeginLanding()
	{
		preLandingPos = transform.position;
		preLandingRotation = transform.rotation;
		landingProgress = 0.0f;
		particles.Stop();

		state = State.LANDING;
		if (closestAsteroid.gameObject == objectiveMission.gameObject)
		{
			TriggerStoryPhase(StoryPhase.PICKUP_CHEST);
		}
		else if (closestAsteroid.gameObject == objectiveFuelAsteroid.gameObject)
		{
			TriggerStoryPhase(StoryPhase.GET_FUEL_CAN);
		}

	}

	public void SetEmotion(Emotion emotion)
	{
		currentEmotion = emotion;
		faceMaterial.SetInt("_Face", (int)emotion);
	}

	public string GetOverlayText()
	{
		switch(state)
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

	// Brake engaged voice snippet
	private float timeSinceLastBrakeSnippet = 1.0f;

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
		moveInput = Vector3.zero;

		// Get inputs
		Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		switch(state)
		{
			case State.FREE_FLIGHT:
			{
				yaw += input.x * turnSpeed;
				helmYaw += input.x;
				leftPitch += input.x * 30.0f;
				rightPitch -= input.x * 30.0f;
				moveInput = new Vector3(0.0f, 0.0f, input.y);
				if (moveInput.sqrMagnitude > 1.0f)
				{
					moveInput.Normalize();
				}
				moveInput *= moveSpeed;

				timeSinceLastBrakeSnippet += Time.deltaTime;
				if (Mathf.Abs(input.x) > 0.0f || Mathf.Abs(input.y) > 0.0f && timeSinceLastBrakeSnippet >= 5.0f)
				{
					timeSinceLastBrakeSnippet = 0.0f;
				}

				if(closestAsteroid != null)
				{
					if(Input.GetKeyDown(KeyCode.Space))
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
				if(Input.GetKeyDown(KeyCode.Space))
				{
					BeginTakeoff();
				}
				break;
			}
		}

	}
}
