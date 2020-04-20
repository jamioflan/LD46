using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveMarker : MonoBehaviour
{
	public static ObjectiveMarker the;

	private RectTransform rect;
	private Image image;
	public Sprite left, right, here;
	public RectTransform canvasRect;
	public Transform target;

    // Start is called before the first frame update
    void Awake()
    {
		the = this;
		rect = GetComponent<RectTransform>();
		image = GetComponent<Image>();
	}

    // Update is called once per frame
    void Update()
    {
		if (target != null)
		{
			image.enabled = true;

			Vector3 objectivePosition = target.position;
			objectivePosition.y += 1.0f;

			Vector3 ray = objectivePosition - Camera.main.transform.position;
			bool behind = Vector3.Dot(ray, Camera.main.transform.forward) < 0.0f;


			Vector2 screenPos = Camera.main.WorldToScreenPoint(objectivePosition);

			Vector2 canvasPos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out canvasPos);

			image.sprite = here;

			if (canvasPos.x > Screen.width / 2 - 100)
			{
				image.sprite = right;
				canvasPos.x = Screen.width / 2 - 100;
			}
			if (canvasPos.x < 100 - Screen.width / 2)
			{
				image.sprite = left;
				canvasPos.x = 100 - Screen.width / 2;
			}

			if (canvasPos.y > Screen.height / 2 - 100)
				canvasPos.y = Screen.height / 2 - 100;
			if (canvasPos.y < 100 - Screen.height / 2)
				canvasPos.y = 100 - Screen.height / 2;

			Vector3 targetPos = Vector3.zero ;
			if (behind)
			{
				if(Vector3.Dot(ray, -Camera.main.transform.right) < Vector3.Dot(ray, Camera.main.transform.right))
				{
					targetPos = new Vector3(Screen.width / 2 - 100, 0, 0);
					image.sprite = right;
				}
				else
				{
					targetPos = new Vector3(100 - Screen.width / 2, 0, 0);
					image.sprite = left	;
				}
			}
			else
			{
				targetPos = canvasPos;
			}

			rect.localPosition = Vector3.Lerp(rect.localPosition, targetPos, Time.deltaTime * 20.0f);

			//rect.position = new Vector3(screenPos.x, screenPos.y); // - Camera.main.pixelWidth / 2, screenPos.y - Camera.main.pixelHeight / 2);
		}
		else
		{
			image.enabled = false;
		}
    }
}
