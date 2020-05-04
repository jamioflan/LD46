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

public struct FloatAccumulator
{
	public static implicit operator float(FloatAccumulator accumulator)
	{
		if (accumulator.numFrames == 0)
			return 0.0f;
		return accumulator.accumulation / accumulator.numFrames;
	}

	public static FloatAccumulator operator +(FloatAccumulator accumulator, float delta)
	{
		accumulator.Accumulate(delta);
		return accumulator;
	}

	public override string ToString()
	{
		return $"Accumulator:{accumulation} over {numFrames} frames";
	}

	public void Reset()
	{
		accumulation = 0.0f;
		numFrames = 0;
	}

	public void Accumulate(float amount)
	{
		accumulation += amount;
		numFrames++;
	}

	private float accumulation;
	private int numFrames;
}

public struct PlayerInputs
{
	// Input vectors
	public Vector2 mouseDelta ;
	public Vector3 move;
	public Vector3 rotate;
	public FloatAccumulator throttle;
	public SinglePressButton interact;
	public SinglePressButton jump;

	private bool consumed;

	public void ClearInputs()
	{
		// This is called when the FixedUpdate loop is done with these inputs. 
		// However, we might not get another update before the next FixedUpdate
		// So mark for consumption and only clear when we get new data
		consumed = true;
	}

	public void BeginGathering()
	{
		// If we have consumed these values already, start fresh
		// If there was no FixedUpdate, we may not have consumed them.
		// In which case, accumulate.
		if(consumed)
		{
			mouseDelta = Vector2.zero;
			move = Vector3.zero;
			rotate = Vector3.zero;
			throttle.Reset();
			interact.NextFrame();
			jump.NextFrame();
		}
	}
}
