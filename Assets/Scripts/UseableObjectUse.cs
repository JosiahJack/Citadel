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
					Utils.SafeDestroy(gameObject);
				}
			}
		}

		// Remove access cards on Mission difficulty 1 or 0
		if (Const.a.difficultyMission <= 1) {
			if (useableItemIndex >= 81 && useableItemIndex <= 91) {
				Utils.SafeDestroy(gameObject);
			}
		}

		// Remove audiologs on Mission difficulty 0
		if (Const.a.difficultyMission == 0) {
			if (useableItemIndex == 6) Utils.SafeDestroy(gameObject);
		}
	}

	// Was GameObject owner as arguments, now UseData to hold more info.
	public void Use (UseData ud) {
		if (useableItemIndex < 0) Debug.Log("BUG: Useable index less than 0!");
		tex = Const.a.useableItemsFrobIcons[useableItemIndex];
		if (tex != null) MouseCursor.a.cursorImage = tex; // Set cursor to this object
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
		Utils.SafeDestroy(gameObject);
	}

	public void HitForce (DamageData dd) {
		Rigidbody rbody = GetComponent<Rigidbody>();
		if (rbody != null) rbody.AddForceAtPosition((dd.attacknormal*dd.damage),dd.hit.point); // knock me around will you
	}

	public static string Save(GameObject go) {
		UseableObjectUse uou = go.GetComponent<UseableObjectUse>();
		if (uou == null) {
			Debug.Log("UseableObjectUse missing on saveable");
			return "-1|-1|0|0|BUG: Missing UseableObjectUse";
		}

		string line = System.String.Empty;
		line = Utils.UintToString(uou.useableItemIndex); // int - the main lookup index, needed for intanciating on load if doesn't match original SaveID
		line += Utils.splitChar + Utils.UintToString(uou.customIndex); // int - special reference like audiolog message
		line += Utils.splitChar + Utils.UintToString(uou.ammo); // int - how much normal ammo is on the weapon
		line += Utils.splitChar + Utils.UintToString(uou.ammo2); //int - alternate ammo type, e.g. Penetrator or Teflon
		if (uou.useableItemIndex == 35) { // Worker Helmet with its two flaps.
			line += Utils.splitChar + Utils.SaveTransform(go.transform.GetChild(0));
			line += Utils.splitChar + Utils.SaveTransform(go.transform.GetChild(1));
		}
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		UseableObjectUse uou = go.GetComponent<UseableObjectUse>();
		if (uou == null) {
			Debug.Log("UseableObjectUse.Load failure, uou == null");
			return index + 4;
		}

		if (index < 0) {
			Debug.Log("UseableObjectUse.Load failure, index < 0");
			return index + 4;
		}

		if (entries == null) {
			Debug.Log("UseableObjectUse.Load failure, entries == null");
			return index + 4;
		}

		uou.useableItemIndex = Utils.GetIntFromString(entries[index]); index++;
		uou.customIndex = Utils.GetIntFromString(entries[index]); index++;
		uou.ammo = Utils.GetIntFromString(entries[index]); index++;
		uou.ammo2 = Utils.GetIntFromString(entries[index]); index++;
		if (uou.useableItemIndex == 35) { // Worker Helmet with its two flaps.
			Transform tr_child1 = go.transform.GetChild(0);
			Transform tr_child2 = go.transform.GetChild(1);
			index = Utils.LoadTransform(tr_child1,ref entries,index);
			index = Utils.LoadTransform(tr_child2,ref entries,index);
		}
		return index;
	}
}