using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemTextManager : MonoBehaviour {
    public void SetItemText(int index) {
        if (index >= 0) {
            GetComponent<Text>().text = Const.a.useableItemsNameText[index];
        }
	}
}
