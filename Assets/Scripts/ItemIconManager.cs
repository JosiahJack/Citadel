using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemIconManager : MonoBehaviour {
	public void SetItemIcon (int index) {
        if (index >= 0) {
            GetComponent<Image>().overrideSprite = Const.a.useableItemsIcons[index];
        }
	}
}
