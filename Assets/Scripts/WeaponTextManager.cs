using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponTextManager : MonoBehaviour {
	[SerializeField] public string[] wepText;
	
	public void SetWepText (int index) {
		if (index >= 0)
			GetComponent<Text>().text = wepText[index];
	}
}
