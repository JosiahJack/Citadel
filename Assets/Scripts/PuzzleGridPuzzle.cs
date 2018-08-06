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
	public GameObject target;

	void Use (UseData ud) {
		if (dead) {
			Const.sprint("Can't use broken panel",ud.owner);
			return;
		}

		if (LevelManager.a.levelSecurity[LevelManager.a.currentLevel] > securityThreshhold) {
			MFDManager.a.BlockedBySecurity();
			return;
		}

		Const.sprint("Puzzle interface accessed",ud.owner);
		//(bool[] states, CellType[] types, GridType gtype, int start, int end, GridColorTheme colors, UseData ud)
		MFDManager.a.SendGridPuzzleToDataTab(grid,cellType,gridType,sourceIndex,outputIndex,width,height,theme,target,ud);
	}
}
