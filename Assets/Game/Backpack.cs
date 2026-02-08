using UnityEngine.UI;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    // Flour
	private int m_flourLevel = 0;
	public int flourLevel
    {
        get => m_flourLevel;
        set => m_flourLevel = value < 0 ? 0 : value;
    }

	//Bread
	private int m_breadLevel = 1;
	public int breadLevel
	{
        get => m_breadLevel;
        set => m_breadLevel = value < 0 ? 0 : value;
    }

	//Wheat
	private int m_wheatLevel = 0;
	public int wheatLevel
	{
        get => m_wheatLevel;
        set => m_wheatLevel = value < 0 ? 0 : value;
    }

	//Logs
	private int m_logs = 0;
	public int logsLevel
	{
        get => m_logs;
        set => m_logs = value < 0 ? 0 : value;
    }

	private int m_ironOreLevel = 0;
	public int ironOreLevel
	{
		get => m_ironOreLevel;
		set => m_ironOreLevel = value < 0? 0: value;
	}

	// public int buildingSupplies = 0;

	// public int tools = 0;



	public int drawOffset = 0; //vertical offset on the screen. “How far down from the top should I draw this UI?”
	public string inventoryName = "Agent's Inventory";

	void OnGUI()
	{
		GUI.Box(new Rect(0, drawOffset, 150, 80), inventoryName);

		GUI.Label(new Rect(10, 20 + drawOffset, 100, 20), "Flour: " + flourLevel);
		GUI.Label(new Rect(10, 40 + drawOffset, 100, 20), "Bread: " + breadLevel);
		GUI.Label(new Rect(10, 60 + drawOffset, 100, 20), "Wheat: " + wheatLevel);
	}
}
