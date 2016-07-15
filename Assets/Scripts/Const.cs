using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Const : MonoBehaviour {
	[SerializeField] public GameObject[] useableItems;
	[SerializeField] public Texture2D[] useableItemsFrobIcons;
    [SerializeField] public Sprite[] useableItemsIcons;
    [SerializeField] public string[] useableItemsNameText;
	[SerializeField] public Sprite[] searchItemIconSprites;
	[SerializeField] public string[] audiologNames;
	[SerializeField] public AudioClip[] audioLogs;
	[SerializeField] public bool[] audioLogHasAudio;
	[SerializeField] public string[] audioLogSpeech2Text;
	public float doubleClickTime = 0.500f;
	public float frobDistance = 4.5f;
	public enum PoolType{DartImpacts};
	public GameObject Pool_DartImpacts;
	public GameObject statusBar;
    public static Const a;

	// Instantiate it so that it can be accessed globally. MOST IMPORTANT PART!!
	// =========================================================================
	void Awake() {a = this; }
	// =========================================================================

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

	public GameObject GetObjectFromPool(PoolType pool) {
		if (Pool_DartImpacts == null) {
			sprint("Cannot find pool of type PoolType.DartImpacts");
			return null;
		}

		switch (pool) {
		case PoolType.DartImpacts: 
			for (int i=0;i<Pool_DartImpacts.transform.childCount;i++) {
				Transform child = Pool_DartImpacts.transform.GetChild(i);
				if (child.gameObject.activeInHierarchy == false) {
					//sprint("Found a DartImpact!");
					return child.gameObject;
				}
			}
			sprint("Warning: No free objects in DartImpacts pool");
			return null;
		}

		return null;
	}
}
