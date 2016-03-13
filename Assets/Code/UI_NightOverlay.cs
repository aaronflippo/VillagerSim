using UnityEngine;
using System.Collections;

public class UI_NightOverlay : MonoBehaviour {

	public float nightTransitionTime = 5.0f;
	public UnityEngine.UI.Image myImage;

	// Use this for initialization
	void Start () {
		myImage = GetComponent<UnityEngine.UI.Image>();
		Debug.Assert(myImage, "Couldn't find UIImage in UI_NightOverlay!");
	
	}
	
	// Update is called once per frame
	void Update () {

		GameInstanceManager gameInst = GameInstanceManager.Instance();
		float targetAlpha = 0.0f;
		if(!gameInst.PlayingRound())
		{
			targetAlpha = 1.0f;
		}

		Color targetColor = myImage.color;
		if(myImage.color.a != targetAlpha)
		{
			float a = Mathf.MoveTowards( myImage.color.a, targetAlpha, 1.0f / nightTransitionTime * Time.deltaTime);
			targetColor.a = a;
			myImage.color = targetColor;
		}

		myImage.enabled = myImage.color.a != 0.0f;
	
	}


}
