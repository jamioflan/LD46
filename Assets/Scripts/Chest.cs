using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : LargeItem
{
	public override void ClientHover(Player player)
	{
		base.ClientHover(player);
		Story.inst.LookedAtChest();
	}

	public override bool CanInteract(Player player)
	{
		return base.CanInteract(player) && Story.inst.CanPickChest();
	}
}
