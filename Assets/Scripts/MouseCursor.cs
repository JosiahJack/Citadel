using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MouseCursor : MonoBehaviour {
    public GameObject playerCamera;
	public GameObject uiCamera;
	private Camera uiCameraCam;
	public bool liveGrenade = false;
	public string toolTip = "";
	public Camera mainCamera;
	public RectTransform centerMFDPanel;
	public GameObject inventoryAddHelper;
    public float cursorSize = 24f;
    public bool offsetCentering = true;
    public Texture2D cursorImage;
    private float offsetX;
    private float offsetY;
	public bool justDroppedItemInHelper = false;
	public Rect drawTexture;
	public List<RectTransform> uiRaycastRects;
	public List<GameObject> uiRaycastRectGOs;
	public float cursorXmin;
	public float cursorYmin;
	public float cursorX;
	public float cursorY;
	public Vector2 cursorPosition;
	public float x;
	public float y;
	public float cursorXmax;
	public float cursorYmax;
	public GUIStyle liveGrenadeStyle;
	public GUIStyle toolTipStyle;
	public GUIStyle toolTipStyleLH;
	public GUIStyle toolTipStyleRH;
	public Handedness toolTipType;
	public Texture2D cursorLHTexture;
	public Texture2D cursorRHTexture;
	public Texture2D cursorDNTexture;
	private Texture2D tempTexture;
	public Texture2D cursorGUI;
	public RectTransform energySliderRect;
	public float cursorScreenPercentage = 0.02f;
	private float halfFactor = 0.5f;
	private float zero = 0f;
	private string nullStr = "-1";

	public static MouseCursor a;

	void Awake() {
		a = this;
		a.uiCameraCam = uiCamera.GetComponent<Camera>();
		cursorSize = Screen.width * cursorScreenPercentage;
		a.drawTexture = new Rect((Screen.width*halfFactor) - offsetX, (Screen.height * halfFactor) - cursorSize, cursorSize, cursorSize);
	}

	public void RegisterRaycastRect(GameObject go, RectTransform rectToAdd) {
		uiRaycastRects.Add(rectToAdd);
		uiRaycastRectGOs.Add(go);
	}

	void OnGUI () {
		if (MouseLookScript.a == null) return;

		cursorSize = Screen.width * cursorScreenPercentage;

        if (offsetCentering) {
            offsetX = cursorSize * halfFactor;
            offsetY = offsetX;
        }

		//Debug.Log("MouseCursor:: Input.mousePosition.x: " + Input.mousePosition.x.ToString() + ", Input.mousePosition.y: " + Input.mousePosition.y.ToString());
		if (MouseLookScript.a.inventoryMode || PauseScript.a.Paused() || PauseScript.a.MenuActive()) {
            // Inventory Mode Cursor
			drawTexture.Set(Input.mousePosition.x - offsetX,Screen.height - Input.mousePosition.y - offsetY,cursorSize,cursorSize);
        } else {
            // Shoot Mode Cursor
			drawTexture.Set((Screen.width*halfFactor) - offsetX, (Screen.height * halfFactor) - cursorSize, cursorSize, cursorSize);
        }
		cursorXmin = drawTexture.xMin;
		cursorYmin = drawTexture.yMin;
		cursorX = drawTexture.center.x;
		cursorY = drawTexture.center.y;
		x = cursorX;
		if (x < zero) x = zero;
		if (x > Screen.width) x = Screen.width;
		y = cursorY;
		cursorXmax = drawTexture.xMax;
		cursorYmax = drawTexture.yMax;
		if (!string.IsNullOrWhiteSpace(toolTip) && toolTip != nullStr && !PauseScript.a.Paused() && (MouseLookScript.a.inventoryMode || liveGrenade)) {
			switch(toolTipType) {
				case Handedness.LH: GUI.Label(drawTexture,toolTip,toolTipStyleLH); tempTexture = cursorLHTexture; break;
				case Handedness.RH: GUI.Label(drawTexture,toolTip,toolTipStyleRH); tempTexture = cursorRHTexture; break;
				default: GUI.Label(drawTexture,toolTip,toolTipStyle); tempTexture = cursorDNTexture; break; // Handedness.Center
			}
			if (!MouseLookScript.a.holdingObject) cursorImage = tempTexture;
		} else {
			if ((PauseScript.a.Paused() && !(PauseScript.a.mainMenu.activeSelf == true)) || GUIState.a.isBlocking && !MouseLookScript.a.holdingObject) {
				cursorImage = cursorGUI;
			} else {
				if (MouseLookScript.a.vmailActive) {
					cursorImage = Const.a.useableItemsFrobIcons[108];	// vmail
				} else {
					if (MouseLookScript.a.inCyberSpace) {
						cursorImage = MouseLookScript.a.cyberspaceCursor;
					} else {
						if (MouseLookScript.a.holdingObject && MouseLookScript.a.heldObjectIndex >= 0) {
							cursorImage = Const.a.useableItemsFrobIcons[MouseLookScript.a.heldObjectIndex];
						} else {
							switch(WeaponCurrent.a.weaponIndex) {
								case 36:
									cursorImage = Const.a.useableItemsFrobIcons[102];	// red
									break;
								case 37:
									cursorImage = Const.a.useableItemsFrobIcons[107];	// blue
									break;
								case 38:
									cursorImage = Const.a.useableItemsFrobIcons[102];	// red
									break;
								case 39:
									cursorImage = Const.a.useableItemsFrobIcons[105];	// green
									break;
								case 40:
									cursorImage = Const.a.useableItemsFrobIcons[107];	// blue
									break;
								case 41:
									cursorImage = Const.a.useableItemsFrobIcons[103];	// orange
									break;
								case 42:
									cursorImage = Const.a.useableItemsFrobIcons[103];	// orange
									break;
								case 43:
									cursorImage = Const.a.useableItemsFrobIcons[102];	// red
									break;
								case 44:
									cursorImage = Const.a.useableItemsFrobIcons[104];	// yellow
									break;
								case 45:
									cursorImage = Const.a.useableItemsFrobIcons[102];	// red
									break;
								case 46:
									cursorImage = Const.a.useableItemsFrobIcons[106];	// teal
									break;
								case 47:
									cursorImage = Const.a.useableItemsFrobIcons[104];	// yellow
									break;
								case 48:
									cursorImage = Const.a.useableItemsFrobIcons[102];	// red
									break;
								case 49:
									cursorImage = Const.a.useableItemsFrobIcons[105];	// green
									break;
								case 50:
									cursorImage = Const.a.useableItemsFrobIcons[107];	// blue
									break;
								case 51:
									cursorImage = Const.a.useableItemsFrobIcons[106];	// teal
									break;
								default:
									cursorImage = Const.a.useableItemsFrobIcons[105];	// green
									break;
							}
						}
					}
				}
			}
		}

		GUI.DrawTexture(drawTexture, cursorImage);
		if (liveGrenade && !PauseScript.a.Paused()) GUI.Label(drawTexture,Const.a.stringTable[586],liveGrenadeStyle); // Display "live" next to cursor
	}

	void Update() { 
		cursorPosition = new Vector2(cursorX,cursorY);
		cursorSize = (24f * (Screen.width/640f)); // This works well, not changing it from Screen.width/640f
		if (cursorPosition.y > (0.13541f*Screen.height) && cursorPosition.y < (0.70703f*Screen.height)
			&& cursorPosition.x < (0.96925f*Screen.width) && cursorPosition.x > (0.029282f*Screen.width)
			&& !PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			GUIState.a.isBlocking = false; // in the safe zone!
		}

		if (EventSystem.current.IsPointerOverGameObject()) GUIState.a.isBlocking = true;
		else GUIState.a.isBlocking = false;

		if (cursorPosition.y > Screen.height || cursorPosition.y < 0 || cursorPosition.x < 0 || cursorPosition.x > Screen.width && !PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			GUIState.a.isBlocking = true; // outside the screen, don't shoot we're innocent!
		}

		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (cursorPosition.y > (0.13541f*Screen.height) && cursorPosition.y < (0.70703f*Screen.height) && cursorPosition.x < (0.96925f*Screen.width) && cursorPosition.x > (0.029282f*Screen.width)) GUIState.a.isBlocking = false; // in the safe zone!
			if (MouseLookScript.a.inventoryMode && MouseLookScript.a.holdingObject) {
				// Be sure to pass the camera to the 3rd parameter if using "Screen Space - Camera" on the Canvas, otherwise use "null"
				if (RectTransformUtility.RectangleContainsScreenPoint(centerMFDPanel,Input.mousePosition,uiCameraCam)) {
					if (!inventoryAddHelper.activeInHierarchy) inventoryAddHelper.SetActive(true);
					GUIState.a.isBlocking = true;
				} else {
					if (inventoryAddHelper.activeInHierarchy) inventoryAddHelper.SetActive(false);
					if (justDroppedItemInHelper) {
						GUIState.a.PtrHandler(false,false,ButtonType.None,null);
						justDroppedItemInHelper = false; // only disable blocking state once, not constantly
					}
				}
			} else {
				if (justDroppedItemInHelper) {
					justDroppedItemInHelper = false; // only disable blocking state once, not constantly
					inventoryAddHelper.SetActive(false);
					GUIState.a.PtrHandler(false,false,ButtonType.None,null);
				}
			}
		}
	}
}
