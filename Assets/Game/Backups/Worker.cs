using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using RiseReign;
using System;

[RequireComponent(typeof(EnemySensor))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(GoapAgent))]
/// <summary>
/// //Base class for all AI types
/// Worker -> What it does: Reports world facts, Signals danger (NeedsToHide) & Does NOT interrupt or replan
/// </summary>

public abstract class Worker : MonoBehaviour, IGoap
{
	public static Action<float> updateAnimatorPanicSpeed;
	public static Func<float> requestAnimatorSpeed;

	public enum Faction
	{
		VILLAGER,
		PREDATOR
	}
	
	[SerializeField]
    protected Faction faction;

    public Faction GetFaction()
    {
        return faction;
    }
	[SerializeField] private Health health;

	[SerializeField] protected NavMeshAgent agent;
	[SerializeField] protected Animator m_anim;
	[SerializeField] protected float walkSpeed = 0.5f;

	Vector3 previousDestination;
	
	[Tooltip("Stockpile")]
	public Inventory stockpile;
	public Inventory windmill;
	public Inventory granary;
	public Inventory wheatField;
	public Inventory ironMine;
	public Backpack ownInv;
	// public Inventory forest;
	public bool close = false;
	bool hide = false;
	float safetyFactor;
	public float safety
	{
		get => safetyFactor;
		set => safetyFactor = value;
	}

	EnemySensor m_enemySensor;
	public float attackRange = 2f;

	void Start()
	{
		agent = this.GetComponent<NavMeshAgent>();
		if(m_anim == null) Debug.LogErrorFormat("No Animator component attached!");
		if(agent != null) agent.speed = walkSpeed;
		ownInv = this.GetComponent<Backpack>();
		m_enemySensor = GetComponent<EnemySensor>();
		stockpile = FindFirstObjectByType<Stockpile>().GetComponent<Inventory>();;
		windmill = FindFirstObjectByType<Windmill>().GetComponent<Inventory>();;
		granary = FindFirstObjectByType<Granary>().GetComponent<Inventory>();;
		wheatField = FindFirstObjectByType<WheatField>().GetComponent<Inventory>();;
		ironMine = FindFirstObjectByType<IronMine>().GetComponent<Inventory>();
		ownInv = GetComponent<Backpack>();
	}

	Vector3 lastKnownThreatPosition;
    bool hasLastKnownThreat = false;

    public void SetLastKnownThreatPosition(Vector3 pos)
    {
        lastKnownThreatPosition = pos;
        hasLastKnownThreat = true;
    }

    public bool TryGetLastKnownThreatPosition(out Vector3 pos)
    {
        pos = lastKnownThreatPosition;
        return hasLastKnownThreat;
    }

    public void ClearLastKnownThreat()
    {
        hasLastKnownThreat = false;
    }

	public HashSet<KeyValuePair<string,object>> GetWorldState () 
	{
		HashSet<KeyValuePair<string,object>> worldData = new HashSet<KeyValuePair<string,object>> ();
		// Danger / safety state (for GOAP interrupt handling)
		//*****************New danger logic***********************
		Worker enemy = null;

		if (m_enemySensor != null)
		{
		    m_enemySensor.Scan();
		    enemy = m_enemySensor.GetCurrentEnemy();
		}

		// Detect enemy
		//***************************************************
		bool enemyVisible = enemy != null;
		bool enemyInRange = false;

		if (enemyVisible)
		{
		    float distance = Vector3.Distance(
		        transform.position,
		        enemy.transform.position
		    );

		    enemyInRange = distance <= attackRange;
		}


		// if (enemyVisible)
		// {
		//     //when comparing distance .sqrMagnitude is cheaper.  
		// 	// If the actual numerical value is needed, the more expensive .magnitude is necessary
		// 	float sqrDistance = (transform.position - enemy.transform.position).sqrMagnitude;

		//     enemyInRange = sqrDistance <= attackRange * attackRange;
		// }
		bool enemyDead = enemy == null || enemy.IsDead();
		//****************************************************
		//Sticking to manually calling SetHide() for now.  Will use this later for auto-dynamic avoidance
		// bool avoidEnemy = enemyVisible && GetHealthPercent() < 0.8f;
		// worldData.Add(new KeyValuePair<string, object>("avoidEnemy", avoidEnemy));

		worldData.Add(new KeyValuePair<string, object>("enemyVisible", enemyVisible));
		worldData.Add(new KeyValuePair<string, object>("enemyInRange", enemyInRange));
		worldData.Add(new KeyValuePair<string, object>("enemyDead", enemyDead));
		worldData.Add(new KeyValuePair<string, object>("healthPercent", (float)health.AgentHealth / health.MaxHealth));
		worldData.Add(new KeyValuePair<string, object>("healthyEnoughToAttack", GetHealthPercent() > 0.5f));

		//**********************************************

		worldData.Add(new KeyValuePair<string, object>("canSeePlayer", false ));

		// ===== FOOD PIPELINE (CORRECT & ROLE-SAFE) =====

		// Wheat exists in the world (for Miller)
		worldData.Add(new KeyValuePair<string, object>("fieldHasWheat", wheatField.wheatLevel > 0));

		// Miller backpack
		worldData.Add(new KeyValuePair<string, object>("hasWheat", ownInv.wheatLevel > 0));
		worldData.Add(new KeyValuePair<string, object>("hasWheatAtMill", granary.wheatLevel > 0));


		// Flour exists at the mill (Miller goal, Baker dependency)
		worldData.Add(new KeyValuePair<string, object>("hasFlourAtMill", granary.flourLevel > 0));

		// Baker backpack
		worldData.Add(new KeyValuePair<string, object>("hasFlour", ownInv.flourLevel >= 1));
		worldData.Add(new KeyValuePair<string, object>("hasBread", ownInv.breadLevel > 0));

		// Iron Miner Backpack
		worldData.Add(new KeyValuePair<string, object>("hasIronOre", ownInv.ironOreLevel >= 1));

		// Final output
		worldData.Add(new KeyValuePair<string, object>("hasBreadInStockpile", stockpile.breadLevel > 0));
		worldData.Add(new KeyValuePair<string, object>("hasIronOreInStockpile", stockpile.ironOreLevel > 0));
		// worldData.Add(new KeyValuePair<string, object>("hasIronOreInStockpile", ownInv.ironOreLevel == 0));
		return worldData;
	}

