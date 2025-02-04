using UnityEngine;
using System.Collections;
using System.Text;

public class TeleportTouch : MonoBehaviour {
    public int teleportID; // Unique ID for this instance
    public int targetDestinationID; // Destination teleport ID (linked via index)
	public float justUsed = 0f; // save
	public bool touchEnabled = true;
	
    private static TeleportTouch[] allTeleportTouches = new TeleportTouch[8];

	public void Awake() {
		if (teleportID > allTeleportTouches.Length || teleportID < 0) { Destroy(this.gameObject); return; }
		
        allTeleportTouches[teleportID] = this;
    }

	void  OnTriggerEnter ( Collider col  ) {
		if (!touchEnabled) return;
		if (col == null) return;
		if (col.gameObject == null) return;

		if (col.gameObject.CompareTag("Player")) {
			HealthManager hm = Utils.GetMainHealthManager(col.gameObject);
			if (hm != null) {
				if (hm.health > 0f && justUsed < PauseScript.a.relativeTime) {
					MFDManager.a.teleportFX.SetActive(true);
					TeleportTouch tt = allTeleportTouches[targetDestinationID];
					if (tt != null) {
						col.transform.position = tt.transform.position; // Do it!
						tt.justUsed = PauseScript.a.relativeTime + 1.0f;
					}
					
					Utils.PlayUIOneShotSavable(106);
				}
			}
		}
	}

	public static string Save(GameObject go) {
		TeleportTouch tt = go.GetComponent<TeleportTouch>();
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Utils.SaveRelativeTimeDifferential(tt.justUsed,"justUsed"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(tt.touchEnabled,"touchEnabled"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(tt.teleportID,"teleportID"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(tt.targetDestinationID,"targetDestinationID"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		TeleportTouch tt = go.GetComponent<TeleportTouch>();
		tt.justUsed = Utils.LoadRelativeTimeDifferential(entries[index],"justUsed"); index++;
		tt.touchEnabled = Utils.GetBoolFromString(entries[index],"touchEnabled"); index++;
		tt.teleportID = Utils.GetIntFromString(entries[index],"teleportID"); index++;
		tt.targetDestinationID = Utils.GetIntFromString(entries[index],"targetDestinationID"); index++;
		tt.Awake();
		return index;
	}
}
