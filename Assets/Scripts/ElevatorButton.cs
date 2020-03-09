﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ElevatorButton : MonoBehaviour {
	public bool floorAccessible = false;
	public bool tooFarAway = false;
	public bool doorOpen = false;
	[HideInInspector]
	public GameObject targetDestination;
	private Text childText;
	private int levelIndex;
	public GameObject currentPlayer;
	private string levR = "R";
	private string lev1 = "1";
	private string lev2 = "2";
	private string lev3 = "3";
	private string lev4 = "4";
	private string lev5 = "5";
	private string lev6 = "6";
	private string lev7 = "7";
	private string lev8 = "8";
	private string lev9 = "9";
	private string levG1 = "G1";
	private string levG2 = "G2";
	private string levG4 = "G4";
	private string levC = "C";
	private int zero = 0; // because it's in an update and I don't want to generate garbage to collect in random spikes
	private int one = 1;
	private int two = 2;
	private int three = 3;
	private int four = 4;
	private int five = 5;
	private int six = 6;
	private int seven = 7;
	private int eight = 8;
	private int nine = 9;
	private int ten = 10;
	private int eleven = 11;
	private int twelve = 12;
	private int thirteen = 13;

	void Awake() {
		childText = GetComponentInChildren<Text>();
		doorOpen = false;
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { ElevButtonClick(); });
	}

	void ElevButtonClick () {
		if (tooFarAway) {
			Const.sprint(Const.a.stringTable[6],currentPlayer);
		} else {
			if (doorOpen) {
				Const.sprint(Const.a.stringTable[7],currentPlayer);
			} else {
				if (floorAccessible) {
					LevelManager.a.LoadLevel(levelIndex,targetDestination,currentPlayer);
				} else {
					Const.sprint(Const.a.stringTable[8],currentPlayer);
				}
			}
		}
	}

	public void SetTooFarTrue() { tooFarAway = true; }
	public void SetTooFarFalse() { tooFarAway = false; }

	void Update() {
		if (childText.text == levR) {
			levelIndex = zero;
		}
		if (childText.text == lev1) {
			levelIndex = one;
		}
		if (childText.text == lev2) {
			levelIndex = two;
		}
		if (childText.text == lev3) {
			levelIndex = three;
		}
		if (childText.text == lev4) {
			levelIndex = four;
		}
		if (childText.text == lev5) {
			levelIndex = five;
		}
		if (childText.text == lev6) {
			levelIndex = six;
		}
		if (childText.text == lev7) {
			levelIndex = seven;
		}
		if (childText.text == lev8) {
			levelIndex = eight;
		}
		if (childText.text == lev9) {
			levelIndex = nine;
		}
		if (childText.text == levG1) {
			levelIndex = ten;
		}
		if (childText.text == levG2) {
			levelIndex = eleven;
		}
		if (childText.text == levG4) {
			levelIndex = twelve;
		}
		if (childText.text == levC) {
			levelIndex = thirteen;
		}
	}
}
