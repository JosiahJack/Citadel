using System;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;

// Globally accessible utility functions for parsing values, generating save
// strings, converting enumerated and integer values, and other helpful things.
//
// Most of this is for saving.
// All save strings should follow this rule:
//
// Start with data; End with data.
//
// E.g. 0|1|1 and NOT these: |0|1|1| or 0|1|1|.
public class Utils {
	public static string splitChar = "|"; // Common delimiter, not a comma as
										  // text could contain commas.  Used
										  // for all savefile text.
	public static char splitCharChar = '|';

	public static CultureInfo en_US_Culture = new CultureInfo("en-US");

	private static bool getValparsed;
	private static int getValreadInt;
	private static float getValreadFloat;
	private static float readFloatx, readFloaty, readFloatz, readFloatw;
	private static Vector3 tempvec;
	private static Quaternion tempquat;

	public static object SafeIndex(ref object[] array, int index, int max,
                                   object failValue) {
		if (array.Length < 1) {
            Debug.Log("SafeIndex: Unexpected situation, array " + nameof(array)
                      + " was empty!  Using fallback value of "
                      + failValue.ToString());
            return failValue;
        }

		if (index < 0 || index > max || index > array.Length) {
            Debug.Log("SafeIndex: Unexpected situation, index "
                      + index.ToString() + " out of bounds [0,"
                      + array.Length.ToString() + "] or context max of "
                      + max.ToString() + ".  Set to fallback value of "
                      + failValue.ToString());
            return failValue;
        }

		return array[index]; // Safe to pass the index value into the array space.
	}

	public static void BlankBoolArray(ref bool[] array, bool value) {
		for (int i=0;i<array.Length;i++) {
			array[i] = value; // Reset the list
		}
	}

	public static void Activate(GameObject go) {
		if (go == null) return;

		if (!go.activeSelf) go.SetActive(true);
	}

	public static void Deactivate(GameObject go) {
		if (go == null) return;

		if (go.activeSelf) go.SetActive(false);
	}

	public static void EnableImage(Image img) {
		if (img == null) return;

		if (!img.enabled) img.enabled = true;
	}

	public static void DisableImage(Image img) {
		if (img == null) return;

		if (img.enabled) img.enabled = false;
	}

	public static void EnableMeshRenderer(MeshRenderer mr) {
		if (mr == null) return;

		if (!mr.enabled) mr.enabled = true;
	}

	public static void DisableMeshRenderer(MeshRenderer mr) {
		if (mr == null) return;

		if (mr.enabled) mr.enabled = false;
	}

	public static void EnableBoxCollider(BoxCollider col) {
		if (col == null) return;

		if (!col.enabled) col.enabled = true;
	}

	public static void DisableBoxCollider(BoxCollider col) {
		if (col == null) return;

		if (col.enabled) col.enabled = false;
	}

	public static void EnableMeshCollider(MeshCollider col) {
		if (col == null) return;

		if (!col.enabled) col.enabled = true;
	}

	public static void DisableMeshCollider(MeshCollider col) {
		if (col == null) return;

		if (col.enabled) col.enabled = false;
	}

	public static void EnableCapsuleCollider(CapsuleCollider col) {
		if (col == null) return;

		if (!col.enabled) col.enabled = true;
	}

	public static void DisableCapsuleCollider(CapsuleCollider col) {
		if (col == null) return;

		if (col.enabled) col.enabled = false;
	}

	public static void EnableSphereCollider(SphereCollider col) {
		if (col == null) return;

		if (!col.enabled) col.enabled = true;
	}

	public static void DisableSphereCollider(SphereCollider col) {
		if (col == null) return;

		if (col.enabled) col.enabled = false;
	}

	public static void EnableCollision(GameObject go) {
		BoxCollider bcol = go.GetComponent<BoxCollider>();
		EnableBoxCollider(bcol);
		SphereCollider scol = go.GetComponent<SphereCollider>();
		EnableSphereCollider(scol);
		CapsuleCollider ccol = go.GetComponent<CapsuleCollider>();
		EnableCapsuleCollider(ccol);
		MeshCollider mcol = go.GetComponent<MeshCollider>();
		EnableMeshCollider(mcol);
	}

	public static void DisableCollision(GameObject go) {
		BoxCollider bcol = go.GetComponent<BoxCollider>();
		DisableBoxCollider(bcol);
		SphereCollider scol = go.GetComponent<SphereCollider>();
		DisableSphereCollider(scol);
		CapsuleCollider ccol = go.GetComponent<CapsuleCollider>();
		DisableCapsuleCollider(ccol);
		MeshCollider mcol = go.GetComponent<MeshCollider>();
		DisableMeshCollider(mcol);
	}

	public static void EnableNavMeshObstacle(NavMeshObstacle nmo) {
		if (nmo == null) return;

		if (!nmo.enabled) nmo.enabled = true;
	}

