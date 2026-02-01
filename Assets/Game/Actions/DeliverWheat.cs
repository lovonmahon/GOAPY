using UnityEngine;

public class DeliverWheat : GoapAction {

	bool completed = false;
	float startTime = 0;
	public float workDuration = 2; // seconds
	public Inventory windmillInv;
	Worker worker;
	
	public DeliverWheat () 
	{
		addPrecondition ("hasWheat", true); 
		// After delivery, agent no longer has wheat
		addEffect ("hasWheat", false);

		// World can now produce / has flour stock
        addEffect("hasFlourStock", true);
	}
    void Awake()
    {
        ActionName = "Deliver Wheat";
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

        target = GameObject.FindGameObjectWithTag("Windmill");
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
			agentInv.wheatLevel -= 5;
			windmillInv.wheatLevel += 5;
			completed = true;
		}
		return true;
	}
}
