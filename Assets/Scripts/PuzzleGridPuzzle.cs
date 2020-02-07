using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGridPuzzle : MonoBehaviour {
	public int securityThreshhold = 100; // if security level is not below this level, this is unusable
	public bool dead = false;
	public bool[] grid;
	public PuzzleGrid.CellType[] cellType;
	public PuzzleGrid.GridType gridType;
	public int sourceIndex;
	public int outputIndex;
	public int width;
	public int height;
	public PuzzleGrid.GridColorTheme theme;
	public string target;
	public string argvalue;
	public bool locked = false;
	public string messageOnLocked = "Access Panel is Locked";
	public string messageOnBroken = "Can't use broken panel";

	private Animator anim;
	public bool animate = true;
	public bool inUse = false;
	private bool alreadyOpen = false;

	void Awake() {
		if (animate) {
			anim = GetComponent<Animator>();
			if (anim == null) Debug.Log("BUG: Puzzle panel has no animator on PuzzleGridPuzzle.cs");
			alreadyOpen = false;
		}
	}

	public void SendDataBackToPanel(PuzzleGrid pg) {
		grid = pg.grid;
	}

	public void Use (UseData ud) {
		if (dead) {
			Const.sprint(messageOnBroken,ud.owner);
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
			Const.sprint(messageOnLocked,ud.owner);
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
		//(bool[] states, CellType[] types, GridType gtype, int start, int end, GridColorTheme colors, UseData ud)
		inUse = true;
		if (animate && anim != null && !alreadyOpen) { anim.Play("Open"); alreadyOpen = true; }
		MFDManager.a.SendGridPuzzleToDataTab(grid,cellType,gridType,sourceIndex,outputIndex,width,height,theme,target,ud,transform.position);
	}
}