	public static void DisableNavMeshObstacle(NavMeshObstacle nmo) {
		if (nmo == null) return;

		if (nmo.enabled) nmo.enabled = false;
	}

	public static void EnableCamera(Camera cam) {
		if (cam == null) return;

		if (!cam.enabled) cam.enabled = true;
	}

	public static void DisableCamera(Camera cam) {
		if (cam == null) return;

		if (cam.enabled) cam.enabled = false;
	}

	public static void EnableGrayscale(Grayscale gsc) {
		if (gsc == null) return;

		if (!gsc.enabled) gsc.enabled = true;
	}

	public static void DisableGrayscale(Grayscale gsc) {
		if (gsc == null) return;

		if (gsc.enabled) gsc.enabled = false;
	}

	public static void EnableLight(Light lit) {
		if (lit == null) return;

		if (!lit.enabled) lit.enabled = true;
	}

	public static void DisableLight(Light lit) {
		if (lit == null) return;

		if (lit.enabled) lit.enabled = false;
	}

	public static void AssignImageOverride(Image img, Sprite over) {
		if (img == null) return;
		if (over == null) return;

		if (img.overrideSprite != over) img.overrideSprite = over;
	}

	// Safely combines multiple strings into a path using the operating
	// system path delimiter or forces forward slashes for Resources.Load due
	// to all Unity internal asset paths using forward slashes per the docs.
	//
	// Protects against the use of slashes by mistake in passed parameters as
	// Path.Combine is too dumb to handle this properly on all systems.
    public static string SafePathCombine(string basePath,
										 params string[] additional) {
        int totalLength = 0;
        for (int i = 0; i < additional.Length; i++) {
            totalLength += additional[i].Split(pathSplitCharacters).Length;
        }

        string[] segments = new string[totalLength + 1];
        segments[0] = basePath;
        int index = 0;
        for (int i = 0; i < additional.Length; i++) {
            string[] split = additional[i].Split(pathSplitCharacters);
            for (int j = 0; j < split.Length; j++) {
                index++;
                segments[index] = split[j];
            }
        }

		string combinedPath = Path.Combine(segments);
		combinedPath = combinedPath.Replace("\\","/"); // Turns out / works on
													   // Windows so just use
													   // it for everything.

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
			Debug.Log("SafePathCombine result: " + combinedPath);
		}
        return combinedPath;
    }

	// From the Unity Documentation on Resources.Load:
	// Note: All asset names and paths in Unity use forward slashes, paths
	//       using backslashes will not work.
	// Using Utils.SafePathCombine to force / separator.
	// This wrapper needed to remove the file extension per the docs.
	// docs.unity3d.com/2019.4/Documentation/ScriptReference/Resources.Load.html
	public static string ResourcesPathCombine(string folderInResources,
											  string fileName) {
		string fname = Path.GetFileNameWithoutExtension(fileName);
		return SafePathCombine(folderInResources,fname);
	}

	public static void ConfirmExistsInStreamingAssetsMakeIfNot(string fileName) {
		if (string.IsNullOrWhiteSpace(fileName)) {
			UnityEngine.Debug.Log("fileName was null or whitespace passed to "
								  + "ConfirmExistsInStreamingAssetsMakeIfNot");
			return;
		}

		string strmAstPth = SafePathCombine(Application.streamingAssetsPath,
											fileName);

		if (File.Exists(strmAstPth)) {
			return; // Already exists, all good!
		}

		 // Recreate StreamingAssets if it doesn't exist.
        if (!Directory.Exists(Application.streamingAssetsPath)) {
			Directory.CreateDirectory(Application.streamingAssetsPath);
		}

		string rsrc = ResourcesPathCombine("StreamingAssetsRecovery",fileName);
        TextAsset resourcesFile = (TextAsset)Resources.Load(rsrc);
        if (resourcesFile != null) {
			// Recreate from Resources/StreamingAssetsRecovery/*
			File.WriteAllText(strmAstPth, resourcesFile.text, // new, contents
							  Encoding.ASCII);
			if (File.Exists(strmAstPth)) {
				UnityEngine.Debug.Log("File " + strmAstPth + " recreated");
			} else {
				UnityEngine.Debug.Log("File " + strmAstPth + " failed to be "
									  + "created by File.WriteAllText!");
			}
        } else {
			UnityEngine.Debug.Log("File " + strmAstPth + " not found in the "
								  + "Resources folder");
		}
	}

    static readonly char[] pathSplitCharacters = new char[] { '/', '\\' };

	// Check if particular bit is 1 (ON/TRUE) in binary format of given integer
	public static bool CheckFlags (int checkInt, int flag) {
        return ((checkInt & flag) != 0);
    }

	public static int AccessCardTypeToInt(AccessCardType acc) {
		switch (acc) {
			case AccessCardType.None:        return  0;
			case AccessCardType.Standard:    return  1;
			case AccessCardType.Medical:     return  2;
			case AccessCardType.Science:     return  3;
			case AccessCardType.Admin:       return  4;
			case AccessCardType.Group1:      return  5;
			case AccessCardType.Group2:      return  6;
			case AccessCardType.Group3:      return  7;
			case AccessCardType.Group4:      return  8;
			case AccessCardType.GroupA:      return  9;
			case AccessCardType.GroupB:      return 10;
			case AccessCardType.Storage:     return 11;
			case AccessCardType.Engineering: return 12;
			case AccessCardType.Maintenance: return 13;
			case AccessCardType.Security:    return 14;
			case AccessCardType.Per1:        return 15;
			case AccessCardType.Per2:        return 16;
			case AccessCardType.Per3:        return 17;
			case AccessCardType.Per4:        return 18;
			case AccessCardType.Per5:        return 19;
		}
		return 0;
	}

	public static AccessCardType IntToAccessCardType(int cardType) {
		switch (cardType) {
			case 0:  return AccessCardType.None;
			case 1:  return AccessCardType.Standard;
			case 2:  return AccessCardType.Medical;
			case 3:  return AccessCardType.Science;
			case 4:  return AccessCardType.Admin;
			case 5:  return AccessCardType.Group1;
			case 6:  return AccessCardType.Group2;
			case 7:  return AccessCardType.Group3;
			case 8:  return AccessCardType.Group4;
			case 9:  return AccessCardType.GroupA;
			case 10: return AccessCardType.GroupB;
			case 11: return AccessCardType.Storage;
			case 12: return AccessCardType.Engineering;
			case 13: return AccessCardType.Maintenance;
			case 14: return AccessCardType.Security;
			case 15: return AccessCardType.Per1;
			case 16: return AccessCardType.Per2;
			case 17: return AccessCardType.Per3;
			case 18: return AccessCardType.Per4;
			case 19: return AccessCardType.Per5;
		}
		return AccessCardType.None;
	}

	public static int ButtonTypeToInt(ButtonType bt) {
		switch (bt) { // Man what a load of
			case ButtonType.None:       return 0;
			case ButtonType.Generic:    return 1;
			case ButtonType.GeneralInv: return 2;
			case ButtonType.Patch:      return 3;
			case ButtonType.Grenade:    return 4;
			case ButtonType.Weapon:     return 5;
			case ButtonType.Search:     return 6;
			case ButtonType.PGrid:      return 7;
			case ButtonType.PWire:      return 8;
		}
		return 0;
	}

	public static ButtonType IntToButtonType(int state) {
		switch (state) {
			case 0: return ButtonType.None;
			case 1: return ButtonType.Generic;
			case 2: return ButtonType.GeneralInv;
			case 3: return ButtonType.Patch;
			case 4: return ButtonType.Grenade;
			case 5: return ButtonType.Weapon;
			case 6: return ButtonType.Search;
			case 7: return ButtonType.PGrid;
			case 8: return ButtonType.PWire;
		}
		return ButtonType.None;
	}

	public static int BodyStateToInt(BodyState bs) {
		switch (bs) { // Man what a load of
			case BodyState.Standing: return 0;
			case BodyState.Crouch: return 1;
			case BodyState.CrouchingDown: return 2;
			case BodyState.StandingUp: return 3;
			case BodyState.Prone: return 4;
			case BodyState.ProningDown: return 5;
			case BodyState.ProningUp: return 6;
		}
		return 0;
	}

	public static BodyState IntToBodyState(int state) {
		switch (state) {
			case 0: return BodyState.Standing;
			case 1: return BodyState.Crouch;
			case 2: return BodyState.CrouchingDown;
			case 3: return BodyState.StandingUp;
			case 4: return BodyState.Prone;
			case 5: return BodyState.ProningDown;
			case 6: return BodyState.ProningUp;
		}
		return BodyState.Standing;
	}

	public static string IntToString(int val) {
		return val.ToString();
	}

	public static string UintToString(int val) {
		if (val < 0) return "-1";
		return val.ToString();
	}

	public static bool GetBoolFromString(string val) {
		return val.Equals("1");
	}

	public static string BoolToString(bool inputValue) {
		if (inputValue) return "1";
		return "0";
	}

	public static int GetIntFromString(string val) {
		if (val == "0") return 0;

		getValparsed = Int32.TryParse(val, NumberStyles.Integer, en_US_Culture,
                                      out getValreadInt);
		if (!getValparsed) {
			UnityEngine.Debug.Log("BUG: Could not parse int from: " + val
                                  + ", returning 0 as a fallback");
			return 0;
		}

		return getValreadInt;
	}

	public static float GetFloatFromString(string val) {
		getValparsed = Single.TryParse(val, NumberStyles.Float, en_US_Culture,
                                       out getValreadFloat);
		if (!getValparsed) {
			UnityEngine.Debug.Log("BUG: Could not parse float from: " + val
                                  + ", returning 0.0 as fallback");
			return 0.0f;
		}
		return getValreadFloat;
	}

    // Output with 4 integer places and 5 mantissa, culture invariant to
    // guarantee . is used as a separator rather than , for all regions.
	public static string FloatToString(float val) {
		return val.ToString("0000.00000", CultureInfo.InvariantCulture); 
	}

	public static string Vector3ToString(Vector3 vec) {
		if (vec == null) return DTypeWordToSaveString("fff");

		string line = System.String.Empty;
        line =              FloatToString(vec.x);
		line += splitChar + FloatToString(vec.y);
		line += splitChar + FloatToString(vec.z);
		return line;
	}

	// This is mostly just here for me to make the save strings commonized when
	// dummying them out in the case of an error.  Robustness above all else.
	// If I have some preference on how to handle a particular variable, shove
	// it here to make sure all types are consistently saved same as the other
	// to strings here.
	public static string DTypeWordToSaveString(string word) {
		char[] characters = word.ToLower().ToCharArray(); // In case I forget when making my debug words.
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		for (int i=0;i<characters.Length;i++) {
			if (characters[i] == 'b') s1.Append(BoolToString(false));
			else if (characters[i] == 'u') s1.Append(UintToString(-99999));
			else if (characters[i] == 'i') s1.Append(IntToString(0));
			else if (characters[i] == 'f') s1.Append(FloatToString(0.0f));
			else if (characters[i] == 't') s1.Append(FloatToString(0.0f));
			else if (characters[i] == '1') s1.Append("1"); // Force it true
			else s1.Append("-");
			if (i != (characters.Length - 1)) s1.Append(splitChar);
		}
		return s1.ToString();
	}

	public static int NPCTypeToInt(NPCType typ) {
		switch(typ) {
			case NPCType.Mutant: return 0;
			case NPCType.Supermutant: return 1;
			case NPCType.Robot: return 2;
			case NPCType.Cyborg: return 3;
			case NPCType.Supercyborg: return 4;
			case NPCType.MutantCyborg: return 5;
			case NPCType.Cyber: return 6;
		}
		return 0;
	}

	public static NPCType GetNPCTypeFromInt(int typ_i) {
		switch(typ_i) {
			case 0: return NPCType.Mutant;
			case 1: return NPCType.Supermutant;
			case 2: return NPCType.Robot;
			case 3: return NPCType.Cyborg;
			case 4: return NPCType.Supercyborg;
			case 5: return NPCType.MutantCyborg;
			case 6: return NPCType.Cyber;
		}
		return NPCType.Mutant;
	}

    public static int AIStateToInt(AIState ai_state) {
		switch (ai_state) {
			case AIState.Walk: return 1;
			case AIState.Run: return 2;
			case AIState.Attack1: return 3;
			case AIState.Attack2: return 4;
			case AIState.Attack3: return 5;
			case AIState.Pain: return 6;
			case AIState.Dying: return 7;
			case AIState.Inspect: return 8;
			case AIState.Interacting: return 9;
			case AIState.Dead: return 10;
		}
        return 0; // Idle
    }

    public static AIState GetAIStateFromInt(int ai_state_i) {
        switch (ai_state_i) {
            case 0:  return AIState.Idle;
            case 1:  return AIState.Walk;
            case 2:  return AIState.Run;
            case 3:  return AIState.Attack1;
            case 4:  return AIState.Attack2;
            case 5:  return AIState.Attack3;
            case 6:  return AIState.Pain;
            case 7:  return AIState.Dying;
            case 8:  return AIState.Inspect;
            case 9:  return AIState.Interacting;
            case 10: return AIState.Dead;
        }
        return AIState.Idle;
    }

	public static AttackType GetAttackTypeFromInt(int att_type_i) {
		switch(att_type_i) {
			case 0: return AttackType.None;
			case 1: return AttackType.Melee;
			case 2: return AttackType.EnergyBeam;
			case 3: return AttackType.Magnetic;
			case 4: return AttackType.Projectile;
			case 5: return AttackType.ProjectileEnergyBeam;
			case 6: return AttackType.MeleeEnergy;
			case 7: return AttackType.ProjectileLaunched;
			case 8: return AttackType.Gas;
			case 9: return AttackType.ProjectileNeedle;
			case 10:return AttackType.Tranq;
		}
		return AttackType.None;
	}

	public static PerceptionLevel GetPerceptionLevelFromInt(int percep_i) {
		switch(percep_i) {
			case 1: return PerceptionLevel.Medium;
			case 2: return PerceptionLevel.High;
			case 3: return PerceptionLevel.Omniscient;
		}
		return PerceptionLevel.Low;
	}

	public static AIMoveType GetMoveTypeFromInt(int movetype_i) { 
		switch (movetype_i) {
			case 0: return AIMoveType.None;
			case 1: return AIMoveType.Walk;
			case 2: return AIMoveType.Fly;
			case 3: return AIMoveType.Swim;
			case 4: return AIMoveType.Cyber;
		}
		return AIMoveType.None;
	}

	public static AudioLogType GetAudioLogTypeFromInt(int audtype_i) {
		switch(audtype_i) {
			case 0: return AudioLogType.TextOnly;
			case 1: return AudioLogType.Normal;
			case 2: return AudioLogType.Email;
			case 3: return AudioLogType.Papers;
			case 4: return AudioLogType.Vmail;
		}
		return AudioLogType.Normal;
	}

	public static int GetIntFromAudioLogType(AudioLogType audtype) {
		switch(audtype) {
			case AudioLogType.TextOnly: return 0;
			case AudioLogType.Normal:   return 1;
			case AudioLogType.Email:    return 2;
			case AudioLogType.Papers:   return 3;
			case AudioLogType.Vmail:    return 4;
		}
		return 1;
	}

	public static LightType GetLightTypeFromString(string type) {
		if (type == "Spot") return LightType.Spot;
		else if (type == "Directional") return LightType.Directional;
		else if (type == "Rectangle") return LightType.Rectangle;
		else if (type == "Disc") return LightType.Disc;
		return LightType.Point;	
	}

	public static LightShadows GetLightShadowsFromString(string shadows) {
		if (shadows == "None") return LightShadows.None;
		else if (shadows == "Hard") return LightShadows.Hard;
		return LightShadows.Soft;	
	}

	public static LightShadowResolution GetShadowResFromString(string res) {
		if (res == "Low") return LightShadowResolution.Low;
		else if (res == "Medium") return LightShadowResolution.Medium;
		else if (res == "High") return LightShadowResolution.High;
		else if (res == "VeryHigh") return LightShadowResolution.VeryHigh;
		return LightShadowResolution.FromQualitySettings;	
	}

    public static int FuncStatesToInt(FuncStates funcStates) {
		switch (funcStates) {
			case FuncStates.Start:            return 1;
			case FuncStates.Target:           return 2;
			case FuncStates.MovingStart:      return 3;
			case FuncStates.MovingTarget:     return 4;
			case FuncStates.AjarMovingStart:  return 5;
			case FuncStates.AjarMovingTarget: return 6;
		}
        return 0; // Idle
    }

    public static FuncStates GetFuncStatesFromInt(int state_i) {
        switch (state_i) {
            case 0: return FuncStates.Start;
            case 1: return FuncStates.Start;
            case 2: return FuncStates.Target;
            case 3: return FuncStates.MovingStart;
            case 4: return FuncStates.MovingTarget;
            case 5: return FuncStates.AjarMovingStart;
            case 6: return FuncStates.AjarMovingTarget;
        }
        return FuncStates.Start;
    }

