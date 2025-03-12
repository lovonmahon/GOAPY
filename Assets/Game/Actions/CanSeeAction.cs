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
		 // Only perform the action if the player is visible
    	if (!m_sawPlayer && sight.isInFOV)
    	{
    	    m_sawPlayer = true;
    	    Debug.Log("Ah ketch eem!");    

    	    // Interrupt the current action if agent sees the player
    	    GoapAgent goapAgent = agent.GetComponent<GoapAgent>();
    	    if (goapAgent != null) 
    	    {
    	        goapAgent.InterruptAction(); // Interrupt current action and replan
    	    }
    	}

    	return m_sawPlayer;
	}
}
