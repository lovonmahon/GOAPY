using UnityEngine;
using System.Collections.Generic;

/**
 * Base class for all GOAP actions.
 * Supports clean interruption and forced replanning.
 */
public abstract class GoapAction : MonoBehaviour {

    public string actionName = "No Name";

    protected HashSet<KeyValuePair<string,object>> preconditions;
    protected HashSet<KeyValuePair<string,object>> effects;

    protected bool inRange = false;

    /* Cost used by planner */
    public float cost = 1f;

    /* Target of the action */
    public GameObject target;

    /* Interruption flag */
    protected bool interrupted = false;


    public GoapAction() {
        preconditions = new HashSet<KeyValuePair<string, object>>();
        effects = new HashSet<KeyValuePair<string, object>>();
    }

    /* ============================================================
     * RESET / LIFECYCLE
     * ============================================================
     */

    // Called before every new planning run
    public virtual void doReset() {

        // Clear movement state
        inRange = false;
        target = null;

        // Clear interruption state
        interrupted = false;

        // Let derived action reset itself
        reset();
    }

    /**
     * Reset custom variables in derived actions
     */
    public abstract void reset();

    /**
     * Is this action finished?
     */
    public abstract bool isDone();

    /**
     * Check if this action can run in current world state
     */
    public abstract bool checkProceduralPrecondition(GameObject agent);

    /**
     * Perform action.
     * Return false = action failed → abort plan
     */
    public abstract bool perform(GameObject agent);

    /**
     * Does this action require being in range?
     */
    public abstract bool requiresInRange ();


    /* ============================================================
     * RANGE CONTROL
     * ============================================================
     */

    public bool isInRange () {
        return inRange;
    }
    
    public void setInRange(bool inRange) {
        this.inRange = inRange;
    }


    /* ============================================================
     * INTERRUPTION
     * ============================================================
     */

    /**
     * Force this action to fail and trigger replanning.
     */
    public virtual void interrupt() {

        // Mark as invalid
        interrupted = true;

        Debug.Log(actionName + " was interrupted.");
    }

    /**
     * Has this action been interrupted?
     */
    public bool isInterrupted() {
        return interrupted;
    }


    /* ============================================================
     * PRECONDITIONS / EFFECTS
     * ============================================================
     */

    public void addPrecondition(string key, object value) {
        preconditions.Add(new KeyValuePair<string, object>(key, value));
    }

    public void removePrecondition(string key) {

        KeyValuePair<string, object> remove = default;

        foreach (KeyValuePair<string, object> kvp in preconditions) {
            if (kvp.Key.Equals(key)) {
                remove = kvp;
                break;
            }
        }

        if (!remove.Equals(default(KeyValuePair<string,object>)))
            preconditions.Remove(remove);
    }

    public void addEffect(string key, object value) {
        effects.Add(new KeyValuePair<string, object>(key, value));
    }

    public void removeEffect(string key) {

        KeyValuePair<string, object> remove = default;

        foreach (KeyValuePair<string, object> kvp in effects) {
            if (kvp.Key.Equals(key)) {
                remove = kvp;
                break;
            }
        }

        if (!remove.Equals(default(KeyValuePair<string,object>)))
            effects.Remove(remove);
    }

    public HashSet<KeyValuePair<string, object>> Preconditions {
        get { return preconditions; }
    }

    public HashSet<KeyValuePair<string, object>> Effects {
        get { return effects; }
    }
}
