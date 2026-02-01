using UnityEngine;

public class PickupFlour : GoapAction
{
    bool completed = false;
    float startTime = 0f;
    public float workDuration = 2f; // seconds

    public Inventory windmill;
    Worker worker;

    public PickupFlour()
    {
        // Planner-facing logic
        addPrecondition("hasFlour", false);// agent doesn not have flours but...
		addPrecondition("hasFlourStock", true); // ...the windmill has some.  Go pick some up

        addEffect("hasFlour", true);
		addEffect("hasFlourStock", false); // stock reduced / may be empty
    }
    void Awake()
    {
        ActionName = "Pickup Flour";
    }

    public override void reset()
    {
        completed = false;
        startTime = 0f;
        target = null;
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

        target = GameObject.FindGameObjectWithTag("Windmill");
        return target != null;
    }

    public override bool perform(GameObject agent)
    {
        // Abort if danger appears
        if (isInterrupted() || worker.GetNeedsToHide())
            return false;

        if (startTime == 0f)
        {
            Debug.Log("Starting: " + ActionName);
            startTime = Time.time;
        }

        if (Time.time - startTime > workDuration)
        {
            Debug.Log("Finished: " + ActionName);

            Backpack inv = agent.GetComponent<Backpack>();
            inv.flourLevel += 5;
            windmill.flourLevel -= 5;

            completed = true;
        }

        return true;
    }
}