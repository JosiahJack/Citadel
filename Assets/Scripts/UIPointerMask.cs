using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIPointerMask : MonoBehaviour {
	private BoxCollider boxCol;
	private RectTransform rect;

	void Awake () {
		// Create box collider for cursor entry detection.
		rect = GetComponent<RectTransform>();
		boxCol = gameObject.AddComponent<BoxCollider>();
		float width = rect.sizeDelta.x * rect.localScale.x;
		float height = rect.sizeDelta.y * rect.localScale.y;
		if (width < 0) width *= -1f;
		if (height < 0) height *= -1f; // Cannot have negative size on box colliders.
		boxCol.size = new Vector3(width,height,1f);
	}

	public void OnMouseEnter() {
		PtrEnter();
	}

	public void OnMouseExit() {
		PtrExit();
	}

	public void PtrEnter () {
		GUIState.a.isBlocking = true;
	}
	
	public void PtrExit () {
		GUIState.a.isBlocking = false;
	}
}
