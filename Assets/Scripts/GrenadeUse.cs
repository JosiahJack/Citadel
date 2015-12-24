using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrenadeUse : MonoBehaviour {
	public Texture2D cursorTexture;
	public int useableItemIndex;
    private GameObject mouseCursor;

    void Awake () {
        mouseCursor = GameObject.Find("MouseCursorHandler");
    }

	void Use (GameObject owner) {
        //cursorHotspot = new Vector2 (cursorTexture.width/2, cursorTexture.height/2);
        //Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
        //Cursor.visible = true;
        mouseCursor.GetComponent<MouseCursor>().cursorImage = cursorTexture;
		Cursor.lockState = CursorLockMode.None;
		owner.GetComponent<MouseLookScript>().inventoryMode = true;  // Start with inventory mode turned on
		owner.GetComponent<MouseLookScript>().holdingObject = true;
		owner.GetComponent<MouseLookScript>().heldObjectIndex = useableItemIndex;
		Destroy(this.gameObject);
	}
}
