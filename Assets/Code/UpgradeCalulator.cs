using UnityEngine;
using System.Collections;

public class UpgradeCalulator : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static int CalculateItemUpgradeCost(UpgradeItemDefinition itemDef, int level)
	{
		return  (int)(itemDef.baseUpgradeCost_Add + itemDef.baseUpgradeCostMultiply * Mathf.Pow(level, itemDef.upgradeExponent ));
	}

	public static int CalculateItemUpgradeDPS(UpgradeItemDefinition itemDef, int level)
	{
		Debug.Assert(itemDef != null, "ItemDef was null in CalculateItemUpgradeDPS");
		return  Mathf.CeilToInt(itemDef.baseDPS_Add + itemDef.baseDPS_Multiply * Mathf.Pow(level, itemDef.upgradeExponent ));
	}

}
