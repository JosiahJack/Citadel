using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System;
using UnityEngine;

public class Music : MonoBehaviour {
	public AudioSource SFXMain;
	public AudioSource SFXOverlay;
	public GameObject mainMenu;

	[HideInInspector] public AudioClip titleMusic;
	[HideInInspector] public AudioClip creditsMusic;
	[HideInInspector] public AudioClip[] levelMusic1;
	[HideInInspector] public AudioClip[] levelMusic2;
	[HideInInspector] public AudioClip[] levelMusicReactor;
	[HideInInspector] public AudioClip[] levelMusic6;
	[HideInInspector] public AudioClip[] levelMusicGroves;
	[HideInInspector] public AudioClip[] levelMusic8;
	[HideInInspector] public AudioClip[] levelMusicRevive;
	[HideInInspector] public AudioClip[] levelMusicDeath;
	[HideInInspector] public AudioClip[] levelMusicCyber;
	[HideInInspector] public AudioClip[] levelMusicElevator;
	[HideInInspector] public AudioClip[] levelMusicDistortion;
	[HideInInspector] public AudioClip[] levelMusicLooped;

	private float clipFinished;
	private float clipLength;
	private float clipOverlayLength;
	private float clipOverlayFinished;
	private AudioClip tempC;
	private AudioClip curC;
	private AudioClip curOverlayC;
	private bool paused;
	private bool cyberTube;
	[HideInInspector] public bool levelEntry;
	private int rand;
	private bool inZone;
	private bool distortion;
	private bool elevator;
	[HideInInspector] public bool inCombat;
	private float combatImpulseFinished;
	private AudioClip tempClip;
	private string musicRPath;
	private string musicRLoopedPath;

	public static Music a;

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

	public void LoadAudio(string fName, MusicResourceType type, int index) {
		StartCoroutine(LoadHelper(fName,type,index));
	}

