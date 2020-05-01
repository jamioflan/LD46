using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnims : MonoBehaviour
{
	public Transform topTeeth;
	public Transform leftRadar, rightRadar;

	public float spin = 10.0f;
	public float chomp = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		leftRadar.localEulerAngles = new Vector3(0.0f, leftRadar.localEulerAngles.y + Time.deltaTime * spin, 0.0f);
		rightRadar.localEulerAngles = new Vector3(0.0f, rightRadar.localEulerAngles.y - Time.deltaTime * spin, 0.0f);

		topTeeth.localPosition = new Vector3(0.0f, Mathf.Sin(Time.time * chomp) * 8.0f, 0.0f);
	}
}
