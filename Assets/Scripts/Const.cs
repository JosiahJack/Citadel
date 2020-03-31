using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Runtime.Serialization;
using UnityStandardAssets.ImageEffects;

// Global types
public enum Handedness {Center,LH,RH};

public class Const : MonoBehaviour {
	//Item constants
	[SerializeField] public QuestBits questData;
	[SerializeField] public GameObject[] useableItems;
	[SerializeField] public Texture2D[] useableItemsFrobIcons;
    [SerializeField] public Sprite[] useableItemsIcons;
    [SerializeField] public string[] useableItemsNameText;
	[SerializeField] public Sprite[] searchItemIconSprites;
	[SerializeField] public string[] genericText;

	//Audiolog constants
	[SerializeField] public string[] audiologNames;
	[SerializeField] public string[] audiologSenders;
	[SerializeField] public string[] audiologSubjects;
	[SerializeField] public AudioClip[] audioLogs;
	[SerializeField] public int[] audioLogType;  // 0 = text only, 1 = normal, 2 = email, 3 = vmail
	[SerializeField] public string[] audioLogSpeech2Text;
	[SerializeField] public int[] audioLogLevelFound;

	//Weapon constants
	[SerializeField] public bool[] isFullAutoForWeapon;
	[SerializeField] public float[] delayBetweenShotsForWeapon;
	[SerializeField] public float[] delayBetweenShotsForWeapon2;
	[SerializeField] public float[] damagePerHitForWeapon;
	[SerializeField] public float[] damagePerHitForWeapon2;
	[SerializeField] public float[] damageOverloadForWeapon;
	[SerializeField] public float[] energyDrainLowForWeapon;
	[SerializeField] public float[] energyDrainHiForWeapon;
	[SerializeField] public float[] energyDrainOverloadForWeapon;
	[SerializeField] public float[] penetrationForWeapon;
	[SerializeField] public float[] penetrationForWeapon2;
	[SerializeField] public float[] offenseForWeapon;
	[SerializeField] public float[] offenseForWeapon2;
	[SerializeField] public float[] rangeForWeapon;
	[SerializeField] public int[] magazinePitchCountForWeapon;
	[SerializeField] public int[] magazinePitchCountForWeapon2;
	[SerializeField] public float[] recoilForWeapon;
	public enum AttackType{None,Melee,MeleeEnergy,EnergyBeam,Magnetic,Projectile,ProjectileNeedle,ProjectileEnergyBeam,ProjectileLaunched,Gas,Tranq};
	[SerializeField] public AttackType[] attackTypeForWeapon;
	public enum npcType{Mutant,Supermutant,Robot,Cyborg,Supercyborg,Cyber,MutantCyborg};
	[SerializeField] public npcType[] npcTypes;

	//NPC constants
	[SerializeField] public GameObject[] npcPrefabs;
	[SerializeField] public string[] nameForNPC;
	[SerializeField] public AttackType[] attackTypeForNPC;
	[SerializeField] public AttackType[] attackTypeForNPC2;
	[SerializeField] public AttackType[] attackTypeForNPC3;
	[SerializeField] public float[] damageForNPC; // Primary attack damage
	[SerializeField] public float[] damageForNPC2; // Secondary attack damage
	[SerializeField] public float[] damageForNPC3; // Grenade attack damage
	[SerializeField] public float[] rangeForNPC; // Primary attack range
	[SerializeField] public float[] rangeForNPC2; // Secondary attack range
	[SerializeField] public float[] rangeForNPC3; // Grenade throw range
	[SerializeField] public float[] healthForNPC;
	public enum PerceptionLevel{Low,Medium,High,Omniscient};
	[SerializeField] public PerceptionLevel[] perceptionForNPC;
	[SerializeField] public float[] disruptabilityForNPC;
	[SerializeField] public float[] armorvalueForNPC;
	[SerializeField] public float[] defenseForNPC;
	[SerializeField] public float[] randomMinimumDamageModifierForNPC; // minimum value that NPC damage can be

	[SerializeField] public HealthManager[] healthObjectsRegistration; // List of objects with health, used for fast application of damage in explosions
	public GameObject[] levelChunks;

	//System constants
	public float doubleClickTime = 0.500f;
	public float frobDistance = 5f;
	public GameObject player1;
	public GameObject player2;
	public GameObject player3;
	public GameObject player4;
	public GameObject allPlayers;
	public float playerCameraOffsetY = 0.84f; //Vertical camera offset from player 0,0,0 position (mid-body)
	public Color ssYellowText = new Color(0.8902f, 0.8745f, 0f); // Yellow, e.g. for current inventory text
	public Color ssGreenText = new Color(0.3725f, 0.6549f, 0.1686f); // Green, e.g. for inventory text

	//Patch constants
	public float berserkTime = 15.5f;
	public float detoxTime = 60f;
	public float geniusTime = 35f;
	public float mediTime = 35f;
	public float reflexTime = 155f;
	public float sightTime= 35f;
	public float sightSideEffectTime = 10f;
	public float staminupTime = 60f;
	public float reflexTimeScale = 0.25f;
	public float defaultTimeScale = 1.0f;
	public float berserkDamageMultiplier = 4.0f;

	//Grenade constants
	public float nitroMinTime = 1.0f;
	public float nitroMaxTime = 60.0f;
	public float nitroDefaultTime = 7.0f;
	public float earthShMinTime = 4.0f;
	public float earthShMaxTime = 60.0f;
	public float earthShDefaultTime = 10.0f;

	//Pool references
	public enum PoolType {None,SparqImpacts,CameraExplosions,ProjEnemShot2,SparksSmall,BloodSpurtSmall,
						BloodSpurtSmallYellow,BloodSpurtSmallGreen,SparksSmallBlue,HopperImpact,
						GrenadeFragExplosions,Vaporize, BlasterImpacts, IonImpacts,
						MagpulseShots, MagpulseImpacts,StungunShots, StungunImpacts, RailgunShots, RailgunImpacts,
						PlasmaShots, PlasmaImpacts, ProjEnemShot6,ProjEnemShot6Impacts, ProjEnemShot2Impacts,
						ProjSeedPods, ProjSeedPodsImpacts, TempAudioSources,GrenadeEMPExplosions, ProjEnemShot4,
						ProjEnemShot4Impacts,CrateExplosions,GrenadeFragLive,CyborgAssassinThrowingStars,
						ConcussionLive,EMPLive,GasLive,GasExplosions,CorpseHit,NPCMagpulseShots,NPCRailgunShots};
	public GameObject Pool_SparqImpacts;
	public GameObject Pool_CameraExplosions;
	public GameObject Pool_ProjectilesEnemShot2;
	public GameObject Pool_BloodSpurtSmall;
	public GameObject Pool_SparksSmall;
	public GameObject Pool_SparksSmallBlue;
	public GameObject Pool_BloodSpurtSmallYellow;
	public GameObject Pool_BloodSpurtSmallGreen;
	public GameObject Pool_HopperImpact;
	public GameObject Pool_GrenadeFragExplosions;
    public GameObject Pool_Vaporize;
    public GameObject Pool_BlasterImpacts;
    public GameObject Pool_IonImpacts;
    public GameObject Pool_MagpulseShots;
    public GameObject Pool_MagpulseImpacts;
    public GameObject Pool_StungunShots;
    public GameObject Pool_StungunImpacts;
    public GameObject Pool_RailgunShots;
    public GameObject Pool_RailgunImpacts;
    public GameObject Pool_PlasmaShots;
    public GameObject Pool_PlasmaImpacts;
	public GameObject Pool_ProjEnemShot6;
	public GameObject Pool_ProjEnemShot6Impacts;
	public GameObject Pool_ProjEnemShot2Impacts;
	public GameObject Pool_ProjSeedPods;
	public GameObject Pool_ProjSeedPodsImpacts;
	public GameObject Pool_TempAudioSources;
	public GameObject Pool_GrenadeEMPExplosions;
	public GameObject Pool_ProjEnemShot4;
	public GameObject Pool_ProjEnemShot4Impacts;
	public GameObject Pool_CrateExplosions;
	public GameObject Pool_GrenadeFragLive;
	public GameObject Pool_CyborgAssassinThrowingStars;
	public GameObject Pool_ConcussionLive;
	public GameObject Pool_EMPLive;
	public GameObject Pool_GasLive;
	public GameObject Pool_GasExplosions;
	public GameObject Pool_CorpseHit;
	public GameObject Pool_NPCMagpulseShots;
	public GameObject Pool_NPCRailgunShots;

	//Global object references
	public GameObject statusBar;
   
	//Config constants
	public int difficultyCombat;
	public int difficultyMission;
	public int difficultyPuzzle;
	public int difficultyCyber;
	public string playerName;
	public AudioSource mainmenuMusic;
	public int GraphicsResWidth;
	public int GraphicsResHeight;
	public bool GraphicsFullscreen;
	public bool GraphicsSSAO;
	public bool GraphicsBloom;
	public int GraphicsFOV;
	public int GraphicsGamma;
	public int AudioSpeakerMode;
	public bool AudioReverb;
	public int AudioVolumeMaster;
	public int AudioVolumeMusic;
	public int AudioVolumeMessage;
	public int AudioVolumeEffects;
	public int AudioLanguage;
	public bool AudioSubtitles;
	public int[] InputCodeSettings;
	public string[] InputCodes;
	public string[] InputValues;
	public string[] InputConfigNames;
	public bool InputInvertLook;
	public bool InputInvertCyberspaceLook;
	public bool InputInvertInventoryCycling;
	public bool InputQuickItemPickup;
	public bool InputQuickReloadWeapons;
	public enum aiState{Idle,Walk,Run,Attack1,Attack2,Attack3,Pain,Dying,Dead,Inspect,Interacting};
    public enum aiMoveType {Walk,Fly,Swim,Cyber,None};
    public Font mainFont1;
	public Font mainFont2;
	public GameObject[] TargetRegister; // doesn't need to be full, available space for maps and mods made by the community to use tons of objects
	public string[] TargetnameRegister;
	public float globalShakeDistance;
	public float globalShakeForce;
    public string[] stringTable;
	public float[] reloadTime;
	public CyberWall[] cyberpanelsRegistry;