	#pragma warning disable 618
	IEnumerator LoadHelper(string fName, MusicResourceType type, int index) {
		tempClip = null;
		string fPath;
		if (type == MusicResourceType.Looped) {
			fPath = Application.streamingAssetsPath + "/music/looped/" + fName;
		} else {
			fPath = Application.streamingAssetsPath + "/music/" + fName;
		}

		string fPathMp3 = fPath + ".mp3";
		string fPathWave = fPath + ".wav";
		bool wavExists = File.Exists(fPathWave);
		bool mp3Exists = File.Exists(fPathMp3);
		bool madeNewWave = false;
		if (!wavExists && !mp3Exists) {
			if (type == MusicResourceType.Looped) {
				tempClip = (AudioClip)Resources.Load(
					"StreamingAssetsRecovery/music/looped/" + fName
				);
			} else {
				tempClip = (AudioClip)Resources.Load(
					"StreamingAssetsRecovery/music/" + fName
				);
			}
		} else {
			if (!wavExists && mp3Exists) {
				if (Application.platform == RuntimePlatform.WindowsPlayer
					|| Application.platform == RuntimePlatform.WindowsEditor) {
					string url = string.Format("file://{0}", fPathMp3);
					WWW www = new WWW(url);
					using (www) {
						yield return www;

						tempClip = NAudioPlayer.FromMp3Data(www.bytes);
					}

					tempClip.name = fName;
				} else {
					ProcessStartInfo psi = new ProcessStartInfo();
					psi.FileName = "/bin/sh";
					psi.UseShellExecute = false;
					psi.CreateNoWindow = true;
					psi.RedirectStandardInput = true;
 					psi.RedirectStandardOutput = true;
					if (type == MusicResourceType.Looped) {
						psi.WorkingDirectory = Application.streamingAssetsPath
						+ "/music/looped";
					} else {
						psi.WorkingDirectory = Application.streamingAssetsPath
											   + "/music";
					}

					Process proc = new Process();
					using (proc) {
						proc.StartInfo = psi;
						proc.Start();
						proc.StandardInput.WriteLine("ffmpeg -i " + fName
													 + ".mp3 " + fName
													 + ".wav");

						proc.StandardInput.WriteLine("exit");
						proc.StandardInput.Flush();
						while (!proc.HasExited) {
							yield return null;
						}
					}

					if (File.Exists(fPathWave)) {
						madeNewWave = true;
						// TODO: Need 3 /// on Windows?  Need to test.
						string url;
						url = string.Format("file://{0}", fPathWave);
						WWW www = new WWW(url);
						using (www) {
							yield return www;

							tempClip = www.GetAudioClipCompressed(false);
						}

						tempClip.name = fName;
					} else {
						UnityEngine.Debug.Log("Process failed.");
						if (type == MusicResourceType.Looped) {
							tempClip = (AudioClip)Resources.Load(
								"StreamingAssetsRecovery/music/looped/" + fName
							);
						} else {
							tempClip = (AudioClip)Resources.Load(
								"StreamingAssetsRecovery/music/" + fName
							);
						}
					}
				}
			} else {
				// Load .wav file.
				// TODO: Need 3 /// on Windows?  Need to test.
				string url = string.Format("file://{0}", fPathWave);
				WWW www = new WWW(url);
				using (www) {
					yield return www;

					tempClip = www.GetAudioClipCompressed(false);
				}

				tempClip.name = fName;
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
					PlayMenuMusic();
				} else if (index == 1) {
					creditsMusic = tempClip;
					creditsMusic.LoadAudioData();
				}

				break;
			case MusicResourceType.Medical:
				levelMusic1[index] = tempClip;
				levelMusic1[index].LoadAudioData();
				break;
			case MusicResourceType.Science:
				levelMusic2[index] = tempClip;
				levelMusic2[index].LoadAudioData();
				break;
			case MusicResourceType.Reactor:
				levelMusicReactor[index] = tempClip;
				levelMusicReactor[index].LoadAudioData();
				break;
			case MusicResourceType.Executive:
				levelMusic6[index] = tempClip;
				levelMusic6[index].LoadAudioData();
				break;
			case MusicResourceType.Grove:
				levelMusicGroves[index] = tempClip;
				levelMusicGroves[index].LoadAudioData();
				break;
			case MusicResourceType.Security:
				levelMusic8[index] = tempClip;
				levelMusic8[index].LoadAudioData();
				break;
			case MusicResourceType.Revive:
				levelMusicRevive[index] = tempClip;
				levelMusicRevive[index].LoadAudioData();
				break;
			case MusicResourceType.Death:
				levelMusicDeath[index] = tempClip;
				levelMusicDeath[index].LoadAudioData();
				break;
			case MusicResourceType.Cyber:
				levelMusicCyber[index] = tempClip;
				levelMusicCyber[index].LoadAudioData();
				break;
			case MusicResourceType.Elevator:
				levelMusicElevator[index] = tempClip;
				levelMusicElevator[index].LoadAudioData();
				break;
			case MusicResourceType.Distortion:
				levelMusicDistortion[index] = tempClip;
				levelMusicDistortion[index].LoadAudioData();
				break;
			case MusicResourceType.Looped:
				levelMusicLooped[index] = tempClip;
				levelMusicLooped[index].LoadAudioData();
				break;
		}

		if (madeNewWave) {
			if (File.Exists(fPathWave)) File.Delete(fPathWave); // Clean up.
		}
	}

	#pragma warning restore 618
	private void PlayMenuMusic() {
		if (Const.a != null) {
			if (Const.a.DynamicMusic) {
				MainMenuHandler.a.BackGroundMusic.clip = titleMusic;
			} else {
				MainMenuHandler.a.BackGroundMusic.clip = levelMusicLooped[14];
			}
		} else {
			MainMenuHandler.a.BackGroundMusic.clip = titleMusic;
		}

		if (MainMenuHandler.a.gameObject.activeSelf
			&& !MainMenuHandler.a.inCutscene
			&& MainMenuHandler.a.dataFound) {

			MainMenuHandler.a.BackGroundMusic.Play();
		}
	}

