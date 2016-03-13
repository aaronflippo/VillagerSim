using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_GoldDisplay : MonoBehaviour {

	Text label;
	long lastGold = -1;

	// Use this for initialization
	void Start () {
		label = GetComponent<Text>();
		Debug.Assert( label != null, "Couldn't find label in UI_GoldDisplay");
	}
	
	// Update is called once per frame
	void Update () {
		long gold = GameInstanceManager.Instance().GetGold();
		if(gold != lastGold)
		{
			label.text = gold.ToString("N0");
		}

	}
}
