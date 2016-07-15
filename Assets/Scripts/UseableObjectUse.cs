using UnityEngine;
using System.Collections;

public class UseableObjectUse : MonoBehaviour {
	public Texture2D cursorTexture;
	public int useableItemIndex;
	public int customIndex = -1;
	private Vector2 cursorHotspot;
	private GameObject mouseCursor;

	void Awake() {
		mouseCursor = GameObject.Find("MouseCursorHandler");
		if (mouseCursor == null)
		{
			print("Warning: Could Not Find object 'MouseCursorHandler' in scene\n");
		}
	}

	void Use (GameObject owner) {
		mouseCursor.GetComponent<MouseCursor>().cursorImage = cursorTexture;
		Cursor.lockState = CursorLockMode.None;
		owner.GetComponent<MouseLookScript>().inventoryMode = true;  // inventory mode is turned on when picking something up
		owner.GetComponent<MouseLookScript>().holdingObject = true;
		owner.GetComponent<MouseLookScript>().heldObjectIndex = useableItemIndex;
		owner.GetComponent<MouseLookScript>().heldObjectCustomIndex = customIndex;
		Destroy(this.gameObject);
	}
}
