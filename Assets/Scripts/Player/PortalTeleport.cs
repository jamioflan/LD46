using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleport : MonoBehaviour
{
	List<PortalTraveller> trackedTravellers;
	public PortalTeleport target;

	void Awake()
    {
		trackedTravellers = new List<PortalTraveller>();
	}

	void FixedUpdate()
	{
		HandleTravellers();
	}

	void HandleTravellers()
	{
		for (int i = 0; i < trackedTravellers.Count; i++)
		{
			PortalTraveller traveller = trackedTravellers[i];
			Transform travellerT = traveller.transform;
			var m = target.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

			Vector3 offsetFromPortal = travellerT.position - transform.position;
			int portalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
			int portalSideOld = System.Math.Sign(Vector3.Dot(traveller.previousOffsetFromPortal, transform.forward));
			// Teleport the traveller if it has crossed from one side of the portal to the other
			if (portalSide != portalSideOld)
			{
				//var positionOld = travellerT.position;
				//var rotOld = travellerT.rotation;
				traveller.Teleport(transform, target.transform, m.GetColumn(3), m.rotation);
				//traveller.graphicsClone.transform.SetPositionAndRotation(positionOld, rotOld);
				// Can't rely on OnTriggerEnter/Exit to be called next frame since it depends on when FixedUpdate runs
				target.OnTravellerEnterPortal(traveller);
				trackedTravellers.RemoveAt(i);
				i--;
			}
			else
			{
				//traveller.graphicsClone.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
				//UpdateSliceParams (traveller);
				traveller.previousOffsetFromPortal = offsetFromPortal;
			}
		}
	}

	void OnTravellerEnterPortal(PortalTraveller traveller)
	{
		if (!trackedTravellers.Contains(traveller))
		{
			traveller.EnterPortalThreshold();
			traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
			trackedTravellers.Add(traveller);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		var traveller = other.GetComponent<PortalTraveller>();
		if (traveller)
		{
			OnTravellerEnterPortal(traveller);
		}
	}

	void OnTriggerExit(Collider other)
	{
		var traveller = other.GetComponent<PortalTraveller>();
		if (traveller && trackedTravellers.Contains(traveller))
		{
			traveller.ExitPortalThreshold();
			trackedTravellers.Remove(traveller);
		}
	}

	void OnValidate()
	{
		if (target != null)
		{
			target.target = this;
		}
	}
}
