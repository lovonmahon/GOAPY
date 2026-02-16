using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class IronOreMiner : Worker
{
    //Iron Ore miner mines iron ore for the making of tools and weapons
    void Awake()
    {
        faction = Faction.VILLAGER;
    }

    public override HashSet<KeyValuePair<string,object>> CreateGoalState ()
    {
    	HashSet<KeyValuePair<string,object>> goal = new HashSet<KeyValuePair<string,object>> ();
    
        // Safety must override everything
        if (GetNeedsToHide())
        {
            goal.Add(new KeyValuePair<string, object>("avoidEnemy", true));
            return goal;
        }

        goal.Add(new KeyValuePair<string, object>("hasIronOreInStockpile", true ));
    	return goal;
    }
}
