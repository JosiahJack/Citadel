using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TickIndicatorAnimation : MonoBehaviour {
	public PlayerHealth playerHealth;
	public PlayerEnergy playerEnergy;
	public Sprite[] indicatorImages;
	public bool healthIndicator; // if true, for health, if false, for energy
	public float thinkTime = 0.5f;
	private float nextthink;
	private Image indicator;
	private int tick;

	void Awake () {
		tick = 0;
		indicator = GetComponent<Image>();
	}

	void Update () {
		if (nextthink < Time.time) {
			if (healthIndicator) {
				if (playerHealth.hm.health > 176) {
					indicator.overrideSprite = indicatorImages[0];
					//indicator.sprite = indicatorImages[0];
				} else {
					if (playerHealth.hm.health > 88) {
						indicator.overrideSprite = indicatorImages[1];
					} else {
						switch (tick) {
						case 1: 
							indicator.overrideSprite = indicatorImages[2];
							break;
						case 2: 
							indicator.overrideSprite = indicatorImages[3];
							break;
						case 3: 
							indicator.overrideSprite = indicatorImages[4];
							break;
						case 4: 
							indicator.overrideSprite = indicatorImages[5];
							break;
						case 5: 
							indicator.overrideSprite = indicatorImages[6];
							break;
						}
					}
				}
			} else {
				if (playerEnergy.energy > 176) {
					indicator.overrideSprite = indicatorImages[0];
				} else {
					if (playerEnergy.energy > 88) {
						indicator.overrideSprite = indicatorImages[1];
					} else {
						switch (tick) {
						case 1: 
							indicator.overrideSprite = indicatorImages[2];
							break;
						case 2: 
							indicator.overrideSprite = indicatorImages[3];
							break;
						case 3: 
							indicator.overrideSprite = indicatorImages[4];
							break;
						case 4: 
							indicator.overrideSprite = indicatorImages[5];
							break;
						case 5: 
							indicator.overrideSprite = indicatorImages[2]; // 1 less frame than the health indicator, hold the dark one twice as long
							break;
						}
					}
				}

			}
			tick++;
			if (tick > 5)
				tick = 0;
			nextthink = Time.time + thinkTime;
		}
	}
}
