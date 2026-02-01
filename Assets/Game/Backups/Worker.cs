using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using RiseReign;

/// <summary>
/// Worker -> What it does: Reports world facts, Signals danger (NeedsToHide) & Does NOT interrupt or replan
/// </summary>
public abstract class Worker : MonoBehaviour, IGoap
{
	//Base class for all AI types
	NavMeshAgent agent;
	Vector3 previousDestination;
	
	[Tooltip("Stockpile")]
	public Inventory stockpile;
	public Inventory windmill;
	// public Inventory lumbermill;
	public Backpack ownInv;
	Inventory inv;
	// public Inventory forest;
	public bool close = false;
	bool hide = false;
	public float moveSpeed = 1.5f;
	void Start()
	{
		agent = this.GetComponent<NavMeshAgent>();
		// inv = this.GetComponent<Inventory>();
		ownInv = this.GetComponent<Backpack>();
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
		worldData.Add(new KeyValuePair<string, object>("enemyVisible", hide));
		worldData.Add(new KeyValuePair<string, object>("isSafe", !hide));
		
		worldData.Add(new KeyValuePair<string, object>("canSeePlayer", false ));
		worldData.Add(new KeyValuePair<string, object>("hasWheat", (stockpile.wheatLevel > 4) ));

		worldData.Add(new KeyValuePair<string, object>("hasFlourStock", (windmill.flourLevel > 4) ));
		worldData.Add(new KeyValuePair<string, object>("hasFlour", (ownInv.flourLevel > 1) ));

		worldData.Add(new KeyValuePair<string, object>("hasBread", (ownInv.breadLevel > 4) ));
		worldData.Add(new KeyValuePair<string, object>("hasBreadInStockpile", (stockpile.breadLevel > 4) ));
		
		// worldData.Add(new KeyValuePair<string, object>("hasTrees", (stockpile.trees > 1) ));
		// worldData.Add(new KeyValuePair<string, object>("hasLogs", (inv.trees > 0) ));
		
		//Wood Cutter
		// worldData.Add(new KeyValuePair<string, object>("hasTrees", (forest.logs > 4) ));
		worldData.Add(new KeyValuePair<string, object>("hasLogs", (ownInv.logs > 4) ));
		worldData.Add(new KeyValuePair<string, object>("hasLogsDelivery", (ownInv.logsToDeliver > 4) ));

		//Saw mill
		worldData.Add(new KeyValuePair<string, object>("hasLogsStocked", (stockpile.logs > 4) ));

		//Builder
		// worldData.Add(new KeyValuePair<string, object>("hasLumber", (lumbermill.lumber > 4) ));
		worldData.Add(new KeyValuePair<string, object>("hasTools", (stockpile.tools > 1) ));

		//Toolsmith needs Iron ore to make tools
		worldData.Add(new KeyValuePair<string, object>("hasIronOre", (stockpile.ironOre > 4) ));
		
		//Iron ore miner  (also will need pick axe to mine - toolsmith supplies axes)
		worldData.Add(new KeyValuePair<string, object>("hasIronOreInMine", true ));


		//Hiding		
		// worldData.Add(new KeyValuePair<string, object>("Hide", false ));
		return worldData;
	}


	public abstract HashSet<KeyValuePair<string,object>> CreateGoalState ();
	// {
	// 	// HashSet<KeyValuePair<string,object>> goal = new HashSet<KeyValuePair<string,object>> ();
	// 	// goal.Add(new KeyValuePair<string, object>("doJob", true ));

	// 	// return goal;
	// }


	public bool MoveAgent(GoapAction nextAction) 
	{
		//if we don't need to move anywhere
		if(previousDestination == nextAction.target.transform.position)
		{
			nextAction.setInRange(true);
			return true;
		}
		
		agent.SetDestination(nextAction.target.transform.position);
		
		if (agent.hasPath && agent.remainingDistance < 2) 
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

	void Update()
	{
		if(agent.hasPath)
		{
			Vector3 toTarget = agent.steeringTarget - this.transform.position;
         	float turnAngle = Vector3.Angle(this.transform.forward,toTarget);
         	agent.acceleration = turnAngle * agent.speed;
		}
		// if(GetComponent<Sight>().isInFOV)
		// {
		// 	interrupt = true;
		// }
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
    	    agent.ResetPath();
    	}
	}

	public bool GetNeedsToHide()
	{
		return hide;
	}

	public void SetHide(bool hideOrNot)
	{
		hide = hideOrNot;
	}
}
