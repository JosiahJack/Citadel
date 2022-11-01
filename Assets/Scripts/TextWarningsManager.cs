using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextWarningsManager : MonoBehaviour {
	public GameObject[] warningTextGObjects;

	private Text[] warningTexts;
	private bool[] initialized;
	private float[] finishedTime;
	[HideInInspector] public float[] warningLifeTimes;
	[HideInInspector] public int[] uniqueID;
	private float warningDefaultLifeTime = 2f;

	void Start () {
		warningTexts = new Text[warningTextGObjects.Length];
		initialized = new bool[warningTextGObjects.Length];
		finishedTime = new float[warningTextGObjects.Length];
		uniqueID = new int[warningTextGObjects.Length];
		for (int i=0;i<warningTextGObjects.Length;i++) {
			warningTexts[i] = warningTextGObjects[i].GetComponent<Text>();
			if (warningTexts[i] != null) warningTexts[i].text = System.String.Empty;
			initialized[i] = false;
			finishedTime[i] = PauseScript.a.relativeTime;
			uniqueID[i] = -1;
		}
	}

	public void SendWarning(string message, float lifetime, int forcedReference, HUDColor col, int id) {
		int setIndex = (warningTextGObjects.Length - 1);

		for (int i=setIndex;i>=0;i--) {
			if (uniqueID[i] == id) { setIndex = i; break;}
			if (finishedTime[i] < PauseScript.a.relativeTime) { setIndex = i; break;}
		}

		if (id == 322) forcedReference = 0;
		if (forcedReference >= 0) setIndex = forcedReference;
		warningTexts[setIndex].text = message;
		uniqueID[setIndex] = id;
		if (col == HUDColor.Green) warningTexts[setIndex].color = Const.a.ssGreenText;
		if (col == HUDColor.White) warningTexts[setIndex].color = Const.a.ssWhiteText;
		if (col == HUDColor.Red) warningTexts[setIndex].color = Const.a.ssRedText;
		if (col == HUDColor.Yellow) warningTexts[setIndex].color = Const.a.ssYellowText;

		if (lifetime > -1)
			if (warningLifeTimes[setIndex] < lifetime) warningLifeTimes[setIndex] = lifetime;
		else
			if (warningLifeTimes[setIndex] < warningDefaultLifeTime) warningLifeTimes[setIndex] = warningDefaultLifeTime;

		finishedTime[setIndex] = PauseScript.a.relativeTime + warningLifeTimes[setIndex];
	}

	void Update() {
		if (PauseScript.a.Paused() || PauseScript.a.MenuActive()) return;

		for (int i=0;i<warningTextGObjects.Length;i++) {
			if (!string.IsNullOrWhiteSpace(warningTexts[i].text)) {
				if (finishedTime[i] < PauseScript.a.relativeTime) {
					warningTexts[i].text = System.String.Empty;
					if (warningTextGObjects[i].activeInHierarchy) warningTextGObjects[i].SetActive(false);
					continue;
				}
				if (!warningTextGObjects[i].activeInHierarchy) warningTextGObjects[i].SetActive(true);
			} else {
				if (warningTextGObjects[i].activeInHierarchy) warningTextGObjects[i].SetActive(false);
			}
		}
	}
}
