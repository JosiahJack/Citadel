using UnityEngine;
using System.Collections;

public class SearchableItem : MonoBehaviour {
	[Tooltip(">0 = Use a lookUp table instead of randomItem#'s")] public int lookUpIndex = 0; // For randomly generating items
	[Tooltip("The indices referring to the prefab in Const table to have inside this searchable")] public int[] contents = {-1,-1,-1,-1};
	[Tooltip("Custom item indices of contents, for referring to specific attributes of content such as log type")] public int[] customIndex = {-1,-1,-1,-1};
	[Tooltip("Whether to randomly generate search items based on randomItem# indices")] public bool  generateContents = false;
	[Tooltip("Pick index from Const list of potential random item.")] public int[] randomItem; // possible item this container could contain if generateContents is true
	[Tooltip("Pick index from Const list of potential random item.")] public int[] randomItemCustomIndex; // possible item this container could contain if generateContents is true
	public int[] randomItemDropChance;
	[Tooltip("Name of the searchable item.")] public string objectName;
	[HideInInspector] public bool searchableInUse;
	[HideInInspector] public GameObject currentPlayerCapsule;
	private bool generationDone = false;
	private int tempInt = 100;
	public int maxRandomItems = 2;
	private int numRandomGeneratedItems;

	void Start () {
		numRandomGeneratedItems = 0;
		if (generateContents && !generationDone) {
			// Generate random contents once
			tempInt = 100;
			for(int i=0;i<randomItem.Length;i++) {
				tempInt = Random.Range(0,100); // generate even distribution random value from 0 to 100, e.g. 35
				if (randomItemDropChance[i] <= 0) continue; // next!
				if (tempInt <= randomItemDropChance[i]) {
					contents[numRandomGeneratedItems] = randomItem[i]; // ok item is now present
					numRandomGeneratedItems++;
					if (numRandomGeneratedItems>maxRandomItems) break; // all done we have all our contents
				}
			}
			generationDone = true;
		}
	}

	public void ResetSearchable(bool wipeContents) {
		searchableInUse = false;
		if (wipeContents) {
			contents[0] = -1;
			contents[1] = -1;
			contents[2] = -1;
			contents[3] = -1;
			customIndex[0] = -1;
			customIndex[1] = -1;
			customIndex[2] = -1;
			customIndex[3] = -1;
		}
	}

	// Save searchable data
	public static string Save(GameObject go, PrefabIdentifier prefID) {
		SearchableItem se = go.GetComponent<SearchableItem>();
		if (se == null) {
			Debug.Log("SearchableItem missing on savetype of SearchableItem!  GameObject.name: " + go.name);
			return "-1|-1|-1|-1|-1|-1|-1|-1|0";
		}

		string line = System.String.Empty;
		line = Utils.UintToString(se.contents[0]); // int main lookup index
		line += Utils.splitChar + Utils.UintToString(se.contents[1]); // int main lookup index
		line += Utils.splitChar + Utils.UintToString(se.contents[2]); // int main lookup index
		line += Utils.splitChar + Utils.UintToString(se.contents[3]); // int main lookup index
		line += Utils.splitChar + Utils.UintToString(se.customIndex[0]); // int custom index
		line += Utils.splitChar + Utils.UintToString(se.customIndex[1]); // int custom index
		line += Utils.splitChar + Utils.UintToString(se.customIndex[2]); // int custom index
		line += Utils.splitChar + Utils.UintToString(se.customIndex[3]); // int custom index
		line += Utils.splitChar + Utils.BoolToString(se.searchableInUse);
		if (prefID.constIndex == 464) { // Briefcase with its gibs.
			Transform child1 = go.transform.GetChild(0);
			GameObject child1GO = child1.gameObject;
			line += Utils.splitChar + Utils.SaveTransform(child1);
			line += Utils.splitChar + Utils.SaveRigidbody(child1GO);
			line += Utils.splitChar + Utils.BoolToString(child1GO.activeSelf);

			Transform child2 = go.transform.GetChild(1);
			GameObject child2GO = child2.gameObject;
			line += Utils.splitChar + Utils.SaveTransform(child2);
			line += Utils.splitChar + Utils.SaveRigidbody(child2GO);
			line += Utils.splitChar + Utils.BoolToString(child2GO.activeSelf);

			Transform child3 = go.transform.GetChild(2);
			GameObject child3GO = child3.gameObject;
			line += Utils.splitChar + Utils.SaveTransform(child3);
			line += Utils.splitChar + Utils.SaveRigidbody(child3GO);
			line += Utils.splitChar + Utils.BoolToString(child3GO.activeSelf);
		}
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index,
						   PrefabIdentifier prefID) {
		SearchableItem se = go.GetComponent<SearchableItem>();
		if (se == null || index < 0 || entries == null) return index + 9;

		se.contents[0] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
		se.contents[1] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
		se.contents[2] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
		se.contents[3] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
		se.customIndex[0] = Utils.GetIntFromString(entries[index]); index++; // int custom index
		se.customIndex[1] = Utils.GetIntFromString(entries[index]); index++; // int custom index
		se.customIndex[2] = Utils.GetIntFromString(entries[index]); index++; // int custom index
		se.customIndex[3] = Utils.GetIntFromString(entries[index]); index++; // int custom index
		se.searchableInUse = Utils.GetBoolFromString(entries[index]); index++; // bool - is this being searched by Player?

		if (prefID.constIndex == 464) { // Briefcase with its gibs.
			bool childActive = false;
			Transform child1 = go.transform.GetChild(0);
			GameObject child1GO =child1.gameObject;

			Transform child2 = go.transform.GetChild(1);
			GameObject child2GO = child2.gameObject;

			Transform child3 = go.transform.GetChild(2);
			GameObject child3GO = child3.gameObject;

			index = Utils.LoadTransform(child1,ref entries,index);
			index = Utils.LoadRigidbody(child1GO,ref entries,index);
			childActive = Utils.GetBoolFromString(entries[index]); index++; // bool - is the gib active?
			child1GO.SetActive(childActive);

			index = Utils.LoadTransform(child2,ref entries,index);
			index = Utils.LoadRigidbody(child2GO,ref entries,index);
			childActive = Utils.GetBoolFromString(entries[index]); index++; // bool - is the gib active?
			child2GO.SetActive(childActive);

			index = Utils.LoadTransform(child3,ref entries,index);
			index = Utils.LoadRigidbody(child3GO,ref entries,index);
			childActive = Utils.GetBoolFromString(entries[index]); index++; // bool - is the gib active?
			child3GO.SetActive(childActive);
		}

		if (se.searchableInUse) {
			int numberFoundContents = 0;
			for (int i=3;i>=0;i--) {
				// If something was found, add 1 to count.
				if (se.contents[i] > -1) numberFoundContents++;
			}

			if (MFDManager.a.tetheredSearchable != se) {
				if (MFDManager.a.tetheredSearchable != null) {
					MFDManager.a.tetheredSearchable.ResetSearchable(false);
				}
			}
			MFDManager.a.tetheredSearchable = se;
			MFDManager.a.objectInUsePos = se.gameObject.transform.position;
			MFDManager.a.usingObject = true;
		}
		return index;
	}
}