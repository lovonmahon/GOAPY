using UnityEngine;

public class RestAction : GoapAction
{
    bool completed = false;
    Worker worker;

    public RestAction()
    {
        // Only rest when unsafe due to fatigue or injury
        addPrecondition("isSafe", false);
        addPrecondition("lowStamina", true);

        // After resting, the agent is safe again
        addEffect("isSafe", true);
        addEffect("lowStamina", false);

        name = "Rest";
    }

    public override void reset()
    {
        completed = false;
    }

    public override bool isDone()
    {
        return completed;
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        worker = agent.GetComponent<Worker>();
        return worker != null;
    }

    public override bool perform(GameObject agent)
    {
        // Abort if danger spikes again
        if (isInterrupted() || worker.GetNeedsToHide())
            return false;

        // Rest system runs elsewhere (regen, stamina recovery, etc.)
        // When recovery finishes, world state flips

        if (!worker.GetNeedsToHide())
        {
            completed = true;
            return true;
        }

        return true;
    }
}
