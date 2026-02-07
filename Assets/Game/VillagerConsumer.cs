using UnityEngine;

/// <summary>
/// Attach this to any Villager GameObject
/// 
/// Once basic consumption works, try:

///Increase villagers

///Add 3–5 VillagerConsumer objects

///Lower eatInterval

///Try 2f
///
///Lower baking output
///
///Bread +1, Flour -2 (already done)
///
///You should see:
///
///Baker can’t keep up
///
///Flour backs up
///
///Miller works constantly
///
///Bread oscillates
///
///That’s emergent behavior — the good kind.
/// 
/// </summary>
public class VillagerConsumer : MonoBehaviour
{
    public Inventory stockpile;

    public float eatInterval = 5f; // seconds
    public int breadPerMeal = 1;

    float nextEatTime;

    void Update()
    {
        if (Time.time < nextEatTime)
            return;

        if (stockpile == null)
        {
            Debug.LogError("VillagerConsumer: Stockpile not assigned");
            return;
        }

        if (stockpile.breadLevel > 0)
        {
            stockpile.breadLevel -= breadPerMeal;
            Debug.Log("Villager ate bread. Remaining: " + stockpile.breadLevel);
        }

        nextEatTime = Time.time + eatInterval;
    }
}
