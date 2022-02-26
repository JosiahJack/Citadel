using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ElevatorKeypad : MonoBehaviour {
	public GameObject[] buttons;
	public GameObject[] buttonHandlers;
	public bool[] buttonsEnabled;
	public bool[] buttonsDarkened;
	public string[] buttonText;
	public Sprite buttonNormal;
	public Sprite buttonDarkened;
	public Image[] buttonSprites;
	public Text[] buttonTextHolders;
	public Image currentFloorIndicator;
	public int currentFloor;
	[DTValidator.Optional] public GameObject[] targetDestination; // set by ElevatorButton.cs within the buttonHandlers[]
	public Sprite[] indicatorSprites;
	[HideInInspector] public GameObject activeKeypad;
	public Color textEnabledColor;
	public Color textDarkenedColor;

	public void SetCurrentFloor() {
		switch (currentFloor) {
		case 0: currentFloorIndicator.overrideSprite = indicatorSprites[0];
			break;
		case 1: currentFloorIndicator.overrideSprite = indicatorSprites[1];
			break;
		case 2: currentFloorIndicator.overrideSprite = indicatorSprites[2];
			break;
		case 3: currentFloorIndicator.overrideSprite = indicatorSprites[3];
			break;
		case 4: currentFloorIndicator.overrideSprite = indicatorSprites[4];
			break;
		case 5: currentFloorIndicator.overrideSprite = indicatorSprites[5];
			break;
		case 6: currentFloorIndicator.overrideSprite = indicatorSprites[6];
			break;
		case 7: currentFloorIndicator.overrideSprite = indicatorSprites[7];
			break;
		case 8: currentFloorIndicator.overrideSprite = indicatorSprites[8];
			break;
		case 9: currentFloorIndicator.overrideSprite = indicatorSprites[9];
			break;
		case 10: currentFloorIndicator.overrideSprite = indicatorSprites[10];
			break;
		}
	}
}
