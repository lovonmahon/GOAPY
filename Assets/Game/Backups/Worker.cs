using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using RiseReign;

public abstract class Worker : MonoBehaviour, IGoap
{
	//Base class for all AI types
	NavMeshAgent agent;
	Vector3 previousDestination;
	// Inventory inv;
	[Tooltip("Stockpile")]
	public Inventory stockpile;
	public Inventory lumbermill;
	public Backpack ownInv;
	public Inventory forest;
	public bool interrupt = false;
	public bool close = false;
	bool hide = false;
	public float moveSpeed = 1.5f;
	void Start()
	{
		agent = this.GetComponent<NavMeshAgent>();
		// inv = this.GetComponent<Inventory>();
		ownInv = this.GetComponent<Backpack>();
	}

	public HashSet<KeyValuePair<string,object>> GetWorldState () 
	{
		HashSet<KeyValuePair<string,object>> worldData = new HashSet<KeyValuePair<string,object>> ();
		worldData.Add(new KeyValuePair<string, object>("canSeePlayer", false ));
		worldData.Add(new KeyValuePair<string, object>("hasStock", (stockpile.flourLevel > 4) ));
		worldData.Add(new KeyValuePair<string, object>("hasFlour", (ownInv.flourLevel > 1) ));
		worldData.Add(new KeyValuePair<string, object>("hasDelivery", (ownInv.breadLevel > 4) ));
		// worldData.Add(new KeyValuePair<string, object>("hasTrees", (stockpile.trees > 1) ));
		// worldData.Add(new KeyValuePair<string, object>("hasLogs", (inv.trees > 0) ));
		
		//Wood Cutter
		worldData.Add(new KeyValuePair<string, object>("hasTrees", (forest.logs > 4) ));
		worldData.Add(new KeyValuePair<string, object>("hasLogs", (ownInv.logs > 4) ));
		worldData.Add(new KeyValuePair<string, object>("hasLogsDelivery", (ownInv.logsToDeliver > 4) ));

		//Saw mill
		worldData.Add(new KeyValuePair<string, object>("hasLogsStocked", (stockpile.logs > 4) ));

		//Builder
		worldData.Add(new KeyValuePair<string, object>("hasLumber", (lumbermill.lumber > 4) ));
		worldData.Add(new KeyValuePair<string, object>("hasTools", (stockpile.tools > 1) ));

		//Toolsmith needs Iron ore to make tools
		worldData.Add(new KeyValuePair<string, object>("hasIronOre", (stockpile.ironOre > 4) ));
		
		//Iron ore miner  (also will need pick axe to mine - toolsmith supplies axes)
		worldData.Add(new KeyValuePair<string, object>("hasIronOre", true ));


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


	public bool MoveAgent(GoapAction nextAction) {
		//if we don't need to move anywhere
		if(previousDestination == nextAction.target.transform.position)
		{
			nextAction.setInRange(true);
			return true;
		}
		
		agent.SetDestination(nextAction.target.transform.position);
		
		if (agent.hasPath && agent.remainingDistance < 2) {
			nextAction.setInRange(true);
			previousDestination = nextAction.target.transform.position;
			return true;
		} else
			return false;
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
		// GetComponent<GoapAgent>().DataProvider().ActionsFinished();
		// aborter.reset ();	//Calling from action scripts
		// aborter.doReset();//Calling from GoapAction.cs
	}

	public bool GetNeedsToHide()
	{
		return hide;
	}

	public void SetHide(bool hideOrNot)
	{
		hide = hideOrNot;
		interrupt = true;
	}
}
