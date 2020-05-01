using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents an item that must be hauled physically,
// rather than in an inventory slot.
public class LargeItem : Interactable
{
	public enum ItemType
	{
		COMPONENT,
		FUEL_CAN,
		CHEST,
	}

	public ItemType type;
	public string pickupText;

	public override string GetHoverText(Player player)
	{
		if(CanInteract(player))
		{
			return pickupText;
		}
		else
		{
			return string.Empty;
		}
	}

	public Player GetPlayer()
	{
		foreach (Player player in Player.players)
		{
			if (player.currentLargeItem == this)
				return player;
		}

		return null;
	}

	public override bool CanInteract(Player player)
	{
		return player.currentLargeItem == null && GetPlayer() == null;
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
