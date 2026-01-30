using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Stack-based Finite State Machine
/// 
/// Improvements over tutorial version:
/// - Prevents empty stack crashes
/// - Allows hard reset
/// - Adds safety checks
/// 
/// Designed for GOAP-style AI.
/// </summary>
public class FSM {

    // Stack of states
    private Stack<FSMState> stateStack = new Stack<FSMState>();

    // State delegate
    public delegate void FSMState(FSM fsm, GameObject gameObject);

    /// <summary>
    /// Update current state.
    /// Called once per frame.
    /// </summary>
    public void Update(GameObject gameObject) {

        // Safety: Do nothing if empty
        if (stateStack.Count == 0)
            return;

        FSMState state = stateStack.Peek();

        // Safety: Avoid null invoke
        if (state != null) {
            state(this, gameObject);
        }
    }

    /// <summary>
    /// Push new state on top.
    /// </summary>
    public void pushState(FSMState state) {

        if (state == null) {
            Debug.LogWarning("FSM: Tried to push null state.");
            return;
        }

        stateStack.Push(state);
    }

    /// <summary>
    /// Pop current state.
    /// </summary>
    public void popState() {

        if (stateStack.Count == 0) {
            Debug.LogWarning("FSM: Tried to pop empty stack.");
            return;
        }

        stateStack.Pop();
    }

    /// <summary>
    /// HARD RESET:
    /// Clears all states.
    /// Used for interruptions/replanning.
    /// </summary>
    public void clearStack() {
        stateStack.Clear();
    }

    /// <summary>
    /// Debug helper (optional).
    /// Shows how many states are active.
    /// </summary>
    public int GetStateCount() {
        return stateStack.Count;
    }
}
