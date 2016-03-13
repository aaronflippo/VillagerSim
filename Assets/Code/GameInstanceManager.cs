using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UpgradeType
{
	AvatarCount,
	Forestry,	//damage vs. trees.
	AvatarMoveSpeed,
	MushroomFarming,
	RestHouse,
	Farming,
	AvatarStamina,
	StoneMining,
	AvatarHealth,
	Blacksmithing,
	MonsterFighting,
	Hospitals,
	Upgrade_MAX
}


public class GameInstanceManager : MonoBehaviour {


	public List<AvatarCommandHistory> ghostRecords = new List<AvatarCommandHistory>(100);
	public AvatarControl avatarPrefab;
	public Transform spawnPoint;

	public GameObject goldRewardTextPrefab;


	[System.NonSerialized]
	public string playerMessage;
	private int playerMessageID;

	private GameObject[] avatarHouses;
	private int lastHouseSpawnIdx;

	private AvatarControl currentAvatar;

	private bool pendingAvatarSpawn;

	public class GameInstanceData
	{
		
		public float roundTimePassed;
		public float roundCountDown;


		//corresponds to  UpgradeType enum
		public int[] upgradeLevels;

		public int currentRound = 1;

		public long gold;
			
	}

	private GameInstanceData gameInstanceData;

	private static int ROUND_MSG_ID = 1;

	private static GameInstanceManager instance;
	public static GameInstanceManager Instance()
	{
		if(!instance)
			instance = GameObject.FindObjectOfType<GameInstanceManager>();

		return instance;
	}

	// Use this for initialization
	void Start () 
	{
		avatarHouses = GameObject.FindGameObjectsWithTag("house");

		DontDestroyOnLoad(gameObject);

		StartGame();



	}

	void StartGame()
	{
		int initialLevel = 0;

		gameInstanceData = new GameInstanceData();
		gameInstanceData.upgradeLevels = new int[ (int)UpgradeType.Upgrade_MAX ];
		for(int i=0; i< gameInstanceData.upgradeLevels.Length; i++)
			gameInstanceData.upgradeLevels[i] = initialLevel;

		//give me a level of forestry!
		gameInstanceData.upgradeLevels[(int)(UpgradeType.Forestry)] = 1;


		gameInstanceData.gold = GameData.Instance().initialGold;
		StartRound();

			
		//one freebie villager
		//SpawnAnAvatar();
	}

	// Update is called once per frame
	void Update () {

		/*if( Input.GetKeyDown( KeyCode.R ) )
		{
			EndRound();
		}*/

		if( gameInstanceData.roundCountDown <= 0 )
		{
			//in a round!
			gameInstanceData.roundTimePassed += Time.deltaTime;

			if(gameInstanceData.roundTimePassed >= GameData.Instance().timePerRound)
			{
				EndRound();
			}
		}
		else
		{
			//counting down to next round
			gameInstanceData.roundCountDown-= Time.deltaTime;
			if(gameInstanceData.roundCountDown <= 0)
			{
				//destroy avatars, reload scene.
				EndRound_FinalCleanup();

				gameInstanceData.currentRound++;
				StartRound();
			}
		}
		
	}


