using UnityEngine;
using System.Collections;

public class UI_PlayerMessage : MonoBehaviour {

	UnityEngine.UI.Text textLabel;

	// Use this for initialization
	void Start () 
	{
		textLabel = GetComponent<UnityEngine.UI.Text>();
		Debug.Assert(textLabel != null, "UI_PlayerMessage couldn't find Text object!");
	}
	
	// Update is called once per frame
	void Update () 
	{
		string msg = GameInstanceManager.Instance().playerMessage;
		if(textLabel.text != msg)
		{
			textLabel.text = msg;
		}

	}
}
