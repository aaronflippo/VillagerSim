using UnityEngine;
using System.Collections;

public class UI_ObjectTooltips : MonoBehaviour {

	//static Target currentTarget = null;
	static int lastTargetID = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	

	}

	static public void SetUnderMouseTarget(Target t)
	{
		
		if(t)
		{
			lastTargetID = t.GetInstanceID();

			string msg = t.readableName + " (Level "+t.currentLevel+")";	
			GameInstanceManager.Instance().SetPlayerMessage( msg, lastTargetID);
		}
		else
		{
			GameInstanceManager.Instance().CancelPlayerMessage(lastTargetID);

		}

		


	}
}

