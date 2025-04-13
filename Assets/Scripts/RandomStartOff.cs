using UnityEngine;

public class RandomStartOff : MonoBehaviour {
	public float chanceOn = 0.25f;
	public GameObject[] objectsToToggle;
	
	void Awake() {
		for (int i=objectsToToggle.Length - 1;i>=0;i--) {
			if (objectsToToggle[i] == null) continue;

			if (Random.Range(0f,1f) < chanceOn) {
				objectsToToggle[i].SetActive(true);
			} else {
				objectsToToggle[i].SetActive(false);
			}
		}
	}
}
