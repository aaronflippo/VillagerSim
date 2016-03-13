using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_HealthDisplay : MonoBehaviour {

	Text myLabel;

	private int lastHealth = -1;

	// Use this for initialization
	void Start () {
		myLabel = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		AvatarControl av = GameInstanceManager.Instance().GetCurrentAvatar();
		int health = -2;
		if(av)
		{
			health = av.currentHealth;

		}

		if(health != lastHealth)
		{
			myLabel.text = (health >= 0) ? health.ToString() : "";
		}

	}
}
