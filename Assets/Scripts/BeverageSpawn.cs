using UnityEngine;
using System.Collections;

public class BeverageSpawn : MonoBehaviour {
	public Texture skin1;
	public Texture skin2;
	public Texture skin3;
	public Texture skin4;

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
