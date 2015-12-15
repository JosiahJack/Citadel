using UnityEngine;
using System.Collections;

public class PatchUse : MonoBehaviour {
	public Texture2D cursorTexture;
	public int useableItemIndex;
	private Vector2 cursorHotspot;
	
	void Use (GameObject owner) {
		cursorHotspot = new Vector2 (cursorTexture.width/2, cursorTexture.height/2);
		Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		owner.GetComponent<MouseLookScript>().inventoryMode = true;  // Start with inventory mode turned on
		owner.GetComponent<MouseLookScript>().holdingObject = true;
		owner.GetComponent<MouseLookScript>().heldObjectIndex = useableItemIndex;
		Destroy(this.gameObject);
	}
}
