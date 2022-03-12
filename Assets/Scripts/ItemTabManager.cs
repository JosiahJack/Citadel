using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemTabManager : MonoBehaviour {
    public GameObject iconManager;
    public GameObject textManager;
	public GameObject vaporizeButton;
	public GameObject applyButton;
	public GameObject useButton;
	public GameObject eReaderSectionsContainer;

	public void Reset () {
		eReaderSectionsContainer.SetActive(false);
		iconManager.GetComponent<Image>().overrideSprite = Const.a.useableItemsIcons[0]; //nullsprite
		textManager.GetComponent<Text>().text = System.String.Empty;
		applyButton.SetActive(false);
		vaporizeButton.SetActive(false);
		useButton.SetActive(false);
	}

	public void EReaderSectionSContainerOpen () {
		applyButton.SetActive(false);
		vaporizeButton.SetActive(false);
		useButton.SetActive(false);
		eReaderSectionsContainer.SetActive(true);
		iconManager.GetComponent<Image>().overrideSprite = Const.a.useableItemsIcons[23]; //datareader
		textManager.GetComponent<Text>().text = Const.a.useableItemsNameText[23];
	}

	public void SendItemDataToItemTab(int constIndex) {
		Reset();
		if (constIndex < 0) return;

		if (Const.a.useableItemsIcons[constIndex] != null) iconManager.GetComponent<Image>().overrideSprite = Const.a.useableItemsIcons[constIndex]; //datareader
		textManager.GetComponent<Text>().text = Const.a.useableItemsNameText[constIndex];
		if ((constIndex >= 14 && constIndex < 21) || constIndex == 52 || constIndex == 53 || constIndex == 55) {
			applyButton.SetActive(true);
			MFDManager.a.applyButtonReferenceIndex = constIndex;
		}

		if (constIndex < 6 || constIndex == 33 || constIndex == 35 ||
            (constIndex > 51 && constIndex < 63) || constIndex == 64 ||
            (constIndex > 91 && constIndex < 95)) {
			vaporizeButton.SetActive(true);
		}
	}
}
