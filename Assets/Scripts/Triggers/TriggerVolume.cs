using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerVolume : MonoBehaviour
{
	private List<Collider> colliders;

	void Awake()
	{
		colliders = new List<Collider>(4);
	}

	private void OnTriggerEnter(Collider other)
	{
		colliders.Add(other);
	}

	private void OnTriggerExit(Collider other)
	{
		colliders.Remove(other);
	}

	public T Get<T>() where T : MonoBehaviour
	{
		T ret = null;
		foreach (Collider collider in colliders)
		{
			ret = collider.GetComponent<T>();
			if (ret != null)
				return ret;
		}
		return ret;
	}

	public bool Contains(MonoBehaviour t)
	{
		foreach (Collider collider in colliders)
		{
			if (collider.gameObject == t.gameObject)
				return true;
		}

		return false;
	}
}
