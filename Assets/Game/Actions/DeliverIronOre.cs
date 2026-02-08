using UnityEngine;

public class DeliverIronOre : GoapAction {

	bool completed = false;
	float startTime = 0;
	public float workDuration = 2; // seconds
	Worker worker;
	public Inventory stockpile;
	
	public DeliverIronOre () 
	{
		// Planner-facing logic
        addPrecondition("hasIronOre", true);

        addEffect("hasIronOre", false);
        addEffect("hasIronOreInStockpile", true);
	}
    void Awake()
    {
        ActionName = "Deliver Iron Ore";
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

        stockpile = worker.stockpile;
        if (stockpile == null) return false;

        target = stockpile.gameObject;
        return stockpile != null;
	}
	
	public override bool perform (GameObject agent)
	{
		// Abort if danger appears
        if (isInterrupted() || worker.GetNeedsToHide())
		{
			return false;
		}
            
		Backpack inv = agent.GetComponent<Backpack>();
        if (inv.ironOreLevel < 1)
            return false;
		
		if (startTime == 0)
		{
			Debug.Log("Starting: " + ActionName);
			startTime = Time.time;
		}

		if (Time.time - startTime > workDuration) 
		{
			Debug.Log("Finished: " + ActionName);
			
			inv.ironOreLevel -= 1;
            stockpile.ironOreLevel += 1;

			completed = true;
		}
		return true;
	}
}
