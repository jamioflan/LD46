using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureSphere : MonoBehaviour
{
	public Asteroid asteroid;

    // Start is called before the first frame update
    void Start()
    {
		asteroid = GetComponentInParent<Asteroid>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
		ShipControls ship = other.GetComponentInParent<ShipControls>();
		if(ship != null)
		{
			ship.closestAsteroid = asteroid;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		ShipControls ship = other.GetComponentInParent<ShipControls>();
		if (ship != null)
		{
			ship.closestAsteroid = null;
		}
	}
}
