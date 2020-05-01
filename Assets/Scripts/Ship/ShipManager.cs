using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
	public static ShipManager inst;

	public GameObject externalShipMesh;
	public GameObject internalDoorMesh;
	public TriggerVolume wholeShipVolume;

	public Transform interiorReferencePoint;
	public Transform exteriorReferencePoint;

	public GameObject roof;

	public bool LocalPlayerIsOnShip { get; private set; } = false;

	public Vector3 ShipSpaceToWorldSpace(Vector3 shipSpace)
	{
		return exteriorReferencePoint.localToWorldMatrix * interiorReferencePoint.worldToLocalMatrix * shipSpace;
	}

	public Vector3 WorldSpaceToShipSpace(Vector3 worldSpace)
	{
		return exteriorReferencePoint.localToWorldMatrix * interiorReferencePoint.worldToLocalMatrix * worldSpace;
	}

	void Awake()
    {
		inst = this;
		roof.SetActive(true);

	}

    void Update()
    {
		if (LocalPlayerIsOnShip != wholeShipVolume.Contains(Player.LocalPlayer))
		{
			LocalPlayerIsOnShip = !LocalPlayerIsOnShip;
			Debug.Log($"LocalPlayerIsOnShip changed to {LocalPlayerIsOnShip}");
			externalShipMesh.SetActive(!LocalPlayerIsOnShip);
			internalDoorMesh.SetActive(LocalPlayerIsOnShip);
		}
	}
}
