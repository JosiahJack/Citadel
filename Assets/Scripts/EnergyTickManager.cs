using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnergyTickManager : MonoBehaviour {
	public GameObject playerObject;
	public PlayerEnergy playerEnergy;
	public GameObject[] energyTicks;
	public Sprite[] tickImages;
	private Image tickImage;
	private int tempSpriteIndex;
	private float lastEnergy;

	void Awake () {
		tickImage = GetComponent<Image>();
	}

	void  Update (){
		if (lastEnergy != playerEnergy.energy) DrawTicks();
		lastEnergy = playerEnergy.energy; // reason why this script can't be combined with health ticks script
	}

	void DrawTicks() {
		tempSpriteIndex = -1;
		int step = 0;

		while (step < 256) {
			if (playerEnergy.energy < (256 - step)) {
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
			energyTicks[i].SetActive(false);
		}

		// Keep drawing ticks out until playerHealth is met
		while (h <= playerEnergy.energy) {
			energyTicks[tickcnt].SetActive(true);
			tickcnt++;
			h = h + 11;		
		}*/
	}
}