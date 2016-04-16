using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Const : MonoBehaviour {
	[SerializeField] public string grenade1 = "FRAGMENTATION\nGRENADE";
	[SerializeField] public GameObject[] useableItems;
	[SerializeField] public Texture2D[] useableItemsFrobIcons;
    [SerializeField] public Sprite[] useableItemsIcons;
    [SerializeField] public string[] useableItemsNameText;
	public float doubleClickTime = 0.500f;
	public float frobDistance = 4.5f;
	public GameObject statusBar;
    public static Const a;

	// Instantiate it so that it can be accessed globally
	void Awake() {
		a = this;
	}

	// Check if particular bit is 1 (ON/TRUE) in binary format of given integer
	public bool CheckFlags (int checkInt, int flag) {
		if ((checkInt & flag) != 0)
			return true;

		return false;
	}

	// StatusBar Print
	public static void sprint (string input) {
		print(input);  // print to console
		if (a != null) {
			if (a.statusBar != null)
				a.statusBar.GetComponent<StatusBarTextDecay>().SendText(input);
		}
	}
}