//public enum ForceFieldColor : byte {Red,Green,Blue,Purple,RedFaint};
    public static int ForceFieldColorToInt(ForceFieldColor funcStates) {
		switch (funcStates) {
			case ForceFieldColor.Red:      return 1;
			case ForceFieldColor.Green:    return 2;
			case ForceFieldColor.Blue:     return 3;
			case ForceFieldColor.Purple:   return 4;
			case ForceFieldColor.RedFaint: return 5;
		}
        return 1; // Red
    }

    public static ForceFieldColor GetForceFieldColorFromInt(int state_i) {
        switch (state_i) {
            case 1: return ForceFieldColor.Red;
            case 2: return ForceFieldColor.Green;
            case 3: return ForceFieldColor.Blue;
            case 4: return ForceFieldColor.Purple;
            case 5: return ForceFieldColor.RedFaint;
        }
        return ForceFieldColor.Red;
    }

    public static SaveableType GetSaveableTypeFromInt(int savtyp) {
        switch (savtyp) {
            case 0:  return SaveableType.Player;
            case 1:  return SaveableType.Useable;
            case 2:  return SaveableType.Grenade;
            case 3:  return SaveableType.NPC;
            case 4:  return SaveableType.Destructable;
            case 5:  return SaveableType.SearchableStatic;
            case 6:  return SaveableType.SearchableDestructable;
            case 7:  return SaveableType.Door;
            case 8:  return SaveableType.ForceBridge;
            case 9:  return SaveableType.Switch;
            case 10: return SaveableType.FuncWall;
            case 11: return SaveableType.TeleDest;
            case 12: return SaveableType.LBranch;
            case 13: return SaveableType.LRelay;
            case 14: return SaveableType.LSpawner;
            case 15: return SaveableType.InteractablePanel;
            case 16: return SaveableType.ElevatorPanel;
            case 17: return SaveableType.Keypad;
            case 18: return SaveableType.PuzzleGrid;
            case 19: return SaveableType.PuzzleWire;
            case 20: return SaveableType.TCounter;
            case 21: return SaveableType.TGravity;
            case 22: return SaveableType.MChanger;
            case 23: return SaveableType.GravPad;
            case 24: return SaveableType.TransformParentless;
            case 25: return SaveableType.ChargeStation;
            case 26: return SaveableType.Light;
            case 27: return SaveableType.LTimer;
            case 28: return SaveableType.Camera;
            case 29: return SaveableType.DelayedSpawn;
            case 30: return SaveableType.SecurityCamera;
            case 31: return SaveableType.Trigger;
            case 32: return SaveableType.Projectile;
            case 33: return SaveableType.NormalScreen;
            case 34: return SaveableType.CyberSwitch;
        }
        return SaveableType.Transform;
    }

    public static string SaveTransform(Transform tr) {
		if (tr == null) {
			Debug.Log("Transform null while trying to save!");
			return DTypeWordToSaveString("ffffffffff");
		}

        StringBuilder s1 = new StringBuilder();
        s1.Clear();
        s1.Append(FloatToString(tr.localPosition.x));
        s1.Append(splitChar);
        s1.Append(FloatToString(tr.localPosition.y));
        s1.Append(splitChar);
        s1.Append(FloatToString(tr.localPosition.z));
        s1.Append(splitChar);
        s1.Append(FloatToString(tr.localRotation.x));
        s1.Append(splitChar);
        s1.Append(FloatToString(tr.localRotation.y));
        s1.Append(splitChar);
        s1.Append(FloatToString(tr.localRotation.z));
        s1.Append(splitChar);
        s1.Append(FloatToString(tr.localRotation.w));
        s1.Append(splitChar);
        s1.Append(FloatToString(tr.localScale.x));
        s1.Append(splitChar);
        s1.Append(FloatToString(tr.localScale.y));
        s1.Append(splitChar);
        s1.Append(FloatToString(tr.localScale.z));
        return s1.ToString();
    }

	public static int LoadTransform(Transform tr, ref string[] entries,
									 int index) {
		if (tr == null) {
			Debug.Log("Transform null while trying to load!");
			return index + 10;
		}

		// Get position
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
		if (tr.localPosition != tempvec) tr.localPosition = tempvec;

		// Get rotation
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		readFloatw = Utils.GetFloatFromString(entries[index]); index++;
		tempquat = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);
		tr.localRotation = tempquat;

		// Don't use Transform.SetPositionAndRotation since that sets global
		// position and the local is what is saved and loaded here.

		// Get scale
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
		tr.localScale = tempvec;
		return index; // Carry on with current index read.
	}

    public static string SaveRigidbody(GameObject go) {
		Rigidbody rbody = go.GetComponent<Rigidbody>();
		if (rbody == null) {
			return DTypeWordToSaveString("fff1"); // No warn, normal scenario.
		}

        StringBuilder s1 = new StringBuilder();
        s1.Clear();
        s1.Append(FloatToString(rbody.velocity.x));
        s1.Append(splitChar);
        s1.Append(FloatToString(rbody.velocity.y));
        s1.Append(splitChar);
        s1.Append(FloatToString(rbody.velocity.z));
        s1.Append(splitChar);
        s1.Append(BoolToString(rbody.isKinematic));
        return s1.ToString();
    }

	public static int LoadRigidbody(GameObject go, ref string[] entries,
									 int index) {
		Rigidbody rbody = go.GetComponent<Rigidbody>();
		if (rbody == null) {
			return index + 4; // No warn, normal scenario.
		}

		// Get rigidbody velocity
		readFloatx = GetFloatFromString(entries[index]); index++;
		readFloaty = GetFloatFromString(entries[index]); index++;
		readFloatz = GetFloatFromString(entries[index]); index++;
		tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
		rbody.velocity = tempvec;
		CollisionDetectionMode oldCollision = rbody.collisionDetectionMode;
		rbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		rbody.isKinematic = GetBoolFromString(entries[index]); index++;
		if (!rbody.isKinematic) rbody.collisionDetectionMode = oldCollision;
		if (rbody.collisionDetectionMode != oldCollision) Debug.Log("Collision mode changed on " + go.name + " from " + oldCollision.ToString() + " to " + rbody.collisionDetectionMode.ToString());
		return index; // Carry on with current index read.
	}

	public static string SaveSubActivatedGOState(GameObject subGO) {
		string line = System.String.Empty;
		line = Utils.SaveTransform(subGO.transform);
		line += Utils.splitChar + Utils.SaveRigidbody(subGO);
		line += Utils.splitChar + Utils.BoolToString(subGO.activeSelf);
		return line;
	}

	public static int LoadSubActivatedGOState(GameObject subGO,
											  ref string[] entries, int index) {
		index = Utils.LoadTransform(subGO.transform,ref entries,index); // 10
		index = Utils.LoadRigidbody(subGO,ref entries,index); // 4
		subGO.SetActive(Utils.GetBoolFromString(entries[index])); index++; // 10+4+1=15
		return index;
	}

	public static string SaveChildGOState(GameObject mainParent, int childex) {
			Transform childTR = mainParent.transform.GetChild(childex);
			return SaveSubActivatedGOState(childTR.gameObject);
	}

	public static string SaveCamera(GameObject go) {
		Camera cm = go.GetComponent<Camera>();
        Grayscale gsc = go.GetComponent<Grayscale>();
		if (cm == null) {
			Debug.Log("Camera missing on savetype of Camera!  GameObject.name: " + go.name);
			return DTypeWordToSaveString("bbfb");
		}

		string line = System.String.Empty;
        line = BoolToString(cm.enabled); // bool
        if (gsc != null) line += splitChar + BoolToString(gsc.enabled);
        else line += splitChar + "0";
		// 4
		return line;
	}

	public static int LoadCamera(GameObject go, ref string[] entries, int index) {
		Camera cm = go.GetComponent<Camera>();
		Grayscale gsc = go.GetComponent<Grayscale>();
		if (cm == null || index < 0 || entries == null) return index + 4;

		cm.enabled = GetBoolFromString(entries[index]); index++;
		if (gsc != null) {
			gsc.enabled = GetBoolFromString(entries[index]); index++;
		} else index++;

		return index;
	}

	public static void PlayAudioSavable(AudioSource SFX, AudioClip fxclip,
										  float vol, bool isOneShot) {
		if (SFX == null) return;

		GameObject sourceGO = SFX.gameObject;
		if (!sourceGO.activeInHierarchy) return;
		if (fxclip == null) return; // This is a common usage, don't warn. 
		if (SFX.enabled == false) return;

		SFX.clip = fxclip; // Save the currently playing clip otherwise this is
						   // always the default that was assigned in inspector.
		if (isOneShot) {
			if (vol != 0) SFX.PlayOneShot(fxclip,vol);
			else		  SFX.PlayOneShot(fxclip);
		} else {
			SFX.Play();
		}
	}

	public static void PlayOneShotSavable(AudioSource SFX, AudioClip fxclip,
										  float vol) {
		PlayAudioSavable(SFX,fxclip,vol,true);
	}

	public static void PlayOneShotSavable(AudioSource SFX, AudioClip fxclip) {
		PlayOneShotSavable(SFX,fxclip,0);
	}

	public static void PlayOneShotSavable(AudioSource SFX, int fxclip) {
		if (fxclip < 0) return;
		if (fxclip >= Const.a.sounds.Length) return;

		PlayOneShotSavable(SFX,Const.a.sounds[fxclip],0);
	}

	public static void PlayOneShotSavable(AudioSource SFX, int fxclip,
										  float vol) {
		if (fxclip < 0) return;
		if (fxclip >= Const.a.sounds.Length) return;

		PlayOneShotSavable(SFX,Const.a.sounds[fxclip],vol);
	}

	public static void PlaySavable(AudioSource SFX, AudioClip fxclip) {
		PlayAudioSavable(SFX,fxclip,0,false);
	}

	public static void PlaySavable(AudioSource SFX, int fxclip) {
		PlayAudioSavable(SFX,Const.a.sounds[fxclip],0,false);
	}

	public static string SaveAudioSource(GameObject go) {
		AudioSource aus = go.GetComponent<AudioSource>();
		if (aus == null) {
			Debug.Log("AudioSource missing!  GameObject.name: " + go.name);
			return DTypeWordToSaveString("bfs");
		}

		string line = System.String.Empty;
        line = BoolToString(aus.enabled);
		line += splitChar + FloatToString(aus.time);
		if (aus.clip == null) line += splitChar + "none";
		else line += splitChar + aus.clip.name;

		return line;
	}

	public static int LoadAudioSource(GameObject go, ref string[] entries, int index) {
		AudioSource aus = go.GetComponent<AudioSource>();
		if (aus == null || index < 0 || entries == null) return index + 3;

		aus.enabled = GetBoolFromString(entries[index]); index++;
		aus.time = GetFloatFromString(entries[index]); index++;
		index++;
		return index;
	}

    // Using "relative time" = PauseScript.a.relativeTime and "finished" = some
    // script's timer float value, e.g. attackFinished, in the notes below...
    //
    // If the relative time is 123 when we save and finished is 156, then when
    // we load and relative time is 160 and we set finished to 156, it will
    // immediately finish.
    //
    // Need to take finished - relative time = 33 and save that.  Then on load
    // take this differential value and do relative time + differential = 160 +
    // 33 = 193, then the same condition is restored such that the finished 
    // timer still has 33 before it is up.
    //
    // In the scenario where finished is less than relative time, if finished =
    // 103 and relative time is still 123, then when we load and relative time
    // is 160, all is still fine, timer is already up.
    //
    // Still can't hurt to do finished - relative time = -20. Then when we load
    // the value do relative time 160 + -20 = 140.  This is arguably best just
    // in case there is a whackado one-off instance of comparing (time - 
    // finished) somewhere instead of (finished < time) which is my usual Quake
    // derived timer pattern.
    public static string SaveRelativeTimeDifferential(float timerValue) {
        if (PauseScript.a == null) return "0000.00000";

        float val = timerValue - PauseScript.a.relativeTime; // Remove current
                                                             // instance's
                                                             // relative time.
        return FloatToString(val);
    }

    public static float LoadRelativeTimeDifferential(string savedTimer) {
        float val = GetFloatFromString(savedTimer);
        if (PauseScript.a == null) return val;
        return PauseScript.a.relativeTime + val; // Add current instance's
                                                 // relative time to get same
                                                 // timer in context of current
                                                 // time.  See above notes.
    }

	public static void SafeDestroy(GameObject go, bool immediate) {
		if (go == null) return;

		if (go.layer == 12) {
			Debug.Log("Tried to Destroy() layered part of player!");
			return;
		}

		if (go.name == "Player") {
			Debug.Log("Tried to Destroy() Player!");
			return;
		}

		if (go.name == "Const") {
			Debug.Log("Tried to Destroy() Const!");
			return;
		}

		if (go.name == "PlayerCapsule") {
			Debug.Log("Tried to Destroy() PlayerCapsule!");
			return;
		}

		if (go.name == "Sky") {
			Debug.Log("Tried to Destroy() Sky!");
			return;
		}

		if (go.name == "LevelManager") {
			Debug.Log("Tried to Destroy() LevelManager!");
			return;
		}

		if (go.name == "EventSystem") {
			Debug.Log("Tried to Destroy() EventSystem!");
			return;
		}

		if (go.name == "Exterior") {
			Debug.Log("Tried to Destroy() Exterior!");
			return;
		}

		if (go.name == "1.MedicalLevel") {
			Debug.Log("Tried to Destroy() Medical level!");
			return;
		}

		if (immediate) MonoBehaviour.DestroyImmediate(go);
		else MonoBehaviour.Destroy(go);
	}

	public static void SafeDestroy(GameObject go) {
		SafeDestroy(go,false);
	}

	public static void SafeDestroyImmediate(GameObject go) {
		SafeDestroy(go,true);
	}

	public static void DestroyImmediateAllChildren(Transform parent) {
		for (int i=0;i<parent.childCount;i++) {
			SafeDestroyImmediate(parent.GetChild(i).gameObject);
		}
	}

	public static void ApplyImpactForce(GameObject go, float impactVelocity,
										Vector3 attackNormal, Vector3 spot) {
		Rigidbody rbody = go.GetComponent<Rigidbody>();
		if (rbody == null) return;

		rbody.WakeUp();
		if (impactVelocity <= 0) return;

		rbody.AddForceAtPosition((attackNormal*impactVelocity*30f),spot);
	}

	public static void ApplyImpactForceSphere(DamageData dd,Vector3 centerPoint,
											  float radius,float impactScale,
											  int mask) {
		HealthManager hm = null;
		Collider[] colliders = Physics.OverlapSphere(centerPoint,radius);
		int i = 0;
		while (i < colliders.Length) {
			GameObject go = colliders[i].gameObject;
			if (go == null) { i++; continue; }
			//if (go.isStatic) { i++; continue; } EDITOR ONLY!!!!!!

			Rigidbody rbody = go.GetComponent<Rigidbody>();
			if (rbody == null) { i++; continue; }

			dd.impactVelocity = dd.damage * impactScale;
			Vector3 dir = go.transform.position - centerPoint;
			RaycastHit hit;
			if (Physics.Raycast(centerPoint,dir,out hit,radius + 0.02f,mask)) {
				hm = go.GetComponent<HealthManager>();
				if (hm != null) {
					if (hm.isPlayer) {
						dd.damage = dd.damage * 0.5f; // give em a chance mate
					}

					float distPenalty = (radius - hit.distance) / radius;
					float saturation = dd.damage * 0.33f; // minimum damage in
														  // range to feel more
														  // powerful/useful
					dd.damage *= distPenalty; // Linear falloff
					if (dd.damage < saturation) dd.damage = saturation;
					hm.TakeDamage(dd);
				}

				if (hit.collider == colliders[i] || hit.rigidbody == rbody) {
					rbody.AddExplosionForce(dd.impactVelocity,centerPoint,radius,1f);
				}
			}
			i++;
		}
	}

	public static void PlayTempAudio(Vector3 spot,AudioClip clip) {
		GameObject tempAud = Const.a.GetObjectFromPool(PoolType.TempAudioSources);
		if (tempAud != null) {
			tempAud.transform.position = spot; // set temporary audiosource to right here
			tempAud.SetActive(true);
			AudioSource aS = tempAud.GetComponent<AudioSource>();
			PlayOneShotSavable(aS,clip);
		}
	}

	public static bool IndexEntriesOk(int index, ref string[] entries, GameObject go) {
		if (index < 0) {
			Debug.Log("Index less than zero when loading " + go.name);
			return false;
		}

		if (index >= entries.Length) {
			Debug.Log("Index too large at " + index.ToString() + " when loading " + go.name + ", entries size: " + entries.Length.ToString());
			return false;
		}
		return true;
	}
}