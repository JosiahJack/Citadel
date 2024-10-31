using UnityEngine;
using System.Text;
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
		SearchableItem se;
		if (go.name.Contains("se_corpse_eaten")) se = go.transform.GetChild(0).GetComponent<SearchableItem>(); // se_corpse_eaten
		else se = go.GetComponent<SearchableItem>();
		
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		s1.Append(Utils.UintToString(se.lookUpIndex,"lookUpIndex"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.maxRandomItems,"maxRandomItems"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(se.generateContents,"generateContents"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(se.generationDone,"generationDone"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.contents[0],"contents[0]")); // int main lookup index
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.contents[1],"contents[1]")); // int main lookup index
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.contents[2],"contents[2]")); // int main lookup index
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.contents[3],"contents[3]")); // int main lookup index
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.customIndex[0],"customIndex[0]")); // int custom index
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.customIndex[1],"customIndex[1]")); // int custom index
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.customIndex[2],"customIndex[2]")); // int custom index
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.customIndex[3],"customIndex[3]")); // int custom index
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItem[0],"randomItem[0]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItem[1],"randomItem[1]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItem[2],"randomItem[2]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItem[3],"randomItem[3]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItem[4],"randomItem[4]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItem[5],"randomItem[5]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItem[6],"randomItem[6]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemCustomIndex[0],"randomItemCustomIndex[0]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemCustomIndex[1],"randomItemCustomIndex[1]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemCustomIndex[2],"randomItemCustomIndex[2]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemCustomIndex[3],"randomItemCustomIndex[3]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemCustomIndex[4],"randomItemCustomIndex[4]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemCustomIndex[5],"randomItemCustomIndex[5]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemCustomIndex[6],"randomItemCustomIndex[6]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemDropChance[0],"randomItemDropChance[0]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemDropChance[1],"randomItemDropChance[1]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemDropChance[2],"randomItemDropChance[2]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemDropChance[3],"randomItemDropChance[3]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemDropChance[4],"randomItemDropChance[4]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemDropChance[5],"randomItemDropChance[5]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(se.randomItemDropChance[6],"randomItemDropChance[6]"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.BoolToString(se.searchableInUse,"searchableInUse"));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index,
						   PrefabIdentifier prefID) {
		
		SearchableItem se;
		if (go.name.Contains("se_corpse_eaten")) se = go.transform.GetChild(0).GetComponent<SearchableItem>(); // se_corpse_eaten
		else se = go.GetComponent<SearchableItem>();
		
		if (se == null) Debug.LogError("SearchableItem missing on " + go.name);
		se.lookUpIndex = Utils.GetIntFromString(entries[index],"lookUpIndex"); index++;
		se.maxRandomItems = Utils.GetIntFromString(entries[index],"maxRandomItems"); index++;
		se.generateContents = Utils.GetBoolFromString(entries[index],"generateContents"); index++;
		se.generationDone = Utils.GetBoolFromString(entries[index],"generationDone"); index++;
		se.contents[0] = Utils.GetIntFromString(entries[index],"contents[0]"); index++; // int main lookup index
		se.contents[1] = Utils.GetIntFromString(entries[index],"contents[1]"); index++; // int main lookup index
		se.contents[2] = Utils.GetIntFromString(entries[index],"contents[2]"); index++; // int main lookup index
		se.contents[3] = Utils.GetIntFromString(entries[index],"contents[3]"); index++; // int main lookup index
		se.customIndex[0] = Utils.GetIntFromString(entries[index],"customIndex[0]"); index++; // int custom index
		se.customIndex[1] = Utils.GetIntFromString(entries[index],"customIndex[1]"); index++; // int custom index
		se.customIndex[2] = Utils.GetIntFromString(entries[index],"customIndex[2]"); index++; // int custom index
		se.customIndex[3] = Utils.GetIntFromString(entries[index],"customIndex[3]"); index++; // int custom index
		se.randomItem[0] = Utils.GetIntFromString(entries[index],"randomItem[0]"); index++;
		se.randomItem[1] = Utils.GetIntFromString(entries[index],"randomItem[1]"); index++;
		se.randomItem[2] = Utils.GetIntFromString(entries[index],"randomItem[2]"); index++;
		se.randomItem[3] = Utils.GetIntFromString(entries[index],"randomItem[3]"); index++;
		se.randomItem[4] = Utils.GetIntFromString(entries[index],"randomItem[4]"); index++;
		se.randomItem[5] = Utils.GetIntFromString(entries[index],"randomItem[5]"); index++;
		se.randomItem[6] = Utils.GetIntFromString(entries[index],"randomItem[6]"); index++;
		se.randomItemCustomIndex[0] = Utils.GetIntFromString(entries[index],"randomItemCustomIndex[0]"); index++;
		se.randomItemCustomIndex[1] = Utils.GetIntFromString(entries[index],"randomItemCustomIndex[1]"); index++;
		se.randomItemCustomIndex[2] = Utils.GetIntFromString(entries[index],"randomItemCustomIndex[2]"); index++;
		se.randomItemCustomIndex[3] = Utils.GetIntFromString(entries[index],"randomItemCustomIndex[3]"); index++;
		se.randomItemCustomIndex[4] = Utils.GetIntFromString(entries[index],"randomItemCustomIndex[4]"); index++;
		se.randomItemCustomIndex[5] = Utils.GetIntFromString(entries[index],"randomItemCustomIndex[5]"); index++;
		se.randomItemCustomIndex[6] = Utils.GetIntFromString(entries[index],"randomItemCustomIndex[6]"); index++;
		se.randomItemDropChance[0] = Utils.GetIntFromString(entries[index],"randomItemDropChance[0]"); index++;
		se.randomItemDropChance[1] = Utils.GetIntFromString(entries[index],"randomItemDropChance[1]"); index++;
		se.randomItemDropChance[2] = Utils.GetIntFromString(entries[index],"randomItemDropChance[2]"); index++;
		se.randomItemDropChance[3] = Utils.GetIntFromString(entries[index],"randomItemDropChance[3]"); index++;
		se.randomItemDropChance[4] = Utils.GetIntFromString(entries[index],"randomItemDropChance[4]"); index++;
		se.randomItemDropChance[5] = Utils.GetIntFromString(entries[index],"randomItemDropChance[5]"); index++;
		se.randomItemDropChance[6] = Utils.GetIntFromString(entries[index],"randomItemDropChance[6]"); index++;
		se.searchableInUse = Utils.GetBoolFromString(entries[index],"searchableInUse"); index++; // bool - is this being searched by Player?
		if (se.searchableInUse) {
			int numberFoundContents = 0;
			for (int i=3;i>=0;i--) {
				// If something was found, add 1 to count.
				if (se.contents[i] > -1) numberFoundContents++;
			}

			if (MFDManager.a.tetheredSearchable != se) {
				if (MFDManager.a.tetheredSearchable != null) {
					MFDManager.a.tetheredSearchable.ResetSearchable(false);
					MFDManager.a.tetheredSearchable = null;
				}
			}
			MFDManager.a.tetheredSearchable = se;
			MFDManager.a.objectInUsePos = se.gameObject.transform.position;
			MFDManager.a.usingObject = true;
		}
		return index;
	}
}
