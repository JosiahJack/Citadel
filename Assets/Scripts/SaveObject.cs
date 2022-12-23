using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

public class SaveObject : MonoBehaviour {
	public int SaveID;
	public bool isRuntimeObject = false;
	public SaveableType saveType = SaveableType.Transform;
	public bool instantiated = false; // Should oject be instantiated on load?

	[HideInInspector] public string saveableType;
	[HideInInspector] public bool initialized = false;

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
			case SaveableType.SecurityCamera: saveableType = "SecurityCamera"; break;
			case SaveableType.Trigger: saveableType = "Trigger"; break;
			case SaveableType.Projectile: saveableType = "Projectile"; break;
			case SaveableType.NormalScreen: saveableType = "NormalScreen"; break;
			default: saveableType = "Transform"; break;
		}

		if (instantiated) {
			SaveObject[] sos = GetComponentsInChildren<SaveObject>();
			if (sos.Length > 0) {
				if (!(sos.Length == 1 && sos[0] == this)) {
					PrefabIdentifier prefID = GetComponent<PrefabIdentifier>();
					if (prefID != null) {
						if (prefID.constIndex != 467) { // se_corpse_eaten
							Debug.Log("BUG: SaveObject found on child of instantiated "
									+ "Saveable Object: "
									+ gameObject.name + ", child name: "
									+ sos[0].gameObject.name);
						}
					} else {
						Debug.Log("BUG: SaveObject found on child of instantiated "
								+ "Saveable Object: "
								+ gameObject.name + ", child name: "
								+ sos[0].gameObject.name);
					}
				}
			}
		}
		initialized = true;
	}

	// Generates a string of object data with the specific object type's info.
	public static string Save(GameObject go) {
		if (go == null) {
			Debug.Log("BUG: attempting to save for null GameObject");
			string line = "Transform" + Utils.splitChar + "0" + Utils.splitChar
						  + "0" + Utils.splitChar + "-1" + Utils.splitChar
						  + "-1" + Utils.splitChar + "1";
			line += Utils.SaveTransform(go.transform);
			line += Utils.splitChar;
			line += Utils.SaveRigidbody(go);
			line += Utils.splitChar + "0" + Utils.splitChar;
			return line;
		}

		SaveObject so = go.GetComponent<SaveObject>();
		if (so == null) {
			Debug.Log("BUG: SaveObject missing on saveable!  GameObject.name: "
					  + go.name);
			string line = "Transform" + Utils.splitChar + "0" + Utils.splitChar
						  + "0" + Utils.splitChar + "-1" + Utils.splitChar
						  + "-1" + Utils.splitChar + "1";
			line += Utils.SaveTransform(go.transform);
			line += Utils.splitChar;
			line += Utils.SaveRigidbody(go);
			line += Utils.splitChar + "0" + Utils.splitChar;
			return line;
		}

		if (!so.initialized) so.Start();
		PrefabIdentifier prefID = go.GetComponent<PrefabIdentifier>();
		if (prefID == null) Debug.Log("No PrefabIdentifier on " + go.name);
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		// Start Saving
		// --------------------------------------------------------------------
		s1.Append(so.saveableType); s1.Append(Utils.splitChar);					    // 0
		s1.Append(so.SaveID.ToString()); s1.Append(Utils.splitChar);				// 1
		s1.Append(Utils.BoolToString(so.instantiated)); s1.Append(Utils.splitChar); // 2
		s1.Append(Utils.BoolToString(go.activeSelf));  s1.Append(Utils.splitChar);  // 3
		s1.Append(Utils.SaveTransform(go.transform)); s1.Append(Utils.splitChar);	// 4,5,6,7,8,9,10,11,12,13
		s1.Append(Utils.SaveRigidbody(go)); s1.Append(Utils.splitChar);			    // 14,15,16,17
		int levelID = 1;
		GameObject par = go.transform.parent.gameObject;
		for (int i=0; i < 14; i++) {
			if (par == LevelManager.a.levelScripts[i].dynamicObjectsContainer) {
				levelID = i;
				break;
			}
		}

		s1.Append(levelID.ToString()); s1.Append(Utils.splitChar);					// 18
		if (prefID != null) {
			s1.Append(Utils.UintToString(prefID.constIndex));                       // 19
			s1.Append(Utils.splitChar);
		} else s1.Append("307" + Utils.splitChar);

		switch (so.saveType) {
			case SaveableType.Player:         s1.Append(PlayerReferenceManager.SavePlayerData(go,prefID)); break;
			case SaveableType.Useable:              s1.Append(UseableObjectUse.Save(go)); break;
			case SaveableType.Grenade:               s1.Append(GrenadeActivate.Save(go)); break;
			case SaveableType.NPC:                     s1.Append(HealthManager.Save(go,prefID)); s1.Append(Utils.splitChar);
													    s1.Append(AIController.Save(go,prefID)); s1.Append(Utils.splitChar); // Handles SearchableDestructable for corpse child
										       s1.Append(AIAnimationController.Save(go)); break;
			case SaveableType.Destructable:            s1.Append(HealthManager.Save(go,prefID)); break;
			case SaveableType.SearchableStatic:       s1.Append(SearchableItem.Save(go,prefID)); break;
			case SaveableType.SearchableDestructable: s1.Append(SearchableItem.Save(go,prefID)); s1.Append(Utils.splitChar);
                                                       s1.Append(HealthManager.Save(go,prefID)); break;
			case SaveableType.Door:                             s1.Append(Door.Save(go,prefID)); break;
			case SaveableType.ForceBridge:               s1.Append(ForceBridge.Save(go)); break;
			case SaveableType.Switch:                   s1.Append(ButtonSwitch.Save(go)); break;
			case SaveableType.FuncWall:                     s1.Append(FuncWall.Save(go)); s1.Append(Utils.splitChar);
													        s1.Append(TargetIO.Save(go)); break;
			case SaveableType.TeleDest:                s1.Append(TeleportTouch.Save(go)); break;
			case SaveableType.LBranch:                   s1.Append(LogicBranch.Save(go)); break;
			case SaveableType.LRelay:                     s1.Append(LogicRelay.Save(go)); break;
			case SaveableType.LSpawner:                 s1.Append(SpawnManager.Save(go)); break;
			case SaveableType.InteractablePanel:   s1.Append(InteractablePanel.Save(go)); break;
			case SaveableType.ElevatorPanel:          s1.Append(KeypadElevator.Save(go)); break;
			case SaveableType.Keypad:                  s1.Append(KeypadKeycode.Save(go)); break;
			case SaveableType.PuzzleGrid:           s1.Append(PuzzleGridPuzzle.Save(go)); break;
			case SaveableType.PuzzleWire:           s1.Append(PuzzleWirePuzzle.Save(go)); break;
			case SaveableType.TCounter:               s1.Append(TriggerCounter.Save(go)); break;
			case SaveableType.TGravity:                  s1.Append(GravityLift.Save(go)); break;
			case SaveableType.MChanger:              s1.Append(MaterialChanger.Save(go)); break;
			case SaveableType.RadTrig:                     s1.Append(Radiation.Save(go)); break;
			case SaveableType.GravPad:                s1.Append(TextureChanger.Save(go)); break;
			case SaveableType.ChargeStation:           s1.Append(ChargeStation.Save(go)); break;
			case SaveableType.Light:                  s1.Append(LightAnimation.Save(go)); break;
			case SaveableType.LTimer:                     s1.Append(LogicTimer.Save(go)); break;
			case SaveableType.Camera:                  s1.Append(BerserkEffect.Save(go)); s1.Append(Utils.splitChar);
													           s1.Append(Utils.SaveCamera(go)); break;
			case SaveableType.DelayedSpawn:             s1.Append(DelayedSpawn.Save(go)); break;
			case SaveableType.SecurityCamera:   s1.Append(SecurityCameraRotate.Save(go)); s1.Append(Utils.splitChar);
													   s1.Append(HealthManager.Save(go.transform.GetChild(0).gameObject,prefID)); s1.Append(Utils.splitChar);
													           s1.Append(Utils.SaveTransform(go.transform.GetChild(0))); break;
			case SaveableType.Trigger:                       s1.Append(Trigger.Save(go)); break;
			case SaveableType.Projectile:               s1.Append(DelayedSpawn.Save(go)); s1.Append(Utils.splitChar);
										      s1.Append(ProjectileEffectImpact.Save(go)); break;
			case SaveableType.NormalScreen:            s1.Append(HealthManager.Save(go,prefID)); break;
		}
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		SaveObject so = go.GetComponent<SaveObject>();

		// Start Loading
		// --------------------------------------------------------------------
		// saveableType; index++;     // 0
		// SaveID; index++;           // 1
		// instantiated; index++;     // 2
		if (index != 3) {
			Debug.Log("SaveObject.Load:: index was not equal to 3 at start");
			index = 3;
		}
		bool setToActive = Utils.GetBoolFromString(entries[index]); index++;
		// Set active state of GameObject in Hierarchy
		if (setToActive) {
			if (!go.activeSelf) go.SetActive(true); 
		} else {
			if (go.activeSelf) go.SetActive(false);
		}

		if (entries[0] != so.saveableType) { Debug.Log("Saveable type mismatch.  Save data has type " + entries[0] + " but object is " + so.saveableType); return index + 19; }

		// Set parent prior to setting localPosition, localRotation, localScale
		// so that the relative positioning is correct.
		int levelID = Utils.GetIntFromString(entries[18]);
		Transform curLevDynContainer = LevelManager.a.GetRequestedLevelDynamicContainer(levelID).transform;
		if (levelID >= 0 && levelID <= 13 && so.instantiated
			  && go.transform.parent != curLevDynContainer) {
			go.transform.SetParent(curLevDynContainer);
		}

		if (so.saveType == SaveableType.TransformParentless && go.CompareTag("Player")) Debug.Log("Loading player transform from " + go.transform.localPosition.ToString() + "...");
		index = Utils.LoadTransform(go.transform,ref entries,index);
		if (so.saveType == SaveableType.TransformParentless && go.CompareTag("Player")) Debug.Log("Loaded player transform to " + go.transform.localPosition.ToString());
		index = Utils.LoadRigidbody(go,ref entries,index);
		index++; // Already loaded index 18 for levelID.
		index++; // ALready loaded index 19 for prefab master index as that is
				 // how this object was instantiated prior to calling Load.
		if (index != 20) {
			Debug.Log("SaveObject.Load:: index was not equal to 20 prior to "
					  + "loading SaveableType data.");
			index = 20;
		}
		if (index >= entries.Length) return index;

		PrefabIdentifier prefID = go.GetComponent<PrefabIdentifier>();
		if (prefID == null) Debug.Log("No PrefabIdentifier on " + go.name);
		switch (so.saveType) {
			case SaveableType.Player:				  index = PlayerReferenceManager.LoadPlayerDataToPlayer(go,ref entries,index,prefID); break;
			case SaveableType.Useable:				  index =       UseableObjectUse.Load(go,ref entries,index); break;
			case SaveableType.Grenade:				  index =        GrenadeActivate.Load(go,ref entries,index); break;
			case SaveableType.NPC:					  index =          HealthManager.Load(go,ref entries,index,prefID);
													  index =           AIController.Load(go,ref entries,index,prefID); // Handles SearchableDestructable for corpse child
													  index =  AIAnimationController.Load(go,ref entries,index); break;
			case SaveableType.Destructable:			  index =          HealthManager.Load(go,ref entries,index,prefID); break;
			case SaveableType.SearchableStatic:		  index =         SearchableItem.Load(go,ref entries,index,prefID); break;
			case SaveableType.SearchableDestructable: index =         SearchableItem.Load(go,ref entries,index,prefID);
													  index =          HealthManager.Load(go,ref entries,index,prefID); break;
			case SaveableType.Door:                   index =                   Door.Load(go,ref entries,index,prefID); break;
			case SaveableType.ForceBridge:            index =            ForceBridge.Load(go,ref entries,index); break;
			case SaveableType.Switch:                 index =           ButtonSwitch.Load(go,ref entries,index); break;
			case SaveableType.FuncWall:               index =               FuncWall.Load(go,ref entries,index);
													  index =               TargetIO.Load(go,ref entries,index,true); break;
			case SaveableType.TeleDest:               index =          TeleportTouch.Load(go,ref entries,index); break;
			case SaveableType.LBranch:                index =            LogicBranch.Load(go,ref entries,index); break;
			case SaveableType.LRelay:                 index =             LogicRelay.Load(go,ref entries,index); break;
			case SaveableType.LSpawner:               index =           SpawnManager.Load(go,ref entries,index); break;
			case SaveableType.InteractablePanel:      index =      InteractablePanel.Load(go,ref entries,index); break;
			case SaveableType.ElevatorPanel:          index =         KeypadElevator.Load(go,ref entries,index); break;
			case SaveableType.Keypad:                 index =          KeypadKeycode.Load(go,ref entries,index); break;
			case SaveableType.PuzzleGrid:             index =       PuzzleGridPuzzle.Load(go,ref entries,index); break;
			case SaveableType.PuzzleWire:             index =       PuzzleWirePuzzle.Load(go,ref entries,index); break;
			case SaveableType.TCounter:               index =         TriggerCounter.Load(go,ref entries,index); break;
			case SaveableType.TGravity:               index =            GravityLift.Load(go,ref entries,index); break;
			case SaveableType.MChanger:               index =        MaterialChanger.Load(go,ref entries,index); break;
			case SaveableType.RadTrig:                index =              Radiation.Load(go,ref entries,index); break;
			case SaveableType.GravPad:                index =         TextureChanger.Load(go,ref entries,index); break;
			case SaveableType.ChargeStation:          index =          ChargeStation.Load(go,ref entries,index); break;
			case SaveableType.Light:                  index =         LightAnimation.Load(go,ref entries,index); break;
			case SaveableType.LTimer:                  index =            LogicTimer.Load(go,ref entries,index); break;
			case SaveableType.Camera:                 index =          BerserkEffect.Load(go,ref entries,index);
													  index =                  Utils.LoadCamera(go,ref entries,index); break;
			case SaveableType.DelayedSpawn:           index =           DelayedSpawn.Load(go,ref entries,index); break;
			case SaveableType.SecurityCamera:         index =   SecurityCameraRotate.Load(go,ref entries,index);
													  index =          HealthManager.Load(go.transform.GetChild(0).gameObject,ref entries,index,prefID);
													  index =                  Utils.LoadTransform(go.transform.GetChild(0),ref entries,index); break;
			case SaveableType.Trigger:                index =                Trigger.Load(go,ref entries,index); break;
			case SaveableType.Projectile:             index =           DelayedSpawn.Load(go,ref entries,index);
													  index = ProjectileEffectImpact.Load(go,ref entries,index); break;
			case SaveableType.NormalScreen:			  index =          HealthManager.Load(go,ref entries,index,prefID); break;

		}
		return index;
	}
}
