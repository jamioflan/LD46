using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShipAction
{
	OPEN_DOOR,
	CLOSE_DOOR,
}

public class Button : Interactable
{
	public ShipAction action;

	public override bool CanInteract()
	{
		return true;
	}

	public override void ServerInteract(Player player)
	{
		
	}
}
