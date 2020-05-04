using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Story : MonoBehaviour
{
	public static Story inst;

	// Serialized vars
	public AudioSource music;
	public AudioSource explosion;
	public ObjectiveMarker objectiveEngine, objectiveDoor, objectiveHelm, objectiveMission, objectivePostbox, objectiveFuelAsteroid, objectiveFuelCan, objectiveFuelDropoff, objectiveDeath, objectiveChest;
	public ObjectiveMarker objectiveGoInside, objectiveGoOutside;
	public TMPro.TextMeshProUGUI endText;
	public TMPro.TextMeshProUGUI questLog1, questLog2;
	public Image fadeToBlack;
	public float fadeInTimeRemaining = 10.0f;


	public bool CanPickChest() { return phase >= StoryPhase.PICKUP_CHEST; }
	public bool CanFly() { return phase >= StoryPhase.READY_TO_FLY; }

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

	// private
	private StoryPhase phase = (StoryPhase)(-1);
	private bool shouldFadeOut = false;
	private ObjectiveMarker currentObjectiveMarker;


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
					ShipNarrator.inst.QueueVoiceLine(Emotion.CROSS, VoiceClip.WATCH_OUT, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.DOTS, VoiceClip.REBOOTING, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.NEUTRAL_HAPPY, VoiceClip.CAPTAIN_OKAY, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.SCAN, VoiceClip.SCANNING_SHIP, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.CRY, VoiceClip.ENGINE_DAMAGED, ShipNarrator.Priority.STORY);
					questLog1.text = "Check Robin's engine";
					currentObjectiveMarker = objectiveEngine;
					break;
				case StoryPhase.FETCH_COMPONENT:
					ShipNarrator.inst.QueueVoiceLine(Emotion.DEAD, VoiceClip.AM_I_GONNA_DIE, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.LIGHTBULB, VoiceClip.WE_NEED_A_NEW_X, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.THINKING, VoiceClip.MAYBE_ON_ASTEROID, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.HAPPY, VoiceClip.CHAT1, ShipNarrator.Priority.CHAT);
					questLog1.text = "Find the component";
					music.Play();
					currentObjectiveMarker = objectiveDoor;
					break;
				case StoryPhase.HAVE_COMPONENT:
					ShipNarrator.inst.QueueVoiceLine(Emotion.HAPPY, VoiceClip.FOUND_IT, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.CUTE, VoiceClip.CHAT2, ShipNarrator.Priority.CHAT);
					questLog1.text = "Fit the component into the engine";
					currentObjectiveMarker = objectiveEngine;
					break;
				case StoryPhase.READY_TO_PLACE:
					ShipNarrator.inst.QueueVoiceLine(Emotion.EYES_SHUT, VoiceClip.FIX_ME, ShipNarrator.Priority.STORY);
					currentObjectiveMarker = null;
					break;
				case StoryPhase.READY_TO_FLY:
					ShipNarrator.inst.QueueVoiceLine(Emotion.NEAR_DEATH_EXPERIENCE, VoiceClip.FIXED_GRATITUDE, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.NEAR_DEATH_EXPERIENCE, VoiceClip.LETS_GO_DO_MISSION, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.LIGHTBULB, VoiceClip.HOW_TO_HELM, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.HAPPY, VoiceClip.SPACE_TO_TAKEOFF, ShipNarrator.Priority.STORY);
					questLog1.text = "Go to the helm and takeoff";
					currentObjectiveMarker = objectiveHelm;
					break;
				case StoryPhase.FLY_TO_MISSION1:
					ShipNarrator.inst.QueueVoiceLine(Emotion.THINKING, VoiceClip.OTHER_CONTROLS, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.SCAN, VoiceClip.MISSION_QUEST_MARKER, ShipNarrator.Priority.STORY);

					ShipNarrator.inst.QueueVoiceLine(Emotion.HAPPY, VoiceClip.CHAT3, ShipNarrator.Priority.CHAT);
					ShipNarrator.inst.QueueVoiceLine(Emotion.CUTE, VoiceClip.CHAT4, ShipNarrator.Priority.CHAT);
					questLog1.text = "Fly to the postbox";
					currentObjectiveMarker = objectiveMission;
					break;
				case StoryPhase.FLY_TO_REFUEL:
					ShipNarrator.inst.QueueVoiceLine(Emotion.LIGHTBULB, VoiceClip.IM_THIRSTY, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.DIZZY, VoiceClip.CAN_GET_FUEL, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.THINKING, VoiceClip.FUELING_STATION_OVER_THERE, ShipNarrator.Priority.STORY);
					questLog1.text = "Fly to the postbox";
					questLog1.fontStyle = TMPro.FontStyles.Strikethrough;
					questLog2.text = "Fly to the fuel station";
					currentObjectiveMarker = objectiveFuelAsteroid;
					break;
				case StoryPhase.GET_FUEL_CAN:
					ShipNarrator.inst.QueueVoiceLine(Emotion.PERFECT10, VoiceClip.PERFECT_LANDING, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.HAPPY, VoiceClip.GRAB_A_FUEL, ShipNarrator.Priority.STORY);
					//ShipNarrator.inst.QueueVoiceLine(Emotion.HAPPY, VoiceClip.CHAT5, ShipNarrator.Priority.CHAT);
					questLog1.text = "Fly to the postbox";
					questLog1.fontStyle = TMPro.FontStyles.Strikethrough;
					questLog2.text = "Pick up the fuel can";
					currentObjectiveMarker = objectiveFuelCan;
					break;
				case StoryPhase.PLACE_FUEL_CAN:
					ShipNarrator.inst.QueueVoiceLine(Emotion.HAPPY, VoiceClip.PUT_IT_IN_THE_FUEL_HATCH, ShipNarrator.Priority.STORY);
					questLog1.text = "Fly to the postbox";
					questLog1.fontStyle = TMPro.FontStyles.Strikethrough;
					questLog2.text = "Fill the fuel intake";
					currentObjectiveMarker = objectiveFuelDropoff;
					break;
				case StoryPhase.FLY_TO_MISSION2:
					ShipNarrator.inst.QueueVoiceLine(Emotion.TICK, VoiceClip.BACK_TO_THE_MISSION, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.PERFECT10, VoiceClip.GETTING_CLOSE, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.CUTE, VoiceClip.LOOK_AT_THAT_LOVELY_THING, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.NEAR_DEATH_EXPERIENCE, VoiceClip.CHAT6, ShipNarrator.Priority.CHAT);
					ShipNarrator.inst.QueueVoiceLine(Emotion.HAPPY, VoiceClip.CHAT7, ShipNarrator.Priority.CHAT);
					questLog1.text = "Fly to the postbox";
					currentObjectiveMarker = objectiveMission;
					break;
				case StoryPhase.PICKUP_CHEST:
					ShipNarrator.inst.QueueVoiceLine(Emotion.HAPPY, VoiceClip.GRAB_CHEST, ShipNarrator.Priority.STORY);
					currentObjectiveMarker = objectiveChest;
					questLog1.text = "Find the chest in the hold";
					break;
				case StoryPhase.LOOK_AT_CHEST:
					ShipNarrator.inst.QueueVoiceLine(Emotion.HAPPY, VoiceClip.LIFT, ShipNarrator.Priority.STORY);
					questLog1.text = "Pick up the chest";
					currentObjectiveMarker = objectiveChest;
					break;
				case StoryPhase.DROPOFF_CHEST:
					ShipNarrator.inst.QueueVoiceLine(Emotion.THINKING, VoiceClip.PUT_IN_POSTBOX, ShipNarrator.Priority.STORY);
					questLog1.text = "Put the chest in the postbox";
					currentObjectiveMarker = objectivePostbox;
					break;
				case StoryPhase.FLY_TO_DOOM:
					ShipNarrator.inst.QueueVoiceLine(Emotion.PERFECT10, VoiceClip.DELIVERY_SUCCESSFUL, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.THINKING, VoiceClip.NEW_COORDINATES, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.CRY, VoiceClip.YOU_HAVE_TO_KILL_ME, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.SAD, VoiceClip.TIME_IS_UP, ShipNarrator.Priority.STORY);
					ShipNarrator.inst.QueueVoiceLine(Emotion.CRY, VoiceClip.CHAT8, ShipNarrator.Priority.CHAT);
					ShipNarrator.inst.QueueVoiceLine(Emotion.SAD, VoiceClip.CHAT9, ShipNarrator.Priority.CHAT);
					questLog1.text = "Fly to the postbox";
					questLog2.text = "Visit the Rainbow Rocks";
					questLog2.fontStyle = TMPro.FontStyles.Strikethrough;
					currentObjectiveMarker = objectiveDeath;
					break;
				case StoryPhase.DEATH:
					shouldFadeOut = true;
					endText.enabled = true;
					endText.text = @"You succesfully delivered Robin to the SpaceCo decomissioning facility.
Well done employee!";
					currentObjectiveMarker = null;
					ShipNarrator.inst.QueueVoiceLine(Emotion.HAPPY, VoiceClip.GOODBYE, ShipNarrator.Priority.STORY);
					break;
				case StoryPhase.LIFE:
					shouldFadeOut = true;
					endText.enabled = true;
					endText.text = @"You did not successfully deliver Robing to the SpaceCo decommisioning facility.

A requisitioning team will be dispatched to your quadrant shortly. 

This will not look good on your employee record.";
					ShipNarrator.inst.QueueVoiceLine(Emotion.HAPPY, VoiceClip.WE_ESCAPED, ShipNarrator.Priority.STORY);
					currentObjectiveMarker = null;
					break;
				case StoryPhase.PLAYER_WAS_AN_ID:
					shouldFadeOut = true;
					endText.enabled = true;
					endText.text = "You were lost to the inky void";
					ShipNarrator.inst.QueueVoiceLine(Emotion.SAD, VoiceClip.GOODBYE, ShipNarrator.Priority.STORY);
					currentObjectiveMarker = null;
					break;
			}
		}


	}

	public void DoorOpened()
	{
		currentObjectiveMarker = null;
	}

	public void LookedAtChest()
	{
		if (phase == StoryPhase.LOOK_AT_CHEST)
			TriggerStoryPhase(StoryPhase.PICKUP_CHEST);
	}

	public void CheckTakeoffTrigger()
	{
		if (phase == StoryPhase.READY_TO_FLY)
			TriggerStoryPhase(StoryPhase.FLY_TO_MISSION1);
	}

	public void CheckLandingTrigger(GameObject landedOn)
	{
		if (landedOn == objectiveMission.gameObject)
		{
			TriggerStoryPhase(StoryPhase.PICKUP_CHEST);
		}
		else if (landedOn == objectiveFuelAsteroid.gameObject)
		{
			TriggerStoryPhase(StoryPhase.GET_FUEL_CAN);
		}
	}

	// Unity methods
	private void Awake()
    {
		inst = this;

		fadeInTimeRemaining = 1.0f;
		TriggerStoryPhase(StoryPhase.INTRO);
	}

    private void Update()
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

		if (currentObjectiveMarker != null)
		{
			if (currentObjectiveMarker.isInsideShip != ShipManager.inst.LocalPlayerIsOnShip)
			{
				OnScreenObjectiveMarker.the.target = currentObjectiveMarker.isInsideShip ?
					ShipManager.inst.ShipSpaceToWorldSpace(currentObjectiveMarker.transform.position) :
					ShipManager.inst.WorldSpaceToShipSpace(currentObjectiveMarker.transform.position);
				OnScreenObjectiveMarker.the.viewer = Camera.main;// PlayerCamera.inst.GetPortalCameraCurrentlyInUse();
			}
			else
			{
				OnScreenObjectiveMarker.the.viewer = Camera.main;
				OnScreenObjectiveMarker.the.target = currentObjectiveMarker.transform.position;
			}
		}
		else
		{
			OnScreenObjectiveMarker.the.target = Vector3.zero;
		}
	}
}
