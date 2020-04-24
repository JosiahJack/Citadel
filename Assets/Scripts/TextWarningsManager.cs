using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextWarningsManager : MonoBehaviour {
	public GameObject[] warningTextGObjects;
	private Text[] warningTexts;
	private bool[] initialized;
	private float[] finishedTime;
	public float[] warningLifeTimes;
	public int[] uniqueID;
	public float warningDefaultLifeTime = 2f;
	public enum warningTextColor {white,red,green,yellow};
	public Color colwhite;
	public Color colred;
	public Color colgreen;
	public Color colyellow;

	void Awake () {
		warningTexts = new Text[warningTextGObjects.Length];
		initialized = new bool[warningTextGObjects.Length];
		finishedTime = new float[warningTextGObjects.Length];
		uniqueID = new int[warningTextGObjects.Length];
		for (int i=0;i<warningTextGObjects.Length;i++) {
			warningTexts[i] = warningTextGObjects[i].GetComponent<Text>();
			if (warningTexts[i] != null) warningTexts[i].text = System.String.Empty;
			initialized[i] = false;
			finishedTime[i] = Time.time;
			uniqueID[i] = -1;
		}
	}

	public void SendWarning(string message, float lifetime, int forcedReference, warningTextColor col, int id) {
		int setIndex = (warningTextGObjects.Length - 1);

		for (int i=setIndex;i>=0;i--) {
			if (uniqueID[i] == id) { setIndex = i; break;}
			if (finishedTime[i] < Time.time) { setIndex = i; break;}
		}

		if (id == 322) forcedReference = 0;
		if (forcedReference >= 0) setIndex = forcedReference;
		warningTexts[setIndex].text = message;
		uniqueID[setIndex] = id;
		if (col == warningTextColor.green) warningTexts[setIndex].color = colgreen;
		if (col == warningTextColor.white) warningTexts[setIndex].color = colwhite;
		if (col == warningTextColor.red) warningTexts[setIndex].color = colred;
		if (col == warningTextColor.yellow) warningTexts[setIndex].color = colyellow;

		if (lifetime > -1)
			if (warningLifeTimes[setIndex] < lifetime) warningLifeTimes[setIndex] = lifetime;
		else
			if (warningLifeTimes[setIndex] < warningDefaultLifeTime) warningLifeTimes[setIndex] = warningDefaultLifeTime;

		finishedTime[setIndex] = Time.time + warningLifeTimes[setIndex];
	}

	void Update () {
		for (int i=0;i<warningTextGObjects.Length;i++) {
			if (!string.IsNullOrWhiteSpace(warningTexts[i].text)) {
				if (finishedTime[i] < Time.time) {
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
