﻿using System.Collections;
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
	public string messageOnLocked;
	public int successMessageLingdex = 4;
	public string successMessage;
	public int messageOnLockedLingdex = 302;
	public string messageOnBroken;
	public int messageOnBrokenLingdex = 189;
	public string alreadyFiredMessage;
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
			Const.sprintByIndexOrOverride(messageOnBrokenLingdex,
										  messageOnBroken,ud.owner);
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
			Const.sprintByIndexOrOverride(messageOnLockedLingdex,
										  messageOnLocked,ud.owner);
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
			Const.sprintByIndexOrOverride(alreadyFiredMessageLingdex,
										  alreadyFiredMessage,owner);
			return;
		}

		if (onlyFireOnce) fired = true;
		UseData ud = new UseData();
		ud.owner = owner;
		ud.argvalue = argvalue;
		Const.a.UseTargets(gameObject,ud,target);
		Const.sprintByIndexOrOverride (successMessageLingdex,
									   successMessage,ud.owner);
	}

	public static string Save(GameObject go) {
		PuzzleGridPuzzle pgp = go.GetComponent<PuzzleGridPuzzle>();
		if (pgp == null) {
			Debug.Log("PuzzleGridData missing on savetype of PuzzleGrid!  "
					  + "GameObject.name: " + go.name);

			return "0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|"
				   + "0|0|0|0|0|0|0|0|0";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(pgp.puzzleSolved);
		for (int i=0;i<35;i++) {
			line += Utils.splitChar + Utils.BoolToString(pgp.grid[i]);
		}
		line += Utils.splitChar + Utils.BoolToString(pgp.fired);
		line += Utils.splitChar + Utils.BoolToString(pgp.locked);
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		PuzzleGridPuzzle pgp = go.GetComponent<PuzzleGridPuzzle>();
		if (pgp == null) {
			Debug.Log("PuzzleGridPuzzle.Load failure, pgp == null");
			return index + 38;
		}

		if (index < 0) {
			Debug.Log("PuzzleGridPuzzle.Load failure, index < 0");
			return index + 38;
		}

		if (entries == null) {
			Debug.Log("PuzzleGridPuzzle.Load failure, entries == null");
			return index + 38;
		}

		pgp.puzzleSolved = Utils.GetBoolFromString(entries[index]); index++;
		for (int i=0;i<pgp.grid.Length;i++) {
			pgp.grid[i] = Utils.GetBoolFromString(entries[index]); index++;
		}

		pgp.fired = Utils.GetBoolFromString(entries[index]); index++;
		pgp.locked = Utils.GetBoolFromString(entries[index]); index++;
		return index;
	}
}
