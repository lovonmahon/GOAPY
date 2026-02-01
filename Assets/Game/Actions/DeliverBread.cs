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

        target = GameObject.FindGameObjectWithTag("Market");
        if (target == null) return false;

        marketInventory = target.GetComponent<Inventory>();
        return marketInventory != null;
	}
	
	public override bool perform (GameObject agent)
	{
		// Abort if danger appears
        if (isInterrupted() || worker.GetNeedsToHide())
		{
			return false;
		}
            
		Backpack inv = agent.GetComponent<Backpack>();
        if (inv.breadLevel < 5)
            return false;
		
		if (startTime == 0)
		{
			Debug.Log("Starting: " + ActionName);
			startTime = Time.time;
		}

		if (Time.time - startTime > workDuration) 
		{
			Debug.Log("Finished: " + ActionName);
			
			inv.breadLevel -= 5;
            marketInventory.breadLevel += 5;

			completed = true;
		}
		return true;
	}
	
}
