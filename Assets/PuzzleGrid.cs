using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGrid : MonoBehaviour {
	public bool[] grid;
	public bool[] special;
	public bool[] isOn;
	public enum GridType {King,Queen,Knight,Rook,Bishop,Pawn};
	public GridType gridType;
	public int width = 7;
	public int height = 5;
	public Button[] gridCells;
	public Sprite gridPlus;
	public Sprite gridX;
	public Sprite gridNull;
	public Sprite gridSpecial;
	public Sprite gridSpecialOn0;
	public Sprite gridSpecialOn1;

	public void Update() {
		for (int i=0;i<(width*height);i++) {
			if (i < gridCells.Length) {
				if (isOn[i]) {
					if (!special[i]) {
						if (gridCells[i]) {
							gridCells[i].image.overrideSprite = gridX;
						} else {
							gridCells[i].image.overrideSprite = gridPlus;
						}
					} else {
						gridCells[i].image.overrideSprite = gridSpecial;
					}
				} else {
					gridCells[i].image.overrideSprite = gridNull;
				}
			}
		}
	}

	public void OnGridCellClick (int index) {
		switch (gridType) {
		case GridType.King: King(index); break;
		case GridType.Queen: Queen(index); break;
		case GridType.Knight: Knight(index); break;
		case GridType.Rook: Rook(index); break;
		case GridType.Bishop: Bishop(index); break;
		case GridType.Pawn: Pawn(index); break;
		}
	}

	int ReturnCellAbove(int index) {
		int retval = -1;
		bool outOfBounds = ((index-width) < 0) ? true: false;
		if (!outOfBounds) retval = (index-width); // return cell above if not on top row
		return retval;
	}

	int ReturnCellBelow(int index) {
		int retval = -1;
		bool outOfBounds = ((index+width) > ((width*height)-1)) ? true: false;
		if (!outOfBounds) retval = (index+width); // return cell below if not on bottom row
		return retval;
	}

	int ReturnCellToLeft(int index) {
		int retval = -1;
		if (!((index/width) > 0 && (index%width < 1))) retval = index-1;  // return index to left if not in far left column
		return retval;
	}

	int ReturnCellToRight(int index) {
		int retval = -1;
		if (!(((index+1)/width) > 0 && ((index+1)%width < 1))) retval = index+1;  // return index to right if not in far right column
		return retval;
	}

	// Only flip clicked cell
	void Pawn(int index) {
		if (isOn[index] && !special[index])
			grid[index] = !grid[index]; // FLip clicked cell
	}

	// Flip vertically and horizontally adjacent cells along with center
	void King(int index) {
		if (!isOn[index] || special[index]) return;

		grid[index] = !grid[index]; // Flip clicked cell
		int cellAbove = ReturnCellAbove(index);
		int cellBelow = ReturnCellBelow(index);
		int cellLeft = ReturnCellToLeft(index);
		int cellRight = ReturnCellToRight(index);
		grid[cellAbove] = !grid[cellAbove];
		grid[cellBelow] = !grid[cellBelow];
		grid[cellLeft] = !grid[cellLeft];
		grid[cellRight] = !grid[cellRight];
	}

	// Flip vertically, horizontally, and diagonally adjacent cells across entire puzzle along with center
	void Queen(int index) {
		if (!isOn[index] || special[index]) return;

	}

	// Flip corners along with center
	void Knight(int index) {
		if (!isOn[index] || special[index]) return;

	}

	// Flip vertically and horizontally adjacent cells across entire puzzle along with center
	void Rook(int index) {
		if (!isOn[index] || special[index]) return;

	}

	// Flip diagonally adjacent cells across entire puzzle along with center
	void Bishop(int index) {
		if (!isOn[index] || special[index]) return;

	}
}
