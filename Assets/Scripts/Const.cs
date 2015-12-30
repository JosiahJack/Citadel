using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Const : MonoBehaviour {
	[SerializeField] public string grenade1 = "FRAGMENTATION\nGRENADE";
	[SerializeField] public GameObject[] useableItems;
	[SerializeField] public Texture2D[] useableItemsFrobIcons;
    [SerializeField] public string[] useableItemsNameText;
    public static Const a;

	// Instantiate it so that it can be accessed globally
	void Awake() {
		a = this;
	}
}
