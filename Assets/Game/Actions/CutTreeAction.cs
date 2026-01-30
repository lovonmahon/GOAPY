using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class CutTreeAction : GoapAction {

	bool completed = false;
	float startTime = 0;
	public float workDuration = 10; // seconds
    public Inventory forest;
	public Backpack ownInv;
	Animator anim;
	NavMeshAgent _agent;
	[SerializeField] GameObject log;
	[SerializeField] Transform pickupPosition;
	public float ArrivalDistance = 2.0f;
	
	public CutTreeAction () {
		addPrecondition ("hasTrees", true);
		addEffect ("doJob", true);
		addEffect ("hasFallenLogs", true);
		addEffect ("hasTrees", false);
		name = "Cut down trees";
	}

	void Start()
	{
		anim = GetComponent<Animator>();
		if( anim == null)
		{
			Debug.Log("No animator found!");;
		}
		_agent = GetComponent<NavMeshAgent>();
		if( _agent == null)
		{
			Debug.Log("No NavMeshAgent found!");;
		}
	}
	
	public override void reset ()
	{
		completed = false;
		startTime = 0;
	}
	
	public override bool isDone ()
	{
		return completed;
	}
	
	public override bool requiresInRange ()
	{
		return true;
	}
	
	public override bool checkProceduralPrecondition (GameObject agent)
	{	
		target = GameObject.FindGameObjectWithTag("Tree");
		if(target != null)
		{
			return true;
		}
		return false;
	}
	
	public override bool perform (GameObject agent)
	{
		// INTERRUPTION → fail action → agent replans
		if (isInterrupted()) return false;
		
		anim.SetTrigger("cutTree");
		if (startTime == 0 )
		{
			startTime = Time.time;
		}

		if (Time.time - startTime > workDuration) 
		{
			// Invoke("UpdateInventories", 1f);
			forest.logs -= 5;
			ownInv.logs += 5;
			
			completed = true;
			Instantiate(log, pickupPosition.position, Quaternion.identity);
			anim.ResetTrigger("cutTree");
		}
		
		return true;
	}
	IEnumerator SpawnLog()
	{
		yield return new WaitForSeconds(workDuration);
		Instantiate(log, pickupPosition.position, Quaternion.identity);
		
		// anim.ResetTrigger("cutTree");
	}
	IEnumerator PlayCuttingAnimation()
	{
		yield return new WaitForSeconds(0.1f);
		// anim.SetTrigger("cutTree");
	}

	void UpdateInventories()
	{
		forest.logs -= 5;
		ownInv.logs += 5;
		
		if(forest.logs < 0)
		{
			forest.logs = 0;
		}
		if(ownInv.logs < 0)
		{
			ownInv.logs = 0;
		}
		
	}
	
}
