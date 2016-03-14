using UnityEngine;
using System.Collections;

[System.Serializable]
public class UpgradeItemDefinition
{
	public UpgradeType upgradeType;
	public  bool 	enabled = true;	//if this is false, then it's an item upgrade specification.
	public string 	name;
	public Sprite 	sprite;
	public float 	baseUpgradeCost_Add 	= 1.0f;
	public float 	baseUpgradeCostMultiply = 1.0f;
	public float 	baseUpgradeCostExponent = 1.6f;
	public string   upgradeDesc 			= "{0} cuts/s\n{1} cuts/s at next level";
	public float 	baseDPS_Add 			= 0;
	public float	baseDPS_Multiply 		= 1;	//Probably means something different for each upgrade type
	public float    upgradeExponent 		= 1.6f;


}

//prefab only data class describing upgrade items etc.
public class GameData : MonoBehaviour {


	public float 					timePerRound = 20.0f;
	public float					betweenRoundCooldown = 5.0f;
	public int 						initialGold = 100;
	public float 					avatarSleepTime = 10.0f;

	public UpgradeItemDefinition[] 	upgradeDefs;


	private static GameData instance;
	public static GameData Instance()
	{
		if(instance == null)
		{
			instance = Resources.Load<GameData>("GameData");
		}

		return instance;

	}

	public UpgradeItemDefinition FindItemDef(UpgradeType type)
	{
		for(int i=0; i< upgradeDefs.Length; i++)
		{
			if(upgradeDefs[i].upgradeType == type)
				return upgradeDefs[i];
		}

		Debug.Log("Couldn't find item with upgrade type: "+type);
		return null;
	}


}
