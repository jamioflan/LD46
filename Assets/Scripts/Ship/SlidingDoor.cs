using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
	public float openSpeed;
	public Vector3 openDirection = Vector3.forward;
	public Transform doorTransform;
	public float openPitch = 1.0f;
	public float closedPitch = 1.2f;

	private TriggerVolume trigger;
	private AudioSource source;
	public bool IsLocked { get; private set; } = false;
	private bool shouldOpen = false;
	private float openProgress = 0.0f;

    void Awake()
    {
		trigger = GetComponentInChildren<TriggerVolume>();
		source = GetComponentInChildren<AudioSource>();
	}

    void Update()
    {
		// See if we should be open, if situation has changed, trigger SFX
		bool isPlayerInVolume = trigger.Get<Player>() != null;
		if (isPlayerInVolume != shouldOpen)
			source.Play();
		shouldOpen = isPlayerInVolume;

		// Open or close accordingly
		if (shouldOpen)
			openProgress += Time.deltaTime * openSpeed;
		else
			openProgress -= Time.deltaTime * openSpeed;
		openProgress = Mathf.Clamp01(openProgress);

		// Update the door transform and adjust the sound pitch
		doorTransform.localPosition = openDirection * openProgress;
		source.pitch = openPitch + (closedPitch - openPitch) * openProgress;
	}
}