	//Instance container variable
	public static Const a;

	// Private CONSTANTS
	private int TARGET_FPS = 60;

	// Instantiate it so that it can be accessed globally. MOST IMPORTANT PART!!
	// =========================================================================
	void Awake() {
		Application.targetFrameRate = TARGET_FPS;
		a = this;
		//for (int i=0;i<Display.displays.Length;i++) {
		//	Display.displays[i].Activate();
		//}
		FindPlayers();
		LoadTextForLanguage(0); //initialize with US English (index 0)
	}
	// =========================================================================

	private void FindPlayers() {
		List<GameObject> playerGameObjects = new List<GameObject>();

		// Find all gameobjects with SaveObject script attached
		GameObject[] getAllGameObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
		foreach (GameObject gob in getAllGameObjects) {
			if (gob.GetComponentInChildren<PlayerReferenceManager>() != null && gob.GetComponent<RuntimeObject>().isRuntimeObject == true) playerGameObjects.Add(gob);
		}

		if (playerGameObjects.Count > 0) player1 = playerGameObjects[0];
		if (playerGameObjects.Count > 1) player2 = playerGameObjects[1];
		if (playerGameObjects.Count > 2) player3 = playerGameObjects[2];
		if (playerGameObjects.Count > 3) player4 = playerGameObjects[3];
		allPlayers = new GameObject();
		allPlayers.name = "All Players"; // for use in self printing Sprint() function for sending messages to HUD on all players
	}

    public void LoadTextForLanguage(int lang) {
        string readline; // variable to hold each string read in from the file
        int currentline = 0;
        string sourceFile = "/StreamingAssets/text_english.txt";

        // UPKEEP: support other languages
        switch (lang) {
            case 0:
                sourceFile = "/StreamingAssets/text_english.txt";
                break;
            case 1:
                sourceFile = "/StreamingAssets/text_espanol.txt";
                break;
            //case 2:
                //sourceFile = "/StreamingAssets/text_francois.txt";
                //break;
            default:
                sourceFile = "/StreamingAssets/text_english.txt";
                break;
        }
        StreamReader dataReader = new StreamReader(Application.dataPath + sourceFile, Encoding.Default);
        using (dataReader) {
            do {
                // Read the next line
                readline = dataReader.ReadLine();
                if (currentline < stringTable.Length) {
                    stringTable[currentline] = readline;
				} else {
					Debug.Log("WARNING: Ran out of slots in stringTable, didn't finish reading all text from text_<language>.txt");
					dataReader.Close();
					return;
				}
                currentline++;
            } while (!dataReader.EndOfStream);
            dataReader.Close();
            return;
        }
    }

	void Start() {
		LoadConfig();
		LoadAudioLogMetaData();
		LoadItemNamesData();
		LoadDamageTablesData();
		LoadEnemyTablesData();
		questData = new QuestBits ();
		if (mainFont1 != null) mainFont1.material.mainTexture.filterMode = FilterMode.Point;
		if (mainFont2 != null) mainFont2.material.mainTexture.filterMode = FilterMode.Point;
	}

	private void LoadConfig() {
		// Graphics Configurations
		GraphicsResWidth = AssignConfigInt("Graphics","ResolutionWidth");
		GraphicsResHeight = AssignConfigInt("Graphics","ResolutionHeight");
		GraphicsFullscreen = AssignConfigBool("Graphics","Fullscreen");
		GraphicsSSAO = AssignConfigBool("Graphics","SSAO");
		GraphicsBloom = AssignConfigBool("Graphics","Bloom");
		GraphicsFOV = AssignConfigInt("Graphics","FOV");
		GraphicsGamma = AssignConfigInt("Graphics","Gamma");

		// Audio Configurations
		AudioSpeakerMode = AssignConfigInt("Audio","SpeakerMode");
		AudioReverb = AssignConfigBool("Audio","Reverb");
		AudioVolumeMaster = AssignConfigInt("Audio","VolumeMaster");
		AudioVolumeMusic = AssignConfigInt("Audio","VolumeMusic");
		AudioVolumeMessage = AssignConfigInt("Audio","VolumeMessage");
		AudioVolumeEffects = AssignConfigInt("Audio","VolumeEffects");
		AudioLanguage = AssignConfigInt("Audio","Language");  // defaults to 0 = english
		AudioSubtitles = AssignConfigBool("Audio","Subtitles");

		// Input Configurations
		for (int i=0;i<40;i++) {
			string inputCapture = INIWorker.IniReadValue("Input",InputCodes[i]);
			for (int j=0;j<159;j++) {
				if (InputValues[j] == inputCapture) {
					InputCodeSettings[i] = j;
				}
			}
		}
		InputInvertLook = AssignConfigBool("Input","InvertLook");
		InputInvertCyberspaceLook = AssignConfigBool("Input","InvertCyberspaceLook");
		InputInvertInventoryCycling = AssignConfigBool("Input","InvertInventoryCycling");
		InputQuickItemPickup = AssignConfigBool("Input","QuickItemPickup");
		InputQuickReloadWeapons = AssignConfigBool("Input","QuickReloadWeapons");

		SetVolume();
		Screen.SetResolution(GraphicsResWidth,GraphicsResHeight,true);
		Screen.fullScreen = Const.a.GraphicsFullscreen;
	}

	public void WriteConfig() {
		INIWorker.IniWriteValue("Graphics","ResolutionWidth",GraphicsResWidth.ToString());
		INIWorker.IniWriteValue("Graphics","ResolutionHeight",GraphicsResHeight.ToString());
		INIWorker.IniWriteValue("Graphics","Fullscreen",GetBoolAsString(GraphicsFullscreen));
		INIWorker.IniWriteValue("Graphics","SSAO",GetBoolAsString(GraphicsSSAO));
		INIWorker.IniWriteValue("Graphics","Bloom",GetBoolAsString(GraphicsBloom));
		INIWorker.IniWriteValue("Graphics","FOV",GraphicsFOV.ToString());
		INIWorker.IniWriteValue("Graphics","Gamma",GraphicsGamma.ToString());
		INIWorker.IniWriteValue("Audio","SpeakerMode",AudioSpeakerMode.ToString());
		INIWorker.IniWriteValue("Audio","Reverb",GetBoolAsString(AudioReverb));
		INIWorker.IniWriteValue("Audio","VolumeMaster",AudioVolumeMaster.ToString());
		INIWorker.IniWriteValue("Audio","VolumeMusic",AudioVolumeMusic.ToString());
		INIWorker.IniWriteValue("Audio","VolumeMessage",AudioVolumeMessage.ToString());
		INIWorker.IniWriteValue("Audio","VolumeEffects",AudioVolumeEffects.ToString());
		INIWorker.IniWriteValue("Audio","Language",AudioLanguage.ToString());
		INIWorker.IniWriteValue("Audio","Subtitles",GetBoolAsString(AudioSubtitles));
		for (int i=0;i<40;i++) {
			INIWorker.IniWriteValue("Input",InputCodes[i],InputValues[InputCodeSettings[i]]);
		}
		INIWorker.IniWriteValue("Input","InvertLook",GetBoolAsString(InputInvertLook));
		INIWorker.IniWriteValue("Input","InvertCyberspaceLook",GetBoolAsString(InputInvertCyberspaceLook));
		INIWorker.IniWriteValue("Input","InvertInventoryCycling",GetBoolAsString(InputInvertInventoryCycling));
		INIWorker.IniWriteValue("Input","QuickItemPickup",GetBoolAsString(InputQuickItemPickup));
		INIWorker.IniWriteValue("Input","QuickReloadWeapons",GetBoolAsString(InputQuickReloadWeapons));
		SetBloom();
		SetSSAO();
	}

