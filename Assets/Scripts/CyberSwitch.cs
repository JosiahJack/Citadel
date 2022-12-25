using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberSwitch : MonoBehaviour {
	public int textIndex = 463;
	public bool active = false; // save
	public string target;
	public string argvalue;
	public GameObject activeCenter;
	public GameObject deactiveCenter;
	public GameObject iceNode;
	[HideInInspector] public bool iceActive;

	void Awake() {
		if (iceNode != null && iceNode.activeSelf) iceActive = true;
		Initialize(active,iceActive);
	}

	public void Initialize(bool startOn, bool turnIceOn) {
		if (active) {
			deactiveCenter.SetActive(false);
			activeCenter.SetActive(true);
		} else {
			deactiveCenter.SetActive(true);
			activeCenter.SetActive(false);
		}

		if (turnIceOn) {
			iceNode.SetActive(true);
		} else {
			iceNode.SetActive(false);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (active) return;

		if (other.gameObject.CompareTag("Player")) {
			MFDManager.a.CyberSprint(Const.a.stringTable[textIndex]);
			active = true;
			deactiveCenter.SetActive(false);
			activeCenter.SetActive(true);

			UseData ud = new UseData();
			ud.owner = other.gameObject;
			ud.argvalue = argvalue;
			TargetIO tio = GetComponent<TargetIO>();
			if (tio != null) {
				ud.SetBits(tio);
			} else {
				Debug.Log("BUG: no TargetIO.cs found on an object with a ButtonSwitch.cs script!  Trying to call UseTargets without parameters!");
			}
			Const.a.UseTargets(ud,target);
		}
	}

	public static string Save(GameObject go) {
		CyberSwitch cs = go.GetComponent<CyberSwitch>();
		if (cs == null) {
			Debug.Log("CyberSwitch missing on savetype of CyberSwitch!  GameObject.name: " + go.name);
			return "0|-1|BUG|BUG";
		}

		string line = System.String.Empty;
		line = Utils.BoolToString(cs.active);
		line += Utils.splitChar + Utils.UintToString(cs.textIndex);
		line += Utils.splitChar + cs.target;
		line += Utils.splitChar + cs.argvalue;
		line += Utils.splitChar + Utils.BoolToString(cs.iceActive);
		line += Utils.splitChar + Utils.BoolToString(cs.iceNode.activeSelf);
		if (cs.iceActive) {
			line += Utils.splitChar + HealthManager.Save(cs.iceNode,null);
		}
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		CyberSwitch cs = go.GetComponent<CyberSwitch>();
		if (cs == null || index < 0 || entries == null) return index + 4;

		cs.active = Utils.GetBoolFromString(entries[index]); index++;
		cs.textIndex = Utils.GetIntFromString(entries[index]); index++;
		cs.target = entries[index]; index++;
		cs.argvalue = entries[index]; index++;
		cs.iceActive = Utils.GetBoolFromString(entries[index]); index++;
		cs.iceNode.SetActive(Utils.GetBoolFromString(entries[index])); index++;
		if (cs.iceActive) {
			index = HealthManager.Load(cs.iceNode,ref entries,index,null);
		}
		cs.Initialize(cs.active,cs.iceActive);
		return index;
	}
}
