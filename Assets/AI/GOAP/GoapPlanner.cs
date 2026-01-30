using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * GOAP Planner
 *
 * Responsibility:
 * ----------------
 * Given:
 *  - Current world state
 *  - Desired goal state
 *  - Available actions
 *
 * This class finds the cheapest sequence of actions
 * that transforms the world into the goal.
 *
 * IMPORTANT DESIGN:
 * ------------------
 * This planner is STATELESS.
 *
 * It does NOT:
 *  - Remember past plans
 *  - Resume old actions
 *  - Care about interruptions
 *
 * If something fails → replan from scratch.
 *
 * This avoids stuck AI and logic bugs.
 */
public class GoapPlanner
{
    /*
     * Creates a new plan.
     *
     * Returns:
     *  - Queue<GoapAction> if successful
     *  - null if no plan exists
     */
    public Queue<GoapAction> Plan(
        GameObject agent,
        HashSet<GoapAction> availableActions,
        HashSet<KeyValuePair<string, object>> worldState,
        HashSet<KeyValuePair<string, object>> goal)
    {
        // -----------------------------
        // SAFETY: Validate inputs
        // -----------------------------
        // Prevents silent crashes later.
        if (availableActions == null || worldState == null || goal == null)
        {
            Debug.LogError("GoapPlanner: Null argument passed to Plan().");
            return null;
        }

        // -----------------------------
        // RESET ALL ACTIONS
        // -----------------------------
        // Clears runtime state (timers, flags, targets, etc).
        // Without this, old plans leak into new ones.
        foreach (GoapAction action in availableActions)
        {
            action.doReset();
        }

        // -----------------------------
        // FILTER USABLE ACTIONS
        // -----------------------------
        // Only actions that can currently run
        // are considered in planning.
        HashSet<GoapAction> usableActions = new HashSet<GoapAction>();

        foreach (GoapAction action in availableActions)
        {
            if (action.checkProceduralPrecondition(agent))
            {
                usableActions.Add(action);
            }
        }

        // -----------------------------
        // BUILD PLANNING GRAPH
        // -----------------------------

        List<Node> leaves = new List<Node>();

        // Root node = current world
        Node start = new Node(
            parent: null,
            runningCost: 0,
            state: worldState,
            action: null
        );

        bool success = BuildGraph(start, leaves, usableActions, goal);

        if (!success)
        {
            Debug.Log("GoapPlanner: No plan found.");
            return null;
        }

        // -----------------------------
        // FIND CHEAPEST SOLUTION
        // -----------------------------

        Node cheapest = null;

        foreach (Node leaf in leaves)
        {
            if (cheapest == null || leaf.runningCost < cheapest.runningCost)
            {
                cheapest = leaf;
            }
        }

        // -----------------------------
        // RECONSTRUCT ACTION PATH
        // -----------------------------
        // Walk backwards from goal → start
        List<GoapAction> result = new List<GoapAction>();

        Node current = cheapest;

        while (current != null)
        {
            if (current.action != null)
            {
                // Insert at front (reverse traversal)
                result.Insert(0, current.action);
            }

            current = current.parent;
        }

        // -----------------------------
        // CONVERT TO QUEUE
        // -----------------------------
        Queue<GoapAction> queue = new Queue<GoapAction>();

        foreach (GoapAction action in result)
        {
            queue.Enqueue(action);
        }

        return queue;
    }


    /*
     * Recursively builds the planning graph.
     *
     * This is a depth-first search of
     * possible action combinations.
     */
    private bool BuildGraph(
        Node parent,
        List<Node> leaves,
        HashSet<GoapAction> usableActions,
        HashSet<KeyValuePair<string, object>> goal)
    {
        bool foundPath = false;

        foreach (GoapAction action in usableActions)
        {
            // Can this action run in this world state?
            if (InState(action.Preconditions, parent.state))
            {
                // Apply effects
                HashSet<KeyValuePair<string, object>> newState =
                    PopulateState(parent.state, action.Effects);

                Node node = new Node(
                    parent,
                    parent.runningCost + action.cost,
                    newState,
                    action
                );

                // Goal reached?
                if (InState(goal, newState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    // Continue searching deeper
                    HashSet<GoapAction> subset =
                        ActionSubset(usableActions, action);

                    bool found = BuildGraph(node, leaves, subset, goal);

                    if (found)
                        foundPath = true;
                }
            }
        }

        return foundPath;
    }


    /*
     * Creates a copy of actions excluding one.
     *
     * Prevents cycles like:
     * A → B → A → B → A ...
     */
    private HashSet<GoapAction> ActionSubset(
        HashSet<GoapAction> actions,
        GoapAction removeMe)
    {
        HashSet<GoapAction> subset = new HashSet<GoapAction>();

        foreach (GoapAction action in actions)
        {
            if (!action.Equals(removeMe))
            {
                subset.Add(action);
            }
        }

        return subset;
    }


    /*
     * Checks if all test conditions exist in state.
     *
     * Used for:
     *  - Preconditions
     *  - Goal checking
     */
    private bool InState(
        HashSet<KeyValuePair<string, object>> test,
        HashSet<KeyValuePair<string, object>> state)
    {
        foreach (KeyValuePair<string, object> condition in test)
        {
            if (!state.Contains(condition))
            {
                return false;
            }
        }

        return true;
    }


    /*
     * Applies action effects to a world state.
     *
     * Returns a NEW state.
     * Does NOT mutate the old one.
     */
    private HashSet<KeyValuePair<string, object>> PopulateState(
        HashSet<KeyValuePair<string, object>> currentState,
        HashSet<KeyValuePair<string, object>> stateChange)
    {
        // Copy base state
        HashSet<KeyValuePair<string, object>> newState =
            new HashSet<KeyValuePair<string, object>>(currentState);

        foreach (KeyValuePair<string, object> change in stateChange)
        {
            // Remove old value for this key
            newState.RemoveWhere(kvp =>
                kvp.Key.Equals(change.Key));

            // Add new value
            newState.Add(change);
        }

        return newState;
    }


    /*
     * Internal graph node.
     *
     * Used only during planning.
     */
    private class Node
    {
        public Node parent;
        public float runningCost;
        public HashSet<KeyValuePair<string, object>> state;
        public GoapAction action;

        public Node(
            Node parent,
            float runningCost,
            HashSet<KeyValuePair<string, object>> state,
            GoapAction action)
        {
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }
}
