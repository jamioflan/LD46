using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyShape : MonoBehaviour
{
	public Transform chest, breast, shoulders, stomach, hips;

	public const float kSliderMin = 0.8f;
	public const float kSliderMax = 1.2f;

	[Range(0, 1)]
	public float breastSize;

	[Range(kSliderMin, kSliderMax)]
	public float shoulderSize;
	[Range(kSliderMin, kSliderMax)]
	public float chestSize;
	[Range(kSliderMin, kSliderMax)]
	public float stomachSize;
	[Range(kSliderMin, kSliderMax)]
	public float hipsSize;

	public float timeToNext = 1.0f;


    void Awake()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
		timeToNext -= Time.deltaTime;
		if(timeToNext <= 0.0f)
		{
			timeToNext = 1.0f;

			Randomise();

			Apply();
		}
	}

	private void Apply()
	{
		chest.localScale = Vector3.one * chestSize;
		breast.localScale = Vector3.one * chestSize;
		breast.localPosition = new Vector3(0.0f, 0.0625f, Mathf.Lerp(0.0f, 0.025f, breastSize * (chestSize - kSliderMin) / (kSliderMax - kSliderMin)));
		shoulders.localScale = Vector3.one * shoulderSize;
		stomach.localScale = Vector3.one * stomachSize;
		hips.localScale = Vector3.one * hipsSize;
	}

	private void Randomise()
	{
		breastSize = Random.Range(0f, 1f);
		chestSize = Random.Range(kSliderMin, kSliderMax);
		shoulderSize = Random.Range(kSliderMin, kSliderMax);
		stomachSize = Random.Range(kSliderMin, kSliderMax);
		hipsSize = Random.Range(kSliderMin, kSliderMax);
	}
}
