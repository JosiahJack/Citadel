using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGridPuzzle : MonoBehaviour {
	public int securityThreshhold = 100; // If security level is not below this
										 // level, this is unusable.
	public bool dead = false;
	public bool[] grid; // save
	public PuzzleCellType[] cellType;
	public PuzzleGridType gridType;
	public int sourceIndex;
	public int outputIndex;
	public int width;
	public int height;
	public HUDColor theme;
	public string target;
	public string argvalue;
	public bool locked = false; // save
	public int successMessageLingdex = 4;
	public int messageOnLockedLingdex = 302;
	public int messageOnBrokenLingdex = 189;
	public int alreadyFiredMessageLingdex = 312;
	public bool puzzleSolved; // save
	public bool onlyFireOnce = true;
	public bool animate = true;
	public bool inUse = false;

	[HideInInspector] public bool fired = false; // save
	private Animator anim;
	private bool alreadyOpen = false;

	void Awake() {
		puzzleSolved = false;
		if (animate) {
			anim = GetComponent<Animator>();
			if (anim == null) {
				Debug.Log("BUG: Puzzle panel has no animator on "
						  + "PuzzleGridPuzzle.cs");
			}

			alreadyOpen = false;
		}
	}

	public void SendDataBackToPanel(PuzzleGrid pg) {
		grid = pg.grid;
	}

	public void Use (UseData ud) {
		if (dead) {
			Const.sprint(messageOnBrokenLingdex);
			return;
		}

		if (LevelManager.a.GetCurrentLevelSecurity() > securityThreshhold) {
			MFDManager.a.BlockedBySecurity(transform.position);
			return;
		}

		if (LevelManager.a.superoverride || Const.a.difficultyMission == 0) {
			// SHODAN can go anywhere!  Full security override!
			locked = false;
		}

		if (locked) {
			Const.sprint(messageOnLockedLingdex);
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

		Const.sprint(Const.a.stringTable[190],ud.owner); // Puzzle accessed
		inUse = true;
		if (animate && anim != null && !alreadyOpen) {
			anim.Play("Open");
			alreadyOpen = true;
		}

		MFDManager.a.SendGridPuzzleToDataTab(grid,cellType,gridType,
											 sourceIndex,outputIndex,width,
											 height,theme,target,ud,
											 transform.position,this);
	}

	public void UseTargets (GameObject owner) {
		if (onlyFireOnce && fired) {
			Const.sprint(alreadyFiredMessageLingdex);
			return;
		}

		if (onlyFireOnce) fired = true;
		UseData ud = new UseData();
		ud.owner = owner;
		ud.argvalue = argvalue;
		Const.a.UseTargets(gameObject,ud,target);
		Const.sprint(successMessageLingdex);
	}

	public static string Save(GameObject go) {
		PuzzleGridPuzzle pgp = go.GetComponent<PuzzleGridPuzzle>();
		string line = System.String.Empty;
		line = Utils.BoolToString(pgp.puzzleSolved,"puzzleSolved");
		for (int i=0;i<35;i++) {
			line += Utils.splitChar + Utils.BoolToString(pgp.grid[i],"grid[" + i.ToString() + "]");
		}
		line += Utils.splitChar + Utils.BoolToString(pgp.fired,"fired");
		line += Utils.splitChar + Utils.BoolToString(pgp.locked,"locked");
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		PuzzleGridPuzzle pgp = go.GetComponent<PuzzleGridPuzzle>();
		pgp.puzzleSolved = Utils.GetBoolFromString(entries[index],"puzzleSolved"); index++;
		for (int i=0;i<pgp.grid.Length;i++) {
			pgp.grid[i] = Utils.GetBoolFromString(entries[index],"grid[" + i.ToString() + "]"); index++;
		}

		pgp.fired = Utils.GetBoolFromString(entries[index],"fired"); index++;
		pgp.locked = Utils.GetBoolFromString(entries[index],"locked"); index++;
		return index;
	}
}
