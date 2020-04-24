using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIPointerMask : MonoBehaviour {
	void Awake () {
		EventTrigger pointerTrigger = GetComponent<EventTrigger>();
		if (pointerTrigger == null) {
			pointerTrigger = gameObject.AddComponent<EventTrigger>();
			if (pointerTrigger == null) {
				Const.sprint("BUG: Could not create Event Trigger for UIPointerMask",Const.a.allPlayers);
				return;
			}
		}

		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerEnter;
		entry.callback.AddListener((eventData) => { PtrEnter(); } );
		pointerTrigger.triggers.Add(entry);

		EventTrigger.Entry entry2 = new EventTrigger.Entry();
		entry2.eventID = EventTriggerType.PointerExit;
		entry2.callback.AddListener((eventData) => { PtrExit(); } );
		pointerTrigger.triggers.Add(entry2);
	}


	public void PtrEnter () {
		GUIState.a.isBlocking = true;
	}
	
	public void PtrExit () {
		GUIState.a.isBlocking = false;
	}
}
