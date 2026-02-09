using UnityEngine;
using UnityEngine.AI;

public class DeliverIronOre : GoapAction {

	[SerializeField] Animator m_animator;
    bool completed = false;
	float startTime = 0;
	public float workDuration = 2; // seconds
	Worker worker;
	public Inventory stockpile;
    [SerializeField] NavMeshAgent m_navAgent;
	
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
        setInRange(false);
        m_navAgent.isStopped = false;
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

        if(!isInRange()) return true; //back out and reevaluate perform until in range to proceed with task
            
		Backpack inv = agent.GetComponent<Backpack>();
        if (inv.ironOreLevel < 1)
            return false;
		
		if (startTime == 0)
		{
            Debug.Log("Starting: " + ActionName);
			startTime = Time.time;
            m_navAgent.isStopped = true;
            if( m_animator!= null)
            {
                m_animator.SetTrigger("Dropoff");
            }
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
