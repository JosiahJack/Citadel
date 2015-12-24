using UnityEngine;
using System.Collections;

public class MouseCursor : MonoBehaviour {
    public GameObject playerCamera;
    public float cursorSize = 32f;
    public bool offsetCentering = true;
    public Texture2D cursorImage;
    private float offsetX;
    private float offsetY;

	void OnGUI () {
        if (offsetCentering) {
            offsetX = cursorSize / 2;
            offsetY = offsetX;
        }

        if (playerCamera.GetComponent<MouseLookScript>().inventoryMode) {
            // Inventory Mode Cursor
            GUI.DrawTexture(new Rect(Input.mousePosition.x - offsetX, Screen.height - Input.mousePosition.y - offsetY, cursorSize, cursorSize), cursorImage);
        } else {
            // Shoot Mode Cursor
            GUI.DrawTexture(new Rect((Screen.width/2) - offsetX, (Screen.height/2) - cursorSize, cursorSize, cursorSize), cursorImage);
        }
	}
}
