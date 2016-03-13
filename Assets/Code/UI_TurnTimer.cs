using UnityEngine;
using System.Collections;

public class UI_TurnTimer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(GameInstanceManager.Instance().PlayingRound())
		{
			float timerPCT = GameInstanceManager.Instance().RoundTimeElapsed() / GameData.Instance().timePerRound;
			transform.localScale = new Vector3(timerPCT, 1,1);
		}
		else
			transform.localScale = Vector3.zero;
	}
}
