using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_ZeroG_MutantAnims : MonoBehaviour {
	public GameObject[] frames;
	public int startFrame = 0;
	private int currentFrame;
	private float tickFinished;
	public float tickTime = 0.04166f; // 24fps default
	public bool endOnLastFrame = false;
	public AIController aic;

	void Awake() {
		tickFinished = PauseScript.a.relativeTime;
		ResetFrames(); // turn off all frames just in case I left one on in the editor
		if (endOnLastFrame && startFrame == (frames.Length-1)) {
			if (frames[startFrame] == null) { Debug.Log("BUG: frames[startFrame] is null. startFrame is " + startFrame.ToString()); gameObject.SetActive(false); return;}
			frames[startFrame].SetActive(true);
		}
		currentFrame = startFrame;
		if (currentFrame > frames.Length) {
			currentFrame = frames.Length;
		}
		if (aic == null) Debug.Log("BUG: AIController aic was null on NPC_ZeroG_MutantAnims!");
	}

	void ResetFrames() {
		for (int i=0;i<frames.Length;i++) {
			if (frames[i] == null) {Debug.Log("BUG: frames["+i.ToString()+"] is null"); continue;}
			frames[i].SetActive(false);
		}
	}

    void Update() {
		if (!PauseScript.a.Paused() && !PauseScript.a.mainMenu.activeInHierarchy) {
			//if (aic.rangeToEnemy >= aic.sightRange) return;
			// only update model frames every tick seconds and only if we aren't on the last frame if we need to stop there
			if (endOnLastFrame && currentFrame == (frames.Length-1)) return;

			if (tickFinished < PauseScript.a.relativeTime) {
				if (frames.Length < 1) return;

				if (currentFrame > (frames.Length-1)) {
					currentFrame = 0; // wrap around
				}
				if (currentFrame < 0) currentFrame = 0;

				if (frames[currentFrame] != null) frames[currentFrame].SetActive(false); //disable this frame

				// Increment frame more than 1 if it's been more time elapsed than 2 frames worth
				if ((tickFinished - PauseScript.a.relativeTime) > (tickTime * 2f)) {
					currentFrame += (int) (Mathf.Ceil((tickFinished - PauseScript.a.relativeTime)/tickTime)); // increment by however many frames we skipped, e.g. slower than 24fps
				} else {
					currentFrame++; // only been one frame time, increment by 1
				}

				if (currentFrame > (frames.Length-1)) {
					currentFrame = 0; // wrap around
				}
				if (frames[currentFrame] != null) frames[currentFrame].SetActive(true); //enable next frame
				tickFinished = PauseScript.a.relativeTime + tickTime;
			}
		}
    }
}
