using UnityEngine;

public class BakeBread : GoapAction
{
    bool completed = false;
    float startTime = 0f;
    public float workDuration = 2f; // seconds

    Worker worker;

    public BakeBread()
    {
        // Planner-facing logic
        addPrecondition("hasFlour", true);
        addPrecondition("hasBread", false);
        addEffect("hasBread", true);
    }
    void Awake()
    {
        ActionName = "Bake Bread";
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

        target = GameObject.FindGameObjectWithTag("Bakery");
        return target != null;
    }

    public override bool perform(GameObject agent)
    {
        // Abort if danger appears
        if (isInterrupted() || worker.GetNeedsToHide())
            return false;

        Backpack inv = agent.GetComponent<Backpack>();
        if (inv.flourLevel < 2)
            return false;
        if (startTime == 0f)
        {
            Debug.Log("Starting: " + ActionName);
            startTime = Time.time;
        }

        if (Time.time - startTime > workDuration)
        {
            Debug.Log("Finished: " + ActionName);
            
            inv.flourLevel -= 2;
            inv.breadLevel += 1;

            completed = true;
        }

        return true;
    }
}