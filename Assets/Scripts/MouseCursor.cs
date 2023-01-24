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
    public Texture2D cursorImage;
    private float offsetX;
    private float offsetY;
	public bool justDroppedItemInHelper = false;
	public Rect drawTexture;
	public List<RectTransform> uiRaycastRects;
	public List<GameObject> uiRaycastRectGOs;
	public float cursorXmin;
	public float cursorYmin;
	public float debugRawX;
	public float debugRawY;
	public Vector2 cursorPosition;
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
	public float deltaX;
	public float deltaY;
	public Vector2 lastMousePos;
	private string nullStr = "-1";
	public GraphicRaycaster raycaster;

	public static MouseCursor a;

	void Awake() {
		a = this;
		a.uiCameraCam = uiCamera.GetComponent<Camera>();
		cursorSize = Screen.width * cursorScreenPercentage;
		a.drawTexture = new Rect((Screen.width*halfFactor) - offsetX, (Screen.height * halfFactor) - cursorSize, cursorSize, cursorSize);
		deltaX = Input.mousePosition.x;
		deltaY = Input.mousePosition.y;
		lastMousePos = Input.mousePosition;
		debugRawX = Input.mousePosition.x;
		debugRawY = Input.mousePosition.y;
	}

	public void RegisterRaycastRect(GameObject go, RectTransform rectToAdd) {
		uiRaycastRects.Add(rectToAdd);
		uiRaycastRectGOs.Add(go);
	}

	void OnGUI () {
		if (MouseLookScript.a == null) return;

		//Debug.Log("MouseCursor:: Input.mousePosition.x: " + Input.mousePosition.x.ToString() + ", Input.mousePosition.y: " + Input.mousePosition.y.ToString());
		if (MouseLookScript.a.inventoryMode || PauseScript.a.Paused() || PauseScript.a.MenuActive()) {
            // Inventory Mode Cursor
			//drawTexture.Set((Input.mousePosition.x) - offsetX,Screen.height - (Input.mousePosition.y) - offsetY,cursorSize,cursorSize);
			drawTexture.Set(cursorPosition.x - offsetX,Screen.height - cursorPosition.y - offsetY,cursorSize,cursorSize);
        } else {
            // Shoot Mode Cursor
			drawTexture.Set((Screen.width*halfFactor) - offsetX, (Screen.height * halfFactor) - cursorSize, cursorSize, cursorSize);
        }
		cursorXmin = drawTexture.xMin;
		cursorYmin = drawTexture.yMin;
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
		debugRawX = Input.mousePosition.x;
		debugRawY = Input.mousePosition.y;
		cursorSize = Screen.width * cursorScreenPercentage;
		offsetX = cursorSize * halfFactor;
		offsetY = offsetX;
		deltaX = (float)Input.mousePosition.x - lastMousePos.x;
		deltaY = (float)Input.mousePosition.y - lastMousePos.y;
		deltaX = Mathf.Clamp(deltaX,-Screen.width,Screen.width);
		deltaY = Mathf.Clamp(deltaY,-Screen.height,Screen.height);
		if (PauseScript.a.Paused() || PauseScript.a.MenuActive()) {
			cursorPosition = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
		} else {
			cursorPosition = new Vector2(cursorPosition.x + deltaX,cursorPosition.y + deltaY);
		}
		cursorPosition.x = Mathf.Clamp(cursorPosition.x,0,Screen.width);
		cursorPosition.y = Mathf.Clamp(cursorPosition.y,0,Screen.height);
		lastMousePos = Input.mousePosition;
		UpdateSafeZone();
		UpdateEventSystemPointerStatus();
		CheckIfOutOfScreenBounds(); 
		UpdateInventoryAddHelper();
	}

	void UpdateSafeZone() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

		if (cursorPosition.x < (0.96925f * Screen.width) && cursorPosition.x > (0.029282f * Screen.width)
			&& cursorPosition.y > (0.13541f * Screen.height) && cursorPosition.y < (0.70703f * Screen.height)) {
			GUIState.a.isBlocking = false; // in the safe zone!
		}
	}

	// This method was written by an AI and compiled with no changes.
	// I then needed to update this to assign 'raycaster' in the inspector
	// to the GraphicRaycaster that is on Canvas.  And badda bing badda boom.
	// Later updated with an AI again to add custom callback handling for
	// OnPointerEnter and OnPointerExit to have my fake cursor force these.
	// But it wasn't quite right so I fixed it.  Dang rampant AI's.
	void UpdateEventSystemPointerStatus() {
		Cursor.visible = false;
		if (PauseScript.a.Paused() || PauseScript.a.MenuActive()) {
			GUIState.a.isBlocking = true;
			return;
		}

		PointerEventData pointerData = new PointerEventData(EventSystem.current);
		pointerData.position = cursorPosition;
		List<RaycastResult> results = new List<RaycastResult>();
		raycaster.Raycast(pointerData, results);
		if (results.Count > 0) {
			GUIState.a.isBlocking = true;
			EventSystem.current.SetSelectedGameObject(results[0].gameObject);
			EventTrigger evt = results[0].gameObject.GetComponent<EventTrigger>();
			if (evt != null) {
				RectTransform recTr = results[0].gameObject.GetComponent<RectTransform>();
				if (recTr != null) {
					if (RectTransformUtility.RectangleContainsScreenPoint(recTr,cursorPosition,uiCameraCam)) {
						evt.OnPointerEnter(pointerData);
					} else {
						evt.OnPointerExit(pointerData);
					}
				}
			}
			if (Input.GetMouseButtonDown(0)) {
				ExecuteEvents.Execute(results[0].gameObject, pointerData, ExecuteEvents.submitHandler);
			}
		} else {
			GUIState.a.isBlocking = false;
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	void CheckIfOutOfScreenBounds() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

		if (cursorPosition.y > Screen.height || cursorPosition.y < 0
			|| cursorPosition.x < 0 || cursorPosition.x > Screen.width) {
			GUIState.a.isBlocking = true; // outside the screen, don't shoot we're innocent!
		}
	}

	void UpdateInventoryAddHelper() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

		if (cursorPosition.y > (0.13541f*Screen.height)
			&& cursorPosition.y < (0.70703f*Screen.height)
			&& cursorPosition.x < (0.96925f*Screen.width)
			&& cursorPosition.x > (0.029282f*Screen.width)) {
			GUIState.a.isBlocking = false; // in the safe zone!
		}

		if (MouseLookScript.a.inventoryMode && MouseLookScript.a.holdingObject) {
			// Be sure to pass the camera to the 3rd parameter if using
			// "Screen Space - Camera" on the Canvas, otherwise use "null"
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
