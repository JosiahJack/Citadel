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
	public PuzzleWire.WireColorTheme theme;
	public PuzzleWire.WireColor[] wireColors;
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
			Debug.Log("BUG: no TargetIO.cs found on an object with a PuzzleGridPuzzle.cs script!  Trying to call Use without parameters!");
		}

		Const.sprint(Const.a.stringTable[190],ud.owner); //Puzzle interface accessed
		inUse = true;
		MFDManager.a.SendWirePuzzleToDataTab(wiresOn, rowsActive, currentPositionsLeft, currentPositionsRight, solutionPositionsLeft, solutionPositionsRight, theme, wireColors, target, argvalue, ud,transform.position,this);
	}

	public void UseTargets (GameObject owner) {
		UseData ud = new UseData();
		ud.owner = owner;
		ud.argvalue = argvalue;
		TargetIO tio = GetComponent<TargetIO>();
		if (tio != null) {
			ud.SetBits(tio);
		} else {
			Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
		}
		Const.a.UseTargets(ud,target);
		Const.sprintByIndexOrOverride (successMessageLingdex, successMessage,ud.owner);
	}
}
