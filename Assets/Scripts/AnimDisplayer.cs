using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimDisplayer : MonoBehaviour {
	public string[] animName;
	private Animation anim;
	public int currentIndex = 0;
	public float timeStamp;

	void Start () {
		anim = GetComponent<Animation>();
		Debug.Log(anim.name);
		currentIndex = 0;
		timeStamp = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		anim[animName[currentIndex]].wrapMode = WrapMode.Loop;
		anim.Play(animName[currentIndex]);

		timeStamp = anim[animName[currentIndex]].normalizedTime;
		if (timeStamp > 0.9f) {
			currentIndex++;
			if (currentIndex >= animName.Length) {
				currentIndex = 0;
			}
		}
	}
}
