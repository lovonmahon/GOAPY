using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class PredatoryAnimal : Worker
{
    //Predatory animal
    void Awake()
    {
        faction = Faction.PREDATOR;
    }
    public override HashSet<KeyValuePair<string,object>> CreateGoalState ()
    {
    	HashSet<KeyValuePair<string,object>> goal = new HashSet<KeyValuePair<string,object>> ();
    
        // Safety must override everything
        if (GetNeedsToHide())
        {
            goal.Add(new KeyValuePair<string, object>("avoidEnemy", true));
            return goal;
        }
          
        EnemySensor sensor = GetComponent<EnemySensor>();
        Worker enemy = sensor?.GetCurrentEnemy();
    
        goal.Add(new KeyValuePair<string, object>("enemyDead", true));
        return goal;
    }

    // Update is called once per frame
    protected override void Update()
    {
        UpdateAnimator();
    }

    // Good for 1D blend tree
    void UpdateAnimator()
    {
        //First get global velocity on navmesh agent
        Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
        //convert to local velocity
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        //Which direction of interest for movement
        float speed = localVelocity.z;
        //Influence the float parameter on the animator by feding it the speed values from the local velocity.
        m_anim.SetFloat("forwardSpeed", speed);
    }
}
