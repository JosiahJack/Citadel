using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DataTab : MonoBehaviour {
	public GameObject headerText;
	public GameObject noItemsText;
	public GameObject elevatorUIControl;
	public GameObject keycodeUIControl;
	public GameObject[] searchItemImages;
	public Text headerText_text;
	public SearchButtonsScript searchContainer;

	void Awake () {
		Reset();
	}

	public void Reset() {
		headerText.SetActive(false);
		headerText_text.text = "";
		noItemsText.SetActive(false);
		elevatorUIControl.SetActive(false);
		keycodeUIControl.SetActive(false);
		for (int i=0; i<=3;i++) {
			searchItemImages[i].SetActive(false);
		}
	}

	public void Search(string head, int numberFoundContents, int[] contents) {
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
			}
		}
	}
}
