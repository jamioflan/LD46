using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
	public virtual string GetHoverText(Player player) { return $"Interact with {name}"; }

	public abstract bool CanInteract(Player player);

	public void ClientRequestInteract(Player player)
	{
		// if(singleplayer)
		ServerInteract(player);
	}

	public abstract void ServerInteract(Player player);
	public virtual void ClientHover(Player player) { }
	public virtual void ClientUnhover(Player player) { }



	protected virtual void Awake()
	{
	}
	protected virtual void Update()
	{
	}
	protected virtual void FixedUpdate()
	{
	}
}
