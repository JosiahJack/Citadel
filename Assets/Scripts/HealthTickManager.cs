using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthTickManager : MonoBehaviour {
	public GameObject playerObject;
	public PlayerHealth playerHealth;
	public GameObject[] healthTicks;
	public Sprite[] tickImages;
	private Image tickImage;
	private int tempSpriteIndex;
	private float lasthealth;

	void Awake () {
		tickImage = GetComponent<Image>();
	}

	// void  Update (){
		// if (lasthealth != playerHealth.hm.health) DrawTicks();
		// lasthealth = playerHealth.hm.health;  // reason why this script can't be combined with energy ticks script
	// }
		
	public void DrawTicks() {
		tempSpriteIndex = -1;
		int step = 0;
		float h = playerHealth.hm.health;
		if (h > 255f) h = 255f; // always display ticks properly no matter what crazy health we've been hacked to have
		while (step < 256) {
			if (h < (256 - step)) {
				tempSpriteIndex++;
			}
			step += 11;
		}
		//Debug.Log(tempSpriteIndex.ToString());
		if (tempSpriteIndex >= 0 && tempSpriteIndex < 24) {
			tickImage.overrideSprite = tickImages[tempSpriteIndex];
		}
		/*
		int h = 0;
		int tickcnt = 0;

		// Clear health ticks
		for (int i=0;i<24;i++) {
			healthTicks[i].SetActive(false);
		}

		// Keep drawing ticks out until playerHealth is met
		while (h < playerHealth.hm.health) {
			healthTicks[tickcnt].SetActive(true);
			tickcnt++;
			h = h + 11;		
		}
		*/
	}
}