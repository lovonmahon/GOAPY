using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// GOAP Agent Controller
/// 
/// Responsibilities:
/// - Maintain FSM (Idle / Move / Perform)
/// - Ask planner for plans
/// - Execute plans
/// - Handle interruptions safely
/// 
/// Design Rule:
/// We NEVER try to resume actions.
/// We ALWAYS replan from world state.
/// This keeps the AI stable and predictable.
/// 
/// 
/// 
/// REMEMBER: Aborting is handled in GoapAgent.
/// REMEMBER: Replanning is also handled in GoapAgent
/// 
/// Actions never “handle” the interrupt — they just trigger it
/// 
/// GoapAgent -> What it does: Receives failure signal, Aborts current plan and Forces replanning
/// </summary>
public sealed class GoapAgent : MonoBehaviour {

    // Finite State Machine
    private FSM stateMachine;

    // FSM States
    private FSM.FSMState idleState;           // Planning
    private FSM.FSMState moveToState;         // Movement
    private FSM.FSMState performActionState;  // Action execution

    // All actions available on this agent
    private HashSet<GoapAction> availableActions;

    // Current action plan
    private Queue<GoapAction> currentActions;

    // Supplies world state and goals
    private IGoap dataProvider;

    // Planner
    private GoapPlanner planner;

    // ------------------------------------------------------------
    // Unity
    // ------------------------------------------------------------

    void Start () {

        // Core systems
        stateMachine = new FSM();
        availableActions = new HashSet<GoapAction>();
        currentActions = new Queue<GoapAction>();
        planner = new GoapPlanner();

        // Find IGoap provider
        FindDataProvider();

        // Build FSM
        CreateIdleState();
        CreateMoveToState();
        CreatePerformActionState();

        // Start in planning state
        stateMachine.pushState(idleState);

        // Load all actions on this agent
        LoadActions();
    }

    void Update () 
    {
        stateMachine.Update(this.gameObject);
    }

    // ------------------------------------------------------------
    // Action Management
    // ------------------------------------------------------------

    public void AddAction(GoapAction a) {
        availableActions.Add(a);
    }

    public GoapAction GetAction(Type action) {

        foreach (GoapAction g in availableActions) {

            if (g.GetType().Equals(action))
                return g;
        }

        return null;
    }

    public void RemoveAction(GoapAction action) {
        availableActions.Remove(action);
    }

    private bool HasActionPlan() {
        return currentActions.Count > 0;
    }

    // ------------------------------------------------------------
    // FSM States
    // ------------------------------------------------------------

    /// <summary>
    /// IDLE = PLANNING STATE
    /// 
    /// Reads world + goal and generates a new plan.
    /// </summary>
    private void CreateIdleState() {

        idleState = (fsm, gameObj) => {

            // Get world state
            HashSet<KeyValuePair<string,object>> worldState =
                dataProvider.GetWorldState();

            // Get goal
            HashSet<KeyValuePair<string,object>> goal =
                dataProvider.CreateGoalState();

            // Ask planner
            Queue<GoapAction> plan =
                planner.Plan(gameObject, availableActions, worldState, goal);

            if (plan != null) {

                // Valid plan
                currentActions = plan;

                dataProvider.PlanFound(goal, plan);

                // Switch to execution
                fsm.popState();
                fsm.pushState(performActionState);

            } else {

                // No plan possible
                Debug.Log("GOAP: Failed plan: " + goal);

                dataProvider.PlanFailed(goal);

                // Try again later
                fsm.popState();
                fsm.pushState(idleState);
            }
        };
    }

    /// <summary>
    /// MOVETO = MOVEMENT STATE
    /// 
    /// Moves agent to current action target.
    /// </summary>
    private void CreateMoveToState() {

        moveToState = (fsm, gameObj) => {

            GoapAction action = currentActions.Peek();

            // Critical error: plan is invalid
            if (action.requiresInRange() && action.target == null) {

                Debug.LogError(
                    "GOAP Error: Action requires target but has none."
                );

                // HARD RESET → REPLAN
                stateMachine.clearStack();
                stateMachine.pushState(idleState);

                return;
            }

            // Let provider move agent
            if (dataProvider.MoveAgent(action)) {

                // Done moving → back to Perform
                fsm.popState();
            }
        };
    }

    /// <summary>
    /// PERFORM = ACTION EXECUTION
    /// 
    /// Executes actions in sequence.
    /// </summary>
    private void CreatePerformActionState() {

        performActionState = (fsm, gameObj) => {

            // No actions left
            if (!HasActionPlan()) {

                Debug.Log("GOAP: Actions finished");

                fsm.popState();
                fsm.pushState(idleState);

                dataProvider.ActionsFinished();

                return;
            }

            GoapAction action = currentActions.Peek();

            // Remove completed action
            if (action.isDone()) {
                action.doReset();
                currentActions.Dequeue();
            }

            // Still actions?
            if (HasActionPlan()) {

                action = currentActions.Peek();

                bool inRange =
                    action.requiresInRange()
                    ? action.isInRange()
                    : true;

                if (inRange) {

                    // Perform
                    bool success = action.perform(gameObj);

                    if (!success) {

                        // Action failed → replan
                        stateMachine.clearStack();
                        stateMachine.pushState(idleState);

                        dataProvider.PlanAborted(action);
                    }

                } else {

                    // Need movement
                    fsm.pushState(moveToState);
                }

            } else {

                // Plan completed
                fsm.popState();
                fsm.pushState(idleState);

                dataProvider.ActionsFinished();
            }
        };
    }

    // ------------------------------------------------------------
    // Setup
    // ------------------------------------------------------------

    /// <summary>
    /// Finds component implementing IGoap.
    /// </summary>
    private void FindDataProvider() {

        foreach (Component comp in
            gameObject.GetComponents(typeof(Component))) {

            if (typeof(IGoap).IsAssignableFrom(comp.GetType())) {

                dataProvider = (IGoap)comp;
                return;
            }
        }
    }

    /// <summary>
    /// Loads all GoapAction components.
    /// </summary>
    private void LoadActions () {

        GoapAction[] actions =
            gameObject.GetComponents<GoapAction>();

        foreach (GoapAction a in actions) {
            availableActions.Add(a);
        }

        Debug.Log("GOAP: Found " + actions.Length + " actions");
    }

    // ------------------------------------------------------------
    // Interruption
    // ------------------------------------------------------------

    /// <summary>
    /// Forces immediate replanning.
    /// 
    /// Used when:
    /// - Enemy appears
    /// - Goal changes
    /// - Environment changes
    /// 
    /// Strategy:
    /// Clear → Reset → Replan
    /// </summary>
    public void InterruptAction() 
    {
        Debug.Log("GOAP: Interrupted → Replan");

        // Destroy plan
        currentActions.Clear();

        // Reset FSM completely
        stateMachine.clearStack();

        // Restart planning
        stateMachine.pushState(idleState);
    }

    // ------------------------------------------------------------

    public IGoap DataProvider() {
        return dataProvider;
    }
}
