using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveObject : MonoBehaviour {
	public int SaveID;
	public bool isRuntimeObject = false;
	public enum SaveableType : byte {Transform,Player,Useable,Grenade,NPC,Destructable,SearchableStatic,
							SearchableDestructable,Door,ForceBridge,Switch,FuncWall,TeleDest,
							LBranch,LRelay,LSpawner,InteractablePanel,ElevatorPanel,Keypad,PuzzleGrid,
							PuzzleWire,TCounter,TGravity,MChanger,RadTrig,GravPad,TransformParentless,
							ChargeStation,Light,LTimer};
	public SaveableType saveType = SaveableType.Transform;
	[HideInInspector]
	public string saveableType;
	public int levelParentID = -1;
	[HideInInspector]
	public bool initialized = false;
	public bool instantiated = false; // True when this object has been instantiated at runtime
	public int constLookupTable = 0; // The table to check. 0 = useableItems, 1 = npcPrefabs
	public int constLookupIndex = -1; // Index into the Const lookup table for referencing during instantiation.

	public void Start () {
		if (initialized) return;
		SaveID = gameObject.GetInstanceID();
		isRuntimeObject = true;  // Lets us know if this object is indeed not the prefab but rather an instance of a prefab
		switch (saveType) {
			case SaveableType.Player: saveableType = "Player"; break;
			case SaveableType.Useable: saveableType = "Useable"; break;
			case SaveableType.Grenade: saveableType = "Grenade"; break; //live only
			case SaveableType.NPC: saveableType = "NPC"; break;
			case SaveableType.Destructable: saveableType = "Destructable"; break;
			case SaveableType.SearchableStatic: saveableType = "SearchableStatic"; break;
			case SaveableType.SearchableDestructable: saveableType = "SearchableDestructable"; break;
			case SaveableType.Door: saveableType = "Door"; break;
			case SaveableType.ForceBridge: saveableType = "ForceBridge"; break;
			case SaveableType.Switch: saveableType = "Switch"; break;
			case SaveableType.FuncWall: saveableType = "FuncWall"; break;
			case SaveableType.TeleDest: saveableType = "TeleDest"; break;
			case SaveableType.LBranch: saveableType = "LBranch"; break;
			case SaveableType.LRelay: saveableType = "LRelay"; break;
			case SaveableType.LSpawner: saveableType = "LSpawner"; break;
			case SaveableType.InteractablePanel: saveableType = "InteractablePanel"; break;
			case SaveableType.ElevatorPanel: saveableType = "ElevatorPanel"; break;
			case SaveableType.Keypad: saveableType = "Keypad"; break;
			case SaveableType.PuzzleGrid: saveableType = "PuzzleGrid"; break;
			case SaveableType.PuzzleWire: saveableType = "PuzzleWire"; break;
			case SaveableType.TCounter: saveableType = "TCounter"; break;
			case SaveableType.TGravity: saveableType = "TGravity"; break;
			case SaveableType.MChanger: saveableType = "MChanger"; break;
			case SaveableType.RadTrig: saveableType = "RadTrig"; break;
			case SaveableType.GravPad: saveableType = "GravPad"; break;
			case SaveableType.TransformParentless: saveableType = "TransformParentless"; break;
			case SaveableType.ChargeStation: saveableType = "ChargeStation"; break;
			case SaveableType.Light: saveableType = "Light"; break;
			case SaveableType.LTimer: saveableType = "LTimer"; break;
			default: saveableType = "Transform"; break;
		}
		initialized = true;
	}
}
