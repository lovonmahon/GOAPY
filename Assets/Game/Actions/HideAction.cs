using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiseReign;
using UnityEditor.Build.Reporting;
using UnityEngine.AI;
using System;


/// <summary>
/// While Cutting trees....
// EXAMPLE:
//Bear appears → hide = true
//World state: isSafe = false
//Planner includes HideAction
//Agent executes HideAction
//Bear leaves → hide = false
//World state: isSafe = true
//HideAction completes
//Agent replans to work
/// </summary>

// public class HideAction : GoapAction {

// 	[SerializeField] NavMeshAgent m_nav;

// 	bool m_sawPlayer = false;
//     [SerializeField] Animator anim;
//     Sight sight;

//     void Awake()
//     {
//         ActionName = "Hide";
//     }
//     public HideAction()
// 	{
// 		addPrecondition("isSafe", false);//This tells the planner: “Only consider HideAction when I am not safe.”
// 		addEffect ("isSafe", true); //"what the world is expected to look like after the action completes successfully"
// 	}

//     public override void reset() 
// 	{
// 		if (m_nav != null)
// 		{
// 			m_nav.isStopped = false;
// 		}
//     	// if (anim != null)
// 		// {
// 		// 	anim.SetBool("IsHiding", false);
// 		// }
// 		target = null;
// 	}

// 	public override bool isDone() // Am I finished forever?
// 	{
// 	    // Done when no longer in danger
//         return isInRange() && !GetComponent<Worker>().GetNeedsToHide();
// 	}

// 	public override bool requiresInRange()
// 	{
// 		return true;
// 	}

// 	public override bool checkProceduralPrecondition(GameObject agent)
// 	{
// 		//  sight = gameObject.GetComponent<Sight>();
// 		if(anim == null)
// 		{
// 			anim = agent.GetComponentInChildren<Animator>();
// 		}
// 		if(GetComponent<Worker>().GetNeedsToHide())
//         {
//             target = GameObject.FindGameObjectWithTag("HidingSpot");
//             return target != null;
//         }
//         return false;
// 	}

// 	/// <summary>
// 	/// 👉 Because perform() is called every frame until isDone() returns true.
// 	///Returning true does not mean “done”.
// 	/// It means “I’m still executing successfully.
// 	/// </summary>
// 	/// <param name="agent"></param>
// 	/// <returns></returns>

// 	public override bool perform(GameObject agent)  // Am I still valid this frame?
// 	{
// 		// Stay hiding until safe
//         if (!GetComponent<Worker>().GetNeedsToHide()) return false; 

// 		// Still moving toward hiding spot
//     	if(!isInRange())
// 		{
// 			return true;
// 		}
// 		 // NOW we are at the hiding spot
//     	m_nav.isStopped = true;
//     	m_nav.velocity = Vector3.zero;

// 		Debug.Log($"HideAction: inRange={isInRange()} stopped={m_nav.isStopped}");


//     	// if(anim != null)
// 		// {
// 		// 	anim.SetBool("IsHiding", true);
// 		// }

//     	return true;
// 	}
// }


public class HideAction : GoapAction
{
    public static Action<float> panicSpeedEventNotifier;
    public static Action normalAgentSpeedEventNotifier;
    Worker worker;
    NavMeshAgent agent;
    [SerializeField] Animator m_anim;
    [SerializeField] float m_speedMultiplier = 3.0f; // pick what feels right

    public Transform hideSpot;
    bool hiding = false;

    public HideAction()
    {
        // Effects
        addEffect("avoidEnemy", true);

        cost = 1f; // VERY low cost so it always wins
    }

    public override void reset()
    {
        hiding = false;
        worker = null;
        agent = null;
        target = null;
        normalAgentSpeedEventNotifier?.Invoke();
    }

    public override bool requiresInRange()
    {
        return true;
    }

    public override bool isDone()
    {
        // Stay hidden UNTIL danger clears
        return hiding && !worker.GetNeedsToHide();
    }

    public override bool checkProceduralPrecondition(GameObject agentObj)
    {
        //checkProceduralPrecondition is GOAP's version of Start()
        worker = agentObj.GetComponent<Worker>();
        agent = agentObj.GetComponent<NavMeshAgent>();
        
        if (!worker.GetNeedsToHide())
        {
            return false;
        }
        panicSpeedEventNotifier?.Invoke(m_speedMultiplier);
       
        if (hideSpot == null)
		{
			return false;
		}
        target = hideSpot.gameObject;

        if(m_anim != null)
        {
            m_anim.SetTrigger("Scared");
        }
        return true;
    }

    public override bool perform(GameObject agentObj)
    {
		//While moving toward the hiding spot, run fast
        if (!hiding)
        {
            hiding = true;
        }
        // If not close to the hiding spot, keep reevaluating
        if (!isInRange())
		{
			//true → “I am still executing successfully.”
            //false → “I failed. Abort and replan.”
            return true;
		}
        // While hiding, do NOTHING — wait
        if (worker.GetNeedsToHide())
		{
            return true;
		}
        return true;
    }

	public void MoveToImmediate(Vector3 position)
	{
	    if (agent == null) return;

	    agent.ResetPath();
	    agent.isStopped = false;
	    agent.SetDestination(position);
	}
}

