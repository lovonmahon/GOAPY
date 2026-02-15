using UnityEngine;
public class EnemySensor : MonoBehaviour
{
    public float detectionRadius = 15f;
    public LayerMask unitMask;

    private Worker self;
    private Worker currentEnemy;

    void Awake()
    {
        self = GetComponent<Worker>();
    }

    public void Scan()
    {
        currentEnemy = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, unitMask);

        foreach (var hit in hits)
        {
            Worker other = hit.GetComponent<Worker>();
            if (other == null || other == self)
                continue;

            if (IsEnemy(other))
            {
                currentEnemy = other;
                return;
            }
        }
    }

    private bool IsEnemy(Worker other)
    {
        return other.GetFaction() != self.GetFaction();
    }

    public Worker GetCurrentEnemy()
    {
        return currentEnemy;
    }
}
