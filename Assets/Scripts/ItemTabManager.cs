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
	public GameObject grenadeTimerSlider;

	public void Reset() {
		eReaderSectionsContainer.SetActive(false);
		iconManager.GetComponent<Image>().overrideSprite =
			Const.a.useableItemsIcons[0]; //nullsprite

		textManager.GetComponent<Text>().text = System.String.Empty;
		applyButton.SetActive(false);
		vaporizeButton.SetActive(false);
		useButton.SetActive(false);
		grenadeTimerSlider.SetActive(false);
	}

	public void EReaderSectionSContainerOpen () {
		applyButton.SetActive(false);
		vaporizeButton.SetActive(false);
		useButton.SetActive(false);
		grenadeTimerSlider.SetActive(false);
		eReaderSectionsContainer.SetActive(true);
		iconManager.GetComponent<Image>().overrideSprite =
			Const.a.useableItemsIcons[23]; //datareader

		textManager.GetComponent<Text>().text =
			Const.a.useableItemsNameText[23];
	}

	public void SendItemDataToItemTab(int constIndex, int customIndex) {
		Reset();
		if (constIndex < 0) return;

		if (constIndex == 92 || constIndex == 93 || constIndex == 94) { // Head
			int ind = 37; // Generic indeterminate head.
			switch(customIndex) {
				case 1: ind = 11; break; // Abe Ghiran
				case 2: ind = 32; break; // Mira Stackhouse
				case 3: ind =  1; break; // Baerga
				case 4: ind =  7; break; // D'Arcy
				case 5: ind =  9; break; // Diego (hope a mod uses this!)
				case 6: ind = 10; break; // Engle
				case 7: ind = 12; break; // Grossman
				case 8: ind = 13; break; // Hessman
				case 9: ind = 14; break; // Honig
				case 10: ind = 15; break; // Kell
				case 11: ind = 17; break; // Lansing (hope a mod uses this too!)
				case 12: ind = 25; break; // MacLeod
				case 13: ind = 27; break; // Parovski
				case 14: ind = 28; break; // Schuler unit
				case 15: ind = 31; break; // SHODAN (mods can be weird lol)
				case 16: ind = 33; break; // Stannek
				case 17: ind = 35; break; // Voyage of the Don Travers
				case 18: ind = 36; break; // Wong
				default: ind = 37; break; // You're not Wong there
			}

			if (ind >= 0 && ind < 38) {
				iconManager.GetComponent<Image>().overrideSprite =
					Const.a.logImages[ind];
			} else {
				iconManager.GetComponent<Image>().overrideSprite =
					Const.a.logImages[0];
			}
		} else {
			if (Const.a.useableItemsIcons[constIndex] != null) {
				iconManager.GetComponent<Image>().overrideSprite =
					Const.a.useableItemsIcons[constIndex]; //datareader
			}
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
			vaporizeButton.SetActive(false);
			grenadeTimerSlider.SetActive(false);
			MFDManager.a.applyButtonReferenceIndex = constIndex;
		} else {
			applyButton.SetActive(false);
		}

		// Enable Vaporize button for junk.
		if (constIndex < 6 || constIndex == 33 || constIndex == 35
			|| constIndex == 58 || constIndex == 62) {
			vaporizeButton.SetActive(true);
			grenadeTimerSlider.SetActive(false);
		} else {
			vaporizeButton.SetActive(false);
		}

		if (constIndex == 12 || constIndex == 10) { // Nitro and Earthshaker.
			grenadeTimerSlider.SetActive(true);
		}
	}

	public void SendItemDataToItemTab(int constIndex) {
		SendItemDataToItemTab(constIndex,-1);
	}
}
