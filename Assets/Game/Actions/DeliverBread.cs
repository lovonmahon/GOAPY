using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverBread : GoapAction {

	bool completed = false;
	float startTime = 0;
	public float workDuration = 2; // seconds
	Worker worker;
	public Inventory marketInventory;
	
	public DeliverBread () 
	{
		// Planner-facing logic
        addPrecondition("hasBread", true);

        // After delivery, the agent no longer has bread
        addEffect("hasBread", false);
        addEffect("hasBreadInStockpile", true);
	}
    void Awake()
    {
        ActionName = "Deliver Bread";
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
		worker = agent.GetComponent<Worker>();

        target = GameObject.FindGameObjectWithTag("Stockpile");
        return target != null;
	}
	
	public override bool perform (GameObject agent)
	{
		// Abort if danger appears
        if (isInterrupted() || worker.GetNeedsToHide())
		{
			return false;
		}
            
		if (startTime == 0)
		{
			Debug.Log("Starting: " + ActionName);
			startTime = Time.time;
		}

		if (Time.time - startTime > workDuration) 
		{
			Debug.Log("Finished: " + ActionName);
			agent.GetComponent<Backpack>().breadLevel -= 5;
			marketInventory.breadLevel += 5;

			completed = true;
		}
		return true;
	}
	
}
