using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBox : MonoBehaviour
{
	public enum Event
	{
		NEED_FUEL,
		DIES,
		LIVES,
	}

	public Event trigger;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{

			switch (trigger)
			{
				case Event.NEED_FUEL:
				{
					Ship.theShip.TriggerStoryPhase(Ship.StoryPhase.FLY_TO_REFUEL);
					break;
				}
				case Event.DIES:
				{
					Ship.theShip.TriggerStoryPhase(Ship.StoryPhase.DEATH);
					break;
				}
				case Event.LIVES:
				{
					Ship.theShip.TriggerStoryPhase(Ship.StoryPhase.LIFE);
					break;
				}
			}
	}
}
