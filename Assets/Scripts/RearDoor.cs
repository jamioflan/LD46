using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearDoor : MonoBehaviour
{
	public static RearDoor theDoor;

    // Start is called before the first frame update
    void Start()
    {
		theDoor = this;

	}

    // Update is called once per frame
    void Update()
    {
		progress = Mathf.Lerp(progress, target, speed * Time.deltaTime);
		transform.localEulerAngles = new Vector3(Mathf.LerpAngle(0.0f, -120.0f, progress), 0.0f, 0.0f);
	}

	private float progress = 0.0f;
	private float target = 0.0f;
	public float speed = 1.0f;

	public void Open()
	{
		target = 1.0f;
	}

	public void Close()
	{
		target = 0.0f;
	}
}
