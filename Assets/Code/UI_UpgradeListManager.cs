using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_UpgradeListManager : MonoBehaviour {

	// Use this for initialization
	GameObject listItemPrefab;
	void Start () 
	{
		listItemPrefab = transform.GetChild(0).gameObject;
		listItemPrefab.SetActive(false);
	
		InitUpgradeButtons();

	}

	List<UI_UpgradeItem> upgradeItems = new List<UI_UpgradeItem>(20);

	
	// Update is called once per frame
	void Update () {
		for(int i=0; i< upgradeItems.Count; i++)
		{
			upgradeItems[i].UpdateButtonState( GameData.Instance().upgradeDefs[i]);
		}
	}

	void InitUpgradeButtons()
	{
		upgradeItems.Clear();

		GameData data = GameData.Instance();

		for( int i=0; i< data.upgradeDefs.Length; i++ )
		{
			GameObject inst = GameObject.Instantiate( listItemPrefab );
			inst.transform.SetParent(transform, false);
			inst.SetActive(true);
			inst.name = i.ToString();

			UI_UpgradeItem upgradeItem = inst.GetComponent<UI_UpgradeItem>();
			upgradeItems.Add(upgradeItem);

			Debug.Assert(upgradeItem, "Couldn't find upgrade item!");
			upgradeItem.UpdateLabelsFromData( data.upgradeDefs[i], GameInstanceManager.Instance().GetUpgradeLevel(data.upgradeDefs[i].upgradeType));

			int currentLevel = GameInstanceManager.Instance().GetUpgradeLevel( data.upgradeDefs[i].upgradeType);
			if(currentLevel == 0)
			{
				//don't show next level until I've upgraded this one.
				break;
			}
		}

	}

	public void DoUpgrade(GameObject obj)
	{
		//Debug.Log("Upgrade index: "+obj);
		int idx = int.Parse(obj.name);
		UpgradeItemDefinition itemDef = GameData.Instance().upgradeDefs[idx];

		GameInstanceManager.Instance().DoUpgrade(itemDef.upgradeType );

		ClearUpgradeButtons();
		InitUpgradeButtons();
	} 

	private void ClearUpgradeButtons()
	{
		int childCount = transform.childCount;

		for(int i=childCount-1; i >= 0; i--)
		{
			GameObject child = transform.GetChild(i).gameObject;
			if(child.activeSelf)
			{
				child.transform.SetParent(null);
				GameObject.Destroy(child);
			}
		}
	}
}
