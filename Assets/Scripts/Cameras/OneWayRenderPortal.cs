using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This represents a one-way portal from outside the ship to inside
// Inside to out is done just by rendering two cameras.
public class OneWayRenderPortal : MonoBehaviour
{
	[Header("Main Settings")]
	public MeshRenderer renderSurface;
	MeshFilter renderSurfaceMeshFilter;

	public Transform cameraSideTransform;
	public Transform renderSideTransform;
	private Camera renderCamera;

	[Header("Advanced Settings")]
	public float nearClipOffset = 0.05f;
	public float nearClipLimit = 0.2f;
	public float maxRenderDistance = 100.0f;
	public TriggerVolume playerCheckVolume;

	RenderTexture viewTexture;

	void Awake()
	{
		if (cameraSideTransform == null)
			cameraSideTransform = transform;
		renderCamera = renderSideTransform.GetComponentInChildren<Camera>();
		renderCamera.enabled = false;
		renderSurfaceMeshFilter = renderSurface.GetComponent<MeshFilter>();
		renderSurface.material.SetInt("displayMask", 1);
	}

	public bool Visible(Camera fromCamera)
	{
		// Skip render planes that are too far away
		if (Vector3.Distance(cameraSideTransform.position, fromCamera.transform.position) > maxRenderDistance)
			return false;

		// If we have a player check volume, then we can't show anything unless the player is in that volume
		if (playerCheckVolume != null)
		{
			Player player = playerCheckVolume.Get<Player>();
			if (player == null) // TODO: == MyPlayer
				return false;
		}


		// Skip rendering the view from this portal if player is not looking at the linked portal
		return CameraUtility.VisibleFromCamera(renderSurface, fromCamera);
	}

	public void PrePortalRender(Camera fromCamera)
	{

	}

	// Manually render the camera attached to this portal
	public void Render(Camera fromCamera)
	{
		CreateViewTexture();

		renderCamera.projectionMatrix = fromCamera.projectionMatrix;

		Matrix4x4 localToWorldMatrix =
			  renderSideTransform.localToWorldMatrix 
			* cameraSideTransform.worldToLocalMatrix 
			* fromCamera.transform.localToWorldMatrix;
		Vector3 renderPosition = localToWorldMatrix.GetColumn(3);
		Quaternion renderRotation = localToWorldMatrix.rotation;

		// Hide screen so that camera can see through portal
		renderCamera.transform.SetPositionAndRotation(renderPosition, renderRotation);
		SetNearClipPlane(fromCamera);
		renderCamera.Render();
	}

	public void PostPortalRender(Camera fromCamera)
	{
		ProtectScreenFromClipping(fromCamera);
	}

	void CreateViewTexture()
	{
		if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
		{
			if (viewTexture != null)
			{
				viewTexture.Release();
			}
			viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
			// Render the view from the portal camera to the view texture
			renderCamera.targetTexture = viewTexture;
			// Display the view texture on the screen of the linked portal
			renderSurface.material.SetTexture("_MainTex", viewTexture);
		}
	}

	// Sets the thickness of the portal screen so as not to clip with camera near plane when player goes through
	float ProtectScreenFromClipping(Camera fromCamera)
	{
		Vector3 viewPoint = fromCamera.transform.position;
		float halfHeight = fromCamera.nearClipPlane * Mathf.Tan(fromCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float halfWidth = halfHeight * fromCamera.aspect;
		float dstToNearClipPlaneCorner = new Vector3(halfWidth, halfHeight, fromCamera.nearClipPlane).magnitude;
		float screenThickness = dstToNearClipPlaneCorner;

		Transform screenT = renderSurface.transform;
		bool camFacingSameDirAsPortal = Vector3.Dot(transform.forward, transform.position - viewPoint) > 0;
		screenT.localScale = new Vector3(screenT.localScale.x, screenT.localScale.y, screenThickness);
		screenT.localPosition = Vector3.forward * screenThickness * ((camFacingSameDirAsPortal) ? 0.5f : -0.5f);
		return screenThickness;
	}

	// Use custom projection matrix to align portal camera's near clip plane with the surface of the portal
	// Note that this affects precision of the depth buffer, which can cause issues with effects like screenspace AO
	void SetNearClipPlane(Camera fromCamera)
	{
		
		// Learning resource:
		// http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
		Transform clipPlane = renderSideTransform;
		int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, clipPlane.position - renderCamera.transform.position));

		Vector3 camSpacePos = renderCamera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
		Vector3 camSpaceNormal = renderCamera.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
		float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

		// Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
		if (Mathf.Abs(camSpaceDst) > nearClipLimit)
		{
			Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

			// Update projection based on new clip plane
			// Calculate matrix with player cam so that player camera settings (fov, etc) are used
			renderCamera.projectionMatrix = fromCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
		}
		else
		{
			renderCamera.projectionMatrix = fromCamera.projectionMatrix;
		}
	}
}
