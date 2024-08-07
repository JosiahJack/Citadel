using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionTimer : MonoBehaviour {
	public Text text;
	public Text timerTypeText;
	public string currentMission;
	public int currentMissionIndex;

	[HideInInspector] public bool lastTimer = false;
	private float t;
	private float minutes;
	private float seconds;
	private float timerFinished;
	private bool timesUP = false;

	public static MissionTimer a;

	void Awake() {
		a = this;
		a.t = 6000f;
		a.timerFinished = PauseScript.a.relativeTime + 1f;
		a.currentMission = Const.a.stringTable[504];
		a.currentMissionIndex = 0;
		a.timesUP = false;
		if (Const.a.difficultyMission < 3) {
			text.text = System.String.Empty;
			timerTypeText.text = System.String.Empty;
			gameObject.SetActive(false);
		}
	}

	// 25200
	// 6000 for laser mission (level 1, 2, R)
	// 10800 for download completion (level 3, 6, G1, G2, G4, 4, 7)
	// 2700 for bridge in range for finishing download (level R, 5)
	// 3000 for bridge separation countdown (level 8)
	// 2700 for biotoxin release (level 9)

    public void UpdateToNextMission(float newTimerAmount,int misTextIndex, int nextMissionIndex) {
		QuestLogNotesManager.a.UpdateToNextMission(nextMissionIndex);

		if (Const.a.difficultyMission < 3) return; // Don't update timer on lower skill settings.
		t = newTimerAmount;
		currentMissionIndex = nextMissionIndex;
		currentMission = Const.a.stringTable[misTextIndex];
		if (currentMissionIndex == 4) lastTimer = true; // No gameover for last timer.
    }

    void Update() {
		if (Const.a.difficultyMission < 3) return;
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;
		if (MouseLookScript.a.inCyberSpace) return; // timer doesn't count down in cyberspace, yay!

		if (timesUP) {
			if (PlayerHealth.a.hm.health > 0f) {
				PlayerHealth.a.radiationArea = true;
				PlayerHealth.a.GiveRadiation(0.1f); // Every frame! Muahaha!!!
				return;
			}
		}

		if (t <= 0) {
			if (lastTimer) {
				text.text = Const.a.stringTable[869];
				timerTypeText.text = Const.a.stringTable[509];
				timesUP = true;
				return;
			} else {
				PlayerHealth.a.PlayerDeathToMenu();
				return;
			}
		}

		switch (currentMissionIndex) {
			case 0: if (Const.a.questData.LaserDestroyed) UpdateToNextMission(10800f,505,1); break;
			case 1:
				if (Const.a.questData.AntennaNorthDestroyed
					&& Const.a.questData.AntennaSouthDestroyed
					&& Const.a.questData.AntennaEastDestroyed
					&& Const.a.questData.AntennaWestDestroyed) {
					UpdateToNextMission(2700f,506,2);
				}
				break;
			case 2: if (Const.a.questData.SelfDestructActivated) UpdateToNextMission(3000f,507,3); break;
			case 3: if (Const.a.questData.BridgeSeparated) UpdateToNextMission(2700f,506,4); break;
		}

		if (timerFinished < PauseScript.a.relativeTime) {
			t -= 1f;
			minutes = Mathf.Floor(t/60f);
			seconds = t - (minutes*60);
			text.text = (minutes.ToString("00") + ":" + seconds.ToString("00"));
			timerTypeText.text = currentMission;
			timerFinished = PauseScript.a.relativeTime + 1f;
		}
    }
}
