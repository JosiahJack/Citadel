using UnityEngine;
using System.Collections;

public class SearchableItem : MonoBehaviour {
	public int lookUpIndex = 0; // For randomly generating items
	public int[] contents = {-1,-1,-1,-1};
	public int[] customIndex = {-1,-1,-1,-1};
	public bool  generateContents = false;
	public string objectName;
	public int numSlots = 4;
}