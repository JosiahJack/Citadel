using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemTabManager : MonoBehaviour {
    public GameObject iconManager;
    public GameObject textManager;
	public GameObject eReaderSectionsContainer;
    public int itemTabType;
	public int lastCurrent = -1;

	private string nullstr;

	void Start () {
		lastCurrent = -1;
		nullstr = "";
		Reset();
	}

	public void Reset () {
		eReaderSectionsContainer.SetActive(false);
		iconManager.GetComponent<Image>().overrideSprite = Const.a.useableItemsIcons[0]; //nullsprite
		textManager.GetComponent<Text>().text = nullstr;
	}

	public void EReaderSectionSContainerOpen () {
		Const.sprint("openingEreaderSectioninMFDLH",Const.a.player1);
		eReaderSectionsContainer.SetActive(true);
		iconManager.GetComponent<Image>().overrideSprite = Const.a.useableItemsIcons[23]; //datareader
		textManager.GetComponent<Text>().text = Const.a.useableItemsNameText[23];
	}
}
