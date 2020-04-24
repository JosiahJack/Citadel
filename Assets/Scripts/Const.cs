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
	[SerializeField] public int[] audioLogType;  // 0 = text only, 1 = normal, 2 = email, 3 = papers, 4 = vmail
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
	[SerializeField] public string[] creditsText;
	[HideInInspector]
	public int creditsLength;

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
						ConcussionLive,EMPLive,GasLive,GasExplosions,CorpseHit,NPCMagpulseShots,NPCRailgunShots,
						LeafBurst,MutationBurst,GraytationBurst,BarrelExplosions};
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
	public GameObject Pool_LeafBurst;
	public GameObject Pool_MutationBurst;
	public GameObject Pool_GraytationBurst;
	public GameObject Pool_BarrelExplosions;

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
	public Material[] screenCodes;
	public int[] npcCount;

	public bool gameFinished = false;

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
			if (gob.GetComponent<PlayerReferenceManager>() != null) {
				if (gob.GetComponent<SaveObject>().isRuntimeObject) {
					playerGameObjects.Add(gob);
				}
			}
		}

		//if (playerGameObjects.Count > 0) player1 = playerGameObjects[0];
		//if (playerGameObjects.Count > 1) player2 = playerGameObjects[1];
		//if (playerGameObjects.Count > 2) player3 = playerGameObjects[2];
		//if (playerGameObjects.Count > 3) player4 = playerGameObjects[3];
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
		LoadCreditsData();
		questData = new QuestBits ();
		questData.lev1SecCode = UnityEngine.Random.Range(0,10);
		questData.lev2SecCode = UnityEngine.Random.Range(0,10);
		questData.lev3SecCode = UnityEngine.Random.Range(0,10);
		questData.lev4SecCode = UnityEngine.Random.Range(0,10);
		questData.lev5SecCode = UnityEngine.Random.Range(0,10);
		questData.lev6SecCode = UnityEngine.Random.Range(0,10);
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

	private void LoadCreditsData () {
		string readline; // variable to hold each string read in from the file
		int currentline = 0;
		int pagenum = 0;
		creditsLength = 1;

		StreamReader dataReader = new StreamReader(Application.dataPath + "/StreamingAssets/credits.txt",Encoding.Default);
		using (dataReader) {
			do {
				// Read the next line
				readline = dataReader.ReadLine();
				char[] checkCharacter = readline.ToCharArray();
				if (checkCharacter.Length > 0) {
					if (checkCharacter[0] == '#') {
						pagenum++;
						creditsLength++;
						continue;
					}
				}
				if (pagenum >= creditsText.Length) { Debug.Log("pagenum was too large at " + pagenum.ToString()); return; }
                creditsText[pagenum] += readline + System.Environment.NewLine;
				currentline++;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			Debug.Log("Credits pages length is " + creditsLength.ToString());
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

	public static string GetTargetID(int npc23Index) {
		Const.a.npcCount[npc23Index]++;
		return Const.a.nameForNPC[npc23Index] + Const.a.npcCount[npc23Index];
	}

	public static void sprintByIndexOrOverride (int index, string overrideString, GameObject playerPassed) {
		bool useAllPlayers = false;
		if (playerPassed == null) useAllPlayers = true;

		if (string.IsNullOrWhiteSpace(overrideString)) {
			if (index >= 0) {
				if (useAllPlayers) {
					sprint(Const.a.stringTable[index],Const.a.allPlayers);
				} else {
					sprint(Const.a.stringTable[index],playerPassed);
				}
			} else {
				//Debug.Log("Attempting to sprintByIndexOrOverride with a -1 index and a nullorwhitespace overrideString");
			}
		} else {
			if (useAllPlayers) {
				sprint(overrideString,Const.a.allPlayers);
			} else {
				sprint(overrideString,playerPassed);
			}
		}
	}

	// StatusBar Print
	public static void sprint (string input, GameObject player) {
		//Debug.Log(input);  // print to console
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
		case PoolType.LeafBurst:
			poolContainer = Pool_LeafBurst;
			poolName = "LeafBurst ";
			break;
		case PoolType.MutationBurst:
			poolContainer = Pool_MutationBurst;
			poolName = "MutationBurst ";
			break;
		case PoolType.GraytationBurst:
			poolContainer = Pool_GraytationBurst;
			poolName = "GraytationBurst ";
			break;
		case PoolType.BarrelExplosions:
			poolContainer = Pool_BarrelExplosions;
			poolName = "BarrelExplosions ";
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

	string SavePlayerData(GameObject plyr) {
		string line = System.String.Empty;
		Transform tr, trml;
		int j = 0;
		// First get all references to relevant componenets on all relevant gameobjects in the player gameobject
		PlayerReferenceManager PRman = plyr.GetComponent<PlayerReferenceManager>();
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
		SaveObject sav = plyr.GetComponent<SaveObject>();

		line = sav.saveableType;
		line += "|" + sav.SaveID.ToString();
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
		return line;
	}

	// For ammo on the weapons
	string SaveUseableData(GameObject go) {
		string line = System.String.Empty;
		UseableObjectUse uou = go.GetComponent<UseableObjectUse>();
		if (uou != null) {
			line = uou.useableItemIndex.ToString(); // int - the main lookup index, needed for intanciating on load if doesn't match original SaveID
			line += "|" + uou.customIndex.ToString(); // int - special reference like audiolog message
			line += "|" + uou.ammo.ToString(); // int - how much ammo is on the weapon
			line += "|" + uou.ammoIsSecondary.ToString(); //bool - is it the alternate ammo type? e.g. Penetrator or Teflon
		}
		return line;
	}

	// Live grenades - These should only be up in the air or active running timer, but still...or it's a landmine
	string SaveGrenadeData(GameObject go) {
		string line = System.String.Empty;
		GrenadeActivate ga = go.GetComponent<GrenadeActivate>();
		if (ga != null) {
			line = ga.useTimer.ToString(); // do we have a timer going? MAKE SURE YOU CHECK THIS BIT IN LOAD!
			if (ga.useTimer) line += "|" + ga.timeFinished.ToString("0000.00000"); // float - how much time left before the fun part?
			line += "|" + ga.explodeOnContact.ToString(); // bool - or not a landmine
			line += "|" + ga.useProx.ToString(); // bool - is this a landmine?
		}
		return line;
	}

	// Generic health info string
	string GetHealthManagerSaveData(HealthManager hm) {
		string line = System.String.Empty;
		if (hm != null) {
			line = hm.health.ToString("0000.00000"); // how much health we have
			line += "|" + hm.deathDone.ToString(); // bool - are we dead yet?
			line += "|" + hm.god.ToString(); // are we invincible? - we can save cheats?? OH WOW!
			line += "|" + hm.teleportDone.ToString(); // did we already teleport?
		}
		return line;
	}

	// Save destructable items like cameras, barrels, etc.
	string SaveDestructableData(GameObject go) {
		string line = System.String.Empty;
		line = GetHealthManagerSaveData(go.GetComponent<HealthManager>()); // get the health info
		return line;
	}	

	// Info about the enemy's current state
	string SaveNPCData(GameObject go) {
		if (go == null) { Debug.Log("BUG: attempting to SaveNPCData for null GameObject go"); }
		string line = System.String.Empty;
		AIController aic = go.GetComponent<AIController>();
		AIAnimationController aiac = go.GetComponentInChildren<AIAnimationController>();
		if (aic != null) {
			switch (aic.currentState) {
				case Const.aiState.Idle: line = "0"; break;
				case Const.aiState.Walk: line = "1"; break;
				case Const.aiState.Run: line = "2"; break;
				case Const.aiState.Attack1: line = "3"; break;
				case Const.aiState.Attack2: line = "4"; break;
				case Const.aiState.Attack3: line = "5"; break;
				case Const.aiState.Pain: line = "6"; break;
				case Const.aiState.Dying: line = "7"; break;
				case Const.aiState.Inspect: line = "8"; break;
				case Const.aiState.Interacting: line = "9"; break;
			}
			line += "|" + aic.enemy.GetComponent<SaveObject>().SaveID.ToString(); // saveID of playerCapsule
		}
		line += "|" + GetHealthManagerSaveData(go.GetComponent<HealthManager>());
		return line;
	}

	// Save searchable data
	string SaveSearchableStaticData(GameObject go) {
		string line = System.String.Empty;
		SearchableItem se = go.GetComponent<SearchableItem>();
		if (se != null) {
			line = se.contents[0].ToString(); // int main lookup index
			line += "|" + se.contents[1].ToString(); // int main lookup index
			line += "|" + se.contents[2].ToString(); // int main lookup index
			line += "|" + se.contents[3].ToString(); // int main lookup index
			line += "|" + se.customIndex[0].ToString(); // int custom index
			line += "|" + se.customIndex[1].ToString(); // int custom index
			line += "|" + se.customIndex[2].ToString(); // int custom index
			line += "|" + se.customIndex[3].ToString(); // int custom index
		}
		return line;
	}	

	string SaveSearchableDestructsData(GameObject go) {
		string line = System.String.Empty;
		line = SaveSearchableStaticData(go); // get the searchable data
		line += GetHealthManagerSaveData(go.GetComponent<HealthManager>()); // get health info
		return line;
	}

	string SaveDoorData(GameObject go) {
		string line = System.String.Empty;
		Door dr = go.GetComponent<Door>();
		if (dr != null) {
			line = dr.targetAlreadyDone.ToString(); // bool - have we already ran targets
			line += "|" + dr.locked.ToString(); // bool - is this locked?
			line += "|" + dr.blocked.ToString(); // bool - is the door blocked currently?
			switch (dr.doorOpen) {
				case Door.doorState.Closed: line += "|0"; break;
				case Door.doorState.Open: line += "|1"; break;
				case Door.doorState.Closing: line += "|2"; break;
				case Door.doorState.Opening: line += "|3"; break;
			}
		}
		return line;
	}	

	string SaveForceBridgeData(GameObject go) {
		string line = System.String.Empty;
		ForceBridge fb = go.GetComponent<ForceBridge>();
		if (fb != null) {
			line = fb.activated.ToString(); // bool - is the bridge on?
			line += "|" + fb.lerping.ToString(); // bool - are we currently lerping one way or tother
		}
		return line;
	}

	string SaveSwitchData(GameObject go) {
		string line = System.String.Empty;
		ButtonSwitch bs = go.GetComponent<ButtonSwitch>();
		if (bs != null) {
			// bs?  null?  that's bs
			line = bs.locked.ToString(); // bool - is this switch locked
			line += "|" + bs.active.ToString(); // bool - is the switch flashing?
			line += "|" + bs.alternateOn.ToString(); // bool - is the flashing material on?
			line += "|" + bs.delayFinished.ToString("0000.00000"); // float - time before firing targets
		}
		return line;
	}	

	string SaveFuncWallData(GameObject go) {
		string line = System.String.Empty;
		FuncWall fw = go.GetComponent<FuncWall>();
		if (fw != null) {
			switch (fw.currentState) {
				case FuncWall.FuncStates.Start: line = "0"; break;
				case FuncWall.FuncStates.Target: line = "1"; break;
				case FuncWall.FuncStates.MovingStart: line = "2"; break;
				case FuncWall.FuncStates.MovingTarget: line = "3"; break;
				case FuncWall.FuncStates.AjarMovingStart: line = "4"; break;
				case FuncWall.FuncStates.AjarMovingTarget: line = "5"; break;
			}
		}
		return line;
	}	

	string SaveTeleDestData(GameObject go) {
		string line = System.String.Empty;
		TeleportTouch tt = go.GetComponent<TeleportTouch>();
		if (tt != null) {
			line = tt.justUsed.ToString("0000.00000"); // float - is the player still touching it?
		}
		return line;
	}	

	string SaveLogicBranchData(GameObject go) {
		string line = System.String.Empty;
		LogicBranch lb = go.GetComponent<LogicBranch>();
		if (lb != null) {
			line = lb.relayEnabled.ToString(); // bool - is this enabled
			line += "|" + lb.onSecond.ToString(); // bool - which one are we on?
		}
		return line;
	}	

	string SaveLogicRelayData(GameObject go) {
		string line = System.String.Empty;
		LogicRelay lr = go.GetComponent<LogicRelay>();
		if (lr != null) {
			line = lr.relayEnabled.ToString(); // bool - is this enabled
		}
		return line;
	}

	string SaveSpawnerData(GameObject go) {
		string line = System.String.Empty;
		SpawnManager sm = go.GetComponent<SpawnManager>();
		if (sm != null) {
			line = sm.active.ToString(); // bool - is this enabled
			line += "|" + sm.numberActive.ToString(); // int - number spawned
			line += "|" + sm.delayFinished.ToString("0000.00000"); // float - time that we need to spawn next
		}
		return line;
	}	

	string SaveInteractablePanelData(GameObject go) {
		string line = System.String.Empty;
		InteractablePanel ip = go.GetComponent<InteractablePanel>();
		if (ip != null) {
			line = ip.open.ToString(); // bool - is the panel opened
			line += "|" + ip.installed.ToString(); // bool - is the item installed, MAKE SURE YOU ENABLE THE INSTALL ITEM GameObject IN LOAD
		}
		return line;
	}		

	string SaveElevatorPanelData(GameObject go) {
		string line = System.String.Empty;
		KeypadElevator ke = go.GetComponent<KeypadElevator>();
		if (ke != null) {
			line = ke.padInUse.ToString(); // bool - is the pad being used by a player
			line += "|" + ke.locked.ToString(); // bool - locked?
		}
		return line;
	}	

	string SaveKeypadData(GameObject go) {
		string line = System.String.Empty;
		KeypadKeycode kk = go.GetComponent<KeypadKeycode>();
		if (kk != null) {
			line = kk.padInUse.ToString(); // bool - is the pad being used by a player
			line += "|" + kk.locked.ToString(); // bool - locked?
			line += "|" + kk.solved.ToString(); // bool - already entered correct keycode?
		}
		return line;
	}	

	string SavePuzzleGridData(GameObject go) {
		string line = System.String.Empty;
		PuzzleGridPuzzle pgp = go.GetComponent<PuzzleGridPuzzle>();
		if (pgp != null) {
			line = pgp.puzzleSolved.ToString(); // bool - is this puzzle already solved?
			for (int i=0;i<pgp.grid.Length;i++) { line += "|" + pgp.grid[i].ToString(); } // bool - get the current grid states + or X
			line += "|" + pgp.fired.ToString(); // bool - have we already fired yet?
			line += "|" + pgp.locked.ToString(); // bool - is this locked?
		}
		return line;
	}	

	string SavePuzzleWireData(GameObject go) {
		string line = System.String.Empty;
		PuzzleWirePuzzle pwp = go.GetComponent<PuzzleWirePuzzle>();
		if (pwp != null) {
			line = pwp.puzzleSolved.ToString(); // bool - is this puzzle already solved?
			for (int i=0;i<pwp.currentPositionsLeft.Length;i++) { line += "|" + pwp.currentPositionsLeft[i].ToString(); } // int - get the current wire positions
			for (int i=0;i<pwp.currentPositionsRight.Length;i++) { line += "|" + pwp.currentPositionsRight[i].ToString(); } // int - get the current wire positions
			line += "|" + pwp.locked.ToString(); // bool - is this locked?
		}
		return line;
	}

	string SaveTCounterData(GameObject go) {
		string line = System.String.Empty;
		TriggerCounter tc = go.GetComponent<TriggerCounter>();
		if (tc != null) {
			line = tc.counter.ToString(); // int - how many counts we have
		}
		return line;	
	}

	string SaveTGravityData(GameObject go) {
		string line = System.String.Empty;
		GravityLift gl = go.GetComponent<GravityLift>();
		if (gl != null) {
			line = gl.active.ToString(); // bool - is this gravlift on?
		}
		return line;
	}

	string SaveMChangerData(GameObject go) {
		string line = System.String.Empty;
		MaterialChanger mch = go.GetComponent<MaterialChanger>();
		if (mch != null) {
			line = mch.alreadyDone.ToString(); // bool - is this gravlift on?
		}
		return line;
	}

	string SaveTRadiationData(GameObject go) {
		string line = System.String.Empty;
		Radiation rad = go.GetComponent<Radiation>();
		if (rad != null) {
			line = rad.isEnabled.ToString(); // bool - hey is this on? hello?
			line += "|" + rad.numPlayers.ToString(); // int - how many players we are affecting
		}
		return line;
	}

	string SaveGravLiftPadTextureData(GameObject go) {
		string line = System.String.Empty;
		TextureChanger tex = go.GetComponent<TextureChanger>();
		if (tex != null) {
			line = tex.currentTexture.ToString(); // bool - is this gravlift on?
		}
		return line;
	}



	// Save the Game
	// ============================================================================
	public void Save(int saveFileIndex,string savename) {
		string[] saveData = new string[4096]; // Found 2987 saveable objects on main level - should be enough for any instantiated dropped items...maybe
		string line;
		int i,j;
		int index = 0;
		Transform tr;
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

		if (player1 != null) playerGameObjects.Add(player1);
		if (player2 != null) playerGameObjects.Add(player2);
		if (player3 != null) playerGameObjects.Add(player3);
		if (player4 != null) playerGameObjects.Add(player4);
		// Find all gameobjects with SaveObject script attached
		GameObject[] getAllGameObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
		foreach (GameObject gob in getAllGameObjects) {
			if (gob.GetComponent<SaveObject>() != null) {
				if (gob.GetComponent<SaveObject>().isRuntimeObject == true) {
					saveableGameObjects.Add(gob);
				}
			}
		}

		Debug.Log("Num players: " + playerGameObjects.Count.ToString(),allPlayers);
		Debug.Log("Num saveables: " + saveableGameObjects.Count.ToString(),allPlayers);

		int numTransforms = 0;
		int numUseables = 0;
		int numGrenades = 0;
		int numNPCs = 0;
		int numDestructables = 0;
		int numSearchableStatics = 0;
		int numSearchableDestructs = 0;
		int numDoors = 0;
		int numForceBs = 0;
		int numSwitches = 0;
		int numFuncWalls = 0;
		int numTeleDests = 0;
		int numBranches = 0;
		int numRelays = 0;
		int numSpawners = 0;
		int numIntPanels = 0;
		int numElevPanels = 0;
		int numKeypads = 0;
		int numPuzGrids = 0;
		int numPuzWires = 0;
		int numTrigCounters = 0;
		int numTrigGravity = 0;
		int numMChangers = 0;
		int numRadTrigs = 0;
		int numGravPads = 0;

		// Save all the players' data
		if (player1 != null) {
			saveData[index] = SavePlayerData(player1);
			index++;
		}
		if (player2 != null) {
			saveData[index] = SavePlayerData(player2);
			index++;
		}
		if (player3 != null) {
			saveData[index] = SavePlayerData(player3);
			index++;
		}
		if (player4 != null) {
			saveData[index] = SavePlayerData(player4);
			index++;
		}

		// Save all the objects data
		for (i=0;i<saveableGameObjects.Count;i++) {
			line = saveableGameObjects[i].GetComponent<SaveObject>().SaveID.ToString();
			line += "|" + saveableGameObjects[i].activeSelf.ToString();
			tr = saveableGameObjects[i].GetComponent<Transform>();
			line += "|" + (tr.localPosition.x.ToString("0000.00000") + "|" + tr.localPosition.y.ToString("0000.00000") + "|" + tr.localPosition.z.ToString("0000.00000"));
			line += "|" + (tr.localRotation.x.ToString("0000.00000") + "|" + tr.localRotation.y.ToString("0000.00000") + "|" + tr.localRotation.z.ToString("0000.00000") + "|" + tr.localRotation.w.ToString("0000.00000"));
			line += "|" + (tr.localScale.x.ToString("0000.00000") + "|" + tr.localScale.y.ToString("0000.00000") + "|" + tr.localScale.z.ToString("0000.00000"));
			Rigidbody rbody = saveableGameObjects[i].GetComponent<Rigidbody>();
			if (rbody != null) {
				line += "|" + rbody.velocity.x.ToString("0000.00000") + "|" + rbody.velocity.y.ToString("0000.00000") + "|" + rbody.velocity.z.ToString("0000.00000");
			} else {
				line += "|0000.00000|0000.00000|0000.00000";
			}
			string stype = saveableGameObjects[i].GetComponent<SaveObject>().saveableType;
			line += "|" + stype;
			line += "|" + saveableGameObjects[i].GetComponent<SaveObject>().levelParentID.ToString(); // int level dynamic object index
			switch (saveableGameObjects[i].GetComponent<SaveObject>().saveType) {
				case SaveObject.SaveableType.Useable: numUseables++; line += "|" + SaveUseableData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.Grenade: numGrenades++; line += "|" + SaveGrenadeData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.NPC: numNPCs++; line += "|" + SaveNPCData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.Destructable: numDestructables++;  line += "|" + SaveDestructableData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.SearchableStatic: numSearchableStatics++;  line += "|" + SaveSearchableStaticData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.SearchableDestructable: numSearchableDestructs++;  line += "|" + SaveSearchableDestructsData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.Door: numDoors++;  line += "|" + SaveDoorData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.ForceBridge: numForceBs++;  line += "|" + SaveForceBridgeData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.Switch: numSwitches++;  line += "|" + SaveSwitchData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.FuncWall: numFuncWalls++;  line += "|" + SaveFuncWallData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.TeleDest: numTeleDests++;  line += "|" + SaveTeleDestData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.LBranch: numBranches++;  line += "|" + SaveLogicBranchData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.LRelay: numRelays++;  line += "|" + SaveLogicRelayData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.LSpawner: numSpawners++;  line += "|" + SaveSpawnerData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.InteractablePanel: numIntPanels++;  line += "|" + SaveInteractablePanelData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.ElevatorPanel: numElevPanels++;  line += "|" + SaveElevatorPanelData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.Keypad: numKeypads++;  line += "|" + SaveKeypadData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.PuzzleGrid: numPuzGrids++;  line += "|" + SavePuzzleGridData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.PuzzleWire: numPuzWires++;  line += "|" + SavePuzzleWireData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.TCounter: numTrigCounters++;  line += "|" + SaveTCounterData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.TGravity: numTrigGravity++;  line += "|" + SaveTGravityData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.MChanger: numMChangers++;  line += "|" + SaveMChangerData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.RadTrig: numRadTrigs++;  line += "|" + SaveTRadiationData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.GravPad: numGravPads++;  line += "|" + SaveGravLiftPadTextureData(saveableGameObjects[i]); break;
				default: numTransforms++; break; // we already did the plain ol transform data first up above
			}
			saveData[index] = line; // take this objects data and add it to the array
			index++; // move to the next line
		}

		Debug.Log("Number of Transforms: " + numTransforms.ToString());
		Debug.Log("Number of Useable: " + numUseables.ToString());
		Debug.Log("Number of Grenades: " + numGrenades.ToString());
		Debug.Log("Number of NPCs: " + numNPCs.ToString());
		Debug.Log("Number of Destructables: " + numDestructables.ToString());
		Debug.Log("Number of SearchableStatics: " + numSearchableStatics.ToString());
		Debug.Log("Number of SearchableDestructables: " + numSearchableDestructs.ToString());
		Debug.Log("Number of Doors: " + numDoors.ToString());
		Debug.Log("Number of ForceBridges: " + numForceBs.ToString());
		Debug.Log("Number of Switches: " + numSwitches.ToString());
		Debug.Log("Number of FuncWalls: " + numFuncWalls.ToString());
		Debug.Log("Number of TeleDests: " + numTeleDests.ToString());
		Debug.Log("Number of Branches: " + numBranches.ToString());
		Debug.Log("Number of Relays: " + numRelays.ToString());
		Debug.Log("Number of Spawners: " + numSpawners.ToString());
		Debug.Log("Number of InteractablePanels: " + numIntPanels.ToString());
		Debug.Log("Number of ElevatorPanels: " + numElevPanels.ToString());
		Debug.Log("Number of Keypad: " + numKeypads.ToString());
		Debug.Log("Number of PuzzleGrid: " + numPuzGrids.ToString());
		Debug.Log("Number of PuzzleWire: " + numPuzWires.ToString());
		Debug.Log("Number of TriggerCounters: " + numTrigCounters.ToString());
		Debug.Log("Number of TriggerGravities: " + numTrigGravity.ToString());
		Debug.Log("Number of MaterialChangers: " + numMChangers.ToString());
		Debug.Log("Number of RadiationTriggers: " + numRadTrigs.ToString());
		Debug.Log("Number of GravityPads: " + numGravPads.ToString());

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

	void LoadNewGamePlayerDataToPlayer(GameObject currentPlayer) {
		int j=0;
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

		ph.hm.health = 211f;
		pe.energy = 54f;
		pm.currentCrouchRatio = 1f;
		pm.bodyState = 0;
		pm.ladderState = false;
		pm.gravliftState = false;
		pp.patchActive = 0;
		pp.berserkIncrement = 0;
		pp.sightFinishedTime = Time.time;
		pp.staminupFinishedTime = Time.time;
		ml.yRotation = ml.startyRotation;
		ml.xRotation = ml.startxRotation;
		tr.localRotation = Quaternion.Euler(0f, ml.yRotation, 0f); // left right component applied to capsule	
		ml.transform.localRotation = Quaternion.Euler(ml.xRotation,0f,0f); // Up down component only applied to camera
		tr.localScale = new Vector3(1f,1f,1f);
		ml.inventoryMode = false;
		ml.ToggleInventoryMode();
		ml.holdingObject = false;
		ml.heldObjectIndex = -1;
		ml.heldObjectCustomIndex = -1;
		GUIState.a.overButtonType = GUIState.ButtonType.None;
		GUIState.a.overButton = false;
		ml.geniusActive = false;
		for (j=0;j<7;j++) { wi.weaponInventoryIndices[j] = -1; }
		for (j=0;j<7;j++) { wi.weaponInventoryAmmoIndices[j] = j;}
		for (j=0;j<16;j++) { wi.hasWeapon[j] = false; }
		for (j=0;j<16;j++) { wa.wepAmmo[j] = 0; }
		wc.weaponCurrent = 0;
		wc.weaponIndex = -1;
		gc.grenadeCurrent = 0;
		gc.grenadeIndex = -1;
		for (j=0;j<7;j++) { gi.grenAmmo[j] = 0; }
		pc.patchCurrent = 0;
		pc.patchIndex = -1;
		for (j=0;j<7;j++) { pi.patchCounts[j] = 0; }
		for (j=0;j<128;j++) { li.hasLog[j] = false; }
		for (j=0;j<128;j++) { li.readLog[j] = false; }
		for (j=0;j<10;j++) { li.numLogsFromLevel[j] = 0; }
		li.lastAddedIndex = -1;
		for (j=0;j<12;j++) { hi.hasHardware[j] = false; }
		for (j=0;j<12;j++) { hi.hardwareVersion[j] = -1; }
	}

	void LoadPlayerDataToPlayer(GameObject currentPlayer, string[] entries, int currentline,int index) {
		//int index = 1;  // Already parsed ID number in main Load() function, skip index 0
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

	void LoadObjectDataToObject(GameObject currentGameObject, string[] entries, int currentline, int index) {
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

		// Get rigidbody velocity
		Rigidbody rbody = currentGameObject.GetComponent<Rigidbody>();
		if (rbody != null) {
			readFloatx = GetFloatFromString(entries[index],currentline); index++;
			readFloaty = GetFloatFromString(entries[index],currentline); index++;
			readFloatz = GetFloatFromString(entries[index],currentline); index++;
			tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
			rbody.velocity = tempvec;
		} else {
			index = index + 3; // moving along and ignoring the zeros
		}

		HealthManager hm = currentGameObject.GetComponent<HealthManager>(); // used multiple times below
		SearchableItem se = currentGameObject.GetComponent<SearchableItem>(); // used multiple times below
		switch (currentGameObject.GetComponent<SaveObject>().saveType) {
			case SaveObject.SaveableType.Useable:
				UseableObjectUse uou = currentGameObject.GetComponent<UseableObjectUse>();
				if (uou != null) {
					uou.useableItemIndex = GetIntFromString(entries[index],currentline,"savegame",index); index++;
					uou.customIndex = GetIntFromString(entries[index],currentline,"savegame",index); index++;
					uou.ammo = GetIntFromString(entries[index],currentline,"savegame",index); index++;
					uou.ammoIsSecondary = GetBoolFromString(entries[index]); index++;
				}
				break;
			case SaveObject.SaveableType.Grenade:
				GrenadeActivate ga = currentGameObject.GetComponent<GrenadeActivate>();
				if (ga != null) {
					ga.useTimer = GetBoolFromString(entries[index]); index++; // do we have a timer going?
					if (ga.useTimer) ga.timeFinished = GetFloatFromString(entries[index],currentline); index++; // float - how much time left before the fun part?
					ga.explodeOnContact = GetBoolFromString(entries[index]); index++; // bool - or not a landmine
					ga.useProx = GetBoolFromString(entries[index]); index++; // bool - is this a landmine?
				}
				break;
			case SaveObject.SaveableType.NPC:
				AIController aic = currentGameObject.GetComponent<AIController>();
				AIAnimationController aiac = currentGameObject.GetComponentInChildren<AIAnimationController>();
				if (aic != null) {
					int state = GetIntFromString(entries[index],currentline,"savegame",index); index++;
					switch (state) {
						case 0: aic.currentState = Const.aiState.Idle; break;
						case 1: aic.currentState = Const.aiState.Walk; break;
						case 2: aic.currentState = Const.aiState.Run; break;
						case 3: aic.currentState = Const.aiState.Attack1; break;
						case 4: aic.currentState = Const.aiState.Attack2; break;
						case 5: aic.currentState = Const.aiState.Attack3; break;
						case 6: aic.currentState = Const.aiState.Pain; break;
						case 7: aic.currentState = Const.aiState.Dying; break;
						case 8: aic.currentState = Const.aiState.Inspect; break;
						case 9: aic.currentState = Const.aiState.Interacting; break;
					}
					int enemIDRead = GetIntFromString(entries[index],currentline,"savegame",index); index++;
					if (player1 != null) {
						if (player1.GetComponent<SaveObject>().SaveID == enemIDRead) {
							aic.enemy = player1;
						}
					}
					if (player2 != null) {
						if (player2.GetComponent<SaveObject>().SaveID == enemIDRead) {
							aic.enemy = player2;
						}
					}
					if (player3 != null) {
						if (player3.GetComponent<SaveObject>().SaveID == enemIDRead) {
							aic.enemy = player3;
						}
					}
					if (player4 != null) {
						if (player4.GetComponent<SaveObject>().SaveID == enemIDRead) {
							aic.enemy = player4;
						}
					}
				} else {
					index = index + 2;
				}
				if (hm != null) {
					hm.health = GetFloatFromString(entries[index],currentline); index++; // how much health we have
					hm.deathDone = GetBoolFromString(entries[index]); index++; // bool - are we dead yet?
					hm.god = GetBoolFromString(entries[index]); index++; // are we invincible? - we can save cheats?? OH WOW!
					hm.teleportDone = GetBoolFromString(entries[index]); index++; // did we already teleport?
				} else {
					index = index + 4;
				}
				break;
			case SaveObject.SaveableType.Destructable:
				if (hm != null) {
					hm.health = GetFloatFromString(entries[index],currentline); index++; // how much health we have
					hm.deathDone = GetBoolFromString(entries[index]); index++; // bool - are we dead yet?
					hm.god = GetBoolFromString(entries[index]); index++; // are we invincible? - we can save cheats?? OH WOW!
					hm.teleportDone = GetBoolFromString(entries[index]); index++; // did we already teleport?
				} else {
					index = index + 4;
				}
				break;
			case SaveObject.SaveableType.SearchableStatic:
				if (se != null) {
					se.contents[0] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int main lookup index
					se.contents[1] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int main lookup index
					se.contents[2] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int main lookup index
					se.contents[3] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int main lookup index
					se.customIndex[0] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int custom index
					se.customIndex[1] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int custom index
					se.customIndex[2] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int custom index
					se.customIndex[3] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int custom index
				} else {
					index += 8;
				}
				break;
			case SaveObject.SaveableType.SearchableDestructable:
				if (se != null) {
					se.contents[0] = GetIntFromString(entries[index],currentline,"savegame",index); index++;; // int main lookup index
					se.contents[1] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int main lookup index
					se.contents[2] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int main lookup index
					se.contents[3] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int main lookup index
					se.customIndex[0] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int custom index
					se.customIndex[1] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int custom index
					se.customIndex[2] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int custom index
					se.customIndex[3] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int custom index
				} else {
					index += 8;
				}
				if (hm != null) {
					hm.health = GetFloatFromString(entries[index],currentline); index++; // how much health we have
					hm.deathDone = GetBoolFromString(entries[index]); index++; // bool - are we dead yet?
					hm.god = GetBoolFromString(entries[index]); index++; // are we invincible? - we can save cheats?? OH WOW!
					hm.teleportDone = GetBoolFromString(entries[index]); index++; // did we already teleport?
				} else {
					index = index + 4;
				}
				break;
			case SaveObject.SaveableType.Door:
				Door dr = currentGameObject.GetComponent<Door>();
				if (dr != null) {
					dr.targetAlreadyDone = GetBoolFromString(entries[index]); index++; // bool - have we already ran targets
					dr.locked = GetBoolFromString(entries[index]); index++; // bool - is this locked?
					dr.blocked = GetBoolFromString(entries[index]); index++; // bool - is the door blocked currently?
					int state = GetIntFromString(entries[index],currentline,"savegame",index); index++;
					switch (state) {
						case 0: dr.doorOpen = Door.doorState.Closed; break;
						case 1: dr.doorOpen = Door.doorState.Open; break;
						case 2: dr.doorOpen = Door.doorState.Closing; break;
						case 3: dr.doorOpen = Door.doorState.Opening; break;
					}
				} else {
					index += 4;
				}
				break;
			case SaveObject.SaveableType.ForceBridge:
				ForceBridge fb = currentGameObject.GetComponent<ForceBridge>();
				if (fb != null) {
					fb.activated = GetBoolFromString(entries[index]); index++; // bool - is the bridge on?
					fb.lerping = GetBoolFromString(entries[index]); index++; // bool - are we currently lerping one way or tother
				} else {
					index += 2;
				}
				break;
			case SaveObject.SaveableType.Switch:
				ButtonSwitch bs = currentGameObject.GetComponent<ButtonSwitch>();
				if (bs != null) {
					// bs?  null?  that's bs
					bs.locked = GetBoolFromString(entries[index]); index++; // bool - is this switch locked
					bs.active = GetBoolFromString(entries[index]); index++; // bool - is the switch flashing?
					bs.alternateOn = GetBoolFromString(entries[index]); index++; // bool - is the flashing material on?
					bs.delayFinished = GetFloatFromString(entries[index],currentline); index++; // float - time before firing targets
				} else {
					index += 4;
				}
				break;
			case SaveObject.SaveableType.FuncWall:
				FuncWall fw = currentGameObject.GetComponent<FuncWall>(); // actually this is on movertarget gameObjects
				if (fw != null) {
					int state = GetIntFromString(entries[index],currentline,"savegame",index); index++;
					switch (state) {
						case 0: fw.currentState = FuncWall.FuncStates.Start; break;
						case 1: fw.currentState = FuncWall.FuncStates.Target; break;
						case 2: fw.currentState = FuncWall.FuncStates.MovingStart; break;
						case 3: fw.currentState = FuncWall.FuncStates.MovingTarget; break;
						case 4: fw.currentState = FuncWall.FuncStates.AjarMovingStart; break;
						case 5: fw.currentState = FuncWall.FuncStates.AjarMovingTarget; break;
					}
				} else {
					index++;
				}
				break;
			case SaveObject.SaveableType.TeleDest:
				TeleportTouch tt = currentGameObject.GetComponent<TeleportTouch>();
				if (tt != null) {
					tt.justUsed = GetFloatFromString(entries[index],currentline); index++; // bool - is the player still touching it?
				} else {
					index++;
				}
				break;
			case SaveObject.SaveableType.LBranch:
				LogicBranch lb = currentGameObject.GetComponent<LogicBranch>();
				if (lb != null) {
					lb.relayEnabled = GetBoolFromString(entries[index]); index++; // bool - is this enabled
					lb.onSecond = GetBoolFromString(entries[index]); index++; // bool - which one are we on?
				} else {
					index += 2;
				}
				break;
			case SaveObject.SaveableType.LRelay:
				LogicRelay lr = currentGameObject.GetComponent<LogicRelay>();
				if (lr != null) {
					lr.relayEnabled = GetBoolFromString(entries[index]); index++; // bool - is this enabled
				} else {
					index++;
				}
				break;
			case SaveObject.SaveableType.LSpawner:
				SpawnManager sm = currentGameObject.GetComponent<SpawnManager>();
				if (sm != null) {
					sm.active = GetBoolFromString(entries[index]); index++; // bool - is this enabled
					sm.numberActive = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int - number spawned
					sm.delayFinished = GetFloatFromString(entries[index],currentline); index++; // float - time that we need to spawn next
				} else {
					index += 3;
				}
				break;
			case SaveObject.SaveableType.InteractablePanel:
				InteractablePanel ip = currentGameObject.GetComponent<InteractablePanel>();
				if (ip != null) {
					ip.open = GetBoolFromString(entries[index]); index++; // bool - is the panel opened
					ip.installed = GetBoolFromString(entries[index]); index++; // bool - is the item installed, MAKE SURE YOU ENABLE THE INSTALL ITEM GameObject IN LOAD
				} else {
					index += 2;
				}
				break;
			case SaveObject.SaveableType.ElevatorPanel:
				KeypadElevator ke = currentGameObject.GetComponent<KeypadElevator>();
				if (ke != null) {
					ke.padInUse = GetBoolFromString(entries[index]); index++; // bool - is the pad being used by a player
					ke.locked = GetBoolFromString(entries[index]); index++; // bool - locked?
				} else {
					index += 2;
				}
				break;
			case SaveObject.SaveableType.Keypad:
				KeypadKeycode kk = currentGameObject.GetComponent<KeypadKeycode>();
				if (kk != null) {
					kk.padInUse = GetBoolFromString(entries[index]); index++; // bool - is the pad being used by a player
					kk.locked = GetBoolFromString(entries[index]); index++; // bool - locked?
					kk.solved = GetBoolFromString(entries[index]); index++; // bool - already entered correct keycode?
				} else {
					index += 3;
				}
				break;
			case SaveObject.SaveableType.PuzzleGrid:
				PuzzleGridPuzzle pgp = currentGameObject.GetComponent<PuzzleGridPuzzle>();
				if (pgp != null) {
					pgp.puzzleSolved = GetBoolFromString(entries[index]); index++; // bool - is this puzzle already solved?
					for (int i=0;i<pgp.grid.Length;i++) {
						pgp.grid[i] = GetBoolFromString(entries[index]); index++;  // bool - get the current grid states + or X
					}
					pgp.fired = GetBoolFromString(entries[index]); index++; // bool - have we already fired yet?
					pgp.locked = GetBoolFromString(entries[index]); index++; // bool - is this locked?
				} else {
					index += 38; // grid length is always 35
				}
				break;
			case SaveObject.SaveableType.PuzzleWire:
				PuzzleWirePuzzle pwp = currentGameObject.GetComponent<PuzzleWirePuzzle>();
				if (pwp != null) {
					pwp.puzzleSolved = GetBoolFromString(entries[index]); index++; // bool - is this puzzle already solved?
					for (int i=0;i<pwp.currentPositionsLeft.Length;i++) {
						pwp.currentPositionsLeft[i] = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int - get the current wire positions
					}
					for (int i=0;i<pwp.currentPositionsRight.Length;i++) {
						pwp.currentPositionsRight[i] = GetIntFromString(entries[index],currentline,"savegame",index); index++;  // int - get the current wire positions
					}
					pwp.locked = GetBoolFromString(entries[index]); index++; // bool - is this locked?
				} else {
					index += 16; // number of wire positions is always 7 for each side
				}
				break;
			case SaveObject.SaveableType.TCounter:
				TriggerCounter tc = currentGameObject.GetComponent<TriggerCounter>();
				if (tc != null) {
					tc.counter = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int - how many counts we have
				} else {
					index++;
				}
				break;
			case SaveObject.SaveableType.TGravity:
				GravityLift gl = currentGameObject.GetComponent<GravityLift>();
				if (gl != null) {
					gl.active = GetBoolFromString(entries[index]); index++; // bool - is this gravlift on?
				} else {
					index++;
				}
				break;
			case SaveObject.SaveableType.MChanger:
				MaterialChanger mch = currentGameObject.GetComponent<MaterialChanger>();
				if (mch != null) {
					mch.alreadyDone = GetBoolFromString(entries[index]); index++; // bool - is this gravlift on?
					// if (mch.alreadyDone) {
						// mch.SetMaterialFromCode(); // do it agains
					// }
				} else {
					index++;
				}
				break;
			case SaveObject.SaveableType.RadTrig:
				Radiation rad = currentGameObject.GetComponent<Radiation>();
				if (rad != null) {
					rad.isEnabled = GetBoolFromString(entries[index]); index++; // bool - hey is this on? hello?
					rad.numPlayers = GetIntFromString(entries[index],currentline,"savegame",index); index++; // int - how many players we are affecting
				} else {
					index += 2;
				}
				break;
			case SaveObject.SaveableType.GravPad:
				TextureChanger tex = currentGameObject.GetComponent<TextureChanger>();
				if (tex != null) {
					tex.currentTexture = GetBoolFromString(entries[index]); index++; // bool - is this gravlift on?
					tex.currentTexture = !tex.currentTexture; // gets done again in Toggle()
					tex.Toggle(); // set it again to be sure
				}
				break;
		}
	}

	public void Load(int saveFileIndex) {
		if (saveFileIndex < 0) {
			if (player1 != null) {
				Transform pCapT = player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
				pCapT.localPosition = new Vector3(-17.894f,-43.769f,17.898f);
				LoadNewGamePlayerDataToPlayer(player1);
			}
			if (player2 != null) {
				Transform pCapT = player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
				pCapT.localPosition = new Vector3(-16.819f,-43.769f,17.898f);
				LoadNewGamePlayerDataToPlayer(player1);
			}
			if (player3 != null) {
				Transform pCapT = player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
				pCapT.localPosition = new Vector3(-15.782f,-43.769f,17.898f);
				LoadNewGamePlayerDataToPlayer(player1);
			}
			if (player4 != null) {
				Transform pCapT = player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform;
				pCapT.localPosition = new Vector3(-14.944f,-43.769f,19.086f);
				LoadNewGamePlayerDataToPlayer(player1);
			}
			questData.ResetQuestData(questData);
			return; // TODO add default scene save data
		}
		string readline;
		int currentline = 0;
		int numPlayers = 0;
		List<GameObject> saveableGameObjects = new List<GameObject>();
		GameObject currentGameObject = null;

		sprint(stringTable[196],allPlayers); // Loading...
		if (player1 != null) numPlayers++;
		if (player2 != null) numPlayers++;
		if (player3 != null) numPlayers++;
		if (player4 != null) numPlayers++;
		// Find all gameobjects with SaveObject script attached
		GameObject[] getAllGameObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
		foreach (GameObject gob in getAllGameObjects) {
			if (gob.GetComponent<SaveObject>() != null) {
				if (gob.GetComponent<SaveObject>().isRuntimeObject == true) {
					saveableGameObjects.Add(gob);
				}
			}
		}

		Debug.Log("Num players: " + numPlayers.ToString(),allPlayers);
		Debug.Log("Num saveables: " + saveableGameObjects.Count.ToString(),allPlayers);

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
					int index = 0;
					if (currentline == 1) {
						// Read in global states
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

					// And now to load object specific data per the saveableType
					string stype = entries[index]; index++;
					int readID = GetIntFromString(entries[index],currentline,"savegame",index); index++;
					if (stype == "Player") {
						currentGameObject = null;
						if (player1 != null) {
							if (player1.GetComponent<SaveObject>().SaveID == readID) {
								currentGameObject = player1;
							}
						}
						if (player2 != null) {
							if (player2.GetComponent<SaveObject>().SaveID == readID) {
								currentGameObject = player2;
							}
						}
						if (player3 != null) {
							if (player3.GetComponent<SaveObject>().SaveID == readID) {
								currentGameObject = player3;
							}
						}
						if (player4 != null) {
							if (player4.GetComponent<SaveObject>().SaveID == readID) {
								currentGameObject = player4;
							}
						}
						if (currentGameObject != null) LoadPlayerDataToPlayer(currentGameObject,entries,currentline,index);
					} else {
						int levIndex = GetIntFromString(entries[index],currentline,"savegame",index); index++;
						bool foundMatch = false;
						for (int i=0;i<saveableGameObjects.Count;i++) {
							if (saveableGameObjects[i].GetComponent<SaveObject>().SaveID == readID) {
								currentGameObject = saveableGameObjects[i];
								foundMatch = true;
								break;
							}
						}
						if (!foundMatch) currentGameObject = new GameObject(); // create a new one
						if (currentGameObject != null) {
							if (levIndex > -1) currentGameObject.transform.parent = LevelManager.a.GetRequestedLevelDynamicContainer(levIndex).transform;
							LoadObjectDataToObject(currentGameObject,entries,currentline,index); // last parameter is 1, start on index 1 for loading the transform data
						}
					}
					currentline++;
				} while (!sr.EndOfStream);
				sr.Close();
			}
		}
		PauseScript.a.mainMenu.SetActive(false);
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
			if (healthObjectsRegistration[i] != null) {
				if (healthObjectsRegistration[i] == hm) {
					return; // already in the list
				}
			}
		}

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
			case 0: return AttackType.None;
			case 1: return AttackType.Melee;
			case 6: return AttackType.MeleeEnergy;
			case 2: return AttackType.EnergyBeam;
			case 3: return AttackType.Magnetic;
			case 4: return AttackType.Projectile;
			case 5: return AttackType.ProjectileEnergyBeam;
			case 7: return AttackType.ProjectileLaunched;
			case 8: return AttackType.Gas;
			case 9: return AttackType.ProjectileNeedle;
			case 10:return AttackType.Tranq;
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
		if (string.IsNullOrWhiteSpace(targetname)) {
			Debug.Log("WARNING: invalid targetname is null or white space!  Ignoring and returning from Const.UseTargets()");
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
					tio.Targetted(ud);
				} else {
					Debug.Log("WARNING: null TargetRegister GameObject linked to targetname of " + targetname + ". Could not run Targetted.");
				}
			}
		}
		Debug.Log("Ran Targetted() on " + numtargetsfound.ToString() + " GameObject(s) with targetname of " + targetname);
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

	public void ResetQuestData(QuestBits qD) {
		qD.lev1SecCode = UnityEngine.Random.Range(0,10);
		qD.lev2SecCode = UnityEngine.Random.Range(0,10);
		qD.lev3SecCode = UnityEngine.Random.Range(0,10);
		qD.lev4SecCode = UnityEngine.Random.Range(0,10);
		qD.lev5SecCode = UnityEngine.Random.Range(0,10);
		qD.lev6SecCode = UnityEngine.Random.Range(0,10);
		qD.RobotSpawnDeactivated = false;
		qD.IsotopeInstalled = false;
		qD.ShieldActivated = false;
		qD.LaserSafetyOverriden = false;
		qD.LaserDestroyed = false;
		qD.BetaGroveCyberUnlocked = false;
		qD.GroveAlphaJettisonEnabled = false;
		qD.GroveBetaJettisonEnabled = false;
		qD.GroveDeltaJettisonEnabled = false;
		qD.MasterJettisonBroken = false;
		qD.Relay428Fixed = false;
		qD.MasterJettisonEnabled = false;
		qD.BetaGroveJettisoned = false;
		qD.AntennaNorthDestroyed = false;
		qD.AntennaSouthDestroyed = false;
		qD.AntennaEastDestroyed = false;
		qD.AntennaWestDestroyed = false;
		qD.SelfDestructActivated = false;
		qD.BridgeSeparated = false;
		qD.IsolinearChipsetInstalled = false;
	}

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