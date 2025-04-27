using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class Music : MonoBehaviour {
	public AudioSource SFXMain;
	public AudioSource SFXMain2;
	public bool twoPlaying;
	public AudioSource SFXOverlay;
	public GameObject mainMenu;
	public float audBuffer = 0.05f;

	public AudioClip titleMusic;
	public AudioClip creditsMusic;
	public AudioClip[] levelMusic; // Array due to many dynamically interchanged clips.
	public AudioClip levelMusicRevive; // Singular revive, death, etc. but unique and loaded per level.
	public AudioClip levelMusicDeath;
	public AudioClip levelMusicElevator;
	public AudioClip levelMusicDistortion;
	public AudioClip levelMusicLooped;

	private float clipFinished;
	private float clipLength;
	private float clipOverlayLength;
	private float clipOverlayFinished;
	private AudioClip tempClip;
	private AudioClip curC;
	private AudioClip curOverlayC;
	private bool paused;
	private bool cyberTube;
	public bool levelEntry;
	private int rand;
	private bool inZone;
	private bool distortion;
	private bool elevator;
	public bool inCombat;
	private float combatImpulseFinished;
	private string musicRPath;
	private string musicRLoopedPath;

	public static Music a;

	void Awake() {
		a = this;
		
		clipFinished = Time.time;
		clipOverlayFinished = Time.time;
		tempClip = null;
		levelEntry = true;
		inZone = twoPlaying = cyberTube = false;
		rand = 0;
		combatImpulseFinished = Time.time + 5f;
	}

	IEnumerator LoadHelper(string fName, MusicResourceType type, int index) {
		tempClip = null;
		string basePath = Utils.GetAppropriateDataPath();
		string fPath = type == MusicResourceType.Looped
							   ? Utils.SafePathCombine(basePath,"music","looped",fName)
							   : Utils.SafePathCombine(basePath,"music",fName);

		string fPathMp3 = fPath + ".mp3";
		string fPathWave = fPath + ".wav";
		bool isAndroidOrMacOS = (Application.platform == RuntimePlatform.Android
								 || Application.platform == RuntimePlatform.OSXEditor
								 || Application.platform == RuntimePlatform.OSXPlayer);

		bool wavExists = false;
		if (!isAndroidOrMacOS) wavExists = File.Exists(fPathWave); // Don't bother checking on Android or MacOS.

		bool mp3Exists = false;
		if (!isAndroidOrMacOS) mp3Exists = File.Exists(fPathMp3);

		if ((!wavExists && !mp3Exists) || isAndroidOrMacOS) {
			if (type == MusicResourceType.Looped) {
				tempClip = (AudioClip)Resources.Load("StreamingAssetsRecovery/music/looped/" + fName);
			} else {
				tempClip = (AudioClip)Resources.Load("StreamingAssetsRecovery/music/" + fName);
			}
		} else {
			if (!wavExists && mp3Exists) {
				string url = string.Format("file://{0}", fPathMp3);
				using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG)) {
					yield return uwr.SendWebRequest();
					if (uwr.result == UnityWebRequest.Result.Success) {
						tempClip = DownloadHandlerAudioClip.GetContent(uwr);
						tempClip.name = fName;
					}
				}
			} else {
				// Load .wav file.
				string url = string.Format("file://{0}", fPathWave);
				using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV)) {
					yield return uwr.SendWebRequest();
					if (uwr.result == UnityWebRequest.Result.Success) {
						tempClip = DownloadHandlerAudioClip.GetContent(uwr);
						tempClip.name = fName;
					}
				}
			}
		}

		if (tempClip == null) {
			UnityEngine.Debug.LogWarning("Unable to load " + fName);
			if (type == MusicResourceType.Looped) {
				tempClip = (AudioClip)Resources.Load(
					"StreamingAssetsRecovery/music/looped/" + fName
				);
			} else {
				tempClip = (AudioClip)Resources.Load(
					"StreamingAssetsRecovery/music/" + fName
				);
			}
			
			yield break;
		}

		switch (type) {
			case MusicResourceType.Menu:
				if (index == 0) {
					titleMusic = tempClip;
					titleMusic.LoadAudioData();
					while (titleMusic.loadState != AudioDataLoadState.Loaded) {
						yield return null;
					}
					PlayMenuMusic();
				} else if (index == 1) {
					creditsMusic = tempClip;
					creditsMusic.LoadAudioData();
					while (creditsMusic.loadState != AudioDataLoadState.Loaded) {
						yield return null;
					}
				}

				break;
			case MusicResourceType.Level:
				levelMusic[index] = tempClip;
				levelMusic[index].LoadAudioData();
				while (levelMusic[index].loadState != AudioDataLoadState.Loaded) {
					yield return null;
				}
				break;
			case MusicResourceType.Revive:
				levelMusicRevive = tempClip;
				levelMusicRevive.LoadAudioData();
				while (levelMusicRevive.loadState != AudioDataLoadState.Loaded) {
					yield return null;
				}
				break;
			case MusicResourceType.Death:
				levelMusicDeath = tempClip;
				levelMusicDeath.LoadAudioData();
				while (levelMusicDeath.loadState != AudioDataLoadState.Loaded) {
					yield return null;
				}
				break;
			case MusicResourceType.Elevator:
				levelMusicElevator = tempClip;
				levelMusicElevator.LoadAudioData();
				while (levelMusicElevator.loadState != AudioDataLoadState.Loaded) {
					yield return null;
				}
				break;
			case MusicResourceType.Distortion:
				levelMusicDistortion = tempClip;
				levelMusicDistortion.LoadAudioData();
				while (levelMusicDistortion.loadState != AudioDataLoadState.Loaded) {
					yield return null;
				}
				break;
			case MusicResourceType.Looped:
				levelMusicLooped = tempClip;
				levelMusicLooped.LoadAudioData();
				while (levelMusicLooped.loadState != AudioDataLoadState.Loaded) {
					yield return null;
				}
				break;
		}
	}

	private void PlayMenuMusic() {
		MainMenuHandler.a.BackGroundMusic.clip = titleMusic;
		if (MainMenuHandler.a.gameObject.activeSelf
			&& !MainMenuHandler.a.inCutscene
			&& MainMenuHandler.a.dataFound) {

			MainMenuHandler.a.BackGroundMusic.Play();
			if (MainMenuHandler.a.BackGroundMusic.clip == titleMusic
				&& MainMenuHandler.a.BackGroundMusic.isPlaying) {
				
				UnityEngine.Debug.Log("Back ground music started");
			}
		}
	}

	public void LoadLevelMusic(int levnum) {
		StartCoroutine(LoadLevelMusicActual(levnum));
	}
	
	IEnumerator LoadLevelMusicActual(int levnum) {
		levelMusic = null;
		levelMusicRevive = null;
		levelMusicDeath = null;
		levelMusicElevator = null;
		levelMusicDistortion = null;
		levelMusicLooped = null;
		tempClip = null;
		curC = null;
		curOverlayC = null;
		
		// Load all the audio clips at the start of level to prevent stutter during dynamic transitions.
		if (levnum == 1) {
			
			levelMusic = new AudioClip[11];
			yield return StartCoroutine(LoadHelper("THM1-19_medicalstart",MusicResourceType.Level,0)); 
			yield return StartCoroutine(LoadHelper("THM1-01_medicalwalking1",MusicResourceType.Level,1));
			yield return StartCoroutine(LoadHelper("THM1-02_medicalwalking2",MusicResourceType.Level,2));
			yield return StartCoroutine(LoadHelper("THM1-03_medicalwalking3",MusicResourceType.Level,3));
			yield return StartCoroutine(LoadHelper("THM1-04_medicalwalking4",MusicResourceType.Level,4));
			yield return StartCoroutine(LoadHelper("THM1-05_medicalcombat1",MusicResourceType.Level,5));
			yield return StartCoroutine(LoadHelper("THM1-06_medicalcombat2",MusicResourceType.Level,6));
			yield return StartCoroutine(LoadHelper("THM1-07_medicalcombat3",MusicResourceType.Level,7));
			yield return StartCoroutine(LoadHelper("THM1-08_medicalcombat4",MusicResourceType.Level,8));
			yield return StartCoroutine(LoadHelper("THM1-09_medicalcombat5",MusicResourceType.Level,9));
			yield return StartCoroutine(LoadHelper("THM1-10_medicalcombat6",MusicResourceType.Level,10));
			yield return StartCoroutine(LoadHelper("THM1-18_medicalrevive",MusicResourceType.Revive,1));
			yield return StartCoroutine(LoadHelper("track1",MusicResourceType.Looped,1));
			yield return StartCoroutine(LoadHelper("THM7-01_elevator1",MusicResourceType.Elevator,1));
			yield return StartCoroutine(LoadHelper("THM1-48_medicaldistorted",MusicResourceType.Distortion,1));
			yield return StartCoroutine(LoadHelper("THM1-17_death",MusicResourceType.Death,1));
		} else if (levnum == 3) {
			yield return StartCoroutine(LoadHelper("track3",MusicResourceType.Looped,3));
			yield return StartCoroutine(LoadHelper("THM7-03_elevator3",MusicResourceType.Elevator,3));
			yield return StartCoroutine(LoadHelper("THM6-22_securityrevive",MusicResourceType.Revive,8));
			yield return StartCoroutine(LoadHelper("THM0-17_death",MusicResourceType.Death,0));
			yield return StartCoroutine(LoadHelper("THM1-48_medicaldistorted",MusicResourceType.Distortion,1));
		} else if (levnum == 6) {
			levelMusic = new AudioClip[13];
			yield return StartCoroutine(LoadHelper("THM2-11_executive1",MusicResourceType.Level,0));
			yield return StartCoroutine(LoadHelper("THM2-12_executive2",MusicResourceType.Level,1));
			yield return StartCoroutine(LoadHelper("THM2-13_executive3",MusicResourceType.Level,2));
			yield return StartCoroutine(LoadHelper("THM2-08_executive4",MusicResourceType.Level,3));
			yield return StartCoroutine(LoadHelper("THM2-09_executive5",MusicResourceType.Level,4));
			yield return StartCoroutine(LoadHelper("THM2-10_executive6",MusicResourceType.Level,5));
			yield return StartCoroutine(LoadHelper("THM2-04_executive2",MusicResourceType.Level,6));
			yield return StartCoroutine(LoadHelper("THM2-05_executive3",MusicResourceType.Level,7));
			yield return StartCoroutine(LoadHelper("THM2-06_executivefluterlude",MusicResourceType.Level,8));
			yield return StartCoroutine(LoadHelper("THM2-07_executivefluterludewithguitar",MusicResourceType.Level,9));
			yield return StartCoroutine(LoadHelper("THM2-01_executiveaction3",MusicResourceType.Level,10));
			yield return StartCoroutine(LoadHelper("THM2-02_executiveaction4",MusicResourceType.Level,11));
			yield return StartCoroutine(LoadHelper("THM2-03_executiveaction5",MusicResourceType.Level,12));
			yield return StartCoroutine(LoadHelper("THM2-18_executiverevive",MusicResourceType.Revive,6));
			yield return StartCoroutine(LoadHelper("track6",MusicResourceType.Looped,6));
			yield return StartCoroutine(LoadHelper("THM2-17_death",MusicResourceType.Death,6));
			yield return StartCoroutine(LoadHelper("THM7-06_elevator6",MusicResourceType.Elevator,6));
			yield return StartCoroutine(LoadHelper("THM2-46_executivedistorted",MusicResourceType.Distortion,6));
		} else if (levnum == 8) {
			levelMusic = new AudioClip[19];
			yield return StartCoroutine(LoadHelper("THM6-05_securityaction1",MusicResourceType.Level,0));
			yield return StartCoroutine(LoadHelper("THM6-06_securityaction2",MusicResourceType.Level,1));
			yield return StartCoroutine(LoadHelper("THM6-07_securityaction3",MusicResourceType.Level,2));
			yield return StartCoroutine(LoadHelper("THM6-08_securityaction4",MusicResourceType.Level,3));
			yield return StartCoroutine(LoadHelper("THM6-09_securityaction5",MusicResourceType.Level,4));
			yield return StartCoroutine(LoadHelper("THM6-10_securityaction6",MusicResourceType.Level,5));
			yield return StartCoroutine(LoadHelper("THM6-01_security1",MusicResourceType.Level,6));
			yield return StartCoroutine(LoadHelper("THM6-02_security2",MusicResourceType.Level,7));
			yield return StartCoroutine(LoadHelper("THM6-03_security3",MusicResourceType.Level,8));
			yield return StartCoroutine(LoadHelper("THM6-04_security4",MusicResourceType.Level,9));
			yield return StartCoroutine(LoadHelper("THM6-11_security100",MusicResourceType.Level,10));
			yield return StartCoroutine(LoadHelper("THM6-12_security101",MusicResourceType.Level,11));
			yield return StartCoroutine(LoadHelper("THM6-13_security1",MusicResourceType.Level,12));
			yield return StartCoroutine(LoadHelper("THM6-14_security2",MusicResourceType.Level,13));
			yield return StartCoroutine(LoadHelper("THM6-15_security3",MusicResourceType.Level,14));
			yield return StartCoroutine(LoadHelper("THM6-17_security4",MusicResourceType.Level,15));
			yield return StartCoroutine(LoadHelper("THM6-18_security5",MusicResourceType.Level,16));
			yield return StartCoroutine(LoadHelper("THM6-19_security6",MusicResourceType.Level,17));
			yield return StartCoroutine(LoadHelper("THM6-20_security7",MusicResourceType.Level,18));
			yield return StartCoroutine(LoadHelper("THM6-22_securityrevive",MusicResourceType.Revive,8));
			yield return StartCoroutine(LoadHelper("THM6-21_death",MusicResourceType.Death,8));
			yield return StartCoroutine(LoadHelper("THM7-08_elevator8",MusicResourceType.Elevator,8));
			yield return StartCoroutine(LoadHelper("THM6-49_securitydistorted",MusicResourceType.Distortion,8));
			yield return StartCoroutine(LoadHelper("track8",MusicResourceType.Looped,8));
		} else if (levnum == 9) {
			yield return StartCoroutine(LoadHelper("THM1-18_medicalrevive",MusicResourceType.Revive,1));
			yield return StartCoroutine(LoadHelper("THM0-17_death",MusicResourceType.Death,0));
			yield return StartCoroutine(LoadHelper("THM7-01_elevator1",MusicResourceType.Elevator,1));
			yield return StartCoroutine(LoadHelper("THM1-48_medicaldistorted",MusicResourceType.Distortion,1));
			yield return StartCoroutine(LoadHelper("track9",MusicResourceType.Looped,9));
		} else if (levnum == 13) {
			levelMusic = new AudioClip[9];
			yield return StartCoroutine(LoadHelper("THM10-02_cyberstart",MusicResourceType.Level,0));
			yield return StartCoroutine(LoadHelper("THM10-01_cyber1",MusicResourceType.Level,1));
			yield return StartCoroutine(LoadHelper("THM10-03_cyber2",MusicResourceType.Level,2));
			yield return StartCoroutine(LoadHelper("THM10-04_cyber3",MusicResourceType.Level,3));
			yield return StartCoroutine(LoadHelper("THM10-05_cyber4",MusicResourceType.Level,4));
			yield return StartCoroutine(LoadHelper("THM10-06_cyber5",MusicResourceType.Level,5));
			yield return StartCoroutine(LoadHelper("THM10-07_cyber6",MusicResourceType.Level,6));
			yield return StartCoroutine(LoadHelper("THM10-08_cyber7",MusicResourceType.Level,7));
			yield return StartCoroutine(LoadHelper("THM10-09_cyber8",MusicResourceType.Level,8));
			yield return StartCoroutine(LoadHelper("THM10-16_death",MusicResourceType.Death,13));
			yield return StartCoroutine(LoadHelper("THM10-41_cyberdistorted",MusicResourceType.Distortion,13));
			yield return StartCoroutine(LoadHelper("THM1-18_medicalrevive",MusicResourceType.Revive,1));
			yield return StartCoroutine(LoadHelper("THM7-01_elevator1",MusicResourceType.Elevator,1));
			yield return StartCoroutine(LoadHelper("track13",MusicResourceType.Looped,13));
		} else if (levnum == 2 || levnum == 4) {
			levelMusic = new AudioClip[10];
			yield return StartCoroutine(LoadHelper("THM3-17_sciencestart",MusicResourceType.Level,0));
			yield return StartCoroutine(LoadHelper("THM3-03_science1",MusicResourceType.Level,1));
			yield return StartCoroutine(LoadHelper("THM3-04_science2",MusicResourceType.Level,2));
			yield return StartCoroutine(LoadHelper("THM3-05_science3",MusicResourceType.Level,3));
			yield return StartCoroutine(LoadHelper("THM3-06_science4",MusicResourceType.Level,4));
			yield return StartCoroutine(LoadHelper("THM3-07_science5",MusicResourceType.Level,5));
			yield return StartCoroutine(LoadHelper("THM3-08_science6",MusicResourceType.Level,6));
			yield return StartCoroutine(LoadHelper("THM3-09_science7",MusicResourceType.Level,7));
			yield return StartCoroutine(LoadHelper("THM3-01_scienceaction1",MusicResourceType.Level,8));
			yield return StartCoroutine(LoadHelper("THM3-02_scienceaction2",MusicResourceType.Level,9));
			yield return StartCoroutine(LoadHelper("THM3-19_sciencerevive",MusicResourceType.Revive,2));
			yield return StartCoroutine(LoadHelper("THM3-18_death",MusicResourceType.Death,2));

			// Still allow for unique per level tracks and unique per level looped tracks when Dynamic Music is off.
			if (levnum == 2) {
				yield return StartCoroutine(LoadHelper("track2",MusicResourceType.Looped,2));
				yield return StartCoroutine(LoadHelper("THM7-02_elevator2",MusicResourceType.Elevator,2));
				yield return StartCoroutine(LoadHelper("THM3-49_sciencedistorted",MusicResourceType.Distortion,2));
			} else if (levnum == 4) {
				yield return StartCoroutine(LoadHelper("track4",MusicResourceType.Looped,4));
				yield return StartCoroutine(LoadHelper("THM7-04_elevator4",MusicResourceType.Elevator,4));
				yield return StartCoroutine(LoadHelper("THM1-48_medicaldistorted",MusicResourceType.Distortion,1));
			}
		} else if (levnum == 0 || levnum == 5 || levnum == 7) {
			levelMusic = new AudioClip[13];
			yield return StartCoroutine(LoadHelper("THM4-01_reactorcombat1",MusicResourceType.Level,0));
			yield return StartCoroutine(LoadHelper("THM4-02_reactorcombat2",MusicResourceType.Level,1));
			yield return StartCoroutine(LoadHelper("THM4-03_reactorcombat3",MusicResourceType.Level,2));
			yield return StartCoroutine(LoadHelper("THM4-04_reactorcombat4",MusicResourceType.Level,3));
			yield return StartCoroutine(LoadHelper("THM4-05_reactorwalkingatocombat",MusicResourceType.Level,4));
			yield return StartCoroutine(LoadHelper("THM4-06_reactorwalkingbtocombat",MusicResourceType.Level,5));
			yield return StartCoroutine(LoadHelper("THM4-09_reactorwalkinga1",MusicResourceType.Level,6));
			yield return StartCoroutine(LoadHelper("THM4-10_reactorwalkinga2",MusicResourceType.Level,7));
			yield return StartCoroutine(LoadHelper("THM4-11_reactorwalkingb1",MusicResourceType.Level,8));
			yield return StartCoroutine(LoadHelper("THM4-12_reactorwalkingb2",MusicResourceType.Level,9));
			yield return StartCoroutine(LoadHelper("THM4-13_reactorwalkingb3",MusicResourceType.Level,10));
			yield return StartCoroutine(LoadHelper("THM4-14_reactorwalkingc1",MusicResourceType.Level,11));
			yield return StartCoroutine(LoadHelper("THM4-15_reactorwalkingc2",MusicResourceType.Level,12));
			yield return StartCoroutine(LoadHelper("THM0-17_death",MusicResourceType.Death,0));
			
			// Still allow for unique per level tracks and unique per level looped tracks when Dynamic Music is off.
			if (levnum == 0) {
				yield return StartCoroutine(LoadHelper("track0",MusicResourceType.Looped,0));
				yield return StartCoroutine(LoadHelper("THM4-18_reactorrevive",MusicResourceType.Revive,0));
				yield return StartCoroutine(LoadHelper("THM7-01_elevator1",MusicResourceType.Elevator,1));
				yield return StartCoroutine(LoadHelper("THM6-49_securitydistorted",MusicResourceType.Distortion,8));
			} else if (levnum == 5) {
				yield return StartCoroutine(LoadHelper("track5",MusicResourceType.Looped,5));
				yield return StartCoroutine(LoadHelper("THM7-05_elevator5",MusicResourceType.Elevator,5));
				yield return StartCoroutine(LoadHelper("THM1-18_medicalrevive",MusicResourceType.Revive,1));
				yield return StartCoroutine(LoadHelper("THM1-48_medicaldistorted",MusicResourceType.Distortion,1));
			} else if (levnum == 7) {
				yield return StartCoroutine(LoadHelper("track7",MusicResourceType.Looped,7));
				yield return StartCoroutine(LoadHelper("THM7-07_elevator7",MusicResourceType.Elevator,7));
				yield return StartCoroutine(LoadHelper("THM1-18_medicalrevive",MusicResourceType.Revive,1));
				yield return StartCoroutine(LoadHelper("THM1-48_medicaldistorted",MusicResourceType.Distortion,1));
			}
		} else if (levnum == 10 || levnum == 11 || levnum == 12) {
			levelMusic = new AudioClip[24];
			yield return StartCoroutine(LoadHelper("THM5-07_groveaction1",MusicResourceType.Level,0));
			yield return StartCoroutine(LoadHelper("THM5-08_groveaction1",MusicResourceType.Level,1));
			yield return StartCoroutine(LoadHelper("THM5-09_groveaction2",MusicResourceType.Level,2));
			yield return StartCoroutine(LoadHelper("THM5-10_groveaction3",MusicResourceType.Level,3));
			yield return StartCoroutine(LoadHelper("THM5-11_groveaction4",MusicResourceType.Level,4));
			yield return StartCoroutine(LoadHelper("THM5-12_groveaction5",MusicResourceType.Level,5));
			yield return StartCoroutine(LoadHelper("THM5-13_groveaction6",MusicResourceType.Level,6));
			yield return StartCoroutine(LoadHelper("THM5-14_groveaction7",MusicResourceType.Level,7));
			yield return StartCoroutine(LoadHelper("THM5-15_groveaction8",MusicResourceType.Level,8));
			yield return StartCoroutine(LoadHelper("THM5-33_grove1",MusicResourceType.Level,9));
			yield return StartCoroutine(LoadHelper("THM5-34_grove2",MusicResourceType.Level,10));
			yield return StartCoroutine(LoadHelper("THM5-38_grove3",MusicResourceType.Level,11));
			yield return StartCoroutine(LoadHelper("THM5-39_grove4",MusicResourceType.Level,12));
			yield return StartCoroutine(LoadHelper("THM5-40_grove5",MusicResourceType.Level,13));
			yield return StartCoroutine(LoadHelper("THM5-35_grove99",MusicResourceType.Level,14));
			yield return StartCoroutine(LoadHelper("THM5-36_grove100",MusicResourceType.Level,15));
			yield return StartCoroutine(LoadHelper("THM5-37_grove101",MusicResourceType.Level,16));
			yield return StartCoroutine(LoadHelper("THM5-41_grove102",MusicResourceType.Level,17));
			yield return StartCoroutine(LoadHelper("THM5-42_grove103",MusicResourceType.Level,18));
			yield return StartCoroutine(LoadHelper("THM5-01_grove105",MusicResourceType.Level,19));
			yield return StartCoroutine(LoadHelper("THM5-02_grove106",MusicResourceType.Level,20));
			yield return StartCoroutine(LoadHelper("THM5-03_grove107",MusicResourceType.Level,21));
			yield return StartCoroutine(LoadHelper("THM5-04_grove108",MusicResourceType.Level,22));
			yield return StartCoroutine(LoadHelper("THM5-05_grove109",MusicResourceType.Level,23));
			yield return StartCoroutine(LoadHelper("THM5-17_death",MusicResourceType.Death,10));
			yield return StartCoroutine(LoadHelper("THM1-18_medicalrevive",MusicResourceType.Revive,1));
			yield return StartCoroutine(LoadHelper("THM7-01_elevator1",MusicResourceType.Elevator,1));
			yield return StartCoroutine(LoadHelper("THM1-48_medicaldistorted",MusicResourceType.Distortion,1));
			
			// Still allow for unique per level tracks and unique per level looped tracks when Dynamic Music is off.
			if (levnum == 10) {
				yield return StartCoroutine(LoadHelper("track10",MusicResourceType.Looped,10));
			} else if (levnum == 11) {
				yield return StartCoroutine(LoadHelper("track11",MusicResourceType.Looped,11));
			} else if (levnum == 12) {
				yield return StartCoroutine(LoadHelper("track12",MusicResourceType.Looped,12));
			}
		}
	}

	public void PlayTrack(int levnum, TrackType ttype, MusicType mtype) {
		// Looped Music (Dynamic Music off)
		// --------------------------------------------------------------------
		if (!Const.a.DynamicMusic) {
			if (mtype == MusicType.Overlay) return; // No overlays in looped.
			if (mtype == MusicType.Override && (ttype == TrackType.MutantNear
				|| ttype == TrackType.Cybertube || ttype == TrackType.RobotNear
				|| ttype == TrackType.CyborgNear
				|| ttype == TrackType.CyborgDroneNear
				|| ttype == TrackType.Transition)) {

				return; // Some dynamic music not used in looped.
			}

			float vol = 0.2f;
			if (Const.a != null) vol = Const.a.AudioVolumeMusic;
			if (mtype == MusicType.Walking || mtype == MusicType.Combat || mtype == MusicType.None) {
				tempClip = levelMusicLooped;
			} else if (mtype == MusicType.Override) {
				if (ttype == TrackType.Revive) {
					tempClip = levelMusicRevive;
				} else if (ttype == TrackType.Death) {
					tempClip = levelMusicDeath;
				} else if (ttype == TrackType.Elevator) {
					tempClip = levelMusicElevator;
				} else if (ttype == TrackType.Distortion) {
					tempClip = levelMusicDistortion;
				}
			}

			SFXMain.Play();
			SFXMain.loop = true;
			SFXMain2.Stop();
			return;
		}

		// Normal Dynamic Music System
		// --------------------------------------------------------------------
		tempClip = GetCorrespondingLevelClip(levnum,ttype);
		if (!elevator) levelEntry = false; // already used by GetCorresponding... just now
		if (mtype == MusicType.Walking || mtype == MusicType.Combat) {
			AudioSource curr = twoPlaying ? SFXMain2 : SFXMain;
			if (tempClip == curC && curr.isPlaying) return; // no need, already playing

			if (tempClip != null) {
				curr.clip = tempClip;
				curC = tempClip;
				curr.Play();
				curr.loop = false;
			} else {
				curC = null;
			}
		}

		if (mtype == MusicType.Overlay) {
			if (tempClip == curOverlayC && SFXOverlay.isPlaying) return; // no need, already playing
			
			if (SFXOverlay != null) SFXOverlay.Stop(); // stop any overlays
			if (tempClip != null) {
				SFXOverlay.clip = tempClip;
				curOverlayC = tempClip;
				SFXOverlay.Play();
				SFXOverlay.loop = false;
			} else {
				curOverlayC = null;
			}
		}

		if (mtype == MusicType.Override) {
			if (tempClip == curC && SFXMain.isPlaying) return; // no need, already playing

			// stop both
			if (SFXMain != null) SFXMain.Stop();
			if (SFXOverlay != null) SFXOverlay.Stop();
			if (tempClip != null) {
				SFXMain.clip = tempClip;
				curC = tempClip;
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
			if (levelEntry && levelMusic != null) {
				if (levelMusic.Length > 0) {
					if (levelMusic[0] != null) return levelMusic[0];
				}
			}
			if (cyberTube) {
				rand = UnityEngine.Random.Range(4,8);
				return levelMusic[rand];
			}
			if (UnityEngine.Random.Range(0,1f) < 0.5f) {
				rand = UnityEngine.Random.Range(1,5);
				return levelMusic[rand];
			} else {
				return levelMusic[8];
			}
		}

		// These act as override types, return from these first before special level handling
		switch(ttype) {
			//case TrackType.MutantNear: return levelMusicMutantNear[levnum];
			//case TrackType.CyborgNear: return levelMusicCyborgNear[levnum];
			//case TrackType.CyborgDroneNear: return levelMusicCyborgDroneNear[levnum];
			//case TrackType.RobotNear: return levelMusicRobotNear[levnum];
			//case TrackType.Transition: return levelMusicTransition[levnum];
			case TrackType.Revive: return levelMusicRevive;
			case TrackType.Death: return levelMusicDeath;
			case TrackType.Elevator: return levelMusicElevator;
			case TrackType.Distortion: return levelMusicDistortion;
		}

		// 1  MEDICAL
		if (levnum == 1) {
			if (levelEntry) return levelMusic[0];
			if (ttype == TrackType.Combat) {
				rand = UnityEngine.Random.Range(5,11);
				return levelMusic[rand];
			}
			rand = UnityEngine.Random.Range(1,5);
			return levelMusic[rand];
		}

		// 2  SCIENCE, 4 STORAGE
		if (levnum == 2 || levnum == 4) {
			if (levelEntry) return levelMusic[0];
			if (ttype == TrackType.Combat) {
				rand = UnityEngine.Random.Range(8,10);
				return levelMusic[rand];
			}
			rand = UnityEngine.Random.Range(1,8);
			return levelMusic[rand];
		}

		// 0  REACTOR, 5 FLIGHT, 7 ENGINEERING
		if (levnum == 0 || levnum == 5 || levnum == 7) {
			if (levelEntry) return levelMusic[6];
			if (ttype == TrackType.Combat) {
				rand = UnityEngine.Random.Range(0,6);
				return levelMusic[rand];
			}
			rand = UnityEngine.Random.Range(6,13);
			return levelMusic[rand];
		}

		// 8 SECURITY
		if (levnum == 8) {
			if (levelEntry) return levelMusic[9];
			if (ttype == TrackType.Combat) {
				rand = UnityEngine.Random.Range(0,6);
				return levelMusic[rand];
			}
			rand = UnityEngine.Random.Range(6,19);
			return levelMusic[rand];
		}

		// 6 EXECUTIVE
		if (levnum == 6) {
			if (levelEntry) return levelMusic[0];
			if (ttype == TrackType.Combat) {
				rand = UnityEngine.Random.Range(9,13);
				return levelMusic[rand];
			}
			rand = UnityEngine.Random.Range(0,10);
			return levelMusic[rand];
		}

		// 10, 12 GROVES
		if (levnum == 10 || levnum == 11 || levnum == 12) {
			if (levelEntry) return levelMusic[19];
			if (ttype == TrackType.Combat) {
				rand = UnityEngine.Random.Range(0,9);
				return levelMusic[rand];
			}
			rand = UnityEngine.Random.Range(9,24);
			return levelMusic[rand];
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
		if (mainMenu.activeSelf == true
			|| Const.a.loadingScreen.activeSelf == true
			|| !MainMenuHandler.a.dataFound) {

			if (!paused || !MainMenuHandler.a.dataFound) {
				paused = true;
				if (SFXMain != null) SFXMain.Pause();
				if (SFXMain2 != null) SFXMain2.Pause();
				if (SFXOverlay != null) SFXOverlay.Pause();
			}

			return;
		}

		if (paused) {
			paused = false;
			if (SFXMain != null && (!twoPlaying || !Const.a.DynamicMusic)) SFXMain.UnPause();
			if (SFXMain2 != null && twoPlaying && Const.a.DynamicMusic) SFXMain2.UnPause();
			if (SFXOverlay != null) SFXOverlay.UnPause();
		}

		AudioSource curr = twoPlaying ? SFXMain2 : SFXMain;
		AudioSource next = twoPlaying ? SFXMain : SFXMain2;
		SFXMain2.volume = SFXMain.volume;
		if (curr.clip != null && curr.isPlaying) {
            float remaining = (curr.clip.length - curr.time);
            if (remaining > audBuffer) return;
        }

		if (inCombat && !inZone && combatImpulseFinished < PauseScript.a.relativeTime) {
			inCombat = false;
			PlayTrack(LevelManager.a.currentLevel,TrackType.Combat, MusicType.Override);
			combatImpulseFinished = PauseScript.a.relativeTime + 20f;
			return;
		}

		if (inZone) {
			if (distortion) {
				PlayTrack(LevelManager.a.currentLevel,TrackType.Distortion, MusicType.Override);
				return;
			}
			
			if (elevator) {
				PlayTrack(LevelManager.a.currentLevel,TrackType.Elevator, MusicType.Override);
				return;
			}
		}
		
		if (Const.a.DynamicMusic) {
			if (curr.clip != null && curr.isPlaying) {
				float remaining = curr.clip.length - curr.time;
				if (remaining <= audBuffer) { // 50ms buffer before end
					twoPlaying = !twoPlaying;
					PlayTrack(LevelManager.a.currentLevel,TrackType.Walking, MusicType.Walking);
				}
			} else {
				PlayTrack(LevelManager.a.currentLevel,TrackType.Walking, MusicType.Walking);
			}
		} else {
			twoPlaying = false;
			PlayTrack(LevelManager.a.currentLevel,TrackType.Walking, MusicType.Walking);
		}
    }
    
    void OnDestroy() { // This reduces total RAM usage from 4.0GB to 2.6GB 8)
		titleMusic = null;
		creditsMusic = null;
		levelMusic = null;
		levelMusicRevive = null;
		levelMusicDeath = null;
		levelMusicElevator = null;
		levelMusicDistortion = null;
		levelMusicLooped = null;
		tempClip = null;
		curC = null;
		curOverlayC = null;
		if (a == this) a = null;
	}
}
