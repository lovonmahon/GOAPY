using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

/// <summary>
/// “When the baker is done doing their job, what should be true about the world?”
/// </summary>




public sealed class Baker : Worker
{
    //Baker bakes bread (and future pastries).
   public override HashSet<KeyValuePair<string,object>> CreateGoalState ()
	{
		HashSet<KeyValuePair<string,object>> goal = new HashSet<KeyValuePair<string,object>> ();
		goal.Add(new KeyValuePair<string, object>("hasBreadInStockpile", true ));

        ///Now the planner can:

        /// see the goal: hasBread == true
        /// 
        /// choose actions that lead there:
        /// 
        /// PickupFlour
        /// 
        /// BakeBread
        /// 
        /// abort and replan if danger appears
        /// 
        /// reuse the same actions for other roles later

		return goal;
	}

    // Update is called once per frame
    void Update()
    {
        // UpdateAnimator();
    }

    void UpdateAnimator()
    {
        //First get global velocity on navmesh agent
        Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
        //convert to local velocity
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        //Which direction of interest for movement
        float speed = localVelocity.z;
        //Influence the float parameter on the animator by feding it the speed values from the local velocity.
        GetComponent<Animator>().SetFloat("forwardSpeed", speed);
    }
}
