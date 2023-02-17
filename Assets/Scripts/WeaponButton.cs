using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Handles weapon inventory buttons so when player clicks on the weapon name
// text it selects that weapon, also changing to it to have as current.
public class WeaponButton : MonoBehaviour {
    public int useableItemIndex;
	public int WepButtonIndex;

	public void WeaponInvClick () {
		Debug.Log("WeaponInvClick");
		MFDManager.a.mouseClickHeldOverGUI = true;
		WeaponCurrent.a.WeaponChange(useableItemIndex, WepButtonIndex);
	}

	void Start() {
		GetComponent<Button>().onClick.AddListener(() => { WeaponInvClick(); });
	}
}
