using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TheText : MonoBehaviour
{
	public static TheText the;
	public TMPro.TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
		the = this;
		text = GetComponent<TMPro.TextMeshProUGUI>();

	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
