using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Target : MonoBehaviour {

	public string readableName;

	public UpgradeType requiredUpdateType = UpgradeType.Upgrade_MAX;
	public int  currentLevel = 0;

	public AudioClip takeDamageSound;
	public AudioClip defeatedSound;

	public float challengeConstant;
	public float challengeMultiplier = 1.0f;
	public float challengeExponent = 1.6f;

	public float  rewardConstant;
	public float  rewardMultiplier = 1.0f;
	public float  rewardExponent   = 1.6f;

	public float  healthCostConstant;
	public float  healthCostMultiplier = 1.0f;
	public float  healthCostExponent   = 1.0f;

	public float  staminaCostConstant;
	public float  staminaCostMultiplier = 1.0f;
	public float  staminaCostExponent   = 1.0f;

	[System.NonSerialized]
	public long currentHealth;
	public float  rechargeTime = 10.0f;


	private float rechargeTimeLeft = 0.0f;

	private Animator myAnimator;

	//private int   rewardValue;




	public SpriteRenderer mySprite;


	// Use this for initialization
	void Start () 
	{
		
		InitValues(currentLevel);
		if(!mySprite)
		{
			mySprite = GetComponentInChildren<SpriteRenderer>();
		}
		myAnimator = GetComponentInChildren<Animator>();

		UpdateVisibility();

	}

	public void InitValues(int level)
	{
		//rewardValue = (int)(rewardConstant + rewardMultiplier * Mathf.Pow(level, rewardExponent));
		currentHealth =  (long)(challengeConstant + challengeMultiplier * Mathf.Pow(level, challengeExponent));

		//scale up
		float scale = 1.0f + currentLevel * 0.3f;
		transform.localScale = new Vector3(scale, scale, scale);


	}


	public int GetHealthCost()
	{
		return Mathf.CeilToInt(healthCostConstant + healthCostMultiplier * Mathf.Pow(currentLevel, healthCostExponent ));
	}

	public float GetStaminaCost()
	{
		return staminaCostConstant + staminaCostMultiplier * Mathf.Pow(currentLevel, staminaCostExponent );
	}

	// Update is called once per frame
	void Update () 
	{
		if(!Application.isPlaying)
		{
			InitValues(currentLevel);
		}
		else
		{
			
			if( rechargeTimeLeft > 0 )
			{
				rechargeTimeLeft -= Time.deltaTime;
				if(rechargeTimeLeft < 0)
				{
					//end recharge
					currentLevel++;
					rechargeTimeLeft = 0.0f;
					SetRecharging(false);
					InitValues(currentLevel);

				}
			}
		}
	}


	public void UpdateVisibility()
	{
		if(Application.isEditor && !Application.isPlaying)
		{
			gameObject.SetActive(true);

		}
		else
		{
			if(requiredUpdateType != UpgradeType.Upgrade_MAX)
			{
				int lvl = GameInstanceManager.Instance().GetUpgradeLevel( requiredUpdateType );
				gameObject.SetActive( lvl > 0 );
			}
		}
	}

	public bool TakeDamage(long damageAmt)
	{
		
		if(IsDead()) return false;



		currentHealth -= damageAmt;

		if(currentHealth <= 0)
		{
			DoDeath();
		}



		return true;
	}

	public int GetRewardValue()
	{
		return Mathf.CeilToInt((rewardConstant + rewardMultiplier * Mathf.Pow(currentLevel, rewardExponent)));
	}

	void DieLevelUp()
	{
		GameInstanceManager.Instance().AddGold( GetRewardValue(), transform.position );

		rechargeTimeLeft = rechargeTime;
		SetRecharging(true);


	}

	/*public bool IsRecharging()
	{
		return ( rechargeTimeLeft > 0.0f );
	}*/

	void SetRecharging(bool recharging)
	{
		


	}

	private static Color deathColor = new Color(.3f, .3f, .3f, 1.0f);
	private void DoDeath()
	{
		if(defeatedSound != null)
		{
			PlaySound(defeatedSound);
		}

		GameInstanceManager.Instance().AddGold( GetRewardValue(), transform.position );

		if( myAnimator )
		{
			myAnimator.SetBool("sleeping", true);
		}

		Collider c = GetComponentInChildren<Collider>();
		if(c)
		{
			c.enabled = false;
		}

		if(mySprite)
		{
			mySprite.color = deathColor;
		}

		//GameObject.Destroy(gameObject, 0.1f);

	}



	public bool IsDead()
	{
		return (gameObject == null || currentHealth <= 0);
	}

	public void PlayDamageSoundAndEffect()
	{
		if(takeDamageSound != null)
		{
			PlaySound(takeDamageSound);
		}
		myAnimator.SetTrigger("hit");
	}

	void PlaySound(AudioClip clip)
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(clip);
	}

}