	private void EndRound_FinalCleanup()
	{
		

		//delete all existing avatars.
		AvatarControl[] allAvatars = GameObject.FindObjectsOfType<AvatarControl>();
		for( int i=0; i< allAvatars.Length; i++ )
		{
			//allAvatars[i].DoDestroy();
			GameObject.Destroy(allAvatars[i].gameObject);
		}

		Debug.Log("Round cleanup. Cleaned up "+allAvatars.Length + "avatars");

		//easiest way to reset world objects: reload the scene they belong to!
		UnityEngine.SceneManagement.SceneManager.UnloadScene(0);
		UnityEngine.SceneManagement.SceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Additive);
	}

	private AvatarCommandHistory pendingHistory = null;

	public void EndRound()
	{

		//Debug.Log("Turn ended!");
		SetPlayerMessageForTime("DAY FINISHED", 2.0f, ROUND_MSG_ID);

		//delete all existing avatars.
		AvatarControl[] allAvatars = GameObject.FindObjectsOfType<AvatarControl>();
		for( int i=0; i< allAvatars.Length; i++ )
		{
			allAvatars[i].RoundEnded();

			//allAvatars[i].DoDestroy(false);

		}



		//start countdown for next turn
		gameInstanceData.roundCountDown = GameData.Instance().betweenRoundCooldown;

	}

	public float RoundTimeElapsed()
	{
		return (gameInstanceData != null) ? gameInstanceData.roundTimePassed : 0.0f;
	}

	public bool PlayingRound()
	{	
		return ( gameInstanceData.roundCountDown <= 0 );
	}

	public void StartRound()
	{
		Debug.Log("StartRound");
		SetPlayerMessageForTime( "DAY: " + gameInstanceData.currentRound, 3.0f, ROUND_MSG_ID );

		gameInstanceData.roundTimePassed = 0.0f;

		SpawnGhosts();


		//if(pendingAvatarSpawn)
		{
			SpawnAnAvatar();

		}




	}

	void SpawnAnAvatar()
	{
		
		Debug.Log("SpawnAnAvatar");
		//spawn a playable avatar.
		//todo: Make this an upgrade choice.
		AvatarControl player = GameObject.Instantiate<AvatarControl>( avatarPrefab );

		GameObject spawnPoint = avatarHouses[lastHouseSpawnIdx];
		Debug.Assert(avatarHouses != null, "AvatarHouses was null!");

		player.transform.position = spawnPoint.transform.position;
		lastHouseSpawnIdx = (lastHouseSpawnIdx+1)%avatarHouses.Length;

		player.Init( null );

		InitializeAvatar(player);


	}

	void InitializeAvatar(AvatarControl av)
	{
		currentAvatar = av;
	}

	void SpawnGhosts()
	{
		Debug.Log("Starting round. Ghosts: "+ghostRecords.Count);

		for(int i=0; i< ghostRecords.Count; i++)
		{
			AvatarControl av = GameObject.Instantiate<AvatarControl>( avatarPrefab );
			av.transform.position = ghostRecords[i].spawnPoint;

			av.Init( ghostRecords[i] );//send him on his way as a ghost

		}
	}

	public void RecordAvatarGhost( AvatarCommandHistory history )
	{
		ghostRecords.Add(history);
	}

	public void SetPlayerMessageForTime(string message, float time, int id)
	{
		StartCoroutine(SetPlayerMessageForTime_Internal(message, time, id));
	}
	
	private IEnumerator SetPlayerMessageForTime_Internal(string message, float time, int id)
	{
		SetPlayerMessage(message, id);
		yield return new WaitForSeconds(time);
		CancelPlayerMessage(id);
	}

	public void SetPlayerMessage(string message, int id)
	{
		playerMessage = message;
		playerMessageID = id;
	}

	public void CancelPlayerMessage(int id)
	{
		if(playerMessageID == id)
		{
			playerMessage = "";
		}
	}

	public int GetUpgradeLevel(UpgradeType type)
	{
		if(gameInstanceData != null)
			return gameInstanceData.upgradeLevels[(int)type];
		return 0;
				
	}

	private UpgradeItemDefinition FindItemDef(UpgradeType type)
	{
		foreach( UpgradeItemDefinition item in GameData.Instance().upgradeDefs)
		{
			if(item.upgradeType == type)
				return item;
		}
		return null;
	}

	public int GetDPS(UpgradeType type)
	{
		UpgradeItemDefinition def = FindItemDef(type);
		if(def != null)
		{
			return UpgradeCalulator.CalculateItemUpgradeDPS( def, gameInstanceData.upgradeLevels[(int)type] );
		}
		else
		{
			Debug.Log("Couldn't find item def for type: "+type);
			return 0;
		}
	}

	public int GetUpgradeCost(UpgradeType type)
	{
		return (int)UpgradeCalulator.CalculateItemUpgradeCost( FindItemDef(type), gameInstanceData.upgradeLevels[(int)type] );
	}


	public void DoUpgrade(UpgradeType type)
	{
		
		AddGold( -GetUpgradeCost(type), Vector3.zero);

		gameInstanceData.upgradeLevels[(int)type]++;

		if(type == UpgradeType.AvatarCount)
		{
			//pendingAvatarSpawn = true;
			if(currentAvatar)
			{
				currentAvatar.CommitRecordOnRoundEnd();
			}
			SetPlayerMessageForTime("New villager will arrive tomorrow!", 5.0f, -1);
		}

		//update visibiilty of all targets in the world.
		Target[] allTargets = Resources.FindObjectsOfTypeAll<Target>();

		for(int i=0; i<allTargets.Length; i++)
		{
			allTargets[i].UpdateVisibility();
		}

	}

	public void AddGold(long amount, Vector3 worldPos)
	{
		gameInstanceData.gold += amount;
		if(worldPos != Vector3.zero)
		{
			AddGoldReward(amount, worldPos);
		}
	}

	public long GetGold()
	{
		return (gameInstanceData != null ) ? gameInstanceData.gold : 0;
	}

	public void AddGoldReward(long amount, Vector3 worldPosition)
	{
		if( goldRewardTextPrefab )
		{
			GameObject inst = (GameObject)GameObject.Instantiate(goldRewardTextPrefab, worldPosition, goldRewardTextPrefab.transform.rotation);
			TextMesh text = inst.GetComponentInChildren< TextMesh>();
			if(text)
			{
				text.text = amount.ToString("#,#");
			}

			GameObject.Destroy(inst, 3.0f);
		}
	}

	public AvatarControl GetCurrentAvatar()
	{
		return currentAvatar;
	}

}
