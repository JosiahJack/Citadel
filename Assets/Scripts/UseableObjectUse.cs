using UnityEngine;
using System.Collections;

public class UseableObjectUse : MonoBehaviour {
	public int useableItemIndex;
	public int customIndex = -1;
	public int ammo = 0;
	public int ammo2 = 0;
	private Texture2D tex;
	private MouseLookScript mlook;

	void Awake() {
		if (Const.a.difficultyPuzzle == 3) {
			if (useableItemIndex == 54) {
				if (UnityEngine.Random.Range(0,1f) < 0.33f) gameObject.SetActive(false); // 33% chance of not spawning logic probes on hard
			}
		}

		if (Const.a.difficultyMission <= 1) {
			if (useableItemIndex >= 81 && useableItemIndex <= 91) {
				gameObject.SetActive(false); // Remove access cards on difficulty 1 or 0 for Mission
			}
		}

		if (Const.a.difficultyMission == 0) {
			if (useableItemIndex == 6) {
				gameObject.SetActive(false); // Remove audiologs on 0 for Mission
			}
		}
	}

	// was GameObject owner as arguments, now UseData to hold more info
	public void Use (UseData ud) {
		if (useableItemIndex < 0) Debug.Log("BUG: Oh crap, a useable has an index less than 0! UseableObjectUse.cs");
		tex = Const.a.useableItemsFrobIcons[useableItemIndex];
		if (tex != null) ud.owner.GetComponent<PlayerReferenceManager>().playerCursor.GetComponent<MouseCursor>().cursorImage = tex;  // set cursor to this object
		mlook = ud.owner.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<MouseLookScript>();
		mlook.holdingObject = true;
		mlook.heldObjectIndex = useableItemIndex;
		mlook.heldObjectCustomIndex = customIndex;
		mlook.heldObjectAmmo = ammo;
		mlook.heldObjectAmmo2 = ammo2;
		if (Const.a.InputQuickItemPickup) {
			mlook.AddItemToInventory(useableItemIndex);
			mlook.ResetHeldItem();
			mlook.ResetCursor();
		} else {
			mlook.ForceInventoryMode();  // inventory mode is turned on when picking something up
			Const.sprint(Const.a.useableItemsNameText[useableItemIndex] + Const.a.stringTable[319],ud.owner); // <item_name> picked up.
		}
		this.gameObject.SetActive(false); //we've been picked up, quick hide like you are actually in the player's hand
	}

	public void HitForce (DamageData dd) {
		Rigidbody rbody = GetComponent<Rigidbody>();
		if (rbody != null) rbody.AddForceAtPosition((dd.attacknormal*dd.damage),dd.hit.point); // knock me around will you
	}
}