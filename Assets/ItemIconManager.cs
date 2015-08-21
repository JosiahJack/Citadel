using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemIconManager : MonoBehaviour {
	[SerializeField] public Sprite[] itemIconsLookUp;
	
	public void SetItemIcon (int index) {
		if (index >= 0)
			GetComponent<Image>().overrideSprite = itemIconsLookUp[index];
	}
}
