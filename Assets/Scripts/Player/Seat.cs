using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Seat : Interactable
{
	public Transform standingPos;

	public Player GetPlayer()
	{
		foreach(Player player in Player.players)
		{
			if (player.currentSeat == this)
				return player;
		}

		return null;
	}

	public override bool CanInteract(Player player)
	{
		return player.currentSeat == null && GetPlayer() == null;
	}

	public virtual void ServerPlayerEnter(Player player) { }
	public virtual void UpdateControlledByLocalPlayer(PlayerInputs inputs) { }
	public virtual void ServerPlayerLeave(Player player) { }

	public override void ServerInteract(Player player)
	{
		player.ServerSit(this);
	}
}
