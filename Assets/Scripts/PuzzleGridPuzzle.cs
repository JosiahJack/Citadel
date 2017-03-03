using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGridPuzzle : MonoBehaviour {
	public bool[] grid;
	public PuzzleGrid.CellType[] cellType;
	public PuzzleGrid.GridType gridType;
	public int sourceIndex;
	public int outputIndex;
	public int width;
	public int height;
	public PuzzleGrid.GridColorTheme theme;

	void Use () {
		Const.sprint("Puzzle activated");
		//(bool[] states, CellType[] types, GridType gtype, int start, int end, GridColorTheme colors)
		MFDManager.a.SendGridPuzzleToDataTab(grid,cellType,gridType,sourceIndex,outputIndex,width,height,theme);
	}
}
