using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SinglePressButton
{
	private bool pressedThisFrame;
	private bool pressedPreviousFrame;

	public void Press()
	{
		pressedThisFrame = true;
	}

	public void NextFrame()
	{
		pressedPreviousFrame = pressedThisFrame;
		pressedThisFrame = false;
	}

	public void Zero()
	{
		pressedPreviousFrame = pressedThisFrame = false;
	}

	public static implicit operator bool(SinglePressButton button)
	{
		return button.pressedThisFrame && !button.pressedPreviousFrame;
	}
}

public class PlayerInputs
{
	// Input vectors
	public Vector2 mouseDelta = Vector2.zero;
	public Vector3 move = Vector3.zero;
	public SinglePressButton interact;

	public void NextFrame()
	{
		mouseDelta = Vector2.zero;
		move = Vector3.zero;
		interact.NextFrame();
	}
}
