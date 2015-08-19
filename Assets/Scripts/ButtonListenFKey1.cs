using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonListenFKey1 : MonoBehaviour {
	private Button button;
	public KeyCode Fkey;

	// Use this for initialization
	void Awake () {
		button = GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(Fkey))
			button.onClick.Invoke();
	}
}
