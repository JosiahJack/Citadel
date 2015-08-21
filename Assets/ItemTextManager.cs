using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemTextManager : MonoBehaviour {
	[SerializeField] public string[] itemTextLookUp;
	
	public void SetItemText (int index) {
		if (index >= 0)
			GetComponent<Text>().text = itemTextLookUp[index];
	}
}
