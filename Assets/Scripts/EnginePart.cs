﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnginePart : MonoBehaviour
{
	public float slideTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(slideTimer > 0.0f)
		{
			slideTimer -= Time.deltaTime;

		}

		transform.localPosition = new Vector3(slideTimer, 0.0f, 0.0f);
    }
}
