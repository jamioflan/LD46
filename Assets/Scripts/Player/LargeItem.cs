using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents an item that must be hauled physically,
// rather than in an inventory slot.
public class LargeItem : Interactable
{
	public Player GetPlayer()
	{
		foreach (Player player in Player.players)
		{
			if (player.currentLargeItem == this)
				return player;
		}

		return null;
	}

	public override bool CanInteract()
	{
		return GetPlayer() == null;
	}

	public virtual void ServerPlayerPickup(Player player)
	{
		foreach (Collider collider in GetComponentsInChildren<Collider>())
			collider.enabled = false;
	}
	public virtual void UpdateHeldByLocalPlayer(PlayerInputs inputs) { }
	public virtual void ServerPlayerDrop(Player player)
	{
		foreach (Collider collider in GetComponentsInChildren<Collider>())
			collider.enabled = true;
	}

	public override void ServerInteract(Player player)
	{
		player.ServerPickupLarge(this);
	}
}
