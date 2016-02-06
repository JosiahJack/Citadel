using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponIconManager : MonoBehaviour {
	[SerializeField] public Sprite[] wepIcons;

	public void SetWepIcon (int index) {
		if (index >= 0)
			GetComponent<Image>().overrideSprite = Const.a.useableItemsIcons[index];
	}
}
