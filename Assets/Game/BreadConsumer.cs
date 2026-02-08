using UnityEngine;

public class BreadConsumer : MonoBehaviour
{
    public Inventory stockpile;
    public float eatInterval = 5f;
    public int breadPerEat = 1;

    float nextEatTime;

    void Update()
    {
        if (Time.time < nextEatTime)
            return;

        nextEatTime = Time.time + eatInterval;

        if (stockpile.breadLevel >= breadPerEat)
        {
            stockpile.breadLevel -= breadPerEat;
            Debug.Log($"[Consume] Bread eaten. Remaining: {stockpile.breadLevel}");
        }
        else
        {
            Debug.Log("[Consume] No bread available!");
        }
    }
}
