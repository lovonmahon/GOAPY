using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour 
{

	public int wheatLevel = 5;
	public int flourLevel = 5;
	public int breadLevel = 0;
	public int logs = 0;
	public int logsToDeliver = 0;
	public int buildingSupplies = 0;
	public int tools = 0;
	public int lumber = 0;
	public int ironOre = 0;
	public int drawOffset = 120; //vertical offset on the screen. “How far down from the top should I draw this UI?”
	public string inventoryName = "Inventory";

	
	// void OnGUI()
	// {
	// 	GUI.Box(new Rect(0, 0 + drawOffset, 100, 100), "" + inventoryName);
	// 	GUI.Label(new Rect(10, 20 + drawOffset, 100, 20), "Flour: " + flourLevel);
	// 	GUI.Label(new Rect(10, 35 + drawOffset, 100, 20), "Bread: " + breadLevel);
	// }
}
