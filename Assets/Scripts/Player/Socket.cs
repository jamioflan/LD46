using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : Interactable
{
	public LargeItem.ItemType requiredItem;
	public bool isFilled = false;
	public bool triggerStory = false;
	public Story.StoryPhase phaseToTrigger;

	public string overlayTextEmpty;
	public string overlayTextFull;
	public string overlayTextPressF;

	public bool triggerStoryOnHover = false;
	public Story.StoryPhase phaseToTriggerOnHover;

	public override string GetHoverText(Player player)
	{
		if(player.currentLargeItem != null &&
			player.currentLargeItem.type == requiredItem)
		{
			return overlayTextPressF;
		}
		else if(isFilled)
		{
			return overlayTextFull;
		}
		else
		{
			return overlayTextEmpty;
		}
	}

	public override void ClientHover(Player player)
	{
		base.ClientHover(player);
		if (triggerStoryOnHover)
			Story.inst.TriggerStoryPhase(phaseToTriggerOnHover);
	}

	public override bool CanInteract(Player player)
	{
		return !isFilled &&
			player.currentLargeItem != null &&
			player.currentLargeItem.type == requiredItem;
	}

	public override void ServerInteract(Player player)
	{
		LargeItem item = player.currentLargeItem;
		player.ServerPickupLarge(null);
		Destroy(item.gameObject);
		isFilled = true;

		if(triggerStory)
		{
			Story.inst.TriggerStoryPhase(phaseToTrigger);
		}
	}

    protected override void Update()
    {
		base.Update();
    }
}
