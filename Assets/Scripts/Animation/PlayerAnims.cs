using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnims : MonoBehaviour
{
	public Transform leftHand, rightHand;

	public float handBobSpeed = 1.0f;
	public float handBobMagnitude = 1.0f;

	private float bob;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		bob += Time.deltaTime * handBobSpeed;
		leftHand.localPosition = new Vector3(0.0f, Mathf.Sin(bob) * handBobMagnitude, 0.0f);
		rightHand.localPosition = new Vector3(0.0f, Mathf.Sin(-bob) * handBobMagnitude, 0.0f);
	}
}
