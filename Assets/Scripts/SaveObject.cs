using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
//#if UNITY_EDITOR
//	using UnityEditor;
//#endif

public class SaveObject : MonoBehaviour {
	public int SaveID; // Manually set by Tests.cs in Editor.
	public SaveableType saveType = SaveableType.Transform;
	public bool instantiated = false; // Should oject be instantiated on load?
	public static string currentObjectInfo;

	[HideInInspector] public string saveableType;
	[HideInInspector] public bool initialized = false;

	public void Start() {
		if (initialized) return;

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
			case SaveableType.GravPad: saveableType = "GravPad"; break;
			case SaveableType.TransformParentless: saveableType = "TransformParentless"; break;
			case SaveableType.ChargeStation: saveableType = "ChargeStation"; break;
			case SaveableType.Light: saveableType = "Light"; break;
			case SaveableType.LTimer: saveableType = "LTimer"; break;
			case SaveableType.Camera: saveableType = "Camera"; break;
			case SaveableType.DelayedSpawn: saveableType = "DelayedSpawn"; break;
			case SaveableType.SecurityCamera: saveableType = "SecurityCamera"; break;
			case SaveableType.Trigger: saveableType = "Trigger"; break;
			case SaveableType.Projectile: saveableType = "Projectile"; break;
			case SaveableType.NormalScreen: saveableType = "NormalScreen"; break;
			case SaveableType.CyberSwitch: saveableType = "CyberSwitch"; break;
			case SaveableType.CyberItem: saveableType = "CyberItem"; break;
			default: saveableType = "Transform"; break;
		}
		
