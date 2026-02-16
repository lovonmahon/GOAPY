using UnityEngine;

[System.Serializable]
public class Health : MonoBehaviour
{
    [SerializeField] int m_health = 100;
    [SerializeField] int m_maxHealth = 100;
    public int MaxHealth => m_maxHealth;
    public int AgentHealth
    {
        get => m_health;
        set => m_health = Mathf.Clamp(value, 0, m_maxHealth);
    }
    
}