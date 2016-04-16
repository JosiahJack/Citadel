using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PatchButtonScript : MonoBehaviour {
	public int PatchButtonIndex;
	public GameObject playerCamera;
	public int useableItemIndex;
	private float doubleClickTime;
	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject textman;
	private int itemLookup;
	private Texture2D cursorTexture;
	private Vector2 cursorHotspot;
	private float dbclickFinished;
	public int doubleClickTicks;  // takes 2 to activate double click function

	void Awake () {
		doubleClickTime = Const.a.doubleClickTime;
		dbclickFinished = Time.time;
		doubleClickTicks = 0;
	}

	public void PtrEnter () {
		GUIState.isBlocking = true;
        playerCamera.GetComponent<MouseLookScript>().overButton = true;
        playerCamera.GetComponent<MouseLookScript>().overButtonType = 2;
        playerCamera.GetComponent<MouseLookScript>().currentButton = gameObject;
		doubleClickTicks = 0;
    }
	
	public void PtrExit () {
		GUIState.isBlocking = false;
        playerCamera.GetComponent<MouseLookScript>().overButton = false;
        playerCamera.GetComponent<MouseLookScript>().overButtonType = -1;
		doubleClickTicks = 0;
    }

	void Update () {
		if (dbclickFinished < Time.time) {
			doubleClickTicks--;
			if (doubleClickTicks < 0)
				doubleClickTicks = 0;
		}
	}

	void PatchInvClick () {
		//itemLookup = PatchCurrent.PatchInstance.patchInventoryIndices[PatchButtonIndex];
		//if (itemLookup < 0)
			//return;
		doubleClickTicks++;
		//if (dbclickFinished < Time.time) {
			if (doubleClickTicks == 2) {
				print("Double click!");
				doubleClickTicks = 0;
				dbclickFinished = Time.time + doubleClickTime;
				return;
			}
		//}
		iconman.GetComponent<ItemIconManager>().SetItemIcon(useableItemIndex);    //Set icon for MFD
		textman.GetComponent<ItemTextManager>().SetItemText(useableItemIndex); //Set text for MFD
		PatchCurrent.PatchInstance.patchCurrent = PatchButtonIndex;			//Set current
		dbclickFinished = Time.time + doubleClickTime;
	}

    void Start() {
        GetComponent<Button>().onClick.AddListener(() => { PatchInvClick(); });
    }
}