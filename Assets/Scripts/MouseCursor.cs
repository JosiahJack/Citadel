using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseCursor : MonoBehaviour {
    public GameObject playerCamera;
	public GameObject uiCamera;
	public Camera mainCamera;
	public MouseLookScript playerCameraScript;
	public RectTransform centerMFDPanel;
	public GameObject inventoryAddHelper;
    public float cursorSize = 24f;
    public bool offsetCentering = true;
    public Texture2D cursorImage;
    private float offsetX;
    private float offsetY;
	public bool justDroppedItemInHelper = false;
	public static Rect drawTexture;
	public Rect drawTextureInspector;
	public static float cursorXmin;
	public static float cursorYmin;
	public static float cursorX;
	public static float cursorY;
	public float x;
	public float y;
	public static float cursorXmax;
	public static float cursorYmax;

	void OnGUI () {
        if (offsetCentering) {
            offsetX = cursorSize / 2;
            offsetY = offsetX;
        }
			
		//Debug.Log("MouseCursor:: Input.mousePosition.x: " + Input.mousePosition.x.ToString() + ", Input.mousePosition.y: " + Input.mousePosition.y.ToString());
		if (playerCameraScript.inventoryMode || PauseScript.a.paused) {
            // Inventory Mode Cursor
			drawTexture = new Rect(Input.mousePosition.x - offsetX, Screen.height - Input.mousePosition.y - offsetY, cursorSize, cursorSize);
			drawTextureInspector = drawTexture;
			cursorXmin = drawTexture.xMin;
			cursorYmin = drawTexture.yMin;
			cursorX = drawTexture.center.x;
			cursorY = drawTexture.center.y;
			x = cursorX;
			y = cursorY;
			cursorXmax = drawTexture.xMax;
			cursorYmax = drawTexture.yMax;
			GUI.DrawTexture(drawTexture, cursorImage);
        } else {
            // Shoot Mode Cursor
			drawTexture = new Rect((Screen.width/2) - offsetX, (Screen.height/2) - cursorSize, cursorSize, cursorSize);
			drawTextureInspector = drawTexture;
			cursorXmin = drawTexture.xMin;
			cursorYmin = drawTexture.yMin;
			cursorX = drawTexture.center.x;
			cursorY = drawTexture.center.y;
			x = cursorX;
			y = cursorY;
			cursorXmax = drawTexture.xMax;
			cursorYmax = drawTexture.yMax;
			GUI.DrawTexture(drawTexture, cursorImage);
        }
	}

	void Update () {
		cursorSize = (24f * (Screen.width/640f));
		if (playerCameraScript.inventoryMode && playerCameraScript.holdingObject && !PauseScript.a.paused) {
			// Be sure to pass the camera to the 3rd parameter if using "Screen Space - Camer" on the Canvas, otherwise use "null"
			if (RectTransformUtility.RectangleContainsScreenPoint(centerMFDPanel,Input.mousePosition,uiCamera.GetComponent<Camera>())) {
				inventoryAddHelper.SetActive(true);
			} else {
				inventoryAddHelper.SetActive(false);
				if (justDroppedItemInHelper) {
					GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
					justDroppedItemInHelper = false; // only disable blocking state once, not constantly
				}
			}
		} else {
			if (justDroppedItemInHelper) {
				justDroppedItemInHelper = false; // only disable blocking state once, not constantly
				inventoryAddHelper.SetActive(false);
				GUIState.a.PtrHandler(false,false,GUIState.ButtonType.None,null);
			}
		}
	}
}
