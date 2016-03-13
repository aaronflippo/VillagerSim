using UnityEngine;
using System.Collections;

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

	private float currentHealth;
	public float  rechargeTime = 10.0f;


	private float rechargeTimeLeft = 0.0f;

	private Animator myAnimator;

	//private int   rewardValue;




	private SpriteRenderer mySprite;


	// Use this for initialization
	void Start () {
		InitValues(currentLevel);
		mySprite = GetComponentInChildren<SpriteRenderer>();
		myAnimator = GetComponentInChildren<Animator>();

		UpdateVisibility();
	}

	public void InitValues(int level)
	{
		//rewardValue = (int)(rewardConstant + rewardMultiplier * Mathf.Pow(level, rewardExponent));
		currentHealth =  (int)(challengeConstant + challengeMultiplier * Mathf.Pow(level, challengeExponent));

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


	public void UpdateVisibility()
	{
		if(requiredUpdateType != UpgradeType.Upgrade_MAX)
		{
			int lvl = GameInstanceManager.Instance().GetUpgradeLevel( requiredUpdateType );
			gameObject.SetActive( lvl > 0 );
		}
	}

	public bool TakeDamage(float damageAmt)
	{
		
		if(IsDead()) return false;

		if(takeDamageSound != null)
		{
			PlaySound(takeDamageSound);
		}

		currentHealth -= damageAmt;

		if(currentHealth <= 0)
		{
			DoDeath();
		}

		if(myAnimator)
		{
			myAnimator.SetTrigger("hit");
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
			mySprite.color = Color.gray;
		}

		//GameObject.Destroy(gameObject, 0.1f);

	}



	public bool IsDead()
	{
		return (gameObject == null || currentHealth <= 0);
	}


	void PlaySound(AudioClip clip)
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(clip);
	}

}
