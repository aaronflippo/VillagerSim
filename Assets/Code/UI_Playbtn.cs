using UnityEngine;
using System.Collections;

public class UI_Playbtn : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartGame()
	{
		Debug.Log("Start Game");
		GameInstanceManager.Instance().StartGame();
	}
}
