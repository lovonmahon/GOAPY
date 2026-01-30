using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiseReign;
using UnityEditor.Build.Reporting;


/// <summary>
/// While Cutting trees....
// EXAMPLE:
//Bear appears → hide = true
//World state: isSafe = false
//Planner includes HideAction
//Agent executes HideAction
//Bear leaves → hide = false
//World state: isSafe = true
//HideAction completes
//Agent replans to work
/// </summary>

public class HideAction : GoapAction {

	//[SerializeField] float timeBetweenAttack = 1.0f;
	   
	bool m_sawPlayer = false;
    Animator anim;
    Sight sight;

	void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        sight = gameObject.GetComponent<Sight>();        
    }
    
    public HideAction()
	{
		addPrecondition("isSafe", false);//This tells the planner: “Only consider HideAction when I am not safe.”
		addEffect ("isSafe", true); //"what the world is expected to look like after the action completes successfully"
    	name = "Find a place to hide";
	}

	void Update()
    {
        //
    }
    
    public override void reset() {
		target = null;
	}

	public override bool isDone() // Am I finished forever?
	{
	    // Done when no longer in danger
        return !GetComponent<Worker>().GetNeedsToHide();
	}


	public override bool requiresInRange()
	{
		return true;
	}

	public override bool checkProceduralPrecondition(GameObject agent)
	{
		if (GetComponent<Worker>().GetNeedsToHide())
        {
            target = GameObject.FindGameObjectWithTag("HidingSpot");
            return target != null;
        }
        return false;
	}

	/// <summary>
	/// 👉 Because perform() is called every frame until isDone() returns true.
	///Returning true does not mean “done”.
	/// It means “I’m still executing successfully.
	/// </summary>
	/// <param name="agent"></param>
	/// <returns></returns>

	public override bool perform(GameObject agent)  // Am I still valid this frame?
	{
		// Stay hiding until safe
        return true;
	}
}
