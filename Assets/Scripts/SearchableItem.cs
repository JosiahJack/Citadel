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
	[HideInInspector] public bool generationDone = false;
	public int maxRandomItems = 2;

	void Start () {
		int numRandomGeneratedItems = 0;
		if (generateContents && !generationDone) {
			// Generate random contents once
			int tempInt = 100;
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
			se = go.transform.GetChild(0).GetComponent<SearchableItem>();
			if (se == null) {
				Debug.Log("SearchableItem missing on savetype of SearchableItem!  GameObject.name: " + go.name);
				return Utils.DTypeWordToSaveString("uubbuuuuuuuuuuuuuuuuuuuuuuuuuuuuub");
			}
		}

		string line = System.String.Empty;
		line = Utils.UintToString(se.lookUpIndex);
		line += Utils.splitChar + Utils.UintToString(se.maxRandomItems);
		line += Utils.splitChar + Utils.BoolToString(se.generateContents);
		line += Utils.splitChar + Utils.BoolToString(se.generationDone);
		line += Utils.splitChar + Utils.UintToString(se.contents[0]); // int main lookup index
		line += Utils.splitChar + Utils.UintToString(se.contents[1]); // int main lookup index
		line += Utils.splitChar + Utils.UintToString(se.contents[2]); // int main lookup index
		line += Utils.splitChar + Utils.UintToString(se.contents[3]); // int main lookup index
		line += Utils.splitChar + Utils.UintToString(se.customIndex[0]); // int custom index
		line += Utils.splitChar + Utils.UintToString(se.customIndex[1]); // int custom index
		line += Utils.splitChar + Utils.UintToString(se.customIndex[2]); // int custom index
		line += Utils.splitChar + Utils.UintToString(se.customIndex[3]); // int custom index
		line += Utils.splitChar + Utils.UintToString(se.randomItem[0]);
		line += Utils.splitChar + Utils.UintToString(se.randomItem[1]);
		line += Utils.splitChar + Utils.UintToString(se.randomItem[2]);
		line += Utils.splitChar + Utils.UintToString(se.randomItem[3]);
		line += Utils.splitChar + Utils.UintToString(se.randomItem[4]);
		line += Utils.splitChar + Utils.UintToString(se.randomItem[5]);
		line += Utils.splitChar + Utils.UintToString(se.randomItem[6]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemCustomIndex[0]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemCustomIndex[1]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemCustomIndex[2]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemCustomIndex[3]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemCustomIndex[4]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemCustomIndex[5]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemCustomIndex[6]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemDropChance[0]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemDropChance[1]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemDropChance[2]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemDropChance[3]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemDropChance[4]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemDropChance[5]);
		line += Utils.splitChar + Utils.UintToString(se.randomItemDropChance[6]);
		line += Utils.splitChar + Utils.BoolToString(se.searchableInUse);
		return line;
	}

	public static int Load(GameObject go, ref string[] entries, int index,
						   PrefabIdentifier prefID) {
		SearchableItem se = go.GetComponent<SearchableItem>();
		if (prefID != null) {
			if (prefID.constIndex == 467) {
				if (se == null) se = go.transform.GetChild(0).gameObject.GetComponent<SearchableItem>();
			}
		}
		if (se == null || index < 0 || entries == null) return index + 36;

		se.lookUpIndex = Utils.GetIntFromString(entries[index]); index++;
		se.maxRandomItems = Utils.GetIntFromString(entries[index]); index++;
		se.generateContents = Utils.GetBoolFromString(entries[index]); index++;
		se.generationDone = Utils.GetBoolFromString(entries[index]); index++;
		se.contents[0] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
		se.contents[1] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
		se.contents[2] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
		se.contents[3] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
		se.customIndex[0] = Utils.GetIntFromString(entries[index]); index++; // int custom index
		se.customIndex[1] = Utils.GetIntFromString(entries[index]); index++; // int custom index
		se.customIndex[2] = Utils.GetIntFromString(entries[index]); index++; // int custom index
		se.customIndex[3] = Utils.GetIntFromString(entries[index]); index++; // int custom index
		se.randomItem[0] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItem[1] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItem[2] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItem[3] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItem[4] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItem[5] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItem[6] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemCustomIndex[0] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemCustomIndex[1] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemCustomIndex[2] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemCustomIndex[3] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemCustomIndex[4] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemCustomIndex[5] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemCustomIndex[6] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemDropChance[0] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemDropChance[1] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemDropChance[2] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemDropChance[3] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemDropChance[4] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemDropChance[5] = Utils.GetIntFromString(entries[index]); index++;
		se.randomItemDropChance[6] = Utils.GetIntFromString(entries[index]); index++;
		se.searchableInUse = Utils.GetBoolFromString(entries[index]); index++; // bool - is this being searched by Player?
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