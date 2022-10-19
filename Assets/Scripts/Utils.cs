using System;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

// Globally accessible utility functions for parsing values, generating save
// strings, converting enumerated and integer values, and other helpful things.
public class Utils {
	public static string splitChar = "|";
	public static CultureInfo en_US_Culture = new CultureInfo("en-US");

	private static bool getValparsed;
	private static int getValreadInt;
	private static float getValreadFloat;

	public static int SafeIndex(ref int[] array, int index, int max, int failValue) {
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

    // This isn't used anywhere at the moment but is handy to have as a note.
    //public static int GetNPCConstIndexFromIndex23(int ref23) {
	//	switch (ref23) {
	//		case 0: return 160; // Autobomb
	//		case 1: return 161; // Cyborg Assassin
	//		case 2: return 162; // Avian Mutant
	//		case 3: return 163; // Exec Bot
	//		case 4: return 164; // Cyborg Drone
	//		case 5: return 165; // Cortex Reaver
	//		case 6: return 166; // Cyborg Warrior
	//		case 7: return 167; // Cyborg Enforcer
	//		case 8: return 168; // Cyborg Elite Guard
	//		case 9: return 169; // Cyborg of Edward Diego
	//		case 10: return 170; // Security-1 Robot
	//		case 11: return 171; // Security-2 Robot
	//		case 12: return 172; // Maintenance Bot
	//		case 13: return 173; // Mutated Cyborg
	//		case 14: return 174; // Hopper
	//		case 15: return 175; // Humanoid Mutant
	//		case 16: return 176; // Invisible Mutant
	//		case 17: return 177; // Virus Mutant
	//		case 18: return 178; // Serv-Bot
	//		case 19: return 179; // Flier Bot
	//		case 20: return 180; // Zero-G Mutant
	//		case 21: return 181; // Gorilla Tiger Mutant
	//		case 22: return 182; // Repair Bot
	//		case 23: return 183; // Plant Mutant
	//	}
	//	return -1;
	//}

	//public static float AngleInDeg(Vector3 vec1, Vector3 vec2) {
    //    return ((Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x))
    //            * (180 / Mathf.PI));
    //}

	// Check if particular bit is 1 (ON/TRUE) in binary format of given integer
	public static bool CheckFlags (int checkInt, int flag) {
        return ((checkInt & flag) != 0);
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

	public static AttackType GetAttackTypeFromInt(int att_type_i) {
		switch(att_type_i) {
			case 0: return AttackType.None;
			case 1: return AttackType.Melee;
			case 6: return AttackType.MeleeEnergy;
			case 2: return AttackType.EnergyBeam;
			case 3: return AttackType.Magnetic;
			case 4: return AttackType.Projectile;
			case 5: return AttackType.ProjectileEnergyBeam;
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

    public static string SaveTransform(Transform tr) {
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

    public static string SaveRigidbody(Rigidbody rbody) {
        if (rbody == null) return "0000.00000|0000.00000|0000.00000|1";
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
}