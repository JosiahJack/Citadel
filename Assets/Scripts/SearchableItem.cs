using UnityEngine;
using System.Collections;

public class SearchableItem : MonoBehaviour {
	[Tooltip("Use lookUp table instead of randomItem#'s for default random item generation, for example for NPCs")]
	public int lookUpIndex = 0; // For randomly generating items
	[Tooltip("The indices referring to the prefab in Const table to have inside this searchable")]
	public int[] contents = {-1,-1,-1,-1};
	[Tooltip("Custom item indices of contents, for referring to specific attributes of content such as log type")]
	public int[] customIndex = {-1,-1,-1,-1};
	[Tooltip("Whether to randomly generate search items based on randomItem# indices")]
	public bool  generateContents = false;
	[Tooltip("Pick index from Const list of potential random item.")]
	public int[] randomItem; // possible item this container could contain if generateContents is true
	[Tooltip("Pick index from Const list of potential random item.")]
	public int[] randomItemCustomIndex; // possible item this container could contain if generateContents is true
	public int[] randomItemDropChance;
	[Tooltip("Name of the searchable item.")]
	public string objectName;
	[Tooltip("Number of slots.")]
	public int numSlots = 4;
	[HideInInspector]
	public bool searchableInUse;
	[HideInInspector]
	public GameObject currentPlayerCapsule;
	private float disconnectDist;
	private bool generationDone = false;
	private float tempFloat = 0f;

	void Start () {
		disconnectDist = Const.a.frobDistance;
		if (generateContents && !generationDone) {
			// Generate random contents once
			tempFloat = 0f;
			for(int i=0;i<randomItem.Length;i++) {
				tempFloat = Random.Range(0f,1f); // generate even distribution random value from 0f to 1f, e.g. 0.35
				// if 30% chance of dropping, then if tempFloat is 0.3 or greater there will be a 
				if (tempFloat <= randomItemDropChance[i]) {
					contents[i] = randomItem[i]; // ok item is now present
				}
			}
			generationDone = true;
		}
	}

	void Update () {
		if (searchableInUse) {
			if (Vector3.Distance(currentPlayerCapsule.transform.position, gameObject.transform.position) > disconnectDist) {
				searchableInUse = false;
				//MFDManager.a.ClearDataTab();
			}
		}
	}
}