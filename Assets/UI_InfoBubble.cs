using UnityEngine;
using System.Collections;

public class UI_InfoBubble : MonoBehaviour {

	public Transform bubbleObj;
	public TextMesh text;

	Target lastTarget;

	// Use this for initialization
	void Start () {
		UpdateBubble(null);
	}
	
	// Update is called once per frame
	void Update () 
	{
		AvatarControl currentAvatar = GameInstanceManager.Instance().GetCurrentAvatar();
		if(currentAvatar)
		{
			//if(currentAvatar.underMouseTarget != lastTarget)
			{
				lastTarget = currentAvatar.underMouseTarget;
				UpdateBubble(currentAvatar.underMouseTarget);
			}
		}
	}

	void UpdateBubble(Target t)
	{
		if(!t)
		{
			bubbleObj.gameObject.SetActive(false);
		}
		else
		{
			bubbleObj.gameObject.SetActive(true);
			transform.position = t.transform.position;

			text.text = t.readableName + "\nLevel "+t.currentLevel + "\nHealth: "+t.currentHealth;
		}
	}
}
