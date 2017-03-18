using UnityEngine;
using System.Collections;

public class UseableObjectUse : MonoBehaviour {
	public Texture2D cursorTexture;
	public int useableItemIndex;
	public int customIndex = -1;
	private Vector2 cursorHotspot;
	private GameObject mouseCursor;

	void Awake() {
		mouseCursor = GameObject.Find("MouseCursorHandler");
		if (mouseCursor == null)
		{
			Const.sprint("Warning: Could Not Find object 'MouseCursorHandler' in scene\n",Const.a.allPlayers);
		}
	}

	void Use (GameObject owner) {
		mouseCursor.GetComponent<MouseCursor>().cursorImage = cursorTexture;
		Cursor.lockState = CursorLockMode.None;
		MouseLookScript mlook = owner.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<MouseLookScript>();
		mlook.ForceInventoryMode();  // inventory mode is turned on when picking something up
		mlook.holdingObject = true;
		mlook.heldObjectIndex = useableItemIndex;
		mlook.heldObjectCustomIndex = customIndex;
		this.gameObject.SetActive(false);
	}
}
