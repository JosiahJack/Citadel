using UnityEngine;
using System.Collections;
using System.Text;

public class UseableObjectUse : MonoBehaviour {
	public int useableItemIndex;
	public int customIndex = -1;
	public int ammo = 0;
	public int ammo2 = 0;
	public bool heldObjectLoadedAlternate = false;
	private static StringBuilder s1 = new StringBuilder();

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
	    if (MouseLookScript.a.holdingObject) {
	        MouseLookScript.a.DropHeldItem();
	        return;
	    }
	    
		if (useableItemIndex < 0) Debug.Log("BUG: Useable index less than 0!");
		MouseLookScript.a.holdingObject = true;
		MouseLookScript.a.heldObjectIndex = useableItemIndex;
		MouseLookScript.a.heldObjectCustomIndex = customIndex;
		MouseLookScript.a.heldObjectAmmo = ammo;
		MouseLookScript.a.heldObjectAmmo2 = ammo2;
		MouseLookScript.a.heldObjectLoadedAlternate = heldObjectLoadedAlternate;
		if (Const.a.InputQuickItemPickup) {
			MouseLookScript.a.AddItemToInventory(useableItemIndex,customIndex);
			MouseLookScript.a.ResetHeldItem();
		} else {
			MouseLookScript.a.ForceInventoryMode();  // Inventory mode is turned on when picking something up
			Const.sprint(Const.a.stringTable[useableItemIndex + 326] // <item>
						 + Const.a.stringTable[319]); // picked up.
		}
		
		Destroy(gameObject);
	}

	public void HitForce (DamageData dd) {
		Rigidbody rbody = GetComponent<Rigidbody>();
		if (rbody != null) {
			rbody.AddForceAtPosition((dd.attacknormal*(dd.damage + 80f)),
									 dd.hit.point); // knock me around will you
		}
	}

	public static string Save(GameObject go) {
		UseableObjectUse uou = go.GetComponent<UseableObjectUse>();
		if (uou == null) {
			Debug.Log("UseableObjectUse missing on saveable");
			return "-1|-1|0|0|BUG: Missing UseableObjectUse";
		}

		s1.Clear();
		s1.Append(Utils.UintToString(uou.useableItemIndex,"useableItemIndex")); // int - the main lookup index, needed for intanciating on load if doesn't match original SaveID
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(uou.customIndex,"customIndex")); // int - special reference like audiolog message
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(uou.ammo,"ammo")); // int - how much normal ammo is on the weapon
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(uou.ammo2,"ammo2")); //int - alternate ammo type, e.g. Penetrator or Teflon
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(uou.heldObjectLoadedAlternate,"heldObjectLoadedAlternate")); //int - alternate ammo type, e.g. Penetrator or Teflon
		if (uou.useableItemIndex == 35) { // Worker Helmet with its two flaps.
			s1.Append(Utils.splitChar);
			s1.Append(Utils.SaveTransform(go.transform.GetChild(0)));
			s1.Append(Utils.splitChar);
			s1.Append(Utils.SaveTransform(go.transform.GetChild(1)));
		}
		
		return s1.ToString();
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

		uou.useableItemIndex = Utils.GetIntFromString(entries[index],"useableItemIndex"); index++;
		uou.customIndex = Utils.GetIntFromString(entries[index],"customIndex"); index++;
		uou.ammo = Utils.GetIntFromString(entries[index],"ammo"); index++;
		uou.ammo2 = Utils.GetIntFromString(entries[index],"ammo2"); index++;
		uou.heldObjectLoadedAlternate = Utils.GetBoolFromString(entries[index],"heldObjectLoadedAlternate"); index++;
		if (uou.useableItemIndex == 35) { // Worker Helmet with its two flaps.
			Transform tr_child1 = go.transform.GetChild(0);
			Transform tr_child2 = go.transform.GetChild(1);
			index = Utils.LoadTransform(tr_child1,ref entries,index);
			index = Utils.LoadTransform(tr_child2,ref entries,index);
		}
		return index;
	}
}
