using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throttle : MonoBehaviour
{
	private enum State
	{
		NEGATIVE,
		NEUTRAL,
		POSITIVE,
	}

	public Transform handle;
	public float angleMin, angleMax;

	// From lowest to highest
	public MeshRenderer[] positiveLightbars;
	public MeshRenderer zeroLightbar;
	// From lowest magnitude to highest magnitude
	public MeshRenderer[] negativeLightbars;

	public Material activeMaterial;
	public Material inactiveMaterial;

	public Gradient positveGradient;
	public Color neutralColour;
	public Gradient negativeGradient;

	private float currentThrottle;
	private float targetThrottle;
	private float throttleMin = -2.0f;
	private float throttleMax = 4.0f;

	private State state;

	public void SetValues(float throttle, float throttleMin, float throttleMax)
	{
		targetThrottle = throttle;
		this.throttleMin = throttleMin;
		this.throttleMax = throttleMax;
	}

    private void Update()
    {
		currentThrottle = targetThrottle;// Mathf.Lerp(currentThrottle, targetThrottle, 5.0f * Time.deltaTime);
		float throttleParametric = (currentThrottle - throttleMin) / (throttleMax - throttleMin);
		handle.localEulerAngles = new Vector3(Mathf.Lerp(angleMin, angleMax, throttleParametric), 0.0f, 0.0f);

		if(currentThrottle > Mathf.Epsilon)
		{
			SetState(State.POSITIVE);
			for (int i = 0; i < positiveLightbars.Length; i++)
			{
				float bound = i * throttleMax / (positiveLightbars.Length - 1);
				if(currentThrottle >= bound - Mathf.Epsilon)
				{
					positiveLightbars[i].sharedMaterial = activeMaterial;
				}
				else
				{
					positiveLightbars[i].sharedMaterial = inactiveMaterial;
				}
			}

			activeMaterial.color = positveGradient.Evaluate(currentThrottle / throttleMax);
			activeMaterial.SetColor("_EmissionColor", activeMaterial.color);
		}
		else if(currentThrottle < -Mathf.Epsilon)
		{
			SetState(State.NEGATIVE);
			for (int i = 0; i < negativeLightbars.Length; i++)
			{
				float bound = i * throttleMin / (negativeLightbars.Length - 1);
				if (currentThrottle <= bound + Mathf.Epsilon)
				{
					negativeLightbars[i].sharedMaterial = activeMaterial;
				}
				else
				{
					negativeLightbars[i].sharedMaterial = inactiveMaterial;
				}
			}

			activeMaterial.color = negativeGradient.Evaluate(currentThrottle / throttleMin);
			activeMaterial.SetColor("_EmissionColor", activeMaterial.color);
		}
		else // Zero-ed
		{
			SetState(State.NEUTRAL);
		}
	}

	private void SetState(State set)
	{
		if (state != set)
		{
			state = set;
			switch (state)
			{
				case State.NEGATIVE:
				{
					zeroLightbar.sharedMaterial = inactiveMaterial;
					foreach (MeshRenderer mr in positiveLightbars)
						mr.sharedMaterial = inactiveMaterial;
					foreach (MeshRenderer mr in negativeLightbars)
						mr.sharedMaterial = activeMaterial;
					break;
				}
				case State.NEUTRAL:
				{
					zeroLightbar.sharedMaterial = activeMaterial;
					foreach (MeshRenderer mr in positiveLightbars)
						mr.sharedMaterial = inactiveMaterial;
					foreach (MeshRenderer mr in negativeLightbars)
						mr.sharedMaterial = inactiveMaterial;

					activeMaterial.color = neutralColour;
					activeMaterial.SetColor("_EmissionColor", neutralColour);

					break;
				}
				case State.POSITIVE:
				{
					zeroLightbar.sharedMaterial = inactiveMaterial;
					foreach (MeshRenderer mr in negativeLightbars)
						mr.sharedMaterial = inactiveMaterial;
					foreach (MeshRenderer mr in positiveLightbars)
						mr.sharedMaterial = activeMaterial;
					break;
				}
			}
		}
	}
}
