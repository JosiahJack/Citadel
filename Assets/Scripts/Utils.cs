using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;

#if UNITY_EDITOR
	using UnityEditor;
	using UnityEditor.SceneManagement;
#endif

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

	public static StringBuilder s1;
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

	public static float Sign(float value) {
		if (value < Mathf.Epsilon && value > -Mathf.Epsilon) return 0f;
		if (value > 0f) return 1f;
		return -1f;
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
		//if (img == null) return;

		if (!img.enabled) img.enabled = true;
	}

	public static void DisableImage(Image img) {
		//if (img == null) return;

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
        return combinedPath;
    }

    #pragma warning disable 618
	public static StreamReader ReadStreamingAsset(string fName) {
		StreamReader dataReader;
        string fPath = SafePathCombine(Application.streamingAssetsPath,fName);
        if (Application.platform == RuntimePlatform.Android) {
            WWW reader = new WWW(fPath);
            while (!reader.isDone) { }
            MemoryStream memStr = new MemoryStream(Encoding.ASCII.GetBytes(reader.text));
            dataReader = new StreamReader(memStr,Encoding.ASCII);
        } else {
// 			Debug.Log("Reading " + fName + " from StreamingAssets/");
            Utils.ConfirmExistsInStreamingAssetsMakeIfNot(fName);
		    dataReader = new StreamReader(fPath, Encoding.ASCII);
        }
        
        return dataReader;
	}
	#pragma warning restore 618

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

		if (File.Exists(strmAstPth)) return; // Already exists, all good!

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

	public static void ConfirmExistsInPersistentDataMakeIfNot(string fileName) {
		if (string.IsNullOrWhiteSpace(fileName)) {
			UnityEngine.Debug.Log("fileName was null or whitespace passed to "
								  + "ConfirmExistsInPersistentDataMakeIfNot");
			return;
		}

		string persDatPth = SafePathCombine(Application.persistentDataPath,
											fileName);

		if (File.Exists(persDatPth)) return; // Already exists, all good!

		string rsrc = ResourcesPathCombine("StreamingAssetsRecovery",fileName);
        TextAsset resourcesFile = (TextAsset)Resources.Load(rsrc);
        if (resourcesFile != null) {
			// Recreate from Resources/StreamingAssetsRecovery/*
			File.WriteAllText(persDatPth, resourcesFile.text, // new, contents
							  Encoding.ASCII);
			if (File.Exists(persDatPth)) {
				UnityEngine.Debug.Log("File " + persDatPth + " recreated");
			} else {
				UnityEngine.Debug.Log("File " + persDatPth + " failed to be "
									  + "created by File.WriteAllText!");
			}
        } else {
			UnityEngine.Debug.Log("File " + persDatPth + " not found in the "
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
	
	public static string TrackTypeToString(TrackType typ, string name) {
		switch (typ) {
			case TrackType.None:            return  IntToString(0,name);
			case TrackType.Walking:         return  IntToString(1,name);;
			case TrackType.Combat:          return  IntToString(2,name);;
			case TrackType.MutantNear:      return  IntToString(3,name);;
			case TrackType.CyborgNear:      return  IntToString(4,name);;
			case TrackType.CyborgDroneNear: return  IntToString(5,name);;
			case TrackType.RobotNear:       return  IntToString(6,name);;
			case TrackType.Transition:      return  IntToString(7,name);;
			case TrackType.Revive:          return  IntToString(8,name);;
			case TrackType.Death:           return  IntToString(9,name);;
			case TrackType.Cybertube:       return IntToString(10,name);;
			case TrackType.Elevator:        return IntToString(11,name);;
			case TrackType.Distortion:      return IntToString(12,name);;
		}
		
		return name + SaveLoad.keyValueSplitChar;
	}

	public static TrackType IntToTrackType(string typ, string name) {
		string[] splits = typ.Split(':');
		if (splits.Length < 2) return TrackType.None;

		string nameReceived = splits[0];
		int valueReceived = GetIntFromString(splits[1],name);
		if (nameReceived != name) {
			UnityEngine.Debug.LogError("BUG: Attempting to parse " + typ
								       + " when wanting bool named " + name);
			return TrackType.None;
		}
		
		switch (valueReceived) {
			case 0:  return TrackType.None;
			case 1:  return TrackType.Walking;
			case 2:  return TrackType.Combat;
			case 3:  return TrackType.MutantNear;
			case 4:  return TrackType.CyborgNear;
			case 5:  return TrackType.CyborgDroneNear;
			case 6:  return TrackType.RobotNear;
			case 7:  return TrackType.Transition;
			case 8:  return TrackType.Revive;
			case 9:  return TrackType.Death;
			case 10: return TrackType.Cybertube;
			case 11: return TrackType.Elevator;
			case 12: return TrackType.Distortion;
		}
		
		return TrackType.None;
	}
	
	public static string MusicTypeToString(MusicType typ, string name) {
		switch (typ) {
			case MusicType.None:     return IntToString(0,name);
			case MusicType.Walking:  return IntToString(1,name);
			case MusicType.Combat:   return IntToString(2,name);
			case MusicType.Overlay:  return IntToString(3,name);
			case MusicType.Override: return IntToString(4,name);
		}
		
		return name + SaveLoad.keyValueSplitChar;
	}

	public static MusicType IntToMusicType(string typ, string name) {
		string[] splits = typ.Split(':');
		if (splits.Length < 2) return MusicType.None;

		string nameReceived = splits[0];
		int valueReceived = GetIntFromString(splits[1],name);
		if (nameReceived != name) {
			UnityEngine.Debug.LogError("BUG: Attempting to parse " + typ
								       + " when wanting bool named " + name);
			return MusicType.None;
		}
		
		switch (valueReceived) {
			case 0:  return MusicType.None;
			case 1:  return MusicType.Walking;
			case 2:  return MusicType.Combat;
			case 3:  return MusicType.Overlay;
			case 4:  return MusicType.Override;
		}
		
		return MusicType.None;
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

// 	public static string IntToString(int val) {
// 		return val.ToString();
// 	}

	public static string IntToString(int val, string name) {
		return name + ":" + val.ToString();
	}

// 	public static string UintToString(int val) {
// 		if (val < 0) return "-1";
// 		return val.ToString();
// 	}

	public static string UintToString(int val, string name) {
		if (val < 0) return name + ":-1";
		return name + ":" + val.ToString();
	}

	public static bool GetBoolFromStringInTables(string val) {
		return val.Equals("1");
	}
	
	private static int GetColonIndex(string val) {
		int colonIndex = -1;
		for (int i = 0; i < val.Length; i++) { // Update below in StringEquals if changing here as I've inlined it there for performance.
			if (val[i] == ':') {
				colonIndex = i;
				break;
			}
		}

		return colonIndex;
	}
	
	// Iterate over individual characters to avoid the garbage allocation from
	// builtin string methods (e.g. String.Split).  Especially critical as this
	// is used by _every_ parser of game data files and savegames.
	private static bool StringEquals(string val, string name) {
		int colonIndex = -1;
		for (int i = 0; i < val.Length; i++) { // Inlined of GetColonIndex() above for performance.
			if (val[i] == ':') {
				colonIndex = i;
				break;
			}
		}

		if (colonIndex < 0) return false; // Change logic below in overload if changing here, inlined there for performance.
		if (name.Length > colonIndex) return false;

		for (int i = 0; i < colonIndex; i++) {
			if (val[i] != name[i]) return false;
		}

		return true;
	}
	
	private static bool StringEquals(string val, string name, int colonIndex) {
		if (colonIndex < 0) return false; // Inlined from above
		if (name.Length > colonIndex) return false;

		for (int i = 0; i < colonIndex; i++) {
			if (val[i] != name[i]) return false;
		}

		return true;
	}

	public static bool GetBoolFromString(string val, string name) {
		if (val.Length < 3 || val[val.Length - 2] != ':') {
			return GetBoolFromStringInTables(val); // Fallback if no colon or empty value
		}

		if (!StringEquals(val,name)) {
			UnityEngine.Debug.LogError("BUG: Attempting to parse " + val
								  + " when wanting bool named " + name
								  + ", returning false as fallback on "
								  + SaveObject.currentObjectInfo);

			return false;
		}
		
		if (val.Length - name.Length != 2 || (val[val.Length - 1] != '0' && val[val.Length - 1] != '1')) {
			UnityEngine.Debug.LogError("BUG: Attempting to parse " + val
								       + " when wanting bool named " + name
								       + ", returning false as fallback on "
								       + SaveObject.currentObjectInfo);
			return false;
		}
		
		return val[val.Length - 1] == '1';
	}

	// Variant used for Config.ini without colon and string name
	public static string BoolToStringConfig(bool inputValue) {
		if (inputValue) return "1";
		return "0";
	}

	public static string BoolToString(bool inputValue, string name) {
		if (inputValue) return name + ":1";
		return name + ":0";
	}

	// Used by Const to load text for audio logs.
	public static int GetIntFromStringAudLogText(string val) {
		if (val == "0") return 0;

		getValparsed = Int32.TryParse(val, NumberStyles.Integer, en_US_Culture,
                                      out getValreadInt);
		if (!getValparsed) {
			UnityEngine.Debug.LogError("BUG: Could not parse int from: " + val
                                   + ", returning 0 as a fallback on "
								   + SaveObject.currentObjectInfo);
			return 0;
		}

		return getValreadInt;
	}

	public static int GetIntFromString(string val, string name) {
		int colonIndex = GetColonIndex(val);
		if (!StringEquals(val,name,colonIndex)) {
			UnityEngine.Debug.LogError("BUG: Attempting to parse " + val
						               + " when wanting int named " + name
						               + ", returning 0 as fallback on "
						               + SaveObject.currentObjectInfo);
			return 0;
		}

		int valueStart = colonIndex + 1;								             //    34           Length:15
		int valueLength = val.Length - valueStart;							         // 0123456789ABCDE   15 - 4 = 11 for valueLength
		if (!TryParseIntManual(val,valueStart,valueLength, out int getValreadInt)) { // key:-0000.00000
			UnityEngine.Debug.LogError("BUG: Could not parse int from: " + val
									   + " for variable named " + name 
									   + ", returning 0 as a fallback on "
									   + SaveObject.currentObjectInfo);
			return 0;
		}

		return getValreadInt;
	}
	
	private static bool TryParseIntManual(string source, int start, int length, out int result) {
		result = 0;
		if (length <= 0 || start + length > source.Length) return false;

		bool isNegative = false;
		int i = start;
		if (source[i] == '-') {
			isNegative = true;
			i++;
			length--;
			if (length <= 0) return false;
		}

		int value = 0;
		bool hasDigits = false;
		for (; i < source.Length; i++) {
			char c = source[i];
			if (c < '0' || c > '9') break;
			value = value * 10 + (c - '0');
			hasDigits = true;
			if (value < 0 && !isNegative) return false;
		}

		if (!hasDigits) return false;
		
		if (value != 0) {
			result = isNegative ? -value : value;
		} else result = value;
		return true;
	}

	public static float GetFloatFromStringDataTables(string val) {
		getValparsed = Single.TryParse(val, NumberStyles.Float, en_US_Culture,
                                       out getValreadFloat);
		if (!getValparsed) {
			UnityEngine.Debug.LogError("BUG: Could not parse float from: "
                                   + val + ", returning 0.0 as fallback on "
							   	   + SaveObject.currentObjectInfo);
			return 0.0f;
		}
		return getValreadFloat;
	}

	public static float GetFloatFromString(string val, string name) {
		int colonIndex = GetColonIndex(val);
		if (!StringEquals(val,name,colonIndex)) {
			UnityEngine.Debug.LogError("BUG: Attempting to parse " + val
						               + " when wanting float named " + name
						               + ", returning 0 as fallback on "
						               + SaveObject.currentObjectInfo);
			return 0;
		}
		
		int valueStart = colonIndex + 1;
		int valueLength = val.Length - valueStart;
		if (!TryParseFloatManual(val, valueStart, valueLength, out float getValparsed)) {
			UnityEngine.Debug.LogError("BUG: Could not parse float from: " +
									   val + " for variable named " + name
									   + ", returning 0 as a fallback on "
									   + SaveObject.currentObjectInfo);
			return 0f;
		}

		return getValparsed;
	}

	private static bool TryParseFloatManual(string source, int start, int length, out float result) {
		result = 0f;
		if (length <= 0 || start + length > source.Length) return false;

		bool isNegative = false;
		int i = start;
		if (source[i] == '-') {
			isNegative = true;
			i++;
			length--;
			if (length <= 0) return false;
		} else if (source[i] == '+') { // Handle your +0001.00000 format
			i++;
			length--;
			if (length <= 0) return false;
		}

		float whole = 0f;
		float fraction = 0f;
		float divisor = 1f;
		bool inFraction = false;

		for (; i < start + length; i++) {
			char c = source[i];
			if (c == '.') {
				if (inFraction) return false; // Multiple decimals invalid
				inFraction = true;
				continue;
			}
			if (c < '0' || c > '9') return false; // Only digits allowed
			int digit = c - '0';
			if (inFraction) {
				divisor *= 10f;
				fraction += digit / divisor;
			} else {
				whole = whole * 10f + digit;
			}
		}

		result = whole + fraction;
		if (isNegative) result = -result;
		return true;
	}

    // Output with 4 integer places and 5 mantissa, culture invariant to
    // guarantee . is used as a separator rather than , for all regions.
	public static string FloatToString(float val, string name) {
		return name+":"+val.ToString("0000.00000",CultureInfo.InvariantCulture); 
	}

	public static string SaveString(string val, string name) {
		return name + ":" + val;
	}

	public static string LoadString(string val, string name) {
		string[] splits = val.Split(':');
		if (splits.Length < 2) return val;

		string nameReceived = splits[0];
		string valueReceived = splits[1];
		if (splits.Length > 2) {
			for (int i=2;i<splits.Length;i++) {
				valueReceived += splits[i]; // Stitch together remaining colons
			}
		}

		if (nameReceived != name) {
			UnityEngine.Debug.LogError("BUG: Attempting to parse " + val
								       + " when wanting string named " + name
								       + ", returning 'unknown' as fallback on "
								       + SaveObject.currentObjectInfo);

			return "unknown";
		}

		return valueReceived;
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
			case 5: return AudioLogType.Game;
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
			case AudioLogType.Game:     return 5;
		}
		return 1;
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
			case 35: return SaveableType.CyberItem;
        }
        return SaveableType.Transform;
    }

    public static string SaveTransform(Transform tr) {
        StringBuilder s1 = new StringBuilder();
        s1.Clear();
		
		if (tr is RectTransform rectTr) {
			s1.Append(FloatToString(rectTr.anchoredPosition.x,"anchoredPosition.x"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.anchoredPosition.y,"anchoredPosition.y"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.anchoredPosition3D.x,"anchoredPosition3D.x"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.anchoredPosition3D.y,"anchoredPosition3D.y"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.anchoredPosition3D.z,"anchoredPosition3D.z"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.anchorMax.x,"anchorMax.x"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.anchorMax.y,"anchorMax.y"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.anchorMin.x,"anchorMin.x"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.anchorMin.y,"anchorMin.y"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.offsetMax.x,"offsetMax.x"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.offsetMax.y,"offsetMax.y"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.offsetMin.x,"offsetMin.x"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.offsetMin.y,"offsetMin.y"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.pivot.x,"pivot.x"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.pivot.y,"pivot.y"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.localRotation.x,"localRotation.x"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.localRotation.y,"localRotation.y"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.localRotation.z,"localRotation.z"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.localRotation.w,"localRotation.w"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.localScale.x,"localScale.x"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.localScale.y,"localScale.y"));
			s1.Append(splitChar);
			s1.Append(FloatToString(rectTr.localScale.z,"localScale.z"));
		} else {
			s1.Append(FloatToString(tr.localPosition.x,"localPosition.x"));
			s1.Append(splitChar);
			s1.Append(FloatToString(tr.localPosition.y,"localPosition.y"));
			s1.Append(splitChar);
			s1.Append(FloatToString(tr.localPosition.z,"localPosition.z"));
			s1.Append(splitChar);
			s1.Append(FloatToString(tr.localRotation.x,"localRotation.x"));
			s1.Append(splitChar);
			s1.Append(FloatToString(tr.localRotation.y,"localRotation.y"));
			s1.Append(splitChar);
			s1.Append(FloatToString(tr.localRotation.z,"localRotation.z"));
			s1.Append(splitChar);
			s1.Append(FloatToString(tr.localRotation.w,"localRotation.w"));
			s1.Append(splitChar);
			s1.Append(FloatToString(tr.localScale.x,"localScale.x"));
			s1.Append(splitChar);
			s1.Append(FloatToString(tr.localScale.y,"localScale.y"));
			s1.Append(splitChar);
			s1.Append(FloatToString(tr.localScale.z,"localScale.z"));
		}
        return s1.ToString();
    }

	public static int LoadTransform(Transform tr, ref string[] entries,
									 int index) {
		if (tr == null) {
			Debug.Log("Transform null while trying to load! "
					  + SaveObject.currentObjectInfo);
			
			if (entries[index].Contains("anchoredPosition")) return index + 22;
			else return index + 10;
		}
		
		if (tr is RectTransform rectTr) {
			readFloatx = GetFloatFromString(entries[index],"anchoredPosition.x"); index++;
			readFloaty = GetFloatFromString(entries[index],"anchoredPosition.y"); index++;
			rectTr.anchoredPosition = new Vector2(readFloatx,readFloaty);
			
			readFloatx = GetFloatFromString(entries[index],"anchoredPosition3D.x"); index++;
			readFloaty = GetFloatFromString(entries[index],"anchoredPosition3D.y"); index++;
			readFloatz = GetFloatFromString(entries[index],"anchoredPosition3D.z"); index++;
			rectTr.anchoredPosition3D = new Vector3(readFloatx,readFloaty,readFloatz);
			
			readFloatx = GetFloatFromString(entries[index],"anchorMax.x"); index++;
			readFloaty = GetFloatFromString(entries[index],"anchorMax.y"); index++;
			rectTr.anchorMax = new Vector2(readFloatx,readFloaty);
			
			readFloatx = GetFloatFromString(entries[index],"anchorMin.x"); index++;
			readFloaty = GetFloatFromString(entries[index],"anchorMin.y"); index++;
			rectTr.anchorMin = new Vector2(readFloatx,readFloaty);
			
			readFloatx = GetFloatFromString(entries[index],"offsetMax.x"); index++;
			readFloaty = GetFloatFromString(entries[index],"offsetMax.y"); index++;
			rectTr.offsetMax = new Vector2(readFloatx,readFloaty);
			
			readFloatx = GetFloatFromString(entries[index],"offsetMin.x"); index++;
			readFloaty = GetFloatFromString(entries[index],"offsetMin.y"); index++;
			rectTr.offsetMin = new Vector2(readFloatx,readFloaty);
			
			readFloatx = GetFloatFromString(entries[index],"pivot.x"); index++;
			readFloaty = GetFloatFromString(entries[index],"pivot.y"); index++;
			rectTr.pivot = new Vector2(readFloatx,readFloaty);
			
			// Get rotation
			readFloatx = GetFloatFromString(entries[index],"localRotation.x"); index++;
			readFloaty = GetFloatFromString(entries[index],"localRotation.y"); index++;
			readFloatz = GetFloatFromString(entries[index],"localRotation.z"); index++;
			readFloatw = GetFloatFromString(entries[index],"localRotation.w"); index++;
			rectTr.localRotation = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);

			// Don't use Transform.SetPositionAndRotation since that sets global
			// position and the local is what is saved and loaded here.

			// Get scale
			readFloatx = GetFloatFromString(entries[index],"localScale.x"); index++;
			readFloaty = GetFloatFromString(entries[index],"localScale.y"); index++;
			readFloatz = GetFloatFromString(entries[index],"localScale.z"); index++;
			rectTr.localScale = new Vector3(readFloatx,readFloaty,readFloatz);
		} else {
			// Get position
			readFloatx = GetFloatFromString(entries[index],"localPosition.x"); index++;
			readFloaty = GetFloatFromString(entries[index],"localPosition.y"); index++;
			readFloatz = GetFloatFromString(entries[index],"localPosition.z"); index++;
			tr.localPosition = new Vector3(readFloatx,readFloaty,readFloatz);

			// Get rotation
			readFloatx = GetFloatFromString(entries[index],"localRotation.x"); index++;
			readFloaty = GetFloatFromString(entries[index],"localRotation.y"); index++;
			readFloatz = GetFloatFromString(entries[index],"localRotation.z"); index++;
			readFloatw = GetFloatFromString(entries[index],"localRotation.w"); index++;
			tr.localRotation = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);

			// Don't use Transform.SetPositionAndRotation since that sets global
			// position and the local is what is saved and loaded here.

			// Get scale
			readFloatx = GetFloatFromString(entries[index],"localScale.x"); index++;
			readFloaty = GetFloatFromString(entries[index],"localScale.y"); index++;
			readFloatz = GetFloatFromString(entries[index],"localScale.z"); index++;
			tr.localScale = new Vector3(readFloatx,readFloaty,readFloatz);
		}
		return index; // Carry on with current index read.
	}
	
	public static string SaveBoxCollider(GameObject go) {
		BoxCollider bcol = go.GetComponent<BoxCollider>();
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(FloatToString(bcol.center.x,"center.x"));
		s1.Append(splitChar);
		s1.Append(FloatToString(bcol.center.y,"center.y"));
		s1.Append(splitChar);
		s1.Append(FloatToString(bcol.center.z,"center.z"));
		s1.Append(splitChar);
		s1.Append(FloatToString(bcol.size.x,"size.x"));
		s1.Append(splitChar);
		s1.Append(FloatToString(bcol.size.y,"size.y"));
		s1.Append(splitChar);
		s1.Append(FloatToString(bcol.size.z,"size.z"));
		return s1.ToString();
	}
	
	public static int LoadBoxCollider(GameObject go, ref string[] entries, int index) {
		BoxCollider bcol = go.GetComponent<BoxCollider>();
		float readX = Utils.GetFloatFromString(entries[index],"center.x"); index++;
		float readY = Utils.GetFloatFromString(entries[index],"center.y"); index++;
		float readZ = Utils.GetFloatFromString(entries[index],"center.z"); index++;
		bcol.center = new Vector3(readX,readY,readZ);
		readX = Utils.GetFloatFromString(entries[index],"size.x"); index++;
		readY = Utils.GetFloatFromString(entries[index],"size.y"); index++;
		readZ = Utils.GetFloatFromString(entries[index],"size.z"); index++;
		bcol.size = new Vector3(readX,readY,readZ);
		return index;
	}

    public static string SaveRigidbody(GameObject go) {
		if (go == null) return ("velocity.x:0000.00000" + splitChar + "velocity.y:0000.00000" + splitChar + "velocity.z:0000.00000" + splitChar + "isKinematic:0");
		
		Rigidbody rbody = go.GetComponent<Rigidbody>();
		if (rbody == null) return ("velocity.x:0000.00000" + splitChar + "velocity.y:0000.00000" + splitChar + "velocity.z:0000.00000" + splitChar + "isKinematic:0");

		StringBuilder s1 = new StringBuilder();
        s1.Clear();
        s1.Append(FloatToString(rbody.velocity.x,"velocity.x"));
        s1.Append(splitChar);
        s1.Append(FloatToString(rbody.velocity.y,"velocity.y"));
        s1.Append(splitChar);
        s1.Append(FloatToString(rbody.velocity.z,"velocity.z"));
        s1.Append(splitChar);
        s1.Append(BoolToString(rbody.isKinematic,"isKinematic"));
        return s1.ToString();
    }

	public static int LoadRigidbody(GameObject go, ref string[] entries, int index) {
		Rigidbody rbody = go.GetComponent<Rigidbody>();
		if (rbody == null) return index + 4;

		// Get rigidbody velocity
		readFloatx = GetFloatFromString(entries[index],"velocity.x"); index++;
		readFloaty = GetFloatFromString(entries[index],"velocity.y"); index++;
		readFloatz = GetFloatFromString(entries[index],"velocity.z"); index++;
		tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
		rbody.velocity = tempvec;
// 		CollisionDetectionMode oldCollision = rbody.collisionDetectionMode;
		rbody.isKinematic = GetBoolFromString(entries[index],"isKinematic"); index++;
// 		if (rbody.isKinematic) rbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
// 		else rbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
// 		
// 		if (rbody.collisionDetectionMode != oldCollision && !rbody.isKinematic) {
// 			if (oldCollision == CollisionDetectionMode.Discrete) rbody.collisionDetectionMode = oldCollision;
// 		}

		return index; // Carry on with current index read.
	}

	public static string SaveSubActivatedGOState(GameObject subGO) {
		if (subGO == null) {
			return "localPosition.x:0000.00000|localPosition.y:0000.00000|"
				   + "localPosition.z:0000.00000|localRotation.x:0000.00000|"
				   + "localRotation.y:0000.00000|localRotation.z:0000.00000|"
				   + "localRotation.w:0000.00000|localScale.x:0000.00000|"
				   + "localScale.y:0000.00000|localScale.z:0000.00000|"
				   + SaveRigidbody(null) + "subGO.activeSelf:0";
		}

		StringBuilder s1 = new StringBuilder();
        s1.Clear();
        s1.Append(SaveTransform(subGO.transform));
		s1.Append(splitChar);
		s1.Append(SaveRigidbody(subGO));
		s1.Append(splitChar);
		s1.Append(BoolToString(subGO.activeSelf,"subGO.activeSelf"));
		return s1.ToString();
	}

	public static int LoadSubActivatedGOState(GameObject subGO,
											 ref string[] entries, int index) {

		index = LoadTransform(subGO.transform,ref entries,index); // 10
		index = LoadRigidbody(subGO,ref entries,index); // 4
		subGO.SetActive(GetBoolFromString(entries[index],"subGO.activeSelf"));
		index++;
		return index; // 10+4+1=15
	}

	public static string SaveChildGOState(GameObject mainParent, int childex) {
			Transform childTR = mainParent.transform.GetChild(childex);
			return SaveSubActivatedGOState(childTR.gameObject);
	}

	public static string SaveCamera(GameObject go) {
		Camera cm = go.GetComponent<Camera>();
        Grayscale gsc = go.GetComponent<Grayscale>();
		if (cm == null) {
			Debug.Log("Camera missing on savetype of Camera! GameObject.name: "
					  + go.name);

			return "Camera.enabled:0|Grayscale.enabled:0";
		}

		string line = System.String.Empty;
        line = BoolToString(cm.enabled,"Camera.enabled"); // bool
		line += splitChar;
        if (gsc != null) line += BoolToString(gsc.enabled,"Grayscale.enabled");
        else line += "Grayscale.enabled:0";
		return line;
	}

	public static int LoadCamera(GameObject go,ref string[] entries,int index) {
		Camera cm = go.GetComponent<Camera>();
		Grayscale gsc = go.GetComponent<Grayscale>();
		if (cm == null || index < 0 || entries == null) return index + 2;

		cm.enabled = GetBoolFromString(entries[index],"Camera.enabled"); index++;
		if (gsc != null) {
			gsc.enabled = GetBoolFromString(entries[index],"Grayscale.enabled");
		}
		index++;
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

	public static void PlayOneShotSavable(AudioSource SFX, int fx) {
		if (fx < 0) return;
		if (fx >= Const.a.sounds.Length) return;

		PlayOneShotSavable(SFX,Const.a.sounds[fx],0);
	}

	public static void PlayOneShotSavable(AudioSource SFX, int fx, float vol) {
		if (fx < 0) return;
		if (fx >= Const.a.sounds.Length) return;

		PlayOneShotSavable(SFX,Const.a.sounds[fx],vol);
	}
	
	public static void PlayUIOneShotSavable(int fx, float vol) {
		if (fx < 0) return;
		if (fx >= Const.a.sounds.Length) return;

		AudioSource aud = null;
		for (int i=0;i<MFDManager.a.UIAudSource.Length;i++) { 
			if (MFDManager.a.UIAudSource[i].isPlaying) continue;
			
			aud = MFDManager.a.UIAudSource[i]; // Free channel.
		}
		
		if (aud == null) return; // Couldn't find a free channel.
		
		PlayOneShotSavable(aud,Const.a.sounds[fx],vol);
	}
	
	public static void PlayUIOneShotSavable(int fx) {
		PlayUIOneShotSavable(fx,1.0f);
	}
	
	public static void PlayUIOneShotSavable(AudioClip fxclip) {
		AudioSource aud = null;
		for (int i=0;i<MFDManager.a.UIAudSource.Length;i++) { 
			if (MFDManager.a.UIAudSource[i].isPlaying) continue;
			
			aud = MFDManager.a.UIAudSource[i]; // Free channel.
		}
		
		if (aud == null) return; // Couldn't find a free channel.
		
		PlayOneShotSavable(aud,fxclip,0f);
	}

	public static void PlaySavable(AudioSource SFX, AudioClip fxclip) {
		PlayAudioSavable(SFX,fxclip,0,false);
	}

	public static void PlaySavable(AudioSource SFX, int fxclip) {
		PlayAudioSavable(SFX,Const.a.sounds[fxclip],0,false);
	}

	public static string SaveAudioSource(GameObject go) {
		AudioSource aus = go.GetComponent<AudioSource>();
		string line = System.String.Empty;
        line = BoolToString(aus.enabled,"AudioSource.enabled");
		line += splitChar + FloatToString(aus.time,"time");
		if (aus.clip == null) {
		    line += splitChar + SaveString("none","clip.name");
		} else line += splitChar + SaveString(aus.clip.name,"clip.name");

		return line;
	}

	public static int LoadAudioSource(GameObject go, ref string[] entries,
	                                  int index) {
	                                      
		AudioSource aus = go.GetComponent<AudioSource>();
		if (aus == null || index < 0 || entries == null) return index + 3;

		aus.enabled = GetBoolFromString(entries[index],"AudioSource.enabled");
		index++;
		
		aus.time = GetFloatFromString(entries[index],"time");
		index++;
		
		// clip.name skipped, present for debugging
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
    public static string SaveRelativeTimeDifferential(float timerValue,
													  string name) {

        if (PauseScript.a == null) return name + ":0000.00000";
        float val = timerValue - PauseScript.a.relativeTime; // Remove current
                                                             // instance's
                                                             // relative time.
        return FloatToString(val,name);
    }

    public static float LoadRelativeTimeDifferential(string savedTimer,
													 string name) {

        float val = GetFloatFromString(savedTimer,name);
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
	
	public static void SafeDestroyAllChildren(Transform parent) {
		for (int i=0;i<parent.childCount;i++) {
			SafeDestroy(parent.GetChild(i).gameObject);
		}
	}

	public static void SafeDestroyImmediateAllChildren(Transform parent) {
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
											  float radius,float impactScale) {
		HealthManager hm = null;
		Collider[] colliders = Physics.OverlapSphere(centerPoint,radius);
		int i = 0;
		while (i < colliders.Length) {
			GameObject go = colliders[i].gameObject;
			if (go == null) { i++; continue; }
			//if (go.isStatic) { i++; continue; } EDITOR ONLY!!!!!!

			Rigidbody rbody = go.GetComponent<Rigidbody>();
			dd.impactVelocity = dd.damage * impactScale;
			Vector3 dir = go.transform.position - centerPoint;
			RaycastHit hit;
			float dist = Vector3.Distance(centerPoint, go.transform.position);
			bool success = (dist < 1.5f); // So close we should just shove it.
			bool applyImpact = false;
			if (!success) {
				success = Physics.Raycast(centerPoint,dir,out hit,
										  radius + 0.02f,
										  Const.a.layerMaskExplosion);
				dist = hit.distance;
				applyImpact = (hit.collider == colliders[i]
							   || (hit.rigidbody == rbody && rbody != null));
			} else applyImpact = true;

			if (success) {
				hm = Utils.GetMainHealthManager(go);
				if (hm != null) {
					if (hm.isPlayer) {
						dd.damage = dd.damage * 0.5f; // give em a chance mate
					}

					float distPenalty = (radius - dist) / radius;
					float saturation = dd.damage * 0.33f; // minimum damage in
														  // range to feel more
														  // powerful/useful
					dd.damage *= distPenalty; // Linear falloff
					if (dd.damage < saturation) dd.damage = saturation;
					hm.TakeDamage(dd);
				}

				if (applyImpact && rbody != null) {
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

	public static HealthManager GetMainHealthManager(GameObject go) {
		if (go == null) return null;

		HealthManager retval = null;
		retval = go.GetComponent<HealthManager>();
		if (retval != null) return retval;

		// For hopper joint collisions or other combo-collider setups.
		HealthManagerRelay hmr = go.GetComponent<HealthManagerRelay>();
		if (hmr != null) retval = hmr.healthManagerToRedirectTo;
		return retval;
	}

	public static HealthManager GetMainHealthManager(RaycastHit hit) {
		if (hit.collider == null && hit.transform == null) return null;

		HealthManager retval = null;
		HealthManagerRelay hmr = null;
		GameObject colGO = null;
		GameObject hitGO = null;

		// Check compound collider's main parent first.  Thanks andeeee!!
		if (hit.collider != null) colGO = hit.collider.transform.gameObject;

		if (colGO != null) hmr = colGO.GetComponent<HealthManagerRelay>();
		hitGO = hit.transform.gameObject; // The actual hit object.
		if (colGO != null) retval = colGO.GetComponent<HealthManager>();
		if (retval != null) return retval;
		if (hitGO == null) return null;

		retval = hitGO.GetComponent<HealthManager>();
		if (retval == null) {
			// For hopper joint collisions or other combo-collider setups.
			if (hmr == null) hmr = colGO.GetComponent<HealthManagerRelay>();
			if (hmr != null) {
				retval = hmr.healthManagerToRedirectTo;
			}
		}

		return retval;
	}

    public static void CopyLogFiles(bool prev) {
        #if UNITY_EDITOR
            if (prev) return; // Just the one log in Editor (e.g. tests)
        #endif
        
        string logFilePath = GetLogFilePath(prev);
        string logname = "/Player.log";
        if (prev) logname = "/Player-prev.log";
        string destinationPath = Application.streamingAssetsPath + logname;

        if (File.Exists(logFilePath)) {
            // Delete the existing destination log file if it already exists
            if (File.Exists(destinationPath)) {
                File.Delete(destinationPath);
            }

            // Copy the log file to the StreamingAssets folder
            File.Copy(logFilePath, destinationPath);
            Debug.Log("Log file copied to StreamingAssets folder.");
        } else {
            Debug.LogWarning("Log file not found, hmmm");
        }
    }

    private static string GetLogFilePath(bool prev) {
        string logFilePath = "";
		#if UNITY_EDITOR
			// Editor log doesn't need specified.
		#else
			string logname = "/Player.log";
			if (prev) logname = "/Player-prev.log";
		#endif

        #if UNITY_EDITOR
            // Editor log file path
            logFilePath = Application.consoleLogPath;
        #elif UNITY_ANDROID
            // Android log file path
            logFilePath = Application.persistentDataPath + logname;
        #elif UNITY_IOS
            // iOS log file path
            logFilePath = Application.temporaryCachePath + logname;
        #else
            // Standalone platforms (Windows, macOS, etc.)
            logFilePath = Application.persistentDataPath + logname;
        #endif

        return logFilePath;
    }
    
    public static GameObject CreateSEGIEmitter(GameObject go, int curlevel, int lineNum, Light lit) {
		GameObject segiEmitter = new GameObject("SEGIEmitter" + curlevel.ToString() + "." + lineNum.ToString());
        segiEmitter.transform.parent = go.transform;
        segiEmitter.transform.localPosition = new Vector3(0f,0f,0f);
        MeshFilter mf = segiEmitter.AddComponent<MeshFilter>();
        mf.sharedMesh = Const.a.sphereMesh;
        MeshRenderer mR = segiEmitter.AddComponent<MeshRenderer>();
        mR.material = Const.a.segiEmitterMaterial1;
        mR.material.SetColor("_EmissionColor",new Color(lit.color.r * lit.intensity,lit.color.g * lit.intensity,lit.color.b * lit.intensity,1f));
        segiEmitter.transform.localScale = new Vector3(Mathf.Max(lit.range * Const.segiVoxelSize,8f),Mathf.Max(lit.range * Const.segiVoxelSize,8f),Mathf.Max(lit.range * Const.segiVoxelSize,8f));
        segiEmitter.layer = 2; // IgnoreRaycast
        return segiEmitter;
	}
	
	// Allows for checking if a value given is within the tolerance of a comparison value.
    public static bool InTol(float inVal, float compareVal, float epsilon) {
        return ((inVal > (compareVal - epsilon)) && (inVal < (compareVal + epsilon)));
    }
    
    // Calculate the nearest center for x and z based on the grid size of 2.56
    // Keeping y as is since it's not grid-bound (could be fractional grid in increments of 0.16f or similar).
    public static Vector3 GetCellCenter(Vector3 pos) {
        return new Vector3(Mathf.Round(pos.x / 2.56f) * 2.56f,
                           pos.y,
                           Mathf.Round(pos.z / 2.56f) * 2.56f);
    }
    
    public static bool IsAxisAligned(Quaternion quat) {
        Vector3 euangs = quat.eulerAngles;
        euangs = new Vector3(Mathf.Abs(euangs.x) % 360f, Mathf.Abs(euangs.y) % 360f, Mathf.Abs(euangs.z) % 360f);
        bool xIs90, yIs90, zIs90;
        xIs90 = yIs90 = zIs90 = false;
        float tol = 0.5f; // Must be positive tolerance!  This is degrees.
        if (Utils.InTol(euangs.x,0f,tol) || Utils.InTol(euangs.x,90f,tol) || Utils.InTol(euangs.x,180f,tol) || Utils.InTol(euangs.x,270f,tol) || Utils.InTol(euangs.x,360f,tol)) xIs90 = true;
        if (Utils.InTol(euangs.y,0f,tol) || Utils.InTol(euangs.y,90f,tol) || Utils.InTol(euangs.y,180f,tol) || Utils.InTol(euangs.y,270f,tol) || Utils.InTol(euangs.y,360f,tol)) yIs90 = true;
        if (Utils.InTol(euangs.z,0f,tol) || Utils.InTol(euangs.z,90f,tol) || Utils.InTol(euangs.z,180f,tol) || Utils.InTol(euangs.z,270f,tol) || Utils.InTol(euangs.z,360f,tol)) zIs90 = true;
        return (xIs90 && yIs90 && zIs90);
    }

    public static float GetFloorHeight(Quaternion quat, float yHeight) {
        Vector3 euangs = quat.eulerAngles;
        euangs = new Vector3(Mathf.Abs(euangs.x) % 360f, Mathf.Abs(euangs.y) % 360f, Mathf.Abs(euangs.z) % 360f);
        if (QuaternionApproximatelyEquals(quat,Quaternion.Euler(180f,0f,0f),30f)) return yHeight;
        else return -1300f;
	}
    
    public static bool QuaternionApproximatelyEquals(Quaternion quat, Quaternion other, float toleranceDeg) {
        float angle = Quaternion.Angle(quat, other); // Quaternion.Angle is in degrees.
        
        // Check if the angle between the quaternions is less than or equal to the tolerance
        return angle <= toleranceDeg;
    }
    
    // Magic numbers corresponding to 45deg somehow from the sin/cos of
	// radians of pi/4 best I can tell.  These are just what are saved
	// from ToString() on quaternion transform.rotation channels.
	static float twentySevenths = 0.27060f;
	static float sixtyFifths    = 0.65328f;
    
    public static bool ChunkIs45NW_NE_SW_SE_Laterally(Quaternion quat) {
		int count27 = 0;
		int count65 = 0;
		foreach (var component in new float[] { quat.x, quat.y, quat.z, quat.w }) {
			if (Mathf.Abs(Mathf.Abs(component) - twentySevenths) < 0.01f) count27++;
			else if (Mathf.Abs(Mathf.Abs(component) - sixtyFifths) < 0.01f) count65++;
		}
		
		// Check if we have exactly 2 of each value type for a 45-degree rotation
		// Seems this is the magic incantation to get any 45deg as long as
		// any two of the quaternion channels are one magic number and the
		// other two are tother.
		return (count27 == 2 && count65 == 2);
	}
	
	#if UNITY_EDITOR
		public static List<GameObject> GetAllObjectsOnlyInScene() {
			List<GameObject> objectsInScene = new List<GameObject>();
			foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[]) {
				if (!EditorUtility.IsPersistent(go.transform.root.gameObject)
					&& !(go.hideFlags == HideFlags.NotEditable
						|| go.hideFlags == HideFlags.HideAndDontSave)) objectsInScene.Add(go);
			}

			return objectsInScene;
		}
	#endif
}
