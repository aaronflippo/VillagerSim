using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {


	public float moveSmoothTime;
	public float followCharSize;

	private Vector3 defaultPosition;
	private float defaultSize;
	private Camera myCamera;
	private Vector3 velocity;
	private float sizeVelocity;
	private Vector3 viewOffset;

	// Use this for initialization
	void Start () {
	
		myCamera = GetComponent<Camera>();
		defaultSize = myCamera.orthographicSize;
		defaultPosition = transform.position;
		viewOffset = -myCamera.transform.forward * 50.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{

		Vector3 targetPosition = defaultPosition;
		float targetSize = defaultSize;
		AvatarControl av = GameInstanceManager.Instance().GetCurrentAvatar();
		if(av)
		{
			targetSize = followCharSize;
			targetPosition = av.transform.position + viewOffset;
		}

		if(myCamera.orthographicSize != targetSize || myCamera.transform.position != targetPosition)
		{
			myCamera.transform.position = Vector3.SmoothDamp( myCamera.transform.position, targetPosition, ref velocity, moveSmoothTime);
			myCamera.orthographicSize = Mathf.SmoothDamp( myCamera.orthographicSize, targetSize, ref sizeVelocity, moveSmoothTime);
		}

	}
}
