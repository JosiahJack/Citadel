using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

public class SaveObject : MonoBehaviour {
	public int SaveID;
	public bool isRuntimeObject = false;
	public enum SaveableType : byte {Transform,Player,Useable,Grenade,NPC,Destructable,SearchableStatic,
							SearchableDestructable,Door,ForceBridge,Switch,FuncWall,TeleDest,
							LBranch,LRelay,LSpawner,InteractablePanel,ElevatorPanel,Keypad,PuzzleGrid,
							PuzzleWire,TCounter,TGravity,MChanger,RadTrig,GravPad,TransformParentless,
							ChargeStation,Light,LTimer,Camera,DelayedSpawn};
	public SaveableType saveType = SaveableType.Transform;
	[HideInInspector]
	public string saveableType;
	public int levelParentID = -1;
	[HideInInspector]
	public bool initialized = false;
	public bool instantiated = false; // True when this object has been instantiated at runtime
	public int constLookupTable = 0; // The table to check. 0 = useableItems, 1 = npcPrefabs
	public int constLookupIndex = -1; // Index into the Const lookup table for referencing during instantiation.

	public void SetSaveID() {
		SaveID = gameObject.GetInstanceID();
	}

	public void Start() {
		if (initialized) return;

		SetSaveID();
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
			case SaveableType.Camera: saveableType = "Camera"; break;
			case SaveableType.DelayedSpawn: saveableType = "DelayedSpawn"; break;
			default: saveableType = "Transform"; break;
		}
		initialized = true;
	}

	// Generates a string of object data with the specific object type's info.
	public string Save(GameObject go) {
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		if (!this.initialized) this.Start();
		string stype = this.saveableType;
		s1.Append(stype);
		s1.Append(Utils.splitChar);
		s1.Append(this.SaveID.ToString());
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(this.instantiated)); // bool
		s1.Append(Utils.splitChar);
		s1.Append(this.constLookupTable.ToString());
		s1.Append(Utils.splitChar);
		s1.Append(this.constLookupIndex.ToString());
		s1.Append(Utils.splitChar);

		// bool.  Watch it next time buddy.  Yeesh, 2/28/22 was kind of scary
		// till I realized this was still just using ToString here.  All
		// saveables were turned off!!  Need to use util function, ahhhhh!
		s1.Append(Utils.BoolToString(go.activeSelf)); 
		s1.Append(Utils.splitChar);

		s1.Append(Utils.SaveTransform(go.transform));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRigidbody(go.GetComponent<Rigidbody>()));
		s1.Append(Utils.splitChar);
		s1.Append(this.levelParentID.ToString()); // int
		s1.Append(Utils.splitChar);
		switch (this.saveType) {
			case SaveableType.Useable:                s1.Append(UseableObjectUse.Save(go)); break;
			case SaveableType.Grenade:                s1.Append(GrenadeActivate.Save(go)); break;
			case SaveableType.NPC:                    s1.Append(Const.a.SaveNPCData(go)); break;
			case SaveableType.Destructable:           s1.Append(HealthManager.Save(go)); break;
			case SaveableType.SearchableStatic:       s1.Append(SearchableItem.Save(go)); break;
			case SaveableType.SearchableDestructable: s1.Append(SearchableItem.Save(go));
                                                      s1.Append(HealthManager.Save(go)); break;
			case SaveableType.Door:                   s1.Append(Door.Save(go)); break;
			case SaveableType.ForceBridge:            s1.Append(ForceBridge.Save(go)); break;
			case SaveableType.Switch:                 s1.Append(ButtonSwitch.Save(go)); break;
			case SaveableType.FuncWall:               s1.Append(FuncWall.Save(go)); break;
			case SaveableType.TeleDest:               s1.Append(TeleportTouch.Save(go)); break;
			case SaveableType.LBranch:                s1.Append(LogicBranch.Save(go)); break;
			case SaveableType.LRelay:                 s1.Append(LogicRelay.Save(go)); break;
			case SaveableType.LSpawner:               s1.Append(Const.a.SaveSpawnerData(go)); break;
			case SaveableType.InteractablePanel:      s1.Append(Const.a.SaveInteractablePanelData(go)); break;
			case SaveableType.ElevatorPanel:          s1.Append(Const.a.SaveElevatorPanelData(go)); break;
			case SaveableType.Keypad:                 s1.Append(Const.a.SaveKeypadData(go)); break;
			case SaveableType.PuzzleGrid:             s1.Append(Const.a.SavePuzzleGridData(go)); break;
			case SaveableType.PuzzleWire:             s1.Append(Const.a.SavePuzzleWireData(go)); break;
			case SaveableType.TCounter:               s1.Append(Const.a.SaveTCounterData(go)); break;
			case SaveableType.TGravity:               s1.Append(Const.a.SaveTGravityData(go)); break;
			case SaveableType.MChanger:               s1.Append(Const.a.SaveMChangerData(go)); break;
			case SaveableType.RadTrig:                s1.Append(Const.a.SaveTRadiationData(go)); break;
			case SaveableType.GravPad:                s1.Append(Const.a.SaveGravLiftPadTextureData(go)); break;
			case SaveableType.ChargeStation:          s1.Append(Const.a.SaveChargeStationData(go)); break;
			case SaveableType.Light:                  s1.Append(Const.a.SaveLightAnimationData(go)); break;
			case SaveableType.LTimer:                 LogicTimer lt = go.GetComponent<LogicTimer>();
													  if (lt != null) {
													  	  s1.Append(lt.Save()); 
													  } else {
														  Debug.Log("LogicTimer missing on savetype of LogicTimer! GameObject.name: " + go.name);
													  }
													  break;
			case SaveableType.Camera: s1.Append(Const.a.SaveCameraData(go)); break;
			case SaveableType.DelayedSpawn:
				DelayedSpawn ds = go.GetComponent<DelayedSpawn>();
				if (ds != null) {
					s1.Append(ds.Save());
				} else {
					Debug.Log("DelayedSpawn missing on savetype of DelayedSpawn! GameObject.name: " + go.name);
				}
				break;
		}
		return s1.ToString();
	}
}
