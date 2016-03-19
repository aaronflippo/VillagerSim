using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_GoldRateDisplay : MonoBehaviour {

	Text label;
	long lastGold = -1;

	// Use this for initialization
	void Start () {
		label = GetComponent<Text>();
		Debug.Assert( label != null, "Couldn't find label in UI_GoldRateDisplay");
	}

	// Update is called once per frame
	void Update () 
	{
		long gold = GameInstanceManager.Instance().GetGoldRatePerDay();
		if(gold == 0)
		{
			label.text = "";
		}
		else if(gold != lastGold)
		{
			label.text = "+ "+gold.ToString("N0") + " /day";
		}

	}

}
