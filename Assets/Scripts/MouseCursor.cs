using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseCursor : MonoBehaviour {
    public GameObject playerCamera;
	public Camera mainCamera;
	public MouseLookScript playerCameraScript;
	public RectTransform centerMFDPanel;
	public GameObject inventoryAddHelper;
    public float cursorSize = 48f;
    public bool offsetCentering = true;
    public Texture2D cursorImage;
    private float offsetX;
    private float offsetY;
	public bool justDroppedItemInHelper = false;

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

	void Update () {
		if (playerCameraScript.inventoryMode && playerCameraScript.holdingObject) {
			if (RectTransformUtility.RectangleContainsScreenPoint(centerMFDPanel,Input.mousePosition,mainCamera)) {
				inventoryAddHelper.SetActive(true);
			} else {
				inventoryAddHelper.SetActive(false);
				if (justDroppedItemInHelper) {
					justDroppedItemInHelper = false; // only disable blocking state once, not constantly
					GUIState.a.isBlocking = false;
				}
			}
		} else {
			if (justDroppedItemInHelper) {
				justDroppedItemInHelper = false; // only disable blocking state once, not constantly
				inventoryAddHelper.SetActive(false);
				GUIState.a.isBlocking = false;
			}
		}
	}
}
