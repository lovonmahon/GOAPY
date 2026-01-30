using UnityEngine;

public class InvestigateAction : GoapAction
{
    bool completed = false;
    Worker worker;

    public InvestigateAction()
    {
        //If I see the enemy → react immediately (attack or hide)
        //If I don’t see the enemy but feel unsafe → investigate

        addPrecondition("isSafe", false);
        addPrecondition("enemyVisible", false);

        // So: 
        // 1. Sight sees enemy → Attack or Hide
        // 2. Enemy disappears → Investigate
        // 3. Nothing found → Resume work

        name = "Investigate";
    }

    public override void reset()
    {
        completed = false;

        if (target != null)
            Destroy(target);

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

        Vector3 pos;
        if (!worker.TryGetLastKnownThreatPosition(out pos))
            return false;

        // Create a tiny runtime target at the last known position
        GameObject probe = new GameObject("InvestigatePoint");
        probe.transform.position = pos;

        target = probe;
        return true;
    }

    public override bool perform(GameObject agent)
    {
        // Abort if danger escalates
        if (isInterrupted() || worker.GetNeedsToHide())
            return false;

        // Investigation itself is instant;
        // sensors will update world state afterward
        completed = true;
        return true;
    }
}
