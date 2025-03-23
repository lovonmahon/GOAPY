using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Plans what actions can be completed in order to fulfill a goal state.
 */
public class GoapPlanner
{

    /**
     * Plan what sequence of actions can fulfill the goal.
     * Returns null if a plan could not be found, or a list of the actions
     * that must be performed, in order, to fulfill the goal.
     */
    public Queue<GoapAction> plan(GameObject agent,
                                  HashSet<GoapAction> availableActions, 
                                  HashSet<KeyValuePair<string,object>> worldState, 
                                  HashSet<KeyValuePair<string,object>> goal,
                                  GoapAction lastInterruptedAction = null) // Add a parameter for interrupted action
    {
        // Check for null arguments to prevent runtime errors
        if (availableActions == null || worldState == null || goal == null) {
            Debug.LogError("Null argument passed to GoapPlanner.");
            return null;
        }

        // Reset the actions so we can start fresh with them
        foreach (GoapAction a in availableActions) {
            a.doReset();
        }

        // Check usable actions
        HashSet<GoapAction> usableActions = new HashSet<GoapAction>();
        foreach (GoapAction a in availableActions) {
            if (a.checkProceduralPrecondition(agent)) {
                usableActions.Add(a);
            }
        }

        // If there's an interrupted action, ensure it's still valid to continue
        if (lastInterruptedAction != null && !lastInterruptedAction.isInterrupted()) {
            if (lastInterruptedAction.checkProceduralPrecondition(agent)) {
                usableActions.Add(lastInterruptedAction); // Add it back if it's still valid
            } else {
                Debug.Log("The interrupted action is no longer valid.");
            }
        }

        // Proceed with the rest of the planning process
        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, worldState, null);
        bool success = buildGraph(start, leaves, usableActions, goal);

        if (!success) {
            Debug.Log("NO PLAN");
            return null;
        }

        // Find the cheapest leaf node (lowest running cost)
        Node cheapest = null;
        foreach (Node leaf in leaves) {
            if (cheapest == null || leaf.runningCost < cheapest.runningCost) {
                cheapest = leaf;
            }
        }

        // Traverse back through the tree of nodes to construct the final action sequence
        List<GoapAction> result = new List<GoapAction>();
        Node n = cheapest;
        while (n != null) {
            if (n.action != null) {
                result.Insert(0, n.action);
            }
            n = n.parent;
        }

        // Create the queue from the result
        Queue<GoapAction> queue = new Queue<GoapAction>();
        foreach (GoapAction a in result) {
            queue.Enqueue(a);
        }

        return queue;
    }

    /**
     * Returns true if at least one solution was found.
     * The possible paths are stored in the leaves list. Each leaf has a
     * 'runningCost' value where the lowest cost will be the best action
     * sequence.
     */
    private bool buildGraph(Node parent, List<Node> leaves, HashSet<GoapAction> usableActions, HashSet<KeyValuePair<string, object>> goal)
    {
        bool foundOne = false;

        // Go through each action available at this node and see if we can use it here
        foreach (GoapAction action in usableActions) {
            // If the parent state has the conditions for this action's preconditions, we can use it here
            if (inState(action.Preconditions, parent.state)) {

                // Apply the action's effects to the parent state
                HashSet<KeyValuePair<string,object>> currentState = populateState(parent.state, action.Effects);

                Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);

                if (inState(goal, currentState)) {
                    // We found a solution!
                    leaves.Add(node);
                    foundOne = true;
                } else {
                    // Not at a solution yet, so test all the remaining actions and branch out the tree
                    HashSet<GoapAction> subset = actionSubset(usableActions, action);
                    bool found = buildGraph(node, leaves, subset, goal);
                    if (found)
                        foundOne = true;
                }
            }
        }

        return foundOne;
    }

    /**
     * Create a subset of the actions excluding the removeMe one. Creates a new set.
     */
    private HashSet<GoapAction> actionSubset(HashSet<GoapAction> actions, GoapAction removeMe) {
        HashSet<GoapAction> subset = new HashSet<GoapAction>();
        foreach (GoapAction a in actions) {
            if (!a.Equals(removeMe))
                subset.Add(a);
        }
        return subset;
    }

    /**
     * Check that all items in 'test' are in 'state'. If just one does not match or is not there
     * then this returns false.
     */
    private bool inState(HashSet<KeyValuePair<string,object>> test, HashSet<KeyValuePair<string,object>> state) {
        bool allMatch = true;
        foreach (KeyValuePair<string,object> t in test) {
            bool match = false;
            foreach (KeyValuePair<string,object> s in state) {
                if (s.Equals(t)) {
                    match = true;
                    break;
                }
            }
            if (!match)
                allMatch = false;
        }
        return allMatch;
    }

    /**
     * Apply the stateChange to the currentState
     */
    private HashSet<KeyValuePair<string,object>> populateState(HashSet<KeyValuePair<string,object>> currentState, HashSet<KeyValuePair<string,object>> stateChange) {
        HashSet<KeyValuePair<string,object>> state = new HashSet<KeyValuePair<string,object>>();
        // Copy the KVPs over as new objects
        foreach (KeyValuePair<string,object> s in currentState) {
            state.Add(new KeyValuePair<string, object>(s.Key, s.Value));
        }

        foreach (KeyValuePair<string,object> change in stateChange) {
            // If the key exists in the current state, update the Value
            bool exists = false;

            foreach (KeyValuePair<string,object> s in state) {
                if (s.Equals(change)) {
                    exists = true;
                    break;
                }
            }

            if (exists) {
                state.RemoveWhere((KeyValuePair<string,object> kvp) => { return kvp.Key.Equals(change.Key); });
                KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, change.Value);
                state.Add(updated);
            } else {
                // If it does not exist in the current state, add it
                state.Add(new KeyValuePair<string, object>(change.Key, change.Value));
            }
        }
        return state;
    }

    /**
     * Used for building up the graph and holding the running costs of actions.
     */
    private class Node {
        public Node parent;
        public float runningCost;
        public HashSet<KeyValuePair<string,object>> state;
        public GoapAction action;

        public Node(Node parent, float runningCost, HashSet<KeyValuePair<string,object>> state, GoapAction action) {
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }
}