		initialized = true;
	}

	// Generates a string of object data with the specific object type's info.
	public static string Save(GameObject go) {
		SaveObject so = SaveLoad.GetPrefabSaveObject(go);
		if (so == null) return "";

		if (!so.initialized) so.Start();
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		// Start Saving
		// --------------------------------------------------------------------
		PrefabIdentifier prefID = SaveLoad.GetPrefabIdentifier(go,true);
		if (prefID != null) {
			s1.Append(Utils.UintToString(prefID.constIndex,"constIndex")); // 0
			s1.Append(Utils.splitChar);
		} else return "";

		s1.Append(Utils.SaveString(so.saveableType,"saveableType"));   // 1
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(so.SaveID,"SaveID"));              // 2
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(so.instantiated,"instantiated")); // 3
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(go.activeSelf,"go.activeSelf"));  // 4
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveTransform(go.transform)); // 5,6,7,8,9,10,11,12,
													  // 13,14 or if rectTransform 5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRigidbody(go));           // 15,16,17,18
		s1.Append(Utils.splitChar);

		int levelID = 1;
		bool isNPC = (so.saveType == SaveableType.NPC);
		if (so.instantiated) {
			if (LevelManager.a == null) levelID = 1;//CitadelTests.levelToOutputFrom;
			else levelID = LevelManager.a.GetInstantiateParent(go,isNPC,prefID);
		}

		s1.Append(Utils.UintToString(levelID,"levelID"));     // 19
		s1.Append(Utils.splitChar);

		switch (so.saveType) {
			case SaveableType.Player:         s1.Append(PlayerReferenceManager.SavePlayerData(go,prefID)); break;
			case SaveableType.Useable:              s1.Append(UseableObjectUse.Save(go)); break;
			case SaveableType.Grenade:               s1.Append(GrenadeActivate.Save(go)); break;
			case SaveableType.NPC:                     s1.Append(HealthManager.Save(go,prefID)); s1.Append(Utils.splitChar); // Saves TargetIO
													    s1.Append(AIController.Save(go,prefID)); s1.Append(Utils.splitChar); // Handles SearchableDestructable for corpse child
										       s1.Append(AIAnimationController.Save(go)); break;
			case SaveableType.Destructable:            s1.Append(HealthManager.Save(go,prefID)); break; // Saves TargetIO
			case SaveableType.SearchableStatic:       s1.Append(SearchableItem.Save(go,prefID)); break;
			case SaveableType.SearchableDestructable: s1.Append(SearchableItem.Save(go,prefID)); s1.Append(Utils.splitChar);
                                                       s1.Append(HealthManager.Save(go,prefID)); break; // Saves TargetIO
			case SaveableType.Door:                             s1.Append(Door.Save(go,prefID)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.ForceBridge:               s1.Append(ForceBridge.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.Switch:                   s1.Append(ButtonSwitch.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.FuncWall:                     s1.Append(FuncWall.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.TeleDest:                s1.Append(TeleportTouch.Save(go)); break;
			case SaveableType.LBranch:                   s1.Append(LogicBranch.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.LRelay:                     s1.Append(LogicRelay.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.LSpawner:                 s1.Append(SpawnManager.Save(go)); break;
			case SaveableType.InteractablePanel:   s1.Append(InteractablePanel.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.ElevatorPanel:          s1.Append(KeypadElevator.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.Keypad:                  s1.Append(KeypadKeycode.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.PuzzleGrid:           s1.Append(PuzzleGridPuzzle.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.PuzzleWire:           s1.Append(PuzzleWirePuzzle.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.TCounter:               s1.Append(TriggerCounter.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.TGravity:                  s1.Append(GravityLift.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.GravPad:                s1.Append(TextureChanger.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.ChargeStation:           s1.Append(ChargeStation.Save(go)); break;
			case SaveableType.Light:                  s1.Append(LightAnimation.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.LTimer:                     s1.Append(LogicTimer.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.Camera:                  s1.Append(BerserkEffect.Save(go)); s1.Append(Utils.splitChar);
													           s1.Append(Utils.SaveCamera(go)); break;
			case SaveableType.DelayedSpawn:             s1.Append(DelayedSpawn.Save(go)); break;
			case SaveableType.SecurityCamera:   s1.Append(SecurityCameraRotate.Save(go)); s1.Append(Utils.splitChar);
													   s1.Append(HealthManager.Save(go.transform.GetChild(0).gameObject,prefID)); s1.Append(Utils.splitChar); // Saves TargetIO
													           s1.Append(Utils.SaveTransform(go.transform.GetChild(0))); break;
			case SaveableType.Trigger:                       s1.Append(Trigger.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.Projectile:               s1.Append(DelayedSpawn.Save(go)); s1.Append(Utils.splitChar);
										      s1.Append(ProjectileEffectImpact.Save(go)); break;
			case SaveableType.NormalScreen:            s1.Append(HealthManager.Save(go,prefID)); break; // Saves TargetIO
			case SaveableType.CyberSwitch:               s1.Append(CyberSwitch.Save(go,prefID)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go,prefID)); break;
			case SaveableType.CyberItem:               s1.Append(HealthManager.Save(go.transform.GetChild(0).GetChild(0).gameObject,prefID)); break; // Saves TargetIO
		}
		return s1.ToString();
	}

	// Called after prefab has been instantiated.
	public static void Load(GameObject go, ref string[] entries, int lineNum,
							PrefabIdentifier prefID) {

		if (prefID == null) { 
			prefID = go.GetComponent<PrefabIdentifier>();
			if (prefID == null) { Debug.Log("BUG: Missing PrefabIdentifier on load object " + go.name); return; }
		}

		SaveObject so = SaveLoad.GetPrefabSaveObject(go);
		if (!so.initialized) so.Start();
		currentObjectInfo = go.name + " " + ParentChain(go) + " ("
							+ so.saveType.ToString() + ") Line:"
						    + lineNum.ToString();
		// Start Loading
		// --------------------------------------------------------------------
		int index = 0;
		int constIndex = Utils.GetIntFromString(entries[0],"constIndex"); // 0
		index++;
		string savTypeFromLoad = Utils.LoadString(entries[index],
												  "saveableType"); // 1
		if (savTypeFromLoad != so.saveableType) {
			Debug.Log("Saveable type mismatch.  Save data has type "
					  + savTypeFromLoad + " but object named " + go.name
					  + " is " + so.saveableType.ToString());
			return;
		}
		index++; // Incrementing from saveableType.
		index++; // SaveID;       2
		index++; // instantiated; 3
		bool setToActive = Utils.GetBoolFromString(entries[index], // 4
												   "go.activeSelf");
		go.SetActive(setToActive); // Set active state in Hierarchy
		index++; // Incrementing from go.activeSelf 4
		index = Utils.LoadTransform(go.transform,ref entries,index); // 5,6,7,8,9,10,11,12,13,14 or if rectTransform 5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26
		index = Utils.LoadRigidbody(go,ref entries,index);           // 15,16,17,18
		
		// Set parent prior to setting localPosition, localRotation, localScale
		// so that the relative positioning is correct.
		int levelID = Utils.GetIntFromString(entries[index],"levelID"); index++; // 19
		if (so.instantiated) {
			bool isNPC = (so.saveType == SaveableType.NPC);
			LevelManager.a.SetInstantiateParent(levelID,go,isNPC);
		}
				
		if (index != 20 && ! (go.transform is RectTransform rectTr)) {
			Debug.Log("SaveObject.Load:: index was not 20 prior to type load");
			index = 20;
		}

		switch (so.saveType) {
			case SaveableType.Player:				  index = PlayerReferenceManager.LoadPlayerDataToPlayer(go,ref entries,index,prefID); break;
			case SaveableType.Useable:				  index =       UseableObjectUse.Load(go,ref entries,index); break;
			case SaveableType.Grenade:				  index =        GrenadeActivate.Load(go,ref entries,index); break;
			case SaveableType.NPC:					  index =          HealthManager.Load(go,ref entries,index,prefID); // Loads TargetIO
													  index =           AIController.Load(go,ref entries,index,prefID); // Handles SearchableDestructable for corpse child
													  index =  AIAnimationController.Load(go,ref entries,index); break;
			case SaveableType.Destructable:			  index =          HealthManager.Load(go,ref entries,index,prefID); break; // Loads TargetIO
			case SaveableType.SearchableStatic:		  index =         SearchableItem.Load(go,ref entries,index,prefID); break;
			case SaveableType.SearchableDestructable: index =         SearchableItem.Load(go,ref entries,index,prefID);
													  index =          HealthManager.Load(go,ref entries,index,prefID); break; // Loads TargetIO
			case SaveableType.Door:                   index =                   Door.Load(go,ref entries,index,prefID);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.ForceBridge:            index =            ForceBridge.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.Switch:                 index =           ButtonSwitch.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.FuncWall:               index =               FuncWall.Load(go.transform.GetChild(0).gameObject,ref entries,index);
													  index =               TargetIO.Load(go.transform.GetChild(0).gameObject,ref entries,index,true,prefID); break;
			case SaveableType.TeleDest:               index =          TeleportTouch.Load(go,ref entries,index); break;
			case SaveableType.LBranch:                index =            LogicBranch.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.LRelay:                 index =             LogicRelay.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.LSpawner:               index =           SpawnManager.Load(go,ref entries,index); break;
			case SaveableType.InteractablePanel:      index =      InteractablePanel.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.ElevatorPanel:          index =         KeypadElevator.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.Keypad:                 index =          KeypadKeycode.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.PuzzleGrid:             index =       PuzzleGridPuzzle.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.PuzzleWire:             index =       PuzzleWirePuzzle.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.TCounter:               index =         TriggerCounter.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.TGravity:               index =            GravityLift.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.GravPad:                index =         TextureChanger.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.ChargeStation:          index =          ChargeStation.Load(go,ref entries,index); break;
			case SaveableType.Light:                  index =         LightAnimation.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.LTimer:                 index =             LogicTimer.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.Camera:                 index =          BerserkEffect.Load(go,ref entries,index);
													  index =                  Utils.LoadCamera(go,ref entries,index); break;
			case SaveableType.DelayedSpawn:           index =           DelayedSpawn.Load(go,ref entries,index); break;
			case SaveableType.SecurityCamera:         index =   SecurityCameraRotate.Load(go,ref entries,index); 
													  index =          HealthManager.Load(go.transform.GetChild(0).gameObject,ref entries,index,prefID); // Loads TargetIO
													  index =                  Utils.LoadTransform(go.transform.GetChild(0),ref entries,index); break;
			case SaveableType.Trigger:                index =                Trigger.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.Projectile:             index =           DelayedSpawn.Load(go,ref entries,index);
													  index = ProjectileEffectImpact.Load(go,ref entries,index); break;
			case SaveableType.NormalScreen:			  index =          HealthManager.Load(go,ref entries,index,prefID); break; // Loads TargetIO
			case SaveableType.CyberSwitch:			  index =            CyberSwitch.Load(go,ref entries,index,prefID);
													  index =               TargetIO.Load(go,ref entries,index,true,prefID); break;
			case SaveableType.CyberItem:			  index =          HealthManager.Load(go.transform.GetChild(0).GetChild(0).gameObject,ref entries,index,prefID); break; // Loads TargetIO
		}
	}

	public static string ParentChain(GameObject go) {
		if (go.transform.parent == null) return "none";

		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		Transform tr = go.transform;
		while(tr.parent != null) {
			s1.Insert(0,"->");
			s1.Insert(0,tr.parent.name);
			tr = tr.parent;
		}

		return s1.ToString();
	}
}
