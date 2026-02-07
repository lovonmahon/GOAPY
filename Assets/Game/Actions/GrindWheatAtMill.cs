using UnityEngine;

public class GrindWheatAtMill : GoapAction
{
    bool completed = false;
    float startTime = 0f;
    public float workDuration = 3f;

    Inventory windmillInv;
    Worker worker;

    public GrindWheatAtMill()
    {
        // Planner-facing logic
        addPrecondition("hasWheatAtMill", true);
        addEffect("hasFlourAtMill", true);
    }

    void Awake()
    {
        ActionName = "Grind Wheat At Mill";
    }

    public override void reset()
    {
        completed = false;
        startTime = 0f;
        target = null;
    }

    public override bool isDone() => completed;
    public override bool requiresInRange() => true;

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        worker = agent.GetComponent<Worker>();

        target = GameObject.FindGameObjectWithTag("Windmill");
        if (target == null) return false;

        windmillInv = target.GetComponent<Inventory>();
        return windmillInv != null && windmillInv.wheatLevel > 0;
    }

    public override bool perform(GameObject agent)
    {
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

            // Consume wheat → produce flour
            windmillInv.wheatLevel = Mathf.Max(0, windmillInv.wheatLevel - 5);
            windmillInv.flourLevel += 5;

            completed = true;
        }

        return true;
    }
}
