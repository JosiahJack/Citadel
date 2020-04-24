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

	private string nullstr;

	void Start () {
		nullstr = "";
		//Reset();
	}

	public void Reset () {
		//Debug.Log("Resetted ItemTabManager");
		eReaderSectionsContainer.SetActive(false);
		iconManager.GetComponent<Image>().overrideSprite = Const.a.useableItemsIcons[0]; //nullsprite
		textManager.GetComponent<Text>().text = nullstr;
		applyButton.SetActive(false);
		vaporizeButton.SetActive(false);
		useButton.SetActive(false);
	}

	public void EReaderSectionSContainerOpen () {
		applyButton.SetActive(false);
		vaporizeButton.SetActive(false);
		useButton.SetActive(false);
		//Const.sprint("openingEreaderSectioninMFDLH",Const.a.player1);
		eReaderSectionsContainer.SetActive(true);
		iconManager.GetComponent<Image>().overrideSprite = Const.a.useableItemsIcons[23]; //datareader
		textManager.GetComponent<Text>().text = Const.a.useableItemsNameText[23];
	}

	public void SendItemDataToItemTab(int constIndex) {
		if (constIndex < 0) return;
		eReaderSectionsContainer.SetActive(false);
		if (Const.a.useableItemsIcons[constIndex] != null) iconManager.GetComponent<Image>().overrideSprite = Const.a.useableItemsIcons[constIndex]; //datareader
		textManager.GetComponent<Text>().text = Const.a.useableItemsNameText[constIndex];
	}
}
