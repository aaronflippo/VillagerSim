using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AvatarAnimationEventListener : MonoBehaviour {

	[System.NonSerialized]
	public AvatarControl myAvatar;



	public void Chop()
	{
		if(myAvatar)
		{
			myAvatar.PlayChopEffect();
		}
	}
}
