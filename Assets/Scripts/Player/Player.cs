using System.Collections.Generic;
using UnityEngine;

public class Player : PortalTraveller
{
	public static List<Player> players = new List<Player>(4);

	public static Player LocalPlayer { get { return players[0]; } }

	public bool IsInsideShip
		{ get { return ShipManager.inst.wholeShipVolume.Contains(this); } }

	// --------------------------------------------------------------------------------
	// Seats & LargeItems
	// --------------------------------------------------------------------------------
	public Seat currentSeat { get; private set; }
	public LargeItem currentLargeItem { get; private set; }
	public Transform holdPos;

	public void ClientRequestSit(Seat seat)
	{
		// todo: MP
		ServerSit(seat);
	}

	public void ClientRequestPickup(LargeItem item)
	{
		// todo: MP
		ServerPickupLarge(item);
	}

	public void ServerSit(Seat seat)
	{
		ServerDropCurrent();

		// Enter new seat
		currentSeat = seat;
		if(currentSeat != null)
			currentSeat.ServerPlayerEnter(this);

	}

	public void ServerPickupLarge(LargeItem item)
	{
		ServerDropCurrent();

		// Pickup new item
		currentLargeItem = item;
		if (currentLargeItem != null)
			currentLargeItem.ServerPlayerPickup(this);
	}

	private void ServerDropCurrent()
	{
		if (currentSeat != null)
		{
			currentSeat.ServerPlayerLeave(this);
		}
		if (currentLargeItem != null)
		{
			currentLargeItem.ServerPlayerDrop(this);
		}

		currentSeat = null;
		currentLargeItem = null;
	}

	private void UpdateExitSeat()
	{
		if (inputs.interact)
		{
			ClientRequestSit(null);
		}
	}

	private void UpdateDropItem()
	{
		if (inputs.interact)
		{
			ClientRequestPickup(null);
		}
	}

	private void UpdateHeldItemPos()
	{
		currentLargeItem.transform.position = holdPos.position;
		currentLargeItem.transform.rotation = holdPos.rotation;
	}

	// --------------------------------------------------------------------------------
	// Player Inputs
	// --------------------------------------------------------------------------------
	private PlayerInputs inputs;

	private void GatherInputs()
	{
		// Mouse movement should be calculated cumulatively, so as to not miss any motion
		inputs.mouseDelta.x += Input.GetAxis("Mouse X") * mouseSensitivity;
		inputs.mouseDelta.y += Input.GetAxis("Mouse Y") * mouseSensitivity;

		// Directional input is constant and should not be summed
		inputs.move.x = Input.GetAxis("Horizontal");
		inputs.move.y = 0.0f;
		inputs.move.z = Input.GetAxis("Vertical");
		if (inputs.move.sqrMagnitude > 1.0f)
		{
			inputs.move.Normalize();
		}

		if (Input.GetAxis("Interact") > 0)
			inputs.interact.Press();
	}

	private void ClearInputsForNextFrame()
	{
		inputs.NextFrame();
	}

	private void UpdateUniversalInputs()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	// --------------------------------------------------------------------------------
	// Player motion
	// --------------------------------------------------------------------------------
	private CharacterController cc;
	private Vector3 motion;
	private float yaw;
	private float pitch;

	public float mouseSensitivity = 1.0f;
	public float interactDistance = 3.0f;
	public float moveSpeed = 1.0f;
	public Transform head;

	private Interactable hoveringOver = null;

	private void UpdateMotion()
	{
		if (cc.isGrounded)
			motion.y = 0.0f;
		motion *= 0.5f;

		// Apply mouse input to look angles
		yaw += inputs.mouseDelta.x;
		pitch += inputs.mouseDelta.y;
		// Cap rotations
		pitch = Mathf.Clamp(pitch, -90.0f, 90.0f);
		// Apply rotations
		transform.localEulerAngles = new Vector3(0.0f, yaw, 0.0f);
		head.localEulerAngles = new Vector3(-pitch, 0.0f, 0.0f);

		// Apply move input to motion
		motion += transform.rotation * inputs.move * moveSpeed;
		motion.y -= 3.0f;
		cc.Move(motion * Time.deltaTime);

		if (transform.position.y <= -50.0f)
		{
			Story.inst.TriggerStoryPhase(Story.StoryPhase.PLAYER_WAS_AN_ID);
		}
	}

	private void UpdateCursor()
	{
		string hoverText = "";

		// TODO: Layermask
		Interactable interactable = null;
		RaycastHit hit;
		if (Physics.Raycast(new Ray(head.position, head.forward), out hit, interactDistance))
		{
			interactable = hit.collider.GetComponent<Interactable>();
		}

		// If hover target has changed, update it
		if (interactable != hoveringOver)
		{
			if (hoveringOver != null)
				hoveringOver.ClientUnhover(this);

			hoveringOver = interactable;

			if (hoveringOver != null)
				hoveringOver.ClientHover(this);
		}

		// Update the current hover target
		if (hoveringOver != null)
		{
			hoverText = hoveringOver.GetHoverText(this);

			if(inputs.interact && hoveringOver.CanInteract(this))
			{
				hoveringOver.ClientRequestInteract(this);
			}
		}

		TheText.the.text.text = hoverText;
	}

	private void UpdateSeatedPosition()
	{
		transform.position = currentSeat.standingPos.position;
		transform.rotation = currentSeat.standingPos.rotation;
	}

	// --------------------------------------------------------------------------------
	// Portal logic - PortalTraveller
	// --------------------------------------------------------------------------------

	public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
	{
		base.Teleport(fromPortal, toPortal, pos, rot);

		yaw = rot.eulerAngles.y;
	}

	// ------------------------------------------------------------------------------------------
	// Unity update logic
	// ------------------------------------------------------------------------------------------
	protected void Awake()
	{
		players.Add(this);
		cc = GetComponent<CharacterController>();
		inputs = new PlayerInputs();

		// TODO: Move to Game.cs
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
	}

	protected void Update()
	{
		// Gather input every frame
		GatherInputs();
		UpdateUniversalInputs(); // ESC to quit etc.
	}

	protected void FixedUpdate()
	{
		if (currentSeat == null)
		{
			UpdateMotion();
			
			if (currentLargeItem != null)
			{
				UpdateCursor();
				UpdateHeldItemPos();
				currentLargeItem.UpdateHeldByLocalPlayer(inputs);
				UpdateDropItem();
			}
			else
			{
				UpdateCursor();
			}	
		}
		else
		{
			UpdateSeatedPosition();
			currentSeat.UpdateControlledByLocalPlayer(inputs);
			UpdateExitSeat();
		}

		// Clear input after it has been consumed
		ClearInputsForNextFrame();
	}
}
