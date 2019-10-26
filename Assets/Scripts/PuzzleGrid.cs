using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGrid : MonoBehaviour {
	public bool[] powered;
	public CellType[] cellType;
	public enum CellType {Off,Standard,And,Bypass}; // off is blank, standard is X or +, AND takes two power sources, Bypass is always +
	public enum GridType {King,Queen,Knight,Rook,Bishop,Pawn};
	public GridType gridType;
	public int sourceIndex;
	public int outputIndex;
	public int width;
	public int height;
	public Button[] gridCells;
	public Sprite nodeOn;
	public Sprite gridPlus;
	public Sprite gridPlusgreen;
	public Sprite gridPluspurple;
	public Sprite gridX;
	public Sprite gridXgreen;
	public Sprite gridXpurple;
	public Sprite gridNull;
	public Sprite gridSpecial;
	public Sprite gridSpecialgreen;
	public Sprite gridSpecialpurple;
	public Sprite gridSpecialOn0;
	public Sprite gridSpecialOn0green;
	public Sprite gridSpecialOn0purple;
	public Sprite gridSpecialOn1;
	public Sprite gridPlusOn0;
	public Sprite gridPlusOn1;
	public Sprite gridXOn0;
	public Sprite gridXOn1;
	public Sprite gridAlwaysOn0;
	public Sprite gridAlwaysOn0green;
	public Sprite gridAlwaysOn0purple;
	public Sprite gridAlwaysOn1;
	public Image outputNode;
	public enum GridColorTheme {Gray,Green,Purple};
	public GridColorTheme theme;
	public AudioClip solvedSFX;
	public bool puzzleSolved;
	public Slider progressBar;

	private bool[] grid;
	private AudioSource audsource;
	private UseData udSender;
	[DTValidator.Optional] private GameObject[] targets;

	void Awake () {
		puzzleSolved = false;
		audsource = GetComponent<AudioSource>();
		EvaluatePuzzle();
		UpdateCellImages();
		targets = new GameObject[4];
	}

	public void SendGrid(bool[] states, CellType[] types, GridType gtype, int start, int end, int w, int h, GridColorTheme colors, GameObject senttarget, GameObject senttarget1, GameObject senttarget2, GameObject senttarget3, UseData ud) {
		grid = states;
		cellType = types;
		gridType = gtype;
		sourceIndex = start;
		outputIndex = end;
		width = w;
		height = h;
		theme = colors;
		targets[0] = senttarget;
		targets[1] = senttarget1;
		targets[2] = senttarget2;
		targets[3] = senttarget3;
		udSender = ud;
		EvaluatePuzzle();

		if (udSender.mainIndex == 54) {
			PuzzleSolved ();
		}

		UpdateCellImages();
	}

	void Update () {
		//EvaluatePuzzle(); not needed, useless activity
		UpdateCellImages();
	}

	public void OnGridCellClick (int index) {
		if (puzzleSolved) return;
		if (cellType[index] == CellType.Standard) {
			switch (gridType) {
				case GridType.King: King(index); break;
				case GridType.Queen: Queen(index); break;
				case GridType.Knight: Knight(index); break;
				case GridType.Rook: Rook(index); break;
				case GridType.Bishop: Bishop(index); break;
				case GridType.Pawn: Pawn(index); break;
			}
		}
		EvaluatePuzzle();
		UpdateCellImages();
	}

	public void UpdateCellImages() {
		for (int i=0;i<(width*height);i++) {
			if (cellType[i] != CellType.Off) {
				if (cellType[i] == CellType.Standard) {
					if (!grid[i]) {
						// Theme dependent
						switch (theme) {
						case GridColorTheme.Gray:
							gridCells [i].image.overrideSprite = gridX;
							break;
						case GridColorTheme.Green:
							gridCells [i].image.overrideSprite = gridXgreen;
							break;
						case GridColorTheme.Purple:
							gridCells [i].image.overrideSprite = gridXpurple;
							break;
						}
					} else {
						if (powered [i]) {
							gridCells [i].image.overrideSprite = gridPlusOn0; // powered node
							// TODO: handle different power images for lines between neighbors
						} else {
							// Theme dependent
							switch (theme) {
							case GridColorTheme.Gray:
								gridCells [i].image.overrideSprite = gridPlus;
								break;
							case GridColorTheme.Green:
								gridCells [i].image.overrideSprite = gridPlusgreen;
								break;
							case GridColorTheme.Purple:
								gridCells [i].image.overrideSprite = gridPluspurple;
								break;
							}
						}
					}
				}
				if (cellType[i] == CellType.And) {
					if (powered[i]) {
						// Theme dependent
						switch (theme) {
						case GridColorTheme.Gray:
							gridCells [i].image.overrideSprite = gridSpecialOn0;
							break;
						case GridColorTheme.Green:
							gridCells [i].image.overrideSprite = gridSpecialOn0green;
							break;
						case GridColorTheme.Purple:
							gridCells [i].image.overrideSprite = gridSpecialOn0purple;
							break;
						}
					} else {
						// Theme dependent
						switch (theme) {
						case GridColorTheme.Gray:
							gridCells [i].image.overrideSprite = gridSpecial;
							break;
						case GridColorTheme.Green:
							gridCells [i].image.overrideSprite = gridSpecialgreen;
							break;
						case GridColorTheme.Purple:
							gridCells [i].image.overrideSprite = gridSpecialpurple;
							break;
						}
					}
				}
				if (cellType[i] == CellType.Bypass) {
					if (powered[i]) {
						gridCells[i].image.overrideSprite = gridAlwaysOn1; // And node powered
					} else {
						// Theme dependent
						switch (theme) {
						case GridColorTheme.Gray:
							gridCells [i].image.overrideSprite = gridAlwaysOn0;
							break;
						case GridColorTheme.Green:
							gridCells [i].image.overrideSprite = gridAlwaysOn0green;
							break;
						case GridColorTheme.Purple:
							gridCells [i].image.overrideSprite = gridAlwaysOn0purple;
							break;
						}
					}
				}
			} else {
				gridCells[i].image.overrideSprite = gridNull;
			}
		}
	}

	public void EvaluatePuzzle() {
		List<int> queue = new List<int> ();
		bool[] checkedCells = new bool[width * height];
		int cellAbove;
		int cellBelow;
		int cellLeft;
		int cellRight;
		int movingIndex;

		// Reset the power
		for (int i = 0; i < (width * height); i++) {
			powered [i] = false;
		}

		// Set power for starting node
		if (grid.Length < 1)
			return;
		powered [sourceIndex] = grid [sourceIndex];  // Turn on power for cell adjacent to source node if it is a plus
		movingIndex = sourceIndex;
		if (!powered [movingIndex])
			return; // Source is off

		// Flow power to all nodes, adding any nodes to check to the queue
		queue.Add (sourceIndex); // Initialize queue
		while (queue.Count > 0) {
			movingIndex = queue [0]; // Get next item in the queue
			if (checkedCells [movingIndex]) {
				queue.Remove (movingIndex);
				continue;
			}
			//Const.sprint(("movingIndex = " + movingIndex.ToString()),Const.a.allPlayers);
			cellAbove = ReturnCellAbove (movingIndex);
			cellBelow = ReturnCellBelow (movingIndex);
			cellLeft = ReturnCellToLeft (movingIndex);
			cellRight = ReturnCellToRight (movingIndex);
			if (cellAbove != -1 && !checkedCells [cellAbove] && grid [cellAbove])
				queue.Add (cellAbove);
			if (cellBelow != -1 && !checkedCells [cellBelow] && grid [cellBelow])
				queue.Add (cellBelow);
			if (cellLeft != -1 && !checkedCells [cellLeft] && grid [cellLeft])
				queue.Add (cellLeft);
			if (cellRight != -1 && !checkedCells [cellRight] && grid [cellRight])
				queue.Add (cellRight);
			int poweredCount = 0;
			if (cellAbove != -1) {
				if (powered [cellAbove])
					poweredCount++;
			}
			if (cellBelow != -1) {
				if (powered [cellBelow])
					poweredCount++;
			}
			if (cellLeft != -1) {
				if (powered [cellLeft])
					poweredCount++;
			}
			if (cellRight != -1) {
				if (powered [cellRight])
					poweredCount++;
			}
			if (cellType [movingIndex] == CellType.And) {
				if (poweredCount > 1)
					powered [movingIndex] = true;
			} else {
				if (cellType [movingIndex] == CellType.Standard || cellType [movingIndex] == CellType.Bypass) {
					if (grid [movingIndex] && poweredCount > 0)
						powered [movingIndex] = true;
					Const.sprint (("movingIndex = " + movingIndex.ToString () + ", powered = " + powered [movingIndex].ToString ()), Const.a.allPlayers);
				}
			}
			checkedCells [movingIndex] = true;

			if (poweredCount > 1) {
				float percentProgress = 0f;
				int stepLeft = 0;
				int iteration = 0;
				for (int i = 0; i < grid.Length; i++) {
					if ((i == stepLeft || i == (stepLeft + width) || i == stepLeft + (width * 2)|| i == stepLeft + (width * 3)) && percentProgress < (iteration/7f)) {
						if (powered [i]) {
							percentProgress += (1f / 7f);
							iteration++;
							continue;
						}
						stepLeft++;
					}
				}
				progressBar.value = percentProgress;
			}
		}

		if (powered[outputIndex])
			PuzzleSolved(); // Latched solved state, no else statement to switch solved state back
	}

	void PuzzleSolved() {
		puzzleSolved = true;
		outputNode.overrideSprite = nodeOn;
		audsource.PlayOneShot(solvedSFX);
		Const.a.UseTargets(udSender,targets);
	}

	int ReturnCellAbove(int index) {
		int retval = -1;
		bool outOfBounds = ((index-width) < 0) ? true: false;
		if (!outOfBounds){
			if (cellType[index-width] != CellType.Off)
				retval = (index-width); // return cell above if not on top row
		}
		return retval;
	}

	int ReturnCellBelow(int index) {
		int retval = -1;
		bool outOfBounds = ((index+width) > ((width*height)-1)) ? true: false;
		if (!outOfBounds) {
			if (cellType[index+width] != CellType.Off)
				retval = (index+width); // return cell below if not on bottom row
		}
		return retval;
	}

	int ReturnCellToLeft(int index) {
		int retval = -1;
		if (!((index/width) > 0 && (index%width < 1))) {
			if ((index-1) >= 0) {
				if (cellType[index-1] != CellType.Off)
					retval = index-1;  // return index to left if not in far left column
			}
		}
		return retval;
	}

	int ReturnCellToRight(int index) {
		int retval = -1;
		if (!(((index+1)/width) > 0 && ((index+1)%width < 1))) {
			if ((index+1) < (width*height)) {
				if (cellType[index+1] != CellType.Off)
					retval = index+1;  // return index to right if not in far right column
			}
		} 
		return retval;
	}

	// Only flip clicked cell
	void Pawn(int index) {
		if (cellType[index] == CellType.Standard) grid[index] = !grid[index]; // FLip clicked cell if it is standard
	}

	// Flip vertically and horizontally adjacent standard cells along with center
	void King(int index) {
		grid[index] = !grid[index]; // Flip clicked cell
		int cellAbove = ReturnCellAbove(index);
		int cellBelow = ReturnCellBelow(index);
		int cellLeft = ReturnCellToLeft(index);
		int cellRight = ReturnCellToRight(index);
		if (cellAbove != -1 && cellType[cellAbove] == CellType.Standard) grid[cellAbove] = !grid[cellAbove];
		if (cellBelow != -1 && cellType[cellBelow] == CellType.Standard) grid[cellBelow] = !grid[cellBelow];
		if (cellLeft != -1 && cellType[cellLeft] == CellType.Standard) grid[cellLeft] = !grid[cellLeft];
		if (cellRight != -1 && cellType[cellRight] == CellType.Standard) grid[cellRight] = !grid[cellRight];
	}

	// Flip vertically, horizontally, and diagonally adjacent cells across entire puzzle along with center
	void Queen(int index) {

	}

	// Flip corners along with center
	void Knight(int index) {
	}

	// Flip vertically and horizontally adjacent cells across entire puzzle along with center
	void Rook(int index) {
		grid[index] = !grid[index]; // Flip clicked cell

		int tempIndex = index;
		for (int i=0;i<width;i++) {
			int cellAbove = ReturnCellAbove(tempIndex); // get next cell up

			if (cellAbove != -1 && cellType[cellAbove] == CellType.Standard)
				grid[cellAbove] = !grid[cellAbove];
			else
				break; // blocked by deactivated cells or special
			
			tempIndex = cellAbove; // move index up to cellAbove
		}

		tempIndex = index;
		int cellBelow = -1;
		for (int i=0;i<width;i++) {
			cellBelow = ReturnCellBelow(tempIndex); // get next cell up

			if (cellBelow != -1 && cellType[cellBelow] == CellType.Standard)
				grid[cellBelow] = !grid[cellBelow];
			else
				break; // blocked by deactivated cells or special

			tempIndex = cellBelow; // move index up to cellAbove
		}

		tempIndex = index;
		int cellLeft = -1;
		for (int i=0;i<width;i++) {
			cellLeft= ReturnCellToLeft(tempIndex); // get next cell up

			if (cellLeft != -1 && cellType[cellLeft] == CellType.Standard)
				grid[cellLeft] = !grid[cellLeft];
			else
				break; // blocked by deactivated cells or special

			tempIndex = cellLeft; // move index up to cellAbove
		}

		tempIndex = index;
		for (int i=0;i<width;i++) {
			int cellRight = ReturnCellToRight(tempIndex); // get next cell up

			if (cellRight != -1 && cellType[cellRight] == CellType.Standard)
				grid[cellRight] = !grid[cellRight];
			else
				break; // blocked by deactivated cells or special

			tempIndex = cellRight; // move index up to cellAbove
		}
	}

	// Flip diagonally adjacent cells across entire puzzle along with center
	void Bishop(int index) {

	}
}
