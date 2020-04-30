using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	public OneWayRenderPortal[] portals;
	public List<OneWayRenderPortal> currentPortals;

	public GameObject externalShipMesh;
	public TriggerVolume wholeShipVolume;

	public bool IsOnShip { get; private set; }

	private Player player;
	private Camera playerCamera;

    void Awake()
    {
		portals = FindObjectsOfType<OneWayRenderPortal>();
		currentPortals = new List<OneWayRenderPortal>(4);
		playerCamera = GetComponent<Camera>();
		player = GetComponentInParent<Player>();
	}

	void OnPreCull()
	{
		currentPortals.Clear();
		foreach (OneWayRenderPortal portal in portals)
			if (portal.Visible(playerCamera))
				currentPortals.Add(portal);

		Debug.Assert(currentPortals.Count <= 1, $"Two or more portals visible on screen this frame");

		foreach(OneWayRenderPortal portal in currentPortals)
		{
			portal.PrePortalRender(playerCamera);
			portal.Render(playerCamera);
			portal.PostPortalRender(playerCamera);
		}

		if(IsOnShip != wholeShipVolume.Contains(player))
		{
			IsOnShip = !IsOnShip;
			Debug.Log($"IsOnShip changed to {IsOnShip}");
			externalShipMesh.SetActive(!IsOnShip);
		}
	}

}