	public abstract HashSet<KeyValuePair<string,object>> CreateGoalState ();

	public bool MoveAgent(GoapAction nextAction) 
	{
		//TODo: optimize by using .sqrMagnitude since only comparing distances?
		
		agent.SetDestination(nextAction.target.transform.position);

		if (agent.hasPath && agent.remainingDistance <= 1) 
		{
			nextAction.setInRange(true);
			previousDestination = nextAction.target.transform.position;
			return true;
		} 
		else
		{
			return false;
		}
	}
	float lastSpeed;

	protected virtual void Update()
	{
		// If the agent is alive, check pathing etc.
		if(agent.hasPath)
		{
			Vector3 toTarget = agent.steeringTarget - this.transform.position;
         	float turnAngle = Vector3.Angle(this.transform.forward,toTarget);
         	agent.acceleration = turnAngle * agent.speed;
		}

		if (agent.speed != lastSpeed)
    	{
    	    //See where agent's speed was changed or set.
			Debug.Log(
    	        $"SPEED CHANGED to {agent.speed} by stack trace:\n{Environment.StackTrace}"
    	    );
    	    lastSpeed = agent.speed;
    	}
		UpdateAnimator();
	}

	public void PlanFailed (HashSet<KeyValuePair<string, object>> failedGoal)
	{

	}

	public void PlanFound (HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
	{

	}

	public void ActionsFinished ()
	{

	}

	public void PlanAborted (GoapAction aborter)
	{
		
		// Reset the failed action
    	if (aborter != null)
    	{
    	    aborter.doReset();
    	}

    	// Stop moving
    	if (agent != null)
    	{
    	    agent.isStopped = false;
			agent.ResetPath();
    	}
	}

	public bool GetNeedsToHide()
	{
		return hide;
	}

	public void SetHide(bool hideOrNot)
	{
		Debug.Log("Called SetHide");
		hide = hideOrNot;
		GoapAgent agent = GetComponent<GoapAgent>();
    	if (agent != null)
    	{
    	    agent.InterruptAction();
			Debug.Log("Called interruption");
    	}
	}
	public void TakeDamage(int amount)
	{
	    health.AgentHealth -= amount;

	    if (health.AgentHealth <= 0)
	    {
	        Die();
	    }
	}

	public bool IsDead()
	{
	    return health.AgentHealth <= 0;
	}

	public float GetHealthPercent()
	{
	    return (float)health.AgentHealth / 100f;
	}
    void OnEnable()
    {
        HideAction.panicSpeedEventNotifier += SetPanicApeed;
		HideAction.normalAgentSpeedEventNotifier += ResumeNormalSpeed;
    }
    void OnDisable()
    {
        HideAction.panicSpeedEventNotifier -= SetPanicApeed;
		HideAction.normalAgentSpeedEventNotifier -= ResumeNormalSpeed;
    }
	
	public float boostedSpeed;
	bool isPanicking;
	public void SetPanicApeed(float panicSpeed)
	{
		if( isPanicking) return; //already panicking, no need to do more
		boostedSpeed = walkSpeed * panicSpeed;
		agent.speed = boostedSpeed;
		isPanicking = true;
		Debug.Log($"Panic speed is now {agent.speed}");
	}
	void ResumeNormalSpeed()
	{
		if(isPanicking)
		{
			agent.speed = walkSpeed;
			isPanicking = false;
		}
		Debug.Log($"{this.GetType()}: Back to normal speed {agent.speed}");
	}
	void UpdateAnimator()
    {
        //First get global velocity on navmesh agent
        Vector3 velocity = agent.velocity;
        //convert to local velocity
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        //Which direction of interest for movement
        // float speed = localVelocity.z;

		//Influence the float parameter on the animator by feding it the speed values from the local velocity.
		//2D directional
		float moveX = localVelocity.x / walkSpeed;
    	float moveY = localVelocity.z / walkSpeed;

    	m_anim.SetFloat("MoveX", moveX);
    	m_anim.SetFloat("MoveY", moveY);

		// 1D directional
		// speed = Mathf.Clamp(speed, 0f, 2f);
        // m_anim.SetFloat("MoveY", speed);
		// m_anim.SetFloat("MoveY", boostedSpeed);
		Debug.Log($"UpdateAnimator() speed {moveY}");
    }
	public void Die()
	{
	    if (IsDead())
	    {
	        GetComponent<GoapAgent>().enabled = false;

	        if (agent != null)
	        {
	            agent.isStopped = true;
	            agent.ResetPath();
	        }
	    }
	}
}
