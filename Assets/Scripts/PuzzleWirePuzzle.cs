using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleWirePuzzle : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	public bool dead = false;
	public bool[] wiresOn;
	public bool[] rowsActive;
	public int[] currentPositionsLeft;
	public int[] currentPositionsRight;
	public int[] solutionPositionsLeft;
	public int[] solutionPositionsRight;
	public PuzzleWire.WireColorTheme theme;
	public PuzzleWire.WireColor[] wireColors;
	public string target;
	public string argvalue;
	public bool locked = false;

	private Animator anim;
	public bool animate = true;
	public bool inUse = false;

	void Awake() {
		if (animate) {
			anim = GetComponent<Animator>();
			if (anim == null) Debug.Log("BUG: Puzzle panel has no animator on PuzzleGridPuzzle.cs");
		}
	}

	public void SendDataBackToPanel(PuzzleWire pw) {
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
	}

	public void Use (UseData ud) {
		if (dead) {
			Const.sprint("Can't use broken panel",ud.owner);
			return;
		}

		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			MFDManager.a.BlockedBySecurity(transform.position);
			return;
		}

		if (LevelManager.a.superoverride) {
			// SHODAN can go anywhere!  Full security override!
			locked = false;
		}

		if (locked) {
			Const.sprint("Access Panel is Locked",ud.owner);
			return;
		}

		ud.argvalue = argvalue;
		TargetIO tio = GetComponent<TargetIO>();
		if (tio != null) {
			ud.SetBits(tio);
		} else {
			Debug.Log("BUG: no TargetIO.cs found on an object with a PuzzleGridPuzzle.cs script!  Trying to call Use without parameters!");
		}

		Const.sprint("Puzzle interface accessed",ud.owner);
		inUse = true;
		MFDManager.a.SendWirePuzzleToDataTab(wiresOn, rowsActive, currentPositionsLeft, currentPositionsRight, solutionPositionsLeft, solutionPositionsRight, theme, wireColors, target, argvalue, ud,transform.position);
	}
}
