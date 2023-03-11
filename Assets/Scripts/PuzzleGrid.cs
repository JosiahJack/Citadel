using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGrid : MonoBehaviour {
	[HideInInspector]
	public PuzzleGridPuzzle puzzleGP;
	public bool[] powered;
	public PuzzleCellType[] cellType;
	public PuzzleGridType gridType;
	public int sourceIndex;
	public int outputIndex;
	public int width;
	public int height;
	public Button[] gridCells;
	public Image[] geniusHighlights;
	public Sprite nodeOn;
	public Sprite gridPlus;
	public Sprite gridPlusgreen;
	public Sprite gridPluspurple;
	public Sprite gridPlusblue;
	public Sprite gridX;
	public Sprite gridXgreen;
	public Sprite gridXpurple;
	public Sprite gridXblue;
	public Sprite gridNull;
	public Sprite gridSpecial;
	public Sprite gridSpecialgreen;
	public Sprite gridSpecialpurple;
	public Sprite gridSpecialblue;
	public Sprite gridSpecialOn0;
	public Sprite gridSpecialOn0green;
	public Sprite gridSpecialOn0purple;
	public Sprite gridSpecialOn0blue;
	public Sprite gridSpecialOn1;
	public Sprite gridPlusOn0;
	public Sprite gridPlusOn1;
	public Sprite gridXOn0;
	public Sprite gridXOn1;
	public Sprite gridAlwaysOn0;
	public Sprite gridAlwaysOn0green;
	public Sprite gridAlwaysOn0purple;
	public Sprite gridAlwaysOn0blue;
	public Sprite gridAlwaysOn1;
	public Image outputNode;
	public HUDColor theme;
	public AudioClip solvedSFX;
	public bool puzzleSolved;
	public Slider progressBar;

	public bool[] grid;
	private AudioSource audsource;
	private UseData udSender;
	private string target;
	private string argvalue;

	void Awake () {
		puzzleSolved = false;
		audsource = GetComponent<AudioSource>();
		EvaluatePuzzle();
		UpdateCellImages();
		GeniusHighlightClear();
	}

	public void GeniusHighlightClear() {
		for (int i=0;i<35;i++) {
			geniusHighlights[i].enabled = false;
		}
	}

	public void Reset() {
		// No longer in use, reset reset!
		puzzleSolved = false;
		progressBar.value = 0;
		gameObject.SetActive(false);
	}

	public void SendGrid(bool[] states, PuzzleCellType[] types,
						 PuzzleGridType gtype, int start, int end, int w,
						 int h, HUDColor colors, string senttarget, UseData ud,
						 PuzzleGridPuzzle pgp) {
		grid = states;
		cellType = types;
		gridType = gtype;
		sourceIndex = start >= 0 && start < 35 ? start : 0;
		outputIndex = end >= 0 && end < 35 ? end : w;
		width = w;
		height = h;
		theme = colors;
		target = senttarget;
		udSender = ud;
		puzzleGP = pgp;
		puzzleSolved = pgp.puzzleSolved;
		progressBar.value = 0;
		for (int i=0;i<35;i++) {
			powered[i] = false;
		}

		EvaluatePuzzle();
		UpdateCellImages();

		if (udSender.mainIndex == 54 || Const.a.difficultyPuzzle == 0) {
			PuzzleSolved(true);
		}
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) UpdateCellImages();
	}

	public void OnGridCellClick (int index) {
		MFDManager.a.mouseClickHeldOverGUI = true;
		if (puzzleSolved) return;

		if (cellType[index] == PuzzleCellType.Standard) {
			if (Const.a.difficultyPuzzle == 1) {
				King(index); // Easy puzzle difficulty.  Chose King instead of Pawn to help speed up the puzzle by the antenna trap on Level 7
			} else {
				switch (gridType) {
					case PuzzleGridType.King: King(index); break;
					case PuzzleGridType.Queen: Queen(index); break;
					case PuzzleGridType.Knight: Knight(index); break;
					case PuzzleGridType.Rook: Rook(index); break;
					case PuzzleGridType.Bishop: Bishop(index); break;
					case PuzzleGridType.Pawn: Pawn(index); break;
					default: Pawn(index); break;
				}
			}
		}
		EvaluatePuzzle();
		UpdateCellImages();
		puzzleGP.SendDataBackToPanel(this);
	}

	public void OnGridCellHover (int index) {
		if (MouseLookScript.a.geniusActive) {
			if (puzzleSolved) return;

			if (cellType[index] == PuzzleCellType.Standard) {
				if (Const.a.difficultyPuzzle == 1) {
					HoverKing(index); // Easy puzzle difficulty.  Chose King instead of Pawn to help speed up the puzzle by the antenna trap on Level 7
				} else {
					switch (gridType) {
						case PuzzleGridType.King: HoverKing(index); break;
						case PuzzleGridType.Queen: HoverQueen(index); break;
						case PuzzleGridType.Knight: HoverKnight(index); break;
						case PuzzleGridType.Rook: HoverRook(index); break;
						case PuzzleGridType.Bishop: HoverBishop(index); break;
						case PuzzleGridType.Pawn: HoverPawn(index); break;
						default: HoverPawn(index); break;
					}
				}
			}
		}
	}

	public void UpdateCellImages() {
		for (int i=0;i<(width*height);i++) {
			if (cellType[i] != PuzzleCellType.Off) {
				if (cellType[i] == PuzzleCellType.Standard) {
					if (!grid[i]) {
						// Theme dependent
						switch (theme) {
						case HUDColor.Gray:
							gridCells [i].image.overrideSprite = gridX;
							break;
						case HUDColor.Green:
							gridCells [i].image.overrideSprite = gridXgreen;
							break;
						case HUDColor.Purple:
							gridCells [i].image.overrideSprite = gridXpurple;
							break;
						case HUDColor.Blue:
							gridCells [i].image.overrideSprite = gridXblue;
							break;
						default: 
							gridCells [i].image.overrideSprite = gridX;
							break;
						}
					} else {
						if (powered [i]) {
							gridCells [i].image.overrideSprite = gridPlusOn0; // powered node
							// UPDATE? handle different power images for lines between neighbors
						} else {
							// Theme dependent
							switch (theme) {
							case HUDColor.Gray:
								gridCells [i].image.overrideSprite = gridPlus;
								break;
							case HUDColor.Green:
								gridCells [i].image.overrideSprite = gridPlusgreen;
								break;
							case HUDColor.Purple:
								gridCells [i].image.overrideSprite = gridPluspurple;
								break;
							case HUDColor.Blue:
								gridCells [i].image.overrideSprite = gridPlusblue;
									break;
							default: 
								gridCells [i].image.overrideSprite = gridPlus;
								break;
							}
						}
					}
				}
				if (cellType[i] == PuzzleCellType.And) {
					if (powered[i]) {
						// Theme dependent
						switch (theme) {
						case HUDColor.Gray:
							gridCells [i].image.overrideSprite = gridSpecialOn0;
							break;
						case HUDColor.Green:
							gridCells [i].image.overrideSprite = gridSpecialOn0green;
							break;
						case HUDColor.Purple:
							gridCells [i].image.overrideSprite = gridSpecialOn0purple;
							break;
						case HUDColor.Blue:
							gridCells [i].image.overrideSprite = gridSpecialOn0blue;
							break;
						default: 
							gridCells [i].image.overrideSprite = gridSpecialOn0;
							break;
						}
					} else {
						// Theme dependent
						switch (theme) {
						case HUDColor.Gray:
							gridCells [i].image.overrideSprite = gridSpecial;
							break;
						case HUDColor.Green:
							gridCells [i].image.overrideSprite = gridSpecialgreen;
							break;
						case HUDColor.Purple:
							gridCells [i].image.overrideSprite = gridSpecialpurple;
							break;
						case HUDColor.Blue:
							gridCells [i].image.overrideSprite = gridSpecialblue;
							break;
						default: 
							gridCells [i].image.overrideSprite = gridSpecial;
							break;
						}
					}
				}
				if (cellType[i] == PuzzleCellType.Bypass) {
					if (powered[i]) {
						gridCells[i].image.overrideSprite = gridAlwaysOn1; // And node powered
					} else {
						// Theme dependent
						switch (theme) {
						case HUDColor.Gray:
							gridCells [i].image.overrideSprite = gridAlwaysOn0;
							break;
						case HUDColor.Green:
							gridCells [i].image.overrideSprite = gridAlwaysOn0green;
							break;
						case HUDColor.Purple:
							gridCells [i].image.overrideSprite = gridAlwaysOn0purple;
							break;
						case HUDColor.Blue:
							gridCells [i].image.overrideSprite = gridAlwaysOn0blue;
							break;
						default: 
							gridCells [i].image.overrideSprite = gridAlwaysOn0;
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

		if (grid.Length < 1) return;

		// Set power for starting node
		powered[sourceIndex] = grid[sourceIndex];  // Turn on power for cell adjacent to source node if it is a plus
		movingIndex = sourceIndex;
		if (!powered[movingIndex]) return; // Source is off

		// Flow power to all nodes, adding any nodes to check to the queue
		queue.Add(sourceIndex); // Initialize queue
		while (queue.Count > 0) {
			movingIndex = queue[0]; // Get next item in the queue
			if (checkedCells[movingIndex]) { queue.Remove(movingIndex); continue; }

			cellAbove = ReturnCellAbove(movingIndex);
			cellBelow = ReturnCellBelow(movingIndex);
			cellLeft = ReturnCellToLeft(movingIndex);
			cellRight = ReturnCellToRight(movingIndex);
			if (cellAbove != -1 && !checkedCells[cellAbove] && grid[cellAbove])
				queue.Add(cellAbove);
			if (cellBelow != -1 && !checkedCells[cellBelow] && grid[cellBelow])
				queue.Add(cellBelow);
			if (cellLeft != -1 && !checkedCells[cellLeft] && grid[cellLeft])
				queue.Add(cellLeft);
			if (cellRight != -1 && !checkedCells[cellRight] && grid[cellRight])
				queue.Add(cellRight);
			int poweredCount = 0;
			if (cellAbove != -1) {
				if (powered[cellAbove])
					poweredCount++;
			}
			if (cellBelow != -1) {
				if (powered[cellBelow])
					poweredCount++;
			}
			if (cellLeft != -1) {
				if (powered[cellLeft])
					poweredCount++;
			}
			if (cellRight != -1) {
				if (powered[cellRight])
					poweredCount++;
			}
			if (cellType[movingIndex] == PuzzleCellType.And) {
				if (poweredCount > 1) powered[movingIndex] = true;
			} else {
				if (cellType[movingIndex] == PuzzleCellType.Standard || cellType[movingIndex] == PuzzleCellType.Bypass) {
					if ((grid[movingIndex] || cellType[movingIndex] == PuzzleCellType.Bypass) && poweredCount > 0) powered[movingIndex] = true;
				}
			}
			checkedCells [movingIndex] = true;

			float percentProgress = 0f;
			// well ok this is kind of dumb but hey, works.  Tried figuring out the pattern for a nested for loop but whatever:
			if (powered[0] || powered[7] ||  powered[14] || powered[21] || powered[28]) percentProgress = 1f/7f; // column 1
			if (powered[1] || powered[8] ||  powered[15] || powered[22] || powered[29]) percentProgress = 2f/7f; // column 2
			if (powered[2] || powered[9] ||  powered[16] || powered[23] || powered[30]) percentProgress = 3f/7f; // column 3
			if (powered[3] || powered[10]||  powered[17] || powered[24] || powered[31]) percentProgress = 4f/7f; // column 4
			if (powered[4] || powered[11]||  powered[18] || powered[25] || powered[32]) percentProgress = 5f/7f; // column 5
			if (powered[5] || powered[12]||  powered[19] || powered[26] || powered[33]) percentProgress = 6f/7f; // column 6
			if (powered[6] || powered[13]||  powered[20] || powered[27] || powered[34]) percentProgress = 6.75f/7f; // column 7 
			progressBar.value = percentProgress;
		}

		// Latched solved state, no else statement to switch solved state back.
		if (powered[outputIndex]) PuzzleSolved(false);
	}

	void PuzzleSolved(bool usedLogicProbe) {
		if (puzzleSolved) return;

		puzzleSolved = true;
		outputNode.overrideSprite = nodeOn;
		Utils.PlayOneShotSavable(audsource,solvedSFX);
		puzzleGP.puzzleSolved = true;
		puzzleGP.UseTargets(udSender.owner);
		progressBar.value = 100f;
		if (usedLogicProbe) {
			for (int i=0;i<powered.Length;i++) {
				if (cellType[i] == PuzzleCellType.Standard) {
					grid[i] = true;
					powered[i] = true;
				}
			}
			UpdateCellImages();
			MouseLookScript.a.ResetHeldItem();
			MouseLookScript.a.ResetCursor();
		}
		
		Const.a.UseTargets(udSender,target);
	}

	int ReturnCellAbove(int index) {
		int retval = -1;
		if (index == -1) return retval;
		bool outOfBounds = ((index-width) < 0) ? true: false;
		if (!outOfBounds){
			if (cellType[index-width] != PuzzleCellType.Off)
				retval = (index-width); // return cell above if not on top row
		}
		return retval;
	}

	int ReturnCellBelow(int index) {
		int retval = -1;
		if (index == -1) return retval;
		bool outOfBounds = ((index+width) > ((width*height)-1)) ? true: false;
		if (!outOfBounds) {
			if (cellType[index+width] != PuzzleCellType.Off)
				retval = (index+width); // return cell below if not on bottom row
		}
		return retval;
	}

	int ReturnCellToLeft(int index) {
		int retval = -1;
		if (index == -1) return retval;
		if (!((index/width) > 0 && (index%width < 1))) {
			if ((index-1) >= 0) {
				if (cellType[index-1] != PuzzleCellType.Off)
					retval = index-1;  // return index to left if not in far left column
			}
		}
		return retval;
	}

	int ReturnCellToRight(int index) {
		int retval = -1;
		if (index == -1) return retval;
		if (!(((index+1)/width) > 0 && ((index+1)%width < 1))) {
			if ((index+1) < (width*height)) {
				if (cellType[index+1] != PuzzleCellType.Off)
					retval = index+1;  // return index to right if not in far right column
			}
		} 
		return retval;
	}

	int ReturnCellDiagUpRight(int index) {
		int retval = -1;
		if (!(((index+1)/width) > 0 && ((index+1)%width < 1))) {
			retval = index - (width-1);
			if (retval < -1) retval = -1;
		}
		return retval;
	}

	int ReturnCellDiagUpLeft(int index) {
		int retval = -1;
		if (!((index/width) > 0 && (index%width < 1))) {
			retval = index - (width+1);
			if (retval < -1) retval = -1;
		}
		return retval;
	}

	int ReturnCellDiagDownRight(int index) {
		int retval = -1;
		if (!(((index+1)/width) > 0 && ((index+1)%width < 1))) {
			retval = index + (width+1);
			if (retval > ((width*height)-1)) retval = -1;
		}
		return retval;
	}

	int ReturnCellDiagDownLeft(int index) {
		int retval = -1;
		if (!((index/width) > 0 && (index%width < 1))) {
			retval = index + (width-1);
			if (retval > ((width*height)-1)) retval = -1;
		}
		return retval;
	}

	// Only flip clicked cell
	void Pawn(int index) {
		if (cellType[index] == PuzzleCellType.Standard) grid[index] = !grid[index]; // Flip clicked cell if it is standard
	}

	void HoverPawn(int index) {
		GeniusHighlightClear();
		geniusHighlights[index].enabled = true;
	}

	// Flip vertically and horizontally adjacent standard cells along with center
	void King(int index) {
		if (cellType[index] == PuzzleCellType.Standard) grid[index] = !grid[index]; // Flip clicked cell
		int cellAbove = ReturnCellAbove(index);
		int cellBelow = ReturnCellBelow(index);
		int cellLeft = ReturnCellToLeft(index);
		int cellRight = ReturnCellToRight(index);
		int cellDiagUpRt = ReturnCellDiagUpRight(index);
		int cellDiagUpLf = ReturnCellDiagUpLeft(index);
		int cellDiagDnRt = ReturnCellDiagDownRight(index);
		int cellDiagDnLf = ReturnCellDiagDownLeft(index);
		if (cellAbove != -1 && cellType[cellAbove] == PuzzleCellType.Standard) grid[cellAbove] = !grid[cellAbove];
		if (cellBelow != -1 && cellType[cellBelow] == PuzzleCellType.Standard) grid[cellBelow] = !grid[cellBelow];
		if (cellLeft != -1 && cellType[cellLeft] == PuzzleCellType.Standard) grid[cellLeft] = !grid[cellLeft];
		if (cellRight != -1 && cellType[cellRight] == PuzzleCellType.Standard) grid[cellRight] = !grid[cellRight];
		if (cellDiagUpRt != -1 && cellType[cellDiagUpRt] == PuzzleCellType.Standard) grid[cellDiagUpRt] = !grid[cellDiagUpRt];
		if (cellDiagUpLf != -1 && cellType[cellDiagUpLf] == PuzzleCellType.Standard) grid[cellDiagUpLf] = !grid[cellDiagUpLf];
		if (cellDiagDnRt != -1 && cellType[cellDiagDnRt] == PuzzleCellType.Standard) grid[cellDiagDnRt] = !grid[cellDiagDnRt];
		if (cellDiagDnLf != -1 && cellType[cellDiagDnLf] == PuzzleCellType.Standard) grid[cellDiagDnLf] = !grid[cellDiagDnLf];
	}

	void HoverKing(int index) {
		GeniusHighlightClear();
		if (cellType[index] == PuzzleCellType.Standard) geniusHighlights[index].enabled = true;
		int cellAbove = ReturnCellAbove(index);
		int cellBelow = ReturnCellBelow(index);
		int cellLeft = ReturnCellToLeft(index);
		int cellRight = ReturnCellToRight(index);
		int cellDiagUpRt = ReturnCellDiagUpRight(index);
		int cellDiagUpLf = ReturnCellDiagUpLeft(index);
		int cellDiagDnRt = ReturnCellDiagDownRight(index);
		int cellDiagDnLf = ReturnCellDiagDownLeft(index);
		if (cellAbove != -1 && cellType[cellAbove] == PuzzleCellType.Standard) geniusHighlights[cellAbove].enabled = true;
		if (cellBelow != -1 && cellType[cellBelow] == PuzzleCellType.Standard) geniusHighlights[cellBelow].enabled = true;
		if (cellLeft != -1 && cellType[cellLeft] == PuzzleCellType.Standard) geniusHighlights[cellLeft].enabled = true;
		if (cellRight != -1 && cellType[cellRight] == PuzzleCellType.Standard) geniusHighlights[cellRight].enabled = true;
		if (cellDiagUpRt != -1 && cellType[cellDiagUpRt] == PuzzleCellType.Standard) geniusHighlights[cellDiagUpRt].enabled = true;
		if (cellDiagUpLf != -1 && cellType[cellDiagUpLf] == PuzzleCellType.Standard) geniusHighlights[cellDiagUpLf].enabled = true;
		if (cellDiagDnRt != -1 && cellType[cellDiagDnRt] == PuzzleCellType.Standard) geniusHighlights[cellDiagDnRt].enabled = true;
		if (cellDiagDnLf != -1 && cellType[cellDiagDnLf] == PuzzleCellType.Standard) geniusHighlights[cellDiagDnLf].enabled = true;
	}

	// Flip corners along with center
	void Knight(int index) {
		if (cellType[index] == PuzzleCellType.Standard) grid[index] = !grid[index]; // Flip clicked cell
		int cellDiagUpRt = ReturnCellDiagUpRight(index);
		int cellDiagUpLf = ReturnCellDiagUpLeft(index);
		int cellDiagDnRt = ReturnCellDiagDownRight(index);
		int cellDiagDnLf = ReturnCellDiagDownLeft(index);
		if (cellDiagUpRt != -1 && cellType[cellDiagUpRt] == PuzzleCellType.Standard) grid[cellDiagUpRt] = !grid[cellDiagUpRt];
		if (cellDiagUpLf != -1 && cellType[cellDiagUpLf] == PuzzleCellType.Standard) grid[cellDiagUpLf] = !grid[cellDiagUpLf];
		if (cellDiagDnRt != -1 && cellType[cellDiagDnRt] == PuzzleCellType.Standard) grid[cellDiagDnRt] = !grid[cellDiagDnRt];
		if (cellDiagDnLf != -1 && cellType[cellDiagDnLf] == PuzzleCellType.Standard) grid[cellDiagDnLf] = !grid[cellDiagDnLf];
	}

	void HoverKnight(int index) {
		GeniusHighlightClear();
		if (cellType[index] == PuzzleCellType.Standard) geniusHighlights[index].enabled = true;
		int cellDiagUpRt = ReturnCellDiagUpRight(index);
		int cellDiagUpLf = ReturnCellDiagUpLeft(index);
		int cellDiagDnRt = ReturnCellDiagDownRight(index);
		int cellDiagDnLf = ReturnCellDiagDownLeft(index);
		if (cellDiagUpRt != -1 && cellType[cellDiagUpRt] == PuzzleCellType.Standard)  geniusHighlights[cellDiagUpRt].enabled = true;
		if (cellDiagUpLf != -1 && cellType[cellDiagUpLf] == PuzzleCellType.Standard)  geniusHighlights[cellDiagUpLf].enabled = true;
		if (cellDiagDnRt != -1 && cellType[cellDiagDnRt] == PuzzleCellType.Standard)  geniusHighlights[cellDiagDnRt].enabled = true;
		if (cellDiagDnLf != -1 && cellType[cellDiagDnLf] == PuzzleCellType.Standard)  geniusHighlights[cellDiagDnLf].enabled = true;
	}

	// Flip vertically and horizontally adjacent cells across entire puzzle along with center
	void Rook(int index) {
		if (cellType[index] == PuzzleCellType.Standard) grid[index] = !grid[index]; // Flip clicked cell

		int tempIndex = index;
		for (int i=0;i<width;i++) {
			int cellAbove = ReturnCellAbove(tempIndex); // get next cell up

			if (cellAbove != -1 && cellType[cellAbove] == PuzzleCellType.Standard)
				grid[cellAbove] = !grid[cellAbove];
			else
				break; // blocked by deactivated cells or special
			
			tempIndex = cellAbove; // move index up to cellAbove
		}

		tempIndex = index;
		int cellBelow = -1;
		for (int i=0;i<width;i++) {
			cellBelow = ReturnCellBelow(tempIndex); // get next cell up

			if (cellBelow != -1 && cellType[cellBelow] == PuzzleCellType.Standard)
				grid[cellBelow] = !grid[cellBelow];
			else
				break; // blocked by deactivated cells or special

			tempIndex = cellBelow; // move index up to cellAbove
		}

		tempIndex = index;
		int cellLeft = -1;
		for (int i=0;i<width;i++) {
			cellLeft= ReturnCellToLeft(tempIndex); // get next cell up

			if (cellLeft != -1 && cellType[cellLeft] == PuzzleCellType.Standard)
				grid[cellLeft] = !grid[cellLeft];
			else
				break; // blocked by deactivated cells or special

			tempIndex = cellLeft; // move index up to cellAbove
		}

		tempIndex = index;
		for (int i=0;i<width;i++) {
			int cellRight = ReturnCellToRight(tempIndex); // get next cell up

			if (cellRight != -1 && cellType[cellRight] == PuzzleCellType.Standard)
				grid[cellRight] = !grid[cellRight];
			else
				break; // blocked by deactivated cells or special

			tempIndex = cellRight; // move index up to cellAbove
		}
	}

	void HoverRook(int index) {
		GeniusHighlightClear();
		if (cellType[index] == PuzzleCellType.Standard) geniusHighlights[index].enabled = true;

		int tempIndex = index;
		for (int i=0;i<width;i++) {
			int cellAbove = ReturnCellAbove(tempIndex); // get next cell up

			if (cellAbove != -1 && cellType[cellAbove] == PuzzleCellType.Standard)
				geniusHighlights[cellAbove].enabled = true;
			else
				break; // blocked by deactivated cells or special
			
			tempIndex = cellAbove; // move index up to cellAbove
		}

		tempIndex = index;
		int cellBelow = -1;
		for (int i=0;i<width;i++) {
			cellBelow = ReturnCellBelow(tempIndex); // get next cell up

			if (cellBelow != -1 && cellType[cellBelow] == PuzzleCellType.Standard)
				geniusHighlights[cellBelow].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = cellBelow; // move index up to cellAbove
		}

		tempIndex = index;
		int cellLeft = -1;
		for (int i=0;i<width;i++) {
			cellLeft= ReturnCellToLeft(tempIndex); // get next cell up

			if (cellLeft != -1 && cellType[cellLeft] == PuzzleCellType.Standard)
				geniusHighlights[cellLeft].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = cellLeft; // move index up to cellAbove
		}

		tempIndex = index;
		for (int i=0;i<width;i++) {
			int cellRight = ReturnCellToRight(tempIndex); // get next cell up

			if (cellRight != -1 && cellType[cellRight] == PuzzleCellType.Standard)
				geniusHighlights[cellRight].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = cellRight; // move index up to cellAbove
		}
	}

	// Flip diagonally adjacent cells across entire puzzle along with center
	void Bishop(int index) {
		if (cellType[index] == PuzzleCellType.Standard) grid[index] = !grid[index]; // Flip clicked cell
		int cellAbove = ReturnCellAbove(index);
		int cellBelow = ReturnCellBelow(index);
		int cellDiagUpRt = ReturnCellDiagUpRight(index);
		int cellDiagUpLf = ReturnCellDiagUpLeft(index);
		int cellDiagDnRt = ReturnCellDiagDownRight(index);
		int cellDiagDnLf = ReturnCellDiagDownLeft(index);

		// First run along line up and to the right
		int tempIndex = cellDiagUpRt;
		for (int i=0;i<Mathf.Min(height,width);i++) {
			if (tempIndex != -1 && cellType[tempIndex] == PuzzleCellType.Standard)
				grid[tempIndex] = !grid[tempIndex];
			else
				break; // blocked by deactivated cells or special

			tempIndex = ReturnCellDiagUpRight(tempIndex);
		}

		// Now up and to the left
		tempIndex = cellDiagUpLf;
		for (int i=0;i<Mathf.Min(height,width);i++) {
			if (tempIndex != -1 && cellType[tempIndex] == PuzzleCellType.Standard)
				grid[tempIndex] = !grid[tempIndex];
			else
				break; // blocked by deactivated cells or special

			tempIndex = ReturnCellDiagUpLeft(tempIndex);
		}

		// Now down and to the right
		tempIndex = cellDiagDnRt;
		for (int i=0;i<Mathf.Min(height,width);i++) {
			if (tempIndex != -1 && cellType[tempIndex] == PuzzleCellType.Standard)
				grid[tempIndex] = !grid[tempIndex];
			else
				break; // blocked by deactivated cells or special

			tempIndex = ReturnCellDiagDownRight(tempIndex);
		}

		// Finally down and to the left
		tempIndex = cellDiagDnLf;
		for (int i=0;i<Mathf.Min(height,width);i++) {
			if (tempIndex != -1 && cellType[tempIndex] == PuzzleCellType.Standard)
				grid[tempIndex] = !grid[tempIndex];
			else
				break; // blocked by deactivated cells or special

			tempIndex = ReturnCellDiagDownLeft(tempIndex);
		}
	}

	void HoverBishop(int index) {
		GeniusHighlightClear();
		if (cellType[index] == PuzzleCellType.Standard) geniusHighlights[index].enabled = true;
		int cellAbove = ReturnCellAbove(index);
		int cellBelow = ReturnCellBelow(index);
		int cellDiagUpRt = ReturnCellDiagUpRight(index);
		int cellDiagUpLf = ReturnCellDiagUpLeft(index);
		int cellDiagDnRt = ReturnCellDiagDownRight(index);
		int cellDiagDnLf = ReturnCellDiagDownLeft(index);

		// First run along line up and to the right
		int tempIndex = cellDiagUpRt;
		for (int i=0;i<Mathf.Min(height,width);i++) {
			if (tempIndex != -1 && cellType[tempIndex] == PuzzleCellType.Standard)
				geniusHighlights[tempIndex].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = ReturnCellDiagUpRight(tempIndex);
		}

		// Now up and to the left
		tempIndex = cellDiagUpLf;
		for (int i=0;i<Mathf.Min(height,width);i++) {
			if (tempIndex != -1 && cellType[tempIndex] == PuzzleCellType.Standard)
				geniusHighlights[tempIndex].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = ReturnCellDiagUpLeft(tempIndex);
		}

		// Now down and to the right
		tempIndex = cellDiagDnRt;
		for (int i=0;i<Mathf.Min(height,width);i++) {
			if (tempIndex != -1 && cellType[tempIndex] == PuzzleCellType.Standard)
				 geniusHighlights[tempIndex].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = ReturnCellDiagDownRight(tempIndex);
		}

		// Finally down and to the left
		tempIndex = cellDiagDnLf;
		for (int i=0;i<Mathf.Min(height,width);i++) {
			if (tempIndex != -1 && cellType[tempIndex] == PuzzleCellType.Standard)
				geniusHighlights[tempIndex].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = ReturnCellDiagDownLeft(tempIndex);
		}
	}

	// Flip diagonally adjacent cells and horizontal and vertical across entire puzzle along with center
	void Queen(int index) {
		Rook(index);
		Bishop(index);
		Pawn(index);
	}


	void HoverQueen(int index) {
		GeniusHighlightClear();
		if (cellType[index] == PuzzleCellType.Standard) geniusHighlights[index].enabled = true;

		int cellAbove;
		int cellBelow;
		int cellDiagUpRt;
		int cellDiagUpLf;
		int cellDiagDnRt;
		int cellDiagDnLf;
		int cellLeft;
		int cellRight;
		int tempIndex = index;
		for (int i=0;i<width;i++) {
			cellAbove = ReturnCellAbove(tempIndex); // get next cell up

			if (cellAbove != -1 && cellType[cellAbove] == PuzzleCellType.Standard)
				geniusHighlights[cellAbove].enabled = true;
			else
				break; // blocked by deactivated cells or special
			
			tempIndex = cellAbove; // move index up to cellAbove
		}

		tempIndex = index;
		cellBelow = -1;
		for (int i=0;i<width;i++) {
			cellBelow = ReturnCellBelow(tempIndex); // get next cell up

			if (cellBelow != -1 && cellType[cellBelow] == PuzzleCellType.Standard)
				geniusHighlights[cellBelow].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = cellBelow; // move index up to cellAbove
		}

		tempIndex = index;
		cellLeft = -1;
		for (int i=0;i<width;i++) {
			cellLeft= ReturnCellToLeft(tempIndex); // get next cell up

			if (cellLeft != -1 && cellType[cellLeft] == PuzzleCellType.Standard)
				geniusHighlights[cellLeft].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = cellLeft; // move index up to cellAbove
		}

		tempIndex = index;
		for (int i=0;i<width;i++) {
			cellRight = ReturnCellToRight(tempIndex); // get next cell up

			if (cellRight != -1 && cellType[cellRight] == PuzzleCellType.Standard)
				geniusHighlights[cellRight].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = cellRight; // move index up to cellAbove
		}
		if (cellType[index] == PuzzleCellType.Standard) geniusHighlights[index].enabled = true;
		cellAbove = ReturnCellAbove(index);
		cellBelow = ReturnCellBelow(index);
		cellDiagUpRt = ReturnCellDiagUpRight(index);
		cellDiagUpLf = ReturnCellDiagUpLeft(index);
		cellDiagDnRt = ReturnCellDiagDownRight(index);
		cellDiagDnLf = ReturnCellDiagDownLeft(index);

		// First run along line up and to the right
		tempIndex = cellDiagUpRt;
		for (int i=0;i<Mathf.Min(height,width);i++) {
			if (tempIndex != -1 && cellType[tempIndex] == PuzzleCellType.Standard)
				geniusHighlights[tempIndex].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = ReturnCellDiagUpRight(tempIndex);
		}

		// Now up and to the left
		tempIndex = cellDiagUpLf;
		for (int i=0;i<Mathf.Min(height,width);i++) {
			if (tempIndex != -1 && cellType[tempIndex] == PuzzleCellType.Standard)
				geniusHighlights[tempIndex].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = ReturnCellDiagUpLeft(tempIndex);
		}

		// Now down and to the right
		tempIndex = cellDiagDnRt;
		for (int i=0;i<Mathf.Min(height,width);i++) {
			if (tempIndex != -1 && cellType[tempIndex] == PuzzleCellType.Standard)
				 geniusHighlights[tempIndex].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = ReturnCellDiagDownRight(tempIndex);
		}

		// Finally down and to the left
		tempIndex = cellDiagDnLf;
		for (int i=0;i<Mathf.Min(height,width);i++) {
			if (tempIndex != -1 && cellType[tempIndex] == PuzzleCellType.Standard)
				geniusHighlights[tempIndex].enabled = true;
			else
				break; // blocked by deactivated cells or special

			tempIndex = ReturnCellDiagDownLeft(tempIndex);
		}
		geniusHighlights[index].enabled = true;
	}
}
