using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseCursor : MonoBehaviour {
    public GameObject playerCamera;
	public GameObject uiCamera;
	public bool liveGrenade = false;
	public string toolTip = "";
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
	public GUIStyle liveGrenadeStyle;
	public GUIStyle toolTipStyle;
	public GUIStyle toolTipStyleLH;
	public GUIStyle toolTipStyleRH;
	public Handedness toolTipType;
	public Texture2D cursorLHTexture;
	public Texture2D cursorRHTexture;
	public Texture2D cursorDNTexture;
	public Texture2D cursorOverButtonTexture;
	private Texture2D tempTexture;
	public WeaponCurrent wepCurrent;

	void OnGUI () {
        if (offsetCentering) {
            offsetX = cursorSize / 2;
            offsetY = offsetX;
        }
			
		//Debug.Log("MouseCursor:: Input.mousePosition.x: " + Input.mousePosition.x.ToString() + ", Input.mousePosition.y: " + Input.mousePosition.y.ToString());
		if (playerCameraScript.inventoryMode || PauseScript.a.Paused()) {
            // Inventory Mode Cursor
			drawTexture = new Rect(Input.mousePosition.x - offsetX, Screen.height - Input.mousePosition.y - offsetY, cursorSize, cursorSize);
        } else {
            // Shoot Mode Cursor
			drawTexture = new Rect((Screen.width/2) - offsetX, (Screen.height/2) - cursorSize, cursorSize, cursorSize);
        }
		drawTextureInspector = drawTexture;
		cursorXmin = drawTexture.xMin;
		cursorYmin = drawTexture.yMin;
		cursorX = drawTexture.center.x;
		cursorY = drawTexture.center.y;
		x = cursorX;
		y = cursorY;
		cursorXmax = drawTexture.xMax;
		cursorYmax = drawTexture.yMax;

		if (!string.IsNullOrWhiteSpace(toolTip) && toolTip != "-1" && !PauseScript.a.Paused()) {
			switch(toolTipType) {
			case Handedness.LH: GUI.Label(drawTexture,toolTip,toolTipStyleLH); tempTexture = cursorLHTexture; break;
			case Handedness.RH: GUI.Label(drawTexture,toolTip,toolTipStyleRH); tempTexture = cursorRHTexture; break;
			default: GUI.Label(drawTexture,toolTip,toolTipStyle); tempTexture = cursorDNTexture; break; // Handedness.Center
			}

			if (!playerCameraScript.holdingObject) {
				cursorImage = tempTexture;
			}
		} else {
			if (playerCameraScript.holdingObject) {
				cursorImage = Const.a.useableItemsFrobIcons[playerCameraScript.heldObjectIndex];
			} else {
				//cursorImage = Const.a.useableItemsFrobIcons[115];
				switch(wepCurrent.weaponIndex) {
					case 36:
						cursorImage = Const.a.useableItemsFrobIcons[116];	// red
						break;
					case 37:
						cursorImage = Const.a.useableItemsFrobIcons[125];	// blue
						break;
					case 38:
						cursorImage = Const.a.useableItemsFrobIcons[116];	// red
						break;
					case 39:
						cursorImage = Const.a.useableItemsFrobIcons[115];	// green
						break;
					case 40:
						cursorImage = Const.a.useableItemsFrobIcons[125];	// blue
						break;
					case 41:
						cursorImage = Const.a.useableItemsFrobIcons[126];	// orange
						break;
					case 42:
						cursorImage = Const.a.useableItemsFrobIcons[126];	// orange
						break;
					case 43:
						cursorImage = Const.a.useableItemsFrobIcons[116];	// red
						break;
					case 44:
						cursorImage = Const.a.useableItemsFrobIcons[123];	// yellow
						break;
					case 45:
						cursorImage = Const.a.useableItemsFrobIcons[116];	// red
						break;
					case 46:
						cursorImage = Const.a.useableItemsFrobIcons[127];	// teal
						break;
					case 47:
						cursorImage = Const.a.useableItemsFrobIcons[123];	// yellow
						break;
					case 48:
						cursorImage = Const.a.useableItemsFrobIcons[116];	// red
						break;
					case 49:
						cursorImage = Const.a.useableItemsFrobIcons[115];	// green
						break;
					case 50:
						cursorImage = Const.a.useableItemsFrobIcons[125];	// blue
						break;
					case 51:
						cursorImage = Const.a.useableItemsFrobIcons[127];	// teal
						break;
					default:
						cursorImage = Const.a.useableItemsFrobIcons[115];	// green
						break;
				}
			}
		}

		GUI.DrawTexture(drawTexture, cursorImage);
		if (liveGrenade && !PauseScript.a.Paused()) {
			GUI.Label(drawTexture,"live",liveGrenadeStyle);
		}
	}

	void Update () {
		cursorSize = (24f * (Screen.width/640f));
		if (playerCameraScript.inventoryMode && playerCameraScript.holdingObject && !PauseScript.a.Paused()) {
			// Be sure to pass the camera to the 3rd parameter if using "Screen Space - Camer" on the Canvas, otherwise use "null"
			if (RectTransformUtility.RectangleContainsScreenPoint(centerMFDPanel,Input.mousePosition,uiCamera.GetComponent<Camera>())) {
				if (!inventoryAddHelper.activeInHierarchy) inventoryAddHelper.SetActive(true);
			} else {
				if (inventoryAddHelper.activeInHierarchy) inventoryAddHelper.SetActive(false);
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
