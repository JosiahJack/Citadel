using UnityEngine;
using System.Collections;

public class BeverageSpawn : MonoBehaviour {
	[SerializeField] private Texture skin1;
	[SerializeField] private Texture skin2;
	[SerializeField] private Texture skin3;
	[SerializeField] private Texture skin4;

	// Use this for initialization
	void Awake () {
		float i;
		i = Random.Range(0.0f,1.0f);
		if (i > 0.75) {
			GetComponent<Renderer>().material.mainTexture = skin1;
		} else {
			if (i < 0.75 && i > 0.5) {
				GetComponent<Renderer>().material.mainTexture = skin1;
			} else {
				if (i < 0.5 && i > 0.25) {
					GetComponent<Renderer>().material.mainTexture = skin3;
				} else {
					GetComponent<Renderer>().material.mainTexture = skin4;
				}
			}
		}
	}
}
