using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnergyTickManager : MonoBehaviour {
	public PlayerEnergy playerEnergy;
	public Sprite[] tickImages;
	private Image tickImage;
	private int tempSpriteIndex;
	private float lastEnergy;

	void Awake () {
		tickImage = GetComponent<Image>();
		DrawTicks();
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

		if (tempSpriteIndex >= 0 && tempSpriteIndex < 24) tickImage.overrideSprite = tickImages[tempSpriteIndex];
	}
}