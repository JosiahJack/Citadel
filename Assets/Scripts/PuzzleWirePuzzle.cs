using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleWirePuzzle : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	public bool dead = false;
	public bool[] wiresOn;
	public bool[] rowsActive;
	public int[] currentPositionsLeft; // save
	public int[] currentPositionsRight; // save
	public int[] solutionPositionsLeft;
	public int[] solutionPositionsRight;
	public HUDColor theme;
	public HUDColor[] wireColors;
	public string target;
	public string argvalue;
	public bool locked = false; // save
	public int successMessageLingdex = 4;
	public string successMessage = "";
	public string messageOnLocked;
	public int messageOnLockedLingdex = 302;
	public string messageOnBroken;
	public int messageOnBrokenLingdex = 189;
	public bool puzzleSolved; // save

	private Animator anim;
	public bool animate = true;
	public bool inUse = false;

	void Awake() {
		puzzleSolved = false;
		if (animate) {
			anim = GetComponent<Animator>();
			if (anim == null) Debug.Log("BUG: Puzzle panel has no animator on PuzzleGridPuzzle.cs");
		}
	}

	public void SendDataBackToPanel(PuzzleWire pw, bool stillInUse) {
		currentPositionsLeft[0] = pw.wire1LHPosition;
		currentPositionsLeft[1] = pw.wire2LHPosition;
		currentPositionsLeft[2] = pw.wire3LHPosition;
		currentPositionsLeft[3] = pw.wire4LHPosition;
		currentPositionsLeft[4] = pw.wire5LHPosition;
		currentPositionsLeft[5] = pw.wire6LHPosition;
		currentPositionsLeft[6] = pw.wire7LHPosition;
		currentPositionsRight[0] = pw.wire1RHPosition;
		currentPositionsRight[1] = pw.wire2RHPosition;
		currentPositionsRight[2] = pw.wire3RHPosition;
		currentPositionsRight[3] = pw.wire4RHPosition;
		currentPositionsRight[4] = pw.wire5RHPosition;
		currentPositionsRight[5] = pw.wire6RHPosition;
		currentPositionsRight[6] = pw.wire7RHPosition;
		inUse = stillInUse;
	}

	public void Use (UseData ud) {
		if (dead) {
			Const.sprintByIndexOrOverride (messageOnBrokenLingdex, messageOnBroken,ud.owner);
			return;
		}

		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			MFDManager.a.BlockedBySecurity(transform.position,ud);
			return;
		}

		if (LevelManager.a.superoverride || Const.a.difficultyMission == 0) {
			// SHODAN can go anywhere!  Full security override!
			locked = false;
		}

		if (locked) {
			Const.sprintByIndexOrOverride (messageOnLockedLingdex, messageOnLocked,ud.owner);
			return;
		}

		ud.argvalue = argvalue;
		TargetIO tio = GetComponent<TargetIO>();
		if (tio != null) {
			ud.SetBits(tio);
		} else {
			Debug.Log("BUG: no TargetIO.cs found on an object with a "
					  + "PuzzleGridPuzzle.cs script!  Trying to call Use "
					  + "without parameters!");
		}

		Const.sprint(Const.a.stringTable[190],ud.owner); //Puzzle accessed
		inUse = true;
		MFDManager.a.SendWirePuzzleToDataTab(wiresOn,rowsActive,
											 currentPositionsLeft,
											 currentPositionsRight,
											 solutionPositionsLeft,
											 solutionPositionsRight,theme,
											 wireColors,target,argvalue,ud,
											 transform.position,this);
	}

	public void UseTargets (GameObject owner) {
		UseData ud = new UseData();
		ud.owner = owner;
		ud.argvalue = argvalue;
		Const.a.UseTargets(gameObject,ud,target);
		Const.sprintByIndexOrOverride(successMessageLingdex,
									  successMessage,ud.owner);
	}

	public static string Save(GameObject go) {
		PuzzleWirePuzzle pwp = go.GetComponent<PuzzleWirePuzzle>();
		if (pwp == null) {
			Debug.Log("PuzzleWirePuzzle missing on savetype of PuzzleWire!  "
					  + "GameObject.name: " + go.name);

			return "0|0|1|2|3|4|5|6|0|1|2|3|4|5|6|0";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(pwp.puzzleSolved); // bool - is this puzzle already solved?
		for (int i=0;i<7;i++) { line += Utils.splitChar + pwp.currentPositionsLeft[i].ToString(); } // int - get the current wire positions
		for (int i=0;i<7;i++) { line += Utils.splitChar + pwp.currentPositionsRight[i].ToString(); } // int - get the current wire positions
		line += Utils.splitChar + Utils.BoolToString(pwp.locked); // bool - is this locked?
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		PuzzleWirePuzzle pwp = go.GetComponent<PuzzleWirePuzzle>();
		if (pwp == null) {
			Debug.Log("PuzzleWirePuzzle.Load failure, pwp == null");
			return index + 16;
		}

		if (index < 0) {
			Debug.Log("PuzzleWirePuzzle.Load failure, index < 0");
			return index + 16;
		}

		if (entries == null) {
			Debug.Log("PuzzleWirePuzzle.Load failure, entries == null");
			return index + 16;
		}

		pwp.puzzleSolved = Utils.GetBoolFromString(entries[index]); index++; // bool - is this puzzle already solved?
		for (int i=0;i<pwp.currentPositionsLeft.Length;i++) {
			pwp.currentPositionsLeft[i] = Utils.GetIntFromString(entries[index]); index++; // int - get the current wire positions
		}
		for (int i=0;i<pwp.currentPositionsRight.Length;i++) {
			pwp.currentPositionsRight[i] = Utils.GetIntFromString(entries[index]); index++;  // int - get the current wire positions
		}
		pwp.locked = Utils.GetBoolFromString(entries[index]); index++; // bool - is this locked?
		return index;
	}
}
