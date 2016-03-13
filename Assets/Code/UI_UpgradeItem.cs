using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_UpgradeItem : MonoBehaviour {

	public Text itemLabel;
	public Image sprite;
	public Text levelText;
	public Text skillText;
	public Text upgradeCost;

	public Button upgradeButton;

	[System.NonSerialized]
	public UpgradeItemDefinition itemDef;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void UpdateLabelsFromData( UpgradeItemDefinition itemDef, int itemLevel)
	{
		
		itemLabel.text = itemDef.name;
		sprite.sprite = itemDef.sprite;
		int itemCost = UpgradeCalulator.CalculateItemUpgradeDPS( itemDef, itemLevel);
		string skillDesc = string.Format( itemDef.upgradeDesc,itemCost , UpgradeCalulator.CalculateItemUpgradeDPS(itemDef, itemLevel+1));
		skillDesc = skillDesc.Replace("\\n", "\n");
		skillText.text = skillDesc;

		levelText.text = "Level " + itemLevel;
		upgradeCost.text = UpgradeCalulator.CalculateItemUpgradeCost(itemDef, itemLevel).ToString("#,#") +" gold";

		this.itemDef = itemDef;

	}

	public void UpdateButtonState(UpgradeItemDefinition itemDef)
	{
		int upgradeCost = GameInstanceManager.Instance().GetUpgradeCost(itemDef.upgradeType);
		bool enabled = (upgradeCost <= GameInstanceManager.Instance().GetGold());
		upgradeButton.interactable = enabled;

			
	}
}
