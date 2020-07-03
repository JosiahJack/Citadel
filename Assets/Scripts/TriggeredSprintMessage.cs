using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredSprintMessage : MonoBehaviour {
	public int messageLingdex = -1;
	public string messageToDisplay;
	void Start () {
		if (string.IsNullOrWhiteSpace(messageToDisplay)) {
			if (messageLingdex >= 0) {
				messageToDisplay = Const.a.stringTable[messageLingdex];
			} else {
				Debug.Log("BUG: Attempting to set TriggeredSprintMessage with a -1 index and a nullorwhitespace overrideString");
			}
		}
	}
}
