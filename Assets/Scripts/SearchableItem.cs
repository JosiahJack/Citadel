using UnityEngine;
using System.Collections;

public class SearchableItem : MonoBehaviour {
	public int lookUpIndex = 0; // For randomly generating items
	public GameObject[] contents;
	public bool  generateContents = false;
	public string objectName;
	public int numSlots = 4;
/*
0 = Small crate
1 = Medium crate
2 = Large crate
3 = Cabinet
4 = Thermos
*/
}