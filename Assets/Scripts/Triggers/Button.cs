using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonAction
{
	OPEN_DOOR,
	CLOSE_DOOR,
}

public class Button : Interactable
{
	public ButtonAction action;
	public ButtonTarget target;
	public string hoverText;

	public override string GetHoverText(Player player) { return hoverText; }

	public override bool CanInteract(Player player)
	{
		return target != null && target.CanPressButton(action);
	}

	public override void ServerInteract(Player player)
	{
		if(target != null)
		{
			target.PressButton(action);
		}
	}
}
