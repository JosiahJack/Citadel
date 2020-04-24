using UnityEngine;
using System.Collections;

public class UseableObjectUse : MonoBehaviour {
	public int useableItemIndex;
	public int customIndex = -1;
	public int ammo = 0;
	public bool ammoIsSecondary = false;
	private Texture2D tex;
	private MouseLookScript mlook;

	// was GameObject owner as arguments, now UseData to hold more info
	public void Use (UseData ud) {
		tex = Const.a.useableItemsFrobIcons[useableItemIndex];
		if (tex != null) ud.owner.GetComponent<PlayerReferenceManager>().playerCursor.GetComponent<MouseCursor>().cursorImage = tex;  // set cursor to this object
		mlook = ud.owner.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<MouseLookScript>();
		mlook.ForceInventoryMode();  // inventory mode is turned on when picking something up
		mlook.holdingObject = true;
		mlook.heldObjectIndex = useableItemIndex;
		mlook.heldObjectCustomIndex = customIndex;
		mlook.heldObjectAmmo = ammo;
		mlook.heldObjectAmmoIsSecondary = ammoIsSecondary;

		if (Const.a.InputQuickItemPickup) {
			mlook.AddItemToInventory(useableItemIndex);
			mlook.ResetHeldItem();
			mlook.ResetCursor();
		}
		// should I assign mlook to null at this point so that I don't accidentally have a bug in multiplayer where the wrong player reference gets an object picked up
		// nah, I trust GetComponent to work for the owner who sent the Use command
		// well ok sure...
		mlook = null;
		this.gameObject.SetActive(false); //we've been picked up, quick hide like you are actually in the player's hand
	}

	public void HitForce (DamageData dd) {
		Rigidbody rbody = GetComponent<Rigidbody>();
		if (rbody != null) rbody.AddForceAtPosition((dd.attacknormal*dd.damage),dd.hit.point); // knock me around will you
	}
}