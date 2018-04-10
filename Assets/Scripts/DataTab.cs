using UnityEngine;
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

	void Awake () {
		Reset();
	}

	void Update () {
		if (usingObject) {
			if (Vector3.Distance(playerCapsuleTransform.position, objectInUsePos) > Const.a.frobDistance) {
				Reset();
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

	public void Search(string head, int numberFoundContents, int[] contents, int[] customIndex) {
		headerText.SetActive(true);
		headerText_text.enabled = true;
		headerText_text.text = head;

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

	public void GridPuzzle(bool[] states, PuzzleGrid.CellType[] types, PuzzleGrid.GridType gtype, int start, int end, int width, int height, PuzzleGrid.GridColorTheme colors) {
		puzzleGrid.GetComponent<PuzzleGrid>().SendGrid(states,types,gtype,start,end, width, height,colors);
	}
}
