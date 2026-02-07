using UnityEngine;


//THis is for a Hauler agent.  Logistics only, no harvesting or producing.
public class CollectWheat : GoapAction {

	bool completed = false;
	float startTime = 0;
	public float workDuration = 2; // seconds
	public Inventory wheatField;
	Worker worker;
	
	public CollectWheat () 
	{
		addPrecondition ("hasWheatStock", false); 
		// After delivery, agent no longer has wheat
		// addEffect ("fieldHasWheat", false);

		// World can now produce / has flour stock
        addEffect("hasCollectedWheatFromField", true);
	}
    void Awake()
    {
        ActionName = "Collect Wheat";
    }

    public override void reset ()
	{
		completed = false;
		startTime = 0;
		target = null;
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

        target = GameObject.FindGameObjectWithTag("WheatField");
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
			Backpack agentInv = agent.GetComponent<Backpack>();

			Debug.Log("Finished: " + ActionName);
			agentInv.wheatLevel += 5;
			wheatField.wheatLevel -= 5;
			completed = true;
		}
		return true;
	}
}
