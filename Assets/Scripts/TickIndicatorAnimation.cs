using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TickIndicatorAnimation : MonoBehaviour {
	public PlayerHealth playerHealth;
	public PlayerEnergy playerEnergy;
	public Sprite[] indicatorImages;
	public bool healthIndicator; // true = health, false = energy
	public float thinkTime = 0.5f;
	private float nextthink;
	private Image indicator;
	private int tick;

	void Start () {
		tick = 0;
		indicator = GetComponent<Image>();
	}

	void Update() {
		if (!gameObject.activeSelf) return;
		if (PauseScript.a.MenuActive()) return;

		if (nextthink < PauseScript.a.relativeTime) {
			if (healthIndicator) {
				if (playerHealth.hm.health > 176) {
					if (indicator.overrideSprite != indicatorImages[0]) indicator.overrideSprite = indicatorImages[0];
				} else {
					if (playerHealth.hm.health > 88) {
						if (indicator.overrideSprite != indicatorImages[1]) indicator.overrideSprite = indicatorImages[1];
					} else {
						switch (tick) {
							case 1: if (indicator.overrideSprite != indicatorImages[2]) indicator.overrideSprite = indicatorImages[2]; break;
							case 2: if (indicator.overrideSprite != indicatorImages[3]) indicator.overrideSprite = indicatorImages[3]; break;
							case 3: if (indicator.overrideSprite != indicatorImages[4]) indicator.overrideSprite = indicatorImages[4]; break;
							case 4: if (indicator.overrideSprite != indicatorImages[5]) indicator.overrideSprite = indicatorImages[5]; break;
							case 5: if (indicator.overrideSprite != indicatorImages[6]) indicator.overrideSprite = indicatorImages[6]; break;
						}
					}
				}
			} else {
				if (playerEnergy.energy > 176) {
					if (indicator.overrideSprite != indicatorImages[0]) indicator.overrideSprite = indicatorImages[0];
				} else {
					if (playerEnergy.energy > 88) {
						if (indicator.overrideSprite != indicatorImages[1]) indicator.overrideSprite = indicatorImages[1];
					} else {
						switch (tick) {
							case 1: if (indicator.overrideSprite != indicatorImages[2]) indicator.overrideSprite = indicatorImages[2]; break;
							case 2: if (indicator.overrideSprite != indicatorImages[3]) indicator.overrideSprite = indicatorImages[3]; break;
							case 3: if (indicator.overrideSprite != indicatorImages[4]) indicator.overrideSprite = indicatorImages[4]; break;
							case 4: if (indicator.overrideSprite != indicatorImages[5]) indicator.overrideSprite = indicatorImages[5]; break;
							case 5: if (indicator.overrideSprite != indicatorImages[2]) indicator.overrideSprite = indicatorImages[2]; break; // 1 less frame than the health indicator, hold the dark one twice as long		
						}
					}
				}
			}
			tick++;
			if (tick > 5) tick = 0;
			nextthink = PauseScript.a.relativeTime + thinkTime;
		}
	}
}
