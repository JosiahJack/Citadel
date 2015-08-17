using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponButtonScript : MonoBehaviour {
	void WeaponInvClick () {
		print("WeaponButtonClicked!");
	}

	[SerializeField] private Button WepButton = null; // assign in the editor
	void Start() {
		WepButton.onClick.AddListener(() => { WeaponInvClick();});
	}
}
