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

		// World can now produce flour since wheat has been dropped off
	}
    void Awake()
    {
        ActionName = "Deliver Wheat To Mill";
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
        if (target == null) return false;

        windmillInv = target.GetComponent<Inventory>();
        return windmillInv != null;
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
			Backpack inv = agent.GetComponent<Backpack>();

            Debug.Log("Finished: " + ActionName);
            inv.wheatLevel -= 5;
            windmillInv.wheatLevel += 5;
		}
		return true;
	}
}
