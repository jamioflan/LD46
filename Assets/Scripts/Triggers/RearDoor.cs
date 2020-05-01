using UnityEngine;

public class RearDoor : ButtonTarget
{
	public float speed = 1.0f;

	private float progress = 0.0f;
	private float target = 0.0f;

	void Update()
    {
		progress = Mathf.Lerp(progress, target, speed * Time.deltaTime);
		transform.localEulerAngles = new Vector3(Mathf.LerpAngle(0.0f, 120.0f, progress), 0.0f, 0.0f);
	}

	public override bool CanPressButton(ButtonAction action)
	{
		return action == ButtonAction.OPEN_DOOR || action == ButtonAction.CLOSE_DOOR;
	}

	public override void PressButton(ButtonAction action)
	{
		switch(action)
		{
			case ButtonAction.OPEN_DOOR:
				Story.inst.DoorOpened();
				target = 1.0f;
				break;
			case ButtonAction.CLOSE_DOOR:
				target = 0.0f;
				break;
			default:
				Debug.LogError($"RearDoor received action {action} unexpectedly");
				break;
		}
	}
}
