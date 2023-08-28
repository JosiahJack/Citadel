using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {
	public AudioClip[] levelMusic1;
	public AudioClip[] levelMusic2;
	public AudioClip[] levelMusicReactor;
	public AudioClip[] levelMusic6;
	public AudioClip[] levelMusicGroves;
	public AudioClip[] levelMusic8;
	public AudioClip[] levelMusicRevive;
	public AudioClip[] levelMusicDeath;
	public AudioClip[] levelMusicCyber;
	public AudioClip[] levelMusicElevator;
	public AudioClip[] levelMusicDistortion;
	public AudioSource SFXMain;
	public AudioSource SFXOverlay;
	private float clipFinished;
	private float clipLength;
	private float clipOverlayLength;
	private float clipOverlayFinished;
	public GameObject mainMenu;

	private AudioClip tempC;
	private AudioClip curC;
	private AudioClip curOverlayC;
	public static Music a;
	private bool paused;
	private bool cyberTube;
	[HideInInspector] public bool levelEntry;
	private int rand;
	private bool inZone;
	private bool distortion;
	private bool elevator;
	[HideInInspector] public bool inCombat;
	private float combatImpulseFinished;

	void Start() {
		a = this;
		a.clipFinished = Time.time;
		a.clipOverlayFinished = Time.time;
		a.tempC = null;
		a.cyberTube = false;
		a.levelEntry = true;
		a.inZone = false;
		a.rand = 0;
		a.combatImpulseFinished = PauseScript.a.relativeTime + 5f;
	    a.LoadMusic();
	}
	
	private void LoadMusic() {
		// Load all the audio 1clips at the start to prevent stutter.
		int i = 0;
        for (i = 0; i < levelMusic1.Length; i++) {
            if (levelMusic1[i] != null) levelMusic1[i].LoadAudioData();
        }
        
        for (i = 0; i < levelMusic2.Length; i++) {
            if (levelMusic2[i] != null) levelMusic2[i].LoadAudioData();
        }
        
        for (i = 0; i < levelMusicReactor.Length; i++) {
            if (levelMusicReactor[i] != null) {
                levelMusicReactor[i].LoadAudioData();
            }
        }
        
        for (i = 0; i < levelMusic6.Length; i++) {
            if (levelMusic6[i] != null) levelMusic6[i].LoadAudioData();
        }
        
        for (i = 0; i < levelMusicGroves.Length; i++) {
            if (levelMusicGroves[i] != null) {
                levelMusicGroves[i].LoadAudioData();
            }
        }
        
        for (i = 0; i < levelMusic8.Length; i++) {
            if (levelMusic8[i] != null) levelMusic8[i].LoadAudioData();
        }
        
        for (i = 0; i < levelMusicRevive.Length; i++) {
            if (levelMusicRevive[i] != null) {
                levelMusicRevive[i].LoadAudioData();
            }
        }
        
        for (i = 0; i < levelMusicDeath.Length; i++) {
            if (levelMusicDeath[i] != null) levelMusicDeath[i].LoadAudioData();
        }
        
        for (i = 0; i < levelMusicCyber.Length; i++) {
            if (levelMusicCyber[i] != null) levelMusicCyber[i].LoadAudioData();
        }
        
        for (i = 0; i < levelMusicElevator.Length; i++) {
            if (levelMusicElevator[i] != null) {
                levelMusicElevator[i].LoadAudioData();
            }
        }
        
        for (i = 0; i < levelMusicDistortion.Length; i++) {
            if (levelMusicDistortion[i] != null) {
                levelMusicDistortion[i].LoadAudioData();
            }
        }
	}

	public void PlayTrack(int levnum, TrackType ttype, MusicType mtype) {
		tempC = GetCorrespondingLevelClip(levnum,ttype);
		if (!elevator) levelEntry = false; // already used by GetCorresponding... just now
		if (mtype == MusicType.Walking || mtype == MusicType.Combat) {
			if (tempC == curC && SFXMain.isPlaying) return; // no need, already playing
			if (SFXMain != null) SFXMain.Stop(); // stop playing normal
			if (tempC != null) {
				SFXMain.clip = tempC;
				curC = tempC;
				Utils.PlayOneShotSavable(SFXMain,tempC);
				SFXMain.loop = false;
			} else {
				curC = null;
			}
		}

		if (mtype == MusicType.Overlay) {
			if (tempC == curOverlayC && SFXOverlay.isPlaying) return; // no need, already playing
			if (SFXOverlay != null) SFXOverlay.Stop(); // stop any overlays
			if (tempC != null) {
				SFXOverlay.clip = tempC;
				curOverlayC = tempC;
				Utils.PlayOneShotSavable(SFXOverlay,tempC);
				SFXOverlay.loop = false;
			} else {
				curOverlayC = null;
			}
		}

		if (mtype == MusicType.Override) {
			if (tempC == curC && SFXMain.isPlaying) return; // no need, already playing

			// stop both
			if (SFXMain != null) SFXMain.Stop();
			if (SFXOverlay != null) SFXOverlay.Stop();
			if (tempC != null) {
				SFXMain.clip = tempC;
				curC = tempC;
				SFXMain.Play();
				SFXMain.loop = false;
				//if (ttype == TrackType.Elevator) SFXMain.loop = true;
			} else {
				curC = null;
			}
		}
	}

	AudioClip GetCorrespondingLevelClip(int levnum, TrackType ttype) {
		if (levnum < 0 || levnum > 13) return null; // out of bounds

		// 13 CYBERSPACE
		if (levnum == 13) {
			if (levelEntry) return levelMusicCyber[0];
			if (cyberTube) {
				rand = UnityEngine.Random.Range(4,8);
				return levelMusicCyber[rand];
			}
			if (UnityEngine.Random.Range(0,1f) < 0.5f) {
				rand = UnityEngine.Random.Range(1,5);
				return levelMusicCyber[rand];
			} else {
				return levelMusicCyber[8];
			}
		}

		// These act as override types, return from these first before special level handling
		switch(ttype) {
			//case TrackType.MutantNear: return levelMusicMutantNear[levnum];
			//case TrackType.CyborgNear: return levelMusicCyborgNear[levnum];
			//case TrackType.CyborgDroneNear: return levelMusicCyborgDroneNear[levnum];
			//case TrackType.RobotNear: return levelMusicRobotNear[levnum];
			//case TrackType.Transition: return levelMusicTransition[levnum];
			case TrackType.Revive: return levelMusicRevive[levnum];
			case TrackType.Death: return levelMusicDeath[levnum];
			//case TrackType.Cybertube: return levelMusicCybertube[levnum];
			case TrackType.Elevator: return levelMusicElevator[levnum];
			case TrackType.Distortion: return levelMusicDistortion[levnum];
		}

		// 1  MEDICAL
		if (levnum == 1) {
			if (levelEntry) return levelMusic1[0];
			if (ttype == TrackType.Combat) {
				rand = UnityEngine.Random.Range(5,11);
				return levelMusic1[rand];
			}
			rand = UnityEngine.Random.Range(1,5);
			return levelMusic1[rand];
		}

		// 2  SCIENCE
		if (levnum == 2) {
			if (levelEntry) return levelMusic2[0];
			if (ttype == TrackType.Combat) {
				rand = UnityEngine.Random.Range(8,10);
				return levelMusic2[rand];
			}
			rand = UnityEngine.Random.Range(1,8);
			return levelMusic2[rand];
		}

		// 0  REACTOR, 5 FLIGHT, 7 ENGINEERING
		if (levnum == 0 || levnum == 5 || levnum == 7) {
			if (levelEntry) return levelMusicReactor[6];
			if (ttype == TrackType.Combat) {
				rand = UnityEngine.Random.Range(0,6);
				return levelMusicReactor[rand];
			}
			rand = UnityEngine.Random.Range(6,13);
			return levelMusicReactor[rand];
		}

		// 8 SECURITY, 9 BRIDGE, 4 STORAGE
		if (levnum == 8 || levnum == 9 || levnum == 4) {
			if (levelEntry) return levelMusic8[9];
			if (ttype == TrackType.Combat) {
				rand = UnityEngine.Random.Range(0,6);
				return levelMusic8[rand];
			}
			rand = UnityEngine.Random.Range(6,19);
			return levelMusic8[rand];
		}

		// 6 EXECUTIVE
		if (levnum == 6) {
			if (levelEntry) return levelMusic6[0];
			if (ttype == TrackType.Combat) {
				rand = UnityEngine.Random.Range(9,13);
				return levelMusic6[rand];
			}
			rand = UnityEngine.Random.Range(0,10);
			return levelMusic6[rand];
		}

		// 10, 12 GROVES
		if (levnum == 10 || levnum == 11 || levnum == 12) {
			if (levelEntry) return levelMusicGroves[19];
			if (ttype == TrackType.Combat) {
				rand = UnityEngine.Random.Range(0,9);
				return levelMusicGroves[rand];
			}
			rand = UnityEngine.Random.Range(9,24);
			return levelMusicGroves[rand];
		}

		return null; // Tracktype none when nothing was found
	}

	public void NotifyCyberTube() {
		cyberTube = true;
	}

	public void NotifyLeftCyberTube() {
		cyberTube = false;
	}

	public void NotifyZone(TrackType tt) {
		inZone = true;
		switch(tt) {
			//case TrackType.MutantNear: return levelMusicMutantNear[levnum];
			//case TrackType.CyborgNear: return levelMusicCyborgNear[levnum];
			//case TrackType.CyborgDroneNear: return levelMusicCyborgDroneNear[levnum];
			//case TrackType.RobotNear: return levelMusicRobotNear[levnum];
			//case TrackType.Transition: return levelMusicTransition[levnum];
			//case TrackType.Revive: return levelMusicRevive[levnum];
			//case TrackType.Death: return levelMusicDeath[levnum];
			//case TrackType.Cybertube: return levelMusicCybertube[levnum];
			case TrackType.Elevator: elevator = true; break;
			case TrackType.Distortion: distortion = true; break;
		}
	}

	public void Stop() {
		// stop both
		if (SFXMain != null) SFXMain.Stop();
		if (SFXOverlay != null) SFXOverlay.Stop();
		inZone = false;
		elevator = false;
		distortion = false;
	}

    void Update() {
		// Check if main menu is active and disable playing background music
		if (mainMenu.activeSelf == true || Const.a.loadingScreen.activeSelf == true) {
			if (!paused) {
				paused = true;
				if (SFXMain != null) SFXMain.Pause();
				if (SFXOverlay != null) SFXOverlay.Pause();
			}
		} else {
			if (paused) {
				paused = false;
				if (SFXMain != null) SFXMain.UnPause();
				if (SFXOverlay != null) SFXOverlay.UnPause();
			}

			if (inCombat && !inZone && combatImpulseFinished < PauseScript.a.relativeTime) {
				inCombat = false;
				PlayTrack(LevelManager.a.currentLevel, TrackType.Combat, MusicType.Override);
				combatImpulseFinished = PauseScript.a.relativeTime + 10f;
				return;
			}

			if (!SFXMain.isPlaying) {
				if (inZone) {
					if (distortion) {
						PlayTrack(LevelManager.a.currentLevel, TrackType.Distortion, MusicType.Override);
						return;
					}
					if (elevator) {
						PlayTrack(LevelManager.a.currentLevel, TrackType.Elevator, MusicType.Override);
						return;
					}
				}
				if (LevelManager.a != null) PlayTrack(LevelManager.a.currentLevel, TrackType.Walking, MusicType.Walking);
				else  PlayTrack(1, TrackType.Walking, MusicType.Walking);
			}
		}
    }
}
