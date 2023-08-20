using System.Collections;
using System.Collections.Generic;
using System.Text;
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
			Const.a.UseTargets(gameObject,ud,target);
		}
	}

	public static string Save(GameObject go, PrefabIdentifier prefID) {
		CyberSwitch cs = go.GetComponent<CyberSwitch>();
		if (cs == null) {
			Debug.Log("CyberSwitch missing on savetype of CyberSwitch!  GameObject.name: " + go.name);
			return Utils.DTypeWordToSaveString("bussbbffbbbu");
		}

		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Utils.BoolToString(cs.active,"CyberSwitch.active"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(cs.textIndex,"textIndex"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveString(cs.target,"target"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveString(cs.argvalue,"argvalue"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(cs.iceActive,"iceActive"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(cs.iceNode.activeSelf,"iceNode.activeSelf"));
		s1.Append(Utils.splitChar);
		s1.Append(HealthManager.Save(cs.iceNode,prefID));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index,
						   PrefabIdentifier prefID) {
		CyberSwitch cs = go.GetComponent<CyberSwitch>();
		if (cs == null) {
			Debug.Log("CyberSwitch.Load failure, cs == null");
			return index + 13;
		}

		if (index < 0) {
			Debug.Log("CyberSwitch.Load failure, index < 0");
			return index + 13;
		}

		if (entries == null) {
			Debug.Log("CyberSwitch.Load failure, entries == null");
			return index + 13;
		}

		cs.active = Utils.GetBoolFromString(entries[index],"CyberSwitch.active"); index++;
		cs.textIndex = Utils.GetIntFromString(entries[index],"textIndex"); index++;
		cs.target = Utils.LoadString(entries[index],"target"); index++;
		cs.argvalue = Utils.LoadString(entries[index],"argvalue"); index++;
		cs.iceActive = Utils.GetBoolFromString(entries[index],"iceActive"); index++;
		cs.iceNode.SetActive(Utils.GetBoolFromString(entries[index],"iceNode.activeSelf")); index++;
		index = HealthManager.Load(cs.iceNode,ref entries,index,prefID);
		cs.Initialize(cs.active,cs.iceActive);
		return index;
	}
}
