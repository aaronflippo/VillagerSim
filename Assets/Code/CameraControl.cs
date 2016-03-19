using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {


	public float moveSmoothTime;
	public float followCharSize;

	public float followCharFOVPerspective;

	private Vector3 defaultPosition;
	private float defaultSize;
	private Camera myCamera;
	private Vector3 velocity;
	private float sizeVelocity;

	private float defaultFOV;
	//private float defaultDistance;
	private Vector3 viewOffset;

	// Use this for initialization
	void Start () {
	
		myCamera = GetComponent<Camera>();
		defaultSize = myCamera.orthographicSize;
		defaultPosition = transform.position;
		defaultFOV = myCamera.fieldOfView;

		//figure out default distance
		Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

		float dist;
		groundPlane.Raycast( new Ray( myCamera.transform.position, myCamera.transform.forward), out dist);
		viewOffset = -myCamera.transform.forward * dist;
	}
	
	// Update is called once per frame
	void Update () 
	{


		Vector3 targetPosition = defaultPosition;


		float targetSizeOrtho = defaultSize;
		float targetFOV = defaultFOV;
		AvatarControl av = GameInstanceManager.Instance().GetCurrentAvatar();
		if(av)
		{
			targetSizeOrtho = followCharSize;
			targetFOV = followCharFOVPerspective;
			targetPosition = av.transform.position + viewOffset;
		}

		//lerp ortho size
		if(myCamera.orthographic)
		{
			if(myCamera.orthographicSize != targetSizeOrtho )
			{
				myCamera.orthographicSize = Mathf.SmoothDamp( myCamera.orthographicSize, targetSizeOrtho, ref sizeVelocity, moveSmoothTime);
			}
		}
		else
		{
			
			if(myCamera.fieldOfView != targetFOV )
			{
				myCamera.fieldOfView = Mathf.SmoothDamp( myCamera.fieldOfView, targetFOV, ref sizeVelocity, moveSmoothTime);
			}
		}


		//lerp position
		if( myCamera.transform.position != targetPosition )
		{
			myCamera.transform.position = Vector3.SmoothDamp( myCamera.transform.position, targetPosition, ref velocity, moveSmoothTime);
		}

	}
}
