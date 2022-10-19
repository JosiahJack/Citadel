using UnityEngine;
using System.Collections;

public class UseableObjectUse : MonoBehaviour {
	public int useableItemIndex;
	public int customIndex = -1;
	public int ammo = 0;
	public int ammo2 = 0;
	private Texture2D tex;

	void Awake() {
		// 33% chance of not spawning logic probes on Puzzle difficulty of 3
		if (Const.a.difficultyPuzzle == 3) {
			if (useableItemIndex == 54) {
				if (UnityEngine.Random.Range(0,1f) < 0.33f) {
					gameObject.SetActive(false);
				}
			}
		}

		// Remove access cards on Mission difficulty 1 or 0
		if (Const.a.difficultyMission <= 1) {
			if (useableItemIndex >= 81 && useableItemIndex <= 91) {
				gameObject.SetActive(false); 
			}
		}

		// Remove audiologs on Mission difficulty 0
		if (Const.a.difficultyMission == 0) {
			if (useableItemIndex == 6) {
				gameObject.SetActive(false);
			}
		}
	}

	// Was GameObject owner as arguments, now UseData to hold more info.
	public void Use (UseData ud) {
		if (useableItemIndex < 0) Debug.Log("BUG: Oh crap, a useable has an index less than 0! UseableObjectUse.cs");
		tex = Const.a.useableItemsFrobIcons[useableItemIndex];
		if (tex != null) MouseCursor.a.cursorImage = tex;  // Set cursor to this object
		MouseLookScript.a.holdingObject = true;
		MouseLookScript.a.heldObjectIndex = useableItemIndex;
		MouseLookScript.a.heldObjectCustomIndex = customIndex;
		MouseLookScript.a.heldObjectAmmo = ammo;
		MouseLookScript.a.heldObjectAmmo2 = ammo2;
		if (Const.a.InputQuickItemPickup) {
			MouseLookScript.a.AddItemToInventory(useableItemIndex);
			MouseLookScript.a.ResetHeldItem();
			MouseLookScript.a.ResetCursor();
		} else {
			MouseLookScript.a.ForceInventoryMode();  // Inventory mode is turned on when picking something up
			Const.sprint(Const.a.useableItemsNameText[useableItemIndex] + Const.a.stringTable[319],ud.owner); // <item_name> picked up.
		}
		this.gameObject.SetActive(false); // We've been picked up, quick hide like you are actually in the player's hand
	}

	public void HitForce (DamageData dd) {
		Rigidbody rbody = GetComponent<Rigidbody>();
		if (rbody != null) rbody.AddForceAtPosition((dd.attacknormal*dd.damage),dd.hit.point); // knock me around will you
	}

	public static string Save(GameObject go) {
		UseableObjectUse uou = go.GetComponent<UseableObjectUse>();
		if (uou == null) {
			UnityEngine.Debug.Log("UseableObjectUse missing on saveable");
			return "-1|-1|0|0|BUG: Missing UseableObjectUse";
		}

		string line = System.String.Empty;
		line = uou.useableItemIndex.ToString(); // int - the main lookup index, needed for intanciating on load if doesn't match original SaveID
		line += Utils.splitChar + uou.customIndex.ToString(); // int - special reference like audiolog message
		line += Utils.splitChar + uou.ammo.ToString(); // int - how much normal ammo is on the weapon
		line += Utils.splitChar + uou.ammo2.ToString(); //int - alternate ammo type, e.g. Penetrator or Teflon
		return line;
	}
}