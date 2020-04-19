using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private CharacterController cc;
	private Vector3 motion;
	private float yaw;
	private float pitch;

	public bool isAtHelm = false;

	public float mouseSensitivity = 1.0f;
	public float moveSpeed = 1.0f;
	public Transform head, holdPos;

	public GameObject holding;

    // Start is called before the first frame update
    void Start()
    {
		cc = GetComponent<CharacterController>();
		Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
		string overlayText = "";

		if (isAtHelm)
		{
			Ship.theShip.UpdateMovementControls();
			
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			head.localRotation = Quaternion.identity;

			overlayText = Ship.theShip.GetOverlayText();// Press W to accelerate, A/D to turn and Space/Shift to ascend/descend. Press F to leave.";
		}
		else
		{
			Ship.theShip.NotControlling();
			// Drag old motion
			if (cc.isGrounded)
				motion.y = 0.0f;
			motion *= 0.5f;

			// Get inputs
			yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
			pitch += Input.GetAxis("Mouse Y") * mouseSensitivity;

			Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
			if (moveInput.sqrMagnitude > 1.0f)
			{
				moveInput.Normalize();
			}
			moveInput *= moveSpeed;

			// Cap rotations
			pitch = Mathf.Clamp(pitch, -90.0f, 90.0f);
			// Apply rotations
			transform.localEulerAngles = new Vector3(0.0f, yaw, 0.0f);
			head.localEulerAngles = new Vector3(-pitch, 0.0f, 0.0f);


			// Apply motion
			motion += transform.rotation * moveInput;
			motion.y -= 3.0f;
			cc.Move(motion * Time.deltaTime);
		}

		// Click to interact
		bool drop = Input.GetMouseButtonDown(1);
		if (isAtHelm) // Special case context senstive drop with F/E
		{
			drop = drop || Input.GetKeyDown(KeyCode.F) || Input.GetKeyUp(KeyCode.E);
		}

		if (drop)
		{
			if (isAtHelm)
			{
				isAtHelm = false;
				transform.SetParent(Ship.theShip.transform);
			}
		}
		else if(!isAtHelm)
		{
			bool clicked = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E);

			RaycastHit hit;
			if (Physics.Raycast(new Ray(head.position, head.forward), out hit, 3.0f)
				&& hit.collider.gameObject != holding)
			{
				if (hit.collider.GetComponent<Helm>() != null)
				{
					if (Ship.theShip.CanFly())
					{
						overlayText = "Press F to pilot the ship";
						if (clicked)
						{
							isAtHelm = true;
							transform.SetParent(Ship.theShip.standingPos);
						}
					}
					else
					{
						overlayText = "Ship controls are locked";
					}
				}
				else if (hit.collider.GetComponent<Button>() != null)
				{
					switch (hit.collider.GetComponent<Button>().action)
					{
						case Button.Action.OPEN_DOOR:

							overlayText = "Press F to open the door";
							if (clicked)
								RearDoor.theDoor.Open();
							break;
						case Button.Action.CLOSE_DOOR:
							overlayText = "Press F to close the door";
							if (clicked)
								RearDoor.theDoor.Close();
							break;
					}
				}
				else if(hit.collider.GetComponentInParent<RearDoor>() != null)
				{
					overlayText = "Locked. Please use door control panel to the right";
				}
				else if(hit.collider.GetComponent<EnginePanel>() != null)
				{
					EnginePanel panel = hit.collider.GetComponent<EnginePanel>();
					if(panel.hasComponent)
					{
						overlayText = "All patched up";
					}
					else
					{
						if (holding != null)
						{
							if(holding.GetComponent<EnginePart>() != null)
							{
								overlayText = $"Press F to place {holding.name}";
								Ship.theShip.TriggerStoryPhase(Ship.StoryPhase.READY_TO_PLACE);
								if (clicked)
								{
									holding.transform.SetParent(panel.socket);
									holding.transform.localPosition = Vector3.zero;
									holding.transform.localRotation = Quaternion.identity;
									holding.GetComponent<EnginePart>().slideTimer = 1.0f;
									panel.hasComponent = true;
									Ship.theShip.TriggerStoryPhase(Ship.StoryPhase.PLACED_COMPONENT);
									holding = null;
								}
							}
							else
							{
								overlayText = "That isn't the right part";
							}
						}
						else
						{
							overlayText = "Something's missing from this engine";
							Ship.theShip.TriggerStoryPhase(Ship.StoryPhase.FETCH_COMPONENT);
						}
					}
				}
				else if(hit.collider.GetComponent<EnginePart>() != null)
				{
					if(holding == null)
					{
						overlayText = $"Press F to pickup {hit.collider.name}";
						if(clicked)
						{
							holding = hit.collider.gameObject;
							holding.transform.SetParent(holdPos);
							holding.transform.localPosition = Vector3.zero;
							holding.transform.localRotation = Quaternion.identity;
							Ship.theShip.TriggerStoryPhase(Ship.StoryPhase.HAVE_COMPONENT);
						}
					}
				}
					
			}

			
		}

		TheText.the.text.text = overlayText;
	}
}