	private void LoadAudioLogMetaData () {
		// The following to be assigned to the arrays in the Unity Const data structure
		int readIndexOfLog; // look-up index for assigning the following data on the line in the file to the arrays
		string readLogText; // loaded into string audioLogSpeech2Text[]

		string readline; // variable to hold each string read in from the file
		int currentline = 0;

		StreamReader dataReader = new StreamReader(Application.dataPath + "/StreamingAssets/logs_text.txt",Encoding.Default);
		using (dataReader) {
			do {
				int i = 0;
				// Read the next line
				readline = dataReader.ReadLine();
				if (readline == null) continue; // just in case
				//char[] delimiters = new char[] {','};
				string[] entries = readline.Split(',');
				readIndexOfLog = GetIntFromString(entries[i],currentline,"logs_text",i); i++;
				audiologNames[readIndexOfLog] = entries[i]; i++;
				audiologSenders[readIndexOfLog] = entries[i]; i++;
				audiologSubjects[readIndexOfLog] = entries[i]; i++;
				audioLogType[readIndexOfLog] = GetIntFromString(entries[i],currentline,"logs_text",i); i++;
				audioLogLevelFound[readIndexOfLog] = GetIntFromString(entries[i],currentline,"logs_text",i); i++;
				readLogText = entries[i]; i++;
				// handle extra commas within the body text and append remaining portions of the line
				if (entries.Length > 7) {
					for (int j=7;j<entries.Length;j++) {
						//Debug.Log("Combining remaining comma'ed sections of log text: " + j.ToString());
						readLogText = (readLogText +"," + entries[j]);  // combine remaining portions of text after other commas and add comma back
					}
				}
				audioLogSpeech2Text[readIndexOfLog] = readLogText;
				currentline++;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}

	private void LoadItemNamesData () {
		string readline; // variable to hold each string read in from the file
		int currentline = 0;

		// Default to English
		StreamReader dataReader = new StreamReader(Application.dataPath + "/StreamingAssets/item_names.txt",Encoding.Default);
		using (dataReader) {
			do {
				// Read the next line
				readline = dataReader.ReadLine();
				if (readline == null) break; // just in case
				useableItemsNameText[currentline] = readline;
				currentline++;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}

	private void LoadDamageTablesData () {
		string readline; // variable to hold each string read in from the file
		int currentline = 0;
		int readInt = 0;

		StreamReader dataReader = new StreamReader(Application.dataPath + "/StreamingAssets/damage_tables.txt",Encoding.Default);
		using (dataReader) {
			do {
				int i = 0;
				// Read the next line
				readline = dataReader.ReadLine();
				//char[] delimiters = new char[] {','};
				string[] entries = readline.Split(',');
                //isFullAutoForWeapon[currentline] = GetBoolFromString(entries[i]); i++;
                isFullAutoForWeapon[currentline] = true; i++; // better this way
				delayBetweenShotsForWeapon[currentline] = GetFloatFromString(entries[i],currentline); i++;
				delayBetweenShotsForWeapon2[currentline] = GetFloatFromString(entries[i],currentline); i++;
				damagePerHitForWeapon[currentline] = GetFloatFromString(entries[i],currentline); i++;
				damagePerHitForWeapon2[currentline] = GetFloatFromString(entries[i],currentline); i++;
				damageOverloadForWeapon[currentline] = GetFloatFromString(entries[i],currentline); i++;
				energyDrainLowForWeapon[currentline] = GetFloatFromString(entries[i],currentline); i++;
				energyDrainHiForWeapon[currentline] = GetFloatFromString(entries[i],currentline); i++;
				energyDrainOverloadForWeapon[currentline] = GetFloatFromString(entries[i],currentline); i++;
				penetrationForWeapon[currentline] = GetFloatFromString(entries[i],currentline); i++;
				penetrationForWeapon2[currentline] = GetFloatFromString(entries[i],currentline); i++;
				offenseForWeapon[currentline] = GetFloatFromString(entries[i],currentline); i++;
				offenseForWeapon2[currentline] = GetFloatFromString(entries[i],currentline); i++;
				rangeForWeapon[currentline] = GetFloatFromString(entries[i],currentline); i++;
				readInt = GetIntFromString(entries[i],currentline,"damage_tables",i); i++;
				attackTypeForWeapon[currentline] = GetAttackTypeFromInt(readInt);
				currentline++;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}

	private void LoadEnemyTablesData() {
		//case 1: return AttackType.Melee;
		//case 2: return AttackType.EnergyBeam;
		//case 3: return AttackType.Magnetic;
		//case 4: return AttackType.Projectile;
		//case 5: return AttackType.ProjectileEnergyBeam;

		//case 1: return PerceptionLevel.Medium;
		//case 2: return PerceptionLevel.High;
		//case 3: return PerceptionLevel.Omniscient;

		string readline; // variable to hold each string read in from the file
		int currentline = 0;
		int readInt = 0;
		StreamReader dataReader = new StreamReader(Application.dataPath + "/StreamingAssets/enemy_tables.txt",Encoding.Default);
		using (dataReader) {
			do {
				int i = 0;
				// Read the next line
				readline = dataReader.ReadLine();
				string[] entries = readline.Split(',');
				char[] commentCheck = entries[i].ToCharArray();
				if (commentCheck[0] == '/' && commentCheck[1] == '/') {
					currentline++;
					continue; // Skip lines that start with '//'
				}
				nameForNPC[currentline] = entries[i]; i++;
				readInt = GetIntFromString(entries[i],currentline,"enemy_tables",i); i++; attackTypeForNPC[currentline] = GetAttackTypeFromInt(readInt);
				readInt = GetIntFromString(entries[i],currentline,"enemy_tables",i); i++; attackTypeForNPC2[currentline] = GetAttackTypeFromInt(readInt);
				readInt = GetIntFromString(entries[i],currentline,"enemy_tables",i); i++; attackTypeForNPC3[currentline] = GetAttackTypeFromInt(readInt);
				damageForNPC[currentline] = GetFloatFromString(entries[i],currentline); i++;
				damageForNPC2[currentline] = GetFloatFromString(entries[i],currentline); i++;
				damageForNPC3[currentline] = GetFloatFromString(entries[i],currentline); i++;
				rangeForNPC[currentline] = GetFloatFromString(entries[i],currentline); i++;
				rangeForNPC2[currentline] = GetFloatFromString(entries[i],currentline); i++;
				rangeForNPC3[currentline] = GetFloatFromString(entries[i],currentline); i++;
				healthForNPC[currentline] = GetFloatFromString(entries[i],currentline); i++;
				readInt = GetIntFromString(entries[i],currentline,"enemy_tables",i); i++;
				perceptionForNPC[currentline] = GetPerceptionLevelFromInt(readInt);
				disruptabilityForNPC[currentline] = GetFloatFromString(entries[i],currentline); i++;
				armorvalueForNPC[currentline] = GetFloatFromString(entries[i],currentline); i++;
				defenseForNPC[currentline] = GetFloatFromString(entries[i],currentline); i++;
				randomMinimumDamageModifierForNPC[currentline] = GetFloatFromString(entries[i],currentline);
				currentline++;
				if (currentline > 23) break;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}

	// StatusBar Print
	public static void sprint (string input, GameObject player) {
		Debug.Log(input);  // print to console
		if (player == null) return;
		if (a != null) {
			if (player.name == "All Players") {
				if (a.player1 != null) a.player1.GetComponent<PlayerReferenceManager>().playerStatusBar.GetComponent<StatusBarTextDecay>().SendText(input);
				if (a.player2 != null) a.player2.GetComponent<PlayerReferenceManager>().playerStatusBar.GetComponent<StatusBarTextDecay>().SendText(input);
				if (a.player3 != null) a.player3.GetComponent<PlayerReferenceManager>().playerStatusBar.GetComponent<StatusBarTextDecay>().SendText(input);
				if (a.player4 != null) a.player4.GetComponent<PlayerReferenceManager>().playerStatusBar.GetComponent<StatusBarTextDecay>().SendText(input);
			} else {
				PlayerReferenceManager prm = player.GetComponent<PlayerReferenceManager>();
				if (prm != null) {
					prm.playerStatusBar.GetComponent<StatusBarTextDecay>().SendText(input);
				} else {
					if (a.player1 != null) {
						if (player.transform.IsChildOf(a.player1.transform)) a.player1.GetComponent<PlayerReferenceManager>().playerStatusBar.GetComponent<StatusBarTextDecay>().SendText(input);
					}
					if (a.player2 != null) {
						if (player.transform.IsChildOf(a.player1.transform)) a.player2.GetComponent<PlayerReferenceManager>().playerStatusBar.GetComponent<StatusBarTextDecay>().SendText(input);
					}
					if (a.player3 != null) {
						if (player.transform.IsChildOf(a.player1.transform)) a.player3.GetComponent<PlayerReferenceManager>().playerStatusBar.GetComponent<StatusBarTextDecay>().SendText(input);
					}
					if (a.player4 != null) {
						if (player.transform.IsChildOf(a.player1.transform)) a.player4.GetComponent<PlayerReferenceManager>().playerStatusBar.GetComponent<StatusBarTextDecay>().SendText(input);
					}
				}
			}
		}
	}

	public GameObject GetObjectFromPool(PoolType pool) {
		if (pool == PoolType.None) return null; //do nothing, no pool requested

		GameObject poolContainer = Pool_SparksSmall;
		string poolName = " ";

		switch (pool) {
		case PoolType.SparksSmall: 
			poolContainer = Pool_SparksSmall;
			poolName = "SparksSmall ";
			break;
		case PoolType.SparqImpacts:
			poolContainer = Pool_SparqImpacts;
			poolName = "SparqImpacts ";
			break;
		case PoolType.CameraExplosions:
			poolContainer = Pool_CameraExplosions;
			poolName = "CameraExplosions ";
			break;
		case PoolType.ProjEnemShot2:
			poolContainer = Pool_ProjectilesEnemShot2;
			poolName = "ProjectilesEnemShot2 ";
			break;
		case PoolType.BloodSpurtSmall: 
			poolContainer = Pool_BloodSpurtSmall;
			poolName = "BloodSpurtSmall ";
			break;
		case PoolType.BloodSpurtSmallYellow: 
			poolContainer = Pool_BloodSpurtSmallYellow;
			poolName = "BloodSpurtSmallYellow ";
			break;
		case PoolType.BloodSpurtSmallGreen: 
			poolContainer = Pool_BloodSpurtSmallGreen;
			poolName = "BloodSpurtSmallGreen ";
			break;
		case PoolType.SparksSmallBlue: 
			poolContainer = Pool_SparksSmallBlue;
			poolName = "BloodSpurtSmall ";
			break;
		case PoolType.HopperImpact: 
			poolContainer = Pool_HopperImpact;
			poolName = "HopperImpact ";
			break;
		case PoolType.GrenadeFragExplosions: 
			poolContainer = Pool_GrenadeFragExplosions;
			poolName = "GrenadeFragExplosions ";
			break;
        case PoolType.Vaporize:
            poolContainer = Pool_Vaporize;
            poolName = "Vaporize ";
            break;
        case PoolType.BlasterImpacts:
            poolContainer = Pool_BlasterImpacts;
            poolName = "BlasterImpacts ";
            break;
        case PoolType.IonImpacts:
            poolContainer = Pool_IonImpacts;
            poolName = "IonImpacts ";
            break;
        case PoolType.MagpulseShots:
            poolContainer = Pool_MagpulseShots;
            poolName = "MagpulseShots ";
            break;
        case PoolType.MagpulseImpacts:
            poolContainer = Pool_MagpulseImpacts;
            poolName = "MagpulseImpacts ";
            break;
        case PoolType.StungunShots:
            poolContainer = Pool_StungunShots;
            poolName = "StungunShots ";
            break;
        case PoolType.StungunImpacts:
            poolContainer = Pool_StungunImpacts;
            poolName = "StungunImpacts ";
            break;
        case PoolType.RailgunShots:
            poolContainer = Pool_RailgunShots;
            poolName = "RailgunShots ";
            break;
        case PoolType.RailgunImpacts:
            poolContainer = Pool_RailgunImpacts;
            poolName = "RailgunImpacts ";
            break;
        case PoolType.PlasmaShots:
            poolContainer = Pool_PlasmaShots;
            poolName = "PlasmaShots ";
            break;
        case PoolType.PlasmaImpacts:
            poolContainer = Pool_PlasmaImpacts;
            poolName = "PlasmaImpacts ";
            break;
        case PoolType.ProjEnemShot6:
            poolContainer = Pool_ProjEnemShot6;
            poolName = "ProjEnemShot6 ";
            break;
		case PoolType.ProjEnemShot6Impacts:
            poolContainer = Pool_ProjEnemShot6Impacts;
            poolName = "ProjEnemShot6Impacts ";
            break;
		case PoolType.ProjEnemShot2Impacts:
            poolContainer = Pool_ProjEnemShot2Impacts;
            poolName = "ProjEnemShot2Impacts ";
            break;
		case PoolType.ProjSeedPods:
            poolContainer = Pool_ProjSeedPods;
            poolName = "ProjSeedPods ";
            break;
		case PoolType.ProjSeedPodsImpacts:
            poolContainer = Pool_ProjSeedPodsImpacts;
            poolName = "ProjSeedPodsImpacts ";
            break;
		case PoolType.TempAudioSources:
            poolContainer = Pool_TempAudioSources;
            poolName = "TempAudioSources ";
            break;
		case PoolType.GrenadeEMPExplosions:
            poolContainer = Pool_GrenadeEMPExplosions;
            poolName = "GrenadeEMPExplosions ";
            break;
		case PoolType.ProjEnemShot4:
            poolContainer = Pool_ProjEnemShot4;
            poolName = "ProjEnemShot4 ";
            break;
		case PoolType.ProjEnemShot4Impacts:
            poolContainer = Pool_ProjEnemShot4Impacts;
            poolName = "ProjEnemShot4Impacts ";
            break;
		case PoolType.CrateExplosions:
            poolContainer = Pool_CrateExplosions;
            poolName = "CrateExplosions ";
            break;
		case PoolType.GrenadeFragLive:
            poolContainer = Pool_GrenadeFragLive;
            poolName = "GrenadeFragLive ";
            break;
		case PoolType.CyborgAssassinThrowingStars:
            poolContainer = Pool_CyborgAssassinThrowingStars;
            poolName = "CyborgAssassinThrowingStars ";
            break;
		case PoolType.ConcussionLive:
            poolContainer = Pool_ConcussionLive;
            poolName = "ConcussionLive ";
            break;
		case PoolType.EMPLive:
            poolContainer = Pool_EMPLive;
            poolName = "EMPLive ";
            break;
		case PoolType.GasLive:
            poolContainer = Pool_GasLive;
            poolName = "GasLive ";
            break;
		case PoolType.GasExplosions:
            poolContainer = Pool_GasExplosions;
            poolName = "GasExplosions ";
            break;
		case PoolType.CorpseHit:
            poolContainer = Pool_CorpseHit;
            poolName = "CorpseHit ";
            break;
		case PoolType.NPCMagpulseShots:
			poolContainer = Pool_NPCMagpulseShots;
			poolName = "NPCMagpulseShots ";
			break;
		case PoolType.NPCRailgunShots:
			poolContainer = Pool_NPCRailgunShots;
			poolName = "NPCRailgunShots ";
			break;
        }

		if (poolContainer == null) {
			Debug.Log("Cannot find " + poolName + "pool",allPlayers);
			return null;
		}

		for (int i=0;i<poolContainer.transform.childCount;i++) {
			Transform child = poolContainer.transform.GetChild(i);
			if (child.gameObject.activeInHierarchy == false) {
				return child.gameObject;
			}
		}

		return null;
	}

	// Give it a pool for a projectile, get the impact effect pool
	public PoolType GetPoolImpactFromPoolProjectileType(PoolType pool) {
		if (pool == PoolType.None) return PoolType.None; //do nothing, no impact effect for no object

		switch (pool) {
		case PoolType.ProjEnemShot2: return PoolType.ProjEnemShot2Impacts;
        case PoolType.MagpulseShots: return PoolType.MagpulseImpacts;
        case PoolType.StungunShots: return PoolType.StungunImpacts;
        case PoolType.RailgunShots: return PoolType.RailgunImpacts;
        case PoolType.PlasmaShots: return PoolType.PlasmaImpacts;
        case PoolType.ProjEnemShot6: return PoolType.ProjEnemShot6Impacts;
		case PoolType.ProjSeedPods: return PoolType.ProjSeedPodsImpacts;
        }

		return PoolType.None;
	}

	// ========================DAMAGE SYSTEM===========================
	// 0. First checks against whether the entity is damageable (i.e. not the world)
	// 1. Armor absorption (see ICE Breaker Guide for all of 4 these)
	// 2. Weapon vulnerabilities based on attack type and the a_att_type bits stored in the npc
	// 3. Critical hits, chance for critical hit damage based on defense and offense of attack and target
	// 4. Random Factor, +/- 10% damage for randomness
	// 5. Apply velocity for damage, this is after all the above because otherwise the damage multipliers wouldn't affect velocity
	// 6. Return the damage to original TakeDamage() function

	public float GetDamageTakeAmount (DamageData dd) {
		// a_* = attacker
		float a_damage = dd.damage;
		float a_offense = dd.offense;
		float a_penetration = dd.penetration;
		AttackType a_att_type = dd.attackType;
		bool a_berserk = dd.berserkActive;

		// o_* = other (one being attacked)
		bool o_isnpc = dd.isOtherNPC;
		float o_armorvalue = dd.armorvalue;
		float o_defense = dd.defense;

		float take = 0f;
		float chance = 0f;
		float f = 0f;
		// 1. Armor Absorption
		if (o_armorvalue > a_penetration) {
			take = (a_damage - a_penetration);
		} else {
			take = a_damage;
		}

		// 2. Weapon Vulnerabilities
		if (a_att_type != AttackType.None) {

		}

		// 3. Critical Hits (NPCs only)
		if (o_isnpc) {
			f = (a_offense - o_defense);
			float crit = f;
			if (f > 0) {
				// 71% success with 5/6  5 = f, 6 = max offense or defense value
				// 62% success with 4/6
				// 50% success with 3/6
				// 24% success with 2/6
				// 10% success with 1/6
				chance = (f/6); // 5/6|4/6|3/6|2/6|1/6 = .833|.666|.5|.333|.166
				if (f == 1)
					chance = 0.280f; //anything less than 0.25, 0.1666 in this case taken from 1/6, will fail

				chance = (chance * UnityEngine.Random.Range(0f,1f) * 2);
				if (chance > 0.5f) {
					// SUCCESS! Applying critical hit.
					crit = (take * f);  //How many extra damages we add to damage that we will take
					take = (take + crit); // Maximum extra is 5X + 1X Damage
				}
			}
		}

		// 4. Random Factor +/- 10% (aka 0.10 damage)
		chance = (0.1f * UnityEngine.Random.Range(0f,1f));

		// 50% chance of being positive or negative
		f = UnityEngine.Random.Range(0f,1f);
		if (f > 0.5f) {
			chance = (chance * (-1)); // Make negative
		}
		chance = (chance * take);
		take = (take + chance); // Add the random factor, anywhere up to +/- 10%
		if (take <= 0f)
			return take;

		// 5. Apply Velocity for Damage Amount
		// Done in HealthManager TakeDamage()

		// 6. Specialties
		if (a_berserk) take *= Const.a.berserkDamageMultiplier;

		// 6. Return the Damage 
		return take;
	}

	public static void drawDebugLine(Vector3 start , Vector3 end, Color color,float duration = 0.2f){
		GameObject myLine = new GameObject ();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer> ();
		LineRenderer lr = myLine.GetComponent<LineRenderer> ();
		lr.material = new Material (Shader.Find ("Particles/Additive"));
		lr.startColor = color;
		lr.endColor = color;
		lr.startWidth = 0.1f;
		lr.endWidth = 0.1f;
		lr.SetPosition (0, start);
		lr.SetPosition (1, end);
		Destroy(myLine,duration);
	}


	// Save the Game
	// ============================================================================
	public void Save(int saveFileIndex,string savename) {
		string[] saveData = new string[4096];
		string line;
		int i,j;
		int index = 0;
		Transform tr, trml;
		List<GameObject> playerGameObjects = new List<GameObject>();
		List<GameObject> saveableGameObjects = new List<GameObject>();

		// Indicate we are saving "Saving..."
		sprint(stringTable[194],allPlayers);

		// Header
		// -----------------------------------------------------
		// Save Name
		if (string.IsNullOrWhiteSpace(savename)) savename = "Unnamed " + saveFileIndex.ToString();
		saveData[index] = savename;
		index++;

		// temp string to hold global states
		//string states = "00000000|00000000|";
		string states = (questData.lev1SecCode.ToString() + questData.lev2SecCode.ToString() + questData.lev3SecCode.ToString() +
						questData.lev4SecCode.ToString() + questData.lev5SecCode.ToString() + questData.lev6SecCode.ToString() +
						questData.RobotSpawnDeactivated.ToString() + questData.IsotopeInstalled.ToString() + questData.ShieldActivated.ToString() +
						questData.LaserSafetyOverriden.ToString() + questData.LaserDestroyed.ToString() + questData.BetaGroveCyberUnlocked.ToString() +
						questData.GroveAlphaJettisonEnabled.ToString() + questData.GroveBetaJettisonEnabled.ToString() +
						questData.GroveDeltaJettisonEnabled.ToString() + questData.MasterJettisonBroken.ToString() + 
						questData.Relay428Fixed.ToString() + questData.MasterJettisonEnabled.ToString() + questData.BetaGroveJettisoned.ToString() +
						questData.AntennaNorthDestroyed.ToString() + questData.AntennaSouthDestroyed.ToString() + 
						questData.AntennaEastDestroyed.ToString() + questData.AntennaWestDestroyed.ToString() + questData.SelfDestructActivated.ToString() +
						questData.BridgeSeparated.ToString() + questData.IsolinearChipsetInstalled.ToString() + "|");

		// Global states and Difficulties
		saveData[index] = (LevelManager.a.currentLevel.ToString() + "|" + states + difficultyCombat.ToString() + "|" + difficultyMission.ToString() + "|" + difficultyPuzzle.ToString() + "|" + difficultyCyber.ToString());
		index++;

		// Find all gameobjects with SaveObject script attached
		GameObject[] getAllGameObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
		foreach (GameObject gob in getAllGameObjects) {
			if (gob.GetComponentInChildren<PlayerReferenceManager>() != null && gob.GetComponent<RuntimeObject>().isRuntimeObject == true) playerGameObjects.Add(gob);
			if (gob.GetComponent<SaveObject>() != null && gob.GetComponent<RuntimeObject>().isRuntimeObject == true) saveableGameObjects.Add(gob);
		}

		Debug.Log("Num players: " + playerGameObjects.Count.ToString(),allPlayers);

		// Save all the players' data
		for (i=0;i<playerGameObjects.Count;i++) {
			// First get all references to relevant componenets on all relevant gameobjects in the player gameobject
			PlayerReferenceManager PRman = playerGameObjects[i].GetComponent<PlayerReferenceManager>();
			GameObject pCap = PRman.playerCapsule;
			GameObject playerMainCamera = PRman.playerCapsuleMainCamera;
			GameObject playerInventory = PRman.playerInventory;
			PlayerHealth ph = pCap.GetComponent<PlayerHealth>();
			PlayerEnergy pe = pCap.GetComponent<PlayerEnergy>();
			PlayerMovement pm = pCap.GetComponent<PlayerMovement>();
			PlayerPatch pp = pCap.GetComponent<PlayerPatch>();
			tr = pCap.transform;
			MouseLookScript ml = playerMainCamera.GetComponent<MouseLookScript>();
			trml = playerMainCamera.transform;
			WeaponInventory wi = playerInventory.GetComponent<WeaponInventory>();
			WeaponAmmo wa = playerInventory.GetComponent<WeaponAmmo>();
			WeaponCurrent wc = playerInventory.GetComponent<WeaponCurrent>();
			GrenadeCurrent gc = playerInventory.GetComponent<GrenadeCurrent>();
			GrenadeInventory gi = playerInventory.GetComponent<GrenadeInventory>();
			PatchCurrent pc = playerInventory.GetComponent<PatchCurrent>();
			PatchInventory pi = playerInventory.GetComponent<PatchInventory>();
			LogInventory li = playerInventory.GetComponent<LogInventory>();
			HardwareInventory hi = playerInventory.GetComponent<HardwareInventory>();

			line = playerGameObjects[i].GetInstanceID().ToString();
			line += "|" + Const.a.playerName;
			line += "|" + ph.hm.health.ToString("0000.00000");
			line += "|" + pe.energy.ToString("0000.00000");
			line += "|" + pm.currentCrouchRatio.ToString("0000.00000");
			line += "|" + pm.bodyState.ToString();
			line += "|" + pm.ladderState.ToString();
			line += "|" + pm.gravliftState.ToString();
			line += "|" + pp.patchActive.ToString();
			line += "|" + pp.berserkIncrement.ToString();
			line += "|" + pp.sightFinishedTime.ToString("0000.00000");
			line += "|" + pp.staminupFinishedTime.ToString("0000.00000");
			line += "|" + (tr.localPosition.x.ToString("0000.00000") + "|" + tr.localPosition.y.ToString("0000.00000") + "|" + tr.localPosition.z.ToString("0000.00000"));
			line += "|" + (tr.localRotation.x.ToString("0000.00000") + "|" + tr.localRotation.y.ToString("0000.00000") + "|" + tr.localRotation.z.ToString("0000.00000") + "|" + tr.localRotation.w.ToString("0000.00000"));
			line += "|" + (tr.localScale.x.ToString("0000.00000") + "|" + tr.localScale.y.ToString("0000.00000") + "|" + tr.localScale.z.ToString("0000.00000"));
			line += "|" + (trml.localPosition.x.ToString("0000.00000") + "|" + trml.localPosition.y.ToString("0000.00000") + "|" + trml.localPosition.z.ToString("0000.00000"));
			line += "|" + (trml.localRotation.x.ToString("0000.00000") + "|" + trml.localRotation.y.ToString("0000.00000") + "|" + trml.localRotation.z.ToString("0000.00000") + "|" + trml.localRotation.w.ToString("0000.00000"));
			line += "|" + (trml.localScale.x.ToString("0000.00000") + "|" + trml.localScale.y.ToString("0000.00000") + "|" + trml.localScale.z.ToString("0000.00000"));
			line += "|" + ml.inventoryMode.ToString();
			line += "|" + ml.holdingObject.ToString();
			line += "|" + ml.heldObjectIndex.ToString();
			line += "|" + ml.heldObjectCustomIndex.ToString();
			line += "|" + GUIState.a.overButtonType.ToString();
			line += "|" + GUIState.a.overButton.ToString();
			line += "|" + ml.geniusActive.ToString();
			for (j=0;j<7;j++) { line += "|" + wi.weaponInventoryIndices[j].ToString(); }
			for (j=0;j<7;j++) { line += "|" + wi.weaponInventoryAmmoIndices[j].ToString(); }
			for (j=0;j<16;j++) { line += "|" + wi.hasWeapon[j].ToString(); }
			for (j=0;j<16;j++) { line += "|" + wa.wepAmmo[j].ToString(); }
			line += "|" + wc.weaponCurrent.ToString();
			line += "|" + wc.weaponIndex.ToString();
			line += "|" + gc.grenadeCurrent.ToString();
			line += "|" + gc.grenadeIndex.ToString();
			for (j=0;j<7;j++) { line += "|" + gi.grenAmmo[j].ToString(); }
			line += "|" + pc.patchCurrent.ToString();
			line += "|" + pc.patchIndex.ToString();
			for (j=0;j<7;j++) { line += "|" + pi.patchCounts[j].ToString(); }
			for (j=0;j<128;j++) { line += "|" + li.hasLog[j].ToString(); }
			for (j=0;j<128;j++) { line += "|" + li.readLog[j].ToString(); }
			for (j=0;j<10;j++) { line += "|" + li.numLogsFromLevel[j].ToString(); }
			line += "|" + li.lastAddedIndex.ToString();
			for (j=0;j<12;j++) { line += "|" + hi.hasHardware[j].ToString(); }
			for (j=0;j<12;j++) { line += "|" + hi.hardwareVersion[j].ToString(); }
			saveData[index] = line;
			index++;
		}

		// Save all the objects data
		for (i=0;i<saveableGameObjects.Count;i++) {
			line = saveableGameObjects[i].GetComponent<SaveObject>().SaveID.ToString();
			line += "|" + saveableGameObjects[i].activeInHierarchy.ToString();
			tr = saveableGameObjects[i].GetComponent<Transform>();
			line += "|" + (tr.localPosition.x.ToString("0000.00000") + "|" + tr.localPosition.y.ToString("0000.00000") + "|" + tr.localPosition.z.ToString("0000.00000"));
			line += "|" + (tr.localRotation.x.ToString("0000.00000") + "|" + tr.localRotation.y.ToString("0000.00000") + "|" + tr.localRotation.z.ToString("0000.00000") + "|" + tr.localRotation.w.ToString("0000.00000"));
			line += "|" + (tr.localScale.x.ToString("0000.00000") + "|" + tr.localScale.y.ToString("0000.00000") + "|" + tr.localScale.z.ToString("0000.00000"));
			saveData[index] = line;
			index++;
		}

		// Write to file
		StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/sav"+saveFileIndex.ToString()+".txt");
		if (sw != null) {
			using (sw) {
				for (j=0;j<saveData.Length;j++) {
					sw.WriteLine(saveData[j]);
				}
				sw.Close();
			}
		}

		// Make "Done!" appear at the end of the line after "Saving..." is finished, stole this from Halo's "Checkpoint...Done!"
		sprint(stringTable[195],allPlayers);
	}

	void LoadPlayerDataToPlayer(GameObject currentPlayer, string[] entries, int currentline) {
		int index = 1;  // Already parsed ID number in main Load() function, skip index 0
		int j;
		float readFloatx;
		float readFloaty;
		float readFloatz;
		float readFloatw;

		PlayerReferenceManager PRman = currentPlayer.GetComponent<PlayerReferenceManager>();
		GameObject pCap = PRman.playerCapsule;
		GameObject playerMainCamera = PRman.playerCapsuleMainCamera;
		GameObject playerInventory = PRman.playerInventory;
		PlayerHealth ph = pCap.GetComponent<PlayerHealth>();
		PlayerEnergy pe = pCap.GetComponent<PlayerEnergy>();
		PlayerMovement pm = pCap.GetComponent<PlayerMovement>();
		PlayerPatch pp = pCap.GetComponent<PlayerPatch>();
		Transform tr = pCap.transform;
		MouseLookScript ml = playerMainCamera.GetComponent<MouseLookScript>();
		Transform trml = playerMainCamera.transform;
		WeaponInventory wi = playerInventory.GetComponent<WeaponInventory>();
		WeaponAmmo wa = playerInventory.GetComponent<WeaponAmmo>();
		WeaponCurrent wc = playerInventory.GetComponent<WeaponCurrent>();
		GrenadeCurrent gc = playerInventory.GetComponent<GrenadeCurrent>();
		GrenadeInventory gi = playerInventory.GetComponent<GrenadeInventory>();
		PatchCurrent pc = playerInventory.GetComponent<PatchCurrent>();
		PatchInventory pi = playerInventory.GetComponent<PatchInventory>();
		LogInventory li = playerInventory.GetComponent<LogInventory>();
		HardwareInventory hi = playerInventory.GetComponent<HardwareInventory>();

		Const.a.playerName = entries[index]; index++;
		ph.hm.health = GetFloatFromString(entries[index],currentline); index++;
		pe.energy = GetFloatFromString(entries[index],currentline); index++;
		pm.currentCrouchRatio = GetFloatFromString(entries[index],currentline); index++;
		pm.bodyState = GetIntFromString(entries[index],currentline,"savegame",index); index++;
		pm.ladderState = GetBoolFromString(entries[index]); index++;
		pm.gravliftState = GetBoolFromString(entries[index]); index++;
		pp.patchActive = GetIntFromString(entries[index],currentline,"savegame",index); index++;
		pp.berserkIncrement = GetIntFromString(entries[index],currentline,"savegame",index); index++;
		pp.sightFinishedTime = GetFloatFromString(entries[index],currentline); index++;
		pp.staminupFinishedTime = GetFloatFromString(entries[index],currentline); index++;
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		tr.localPosition = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		readFloatw = GetFloatFromString(entries[index],currentline); index++;
		tr.localRotation = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		tr.localScale = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		trml.localPosition = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		readFloatw = GetFloatFromString(entries[index],currentline); index++;
		trml.localRotation = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		trml.localScale = new Vector3(readFloatx,readFloaty,readFloatz);
		ml.inventoryMode = !GetBoolFromString(entries[index]); index++; // take opposite because we are about to opposite again
		ml.ToggleInventoryMode(); // correctly set cursor lock state, and opposite again, now it is what was saved
		ml.holdingObject = GetBoolFromString(entries[index]); index++;
		ml.heldObjectIndex = GetIntFromString(entries[index],currentline,"savegame",index); index++;
		ml.heldObjectCustomIndex = GetIntFromString(entries[index],currentline,"savegame",index); index++;
		GUIState.ButtonType bt = (GUIState.ButtonType) Enum.Parse(typeof(GUIState.ButtonType), entries[index]);
		if (Enum.IsDefined(typeof(GUIState.ButtonType),bt)) {
			GUIState.a.overButtonType = bt;
		} index++;
		GUIState.a.overButton = GetBoolFromString(entries[index]); index++;
		ml.geniusActive = GetBoolFromString(entries[index]); index++;
		for (j=0;j<7;j++) { wi.weaponInventoryIndices[j] = GetIntFromString(entries[index],currentline,"savegame",index); index++; }
		for (j=0;j<7;j++) { wi.weaponInventoryAmmoIndices[j] = GetIntFromString(entries[index],currentline,"savegame",index); index++; }
		for (j=0;j<16;j++) { wi.hasWeapon[j] = GetBoolFromString(entries[index]); index++; }
		for (j=0;j<16;j++) { wa.wepAmmo[j] = GetIntFromString(entries[index],currentline,"savegame",index); index++; }
		wc.weaponCurrent = GetIntFromString(entries[index],currentline,"savegame",index); index++;
		wc.weaponIndex = GetIntFromString(entries[index],currentline,"savegame",index); index++;
		gc.grenadeCurrent = GetIntFromString(entries[index],currentline,"savegame",index); index++;
		gc.grenadeIndex = GetIntFromString(entries[index],currentline,"savegame",index); index++;
		for (j=0;j<7;j++) { gi.grenAmmo[j] = GetIntFromString(entries[index],currentline,"savegame",index); index++; }
		pc.patchCurrent = GetIntFromString(entries[index],currentline,"savegame",index); index++;
		pc.patchIndex = GetIntFromString(entries[index],currentline,"savegame",index); index++;
		for (j=0;j<7;j++) { pi.patchCounts[j] = GetIntFromString(entries[index],currentline,"savegame",index); index++; }
		for (j=0;j<128;j++) { li.hasLog[j] = GetBoolFromString(entries[index]); index++; }
		for (j=0;j<128;j++) { li.readLog[j] = GetBoolFromString(entries[index]); index++; }
		for (j=0;j<10;j++) { li.numLogsFromLevel[j] = GetIntFromString(entries[index],currentline,"savegame",index); index++; }
		li.lastAddedIndex = GetIntFromString(entries[index],currentline,"savegame",index); index++;
		for (j=0;j<12;j++) { hi.hasHardware[j] = GetBoolFromString(entries[index]); index++; }
		for (j=0;j<12;j++) { hi.hardwareVersion[j] = GetIntFromString(entries[index],currentline,"savegame",index); index++; }
	}

	void LoadObjectDataToObject(GameObject currentGameObject, string[] entries, int currentline) {
		int index = 1;  // Already parsed ID number in main Load() function, skip index 0
		float readFloatx;
		float readFloaty;
		float readFloatz;
		float readFloatw;
		Vector3 tempvec;

		// Set active state of GameObject in Hierarchy
		currentGameObject.SetActive(GetBoolFromString(entries[index])); index++;

		// Get transform
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
		currentGameObject.transform.localPosition = tempvec;

		// Get rotation
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		readFloatw = GetFloatFromString(entries[index],currentline); index++;
		Quaternion tempquat = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);
		currentGameObject.transform.localRotation = tempquat;

		// Get scale
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
		currentGameObject.transform.localScale = tempvec;
	}

	public void Load(int saveFileIndex) {
		string readline;
		int currentline = 0;

		sprint(stringTable[196],allPlayers); // Loading...
		List<GameObject> playerGameObjects = new List<GameObject>();
		List<GameObject> saveableGameObjects = new List<GameObject>();
		// Find all gameobjects with SaveObject script attached
		GameObject[] getAllGameObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
		foreach (GameObject gob in getAllGameObjects) {
			if (gob.GetComponentInChildren<PlayerReferenceManager>() != null && gob.GetComponent<RuntimeObject>().isRuntimeObject == true) {
				playerGameObjects.Add(gob);
			}

			if (gob.GetComponent<SaveObject>() != null && gob.GetComponent<RuntimeObject>().isRuntimeObject == true) {
				saveableGameObjects.Add(gob);
			}
		}

		StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/sav"+saveFileIndex.ToString()+".txt");
		if (sr != null) {
			using (sr) {
				do {
					readline = sr.ReadLine();
					if (readline == null) {
						currentline++;
						continue; // skip blank lines
					}
					string[] entries = readline.Split('|');  // delimited by | character, aka the vertical bar, pipe, obelisk, etc.
					if (entries.Length <= 1) {
						currentline++;
						continue; // Skip save name
					}
					if (currentline == 1) {
						// Read in global states
						int index = 0;
						int levelNum = GetIntFromString(entries[index],currentline,"savegame",index); index++;
						LevelManager.a.LoadLevelFromSave(levelNum);

						// Set mission bits in questData
						questData.lev1SecCode = GetIntFromString(entries[index],currentline,"savegame",index); index++;
						questData.lev2SecCode = GetIntFromString(entries[index],currentline,"savegame",index); index++;
						questData.lev3SecCode = GetIntFromString(entries[index],currentline,"savegame",index); index++;
						questData.lev4SecCode = GetIntFromString(entries[index],currentline,"savegame",index); index++;
						questData.lev5SecCode = GetIntFromString(entries[index],currentline,"savegame",index); index++;
						questData.lev6SecCode = GetIntFromString(entries[index],currentline,"savegame",index); index++;
						questData.RobotSpawnDeactivated = GetBoolFromString(entries[index]); index++;
						questData.IsotopeInstalled = GetBoolFromString(entries[index]); index++;
						questData.ShieldActivated = GetBoolFromString(entries[index]); index++;
						questData.LaserSafetyOverriden = GetBoolFromString(entries[index]); index++;
						questData.LaserDestroyed = GetBoolFromString(entries[index]); index++;
						questData.BetaGroveCyberUnlocked = GetBoolFromString(entries[index]); index++;
						questData.GroveAlphaJettisonEnabled = GetBoolFromString(entries[index]); index++;
						questData.GroveBetaJettisonEnabled = GetBoolFromString(entries[index]); index++;
						questData.GroveDeltaJettisonEnabled = GetBoolFromString(entries[index]); index++;
						questData.MasterJettisonBroken = GetBoolFromString(entries[index]); index++;
						questData.Relay428Fixed = GetBoolFromString(entries[index]); index++;
						questData.MasterJettisonEnabled = GetBoolFromString(entries[index]); index++;
						questData.BetaGroveJettisoned = GetBoolFromString(entries[index]); index++;
						questData.AntennaNorthDestroyed = GetBoolFromString(entries[index]); index++;
						questData.AntennaSouthDestroyed = GetBoolFromString(entries[index]); index++;
						questData.AntennaEastDestroyed = GetBoolFromString(entries[index]); index++;
						questData.AntennaWestDestroyed = GetBoolFromString(entries[index]); index++;
						questData.SelfDestructActivated = GetBoolFromString(entries[index]); index++;
						questData.BridgeSeparated = GetBoolFromString(entries[index]); index++;
						questData.IsolinearChipsetInstalled = GetBoolFromString(entries[index]); index++;

						// Set difficulties
						difficultyCombat = GetIntFromString(entries[index],currentline,"savegame",index); index++;
						difficultyMission = GetIntFromString(entries[index],currentline,"savegame",index); index++;
						difficultyPuzzle = GetIntFromString(entries[index],currentline,"savegame",index); index++;
						difficultyCyber = GetIntFromString(entries[index],currentline,"savegame",index); index++;
						currentline++;
						continue;
					}

					foreach (GameObject pl in playerGameObjects) {
						if (entries[0] == playerGameObjects[0].GetInstanceID().ToString()) {
							LoadPlayerDataToPlayer(pl,entries,currentline);
							break;
						}
					}

					foreach (GameObject ob in saveableGameObjects) {
						if (entries[0] == ob.GetComponent<SaveObject>().SaveID.ToString()) {
							LoadObjectDataToObject(ob,entries,currentline);
							break;
						}
					}
					currentline++;
				} while (!sr.EndOfStream);
				sr.Close();
			}
		}
		sprint(stringTable[197],allPlayers); // Loading...Done!
	}

	public void SetFOV() {
		if (player1 != null) player1.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().fieldOfView = GraphicsFOV;
		if (player2 != null) player2.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().fieldOfView = GraphicsFOV;
		if (player3 != null) player3.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().fieldOfView = GraphicsFOV;
		if (player4 != null) player4.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().fieldOfView = GraphicsFOV;
	}

	public void SetBloom() {
		if (player1 != null) player1.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.bloom.enabled = GraphicsBloom;
		if (player2 != null) player2.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.bloom.enabled = GraphicsBloom;
		if (player3 != null) player3.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.bloom.enabled = GraphicsBloom;
		if (player4 != null) player4.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.bloom.enabled = GraphicsBloom;
	}

	public void SetSSAO() {
		if (player1 != null) player1.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.ambientOcclusion.enabled = GraphicsSSAO;
		if (player2 != null) player2.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.ambientOcclusion.enabled = GraphicsSSAO;
		if (player3 != null) player3.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.ambientOcclusion.enabled = GraphicsSSAO;
		if (player4 != null) player4.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.ambientOcclusion.enabled = GraphicsSSAO;
	}

	public void SetBrightness() {
		float tempf = Const.a.GraphicsGamma;
		if (tempf < 1) tempf = 0;
		else tempf = tempf/100;
		if (player1 != null) player1.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<ColorCurvesManager>().Factor = tempf;
		if (player2 != null) player2.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<ColorCurvesManager>().Factor = tempf;
		if (player3 != null) player3.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<ColorCurvesManager>().Factor = tempf;
		if (player4 != null) player4.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<ColorCurvesManager>().Factor = tempf;
	}

	public void SetVolume() {
		AudioListener.volume = (AudioVolumeMaster/100f);
		mainmenuMusic.volume = (AudioVolumeMusic/100f);
	}

	public void RegisterObjectWithHealth(HealthManager hm) {
		for (int i=0;i<healthObjectsRegistration.Length;i++) {
			if (healthObjectsRegistration[i] == null) {
				healthObjectsRegistration[i] = hm;
				return;
			}
			if (i == (healthObjectsRegistration.Length - 1)) Debug.Log("WARNING: Could not register object with health.  Hit limit of " + healthObjectsRegistration.Length.ToString() + ".");
		}
	}

	private int AssignConfigInt(string section, string keyname) {
		int inputInt = -1;
		string inputCapture = System.String.Empty;
		inputCapture = INIWorker.IniReadValue(section,keyname);
		if (inputCapture == null) inputCapture = "NULL";
		bool parsed = Int32.TryParse(inputCapture, out inputInt);
		if (parsed) return inputInt; else sprint("Warning: Could not parse config key " + keyname + " as integer: " + inputCapture,allPlayers);
		return 0;
	}

	private bool AssignConfigBool(string section, string keyname) {
		int inputInt = -1;
		string inputCapture = System.String.Empty;
		inputCapture = INIWorker.IniReadValue(section,keyname);
		if (inputCapture == null) inputCapture = "NULL";
		bool parsed = Int32.TryParse(inputCapture, out inputInt);
		if (parsed) {
			if (inputInt > 0) return true; else return false;
		} else sprint("Warning: Could not parse config key " + keyname + " as bool: " + inputCapture,allPlayers);
		return false;
	}

	public bool GetBoolFromString(string val) {
		if (val.ToLower() == "true") return true; else return false;
	}

	public int GetIntFromString(string val, int currentline, string source, int index) {
		bool parsed;
		int readInt;
		if (val == "0") return 0;
		parsed = Int32.TryParse(val,out readInt);
		if (!parsed) {
			sprint("BUG: Could not parse int from " + source + " file on line " + currentline.ToString() + ", from index: " + index.ToString(),allPlayers);
			return 0;
		}
		return readInt;
	}

	public float GetFloatFromString(string val, int currentline) {
		bool parsed;
		float readFloat;
		parsed = Single.TryParse(val,out readFloat);
		if (!parsed) {
			sprint("BUG: Could not parse float from save file on line " + currentline.ToString(),allPlayers);
			return 0.0f;
		}
		return readFloat;
	}

	public string GetBoolAsString(bool inputValue) {
		if (inputValue) return "1";
		return "0";
	}

	public AttackType GetAttackTypeFromInt(int att_type_i) {
		switch(att_type_i) {
		case 1: return AttackType.Melee;
		case 2: return AttackType.EnergyBeam;
		case 3: return AttackType.Magnetic;
		case 4: return AttackType.Projectile;
		case 5: return AttackType.ProjectileEnergyBeam;
		}
		return AttackType.None;
	}

	public PerceptionLevel GetPerceptionLevelFromInt(int percep_i) {
		switch(percep_i) {
		case 1: return PerceptionLevel.Medium;
		case 2: return PerceptionLevel.High;
		case 3: return PerceptionLevel.Omniscient;
		}
		return PerceptionLevel.Low;
	}

	public static DamageData SetNPCDamageData (int NPCindex, aiState attackIndex, GameObject ownedBy) {
		if (NPCindex < 0 || NPCindex > 23) {
			Debug.Log("BUG: NPCindex set incorrectly on NPC.  Not 0 to 23 on NPC at: " + ownedBy.transform.position.x.ToString() + ", " + ownedBy.transform.position.y.ToString() + ", " + ownedBy.transform.position.z + ".");
			return null;
		}
		DamageData dd = new DamageData(); 
		// Attacker (self [a]) data
		dd.owner = ownedBy;
		switch (attackIndex) {
		case aiState.Attack1:
			dd.damage = Const.a.damageForNPC[NPCindex];
			break;
		case aiState.Attack2:
			dd.damage = Const.a.damageForNPC2[NPCindex];
			break;
		case aiState.Attack3:
			dd.damage = Const.a.damageForNPC3[NPCindex];
			break;
		default: Debug.Log("BUG: attackIndex not 0,1, or 2 on NPC! Damage set to 1."); dd.damage = 1f; break;
		}
		dd.penetration = 0;
		dd.offense = 0;
		return dd;
	}

	// Check if particular bit is 1 (ON/TRUE) in binary format of given integer
	public bool CheckFlags (int checkInt, int flag) {
		if ((checkInt & flag) != 0)
			return true;

		return false;
	}

	public static float AngleInDeg(Vector3 vec1, Vector3 vec2) { return ((Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x)) * (180 / Mathf.PI)); }
	/*
	public void UseTargets (UseData ud, GameObject[] targets) {
		for (int i = 0; i < targets.Length; i++) {
			if (targets [i] != null) targets [i].SendMessageUpwards ("Targetted", ud);
		}
	}*/

	public void UseTargets (UseData ud, string targetname) {
		// Old method used SendMessage but this is horribly slow and does not give as much control
		//if (target != null) target.SendMessageUpwards ("Targetted", ud);
		//Debug.Log("Running UseTargets() for targetname of " + targetname);
		// New method allows for targetting multiple objects, not just one gameobject

		// First check if targetname is valid
		if (targetname == null || targetname == "" || targetname == " " || targetname == "  ") {
			Debug.Log("WARNING: invalid target name is either empty string or space or space space!  Ignoring and returning from Const.UseTargets()");
			return;
		}

		if (ud.bitsSet == false) Debug.Log("BUG: calling UseTargets without first setting bits on the UseData struct.  Owner:  " + ud.owner.ToString());
		float numtargetsfound = 0;
		// Find each gameobject with matching targetname in the register, then call Use for each
		for (int i=0;i<TargetRegister.Length;i++) {
			if (TargetnameRegister[i] == targetname) {
				if (TargetRegister[i] != null) {
					numtargetsfound++;
					if (ud.GOSetActive && !TargetRegister[i].activeSelf) TargetRegister[i].SetActive(true); //added activeSelf bit to keep from spamming SetActive when running targets through a trigger_multiple
					if (ud.GOSetDeactive && TargetRegister[i].activeSelf) TargetRegister[i].SetActive(false); // diddo for activeSelf to prevent spamming SetActive
					if (ud.GOToggleActive) TargetRegister[i].SetActive(!TargetRegister[i].activeSelf); // if I abuse this with a trigger_multiple someone should shoot me
					TargetIO tio = TargetRegister[i].GetComponent<TargetIO>();
					Debug.Log("Ran Targetted() on " + numtargetsfound.ToString() + " GameObject(s) with targetname of " + targetname);
					tio.Targetted(ud);
				} else {
					Debug.Log("WARNING: null TargetRegister GameObject linked to targetname of " + targetname + ". Could not run Targetted.");
				}
			}
		}
	}

	public void AddToTargetRegister (GameObject go, string tn) {
		for (int i=0;i<TargetRegister.Length;i++) {
			if (TargetRegister[i] == null) {
				TargetRegister[i] = go;
				TargetnameRegister[i] = tn;
				return; // Ok, game object added to the register, end loop and return
			}
		}
	}

	public void DebugQuestBitShoutOut () {
		Debug.Log("Level 1 Security Code: " + questData.lev1SecCode.ToString());
		Debug.Log("Level 2 Security Code: " + questData.lev2SecCode.ToString());
		Debug.Log("Level 3 Security Code: " + questData.lev3SecCode.ToString());
		Debug.Log("Level 4 Security Code: " + questData.lev4SecCode.ToString());
		Debug.Log("Level 5 Security Code: " + questData.lev5SecCode.ToString());
		Debug.Log("Level 6 Security Code: " + questData.lev6SecCode.ToString());
		Debug.Log("Level R Robot Spawning Deactivated: " + questData.RobotSpawnDeactivated.ToString());
		Debug.Log("Isotope Installed: " + questData.IsotopeInstalled.ToString());
		Debug.Log("Shield Activated: " + questData.ShieldActivated.ToString());
		Debug.Log("Laser Safety Override On: " + questData.LaserSafetyOverriden.ToString());
		Debug.Log("Laser Destroyed: " + questData.LaserDestroyed.ToString());
		Debug.Log("Beta Grove Cyber Unlocked: " + questData.BetaGroveCyberUnlocked.ToString());
		Debug.Log("Grove Alpha Jettison Enabled: " + questData.GroveAlphaJettisonEnabled.ToString());
		Debug.Log("Grove Beta Jettison Enabled: " + questData.GroveBetaJettisonEnabled.ToString());
		Debug.Log("Grove Delta Jettison Enabled: " + questData.GroveDeltaJettisonEnabled.ToString());
		Debug.Log("Master Jettison Broken: " + questData.MasterJettisonBroken.ToString());
		Debug.Log("Relay 428 Fixed: " + questData.Relay428Fixed.ToString());
		Debug.Log("Master Jettison Enabled: " + questData.MasterJettisonEnabled.ToString());
		Debug.Log("Beta Grove Jettisoned: " + questData.BetaGroveJettisoned.ToString());
		Debug.Log("Antenna North Destroyed: " + questData.AntennaNorthDestroyed.ToString());
		Debug.Log("Antenna Soutb Destroyed: " + questData.AntennaSouthDestroyed.ToString());
		Debug.Log("Antenna East Destroyed: " + questData.AntennaEastDestroyed.ToString());
		Debug.Log("Antenna West Destroyed: " + questData.AntennaWestDestroyed.ToString());
		Debug.Log("Self Destruct Activated!: " + questData.SelfDestructActivated.ToString());
		Debug.Log("Bridge Separation Complete: " + questData.BridgeSeparated.ToString());
		Debug.Log("Isolinear Chipset Installed: " + questData.IsolinearChipsetInstalled.ToString());
	}

	public void Shake(bool effectIsWorldwide, float distance, float force) {
		if (distance == -1) distance = globalShakeDistance;
		if (force == -1) force = globalShakeForce;
		if (player1 == null) { Debug.Log("WARNING: Shake() check - no host player1."); return; }  // No host player

		if (effectIsWorldwide) {
			// the whole station is a shakin' and a movin'!
			if (player1 != null) { if (player1.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player1.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
			//if (player2 != null) { if (player2.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player2.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
			//if (player3 != null) { if (player3.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player3.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
			//if (player4 != null) { if (player4.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player4.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
		} else {
			// check if player is close enough and shake em' up!
			if (Vector3.Distance(transform.position, player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform.position) < distance) {
				if (player1 != null) { if (player1.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player1.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
			}
			//if (Vector3.Distance(transform.position, player2.GetComponent<PlayerReferenceManager>().playerCapsule.transform.position) < distance) {
			//	if (player2 != null) { if (player2.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player2.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
			//}
			//if (Vector3.Distance(transform.position, player3.GetComponent<PlayerReferenceManager>().playerCapsule.transform.position) < distance) {
			//	if (player3 != null) { if (player3.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player3.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
			//}
			//if (Vector3.Distance(transform.position, player4.GetComponent<PlayerReferenceManager>().playerCapsule.transform.position) < distance) {
			//	if (player4 != null) { if (player4.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player4.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
			//}
		}
	}

	public void AddCyberPanelToRegistry(CyberWall cw) {
		for (int i=0;i<cyberpanelsRegistry.Length;i++) {
			if (cyberpanelsRegistry[i] == null) {
				cyberpanelsRegistry[i] = cw;
				return;
			}
		}
		Debug.Log("ERROR: Cyberpanels list ran out of room to add into registry in Const.AddCyberPanelToRegistry()");
	}

	public void ConwayGameEntry(CyberWall cw,Vector3 pos) {
		// UPDATE add Conway's game of life effect to cyber panels when hit, only propagate across same orientation panels that are touching
		Debug.Log("Registered a Conway's Game of Life hit");
	}
}

public class QuestBits {
	public int lev1SecCode = -1;
	public int lev2SecCode = -1;
	public int lev3SecCode = -1;
	public int lev4SecCode = -1;
	public int lev5SecCode = -1;
	public int lev6SecCode = -1;
	public bool RobotSpawnDeactivated;
	public bool IsotopeInstalled;
	public bool ShieldActivated;
	public bool LaserSafetyOverriden;
	public bool LaserDestroyed;
	public bool BetaGroveCyberUnlocked;
	public bool GroveAlphaJettisonEnabled;
	public bool GroveBetaJettisonEnabled;
	public bool GroveDeltaJettisonEnabled;
	public bool MasterJettisonBroken;
	public bool Relay428Fixed;
	public bool MasterJettisonEnabled;
	public bool BetaGroveJettisoned;
	public bool AntennaNorthDestroyed;
	public bool AntennaSouthDestroyed;
	public bool AntennaEastDestroyed;
	public bool AntennaWestDestroyed;
	public bool SelfDestructActivated;
	public bool BridgeSeparated;
	public bool IsolinearChipsetInstalled;

	public void TargetOnGatePassed(bool bitToCheck, bool passIfTrue, UseData ud, TargetIO tio, string targ, string arg, string targOnFalse, string argOnFalse) {
		if (passIfTrue) {
			if (!bitToCheck) { RunTargets(ud,tio,targOnFalse,argOnFalse); return; }
		} else {
			if (bitToCheck) { RunTargets(ud,tio,targOnFalse,argOnFalse); return; }
		}

		RunTargets(ud,tio,targ,arg);
	}

	private void RunTargets(UseData ud, TargetIO tio, string target, string argvalue) {
		ud.argvalue = argvalue; // grr, arg! (Mutant Enemy reference alert)
		ud.SetBits(tio);
		Const.a.UseTargets(ud,target);
	}
}