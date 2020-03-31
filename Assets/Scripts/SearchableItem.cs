using UnityEngine;
using System.Collections;

public class SearchableItem : MonoBehaviour, IBatchUpdate {
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
	private int tempInt = 100;
	public int maxRandomItems = 2;
	private int numRandomGeneratedItems;

	void Start () {
		disconnectDist = Const.a.frobDistance;
		numRandomGeneratedItems = 0;
		if (generateContents && !generationDone) {
			//Debug.Log("Generating Random Contents...");
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
			//Debug.Log("...done!  Generated indices " + contents[0].ToString() + " and " + contents[1].ToString() + ".");
			generationDone = true;
		}
		UpdateManager.Instance.RegisterSlicedUpdate(this, UpdateManager.UpdateMode.BucketB);
	}

	public void BatchUpdate () {
		if (searchableInUse) {
			if (Vector3.Distance(currentPlayerCapsule.transform.position, gameObject.transform.position) > disconnectDist) {
				searchableInUse = false;
				//MFDManager.a.ClearDataTab();
			}
		}
	}
}