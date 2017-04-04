using UnityEngine;
using System.Collections;

public class ItemTabManager : MonoBehaviour {
    public GameObject iconManager;
    public GameObject textManager;
    public int itemTabType;
	public int lastCurrent = -1;

	void Awake () {
		lastCurrent = -1;
	}
}
