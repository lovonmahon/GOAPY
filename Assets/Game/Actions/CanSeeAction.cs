using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiseReign;

public class CanSeeAction : GoapAction {

	//[SerializeField] float timeBetweenAttack = 1.0f;
	   
	bool m_sawPlayer = false;
    Animator anim;
    Sight sight;
	#region custom interrupt
	GoapAgent goapAgent;
	#endregion

	void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        sight = gameObject.GetComponent<Sight>();        
		goapAgent = gameObject.GetComponent<GoapAgent>();
    }
    
    public CanSeeAction(){
		// addPrecondition("canSeePlayer", false);
		// addEffect ("canSeePlayer", true);
		addEffect ("doJob", true);
    	name = "Can see the player";
	}

	void Update()
    {
        //
    }
    
    public override void reset() {
		target = null;
	}

	public override bool isDone(){
		return m_sawPlayer;
	}

	public override bool requiresInRange(){
		return false;
	}

	public override bool checkProceduralPrecondition(GameObject agent)
	{
		if (sight.isInFOV == true)
		{
			target = GameObject.FindGameObjectWithTag("Player");
			if (target != null)
			{
				Debug.Log("I can see the player now");
				return true;
			}
		}	
		return false;
	}

	
	public override bool perform(GameObject agent)
	{
	    ///<Summary>
		/// 
		/// Rule to follow

		// Actions interrupt actions
		// Agent handles replanning
		// Actions do NOT call GoapAgent.InterruptAction()

		///</Summary>
		if (sight.isInFOV)
	    {
	        Debug.Log("Bear spotted!");

	        // Mark THIS action as done
	        m_sawPlayer = true;

	        // FAIL the plan → agent will replan
	        return false;
	    }

	    return true;
	}

}
