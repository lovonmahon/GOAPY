using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class CollectLogFromStockpileAction : GoapAction {

    bool completed = false;
    float startTime = 0;
    public float workDuration = 5; // seconds
    public Backpack ownInv;
    public Inventory forest;
    Animator anim;
    NavMeshAgent _agent;
    [SerializeField] Transform pickupHand;
    public float ArrivalDistance = 2.0f;
    Tree tree;

    // State-saving variables
    private float savedStartTime;
    private bool savedCompleted;

    public CollectLogFromStockpileAction () {
        addPrecondition ("hasLogsStocked", true);
        addEffect ("gotLogs", true);
        name = "pick up log from stockpile to take to saw mill";
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        if( anim == null)
        {
            Debug.Log("No animator found!");
        }
        _agent = GetComponent<NavMeshAgent>();
        if( _agent == null)
        {
            Debug.Log("No NavMeshAgent found!");
        }
    }

    void Update()
    {
        
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
        target = GameObject.FindGameObjectWithTag("Stockpile");
        if(target != null)
        {
            return true;
        }
        return false;
    }

    public override bool perform (GameObject agent)
    {
        if (startTime == 0 )
        {
            startTime = Time.time;
        }

        if (Time.time - startTime > workDuration) 
        {
            anim.SetTrigger("pickUp");

            // Simulate log collection (assuming you have inventory logic implemented elsewhere)
            // ownInv.logs += 5;
            // stockpile.logs -= 5;
            completed = true;
        }
        return true;
    }

   
}
