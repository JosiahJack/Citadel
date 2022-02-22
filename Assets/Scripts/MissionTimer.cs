using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionTimer : MonoBehaviour {
	public Text text;
	public Text timerTypeText;
	public string currentMission;
	public int currentMissionIndex;
	public PlayerHealth playerHealth;
	public MouseLookScript mls;
	public GameObject radTrigger;

	[HideInInspector] public bool lastTimer = false;
	private float t;
	private float minutes;
	private float seconds;
	private float timerFinished;

	public static MissionTimer a;

	void Awake() {
		a = this;
		a.t = 6000f;
		a.timerFinished = PauseScript.a.relativeTime + 1f;
		a.currentMission = Const.a.stringTable[504];
		a.currentMissionIndex = 0;
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
		if (Const.a.questData.LaserDestroyed && currentMissionIndex == 0) {
			UpdateToNextMission(10800f,505,1);
		}

		if (Const.a.questData.AntennaNorthDestroyed && Const.a.questData.AntennaSouthDestroyed && Const.a.questData.AntennaEastDestroyed && Const.a.questData.AntennaWestDestroyed && currentMissionIndex == 1) {
			UpdateToNextMission(2700f,506,2);
		}

		if (Const.a.questData.SelfDestructActivated && currentMissionIndex == 2) {
			UpdateToNextMission(3000f,507,3);
		}

		if (Const.a.questData.BridgeSeparated && currentMissionIndex == 3) {
			UpdateToNextMission(2700f,506,4);
		}

		if (!PauseScript.a.Paused() && !PauseScript.a.MenuActive()) {
			if (t <= 0) {
				if (lastTimer) {
					text.text = "<shocked>";
					timerTypeText.text = Const.a.stringTable[509];
					radTrigger.SetActive(true);
					gameObject.SetActive(false);
					return;
				} else {
					playerHealth.PlayerDeathToMenu(mls);
					return;
				}
			}

			if (mls.inCyberSpace) return; // timer doesn't count down in cyberspace, yay!

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
}
