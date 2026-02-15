using UnityEngine;
using UnityEngine.AI;

//Make sure navmeshagent stopping distance = 0
//or else isInRange will return true prematurely
//And the agent will perform when not close to the target

public class MineIronOre : GoapAction
{
    [SerializeField] Animator m_animator;
    NavMeshAgent m_agent;
    bool completed = false;
    float startTime = 0f;
    public float workDuration = 2f; // seconds

    Worker worker;
    public Inventory ironMine;

    public MineIronOre()
    {
        // Planner-facing logic
        addPrecondition("hasIronOre", false);
        addPrecondition("hasIronOreInStockpile", false);
        
        addEffect("hasIronOre", true);
    }
    void Awake()
    {
        ActionName = "Mine Iron Ore";
        m_agent = GetComponent<NavMeshAgent>();
    }

    public override void reset()
    {
        completed = false;
        startTime = 0f;
        // setInRange(false);
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
        if( m_agent!= null) m_agent.speed = 0.45f;
        if (isInterrupted() || worker.GetNeedsToHide())
        {
            return false;
        }

        ironMine = worker.ironMine;
        target = ironMine.gameObject;
        return ironMine != null;
    }

    public override bool perform(GameObject agent)
    {
        // Abort if danger appears
        if (isInterrupted() || worker.GetNeedsToHide())
        {
            m_animator.ResetTrigger("Mine");
            return false;
        }
            
        if(!isInRange()) return true; //back out and reevaluate perform until in range to proceed with task
        Backpack inv = agent.GetComponent<Backpack>();

        if (startTime == 0f)
        {
            Debug.Log("Starting: " + ActionName);
            startTime = Time.time;

            if( m_animator!= null)
            {
                m_animator.SetTrigger("Mine");
            }
        }
        
        if (Time.time - startTime > workDuration)
        {
            Debug.Log("Finished: " + ActionName);
            
            inv.ironOreLevel += 1;

            completed = true;
        }

        return true;
    }
    
}