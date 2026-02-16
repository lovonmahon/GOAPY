using UnityEngine;

///<summary>
/// 
/// AttackAction should not decide when the enemy is no longer a threat.
/// It should observe world facts that someone else updates.
/// 
/*
 * AttackAction
 * ------------------------------------------------------------
 * Purpose:
 *     Resolve danger by neutralizing a threat (fight instead of hide).
 *
 * GOAP Semantics:
 *     - This action does NOT own combat logic.
 *     - It does NOT decide damage, death, fear, or morale.
 *     - It simply observes world state and reports success or failure.
 *
 * World Model:
 *     - Danger is represented by the world fact: isSafe == false
 *     - Combat systems update danger via Worker.SetHide(true/false)
 *
 * Planner Contract:
 *     Preconditions:
 *         - enemyVisible == true
 *         - enemyWeaker == true
 *
 *     Effect:
 *         - isSafe == true
 *
 * Execution Rules:
 *     - perform() returns false if danger spikes (fear, low health, fatigue)
 *       → GoapAgent aborts and replans (usually to HideAction).
 *     - perform() returns true while combat is ongoing.
 *     - isDone() becomes true when the world becomes safe.
 *
 * Design Rule:
 *     Fight and Hide are alternative solutions to the SAME problem:
 *         "The world is unsafe."
 *
 *     The planner chooses between them based on world facts and cost.
 */
///</summary>

public class AttackAction : GoapAction
{
    bool completed = false;
    float startTime = 0f;

    Worker worker;
    void Awake()
    {
        ActionName = "Attack the enemy";
    }

    public AttackAction()
    {
        addPrecondition("enemyVisible", true);
        addPrecondition("healthyEnoughToAttack", true);

        addEffect("enemyDead", true);
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
        Debug.Log("AttackAction considered");

        worker = agent.GetComponent<Worker>();
        if(worker == null) return false;

        EnemySensor sensor = worker.GetComponent<EnemySensor>();
        if (sensor == null) return false;

        Worker enemy = sensor.GetCurrentEnemy();
        if (enemy == null || enemy.IsDead()) return false;

        target = enemy.gameObject;
        return true;
    }

    public override bool perform(GameObject agent)
    {
        // Abort immediately if danger spikes or fear triggers
        if (isInterrupted() || worker.GetNeedsToHide())
        {
            return false;
        }
        if (target == null) return false;

        Worker enemy = target.GetComponent<Worker>();
        if (enemy == null) return false;

        if (enemy.IsDead())
        {
            completed = true;
            return true;
        }
        enemy.TakeDamage(10);

        // Still attacking successfully
        return true;
    }
}
