using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct AvatarCommand
{
	public string targetName;
	public float  targetTime;
}

public class AvatarCommandHistory
{
	public List<AvatarCommand> commandList = new List<AvatarCommand>(10);
	public bool endInDeath = false;
	public float spawnTime;
	public Vector3 spawnPoint;
}

public class AvatarControl : MonoBehaviour {
	
	public float attackDistance = 1.0f;
	public float attackDelay = 1.0f;


	private bool recordingMoves = false;

	private Target currentTarget = null;
	private Vector3 currentTargetPos;
	public  int currentHealth;
	private Vector3 targetOffset;

	private bool attacking = false;
	private bool sleeping;

	private float sleepTimeLeft = 0.0f;

	private float attackTimer = 0.0f;
	private int commandHistoryIndex = 0;

	public static Vector3 PLAYER_ATTACK_OFFSET = new Vector3(0, 0, -1f);

	[System.NonSerialized]
	public AvatarCommandHistory myCommandHistory;
	private Animator animator;

	private bool returningHome;
	private bool commitRecordOnRoundEnd = false;
	private SpriteRenderer mySprite;


	[System.NonSerialized]
	Target underMouseTarget;

	// Use this for initialization
	void Start () 
	{
		
		animator = GetComponentInChildren<Animator>();
		mySprite = GetComponentInChildren<SpriteRenderer>();
	}

	public void Init(AvatarCommandHistory commandHistory)
	{
		Debug.Log("Villager Init");
		myCommandHistory = commandHistory;
		if(myCommandHistory == null)
		{
			//I'm the player, create a new history
			recordingMoves = true;
			myCommandHistory = new AvatarCommandHistory();
			myCommandHistory.spawnPoint = transform.position;
			myCommandHistory.spawnTime = GameInstanceManager.Instance().RoundTimeElapsed();
		}
		else
		{
			SetIsGhost(true);
			transform.position = myCommandHistory.spawnPoint;
		}



		currentHealth = GetStartingHealth();
	}


	int GetStartingHealth()
	{
		return Mathf.CeilToInt(GameInstanceManager.Instance().GetDPS(UpgradeType.AvatarHealth));
	}

	// Update is called once per frame
	void Update () 
	{
		underMouseTarget = null;

		attacking = false;
		bool moving = false;


		if(sleepTimeLeft > 0 && !returningHome)
		{
			sleepTimeLeft -= Time.deltaTime;
			if(sleepTimeLeft <=0)
			{
				sleeping = false;
				sleepTimeLeft = 0.0f;
				currentHealth = GetStartingHealth();
			}

		}

		if(returningHome)
		{
			moving = true;
			MoveTowards(myCommandHistory.spawnPoint, 8.0f);

			if( (myCommandHistory.spawnPoint - transform.position).magnitude < 0.5f)
			{
				GameObject.Destroy(gameObject);
				return;
			}
		}

		if(!sleeping && !returningHome)
		{
			Vector3 targetPos = Vector3.zero;
			if(currentTarget)
			{
				targetPos = currentTarget.transform.position + PLAYER_ATTACK_OFFSET + targetOffset;
			}	
			else if (currentTargetPos != Vector3.zero)
			{
				targetPos = currentTargetPos;
			}


			if(targetPos != Vector3.zero)
			{
				Vector3 toTarget = targetPos - transform.position;

				bool targetIsSleeping = currentTarget && currentTarget.IsRecharging();
				if( currentTarget && (toTarget.magnitude < attackDistance) && !targetIsSleeping)
				{
					
					attacking = true;
					moving = false;

				}
				else
				{
					moving = true;
					MoveTowards(targetPos);
				}
			}
		}


		if( recordingMoves )
		{
			if( Camera.main.pixelRect.Contains(Input.mousePosition) )
			{
				RaycastHit hitInfo = GetClickInfo( Input.mousePosition );
				if(hitInfo.collider)
				{
					Target hitTarget = hitInfo.collider.gameObject.GetComponentInParent<Target>();

					underMouseTarget = hitTarget;

					if( Input.GetMouseButtonDown(0) )
					{
						
						if(hitTarget)
						{
							//records any previous commands.
							SetTarget(hitTarget);
						}
						else
						{
							SetTargetPosition(hitInfo.point);	
						}

					}

				}
			}

				
		}
		else
		{
			//playback mode
			UpdateCommandHistoryReplay();
		}

		if(attacking)
		{
			attackTimer -= Time.deltaTime;
			if(attackTimer<=0)
			{
				attackTimer = attackDelay;
				TryAttack();
			}
		}

		if(animator)
		{
			animator.SetBool("sleeping", sleeping);
			animator.SetBool("moving", moving);
			animator.SetBool("attacking", attacking);
		}

		UI_ObjectTooltips.SetUnderMouseTarget(underMouseTarget);

	}


	void MoveTowards(Vector3 targetPos, float minSpeed = 0.0f)
	{
		
		if(mySprite)
		{
			mySprite.flipX = (targetPos.x > transform.position.x );
		}
		
		float moveSpeed = GameInstanceManager.Instance().GetDPS( UpgradeType.AvatarMoveSpeed );
		moveSpeed = Mathf.Max(minSpeed, moveSpeed);

		transform.position = Vector3.MoveTowards( transform.position, targetPos, moveSpeed * Time.deltaTime);
	}

