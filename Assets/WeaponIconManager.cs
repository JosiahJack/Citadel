using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponIconManager : MonoBehaviour {
	[SerializeField] public Sprite[] wepIcons;

	public void SetWepIcon (int index) {
		if (index > -1)
			GetComponent<Image>().overrideSprite = wepIcons[index];
	}
}
