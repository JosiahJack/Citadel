using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

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
	public Vector2 cursorPosition;
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
		deltaX = deltaY = 0;
		lastMousePos = cursorPosition = Input.mousePosition;
	}

	#if UNITY_STANDALONE_LINUX
		[DllImport("libX11")]
		static extern IntPtr XOpenDisplay(string display);

		[DllImport("libX11")]
		static extern int XCloseDisplay(IntPtr display);

		[DllImport("libX11")]
		static extern int XWarpPointer(IntPtr display, IntPtr src_w, 
									   IntPtr dest_w, int src_x, int src_y,
									   uint src_width, uint src_height, 
									   int dest_x, int dest_y);
	#elif UNITY_STANDALONE_WIN
		[DllImport("user32.dll")]
		public static extern bool SetCursorPos(int X, int Y);
	#endif

	public static void SetCursorPosInternal(int x, int y) {
		// Still experiencing issues, best to live with this bug a while yet as
		// it is better to have consistent behavior rather than what this does.

		//#if UNITY_STANDALONE_LINUX
		//	IntPtr display = XOpenDisplay(null);
		//	if (display == IntPtr.Zero) {
		//		throw new Exception("Failed to open display");
		//	}

		//	Debug.Log("warping pointer to " + x.ToString() + ", " + y.ToString());
		//	XWarpPointer(display, IntPtr.Zero, IntPtr.Zero, 0, 0, 0, 0, x, y);
		//	XCloseDisplay(display);
		//#elif UNITY_STANDALONE_WIN
		//	SetCursorPos((int)(Screen.width * 0.5f),(int)(Screen.height * 0.5f));
		//#endif
	}

	public void RegisterRaycastRect(GameObject go, RectTransform rectToAdd) {
		uiRaycastRects.Add(rectToAdd);
		uiRaycastRectGOs.Add(go);
	}

	void OnGUI () {
		if (MouseLookScript.a == null) return;

		if (MouseLookScript.a.inventoryMode || PauseScript.a.Paused() || PauseScript.a.MenuActive()) {
            // Inventory Mode Cursor
			//drawTexture.Set(cursorPosition.x - offsetX,Screen.height - cursorPosition.y - offsetY,cursorSize,cursorSize);
			drawTexture.Set(Input.mousePosition.x - offsetX,Screen.height - Input.mousePosition.y - offsetY,cursorSize,cursorSize);
        } else {
            // Shoot Mode Cursor
			drawTexture.Set((Screen.width * halfFactor) - offsetX, (Screen.height * halfFactor) - cursorSize - offsetY, cursorSize, cursorSize);
        }

		if (!string.IsNullOrWhiteSpace(toolTip) && toolTip != nullStr && !PauseScript.a.Paused() && (MouseLookScript.a.inventoryMode || liveGrenade)) {
			switch(toolTipType) {
				case Handedness.LH: GUI.Label(drawTexture,toolTip,toolTipStyleLH); tempTexture = cursorLHTexture; break;
				case Handedness.RH: GUI.Label(drawTexture,toolTip,toolTipStyleRH); tempTexture = cursorRHTexture; break;
				default: GUI.Label(drawTexture,toolTip,toolTipStyle); tempTexture = cursorDNTexture; break; // Handedness.Center
			}
			if (!MouseLookScript.a.holdingObject) cursorImage = tempTexture;
		} else {
			if ((PauseScript.a.Paused() || PauseScript.a.MenuActive()) || GUIState.a.isBlocking && !MouseLookScript.a.holdingObject) {
				cursorImage = cursorGUI;
			} else if (MouseLookScript.a.vmailActive) {
				cursorImage = Const.a.useableItemsFrobIcons[108];	// vmail
			} else if (MouseLookScript.a.inCyberSpace) {
				cursorImage = MouseLookScript.a.cyberspaceCursor;
			} else if (MouseLookScript.a.holdingObject && MouseLookScript.a.heldObjectIndex >= 0) {
				cursorImage = Const.a.useableItemsFrobIcons[MouseLookScript.a.heldObjectIndex];
			} else {
				switch(WeaponCurrent.a.weaponIndex) {
					case 36: cursorImage = Const.a.useableItemsFrobIcons[102]; break; // red
					case 37: cursorImage = Const.a.useableItemsFrobIcons[107]; break; // blue
					case 38: cursorImage = Const.a.useableItemsFrobIcons[102]; break; // red
					case 39: cursorImage = Const.a.useableItemsFrobIcons[105]; break; // green
					case 40: cursorImage = Const.a.useableItemsFrobIcons[107]; break; // blue
					case 41: cursorImage = Const.a.useableItemsFrobIcons[103]; break; // orange
					case 42: cursorImage = Const.a.useableItemsFrobIcons[103]; break; // orange
					case 43: cursorImage = Const.a.useableItemsFrobIcons[102]; break; // red
					case 44: cursorImage = Const.a.useableItemsFrobIcons[104]; break; // yellow
					case 45: cursorImage = Const.a.useableItemsFrobIcons[102]; break; // red
					case 46: cursorImage = Const.a.useableItemsFrobIcons[106]; break; // teal
					case 47: cursorImage = Const.a.useableItemsFrobIcons[104]; break; // yellow
					case 48: cursorImage = Const.a.useableItemsFrobIcons[102]; break; // red
					case 49: cursorImage = Const.a.useableItemsFrobIcons[105]; break; // green
					case 50: cursorImage = Const.a.useableItemsFrobIcons[107]; break; // blue
					case 51: cursorImage = Const.a.useableItemsFrobIcons[106]; break; // teal
					default: cursorImage = Const.a.useableItemsFrobIcons[105]; break; // green
				}
			}
		}

		GUI.DrawTexture(drawTexture, cursorImage);
		if (liveGrenade && !PauseScript.a.Paused()) GUI.Label(drawTexture,Const.a.stringTable[586],liveGrenadeStyle); // Display "live" next to cursor
	}

	void Update() {
		if (Const.a.noHUD) {
			cursorSize = Screen.width * cursorScreenPercentage * 0.1f;// 1 pixel "beauty" cursor.
		} else {
			cursorSize = Screen.width * cursorScreenPercentage;
		}
		offsetX = cursorSize * halfFactor;
		offsetY = offsetX;
		cursorPosition = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
		cursorPosition.x = Mathf.Clamp(cursorPosition.x,0,Screen.width);
		cursorPosition.y = Mathf.Clamp(cursorPosition.y,0,Screen.height);
		lastMousePos = Input.mousePosition;
		UpdateSafeZone();
		UpdateEventSystemPointerStatus();
		CheckIfOutOfScreenBounds();
		UpdateInventoryAddHelper();

		// Maintain cursor mode.
		if (PauseScript.a.Paused() || PauseScript.a.MenuActive()) {
			Cursor.lockState = CursorLockMode.None;
			return;
		}

		if (MouseLookScript.a.inventoryMode) {
			#if UNITY_EDITOR
				Cursor.lockState = CursorLockMode.None;
			#else	
				Cursor.lockState = CursorLockMode.Confined;
			#endif

			if (GUIState.a.overButton || GUIState.a.overButtonType != ButtonType.None) {
				GUIState.a.isBlocking = true;
			}
		} else {
			Cursor.lockState = CursorLockMode.Locked;
			GUIState.a.isBlocking = false;
		}
	}

	void UpdateSafeZone() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

		if (cursorPosition.x < (0.96925f * Screen.width) && cursorPosition.x > (0.029282f * Screen.width)
			&& cursorPosition.y > (0.13541f * Screen.height) && cursorPosition.y < (0.70703f * Screen.height)) {
			GUIState.a.isBlocking = false; // in the safe zone!
		}
	}

	void UpdateEventSystemPointerStatus() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

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
		if (PauseScript.a.MenuActive()) return;
		if (PauseScript.a.Paused()) return;

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
			if (RectTransformUtility.RectangleContainsScreenPoint(centerMFDPanel,cursorPosition,uiCameraCam)) {
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

	public Vector3 GetCursorScreenPointForRay() {
		Vector3 retval;

		//retval.x = drawTexture.x+(drawTexture.width/2f);
		retval.x = drawTexture.center.x;

		// Flip it. Rect uses y=0 UL corner, ScreenPointToRay uses y=0 LL corner
		//retval.y = Screen.height - drawTexture.y+(drawTexture.height/2f);
		retval.y = Screen.height - drawTexture.center.y;
		retval.z = 0f;
		return retval;
	}
}
