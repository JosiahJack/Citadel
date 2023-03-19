using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text;

public class ItemTabManager : MonoBehaviour {
    public GameObject iconManager;
    public GameObject textManager;
	public GameObject vaporizeButton;
	public GameObject applyButton;
	public GameObject useButton;
	public GameObject eReaderSectionsContainer;
	public GameObject accessCardList;
	public Text accessCardListText;

	public void Reset() {
		eReaderSectionsContainer.SetActive(false);
		iconManager.GetComponent<Image>().overrideSprite =
			Const.a.useableItemsIcons[0]; //nullsprite

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
		iconManager.GetComponent<Image>().overrideSprite =
			Const.a.useableItemsIcons[23]; //datareader

		textManager.GetComponent<Text>().text =
			Const.a.useableItemsNameText[23];
	}

	public void SendItemDataToItemTab(int constIndex) {
		Reset();
		if (constIndex < 0) return;

		if (Const.a.useableItemsIcons[constIndex] != null) {
			iconManager.GetComponent<Image>().overrideSprite =
				Const.a.useableItemsIcons[constIndex]; //datareader
		}

		textManager.GetComponent<Text>().text = 
			Const.a.useableItemsNameText[constIndex];

		// Access Cards need special list enabled.
		if (constIndex == 34 || constIndex == 81 || constIndex == 110
			|| (constIndex >= 83 && constIndex <= 91)) {
			accessCardList.SetActive(true);
			AccessCardType acc = AccessCardType.Standard;
			Array cardTypes = Enum.GetValues(typeof(AccessCardType));
			StringBuilder s1 = new StringBuilder();
			s1.Clear();
			for (int i=0;i<cardTypes.Length;i++) {
				acc = (AccessCardType)cardTypes.GetValue(i);
				if (acc == AccessCardType.None) continue;

				if (Inventory.a.HasAccessCard(acc)) {
					s1.Append(" " + Inventory.AccessCardCodeForType(acc));
				}
			}

			accessCardListText.text = s1.ToString();
		} else {
			accessCardList.SetActive(false);
		}

		// Enable Apply button for consumables.
		if ((constIndex >= 14 && constIndex < 21) || constIndex == 52
			|| constIndex == 53 || constIndex == 55) {
			applyButton.SetActive(true);
			MFDManager.a.applyButtonReferenceIndex = constIndex;
		} else {
			applyButton.SetActive(false);
		}

		// Enable Vaporize button for junk.
		if (constIndex < 6 || constIndex == 33 || constIndex == 35
			|| constIndex == 58 || constIndex == 62
			|| (constIndex > 91 && constIndex < 95)) {
			vaporizeButton.SetActive(true);
		} else {
			vaporizeButton.SetActive(false);
		}
	}
}
