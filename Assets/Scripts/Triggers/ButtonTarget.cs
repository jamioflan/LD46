using UnityEngine;

public abstract class ButtonTarget : MonoBehaviour
{
	public abstract bool CanPressButton(ButtonAction action);
	public abstract void PressButton(ButtonAction action);
}
