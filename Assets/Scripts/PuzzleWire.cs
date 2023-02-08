using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleWire : MonoBehaviour {
	[HideInInspector]
	public PuzzleWirePuzzle puzzleWP;
	public bool[] wireIsActive;
	public bool[] nodeRowIsActive;
	public GameObject[] leftNodes;
	public GameObject[] rightNodes;
	//public int[] targetPositionsLeft;
	//public int[] targetPositionsRight;
	public Image[] leftNodeSelectedIndicators;
	public Image[] leftNodeImages;
	public Image[] rightNodeSelectedIndicators;
	public Image[] rightNodeImages;
	public Sprite normalNode;
	public Sprite highlightedNode;
	public Slider slider;
	public int wire1LHPosition;
	public int wire1RHPosition;
	public int wire2LHPosition;
	public int wire2RHPosition;
	public int wire3LHPosition;
	public int wire3RHPosition;
	public int wire4LHPosition;
	public int wire4RHPosition;
	public int wire5LHPosition;
	public int wire5RHPosition;
	public int wire6LHPosition;
	public int wire6RHPosition;
	public int wire7LHPosition;
	public int wire7RHPosition;

	public int wire1LHTarget;
	public int wire1RHTarget;
	public int wire2LHTarget;
	public int wire2RHTarget;
	public int wire3LHTarget;
	public int wire3RHTarget;
	public int wire4LHTarget;
	public int wire4RHTarget;
	public int wire5LHTarget;
	public int wire5RHTarget;
	public int wire6LHTarget;
	public int wire6RHTarget;
	public int wire7LHTarget;
	public int wire7RHTarget;

	public LineRenderer[] wires;
	public float nodeYOffset = 26f;
	public float nodeXOffset = 106f;
	public float blinkTime = 0.5f;
	public bool Solved = false;
	public float shakeMultiplier = 0.05f;
	private bool wireWasDisplaced;
	private int tempInt; // don't make garbage to collect
	private float tempF;
	private int selectedWire;
	private bool selectedWireLH;
	private Vector3 tempVec;
	private bool blinkState;
	private float blinkTimeFinished;
	public float actualValue;
	private int numberOfWires;
	private AudioSource SFXSource;
	public AudioClip SFX;
	public string target;
	public string argvalue;
	private UseData udSender;
	public bool geniusActive;
	public Image[] geniusHintsLH;
	public Image[] geniusHintsRH;

	public HUDColor theme;
	public Color actualColorRed;
	public Color actualColorOrange;
	public Color actualColorYellow;
	public Color actualColorGreen;
	public Color actualColorBlue;
	public Color actualColorPurple;
	public HUDColor[] wireColors;
	public HUDColor[] rememberColors;

	void Awake () {
		selectedWire = -1;
		blinkState = false;
		CheckEnabledNodes();
		ChangeAppearance();
		DisableAllSelectedIndicators();
		DisableGeniusHints();
		SFXSource = GetComponent<AudioSource>();
		//EvaluatePuzzle();
	}

	public void Reset() {
		// No longer in use, reset reset!
		Solved = false;
		slider.value = 0;
		selectedWire = -1;
		blinkState = false;
		CheckEnabledNodes();
		ChangeAppearance();
		DisableAllSelectedIndicators();
		DisableGeniusHints();
		gameObject.SetActive(false);
	}

	Color GetColor(HUDColor index) {
		if (index == HUDColor.Red) return actualColorRed;
		if (index == HUDColor.Orange) return actualColorOrange;
		if (index == HUDColor.Yellow) return actualColorYellow;
		if (index == HUDColor.Green) return actualColorGreen;
		if (index == HUDColor.Blue) return actualColorBlue;
		if (index == HUDColor.Purple) return actualColorPurple;
		return actualColorRed;
	}

	void DisableGeniusHints () {
		for (tempInt = 0; tempInt < 7; tempInt++) {
			geniusHintsLH[tempInt].enabled = false;
			geniusHintsRH[tempInt].enabled = false;
		}
	}

	void DisableAllSelectedIndicators () {
		for (tempInt = 0; tempInt < 7; tempInt++) {
			leftNodeSelectedIndicators[tempInt].enabled = false;
			rightNodeSelectedIndicators[tempInt].enabled = false;
			leftNodeImages[tempInt].overrideSprite = normalNode;
			rightNodeImages[tempInt].overrideSprite = normalNode;
		}
	}

	void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (Solved) return;

			if (selectedWire != -1) {
				if (blinkTimeFinished < PauseScript.a.relativeTime) {
					BlinkSelectedIndicator();
					blinkState = !blinkState;
					blinkTimeFinished = PauseScript.a.relativeTime + blinkTime;
				}
			} else {
				DisableAllSelectedIndicators();
			}

			if (geniusActive) {
				numberOfWires = 0;
				for (int tempInt = 0; tempInt < 7; tempInt++) {
					if (wireIsActive[tempInt])
						numberOfWires++;
				}

				if (Const.a.difficultyPuzzle == 3) {
					// Set all wire colors to the same on hard
					wireColors[0] = rememberColors[0];
					wireColors[1] = rememberColors[1];
					wireColors[2] = rememberColors[2];
					wireColors[3] = rememberColors[3];
					wireColors[4] = rememberColors[4];
					wireColors[5] = rememberColors[5];
					wireColors[6] = rememberColors[6];
				}

				if (wireIsActive[0]) {
						geniusHintsLH[wire1LHTarget].enabled = true;
						geniusHintsRH[wire1RHTarget].enabled = true;
				}
				if (wireIsActive[1]) {
						geniusHintsLH[wire2LHTarget].enabled = true;
						geniusHintsRH[wire2RHTarget].enabled = true;
				}
				if (wireIsActive[2]) {
						geniusHintsLH[wire3LHTarget].enabled = true;
						geniusHintsRH[wire3RHTarget].enabled = true;
				}
				if (wireIsActive[3]) {
						geniusHintsLH[wire4LHTarget].enabled = true;
						geniusHintsRH[wire4RHTarget].enabled = true;
				}
				if (wireIsActive[4]) {
						geniusHintsLH[wire5LHTarget].enabled = true;
						geniusHintsRH[wire5RHTarget].enabled = true;
				}
				if (wireIsActive[5]) {
						geniusHintsLH[wire6LHTarget].enabled = true;
						geniusHintsRH[wire6RHTarget].enabled = true;
				}
				if (wireIsActive[6]) {
						geniusHintsLH[wire7LHTarget].enabled = true;
						geniusHintsRH[wire7RHTarget].enabled = true;
				}
			} else {
				DisableGeniusHints();
				if (Const.a.difficultyPuzzle == 3) {
					// Set all wire colors to the same on hard
					wireColors[0] = HUDColor.Yellow;
					wireColors[1] = HUDColor.Yellow;
					wireColors[2] = HUDColor.Yellow;
					wireColors[3] = HUDColor.Yellow;
					wireColors[4] = HUDColor.Yellow;
					wireColors[5] = HUDColor.Yellow;
					wireColors[6] = HUDColor.Yellow;
				}
			}

			if (!Solved) {
				slider.value = actualValue + Random.Range(0f,shakeMultiplier);
				if (slider.value > 100f) {
					slider.value = 100f;
				}

				if (slider.value < 0) {
					slider.value = 0;
				}
			}
		}
	}

	private void BlinkSelectedIndicator() {
		if (selectedWireLH) {
			switch (selectedWire) {
				case 0:
					leftNodeSelectedIndicators[wire1LHPosition].enabled = blinkState;
					break;
				case 1:
					leftNodeSelectedIndicators[wire2LHPosition].enabled = blinkState;
					break;
				case 2:
					leftNodeSelectedIndicators[wire3LHPosition].enabled = blinkState;
					break;
				case 3:
					leftNodeSelectedIndicators[wire4LHPosition].enabled = blinkState;
					break;
				case 4:
					leftNodeSelectedIndicators[wire5LHPosition].enabled = blinkState;
					break;
				case 5: 
					leftNodeSelectedIndicators[wire6LHPosition].enabled = blinkState;
					break;
				case 6: 
					leftNodeSelectedIndicators[wire7LHPosition].enabled = blinkState;
					break;
			}

// public Image[] leftNodeImages;
	// public Image[] rightNodeSelectedIndicators;
	// public Image[] rightNodeImages;
	// public Sprite normalNode;
	// public Sprite highlightedNode;
			for (int i=0;i<7;i++) {
				leftNodeImages[i].overrideSprite = highlightedNode;
				rightNodeImages[i].overrideSprite = normalNode;
			}
		} else {
			switch (selectedWire) {
				case 0:
					rightNodeSelectedIndicators[wire1RHPosition].enabled = blinkState;
					break;
				case 1:
					rightNodeSelectedIndicators[wire2RHPosition].enabled = blinkState;
					break;
				case 2:
					rightNodeSelectedIndicators[wire3RHPosition].enabled = blinkState;
					break;
				case 3:
					rightNodeSelectedIndicators[wire4RHPosition].enabled = blinkState;
					break;
				case 4:
					rightNodeSelectedIndicators[wire5RHPosition].enabled = blinkState;
					break;
				case 5: 
					rightNodeSelectedIndicators[wire6RHPosition].enabled = blinkState;
					break;
				case 6: 
					rightNodeSelectedIndicators[wire7RHPosition].enabled = blinkState;
					break;
			}
			for (int i=0;i<7;i++) {
				leftNodeImages[i].overrideSprite = normalNode;
				rightNodeImages[i].overrideSprite = highlightedNode;
			}
		}
	}

	public void SendWirePuzzleData(bool[] sentWiresOn, bool[] sentNodeRowsActive, int[] sentCurrentPositionsLeft, int[] sentCurrentPositionsRight, int[] sentTargetsLeft, int[] sentTargetsRight,HUDColor sentTheme, HUDColor[] sentHUDColors, string t1, string a1, UseData udSent,PuzzleWirePuzzle pwp) {
		wireIsActive = sentWiresOn;
		nodeRowIsActive = sentNodeRowsActive;
		wire1LHPosition = sentCurrentPositionsLeft[0];
		wire2LHPosition = sentCurrentPositionsLeft[1];
		wire3LHPosition = sentCurrentPositionsLeft[2];
		wire4LHPosition = sentCurrentPositionsLeft[3];
		wire5LHPosition = sentCurrentPositionsLeft[4];
		wire6LHPosition = sentCurrentPositionsLeft[5];
		wire7LHPosition = sentCurrentPositionsLeft[6];
		wire1RHPosition = sentCurrentPositionsRight[0];
		wire2RHPosition = sentCurrentPositionsRight[1];
		wire3RHPosition = sentCurrentPositionsRight[2];
		wire4RHPosition = sentCurrentPositionsRight[3];
		wire5RHPosition = sentCurrentPositionsRight[4];
		wire6RHPosition = sentCurrentPositionsRight[5];
		wire7RHPosition = sentCurrentPositionsRight[6];
		wire1LHTarget = sentTargetsLeft[0];
		wire2LHTarget = sentTargetsLeft[1];
		wire3LHTarget = sentTargetsLeft[2];
		wire4LHTarget = sentTargetsLeft[3];
		wire5LHTarget = sentTargetsLeft[4];
		wire6LHTarget = sentTargetsLeft[5];
		wire7LHTarget = sentTargetsLeft[6];
		wire1RHTarget = sentTargetsRight[0];
		wire2RHTarget = sentTargetsRight[1];
		wire3RHTarget = sentTargetsRight[2];
		wire4RHTarget = sentTargetsRight[3];
		wire5RHTarget = sentTargetsRight[4];
		wire6RHTarget = sentTargetsRight[5];
		wire7RHTarget = sentTargetsRight[6];
		udSender = udSent;
		theme = sentTheme;
		wireColors = sentHUDColors;
		rememberColors = sentHUDColors;
		if (Const.a.difficultyPuzzle == 3) {
			// Set all wire colors to the same on hard
			wireColors[0] = HUDColor.Yellow;
			wireColors[1] = HUDColor.Yellow;
			wireColors[2] = HUDColor.Yellow;
			wireColors[3] = HUDColor.Yellow;
			wireColors[4] = HUDColor.Yellow;
			wireColors[5] = HUDColor.Yellow;
			wireColors[6] = HUDColor.Yellow;
		}
		target = t1;
		argvalue = a1;
		puzzleWP = pwp;
		Solved = pwp.puzzleSolved;
		CheckEnabledNodes();
		EvaluatePuzzle();
		ChangeAppearance();

		if (udSent.mainIndex == 54 || Const.a.difficultyPuzzle == 0) {
			PuzzleSolved(true);
		}
	}

	private Vector3 GetPositionOfLHNode(int index) {
		tempVec = Const.a.vectorZero;
		tempVec.x = 0;
		tempVec.y = nodeYOffset * -1 * index;
		return tempVec;
	}

	private Vector3 GetPositionOfRHNode(int index) {
		tempVec = Const.a.vectorZero;
		tempVec.x = nodeXOffset;
		tempVec.y = nodeYOffset * -1 * index;
		return tempVec;
	}

	public void ChangeAppearance() {
		CheckEnabledNodes();
		wires[0].SetPosition(0,GetPositionOfLHNode(wire1LHPosition));
		wires[0].SetPosition(1,GetPositionOfRHNode(wire1RHPosition));
		wires[1].SetPosition(0,GetPositionOfLHNode(wire2LHPosition));
		wires[1].SetPosition(1,GetPositionOfRHNode(wire2RHPosition));
		wires[2].SetPosition(0,GetPositionOfLHNode(wire3LHPosition));
		wires[2].SetPosition(1,GetPositionOfRHNode(wire3RHPosition));
		wires[3].SetPosition(0,GetPositionOfLHNode(wire4LHPosition));
		wires[3].SetPosition(1,GetPositionOfRHNode(wire4RHPosition));
		wires[4].SetPosition(0,GetPositionOfLHNode(wire5LHPosition));
		wires[4].SetPosition(1,GetPositionOfRHNode(wire5RHPosition));
		wires[5].SetPosition(0,GetPositionOfLHNode(wire6LHPosition));
		wires[5].SetPosition(1,GetPositionOfRHNode(wire6RHPosition));
		wires[6].SetPosition(0,GetPositionOfLHNode(wire7LHPosition));
		wires[6].SetPosition(1,GetPositionOfRHNode(wire7RHPosition));
	}

	public void CheckEnabledNodes() {
		for (tempInt = 0; tempInt < 7; tempInt++) {
			if (wireIsActive[tempInt]) {
				wires[tempInt].enabled = true;
				wires[tempInt].startColor = GetColor(wireColors[tempInt]);
				wires[tempInt].endColor = GetColor(wireColors[tempInt]);
			} else {
				wires[tempInt].enabled = false;
			}

			if (nodeRowIsActive[tempInt]) {
				leftNodes[tempInt].SetActive(true);
				rightNodes[tempInt].SetActive(true);
			} else {
				leftNodes[tempInt].SetActive(false);
				rightNodes[tempInt].SetActive(false);
			}
		}
	}

	public void ClickLHNode(int spot) {
		MFDManager.a.mouseClickHeldOverGUI = true;
		if (Solved) return;

		if (selectedWireLH) {
			if (selectedWire < 0) {
				SelectWireLH(spot);
			} else {
				MoveEndpointLeft(spot);
			}
		} else {
			SelectWireLH(spot);
		}
		puzzleWP.SendDataBackToPanel(this,true);
	}

	public void ClickRHNode(int spot) {
		MFDManager.a.mouseClickHeldOverGUI = true;
		if (Solved) return;

		if (!selectedWireLH) {
			if (selectedWire < 0) {
				SelectWireRH(spot);
			} else {
				MoveEndpointRight(spot);
			}
		} else {
			SelectWireRH(spot);
		}
		puzzleWP.SendDataBackToPanel(this,true);
	}

	public void SelectWireLH(int spot) {
		selectedWire = -1;
		// Check LH spots and see which wire we have
		if (wire1LHPosition == spot && wireIsActive[0]) selectedWire = 0;
		if (wire2LHPosition == spot && wireIsActive[1]) selectedWire = 1;
		if (wire3LHPosition == spot && wireIsActive[2]) selectedWire = 2;
		if (wire4LHPosition == spot && wireIsActive[3]) selectedWire = 3;
		if (wire5LHPosition == spot && wireIsActive[4]) selectedWire = 4;
		if (wire6LHPosition == spot && wireIsActive[5]) selectedWire = 5;
		if (wire7LHPosition == spot && wireIsActive[6]) selectedWire = 6;
		if (selectedWire == -1)
			return;
		
		DisableAllSelectedIndicators();
		selectedWireLH = true;
		blinkState = true;
		blinkTimeFinished = PauseScript.a.relativeTime + blinkTime;
		BlinkSelectedIndicator();
		ChangeAppearance();
		//EvaluatePuzzle();
	}

	public void SelectWireRH(int spot) {
		selectedWire = -1;
		// Check RH spots and see which wire we have
		if (wire1RHPosition == spot && wireIsActive[0]) selectedWire = 0;
		if (wire2RHPosition == spot && wireIsActive[1]) selectedWire = 1;
		if (wire3RHPosition == spot && wireIsActive[2]) selectedWire = 2;
		if (wire4RHPosition == spot && wireIsActive[3]) selectedWire = 3;
		if (wire5RHPosition == spot && wireIsActive[4]) selectedWire = 4;
		if (wire6RHPosition == spot && wireIsActive[5]) selectedWire = 5;
		if (wire7RHPosition == spot && wireIsActive[6]) selectedWire = 6;
		if (selectedWire == -1)
			return;

		DisableAllSelectedIndicators();
		selectedWireLH = false;
		blinkState = true;
		blinkTimeFinished = PauseScript.a.relativeTime + blinkTime;
		BlinkSelectedIndicator();
		ChangeAppearance();
		//EvaluatePuzzle();
	}

	public void MoveEndpointLeft(int newSpot) {
		if (selectedWire == -1)	return;
		
		switch (selectedWire) {
			case 0:
				if (wire2LHPosition == newSpot) wire2LHPosition = wire1LHPosition;
				if (wire3LHPosition == newSpot) wire3LHPosition = wire1LHPosition;
				if (wire4LHPosition == newSpot) wire4LHPosition = wire1LHPosition;
				if (wire5LHPosition == newSpot) wire5LHPosition = wire1LHPosition;
				if (wire6LHPosition == newSpot) wire6LHPosition = wire1LHPosition;
				if (wire7LHPosition == newSpot) wire7LHPosition = wire1LHPosition;
				wire1LHPosition = newSpot;
				break;
			case 1:
				if (wire1LHPosition == newSpot) wire1LHPosition = wire2LHPosition;
				if (wire3LHPosition == newSpot) wire3LHPosition = wire2LHPosition;
				if (wire4LHPosition == newSpot) wire4LHPosition = wire2LHPosition;
				if (wire5LHPosition == newSpot) wire5LHPosition = wire2LHPosition;
				if (wire6LHPosition == newSpot) wire6LHPosition = wire2LHPosition;
				if (wire7LHPosition == newSpot) wire7LHPosition = wire2LHPosition;
				wire2LHPosition = newSpot;
				break;
			case 2:
				if (wire1LHPosition == newSpot) wire1LHPosition = wire3LHPosition;
				if (wire2LHPosition == newSpot) wire2LHPosition = wire3LHPosition;
				if (wire4LHPosition == newSpot) wire4LHPosition = wire3LHPosition;
				if (wire5LHPosition == newSpot) wire5LHPosition = wire3LHPosition;
				if (wire6LHPosition == newSpot) wire6LHPosition = wire3LHPosition;
				if (wire7LHPosition == newSpot) wire7LHPosition = wire3LHPosition;
				wire3LHPosition = newSpot;
				break;
			case 3:
				if (wire1LHPosition == newSpot) wire1LHPosition = wire4LHPosition;
				if (wire2LHPosition == newSpot) wire2LHPosition = wire4LHPosition;
				if (wire3LHPosition == newSpot) wire3LHPosition = wire4LHPosition;
				if (wire5LHPosition == newSpot) wire5LHPosition = wire4LHPosition;
				if (wire6LHPosition == newSpot) wire6LHPosition = wire4LHPosition;
				if (wire7LHPosition == newSpot) wire7LHPosition = wire4LHPosition;
				wire4LHPosition = newSpot;
				break;
			case 4:
				if (wire1LHPosition == newSpot) wire1LHPosition = wire5LHPosition;
				if (wire2LHPosition == newSpot) wire2LHPosition = wire5LHPosition;
				if (wire3LHPosition == newSpot) wire3LHPosition = wire5LHPosition;
				if (wire4LHPosition == newSpot) wire4LHPosition = wire5LHPosition;
				if (wire6LHPosition == newSpot) wire6LHPosition = wire5LHPosition;
				if (wire7LHPosition == newSpot) wire7LHPosition = wire5LHPosition;
				wire5LHPosition = newSpot;
				break;
			case 5:
				if (wire1LHPosition == newSpot) wire1LHPosition = wire6LHPosition;
				if (wire2LHPosition == newSpot) wire2LHPosition = wire6LHPosition;
				if (wire3LHPosition == newSpot) wire3LHPosition = wire6LHPosition;
				if (wire4LHPosition == newSpot) wire4LHPosition = wire6LHPosition;
				if (wire5LHPosition == newSpot) wire5LHPosition = wire6LHPosition;
				if (wire7LHPosition == newSpot) wire7LHPosition = wire6LHPosition;
				wire6LHPosition = newSpot;
				break;
			case 6:
				if (wire1LHPosition == newSpot) wire1LHPosition = wire7LHPosition;
				if (wire2LHPosition == newSpot) wire2LHPosition = wire7LHPosition;
				if (wire3LHPosition == newSpot) wire3LHPosition = wire7LHPosition;
				if (wire4LHPosition == newSpot) wire4LHPosition = wire7LHPosition;
				if (wire5LHPosition == newSpot) wire5LHPosition = wire7LHPosition;
				if (wire6LHPosition == newSpot) wire6LHPosition = wire7LHPosition;
				wire7LHPosition = newSpot;
				break;
		}
		selectedWire = -1;
		selectedWireLH = false;
		for (tempInt = 0; tempInt < 7; tempInt++) {
			leftNodeSelectedIndicators[tempInt].enabled = false;
			rightNodeSelectedIndicators[tempInt].enabled = false;
		}
		ChangeAppearance();
		EvaluatePuzzle();
	}

	public void MoveEndpointRight(int newSpot) {
		if (selectedWire == -1)	return;

		switch (selectedWire) {
			case 0:
				if (wire2RHPosition == newSpot) wire2RHPosition = wire1RHPosition;
				if (wire3RHPosition == newSpot) wire3RHPosition = wire1RHPosition;
				if (wire4RHPosition == newSpot) wire4RHPosition = wire1RHPosition;
				if (wire5RHPosition == newSpot) wire5RHPosition = wire1RHPosition;
				if (wire6RHPosition == newSpot) wire6RHPosition = wire1RHPosition;
				if (wire7RHPosition == newSpot) wire7RHPosition = wire1RHPosition;
				wire1RHPosition = newSpot;
				break;
			case 1:
				if (wire1RHPosition == newSpot) wire1RHPosition = wire2RHPosition;
				if (wire3RHPosition == newSpot) wire3RHPosition = wire2RHPosition;
				if (wire4RHPosition == newSpot) wire4RHPosition = wire2RHPosition;
				if (wire5RHPosition == newSpot) wire5RHPosition = wire2RHPosition;
				if (wire6RHPosition == newSpot) wire6RHPosition = wire2RHPosition;
				if (wire7RHPosition == newSpot) wire7RHPosition = wire2RHPosition;
				wire2RHPosition = newSpot;
				break;
			case 2:
				if (wire1RHPosition == newSpot) wire1RHPosition = wire3RHPosition;
				if (wire2RHPosition == newSpot) wire2RHPosition = wire3RHPosition;
				if (wire4RHPosition == newSpot) wire4RHPosition = wire3RHPosition;
				if (wire5RHPosition == newSpot) wire5RHPosition = wire3RHPosition;
				if (wire6RHPosition == newSpot) wire6RHPosition = wire3RHPosition;
				if (wire7RHPosition == newSpot) wire7RHPosition = wire3RHPosition;
				wire3RHPosition = newSpot;
				break;
			case 3:
				if (wire1RHPosition == newSpot) wire1RHPosition = wire4RHPosition;
				if (wire2RHPosition == newSpot) wire2RHPosition = wire4RHPosition;
				if (wire3RHPosition == newSpot) wire3RHPosition = wire4RHPosition;
				if (wire5RHPosition == newSpot) wire5RHPosition = wire4RHPosition;
				if (wire6RHPosition == newSpot) wire6RHPosition = wire4RHPosition;
				if (wire7RHPosition == newSpot) wire7RHPosition = wire4RHPosition;
				wire4RHPosition = newSpot;
				break;
			case 4:
				if (wire1RHPosition == newSpot) wire1RHPosition = wire5RHPosition;
				if (wire2RHPosition == newSpot) wire2RHPosition = wire5RHPosition;
				if (wire3RHPosition == newSpot) wire3RHPosition = wire5RHPosition;
				if (wire4RHPosition == newSpot) wire4RHPosition = wire5RHPosition;
				if (wire6RHPosition == newSpot) wire6RHPosition = wire5RHPosition;
				if (wire7RHPosition == newSpot) wire7RHPosition = wire5RHPosition;
				wire5RHPosition = newSpot;
				break;
			case 5:
				if (wire1RHPosition == newSpot) wire1RHPosition = wire6RHPosition;
				if (wire2RHPosition == newSpot) wire2RHPosition = wire6RHPosition;
				if (wire3RHPosition == newSpot) wire3RHPosition = wire6RHPosition;
				if (wire4RHPosition == newSpot) wire4RHPosition = wire6RHPosition;
				if (wire5RHPosition == newSpot) wire5RHPosition = wire6RHPosition;
				if (wire7RHPosition == newSpot) wire7RHPosition = wire6RHPosition;
				wire6RHPosition = newSpot;
				break;
			case 6:
				if (wire1RHPosition == newSpot) wire1RHPosition = wire7RHPosition;
				if (wire2RHPosition == newSpot) wire2RHPosition = wire7RHPosition;
				if (wire3RHPosition == newSpot) wire3RHPosition = wire7RHPosition;
				if (wire4RHPosition == newSpot) wire4RHPosition = wire7RHPosition;
				if (wire5RHPosition == newSpot) wire5RHPosition = wire7RHPosition;
				if (wire6RHPosition == newSpot) wire6RHPosition = wire7RHPosition;
				wire7RHPosition = newSpot;
				break;
		}
		selectedWire = -1;
		for (tempInt = 0; tempInt < 7; tempInt++) {
			leftNodeSelectedIndicators[tempInt].enabled = false;
			rightNodeSelectedIndicators[tempInt].enabled = false;
		}
		ChangeAppearance();
		EvaluatePuzzle();
	}

	void EvaluatePuzzle() {
		tempF = 0;
		if (wire1LHPosition == wire1LHTarget && wireIsActive[0]) tempF += 0.19f;
		if (wire2LHPosition == wire2LHTarget && wireIsActive[1]) tempF += 0.19f;
		if (wire3LHPosition == wire3LHTarget && wireIsActive[2]) tempF += 0.19f;
		if (wire4LHPosition == wire4LHTarget && wireIsActive[3]) tempF += 0.19f;
		if (wire5LHPosition == wire5LHTarget && wireIsActive[4]) tempF += 0.19f;
		if (wire6LHPosition == wire6LHTarget && wireIsActive[5]) tempF += 0.19f;
		if (wire7LHPosition == wire7LHTarget && wireIsActive[6]) tempF += 0.19f;
		if (wire1RHPosition == wire1RHTarget && wireIsActive[0]) tempF += 0.19f;
		if (wire2RHPosition == wire2RHTarget && wireIsActive[1]) tempF += 0.19f;
		if (wire3RHPosition == wire3RHTarget && wireIsActive[2]) tempF += 0.19f;
		if (wire4RHPosition == wire4RHTarget && wireIsActive[3]) tempF += 0.19f;
		if (wire5RHPosition == wire5RHTarget && wireIsActive[4]) tempF += 0.19f;
		if (wire6RHPosition == wire6RHTarget && wireIsActive[5]) tempF += 0.19f;
		if (wire7RHPosition == wire7RHTarget && wireIsActive[6]) tempF += 0.19f;
		if (Const.a.difficultyPuzzle == 1) tempF += 0.19f;
		actualValue = tempF;
		if (tempF > 0.92f || AllWiresCorrect()) PuzzleSolved(false);
	}

	bool AllWiresCorrect() {
		bool w1 = wireIsActive[0] ? (wire1LHPosition == wire1LHTarget && wire1RHPosition == wire1RHTarget): true;
 		bool w2 = wireIsActive[1] ? (wire2LHPosition == wire2LHTarget && wire2RHPosition == wire2RHTarget): true;
		bool w3 = wireIsActive[2] ? (wire3LHPosition == wire3LHTarget && wire3RHPosition == wire3RHTarget): true;
		bool w4 = wireIsActive[3] ? (wire4LHPosition == wire4LHTarget && wire4RHPosition == wire4RHTarget): true;
		bool w5 = wireIsActive[4] ? (wire5LHPosition == wire5LHTarget && wire5RHPosition == wire5RHTarget): true;
		bool w6 = wireIsActive[5] ? (wire6LHPosition == wire6LHTarget && wire6RHPosition == wire6RHTarget): true;
		bool w7 = wireIsActive[6] ? (wire7LHPosition == wire7LHTarget && wire7RHPosition == wire7RHTarget): true;
		return (w1 && w2 && w3 && w4 && w5 && w6 && w7);
	}

	void PuzzleSolved(bool usedLogicProbe) {
		actualValue = 1f;
		slider.value = actualValue;
		Solved = true;
		Utils.PlayOneShotSavable(SFXSource,SFX,1.0f);
		puzzleWP.puzzleSolved = true;
		puzzleWP.UseTargets(udSender.owner);
		if (usedLogicProbe) {
			MouseLookScript.a.ResetHeldItem();
			MouseLookScript.a.ResetCursor();
		}
	}
}
