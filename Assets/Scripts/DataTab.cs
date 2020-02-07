﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DataTab : MonoBehaviour {
	public GameObject headerText;
	public GameObject noItemsText;
	public GameObject blockedBySecurity;
	public GameObject elevatorUIControl;
	public GameObject keycodeUIControl;
	public GameObject[] searchItemImages;
	public GameObject audioLogContainer;
	public GameObject puzzleGrid;
	public GameObject puzzleWire;
	public Text headerText_text;
	public SearchButton searchContainer;
	public Vector3 objectInUsePos;
	public bool usingObject;
	public Transform playerCapsuleTransform;
	public bool isRH = false;
	private PuzzleGridPuzzle tetheredPGP = null;
	private PuzzleWirePuzzle tetheredPWP = null;

	void Awake () {
		Reset();
	}

	void Update () {
		if (usingObject) {
			if (Vector3.Distance(playerCapsuleTransform.position, objectInUsePos) > Const.a.frobDistance) {
				if (tetheredPGP != null) {
					tetheredPGP.SendDataBackToPanel(puzzleGrid.GetComponent<PuzzleGrid>());
					tetheredPGP = null;
				}

				if (tetheredPWP != null) {
					tetheredPWP.SendDataBackToPanel(puzzleWire.GetComponent<PuzzleWire>());
					tetheredPWP = null;
				}
				Reset();
				MFDManager.a.ReturnToLastTab(isRH);
			}
		}
	}

	public void Reset() {
		usingObject = false;
		headerText.SetActive(false);
		headerText_text.text = System.String.Empty;
		noItemsText.SetActive(false);
		blockedBySecurity.SetActive(false);
		elevatorUIControl.SetActive(false);
		keycodeUIControl.SetActive(false);
		puzzleGrid.SetActive(false);
		puzzleWire.SetActive(false);
		audioLogContainer.SetActive(false);
		for (int i=0; i<=3;i++) {
			searchItemImages[i].SetActive(false);
		}
	}

	public void Search(string head, int numberFoundContents, int[] contents, int[] customIndex, Vector3 searchPosition) {
		headerText.SetActive(true);
		headerText_text.enabled = true;
		headerText_text.text = head;
		usingObject = true;
		objectInUsePos = searchPosition;

		if (numberFoundContents <= 0) {
			noItemsText.SetActive(true);
			noItemsText.GetComponent<Text>().enabled = true;
			return;
		}

		for (int i=0;i<4;i++) {
			if (contents[i] > -1) {
				searchItemImages[i].SetActive(true);
				searchItemImages[i].GetComponent<Image>().overrideSprite = Const.a.searchItemIconSprites[contents[i]];
				searchContainer.contents[i] = contents[i];
				searchContainer.customIndex[i] = customIndex[i];
			}
		}
	}

	public void GridPuzzle(bool[] states, PuzzleGrid.CellType[] types, PuzzleGrid.GridType gtype, int start, int end, int width, int height, PuzzleGrid.GridColorTheme colors, string t1, UseData ud) {
		puzzleGrid.GetComponent<PuzzleGrid>().SendGrid(states,types,gtype,start,end, width, height,colors,t1,ud);
	}

	public void WirePuzzle(bool[] sentWiresOn, bool[] sentNodeRowsActive, int[] sentCurrentPositionsLeft, int[] sentCurrentPositionsRight, int[] sentTargetsLeft, int[] sentTargetsRight, PuzzleWire.WireColorTheme theme, PuzzleWire.WireColor[] wireColors, string t1, string a1, UseData udSent) {
		puzzleWire.GetComponent<PuzzleWire>().SendWirePuzzleData(sentWiresOn,sentNodeRowsActive,sentCurrentPositionsLeft,sentCurrentPositionsRight,sentTargetsLeft,sentTargetsRight,theme,wireColors,t1,a1,udSent);
	}
}
