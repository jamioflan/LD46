using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipNarrator : MonoBehaviour
{
	public static ShipNarrator inst;

	public enum Priority
	{
		STORY,
		CONTEXT,
		CHAT,
	}

	private class NextClip
	{
		public VoiceClip clip;
		public Emotion face;
		public Priority priority;
		public float timeLeft = float.PositiveInfinity;
		public float pause = -1;
	}

	public AudioClip[] clips;
	public float timeToNext = 1.0f;
	public MeshRenderer aFace;
	public Emotion currentEmotion;
	public AudioSource source;

	private Material faceMaterial;
	private List<NextClip> clipsToPlay = new List<NextClip>();
	private NextClip currentClip;
	private float waitTimer = 0.0f;

	public void SetEmotion(Emotion emotion)
	{
		currentEmotion = emotion;
		faceMaterial.SetInt("_Face", (int)emotion);
	}

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
			if (i > 0 && waitTimer > 0.0f)
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
		if (next != null)
		{
			source.Stop();
			source.clip = clips[(int)next.clip];
			source.Play();

			SetEmotion(next.face);

			if (next.priority == Priority.STORY)
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

	private void Awake()
    {
		inst = this;
		faceMaterial = aFace.sharedMaterial;
	}

    private void Update()
    {
		for (int i = clipsToPlay.Count - 1; i >= 0; i--)
		{
			clipsToPlay[i].timeLeft -= Time.deltaTime;
			if (clipsToPlay[i].timeLeft <= 0.0f)
			{
				clipsToPlay.RemoveAt(i);
			}
		}

		if (!source.isPlaying) // We finished the clip
		{
			waitTimer -= Time.deltaTime;
			TriggerNextClip();
		}
	}
}
