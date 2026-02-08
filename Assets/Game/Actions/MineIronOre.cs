using UnityEngine;

public class MineIronOre : GoapAction
{
    bool completed = false;
    float startTime = 0f;
    public float workDuration = 2f; // seconds

    Worker worker;
    public Inventory ironMine;

    public MineIronOre()
    {
        // Planner-facing logic
        addPrecondition("hasIronOre", false);
        
        addEffect("hasIronOre", true);
    }
    void Awake()
    {
        ActionName = "Mine Iron Ore";
    }

    public override void reset()
    {
        completed = false;
        startTime = 0f;
    }

    public override bool isDone()
    {
        return completed;
    }

    public override bool requiresInRange()
    {
        return true;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        worker = agent.GetComponent<Worker>();

        ironMine = worker.ironMine;
        target = ironMine.gameObject;
        return ironMine != null;
    }

    public override bool perform(GameObject agent)
    {
        // Abort if danger appears
        if (isInterrupted() || worker.GetNeedsToHide())
            return false;

        Backpack inv = agent.GetComponent<Backpack>();
        // if (inv.ironOreLevel < 1)
        //     return false;
        if (startTime == 0f)
        {
            Debug.Log("Starting: " + ActionName);
            startTime = Time.time;
        }

        if (Time.time - startTime > workDuration)
        {
            Debug.Log("Finished: " + ActionName);
            
            inv.ironOreLevel += 1;
            // ironMine.ironOreLevel += 1;

            completed = true;
        }

        return true;
    }
}