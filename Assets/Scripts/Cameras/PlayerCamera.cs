using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	public static PlayerCamera inst;

	public OneWayRenderPortal[] portals;
	public List<OneWayRenderPortal> currentPortals;

	private Player player;
	private Camera playerCamera;

	public Camera GetPortalCameraCurrentlyInUse()
	{
		return currentPortals.Count > 0 ? currentPortals[0].GetCamera() : null;
	}

    void Awake()
    {
		inst = this;

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

		//Debug.Assert(currentPortals.Count <= 1, $"Two or more portals visible on screen this frame");

		foreach(OneWayRenderPortal portal in currentPortals)
		{
			portal.PrePortalRender(playerCamera);
			portal.Render(playerCamera);
			portal.PostPortalRender(playerCamera);
		}
	}

}