	RaycastHit GetClickInfo(Vector3 position)
	{

		UnityEngine.RaycastHit hitInfo = new RaycastHit(); 
		if(Camera.main)
		{

			UnityEngine.Ray ray = Camera.main.ScreenPointToRay (position);

			Physics.Raycast (ray, out hitInfo);
		}

		return hitInfo;
	}

	void SetTarget( Target t )
	{
		currentTargetPos = Vector3.zero;

		//RandomizeTargetOffset();

		currentTarget = t;


		if(recordingMoves)
		{
			//if(currentCommandTime > 0)
			{
				RecordMove();
			}
		}


	}

	void SetTargetPosition(Vector3 pos)
	{
		currentTargetPos = pos;
		currentTarget = null;

	}

	void RandomizeTargetOffset()
	{
		targetOffset = Random.onUnitSphere;
		targetOffset.y = 0;
		targetOffset.Normalize();
		targetOffset *= 0.5f;

	}

	//record an a command into my list.
	void RecordMove()
	{
		//Debug.Log("RecordMove: "+currentTarget+ ", time: "+ GameInstanceManager.Instance().RoundTimeElapsed() );
		string name = currentTarget != null? currentTarget.name : string.Empty;
		AvatarCommand cmd = new AvatarCommand(){targetName = name, targetTime = GameInstanceManager.Instance().RoundTimeElapsed()};
		myCommandHistory.commandList.Add(cmd);
	}

	private int CalcDamageVS(Target t)
	{
		if(t.requiredUpdateType != UpgradeType.Upgrade_MAX)
		{
			return GameInstanceManager.Instance().GetDPS( t.requiredUpdateType );
		}
		else
		{
			Debug.LogError("CalculateDamageVS "+t.name +" couldn't properly calculate DPS");
			return 0;
		}
	}

	private void TryAttack()
	{
		if( currentTarget && !currentTarget.IsRecharging() )
		{

			//did it have a cost?
			int healthCost = currentTarget.GetHealthCost();

			int attackDamage = CalcDamageVS(currentTarget);

			if( currentTarget.TakeDamage( attackDamage ) )
			{
				if( currentTarget == null || currentTarget.IsDead() )
				{
					
				}

				if( healthCost > 0)
				{
					currentHealth -= healthCost;
					if( currentHealth <= 0 )
					{
						GoToSleep();
					}
				}
			}
		}
	}

	void UpdateCommandHistoryReplay()
	{
		if( (myCommandHistory != null) && (commandHistoryIndex < myCommandHistory.commandList.Count) )
		{
			float currentTime = GameInstanceManager.Instance().RoundTimeElapsed();
			
			AvatarCommand currentCommand = myCommandHistory.commandList[commandHistoryIndex];
			bool targetDead = false;
			if( currentTarget == null && !string.IsNullOrEmpty( currentCommand.targetName ))
			{
				GameObject target = GameObject.Find( currentCommand.targetName );
				if(target)
				{
					SetTarget( target.GetComponent<Target>() );
				}
				else
				{
					targetDead = true;
					//Debug.Log("Couldn't find target: "+currentCommand.targetName);
				}
					
			}

			bool hasMoreCommands = (commandHistoryIndex < myCommandHistory.commandList.Count-1);
			bool skipToNextCMD = (targetDead || (hasMoreCommands && (currentTime > myCommandHistory.commandList[commandHistoryIndex+1].targetTime)));

			if(skipToNextCMD)
			{
				commandHistoryIndex++;
				//will reset to new target in next loop.

				currentTarget = null;

				if( commandHistoryIndex == myCommandHistory.commandList.Count)
				{
					if(myCommandHistory.endInDeath)
					{
						DoDestroy ( true );
					}
					else
						RoundEnded();
				}
			}
		}	
	}

	public void DoDestroy(bool died)
	{
		Debug.Log("DoDestroy. Died: "+died);

		if(recordingMoves)
		{
			RecordMove();
			myCommandHistory.endInDeath = died;
			GameInstanceManager.Instance().RecordAvatarGhost( myCommandHistory );
		}

		GameObject.Destroy(gameObject);
	}

	public void GoToSleep()
	{
		sleeping = true;
		sleepTimeLeft = GameData.Instance().avatarSleepTime;
		animator.SetTrigger("died");
	}

	private void SetIsGhost( bool isGhost )
	{
		SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
		if(sr)
		{
			sr.color = isGhost? Color.gray : Color.white;
		}
	}


	//records my move history to the ghost list, then returns to home.
	public void RoundEnded()
	{
		if(recordingMoves)
		{
			RecordMove();
			if(commitRecordOnRoundEnd)
			{
				GameInstanceManager.Instance().RecordAvatarGhost( myCommandHistory );
			}
		}

		returningHome = true;
	}

	public void CommitRecordOnRoundEnd()
	{
		commitRecordOnRoundEnd = true;
	}


}