	private void LoadMusic() {
		// Load all the audio clips at the start to prevent stutter.
		levelMusicLooped = new AudioClip[19];
		LoadAudio("TITLOOP-00_menu",MusicResourceType.Menu,0);
		LoadAudio("END-00_end",MusicResourceType.Menu,1);
		LoadAudio("THM1-19_medicalstart",MusicResourceType.Medical,0);
		LoadAudio("THM1-01_medicalwalking1",MusicResourceType.Medical,1);
		LoadAudio("THM1-02_medicalwalking2",MusicResourceType.Medical,2);
		LoadAudio("THM1-03_medicalwalking3",MusicResourceType.Medical,3);
		LoadAudio("THM1-04_medicalwalking4",MusicResourceType.Medical,4);
		LoadAudio("THM1-05_medicalcombat1",MusicResourceType.Medical,5);
		LoadAudio("THM1-06_medicalcombat2",MusicResourceType.Medical,6);
		LoadAudio("THM1-07_medicalcombat3",MusicResourceType.Medical,7);
		LoadAudio("THM1-08_medicalcombat4",MusicResourceType.Medical,8);
		LoadAudio("THM1-09_medicalcombat5",MusicResourceType.Medical,9);
		LoadAudio("THM1-10_medicalcombat6",MusicResourceType.Medical,10);
		LoadAudio("THM3-17_sciencestart",MusicResourceType.Science,0);
		LoadAudio("THM3-03_science1",MusicResourceType.Science,1);
		LoadAudio("THM3-04_science2",MusicResourceType.Science,2);
		LoadAudio("THM3-05_science3",MusicResourceType.Science,3);
		LoadAudio("THM3-06_science4",MusicResourceType.Science,4);
		LoadAudio("THM3-07_science5",MusicResourceType.Science,5);
		LoadAudio("THM3-08_science6",MusicResourceType.Science,6);
		LoadAudio("THM3-09_science7",MusicResourceType.Science,7);
		LoadAudio("THM3-01_scienceaction1",MusicResourceType.Science,8);
		LoadAudio("THM3-02_scienceaction2",MusicResourceType.Science,9);
		LoadAudio("THM4-01_reactorcombat1",MusicResourceType.Reactor,0);
		LoadAudio("THM4-02_reactorcombat2",MusicResourceType.Reactor,1);
		LoadAudio("THM4-03_reactorcombat3",MusicResourceType.Reactor,2);
		LoadAudio("THM4-04_reactorcombat4",MusicResourceType.Reactor,3);
		LoadAudio("THM4-05_reactorwalkingatocombat",MusicResourceType.Reactor,4);
		LoadAudio("THM4-06_reactorwalkingbtocombat",MusicResourceType.Reactor,5);
		LoadAudio("THM4-09_reactorwalkinga1",MusicResourceType.Reactor,6);
		LoadAudio("THM4-10_reactorwalkinga2",MusicResourceType.Reactor,7);
		LoadAudio("THM4-11_reactorwalkingb1",MusicResourceType.Reactor,8);
		LoadAudio("THM4-12_reactorwalkingb2",MusicResourceType.Reactor,9);
		LoadAudio("THM4-13_reactorwalkingb3",MusicResourceType.Reactor,10);
		LoadAudio("THM4-14_reactorwalkingc1",MusicResourceType.Reactor,11);
		LoadAudio("THM4-15_reactorwalkingc2",MusicResourceType.Reactor,12);
		LoadAudio("THM2-11_executive1",MusicResourceType.Executive,0);
		LoadAudio("THM2-12_executive2",MusicResourceType.Executive,1);
		LoadAudio("THM2-13_executive3",MusicResourceType.Executive,2);
		LoadAudio("THM2-08_executive4",MusicResourceType.Executive,3);
		LoadAudio("THM2-09_executive5",MusicResourceType.Executive,4);
		LoadAudio("THM2-10_executive6",MusicResourceType.Executive,5);
		LoadAudio("THM2-04_executive2",MusicResourceType.Executive,6);
		LoadAudio("THM2-05_executive3",MusicResourceType.Executive,7);
		LoadAudio("THM2-06_executivefluterlude",MusicResourceType.Executive,8);
		LoadAudio("THM2-07_executivefluterludewithguitar",MusicResourceType.Executive,9);
		LoadAudio("THM2-01_executiveaction3",MusicResourceType.Executive,10);
		LoadAudio("THM2-02_executiveaction4",MusicResourceType.Executive,11);
		LoadAudio("THM2-03_executiveaction5",MusicResourceType.Executive,12);
		LoadAudio("THM5-07_groveaction1",MusicResourceType.Grove,0);
		LoadAudio("THM5-08_groveaction1",MusicResourceType.Grove,1);
		LoadAudio("THM5-09_groveaction2",MusicResourceType.Grove,2);
		LoadAudio("THM5-10_groveaction3",MusicResourceType.Grove,3);
		LoadAudio("THM5-11_groveaction4",MusicResourceType.Grove,4);
		LoadAudio("THM5-12_groveaction5",MusicResourceType.Grove,5);
		LoadAudio("THM5-13_groveaction6",MusicResourceType.Grove,6);
		LoadAudio("THM5-14_groveaction7",MusicResourceType.Grove,7);
		LoadAudio("THM5-15_groveaction8",MusicResourceType.Grove,8);
		LoadAudio("THM5-33_grove1",MusicResourceType.Grove,9);
		LoadAudio("THM5-34_grove2",MusicResourceType.Grove,10);
		LoadAudio("THM5-38_grove3",MusicResourceType.Grove,11);
		LoadAudio("THM5-39_grove4",MusicResourceType.Grove,12);
		LoadAudio("THM5-40_grove5",MusicResourceType.Grove,13);
		LoadAudio("THM5-35_grove99",MusicResourceType.Grove,14);
		LoadAudio("THM5-36_grove100",MusicResourceType.Grove,15);
		LoadAudio("THM5-37_grove101",MusicResourceType.Grove,16);
		LoadAudio("THM5-41_grove102",MusicResourceType.Grove,17);
		LoadAudio("THM5-42_grove103",MusicResourceType.Grove,18);
		LoadAudio("THM5-01_grove105",MusicResourceType.Grove,19);
		LoadAudio("THM5-02_grove106",MusicResourceType.Grove,20);
		LoadAudio("THM5-03_grove107",MusicResourceType.Grove,21);
		LoadAudio("THM5-04_grove108",MusicResourceType.Grove,22);
		LoadAudio("THM5-05_grove109",MusicResourceType.Grove,23);
		LoadAudio("THM6-05_securityaction1",MusicResourceType.Security,0);
		LoadAudio("THM6-06_securityaction2",MusicResourceType.Security,1);
		LoadAudio("THM6-07_securityaction3",MusicResourceType.Security,2);
		LoadAudio("THM6-08_securityaction4",MusicResourceType.Security,3);
		LoadAudio("THM6-09_securityaction5",MusicResourceType.Security,4);
		LoadAudio("THM6-10_securityaction6",MusicResourceType.Security,5);
		LoadAudio("THM6-01_security1",MusicResourceType.Security,6);
		LoadAudio("THM6-02_security2",MusicResourceType.Security,7);
		LoadAudio("THM6-03_security3",MusicResourceType.Security,8);
		LoadAudio("THM6-04_security4",MusicResourceType.Security,9);
		LoadAudio("THM6-11_security100",MusicResourceType.Security,10);
		LoadAudio("THM6-12_security101",MusicResourceType.Security,11);
		LoadAudio("THM6-13_security1",MusicResourceType.Security,12);
		LoadAudio("THM6-14_security2",MusicResourceType.Security,13);
		LoadAudio("THM6-15_security3",MusicResourceType.Security,14);
		LoadAudio("THM6-17_security4",MusicResourceType.Security,15);
		LoadAudio("THM6-18_security5",MusicResourceType.Security,16);
		LoadAudio("THM6-19_security6",MusicResourceType.Security,17);
		LoadAudio("THM6-20_security7",MusicResourceType.Security,18);
		LoadAudio("THM4-18_reactorrevive",MusicResourceType.Revive,0);
		LoadAudio("THM1-18_medicalrevive",MusicResourceType.Revive,1);
		LoadAudio("THM3-19_sciencerevive",MusicResourceType.Revive,2);
		LoadAudio("THM6-22_securityrevive",MusicResourceType.Revive,3);
		levelMusicRevive[4] = levelMusicRevive[2];
		levelMusicRevive[5] = levelMusicRevive[1];
		LoadAudio("THM2-18_executiverevive",MusicResourceType.Revive,6);
		levelMusicRevive[7] = levelMusicRevive[1];
		levelMusicRevive[8] = levelMusicRevive[3];
		levelMusicRevive[9] = levelMusicRevive[1];
		levelMusicRevive[10] = levelMusicRevive[1];
		levelMusicRevive[11] = levelMusicRevive[1];
		levelMusicRevive[12] = levelMusicRevive[1];
		levelMusicRevive[13] = levelMusicRevive[1];
		LoadAudio("THM0-17_death",MusicResourceType.Death,0);
		LoadAudio("THM1-17_death",MusicResourceType.Death,1);
		LoadAudio("THM3-18_death",MusicResourceType.Death,2);
		levelMusicDeath[3] = levelMusicDeath[0];
		levelMusicDeath[4] = levelMusicDeath[2];
		levelMusicDeath[5] = levelMusicDeath[0];
		LoadAudio("THM2-17_death",MusicResourceType.Death,6);
		levelMusicDeath[7] = levelMusicDeath[0];
		LoadAudio("THM6-21_death",MusicResourceType.Death,8);
		levelMusicDeath[9] = levelMusicDeath[0];
		LoadAudio("THM5-17_death",MusicResourceType.Death,10);
		LoadAudio("THM5-17_death",MusicResourceType.Death,11);
		LoadAudio("THM5-17_death",MusicResourceType.Death,12);
		LoadAudio("THM10-16_death",MusicResourceType.Death,13);
		LoadAudio("THM10-02_cyberstart",MusicResourceType.Cyber,0);
		LoadAudio("THM10-01_cyber1",MusicResourceType.Cyber,1);
		LoadAudio("THM10-03_cyber2",MusicResourceType.Cyber,2);
		LoadAudio("THM10-04_cyber3",MusicResourceType.Cyber,3);
		LoadAudio("THM10-05_cyber4",MusicResourceType.Cyber,4);
		LoadAudio("THM10-06_cyber5",MusicResourceType.Cyber,5);
		LoadAudio("THM10-07_cyber6",MusicResourceType.Cyber,6);
		LoadAudio("THM10-08_cyber7",MusicResourceType.Cyber,7);
		LoadAudio("THM10-09_cyber8",MusicResourceType.Cyber,8);
		LoadAudio("THM7-01_elevator1",MusicResourceType.Elevator,1);
		levelMusicElevator[0] = levelMusicElevator[1];
		LoadAudio("THM7-02_elevator2",MusicResourceType.Elevator,2);
		LoadAudio("THM7-03_elevator3",MusicResourceType.Elevator,3);
		LoadAudio("THM7-04_elevator4",MusicResourceType.Elevator,4);
		LoadAudio("THM7-05_elevator5",MusicResourceType.Elevator,5);
		LoadAudio("THM7-06_elevator6",MusicResourceType.Elevator,6);
		LoadAudio("THM7-07_elevator7",MusicResourceType.Elevator,7);
		LoadAudio("THM7-08_elevator8",MusicResourceType.Elevator,8);
		levelMusicElevator[9] = levelMusicElevator[1];
		levelMusicElevator[10] = levelMusicElevator[1];
		levelMusicElevator[11] = levelMusicElevator[1];
		levelMusicElevator[12] = levelMusicElevator[1];
		levelMusicElevator[13] = levelMusicElevator[1];
		LoadAudio("THM1-48_medicaldistorted",MusicResourceType.Distortion,1);
		LoadAudio("THM3-49_sciencedistorted",MusicResourceType.Distortion,2);
		levelMusicDistortion[3] = levelMusicDistortion[1];
		levelMusicDistortion[4] = levelMusicDistortion[1];
		levelMusicDistortion[5] = levelMusicDistortion[1];
		LoadAudio("THM2-46_executivedistorted",MusicResourceType.Distortion,6);
		levelMusicDistortion[7] = levelMusicDistortion[1];
		LoadAudio("THM6-49_securitydistorted",MusicResourceType.Distortion,8);
		levelMusicDistortion[0] = levelMusicDistortion[8];

		levelMusicDistortion[9] = levelMusicDistortion[1];
		levelMusicDistortion[10] = levelMusicDistortion[1];
		levelMusicDistortion[11] = levelMusicDistortion[1];
		levelMusicDistortion[12] = levelMusicDistortion[1];
		LoadAudio("THM10-41_cyberdistorted",MusicResourceType.Distortion,13);

		// Looped Music for when Dynamic Music is off
		LoadAudio("track0",MusicResourceType.Looped,0);
		LoadAudio("track1",MusicResourceType.Looped,1);
		LoadAudio("track2",MusicResourceType.Looped,2);
		LoadAudio("track3",MusicResourceType.Looped,3);
		LoadAudio("track4",MusicResourceType.Looped,4);
		LoadAudio("track5",MusicResourceType.Looped,5);
		LoadAudio("track6",MusicResourceType.Looped,6);
		LoadAudio("track7",MusicResourceType.Looped,7);
		LoadAudio("track8",MusicResourceType.Looped,8);
		LoadAudio("track9",MusicResourceType.Looped,9);
		LoadAudio("track10",MusicResourceType.Looped,10);
		LoadAudio("track11",MusicResourceType.Looped,11);
		LoadAudio("track12",MusicResourceType.Looped,12);
		LoadAudio("track13",MusicResourceType.Looped,13);
		LoadAudio("titloop",MusicResourceType.Looped,14);
		LoadAudio("elevator",MusicResourceType.Looped,15);
		LoadAudio("death",MusicResourceType.Looped,16);
		LoadAudio("credits",MusicResourceType.Looped,17);
		LoadAudio("revive",MusicResourceType.Looped,18);
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
				|| ttype == TrackType.Transition
				|| ttype == TrackType.Distortion)) {

				return; // Some dynamic music not used in looped.
			}

			float vol = 0.2f;
			if (Const.a != null) vol = Const.a.AudioVolumeMusic;
			if (mtype == MusicType.Walking || mtype == MusicType.Combat
				|| mtype == MusicType.None) {

				tempC = levelMusicLooped[LevelManager.a.currentLevel];
			} else if (mtype == MusicType.Override) {
				if (ttype == TrackType.Revive) {
					tempC = levelMusicLooped[18];
				} else if (ttype == TrackType.Death) {
					tempC = levelMusicLooped[16];
				} else if (ttype == TrackType.Elevator) {
					tempC = levelMusicLooped[15];
				}
			}

			Utils.PlayAudioSavable(SFXMain,tempC,vol,false);
			SFXMain.loop = true;
			return;
		}

		// Normal Dynamic Music System
		// --------------------------------------------------------------------
		tempC = GetCorrespondingLevelClip(levnum,ttype);
		if (!elevator) levelEntry = false; // already used by GetCorresponding... just now
		if (mtype == MusicType.Walking || mtype == MusicType.Combat) {
			if (tempC == curC && SFXMain.isPlaying) return; // no need, already playing
			if (SFXMain != null) SFXMain.Stop(); // stop playing normal
			if (tempC != null) {
				SFXMain.clip = tempC;
				curC = tempC;
// 				Utils.PlayOneShotSavable(SFXMain,tempC);
				SFXMain.Play();
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
// 				Utils.PlayOneShotSavable(SFXOverlay,tempC);
				SFXOverlay.Play();
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

		// 2  SCIENCE, 4 STORAGE
		if (levnum == 2 || levnum == 4) {
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

		// 8 SECURITY
		if (levnum == 8) {
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
		if (mainMenu.activeSelf == true
			|| Const.a.loadingScreen.activeSelf == true
			|| !MainMenuHandler.a.dataFound) {

			if (!paused || !MainMenuHandler.a.dataFound) {
				paused = true;
				if (SFXMain != null) SFXMain.Pause();
				if (SFXOverlay != null) SFXOverlay.Pause();
			}

			return;
		}

		if (paused) {
			paused = false;
			if (SFXMain != null) SFXMain.UnPause();
			if (SFXOverlay != null) SFXOverlay.UnPause();
		}

// 		if (SFXMain.isPlaying) return;
		if (SFXMain.clip != null) {
			float remaining = (SFXMain.clip.length - SFXMain.time);
			if (remaining > Time.deltaTime && SFXMain.isPlaying) return;
		}

		if (inCombat && !inZone && combatImpulseFinished
			< PauseScript.a.relativeTime) {

			inCombat = false;
			PlayTrack(LevelManager.a.currentLevel,TrackType.Combat,
						MusicType.Override);

			combatImpulseFinished = PauseScript.a.relativeTime + 10f;
			return;
		}

		if (inZone) {
			if (distortion) {
				PlayTrack(LevelManager.a.currentLevel,TrackType.Distortion,
							MusicType.Override);
				return;
			}
			if (elevator) {
				PlayTrack(LevelManager.a.currentLevel,TrackType.Elevator,
							MusicType.Override);
				return;
			}
		}
		if (LevelManager.a != null) {
			PlayTrack(LevelManager.a.currentLevel,TrackType.Walking,
						MusicType.Walking);
		} else  PlayTrack(1, TrackType.Walking, MusicType.Walking);
    }
}
