using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Linq;
using System.Runtime.Serialization;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;
using System.Diagnostics;

// Global types
public enum Handedness : byte {Center,LH,RH};

public class Const : MonoBehaviour {
	//Item constants
	public QuestBits questData;
	public GameObject[] useableItems;
	public Texture2D[] useableItemsFrobIcons;
    public Sprite[] useableItemsIcons;
    public string[] useableItemsNameText;
	public Sprite[] searchItemIconSprites;

	//Audiolog constants
	[HideInInspector] public string[] audiologNames;
	[HideInInspector] public string[] audiologSenders;
	[HideInInspector] public string[] audiologSubjects;
	public AudioClip[] audioLogs;
	[HideInInspector] public int[] audioLogType;  // 0 = text only, 1 = normal, 2 = email, 3 = papers, 4 = vmail
	[HideInInspector] public string[] audioLogSpeech2Text;
	[HideInInspector] public int[] audioLogLevelFound;

	//Weapon constants
	public bool[] isFullAutoForWeapon;
	public float[] delayBetweenShotsForWeapon;
	public float[] delayBetweenShotsForWeapon2;
	public float[] damagePerHitForWeapon;
	public float[] damagePerHitForWeapon2;
	public float[] damageOverloadForWeapon;
	public float[] energyDrainLowForWeapon;
	public float[] energyDrainHiForWeapon;
	public float[] energyDrainOverloadForWeapon;
	public float[] penetrationForWeapon;
	public float[] penetrationForWeapon2;
	public float[] offenseForWeapon;
	public float[] offenseForWeapon2;
	public float[] rangeForWeapon;
	public int[] magazinePitchCountForWeapon;
	public int[] magazinePitchCountForWeapon2;
	public float[] recoilForWeapon;
	public enum AttackType : byte {None,Melee,MeleeEnergy,EnergyBeam,Magnetic,Projectile,ProjectileNeedle,ProjectileEnergyBeam,ProjectileLaunched,Gas,Tranq,Drill};
	public AttackType[] attackTypeForWeapon;
	public enum npcType : byte {Mutant,Supermutant,Robot,Cyborg,Supercyborg,Cyber,MutantCyborg};
	public enum PerceptionLevel : byte {Low,Medium,High,Omniscient};

	//NPC constants
	public GameObject[] npcPrefabs;
	[HideInInspector] public string[] nameForNPC;
	[HideInInspector] public AttackType[] attackTypeForNPC;
	[HideInInspector] public AttackType[] attackTypeForNPC2;
	[HideInInspector] public AttackType[] attackTypeForNPC3;
	[HideInInspector] public float[] damageForNPC; // Primary attack damage
	[HideInInspector] public float[] damageForNPC2; // Secondary attack damage
	[HideInInspector] public float[] damageForNPC3; // Grenade attack damage
	[HideInInspector] public float[] rangeForNPC; // Primary attack range
	[HideInInspector] public float[] rangeForNPC2; // Secondary attack range
	[HideInInspector] public float[] rangeForNPC3; // Grenade throw range
	[HideInInspector] public float[] healthForNPC;
	[HideInInspector] public float[] healthForCyberNPC;
	[HideInInspector] public PerceptionLevel[] perceptionForNPC;
	[HideInInspector] public float[] disruptabilityForNPC;
	[HideInInspector] public float[] armorvalueForNPC;
	[HideInInspector] public aiMoveType[] moveTypeForNPC;
	[HideInInspector] public float[] defenseForNPC;
	[HideInInspector] public float[] yawSpeedForNPC;
	[HideInInspector] public float[] fovForNPC;
	[HideInInspector] public float[] fovAttackForNPC;
	[HideInInspector] public float[] fovStartMovementForNPC;
	[HideInInspector] public float[] distToSeeBehindForNPC;
	[HideInInspector] public float[] sightRangeForNPC;
	[HideInInspector] public float[] walkSpeedForNPC;
	[HideInInspector] public float[] runSpeedForNPC;
	[HideInInspector] public float[] attack1SpeedForNPC;
	[HideInInspector] public float[] attack2SpeedForNPC;
	[HideInInspector] public float[] attack3SpeedForNPC;
	[HideInInspector] public float[] attack3ForceForNPC;
	[HideInInspector] public float[] attack3RadiusForNPC;
	[HideInInspector] public float[] timeToPainForNPC;
	[HideInInspector] public float[] timeBetweenPainForNPC;
	[HideInInspector] public float[] timeTillDeadForNPC;
	[HideInInspector] public float[] timeToActualAttack1ForNPC;
	[HideInInspector] public float[] timeToActualAttack2ForNPC;
	[HideInInspector] public float[] timeToActualAttack3ForNPC;
	[HideInInspector] public float[] timeBetweenAttack1ForNPC;
	[HideInInspector] public float[] timeBetweenAttack2ForNPC;
	[HideInInspector] public float[] timeBetweenAttack3ForNPC;
	[HideInInspector] public float[] timeToChangeEnemyForNPC;
	[HideInInspector] public float[] timeIdleSFXMinForNPC;
	[HideInInspector] public float[] timeIdleSFXMaxForNPC;
	[HideInInspector] public float[] timeAttack1WaitMinForNPC;
	[HideInInspector] public float[] timeAttack1WaitMaxForNPC;
	[HideInInspector] public float[] timeAttack1WaitChanceForNPC;
	[HideInInspector] public float[] timeAttack2WaitMinForNPC;
	[HideInInspector] public float[] timeAttack2WaitMaxForNPC;
	[HideInInspector] public float[] timeAttack2WaitChanceForNPC;
	[HideInInspector] public float[] timeAttack3WaitMinForNPC;
	[HideInInspector] public float[] timeAttack3WaitMaxForNPC;
	[HideInInspector] public float[] timeAttack3WaitChanceForNPC;
	[HideInInspector] public PoolType[] attack1ProjectileLaunchedTypeForNPC;
	[HideInInspector] public PoolType[] attack2ProjectileLaunchedTypeForNPC;
	[HideInInspector] public PoolType[] attack3ProjectileLaunchedTypeForNPC;
	[HideInInspector] public float[] projectileSpeedAttack1ForNPC;
	[HideInInspector] public float[] projectileSpeedAttack2ForNPC;
	[HideInInspector] public float[] projectileSpeedAttack3ForNPC;
	[HideInInspector] public bool[] hasLaserOnAttack1ForNPC;
	[HideInInspector] public bool[] hasLaserOnAttack2ForNPC;
	[HideInInspector] public bool[] hasLaserOnAttack3ForNPC;
	[HideInInspector] public bool[] explodeOnAttack3ForNPC;
	[HideInInspector] public bool[] preactivateMeleeCollidersForNPC;
	[HideInInspector] public float[] huntTimeForNPC;
	[HideInInspector] public float[] flightHeightForNPC;
	[HideInInspector] public bool[] flightHeightIsPercentageForNPC;
	[HideInInspector] public bool[] switchMaterialOnDeathForNPC;
	[HideInInspector] public float[] hearingRangeForNPC;
	[HideInInspector] public float[] timeForTranquilizationForNPC;
	[HideInInspector] public bool[] hopsOnMoveForNPC;

	// System constants
	[HideInInspector] public string[] creditsText;
	[HideInInspector] public HealthManager[] healthObjectsRegistration; // List of objects with health, used for fast application of damage in explosions
	[HideInInspector] public GameObject player1;
	[HideInInspector] public GameObject player2;
	[HideInInspector] public GameObject player3;
	[HideInInspector] public GameObject player4;

	// Layer masks
	[HideInInspector] public int layerMaskPlayerFrob;
	[HideInInspector] public int layerMaskPlayerAttack;
	[HideInInspector] public int layerMaskNPCSight;
	[HideInInspector] public int layerMaskNPCAttack;
	[HideInInspector] public int layerMaskNPCCollision;

	//Pool references
	public enum PoolType : byte {None,SparqImpacts,CameraExplosions,ProjEnemShot2,SparksSmall,BloodSpurtSmall,
						  BloodSpurtSmallYellow,BloodSpurtSmallGreen,SparksSmallBlue,HopperImpact,
						  GrenadeFragExplosions,Vaporize, BlasterImpacts, IonImpacts,
						  MagpulseShots, MagpulseImpacts,StungunShots, StungunImpacts, RailgunShots, RailgunImpacts,
						  PlasmaShots, PlasmaImpacts, ProjEnemShot6,ProjEnemShot6Impacts, ProjEnemShot2Impacts,
						  ProjSeedPods, ProjSeedPodsImpacts, TempAudioSources,GrenadeEMPExplosions, ProjEnemShot4,
						  ProjEnemShot4Impacts,CrateExplosions,GrenadeFragLive,CyborgAssassinThrowingStars,
						  ConcussionLive,EMPLive,GasLive,GasExplosions,CorpseHit,NPCMagpulseShots,NPCRailgunShots,
						  LeafBurst,MutationBurst,GraytationBurst,BarrelExplosions, CyberPlayerShots, CyberDogShots,
						  CyberReaverShots, BulletHoleLarge, BulletHoleScorchLarge, BulletHoleScorchSmall, BulletHoleSmall,
						  BulletHoleTiny, BulletHoleTinySpread, CyberPlayerIceShots, CyberDissolve, TargetIDInstances,
						  AutomapBotOverlays,AutomapCyborgOverlays,AutomapMutantOverlays, AutomapCameraOverlays};
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
	public GameObject Pool_CyberPlayerShots;
	public GameObject Pool_CyberDogShots;
	public GameObject Pool_CyberReaverShots;
	public GameObject Pool_BulletHoleLarge;
	public GameObject Pool_BulletHoleScorchLarge;
	public GameObject Pool_BulletHoleScorchSmall;
	public GameObject Pool_BulletHoleSmall;
	public GameObject Pool_BulletHoleTiny;
	public GameObject Pool_BulletHoleTinySpread;
	public GameObject Pool_CyberPlayerIceShots;
	public GameObject Pool_CyberDissolve;
	public GameObject Pool_TargetIDInstances;
	public GameObject Pool_AutomapBotOverlays;
	public GameObject Pool_AutomapCyborgOverlays;
	public GameObject Pool_AutomapMutantOverlays;
	public GameObject Pool_AutomapCameraOverlays;

	//Global object references
	public GameObject loadingScreen;
	public GameObject mainMenuInit; // Used to force mainMenuOn before Start() is called.
   
	//Config constants
	[HideInInspector] public int difficultyCombat;
	[HideInInspector] public int difficultyMission;
	[HideInInspector] public int difficultyPuzzle;
	[HideInInspector] public int difficultyCyber;
	[HideInInspector] public string playerName;
	public AudioSource mainmenuMusic;
	[HideInInspector] public int GraphicsResWidth;
	[HideInInspector] public int GraphicsResHeight;
	[HideInInspector] public bool GraphicsFullscreen;
	[HideInInspector] public bool GraphicsSSAO;
	[HideInInspector] public bool GraphicsBloom;
	[HideInInspector] public int GraphicsAAMode;
	[HideInInspector] public int GraphicsFOV;
	[HideInInspector] public int GraphicsGamma;
	[HideInInspector] public int AudioSpeakerMode;
	[HideInInspector] public bool AudioReverb;
	[HideInInspector] public int AudioVolumeMaster;
	[HideInInspector] public int AudioVolumeMusic;
	[HideInInspector] public int AudioVolumeMessage;
	[HideInInspector] public int AudioVolumeEffects;
	[HideInInspector] public int AudioLanguage;			// The language index. Used for choosing which text to display on-screen.
	[HideInInspector] public float MouseSensitivity;		// The responsiveness of the mouse. Used for scaling slow mice up and fast mice down.
	public int[] InputCodeSettings;		// The integer index values
	public string[] InputCodes;			// The readable mapping names used as labels on the configuration page
	public string[] InputValues;		// The list of all valid keys: letters, numbers, etc.
	public string[] InputConfigNames;	// The readable keys used as text representations on the configuration page for set values.
	[HideInInspector] public bool InputInvertLook;
	[HideInInspector] public bool InputInvertCyberspaceLook;
	[HideInInspector] public bool InputInvertInventoryCycling;
	[HideInInspector] public bool InputQuickItemPickup;
	[HideInInspector] public bool InputQuickReloadWeapons;

	public enum aiState : byte {Idle,Walk,Run,Attack1,Attack2,Attack3,Pain,Dying,Dead,Inspect,Interacting};
    public enum aiMoveType : byte {Walk,Fly,Swim,Cyber,None};

    public Font mainFont1; // Used to force Point filter mode.
	public Font mainFont2; // Used to force Point filter mode.
	/*[DTValidator.Optional] */public GameObject[] TargetRegister; // Doesn't need to be full, available space for maps and mods made by the community to use tons of objects
	public string[] TargetnameRegister;
    public string[] stringTable;
	public float[] reloadTime;

	public Material[] screenCodes;
	[HideInInspector] public int[] npcCount;
	public Sprite[] logImages;
	[HideInInspector] public int[] audioLogImagesRefIndicesLH;
	[HideInInspector] public int[] audioLogImagesRefIndicesRH;
	public GameObject eventSystem;
	public Texture[] sequenceTextures;
	public Text loadPercentText;

	// Irrelevant to inspector constants; automatically assigned during initialization or play.
	[HideInInspector] public string versionString = "v0.98"; // Global CITADEL PROJECT VERSION
	[HideInInspector] public bool gameFinished = false; // Global constants
	[HideInInspector] public float justSavedTimeStamp;
	[HideInInspector] public float savedReminderTime = 7f; // human short-term memory length
	[HideInInspector] public bool startingNewGame = false;
	[HideInInspector] public bool freshRun = false;
	[HideInInspector] public float doubleClickTime = 0.500f;
	[HideInInspector] public float frobDistance = 5f;
	[HideInInspector] public float elevatorPadUseDistance = 2f;
	[HideInInspector] public int creditsLength;
	[HideInInspector] public Transform player1TargettingPos;
	[HideInInspector] public GameObject player1Capsule;
	[HideInInspector] public PlayerMovement player1PlayerMovementScript;
	[HideInInspector] public PlayerHealth player1PlayerHealthScript;
	[HideInInspector] public GameObject player1CapsuleMainCameragGO;
	[HideInInspector] public PauseRigidbody[] prb;
	[HideInInspector] public float playerCameraOffsetY = 0.84f; //Vertical camera offset from player 0,0,0 position (mid-body)
	[HideInInspector] public Color ssYellowText = new Color(0.8902f, 0.8745f, 0f); // Yellow, e.g. for current inventory text
	[HideInInspector] public Color ssGreenText = new Color(0.3725f, 0.6549f, 0.1686f); // Green, e.g. for inventory text
	[HideInInspector] public Color ssRedText = new Color(0.9176f, 0.1373f, 0.1686f); // Red, e.g. for inventory text
	[HideInInspector] public Color ssWhiteText = new Color(1f, 1f, 1f); // White, e.g. for warnings text
	[HideInInspector] public Color ssOrangeText = new Color(1f, 0.498f, 0f); // Orange, e.g. for map buttons text
	[HideInInspector] public float camMaxAmount = 0.2548032f;
	[HideInInspector] public float mapWorldMaxN = 85.83999f;
	[HideInInspector] public float mapWorldMaxS = -78.00001f;
	[HideInInspector] public float mapWorldMaxE = -70.44f;
	[HideInInspector] public float mapWorldMaxW = 93.4f;
	[HideInInspector] public float mapTileMinX = 8; // top left corner
	[HideInInspector] public float mapTileMinY = -1016; // bottom right corner
	[HideInInspector] public bool decoyActive = false;
	[HideInInspector] public float berserkTime = 20f; //Patch constants
	[HideInInspector] public float detoxTime = 60f;
	[HideInInspector] public float geniusTime = 180f;
	[HideInInspector] public float mediTime = 35f;
	[HideInInspector] public float reflexTime = 155f;
	[HideInInspector] public float sightTime= 40f;
	[HideInInspector] public float sightSideEffectTime = 17f;
	[HideInInspector] public float staminupTime = 60f;
	[HideInInspector] public float reflexTimeScale = 0.25f;
	[HideInInspector] public float defaultTimeScale = 1.0f;
	[HideInInspector] public float berserkDamageMultiplier = 4.0f;
	[HideInInspector] public float nitroMinTime = 1.0f; //Grenade constants
	[HideInInspector] public float nitroMaxTime = 60.0f;
	[HideInInspector] public float nitroDefaultTime = 7.0f;
	[HideInInspector] public float earthShMinTime = 4.0f;
	[HideInInspector] public float earthShMaxTime = 60.0f;
	[HideInInspector] public float earthShDefaultTime = 10.0f;
	[HideInInspector] public float globalShakeDistance = 0.3f;
	[HideInInspector] public float globalShakeForce = 5f;
	[HideInInspector] public Quaternion quaternionIdentity;
	[HideInInspector] public Vector3 vectorZero;
	[HideInInspector] public Vector3 vectorOne;
	[HideInInspector] public int numberOfRaycastsThisFrame = 0;
	[HideInInspector] public int maxRaycastsPerFrame = 20;
	[HideInInspector] public float raycastTick = 0.2f;
	[HideInInspector] public float aiTickTime = 0.1f;

	// Private CONSTANTS
	private int TARGET_FPS = 60;
	private StringBuilder s1;
	private StringBuilder s2;
	private static string splitChar = "|";

	//Instance container variable
	public static Const a;

	void Awake() {
		Application.targetFrameRate = TARGET_FPS;
		a = this; // Create a new instance so that it can be accessed globally. MOST IMPORTANT PART!!
		a.justSavedTimeStamp = Time.time - a.savedReminderTime;
		if (a.player1 != null) {
			a.player1CapsuleMainCameragGO = a.player1.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera;
			a.player1TargettingPos = a.player1CapsuleMainCameragGO.transform;
			a.player1Capsule = a.player1.GetComponent<PlayerReferenceManager>().playerCapsule;
			a.player1PlayerMovementScript = a.player1Capsule.GetComponent<PlayerMovement>();
			a.player1PlayerHealthScript = a.player1Capsule.GetComponent<PlayerHealth>();
		}
		a.CheckIfNewGame();
		a.LoadTextForLanguage(0); // Initialize with US English (index 0)
		a.s1 = new StringBuilder();
		a.s2 = new StringBuilder();
		if (a.mainMenuInit != null) {
			if (!a.mainMenuInit.activeSelf) a.mainMenuInit.SetActive(true);
		}
		a.quaternionIdentity = Quaternion.identity;
		a.vectorZero = Vector3.zero;
		a.vectorOne = Vector3.one;
		a.LoadAudioLogMetaData();
		a.LoadItemNamesData();
		a.LoadDamageTablesData();
		a.LoadEnemyTablesData(); // Doing earlier since these are needed by AIController's Start().
	}

    public void LoadTextForLanguage(int lang) {
        string readline; // variable to hold each string read in from the file
        int currentline = 0;
        string sourceFile = "/StreamingAssets/text_english.txt";
        switch (lang) {
            case 0: sourceFile = "/StreamingAssets/text_english.txt"; break;
            case 1: sourceFile = "/StreamingAssets/text_espanol.txt"; break; // UPKEEP: support other languages
        }
        StreamReader dataReader = new StreamReader(Application.dataPath + sourceFile, Encoding.ASCII);
        using (dataReader) {
            do {
                // Read the next line
                readline = dataReader.ReadLine();
                if (currentline < stringTable.Length) {
                    stringTable[currentline] = readline;
				} else {
					UnityEngine.Debug.Log("WARNING: Ran out of slots in stringTable at " + currentline.ToString());
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
		layerMaskNPCSight = LayerMask.GetMask("Default","Geometry","Corpse","Door","InterDebris","PhysObjects","Player","Player2","Player3","Player4");
		layerMaskNPCAttack = LayerMask.GetMask("Default","Geometry","Corpse","Door","InterDebris","PhysObjects","Player","Player2","Player3","Player4");
		layerMaskNPCCollision = LayerMask.GetMask("Default","TransparentFX","IgnoreRaycast","Geometry","NPC","Door","InterDebris","Player","Clip","NPCClip","PhysObjects"); // Not including "Bullets" as this is merely used for spawning, not setting level-wide NPC collisions.
		layerMaskPlayerFrob = LayerMask.GetMask("Default","Geometry","Water","Corpse","Door","InterDebris","PhysObjects","Player2","Player3","Player4","NPC"); // Water is a hidden layer that prevents the player frobbing through gratings, X-doors, etc.  Oh and also water...if that were a thing.
		layerMaskPlayerAttack = LayerMask.GetMask("Default","Geometry","NPC","Bullets","Corpse","Door","InterDebris","PhysObjects","Player2","Player3","Player4");
		LoadCreditsData();
		StartCoroutine(InitializeEventSystem());
		questData = new QuestBits ();
		questData.lev1SecCode = UnityEngine.Random.Range(0,10); // Integer overload is maximum exclusive.  Confirmed maximum return value is 9.
		questData.lev2SecCode = UnityEngine.Random.Range(0,10);
		questData.lev3SecCode = UnityEngine.Random.Range(0,10);
		questData.lev4SecCode = UnityEngine.Random.Range(0,10);
		questData.lev5SecCode = UnityEngine.Random.Range(0,10);
		questData.lev6SecCode = UnityEngine.Random.Range(0,10);
		if (mainFont1 != null) mainFont1.material.mainTexture.filterMode = FilterMode.Point;
		if (mainFont2 != null) mainFont2.material.mainTexture.filterMode = FilterMode.Point;
		prb = FindObjectsOfType<PauseRigidbody>();
		//if (startingNewGame) {
		//	PauseScript.a.mainMenu.SetActive(false);
		//	loadingScreen.SetActive(false);
		//	MainMenuHandler.a.IntroVideo.SetActive(false);
		//	MainMenuHandler.a.IntroVideoContainer.SetActive(false);
		//	sprint(stringTable[197]); // Loading...Done!
		//	WriteDatForNewGame(false,false);
		//}
	}


	IEnumerator InitializeEventSystem () {
		yield return new WaitForSeconds(1f);
		if (eventSystem != null) eventSystem.SetActive(true);
	}

	private void LoadConfig() {
		// Graphics Configurations
		GraphicsResWidth = AssignConfigInt("Graphics","ResolutionWidth");
		GraphicsResHeight = AssignConfigInt("Graphics","ResolutionHeight");
		GraphicsFullscreen = AssignConfigBool("Graphics","Fullscreen");
		GraphicsSSAO = AssignConfigBool("Graphics","SSAO");
		GraphicsBloom = AssignConfigBool("Graphics","Bloom");
		GraphicsFOV = AssignConfigInt("Graphics","FOV");
		GraphicsAAMode = AssignConfigInt("Graphics","AA");
		GraphicsGamma = AssignConfigInt("Graphics","Gamma");

		// Audio Configurations
		AudioSpeakerMode = AssignConfigInt("Audio","SpeakerMode");
		AudioReverb = AssignConfigBool("Audio","Reverb");
		AudioVolumeMaster = AssignConfigInt("Audio","VolumeMaster");
		AudioVolumeMusic = AssignConfigInt("Audio","VolumeMusic");
		AudioVolumeMessage = AssignConfigInt("Audio","VolumeMessage");
		AudioVolumeEffects = AssignConfigInt("Audio","VolumeEffects");
		AudioLanguage = AssignConfigInt("Audio","Language");  // defaults to 0 = english

		MouseSensitivity = ((AssignConfigInt("Input","MouseSensitivity")/100f) * 2f) + 0.01f;;

		string inputCapture;
		// Input Configurations
		for (int i=0;i<40;i++) {
			inputCapture = INIWorker.IniReadValue("Input",InputCodes[i]);
			for (int j=0;j<159;j++) {
				if (InputValues[j] == inputCapture) InputCodeSettings[i] = j;
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
		INIWorker.IniWriteValue("Graphics","Fullscreen",BoolToString(GraphicsFullscreen));
		INIWorker.IniWriteValue("Graphics","SSAO",BoolToString(GraphicsSSAO));
		INIWorker.IniWriteValue("Graphics","Bloom",BoolToString(GraphicsBloom));
		INIWorker.IniWriteValue("Graphics","FOV",GraphicsFOV.ToString());
		INIWorker.IniWriteValue("Graphics","AA",GraphicsAAMode.ToString());
		INIWorker.IniWriteValue("Graphics","Gamma",GraphicsGamma.ToString());
		INIWorker.IniWriteValue("Audio","SpeakerMode",AudioSpeakerMode.ToString());
		INIWorker.IniWriteValue("Audio","Reverb",BoolToString(AudioReverb));
		INIWorker.IniWriteValue("Audio","VolumeMaster",AudioVolumeMaster.ToString());
		INIWorker.IniWriteValue("Audio","VolumeMusic",AudioVolumeMusic.ToString());
		INIWorker.IniWriteValue("Audio","VolumeMessage",AudioVolumeMessage.ToString());
		INIWorker.IniWriteValue("Audio","VolumeEffects",AudioVolumeEffects.ToString());
		INIWorker.IniWriteValue("Audio","Language",AudioLanguage.ToString());
		int ms = (int)(MouseSensitivity/2f*100f);
		INIWorker.IniWriteValue("Input","MouseSensitivity",ms.ToString());
		for (int i=0;i<40;i++) {
			INIWorker.IniWriteValue("Input",InputCodes[i],InputValues[InputCodeSettings[i]]);
		}
		INIWorker.IniWriteValue("Input","InvertLook",BoolToString(InputInvertLook));
		INIWorker.IniWriteValue("Input","InvertCyberspaceLook",BoolToString(InputInvertCyberspaceLook));
		INIWorker.IniWriteValue("Input","InvertInventoryCycling",BoolToString(InputInvertInventoryCycling));
		INIWorker.IniWriteValue("Input","QuickItemPickup",BoolToString(InputQuickItemPickup));
		INIWorker.IniWriteValue("Input","QuickReloadWeapons",BoolToString(InputQuickReloadWeapons));
		SetBloom();
		SetSSAO();
		SetAA();
	}

	private void LoadAudioLogMetaData () {
		// The following to be assigned to the arrays in the Unity Const data structure
		int readIndexOfLog, readLogImageLHIndex, readLogImageRHIndex; // look-up index for assigning the following data on the line in the file to the arrays
		string readLogText; // loaded into string audioLogSpeech2Text[]
		string readline; // variable to hold each string read in from the file
		int currentline = 0;
		char logSplitChar = ',';
		StreamReader dataReader = new StreamReader(Application.dataPath + "/StreamingAssets/logs_text.txt",Encoding.ASCII);
		using (dataReader) {
			do {
				int i = 0;
				// Read the next line
				readline = dataReader.ReadLine();
				if (readline == null) continue; // just in case
				string[] entries = readline.Split(logSplitChar);
				readIndexOfLog = GetIntFromString(entries[i],currentline); i++;
				readLogImageLHIndex = GetIntFromString(entries[i],currentline); i++;
				readLogImageRHIndex = GetIntFromString(entries[i],currentline); i++;
				audioLogImagesRefIndicesLH[readIndexOfLog] = readLogImageLHIndex;
				audioLogImagesRefIndicesRH[readIndexOfLog] = readLogImageRHIndex;
				audiologNames[readIndexOfLog] = entries[i]; i++;
				audiologSenders[readIndexOfLog] = entries[i]; i++;
				audiologSubjects[readIndexOfLog] = entries[i]; i++;
				audioLogType[readIndexOfLog] = GetIntFromString(entries[i],currentline); i++;
				audioLogLevelFound[readIndexOfLog] = GetIntFromString(entries[i],currentline); i++;
				readLogText = entries[i]; i++;
				// handle extra commas within the body text and append remaining portions of the line
				if (entries.Length > 8) {
					for (int j=9;j<entries.Length;j++) {
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
		int offset = 326;
		for (int i=0;i<102;i++) {
			useableItemsNameText[i] = stringTable[i+offset];
		}
	}

	private void LoadDamageTablesData () {
		string readline; // variable to hold each string read in from the file
		int currentline = 0;
		int readInt = 0;
		StreamReader dataReader = new StreamReader(Application.dataPath + "/StreamingAssets/damage_tables.txt",Encoding.ASCII);
		using (dataReader) {
			do {
				int i = 0;
				// Read the next line
				readline = dataReader.ReadLine();
				string[] entries = readline.Split(',');
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
				readInt = GetIntFromString(entries[i],currentline); i++;
				attackTypeForWeapon[currentline] = GetAttackTypeFromInt(readInt);
				currentline++;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}

	private void LoadCreditsData () {
		creditsText = new string[20];
		string readline; // variable to hold each string read in from the file
		int currentline = 0;
		int pagenum = 0;
		creditsLength = 1;
		StreamReader dataReader = new StreamReader(Application.dataPath + "/StreamingAssets/credits.txt",Encoding.ASCII);
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
				if (pagenum >= creditsText.Length) { UnityEngine.Debug.Log("pagenum was too large at " + pagenum.ToString()); return; }
                creditsText[pagenum] += readline + System.Environment.NewLine;
				currentline++;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}

	private void CheckIfNewGame () {
		string readline; // variable to hold each string read in from the file
		int currentline = 0;

		StreamReader dataReader = new StreamReader(Application.dataPath + "/StreamingAssets/ng.dat",Encoding.ASCII);
		using (dataReader) {
			do {
				// Read the next line
				readline = dataReader.ReadLine();
				if (currentline == 0) a.startingNewGame = GetBoolFromString(readline);
				if (currentline == 1) a.freshRun = GetBoolFromString(readline);
				currentline++;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}

	public void WriteDatForNewGame(bool bitStartingNew, bool bitFreshRun) {
		// Write bit to file
		StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/ng.dat",false,Encoding.ASCII);
		if (sw != null) {
			using (sw) {
				sw.WriteLine(BoolToString(bitStartingNew));
				sw.WriteLine(BoolToString(bitFreshRun));
				sw.Close();
			}
		}
		a.startingNewGame = bitStartingNew;
		a.freshRun = bitFreshRun;
	}

	private void LoadEnemyTablesData() {
		int numberOfNPCs = 29;
		nameForNPC = new string[numberOfNPCs];
		attackTypeForNPC = new AttackType[numberOfNPCs];
		attackTypeForNPC2 = new AttackType[numberOfNPCs];
		attackTypeForNPC3 = new AttackType[numberOfNPCs];
		damageForNPC = new float[numberOfNPCs];
		damageForNPC2 = new float[numberOfNPCs];
		damageForNPC3 = new float[numberOfNPCs];
		rangeForNPC = new float[numberOfNPCs];
		rangeForNPC2 = new float[numberOfNPCs];
		rangeForNPC3 = new float[numberOfNPCs];
		healthForNPC = new float[numberOfNPCs];
		healthForCyberNPC = new float[numberOfNPCs];
		perceptionForNPC = new PerceptionLevel[numberOfNPCs];
		disruptabilityForNPC = new float[numberOfNPCs];
		armorvalueForNPC = new float[numberOfNPCs];
		moveTypeForNPC = new aiMoveType[numberOfNPCs];
		defenseForNPC = new float[numberOfNPCs];
		yawSpeedForNPC = new float[numberOfNPCs];
		fovForNPC = new float[numberOfNPCs];
		fovAttackForNPC = new float[numberOfNPCs];
		fovStartMovementForNPC = new float[numberOfNPCs];
		distToSeeBehindForNPC = new float[numberOfNPCs];
		sightRangeForNPC = new float[numberOfNPCs];
		walkSpeedForNPC = new float[numberOfNPCs];
		runSpeedForNPC = new float[numberOfNPCs];
		attack1SpeedForNPC = new float[numberOfNPCs];
		attack2SpeedForNPC = new float[numberOfNPCs];
		attack3SpeedForNPC = new float[numberOfNPCs];
		attack3ForceForNPC = new float[numberOfNPCs];
		attack3RadiusForNPC = new float[numberOfNPCs];
		timeToPainForNPC = new float[numberOfNPCs];
		timeBetweenPainForNPC = new float[numberOfNPCs];
		timeTillDeadForNPC = new float[numberOfNPCs];
		timeToActualAttack1ForNPC = new float[numberOfNPCs];
		timeToActualAttack2ForNPC = new float[numberOfNPCs];
		timeToActualAttack3ForNPC = new float[numberOfNPCs];
		timeBetweenAttack1ForNPC = new float[numberOfNPCs];
		timeBetweenAttack2ForNPC = new float[numberOfNPCs];
		timeBetweenAttack3ForNPC = new float[numberOfNPCs];
		timeToChangeEnemyForNPC = new float[numberOfNPCs];
		timeIdleSFXMinForNPC = new float[numberOfNPCs];
		timeIdleSFXMaxForNPC = new float[numberOfNPCs];
		timeAttack1WaitMinForNPC = new float[numberOfNPCs];
		timeAttack1WaitMaxForNPC = new float[numberOfNPCs];
		timeAttack1WaitChanceForNPC = new float[numberOfNPCs];
		timeAttack2WaitMinForNPC = new float[numberOfNPCs];
		timeAttack2WaitMaxForNPC = new float[numberOfNPCs];
		timeAttack2WaitChanceForNPC = new float[numberOfNPCs];
		timeAttack3WaitMinForNPC = new float[numberOfNPCs];
		timeAttack3WaitMaxForNPC = new float[numberOfNPCs];
		timeAttack3WaitChanceForNPC = new float[numberOfNPCs];
		attack1ProjectileLaunchedTypeForNPC = new PoolType[numberOfNPCs];
		attack2ProjectileLaunchedTypeForNPC = new PoolType[numberOfNPCs];
		attack3ProjectileLaunchedTypeForNPC = new PoolType[numberOfNPCs];
		projectileSpeedAttack1ForNPC = new float[numberOfNPCs];
		projectileSpeedAttack2ForNPC = new float[numberOfNPCs];
		projectileSpeedAttack3ForNPC = new float[numberOfNPCs];
		hasLaserOnAttack1ForNPC = new bool[numberOfNPCs];
		hasLaserOnAttack2ForNPC = new bool[numberOfNPCs];
		hasLaserOnAttack3ForNPC = new bool[numberOfNPCs];
		explodeOnAttack3ForNPC = new bool[numberOfNPCs];
		preactivateMeleeCollidersForNPC = new bool[numberOfNPCs];
		huntTimeForNPC = new float[numberOfNPCs];
		flightHeightForNPC = new float[numberOfNPCs];
		flightHeightIsPercentageForNPC = new bool[numberOfNPCs];
		switchMaterialOnDeathForNPC = new bool[numberOfNPCs];
		hearingRangeForNPC = new float[numberOfNPCs];
		timeForTranquilizationForNPC = new float[numberOfNPCs];
		hopsOnMoveForNPC = new bool[numberOfNPCs];

		//case 1: return AttackType.Melee;
		//case 2: return AttackType.EnergyBeam;
		//case 3: return AttackType.Magnetic;
		//case 4: return AttackType.Projectile;
		//case 5: return AttackType.ProjectileEnergyBeam;

		//case 1: return PerceptionLevel.Medium;
		//case 2: return PerceptionLevel.High;
		//case 3: return PerceptionLevel.Omniscient;

		//case 0: return aiMoveType.None;
		//case 1: return aiMoveType.Walk;
		//case 2: return aiMoveType.Fly;
		//case 3: return aiMoveType.Swim;
		//case 4: return aiMoveType.Cyber;

		//case 0: return PoolType.None;
		//case 1: return PoolType.;
		//case 2: return PoolType.;
		//case 3: return PoolType.;
		//case 4: return PoolType.;

		string readline; // variable to hold each string read in from the file
		bool skippedFirstLine = false;
		int currentline = 1;
		int readInt = 0;
		int i = 0;
		int refIndex = 0;
		StreamReader dataReader = new StreamReader(Application.dataPath + "/StreamingAssets/enemy_tables.csv",Encoding.ASCII);
		using (dataReader) {
			do {
				i = 0;
				refIndex = 0;
				readline = dataReader.ReadLine(); // Read the next line
				if (!skippedFirstLine) {skippedFirstLine = true; continue;}
				string[] entries = readline.Split(',');
				char[] commentCheck = entries[i].ToCharArray();
				if (commentCheck[0] == '/' && commentCheck[1] == '/') continue; // Skip lines that start with '//'

				refIndex = GetIntFromString(entries[i+1],currentline); // Index is stored at 2nd spot
				if (refIndex < 0 || refIndex > 28) continue; // Invalid value, skip
				nameForNPC[refIndex] = entries[i].Trim(); i++;
				i++; // No need to read the index again so we skip over it.
				readInt = GetIntFromString(entries[i].Trim(),currentline); i++; attackTypeForNPC[refIndex] = GetAttackTypeFromInt(readInt);
				readInt = GetIntFromString(entries[i].Trim(),currentline); i++; attackTypeForNPC2[refIndex] = GetAttackTypeFromInt(readInt);
				readInt = GetIntFromString(entries[i].Trim(),currentline); i++; attackTypeForNPC3[refIndex] = GetAttackTypeFromInt(readInt);
				damageForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				damageForNPC2[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				damageForNPC3[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				rangeForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				rangeForNPC2[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				rangeForNPC3[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				healthForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				healthForCyberNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				readInt = GetIntFromString(entries[i].Trim(),currentline); i++;
				perceptionForNPC[refIndex] = GetPerceptionLevelFromInt(readInt);
				disruptabilityForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				armorvalueForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				moveTypeForNPC[refIndex] = GetMoveTypeFromInt(GetIntFromString(entries[i].Trim(),currentline)); i++;
				defenseForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				yawSpeedForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				fovForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				fovAttackForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				fovStartMovementForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				distToSeeBehindForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				sightRangeForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				walkSpeedForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				runSpeedForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				attack1SpeedForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				attack2SpeedForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				attack3SpeedForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				attack3ForceForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				attack3RadiusForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeToPainForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeBetweenPainForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeTillDeadForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeToActualAttack1ForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeToActualAttack2ForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeToActualAttack3ForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeBetweenAttack1ForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeBetweenAttack2ForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeBetweenAttack3ForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeToChangeEnemyForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeIdleSFXMinForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeIdleSFXMaxForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeAttack1WaitMinForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeAttack1WaitMaxForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeAttack1WaitChanceForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeAttack2WaitMinForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeAttack2WaitMaxForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeAttack2WaitChanceForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeAttack3WaitMinForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeAttack3WaitMaxForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeAttack3WaitChanceForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				readInt = GetIntFromString(entries[i].Trim(),currentline); i++; //attack1ProjectileLaunchedTypeForNPC[refIndex] = GetPoolType(readInt);
				readInt = GetIntFromString(entries[i].Trim(),currentline); i++; //attack2ProjectileLaunchedTypeForNPC[refIndex] = GetPoolType(readInt);
				readInt = GetIntFromString(entries[i].Trim(),currentline); i++; //attack3ProjectileLaunchedTypeForNPC[refIndex] = GetPoolType(readInt); // Not worrying about projectile type for now, would require indexing all of the pools.
				projectileSpeedAttack1ForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				projectileSpeedAttack2ForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				projectileSpeedAttack3ForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				hasLaserOnAttack1ForNPC[refIndex] = GetBoolFromString(entries[i].Trim()); i++;
				hasLaserOnAttack2ForNPC[refIndex] = GetBoolFromString(entries[i].Trim()); i++;
				hasLaserOnAttack3ForNPC[refIndex] = GetBoolFromString(entries[i].Trim()); i++;
				explodeOnAttack3ForNPC[refIndex] = GetBoolFromString(entries[i].Trim()); i++;
				preactivateMeleeCollidersForNPC[refIndex] = GetBoolFromString(entries[i].Trim()); i++;
				huntTimeForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				flightHeightForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				flightHeightIsPercentageForNPC[refIndex] = GetBoolFromString(entries[i].Trim()); i++;
				switchMaterialOnDeathForNPC[refIndex] = GetBoolFromString(entries[i].Trim()); i++;
				hearingRangeForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				timeForTranquilizationForNPC[refIndex] = GetFloatFromString(entries[i].Trim(),currentline); i++;
				hopsOnMoveForNPC[refIndex] = GetBoolFromString(entries[i].Trim()); i++;
				currentline++;
				if (currentline > 29) break;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}

	public static string GetTargetID(int npc23Index) {
		Const.a.npcCount[npc23Index]++;
		return Const.a.nameForNPC[npc23Index] + Const.a.npcCount[npc23Index].ToString();
	}

	public static string GetCyberTargetID (int cyberNPCIndex) {
		switch(cyberNPCIndex) {
			case 0: return Const.a.stringTable[499];
			case 1: return Const.a.stringTable[500];
			case 2: return Const.a.stringTable[501];
			case 3: return Const.a.stringTable[502];
		}
		return Const.a.stringTable[503];
	}

	public static void sprintByIndexOrOverride (int index, string overrideString, GameObject playerPassed) {
		if (string.IsNullOrWhiteSpace(overrideString)) {
			if (index >= 0) {
				sprint(Const.a.stringTable[index],playerPassed);
			}
		} else sprint(overrideString,playerPassed);
	}

	// StatusBar Print
	public static void sprint (string input, GameObject player) {
		if (player == null || a == null) return;

		#if UNITY_EDITOR
			UnityEngine.Debug.Log(input); // Print to Editor console.
		#endif

		if (player == null) {
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

	public static void sprint (string input) { Const.sprint(input,null); }

	public GameObject GetObjectFromPool(PoolType pool) {
		if (pool == PoolType.None) return null; // Do nothing, no pool requested.

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
		case PoolType.CyberPlayerShots:
			poolContainer = Pool_CyberPlayerShots;
			poolName = "CyberPlayerShots ";
			break;
		case PoolType.CyberDogShots:
			poolContainer = Pool_CyberDogShots;
			poolName = "CyberDogShots ";
			break;
		case PoolType.CyberReaverShots:
			poolContainer = Pool_CyberReaverShots;
			poolName = "CyberReaverShots ";
			break;
		case PoolType.BulletHoleLarge:
			poolContainer = Pool_BulletHoleLarge;
			poolName = "BulletHoleLarge ";
			break;
		case PoolType.BulletHoleScorchLarge:
			poolContainer = Pool_BulletHoleScorchLarge;
			poolName = "BulletHoleScorchLarge ";
			break;
		case PoolType.BulletHoleScorchSmall:
			poolContainer = Pool_BulletHoleScorchSmall;
			poolName = "BulletHoleScorchSmall ";
			break;
		case PoolType.BulletHoleSmall:
			poolContainer = Pool_BulletHoleSmall;
			poolName = "BulletHoleSmall ";
			break;
		case PoolType.BulletHoleTiny:
			poolContainer = Pool_BulletHoleTiny;
			poolName = "BulletHoleTiny ";
			break;
		case PoolType.BulletHoleTinySpread:
			poolContainer = Pool_BulletHoleTinySpread;
			poolName = "BulletHoleTinySpread ";
			break;
		case PoolType.CyberPlayerIceShots:
			poolContainer = Pool_CyberPlayerIceShots;
			poolName = "CyberPlayerIceShots ";
			break;
		case PoolType.CyberDissolve:
			poolContainer = Pool_CyberDissolve;
			poolName = "CyberDissolve ";
			break;
		case PoolType.TargetIDInstances:
			poolContainer = Pool_TargetIDInstances;
			poolName = "TargetIDInstances ";
			break;
		case PoolType.AutomapBotOverlays:
			poolContainer = Pool_AutomapBotOverlays;
			poolName = "AutomapBotOverlays ";
			break;
		case PoolType.AutomapCyborgOverlays:
			poolContainer = Pool_AutomapCyborgOverlays;
			poolName = "AutomapCyborgOverlays ";
			break;
		case PoolType.AutomapMutantOverlays:
			poolContainer = Pool_AutomapMutantOverlays;
			poolName = "AutomapMutantOverlays ";
			break;
		case PoolType.AutomapCameraOverlays:
			poolContainer = Pool_AutomapCameraOverlays;
			poolName = "AutomapCameraOverlays ";
			break;
        }

		if (poolContainer == null) { UnityEngine.Debug.Log("Cannot find " + poolName + "pool"); return null; }
		for (int i=0;i<poolContainer.transform.childCount;i++) {
			Transform child = poolContainer.transform.GetChild(i);
			if (child.gameObject.activeInHierarchy == false) return child.gameObject;
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
	// 0. First checks against whether the entity is damageable (i.e. not the world) - handled by Physics Layers.
	// 1. Armor Absorption (see ICE Breaker Guide for all of 4 these)
	// 2. Weapon Vulnerabilities based on attack type and the a_att_type bits stored in the npc
	// 3. Critical Hits, chance for critical hit damage based on defense and offense of attack and target
	// 4. Random Factor, +/- 10% damage for randomness
	// 5. Apply Velocity for damage, this is after all the above because otherwise the damage multipliers wouldn't affect velocity
	// 6. Berserk Damage Increase
	// 7. Return the damage to original TakeDamage() function
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

		float take, crit;
		take = (o_armorvalue > a_penetration ? a_damage - a_penetration : a_damage); // 1. Armor Absorption (NPC armor, not player)
		// 2. Weapon Vulnerabilities - handled by HealthManager.
		// 3. Critical Hits (NPCs only)
		if (o_isnpc) {
			crit = (a_offense - o_defense);
			if (crit > 0) {
				// 71% success with 5/6  5 = f, 6 = max offense or defense value
				// 62% success with 4/6
				// 50% success with 3/6
				// 24% success with 2/6
				// 10% success with 1/6
				// chance of f/6 = 5/6|4/6|3/6|2/6|1/6 = .833|.666|.5|.333|.166
				if ((UnityEngine.Random.Range(0f,1f) < (crit/6)) && (UnityEngine.Random.Range(0f,1f) < 0.2f)) take += crit * take; // SUCCESS! Maximum extra is 5X + 1X Damage.
			}
		}
		take *= UnityEngine.Random.Range(0.9f,1.1f); // 4. Random Factor +/- 10% (aka 0.10 damage).
		// 5. Apply Impact Velocity for Damage - handled by HealthManager.
		if (a_berserk) take *= Const.a.berserkDamageMultiplier; // 6. Berserk Damage Increase.
		return take; // 7. Return the Damage.
	}

	string SavePlayerData(GameObject plyr) {
		if (plyr == null) return "!";

		string line = System.String.Empty;
		Transform tr, trml;
		int j = 0;
		// Get all references to relevant components.
		PlayerReferenceManager PRman = plyr.GetComponent<PlayerReferenceManager>();
		GameObject pCap = PRman.playerCapsule;
		GameObject playerMainCamera = PRman.playerCapsuleMainCamera;
		GameObject playerInventory = PRman.playerInventory;
		PlayerHealth ph = pCap.GetComponent<PlayerHealth>();
		PlayerEnergy pe = pCap.GetComponent<PlayerEnergy>();
		PlayerMovement pm = pCap.GetComponent<PlayerMovement>();
		PlayerPatch pp = pCap.GetComponent<PlayerPatch>();
		HealthManager hm = pCap.GetComponent<HealthManager>();
		tr = pCap.transform;
		MouseLookScript ml = playerMainCamera.GetComponent<MouseLookScript>();
		trml = playerMainCamera.transform;
		WeaponCurrent wc = playerInventory.GetComponent<WeaponCurrent>();
		WeaponFire wf = pe.wepFire;
		Inventory inv = playerInventory.GetComponent<Inventory>();
		MFDManager mfd = MFDManager.a;
		SaveObject sav = plyr.GetComponent<SaveObject>();

		line = sav.saveableType;
		line += splitChar + sav.SaveID.ToString();
		string pname = string.IsNullOrEmpty(Const.a.playerName) ? "Hacker" : Const.a.playerName;
		line += splitChar + pname;
		line += splitChar + ph.radiated.ToString("0000.00000"); // float
		line += splitChar + ph.timer.ToString("0000.00000"); // float
		line += splitChar + BoolToString(ph.playerDead); // bool
		line += splitChar + BoolToString(ph.radiationArea); // bool
		line += splitChar + ph.mediPatchPulseFinished.ToString("0000.00000"); // float
		line += splitChar + ph.mediPatchPulseCount.ToString(); // int
		line += splitChar + BoolToString(ph.makingNoise); // bool
		line += splitChar + ph.lastHealth.ToString("0000.00000"); // float
		line += splitChar + ph.painSoundFinished.ToString("0000.00000"); // float
		line += splitChar + ph.radSoundFinished.ToString("0000.00000"); // float
		line += splitChar + ph.radFXFinished.ToString("0000.00000"); // float
		line += splitChar + pe.energy.ToString("0000.00000"); // float
		line += splitChar + pe.timer.ToString("0000.00000"); // float
		line += splitChar + pe.tickFinished.ToString("0000.00000"); // float
		line += splitChar + pm.playerSpeed.ToString("0000.00000"); // float
		line += splitChar + BoolToString(pm.grounded); // bool
		line += splitChar + pm.currentCrouchRatio.ToString("0000.00000"); // float
		line += splitChar + pm.bodyState.ToString(); // int
		line += splitChar + BoolToString(pm.ladderState); // bool
		line += splitChar + BoolToString(pm.gravliftState); // bool
		line += splitChar + BoolToString(pm.inCyberSpace); // bool
		for (j=0;j<4096;j++) { line += splitChar + BoolToString(pm.automapExploredR[j]); } // bool
		for (j=0;j<4096;j++) { line += splitChar + BoolToString(pm.automapExplored1[j]); } // bool
		for (j=0;j<4096;j++) { line += splitChar + BoolToString(pm.automapExplored2[j]); } // bool
		for (j=0;j<4096;j++) { line += splitChar + BoolToString(pm.automapExplored3[j]); } // bool
		for (j=0;j<4096;j++) { line += splitChar + BoolToString(pm.automapExplored4[j]); } // bool
		for (j=0;j<4096;j++) { line += splitChar + BoolToString(pm.automapExplored5[j]); } // bool
		for (j=0;j<4096;j++) { line += splitChar + BoolToString(pm.automapExplored6[j]); } // bool
		for (j=0;j<4096;j++) { line += splitChar + BoolToString(pm.automapExplored7[j]); } // bool
		for (j=0;j<4096;j++) { line += splitChar + BoolToString(pm.automapExplored8[j]); } // bool
		for (j=0;j<4096;j++) { line += splitChar + BoolToString(pm.automapExplored9[j]); } // bool
		for (j=0;j<4096;j++) { line += splitChar + BoolToString(pm.automapExploredG1[j]); } // bool
		for (j=0;j<4096;j++) { line += splitChar + BoolToString(pm.automapExploredG2[j]); } // bool
		for (j=0;j<4096;j++) { line += splitChar + BoolToString(pm.automapExploredG4[j]); } // bool
		line += splitChar + BoolToString(pm.CheatWallSticky); // bool
		line += splitChar + BoolToString(pm.CheatNoclip); // bool
		line += splitChar + pm.jumpTime.ToString("0000.00000"); // float
		line += splitChar + (pm.oldVelocity.x.ToString("0000.00000") + splitChar + pm.oldVelocity.y.ToString("0000.00000") + splitChar + pm.oldVelocity.z.ToString("0000.00000")); // Vector3 (float|float|float)
		line += splitChar + pm.fatigue.ToString("0000.00000"); // float
		line += splitChar + BoolToString(pm.justJumped); // bool
		line += splitChar + pm.fatigueFinished.ToString("0000.00000"); // float
		line += splitChar + pm.fatigueFinished2.ToString("0000.00000"); // float
		line += splitChar + BoolToString(pm.cyberSetup); // bool
		line += splitChar + BoolToString(pm.cyberDesetup); // bool
		line += splitChar + pm.oldBodyState.ToString(); // int
		line += splitChar + pm.leanTarget.ToString("0000.00000"); // float
		line += splitChar + pm.leanShift.ToString("0000.00000"); // float
		line += splitChar + pm.jumpSFXFinished.ToString("0000.00000"); // float
		line += splitChar + pm.jumpLandSoundFinished.ToString("0000.00000"); // float
		line += splitChar + pm.jumpJetEnergySuckTickFinished.ToString("0000.00000"); // float
		line += splitChar + BoolToString(pm.fatigueWarned); // bool
		line += splitChar + pm.ressurectingFinished.ToString("0000.00000"); // float
		line += splitChar + pm.doubleJumpFinished.ToString("0000.00000"); // float
		line += splitChar + pm.turboFinished.ToString("0000.00000"); // float
		line += splitChar + pp.berserkFinishedTime.ToString("0000.00000"); // float
		line += splitChar + pp.berserkIncrementFinishedTime.ToString("0000.00000"); // float
		line += splitChar + pp.detoxFinishedTime.ToString("0000.00000"); // float
		line += splitChar + pp.geniusFinishedTime.ToString("0000.00000"); // float
		line += splitChar + pp.mediFinishedTime.ToString("0000.00000"); // float
		line += splitChar + pp.reflexFinishedTime.ToString("0000.00000"); // float
		line += splitChar + pp.sightFinishedTime.ToString("0000.00000"); // float
		line += splitChar + pp.sightSideEffectFinishedTime.ToString("0000.00000"); // float
		line += splitChar + pp.staminupFinishedTime.ToString("0000.00000"); // float
		line += splitChar + pp.berserkIncrement.ToString(); // int
		line += splitChar + pp.patchActive.ToString(); // int
		line += splitChar + (tr.localPosition.x.ToString("0000.00000") + splitChar + tr.localPosition.y.ToString("0000.00000") + splitChar + tr.localPosition.z.ToString("0000.00000")); // Vector3 (float|float|float)
		line += splitChar + (tr.localRotation.x.ToString("0000.00000") + splitChar + tr.localRotation.y.ToString("0000.00000") + splitChar + tr.localRotation.z.ToString("0000.00000") + splitChar + tr.localRotation.w.ToString("0000.00000")); // Quaternion (float|float|float|float)
		line += splitChar + (tr.localScale.x.ToString("0000.00000") + splitChar + tr.localScale.y.ToString("0000.00000") + splitChar + tr.localScale.z.ToString("0000.00000")); // Vector3 (float|float|float)
		line += splitChar + (trml.localPosition.x.ToString("0000.00000") + splitChar + trml.localPosition.y.ToString("0000.00000") + splitChar + trml.localPosition.z.ToString("0000.00000")); // Vector3 (float|float|float)
		line += splitChar + (trml.localRotation.x.ToString("0000.00000") + splitChar + trml.localRotation.y.ToString("0000.00000") + splitChar + trml.localRotation.z.ToString("0000.00000") + splitChar + trml.localRotation.w.ToString("0000.00000")); // Quaternion (float|float|float|float)
		line += splitChar + (trml.localScale.x.ToString("0000.00000") + splitChar + trml.localScale.y.ToString("0000.00000") + splitChar + trml.localScale.z.ToString("0000.00000")); // Vector3 (float|float|float)
		line += splitChar + BoolToString(ml.inventoryMode); // bool
		line += splitChar + BoolToString(ml.holdingObject); // bool
		line += splitChar + ml.heldObjectIndex.ToString(); // int
		line += splitChar + ml.heldObjectCustomIndex.ToString(); // int
		line += splitChar + ml.heldObjectAmmo.ToString(); // int
		line += splitChar + ml.heldObjectAmmo2.ToString(); // int
		line += splitChar + BoolToString(ml.firstTimePickup); // bool
		line += splitChar + BoolToString(ml.firstTimeSearch); // bool
		line += splitChar + BoolToString(ml.grenadeActive); // bool
		line += splitChar + BoolToString(ml.inCyberSpace); // bool
		line += splitChar + ml.yRotation.ToString("0000.00000"); // float
		line += splitChar + BoolToString(ml.geniusActive); // bool
		line += splitChar + ml.xRotation.ToString("0000.00000"); // float
		line += splitChar + BoolToString(ml.vmailActive); // bool
		line += splitChar + (ml.cyberspaceReturnPoint.x.ToString("0000.00000") + splitChar + ml.cyberspaceReturnPoint.y.ToString("0000.00000") + splitChar + ml.cyberspaceReturnPoint.z.ToString("0000.00000"));
		line += splitChar + (ml.cyberspaceReturnCameraLocalRotation.x.ToString("0000.00000") + splitChar + ml.cyberspaceReturnCameraLocalRotation.y.ToString("0000.00000") + splitChar + ml.cyberspaceReturnCameraLocalRotation.z.ToString("0000.00000"));
		line += splitChar + (ml.cyberspaceReturnPlayerCapsuleLocalRotation.x.ToString("0000.00000") + splitChar + ml.cyberspaceReturnPlayerCapsuleLocalRotation.y.ToString("0000.00000") + splitChar + ml.cyberspaceReturnPlayerCapsuleLocalRotation.z.ToString("0000.00000"));
		line += splitChar + (ml.cyberspaceRecallPoint.x.ToString("0000.00000") + splitChar + ml.cyberspaceRecallPoint.y.ToString("0000.00000") + splitChar + ml.cyberspaceRecallPoint.z.ToString("0000.00000"));
		line += splitChar + ml.cyberspaceReturnLevel.ToString(); // int
		line += splitChar + hm.health.ToString("0000.00000"); // float
		line += splitChar + hm.cyberHealth.ToString("0000.00000"); // float
		line += splitChar + BoolToString(hm.deathDone); // bool
		line += splitChar + BoolToString(hm.god); // bool
		line += splitChar + BoolToString(hm.teleportDone); // bool
		line += splitChar + GUIState.a.overButtonType.ToString(); // int
		line += splitChar + BoolToString(GUIState.a.overButton); // bool
		for (j=0;j<7;j++) { line += splitChar + inv.weaponInventoryIndices[j].ToString(); } // int
		for (j=0;j<7;j++) { line += splitChar + inv.weaponInventoryAmmoIndices[j].ToString(); } // int
		line += splitChar + inv.numweapons.ToString(); // int
		for (j=0;j<16;j++) { line += splitChar + inv.wepAmmo[j].ToString(); } // int
		for (j=0;j<16;j++) { line += splitChar + inv.wepAmmoSecondary[j].ToString(); } // int
		for (j=0;j<7;j++) { line += splitChar + inv.currentEnergyWeaponHeat[j].ToString("0000.00000"); } // float
		for (j=0;j<7;j++) { line += splitChar + BoolToString(inv.wepLoadedWithAlternate[j]); } // bool
		line += splitChar + wc.weaponCurrent.ToString(); // int
		line += splitChar + wc.weaponIndex.ToString(); // int
		for (j=0;j<7;j++) { line += splitChar + wc.weaponEnergySetting[j].ToString("0000.00000"); } // float
		for (j=0;j<7;j++) { line += splitChar + wc.currentMagazineAmount[j].ToString(); } // int
		for (j=0;j<7;j++) { line += splitChar + wc.currentMagazineAmount2[j].ToString(); } // int
		line += splitChar + BoolToString(wc.justChangedWeap); // bool
		line += splitChar + wc.lastIndex.ToString(); // int
		line += splitChar + BoolToString(wc.bottomless); // bool
		line += splitChar + BoolToString(wc.redbull); // bool
		line += splitChar + wc.reloadFinished.ToString("0000.00000"); // float
		line += splitChar + wc.reloadLerpValue.ToString("0000.00000"); // float
		line += splitChar + wc.lerpStartTime.ToString("0000.00000"); // float
		line += splitChar + wc.targetY.ToString("0000.00000"); // float
		line += splitChar + wf.waitTilNextFire.ToString("0000.00000"); // float
		line += splitChar + BoolToString(wf.overloadEnabled); // bool
		line += splitChar + wf.sparqSetting.ToString("0000.00000"); // float
		line += splitChar + wf.ionSetting.ToString("0000.00000"); // float
		line += splitChar + wf.blasterSetting.ToString("0000.00000"); // float
		line += splitChar + wf.plasmaSetting.ToString("0000.00000"); // float
		line += splitChar + wf.stungunSetting.ToString("0000.00000"); // float
		line += splitChar + BoolToString(wf.recoiling); // bool
		line += splitChar + wf.justFired.ToString("0000.00000"); // float
		line += splitChar + wf.energySliderClickedTime.ToString("0000.00000"); // float
		line += splitChar + wf.cyberWeaponAttackFinished.ToString("0000.00000"); // float
		line += splitChar + inv.grenadeCurrent.ToString(); // int
		line += splitChar + inv.grenadeIndex.ToString(); // int
		line += splitChar + inv.nitroTimeSetting.ToString("0000.00000"); // float
		line += splitChar + inv.earthShakerTimeSetting.ToString("0000.00000"); // float
		for (j=0;j<7;j++) { line += splitChar + inv.grenAmmo[j].ToString(); } // int
		line += splitChar + inv.patchCurrent.ToString(); // int
		line += splitChar + inv.patchIndex.ToString(); // int
		for (j=0;j<7;j++) { line += splitChar + inv.patchCounts[j].ToString(); } // int
		for (j=0;j<134;j++) { line += splitChar + BoolToString(inv.hasLog[j]); } // bool
		for (j=0;j<134;j++) { line += splitChar + BoolToString(inv.readLog[j]); } // bool
		for (j=0;j<10;j++) { line += splitChar + inv.numLogsFromLevel[j].ToString(); } // int
		line += splitChar + inv.lastAddedIndex.ToString(); // int
		line += splitChar + BoolToString(inv.beepDone); // bool
		for (j=0;j<13;j++) { line += splitChar + BoolToString(inv.hasHardware[j]); } // bool
		for (j=0;j<13;j++) { line += splitChar + inv.hardwareVersion[j].ToString(); } // int
		for (j=0;j<13;j++) { line += splitChar + inv.hardwareVersionSetting[j].ToString(); } // int
		line += splitChar + inv.hardwareInvCurrent.ToString(); // int
		line += splitChar + inv.hardwareInvIndex.ToString(); // int
		for (j=0;j<13;j++) { line += splitChar + BoolToString(inv.hardwareIsActive[j]); } // bool
		for (j=0;j<32;j++) {
			switch (inv.accessCardsOwned[j]) {
				case Door.accessCardType.None: line+= splitChar + "0"; break;
				case Door.accessCardType.Standard: line+= splitChar + "1"; break;
				case Door.accessCardType.Medical: line+= splitChar + "2"; break;
				case Door.accessCardType.Science: line+= splitChar + "3"; break;
				case Door.accessCardType.Admin: line+= splitChar + "4"; break;
				case Door.accessCardType.Group1: line+= splitChar + "5"; break;
				case Door.accessCardType.Group2: line+= splitChar + "6"; break;
				case Door.accessCardType.Group3: line+= splitChar + "7"; break;
				case Door.accessCardType.Group4: line+= splitChar + "8"; break;
				case Door.accessCardType.GroupA: line+= splitChar + "9"; break;
				case Door.accessCardType.GroupB: line+= splitChar + "10"; break;
				case Door.accessCardType.Storage: line+= splitChar + "11"; break;
				case Door.accessCardType.Engineering: line+= splitChar + "12"; break;
				case Door.accessCardType.Maintenance: line+= splitChar + "13"; break;
				case Door.accessCardType.Security: line+= splitChar + "14"; break;
				case Door.accessCardType.Per1: line+= splitChar + "15"; break;
				case Door.accessCardType.Per2: line+= splitChar + "16"; break;
				case Door.accessCardType.Per3: line+= splitChar + "17"; break;
				case Door.accessCardType.Per4: line+= splitChar + "18"; break;
				case Door.accessCardType.Per5: line+= splitChar + "19"; break;
				case Door.accessCardType.Command: line+= splitChar + "20"; break;
			}
		}
		for (j=0;j<14;j++) { line += splitChar + inv.generalInventoryIndexRef[j].ToString(); } // int
		line += splitChar + inv.generalInvCurrent.ToString(); // int
		line += splitChar + inv.generalInvIndex.ToString(); // int
		line += splitChar + inv.currentCyberItem.ToString(); // int
		line += splitChar + BoolToString(inv.isPulserNotDrill); // bool
		for (j=0;j<7;j++) { line += splitChar + inv.softVersions[j].ToString(); } // int 
		for (j=0;j<7;j++) { line += splitChar + BoolToString(inv.hasSoft[j]); } // bool
		line += splitChar + inv.emailCurrent.ToString(); // int
		line += splitChar + inv.emailIndex.ToString(); // int
		line += splitChar + BoolToString(mfd.lastWeaponSideRH); // bool
		line += splitChar + BoolToString(mfd.lastItemSideRH); // bool
		line += splitChar + BoolToString(mfd.lastAutomapSideRH); // bool
		line += splitChar + BoolToString(mfd.lastTargetSideRH); // bool
		line += splitChar + BoolToString(mfd.lastDataSideRH); // bool
		line += splitChar + BoolToString(mfd.lastSearchSideRH); // bool
		line += splitChar + BoolToString(mfd.lastLogSideRH); // bool
		line += splitChar + BoolToString(mfd.lastLogSecondarySideRH); // bool
		line += splitChar + BoolToString(mfd.lastMinigameSideRH); // bool
		line += splitChar + mfd.objectInUsePos.x.ToString("0000.00000") + splitChar + mfd.objectInUsePos.y.ToString("0000.00000") + splitChar + mfd.objectInUsePos.z.ToString("0000.00000");
		// tetheredPGP
		// tetheredPWP
		// tetheredSearchable
		// tetheredKeypadElevator
		// tetheredKeypadKeycode
		line += splitChar + BoolToString(mfd.paperLogInUse); // bool
		line += splitChar + BoolToString(mfd.usingObject); // bool
		line += splitChar + BoolToString(mfd.logReaderContainer.activeSelf); // bool
		line += splitChar + BoolToString(mfd.DataReaderContentTab.activeSelf); // bool
		line += splitChar + BoolToString(mfd.logTable.activeSelf); // bool
		line += splitChar + BoolToString(mfd.logLevelsFolder.activeSelf); // bool
		line += splitChar + mfd.logFinished.ToString("0000.00000");
		line += splitChar + BoolToString(mfd.logActive); // bool
		line += splitChar + mfd.logType.ToString(); // int
		line += splitChar + mfd.cyberTimer.GetComponent<CyberTimer>().timerFinished.ToString("0000.00000");
		return line;
	}

	// For ammo on the weapons
	string SaveUseableData(GameObject go) {
		int lineSlotsCount_SaveUseableData = 0;
		string line = System.String.Empty;
		UseableObjectUse uou = go.GetComponent<UseableObjectUse>();
		if (uou != null) {
			line = uou.useableItemIndex.ToString(); lineSlotsCount_SaveUseableData++; // int - the main lookup index, needed for intanciating on load if doesn't match original SaveID
			line += splitChar + uou.customIndex.ToString(); lineSlotsCount_SaveUseableData++; // int - special reference like audiolog message
			line += splitChar + uou.ammo.ToString(); lineSlotsCount_SaveUseableData++; // int - how much normal ammo is on the weapon
			line += splitChar + uou.ammo2.ToString(); lineSlotsCount_SaveUseableData++; //int - alternate ammo type, e.g. Penetrator or Teflon
		} else {
			UnityEngine.Debug.Log("UseableObjectUse missing on savetype of UseableObjectUse!");
			line = "-1|-1|0|0|BUG: Missing UseableObjectUse";
		}
		//4
		//UnityEngine.Debug.Log("lineSlotsCount_SaveUseableData: " + lineSlotsCount_SaveUseableData.ToString());
		return line;
	}

	// Live grenades - These should only be up in the air or active running timer, but still...or it's a landmine
	string SaveGrenadeData(GameObject go) {
		string line = System.String.Empty;
		GrenadeActivate ga = go.GetComponent<GrenadeActivate>();
		if (ga != null) {
			line = ga.constIndex.ToString(); // int - lookup index to the const items table for instantiating
			line += splitChar + BoolToString(ga.useTimer); // bool - do we have a timer going? MAKE SURE YOU CHECK THIS BIT IN LOAD!
			line += splitChar + ga.timeFinished.ToString("0000.00000"); // float - how much time left before the fun part?
			line += splitChar + BoolToString(ga.explodeOnContact); // bool - or not a landmine
			line += splitChar + BoolToString(ga.useProx); // bool - is this a landmine?
		} else {
			UnityEngine.Debug.Log("GrenadeActivate missing on savetype of GrenadeActivate!");
			line = "-1|0|0000.00000|0|0";
		}
		return line;
	}

	// Generic health info string
	string GetHealthManagerSaveData(HealthManager hm) {
		if (!hm.awakeInitialized) hm.Awake();
		if (!hm.startInitialized) hm.Start();
		string line = System.String.Empty;
		if (hm != null) {
			line = hm.health.ToString("0000.00000"); // how much health we have
			line += splitChar + hm.cyberHealth.ToString("0000.00000"); // how much health we have
			line += splitChar + BoolToString(hm.deathDone); // bool - are we dead yet?
			line += splitChar + BoolToString(hm.god); // are we invincible? - we can save cheats?? OH WOW!
			line += splitChar + BoolToString(hm.teleportDone); // did we already teleport?
		} else {
			UnityEngine.Debug.Log("HealthManager missing on savetype of HealthManager!");
			line = "0000.00000|0000.00000|0|0|0";
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
		s2.Clear();
		if (go == null) { UnityEngine.Debug.Log("BUG: attempting to SaveNPCData for null GameObject go"); }
		//string line = System.String.Empty;
		AIController aic = go.GetComponent<AIController>();
		AIAnimationController aiac = go.GetComponentInChildren<AIAnimationController>();
		if (aic != null) {
			if (!aic.startInitialized) aic.Start();
			s2.Append(aic.index.ToString()); // int
			s2.Append(splitChar);
			switch (aic.currentState) {
				case Const.aiState.Walk: s2.Append("1"); break;
				case Const.aiState.Run: s2.Append("2"); break;
				case Const.aiState.Attack1: s2.Append("3");  break;
				case Const.aiState.Attack2: s2.Append("4");  break;
				case Const.aiState.Attack3: s2.Append("5");  break;
				case Const.aiState.Pain: s2.Append("6");  break;
				case Const.aiState.Dying: s2.Append("7");  break;
				case Const.aiState.Inspect: s2.Append("8");  break;
				case Const.aiState.Interacting: s2.Append("9");  break;
				case Const.aiState.Dead: s2.Append("10");  break;
				default: s2.Append("0");  break;
			}
			s2.Append(splitChar);
			if (aic.enemy != null) {
				SaveObject so = aic.enemy.GetComponent<SaveObject>();
				if (so != null) {
					s2.Append(so.SaveID.ToString());
					//line += splitChar + so.SaveID.ToString(); // saveID of NPC's enemy
				} else {
					s2.Append("-1");
					UnityEngine.Debug.Log("BUG: SaveNPCData missing SaveObject on aic.enemy with index " + aic.index.ToString());
					//line += splitChar + "-1";
				}
			} else {
				s2.Append("-1");
				//line += splitChar + "-1";
			}
			s2.Append(splitChar); s2.Append(aic.gracePeriodFinished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.meleeDamageFinished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(BoolToString(aic.inSight)); // bool
			s2.Append(splitChar); s2.Append(BoolToString(aic.infront)); // bool
			s2.Append(splitChar); s2.Append(BoolToString(aic.inProjFOV)); // bool
			s2.Append(splitChar); s2.Append(BoolToString(aic.LOSpossible)); // bool
			s2.Append(splitChar); s2.Append(BoolToString(aic.goIntoPain)); // bool
			s2.Append(splitChar); s2.Append(aic.rangeToEnemy.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(BoolToString(aic.firstSighting)); // bool
			s2.Append(splitChar); s2.Append(BoolToString(aic.dyingSetup)); // bool
			s2.Append(splitChar); s2.Append(BoolToString(aic.ai_dying)); // bool
			s2.Append(splitChar); s2.Append(BoolToString(aic.ai_dead)); // bool
			s2.Append(splitChar); s2.Append(aic.currentWaypoint.ToString()); // int
			s2.Append(splitChar); s2.Append(aic.currentDestination.x.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.currentDestination.y.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.currentDestination.z.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.timeTillEnemyChangeFinished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.timeTillDeadFinished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.timeTillPainFinished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.tickFinished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.raycastingTickFinished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.huntFinished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(BoolToString(aic.hadEnemy)); // bool
			s2.Append(splitChar); s2.Append(aic.lastKnownEnemyPos.x.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.lastKnownEnemyPos.y.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.lastKnownEnemyPos.z.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.tempVec.x.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.tempVec.y.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.tempVec.z.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(BoolToString(aic.shotFired)); // bool
			s2.Append(splitChar); s2.Append(aic.randomWaitForNextAttack1Finished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.randomWaitForNextAttack2Finished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.randomWaitForNextAttack3Finished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.idealTransformForward.x.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.idealTransformForward.y.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.idealTransformForward.z.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.idealPos.x.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.idealPos.y.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.idealPos.z.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.attackFinished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.attack2Finished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.attack3Finished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.targettingPosition.x.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.targettingPosition.y.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.targettingPosition.z.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(aic.deathBurstFinished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(BoolToString(aic.deathBurstDone)); // bool
			s2.Append(splitChar); s2.Append(BoolToString(aic.asleep)); // bool
			s2.Append(splitChar); s2.Append(aic.tranquilizeFinished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(BoolToString(aic.hopDone)); // bool
			s2.Append(splitChar); s2.Append(BoolToString(aic.wandering)); // bool
			s2.Append(splitChar); s2.Append(aic.wanderFinished.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(GetHealthManagerSaveData(aic.healthManager));
		} else {
			UnityEngine.Debug.Log("BUG: SaveNPCData missing AIController");
		}
		if (aiac != null) {
			s2.Append(splitChar); s2.Append(aiac.currentClipPercentage.ToString("0000.00000"));
			s2.Append(splitChar); s2.Append(BoolToString(aiac.dying)); // bool
			s2.Append(splitChar); s2.Append(aiac.animSwapFinished.ToString("0000.00000"));
		}
		//12
		if (s2 != null) return s2.ToString();
		return System.String.Empty;
	}

	// Save searchable data
	string SaveSearchableStaticData(GameObject go) {
		string line = System.String.Empty;
		SearchableItem se = go.GetComponent<SearchableItem>();
		if (se != null) {
			line = se.contents[0].ToString(); // int main lookup index
			line += splitChar + se.contents[1].ToString(); // int main lookup index
			line += splitChar + se.contents[2].ToString(); // int main lookup index
			line += splitChar + se.contents[3].ToString(); // int main lookup index
			line += splitChar + se.customIndex[0].ToString(); // int custom index
			line += splitChar + se.customIndex[1].ToString(); // int custom index
			line += splitChar + se.customIndex[2].ToString(); // int custom index
			line += splitChar + se.customIndex[3].ToString(); // int custom index
		} else {
			line = "BUG: SearchableItemMissing";
			UnityEngine.Debug.Log("SearchableItem missing on savetype of SearchableItem!");
		}
		//8
		return line;
	}	

	string SaveSearchableDestructsData(GameObject go) {
		string line = System.String.Empty;
		line = SaveSearchableStaticData(go); // get the searchable data
		line += splitChar + GetHealthManagerSaveData(go.GetComponent<HealthManager>()); // get health info
		//12
		return line;
	}



	string SaveForceBridgeData(GameObject go) {
		string line = System.String.Empty;
		ForceBridge fb = go.GetComponent<ForceBridge>();
		if (fb != null) {
			line = BoolToString(fb.activated); // bool - is the bridge on?
			line += splitChar + BoolToString(fb.lerping); // bool - are we currently lerping one way or tother
			line += splitChar + fb.tickFinished.ToString("0000.00000"); // float - time before firing targets
		} else {
			UnityEngine.Debug.Log("ForceBridge missing on savetype of ForceBridge!");
		}
		//2
		return line;
	}

	string SaveSwitchData(GameObject go) {
		string line = System.String.Empty;
		ButtonSwitch bs = go.GetComponent<ButtonSwitch>();
		if (bs != null) {
			// bs?  null??  that's bs
			line = BoolToString(bs.locked); // bool - is this switch locked
			line += splitChar + BoolToString(bs.active); // bool - is the switch flashing?
			line += splitChar + BoolToString(bs.alternateOn); // bool - is the flashing material on?
			line += splitChar + bs.delayFinished.ToString("0000.00000"); // float - time before firing targets
			line += splitChar + bs.tickFinished.ToString("0000.00000"); // float - time before firing targets
		} else {
			UnityEngine.Debug.Log("ButtonSwitch missing on savetype of ButtonSwitch!");
		}
		//4
		return line;
	}	

	string SaveFuncWallData(GameObject go) {
		string line = System.String.Empty;
		FuncWall fw = go.GetComponent<FuncWall>();
		if (fw != null) {
			switch (fw.currentState) {
				case FuncWall.FuncStates.Start: line = "0"; break;
				case FuncWall.FuncStates.Target: line = "1"; break;
				case FuncWall.FuncStates.MovingStart: line = "2"; break; // Position already handled by saving transform elsewhere.
				case FuncWall.FuncStates.MovingTarget: line = "3"; break;
				case FuncWall.FuncStates.AjarMovingStart: line = "4"; break;
				case FuncWall.FuncStates.AjarMovingTarget: line = "5"; break;
			}
		} else {
			UnityEngine.Debug.Log("FuncWall missing on savetype of FuncWall!");
		}
		//1
		return line;
	}	

	string SaveTeleDestData(GameObject go) {
		string line = System.String.Empty;
		TeleportTouch tt = go.GetComponent<TeleportTouch>();
		if (tt != null) {
			line = tt.justUsed.ToString("0000.00000"); // float - is the player still touching it?
		} else {
			UnityEngine.Debug.Log("TeleportTouch missing on savetype of TeleportTouch! GameObject.name: " + go.name);
		}
		//1
		return line;
	}	

	string SaveLogicBranchData(GameObject go) {
		string line = System.String.Empty;
		LogicBranch lb = go.GetComponent<LogicBranch>();
		if (lb != null) {
			line = BoolToString(lb.relayEnabled); // bool - is this enabled
			line += splitChar + BoolToString(lb.onSecond); // bool - He is. But who's on third? What's on first? Wait what??
		} else {
			UnityEngine.Debug.Log("LogicBranch missing on savetype of LogicBranch!");
		}
		//2
		return line;
	}	

	string SaveLogicRelayData(GameObject go) {
		string line = System.String.Empty;
		LogicRelay lr = go.GetComponent<LogicRelay>();
		if (lr != null) {
			line = BoolToString(lr.relayEnabled); // bool - is this enabled, Sherlock?
		} else {
			UnityEngine.Debug.Log("LogicRelay missing on savetype of LogicRelay!");
		}
		//1
		return line;
	}

	string SaveSpawnerData(GameObject go) {
		string line = System.String.Empty;
		SpawnManager sm = go.GetComponent<SpawnManager>();
		if (sm != null) {
			line = BoolToString(sm.active); // bool - is this enabled
			line += splitChar + sm.numberActive.ToString(); // int - number spawned
			line += splitChar + sm.delayFinished.ToString("0000.00000"); // float - time that we need to spawn next
		} else {
			UnityEngine.Debug.Log("SpawnManager missing on savetype of SpawnManager!");
		}
		//3
		return line;
	}	

	string SaveInteractablePanelData(GameObject go) {
		string line = System.String.Empty;
		InteractablePanel ip = go.GetComponent<InteractablePanel>();
		if (ip != null) {
			line = BoolToString(ip.open); // bool - is the panel opened
			line += splitChar + BoolToString(ip.installed); // bool - is the item installed, MAKE SURE YOU ENABLE THE INSTALL ITEM GameObject IN LOAD
		} else {
			UnityEngine.Debug.Log("InteractablePanel missing on savetype of InteractablePanel!");
		}
		//2
		return line;
	}		

	string SaveElevatorPanelData(GameObject go) {
		string line = System.String.Empty;
		KeypadElevator ke = go.GetComponent<KeypadElevator>();
		if (ke != null) {
			line = BoolToString(ke.padInUse); // bool - is the pad being used by a player
			line += splitChar + BoolToString(ke.locked); // bool - locked?
		} else {
			UnityEngine.Debug.Log("KeypadElevator missing on savetype of KeypadElevator!");
		}
		//2
		return line;
	}	

	string SaveKeypadData(GameObject go) {
		string line = System.String.Empty;
		KeypadKeycode kk = go.GetComponent<KeypadKeycode>();
		if (kk != null) {
			line = BoolToString(kk.padInUse); // bool - is the pad being used by a player
			line += splitChar + BoolToString(kk.locked); // bool - locked?
			line += splitChar + BoolToString(kk.solved); // bool - already entered correct keycode?
		} else {
			UnityEngine.Debug.Log("KeypadKeycode missing on savetype of KeypadKeycode!");
		}
		//3
		return line;
	}	

	string SavePuzzleGridData(GameObject go) {
		string line = System.String.Empty;
		PuzzleGridPuzzle pgp = go.GetComponent<PuzzleGridPuzzle>();
		if (pgp != null) {
			line = BoolToString(pgp.puzzleSolved); // bool - is this puzzle already solved?
			for (int i=0;i<35;i++) { line += splitChar + BoolToString(pgp.grid[i]); } // bool - get the current grid states + or X
			line += splitChar + BoolToString(pgp.fired); // bool - have we already fired yet?
			line += splitChar + BoolToString(pgp.locked); // bool - is this locked?
		} else {
			UnityEngine.Debug.Log("PuzzleGridData missing on savetype of PuzzleGrid!");
		}
		//38
		return line;
	}	

	string SavePuzzleWireData(GameObject go) {
		string line = System.String.Empty;
		PuzzleWirePuzzle pwp = go.GetComponent<PuzzleWirePuzzle>();
		if (pwp != null) {
			line = BoolToString(pwp.puzzleSolved); // bool - is this puzzle already solved?
			for (int i=0;i<7;i++) { line += splitChar + pwp.currentPositionsLeft[i].ToString(); } // int - get the current wire positions
			for (int i=0;i<7;i++) { line += splitChar + pwp.currentPositionsRight[i].ToString(); } // int - get the current wire positions
			line += splitChar + BoolToString(pwp.locked); // bool - is this locked?
		} else {
			UnityEngine.Debug.Log("PuzzleWirePuzzle missing on savetype of PuzzleWire!");
		}
		//16
		return line;
	}

	string SaveTCounterData(GameObject go) {
		string line = System.String.Empty;
		TriggerCounter tc = go.GetComponent<TriggerCounter>();
		if (tc != null) {
			line = tc.counter.ToString(); // int - how many counts we have
		} else {
			UnityEngine.Debug.Log("TriggerCounter missing on savetype of TriggerCounter!");
		}
		//1
		return line;	
	}

	string SaveTGravityData(GameObject go) {
		string line = System.String.Empty;
		GravityLift gl = go.GetComponent<GravityLift>();
		if (gl != null) {
			line = BoolToString(gl.active); // bool - is this gravlift on?
		} else {
			UnityEngine.Debug.Log("GravityLift missing on savetype of GravityLift!");
		}
		//1
		return line;
	}

	string SaveMChangerData(GameObject go) {
		string line = System.String.Empty;
		MaterialChanger mch = go.GetComponent<MaterialChanger>();
		if (mch != null) {
			line = BoolToString(mch.alreadyDone); // bool - is this gravlift on?  Much is already done.
		} else {
			UnityEngine.Debug.Log("MaterialChanger missing on savetype of MaterialChanger!");
		}
		//1
		return line;
	}

	string SaveTRadiationData(GameObject go) {
		string line = System.String.Empty;
		Radiation rad = go.GetComponent<Radiation>();
		if (rad != null) {
			line = BoolToString(rad.isEnabled); // bool - hey is this on? hello?
			line += splitChar + rad.numPlayers.ToString(); // int - how many players we are affecting
		} else {
			UnityEngine.Debug.Log("Radiation missing on savetype of Radiation!");
		}
		//2
		return line;
	}

	string SaveGravLiftPadTextureData(GameObject go) {
		string line = System.String.Empty;
		TextureChanger tex = go.GetComponent<TextureChanger>();
		if (tex != null) {
			line = BoolToString(tex.currentTexture); // bool - is this gravlift on?
		} else {
			UnityEngine.Debug.Log("TextureChanger missing on savetype of TextureChanger!");
		}
		//1
		return line;
	}

	string SaveChargeStationData(GameObject go) {
		string line = System.String.Empty;
		ChargeStation chg = go.GetComponent<ChargeStation>();
		if (chg != null) {
			line = chg.nextthink.ToString("0000.00000"); // float - time before recharged
		} else {
			UnityEngine.Debug.Log("ChargeStation missing on savetype of ChargeStation!");
		}
		//1
		return line;
	}

	string SaveLightAnimationData(GameObject go) {
		string line = System.String.Empty;
		LightAnimation la = go.GetComponent<LightAnimation>();
		if (la != null) {
			line = BoolToString(la.lightOn); // bool
			line += splitChar + BoolToString(la.lerpOn); // bool
			line += splitChar + la.currentStep.ToString(); // int
			line += splitChar + la.lerpValue.ToString("0000.00000");
			line += splitChar + la.lerpTime.ToString("0000.00000");
			line += splitChar + la.stepTime.ToString("0000.00000");
			line += splitChar + la.lerpStartTime.ToString("0000.00000");
		} else {
			UnityEngine.Debug.Log("LightAnimation missing on savetype of Light!");
		}
		return line;
	}

	string SaveLogicTimerData(GameObject go) {
		string line = System.String.Empty;
		LogicTimer lt = go.GetComponent<LogicTimer>();
		if (lt != null) {
			line = lt.intervalFinished.ToString("0000.00000"); // float
		} else {
			UnityEngine.Debug.Log("LogicTimer missing on savetype of LogicTimer!");
		}
		//1
		return line;
	}

	string SaveCameraData(GameObject go) {
		string line = System.String.Empty;
		Camera cm = go.GetComponent<Camera>();
		UnityStandardAssets.ImageEffects.BerserkEffect bzk = go.GetComponent<UnityStandardAssets.ImageEffects.BerserkEffect>();
		Grayscale gsc = go.GetComponent<Grayscale>();
		if (cm != null) {
			line = BoolToString(cm.enabled); // bool
			if (bzk != null) line += splitChar + BoolToString(bzk.enabled);
			else line += splitChar + "0";

			if (gsc != null) line += splitChar + BoolToString(gsc.enabled);
			else line += splitChar + "0";
		} else {
			UnityEngine.Debug.Log("Camera missing on savetype of Camera!");
		}
		// 4
		return line;
	}

	void FindAllSaveObjectsGOs(List<GameObject> gos) {
		List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
		for (int i=0;i<allParents.Count;i++) {
			Component[] compArray = allParents[i].GetComponentsInChildren(typeof(SaveObject),true); // find all SaveObject components, including inactive (hence the true here at the end)
			for (int k=0;k<compArray.Length;k++) {
				gos.Add(compArray[k].gameObject); //add the gameObject associated with all SaveObject components in the scene
			}
		}
	}

	// wrapper function to enable Save to be a coroutine...and now we can enable all the levels before a save for a tiny bit
	public void StartSave(int index, string savename) {
		if (index < 7) Const.a.justSavedTimeStamp = Time.time + Const.a.savedReminderTime; // using normal run time, don't ask again to save for next 7 seconds
		//StartCoroutine(Const.a.Save(index,savename));
		Save(index,savename);
	}

	// Save the Game
	// ============================================================================
	//public IEnumerator Save(int saveFileIndex,string savename) {
	public void Save(int saveFileIndex,string savename) {
		Stopwatch saveTimer = new Stopwatch();
		saveTimer.Start();
		string[] saveData = new string[16000]; // Found 2987 saveable objects on main level - should be enough for any instantiated dropped items...maybe
		string line;
		int i,j;
		int index = 0;
		Transform tr;
		// counting integers for grins and giggles
		//...hey it did let me see I had the wrong saveable type whenever there were zero or only 5 of certain objects
		int playercount = 0;
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
		int numChargeStations = 0;
		int numLights = 0;
		int numTimers = 0;
		int numCameras = 0;
		List<GameObject> saveableGameObjects = new List<GameObject>();
		FindAllSaveObjectsGOs(saveableGameObjects);

		// Indicate we are saving "Saving..."
		sprint(stringTable[194]);

		// Header
		// -----------------------------------------------------
		// Save Name
		if (string.IsNullOrWhiteSpace(savename)) savename = "Unnamed " + saveFileIndex.ToString(); // int
		saveData[index] = savename;
		index++;

		saveData[index] = PauseScript.a.relativeTime.ToString("0000.00000"); // float - time that we need to spawn next
		index++;
		s1.Clear(); // keep reusing s1
		// Global states and Difficulties
		s1.Append(LevelManager.a.currentLevel.ToString()); // int
		for (i=0;i<14;i++) {
			s1.Append(splitChar);
			s1.Append(LevelManager.a.levelSecurity[i].ToString()); // int
		}
		for (i=0;i<14;i++) {
			s1.Append(splitChar);
			s1.Append(LevelManager.a.levelCameraDestroyedCount[i].ToString()); // int
		}
		for (i=0;i<14;i++) {
			s1.Append(splitChar);
			s1.Append(LevelManager.a.levelSmallNodeDestroyedCount[i].ToString()); // int
		}
		for (i=0;i<14;i++) {
			s1.Append(splitChar);
			s1.Append(LevelManager.a.levelLargeNodeDestroyedCount[i].ToString()); // int
		}
		for (i=0;i<14;i++) {
			s1.Append(splitChar);
			s1.Append(BoolToString(LevelManager.a.ressurectionActive[i]));
		}
		s1.Append(splitChar);
		s1.Append(questData.lev1SecCode.ToString());
		s1.Append(splitChar);
		s1.Append(questData.lev2SecCode.ToString());
		s1.Append(splitChar);
		s1.Append(questData.lev3SecCode.ToString());
		s1.Append(splitChar);
		s1.Append(questData.lev4SecCode.ToString());
		s1.Append(splitChar);
		s1.Append(questData.lev5SecCode.ToString());
		s1.Append(splitChar);
		s1.Append(questData.lev6SecCode.ToString());
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.RobotSpawnDeactivated));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.IsotopeInstalled));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.ShieldActivated));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.LaserSafetyOverriden));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.LaserDestroyed));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.BetaGroveCyberUnlocked));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.GroveAlphaJettisonEnabled));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.GroveBetaJettisonEnabled));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.GroveDeltaJettisonEnabled));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.MasterJettisonBroken));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.Relay428Fixed));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.MasterJettisonEnabled));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.BetaGroveJettisoned));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.AntennaNorthDestroyed));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.AntennaSouthDestroyed));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.AntennaEastDestroyed));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.AntennaWestDestroyed));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.SelfDestructActivated));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.BridgeSeparated));
		s1.Append(splitChar);
		s1.Append(BoolToString(questData.IsolinearChipsetInstalled));
		s1.Append(splitChar);
		s1.Append(difficultyCombat.ToString());
		s1.Append(splitChar);
		s1.Append(difficultyMission.ToString());
		s1.Append(splitChar);
		s1.Append(difficultyPuzzle.ToString());
		s1.Append(splitChar);
		s1.Append(difficultyCyber.ToString());
		saveData[index] = s1.ToString();
		index++;

		if (player1 != null) playercount++;
		if (player2 != null) playercount++;
		if (player3 != null) playercount++;
		if (player4 != null) playercount++;

		// Save all the players' data
		saveData[index] = SavePlayerData(player1); index++; // saves as "!" if null
		saveData[index] = SavePlayerData(player2); index++; // saves as "!" if null
		saveData[index] = SavePlayerData(player3); index++; // saves as "!" if null
		saveData[index] = SavePlayerData(player4); index++; // saves as "!" if null

		string rbodynullstr = "|0000.00000|0000.00000|0000.00000";
		Rigidbody rbody;
		// Save all the objects data
		for (i=0;i<saveableGameObjects.Count;i++) {
			s1.Clear();
			SaveObject sav = saveableGameObjects[i].GetComponent<SaveObject>();
			if (!sav.initialized) sav.Start();
			string stype = sav.saveableType;
			if (stype == "Player") { continue;}
			s1.Append(stype);
			s1.Append(splitChar);
			s1.Append(sav.SaveID.ToString());
			s1.Append(splitChar);
			s1.Append(BoolToString(sav.instantiated)); // bool
			s1.Append(splitChar);
			s1.Append(sav.constLookupTable.ToString());
			s1.Append(splitChar);
			s1.Append(sav.constLookupIndex.ToString());
			s1.Append(splitChar);
			s1.Append(BoolToString(saveableGameObjects[i].activeSelf)); // bool.  Watch it next time buddy.  Yeesh, 2/28/22 was kind of scary till I realized this was still just using ToString here.  All saveables were turned off!!
			s1.Append(splitChar);
			tr = saveableGameObjects[i].GetComponent<Transform>();
			s1.Append(tr.localPosition.x.ToString("0000.00000"));
			s1.Append(splitChar);
			s1.Append(tr.localPosition.y.ToString("0000.00000"));
			s1.Append(splitChar);
			s1.Append(tr.localPosition.z.ToString("0000.00000"));
			s1.Append(splitChar);
			s1.Append(tr.localRotation.x.ToString("0000.00000"));
			s1.Append(splitChar);
			s1.Append(tr.localRotation.y.ToString("0000.00000"));
			s1.Append(splitChar);
			s1.Append(tr.localRotation.z.ToString("0000.00000"));
			s1.Append(splitChar);
			s1.Append(tr.localRotation.w.ToString("0000.00000"));
			s1.Append(splitChar);
			s1.Append(tr.localScale.x.ToString("0000.00000"));
			s1.Append(splitChar);
			s1.Append(tr.localScale.y.ToString("0000.00000"));
			s1.Append(splitChar);
			s1.Append(tr.localScale.z.ToString("0000.00000"));
			rbody = saveableGameObjects[i].GetComponent<Rigidbody>();
			if (rbody != null) {
				s1.Append(splitChar);
				s1.Append(rbody.velocity.x.ToString("0000.00000"));
				s1.Append(splitChar);
				s1.Append(rbody.velocity.y.ToString("0000.00000"));
				s1.Append(splitChar);
				s1.Append(rbody.velocity.z.ToString("0000.00000"));
			} else {
				s1.Append(rbodynullstr);
			}
			s1.Append(splitChar);
			s1.Append(sav.levelParentID.ToString()); // int
			s1.Append(splitChar);
			line = s1.ToString();
			switch (sav.saveType) {
				case SaveObject.SaveableType.Useable: numUseables++; line += SaveUseableData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.Grenade: numGrenades++; line += SaveGrenadeData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.NPC: numNPCs++; line += SaveNPCData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.Destructable: numDestructables++;  line += SaveDestructableData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.SearchableStatic: numSearchableStatics++;  line += SaveSearchableStaticData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.SearchableDestructable: numSearchableDestructs++;  line += SaveSearchableDestructsData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.Door: numDoors++;  line += saveableGameObjects[i].GetComponent<Door>().SaveDoorData(splitChar); break;
				case SaveObject.SaveableType.ForceBridge: numForceBs++;  line += SaveForceBridgeData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.Switch: numSwitches++;  line += SaveSwitchData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.FuncWall: numFuncWalls++;  line += SaveFuncWallData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.TeleDest: numTeleDests++;  line += SaveTeleDestData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.LBranch: numBranches++;  line += SaveLogicBranchData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.LRelay: numRelays++;  line += SaveLogicRelayData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.LSpawner: numSpawners++;  line += SaveSpawnerData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.InteractablePanel: numIntPanels++;  line += SaveInteractablePanelData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.ElevatorPanel: numElevPanels++;  line += SaveElevatorPanelData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.Keypad: numKeypads++;  line += SaveKeypadData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.PuzzleGrid: numPuzGrids++;  line += SavePuzzleGridData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.PuzzleWire: numPuzWires++;  line += SavePuzzleWireData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.TCounter: numTrigCounters++;  line += SaveTCounterData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.TGravity: numTrigGravity++;  line += SaveTGravityData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.MChanger: numMChangers++;  line += SaveMChangerData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.RadTrig: numRadTrigs++;  line += SaveTRadiationData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.GravPad: numGravPads++;  line += SaveGravLiftPadTextureData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.ChargeStation: numChargeStations++;  line += SaveChargeStationData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.Light: numLights++;  line += SaveLightAnimationData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.LTimer: numTimers++; line += SaveLogicTimerData(saveableGameObjects[i]); break;
				case SaveObject.SaveableType.Camera: numCameras++;  line += SaveCameraData(saveableGameObjects[i]); break;
				default: numTransforms++; break; // we already did the plain ol transform data first up above
			}
			saveData[index] = line; // take this objects data and add it to the array
			index++; // move to the next line
		}

		// UnityEngine.Debug.Log("Number of Transforms: " + numTransforms.ToString());
		// UnityEngine.Debug.Log("Number of Useable: " + numUseables.ToString());
		// UnityEngine.Debug.Log("Number of Grenades: " + numGrenades.ToString());
		// UnityEngine.Debug.Log("Number of NPCs: " + numNPCs.ToString());
		// UnityEngine.Debug.Log("Number of Destructables: " + numDestructables.ToString());
		// UnityEngine.Debug.Log("Number of SearchableStatics: " + numSearchableStatics.ToString());
		// UnityEngine.Debug.Log("Number of SearchableDestructables: " + numSearchableDestructs.ToString());
		// UnityEngine.Debug.Log("Number of Doors: " + numDoors.ToString());
		// UnityEngine.Debug.Log("Number of ForceBridges: " + numForceBs.ToString());
		// UnityEngine.Debug.Log("Number of Switches: " + numSwitches.ToString());
		// UnityEngine.Debug.Log("Number of FuncWalls: " + numFuncWalls.ToString());
		// UnityEngine.Debug.Log("Number of TeleDests: " + numTeleDests.ToString());
		// UnityEngine.Debug.Log("Number of Branches: " + numBranches.ToString());
		// UnityEngine.Debug.Log("Number of Relays: " + numRelays.ToString());
		// UnityEngine.Debug.Log("Number of Spawners: " + numSpawners.ToString());
		// UnityEngine.Debug.Log("Number of InteractablePanels: " + numIntPanels.ToString());
		// UnityEngine.Debug.Log("Number of ElevatorPanels: " + numElevPanels.ToString());
		// UnityEngine.Debug.Log("Number of Keypad: " + numKeypads.ToString());
		// UnityEngine.Debug.Log("Number of PuzzleGrid: " + numPuzGrids.ToString());
		// UnityEngine.Debug.Log("Number of PuzzleWire: " + numPuzWires.ToString());
		// UnityEngine.Debug.Log("Number of TriggerCounters: " + numTrigCounters.ToString());
		// UnityEngine.Debug.Log("Number of TriggerGravities: " + numTrigGravity.ToString());
		// UnityEngine.Debug.Log("Number of MaterialChangers: " + numMChangers.ToString());
		// UnityEngine.Debug.Log("Number of RadiationTriggers: " + numRadTrigs.ToString());
		// UnityEngine.Debug.Log("Number of GravityPads: " + numGravPads.ToString());
		// UnityEngine.Debug.Log("Number of ChargeStations: " + numChargeStations.ToString());
		// UnityEngine.Debug.Log("Number of Lights: " + numLights.ToString());
		// UnityEngine.Debug.Log("Number of LogicTimers: " + numTimers.ToString());

		// Write to file
		StreamWriter sw = new StreamWriter(Application.streamingAssetsPath + "/sav"+saveFileIndex.ToString()+".txt",false,Encoding.ASCII);
		if (sw != null) {
			using (sw) {
				for (j=0;j<saveData.Length;j++) {
					if (!string.IsNullOrWhiteSpace(saveData[j])) sw.WriteLine(saveData[j]);
				}
				sw.Close();
			}
		}

		LevelManager.a.DisableAllNonOccupiedLevels(); // go back to how we were
		// Make "Done!" appear at the end of the line after "Saving..." is finished, stole this from Halo's "Checkpoint...Done!"
		sprint(stringTable[195]);
		saveTimer.Stop();
		UnityEngine.Debug.Log("Saved to file in " + saveTimer.Elapsed.ToString());
	}

	void LoadPlayerDataToPlayer(GameObject currentPlayer, string[] entries, int currentline,int index) {
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
		HealthManager hm = pCap.GetComponent<HealthManager>();
		Transform tr = pCap.transform;
		MouseLookScript ml = playerMainCamera.GetComponent<MouseLookScript>();
		Transform trml = playerMainCamera.transform;
		WeaponCurrent wc = playerInventory.GetComponent<WeaponCurrent>();
		WeaponFire wf = pe.wepFire;
		Inventory inv = playerInventory.GetComponent<Inventory>();
		MFDManager mfd = MFDManager.a;
		// Already parsed saveableType and ID number in main Load() function, skipping ahead to index 2 (3rd slot).
		Const.a.playerName = entries[index]; index++; 
		ph.radiated = GetFloatFromString(entries[index],currentline); index++;
		ph.timer = GetFloatFromString(entries[index],currentline); index++;
		ph.playerDead = GetBoolFromString(entries[index]); index++;
		ph.radiationArea = GetBoolFromString(entries[index]); index++;
		ph.mediPatchPulseFinished = GetFloatFromString(entries[index],currentline); index++;
		ph.mediPatchPulseCount = GetIntFromString(entries[index],currentline ); index++;
		ph.makingNoise = GetBoolFromString(entries[index]); index++;
		ph.lastHealth = GetFloatFromString(entries[index],currentline); index++;
		ph.painSoundFinished = GetFloatFromString(entries[index],currentline); index++;
		ph.radSoundFinished = GetFloatFromString(entries[index],currentline); index++;
		ph.radFXFinished = GetFloatFromString(entries[index],currentline); index++;
		pe.energy = GetFloatFromString(entries[index],currentline); index++;
		pe.timer = GetFloatFromString(entries[index],currentline); index++;
		pe.tickFinished = GetFloatFromString(entries[index],currentline); index++;
		pm.playerSpeed = GetFloatFromString(entries[index],currentline); index++;
		pm.grounded = GetBoolFromString(entries[index]); index++;
		pm.currentCrouchRatio = GetFloatFromString(entries[index],currentline); index++;
		pm.bodyState = GetIntFromString(entries[index],currentline ); index++;
		pm.ladderState = GetBoolFromString(entries[index]); index++;
		pm.gravliftState = GetBoolFromString(entries[index]); index++;
		pm.inCyberSpace = GetBoolFromString(entries[index]); index++;
		for (j=0;j<4096;j++) { pm.automapExploredR[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored1[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored2[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored3[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored4[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored5[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored6[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored7[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored8[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExplored9[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExploredG1[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExploredG2[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { pm.automapExploredG4[j] = entries[index].Equals("1"); index++; }
		pm.CheatWallSticky = GetBoolFromString(entries[index]); index++;
		pm.CheatNoclip = GetBoolFromString(entries[index]); index++;
		pm.jumpTime = GetFloatFromString(entries[index],currentline); index++;
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		pm.oldVelocity = new Vector3(readFloatx,readFloaty,readFloatz);
		pm.fatigue = GetFloatFromString(entries[index],currentline); index++;
		pm.justJumped = GetBoolFromString(entries[index]); index++;
		pm.fatigueFinished = GetFloatFromString(entries[index],currentline); index++;
		pm.fatigueFinished2 = GetFloatFromString(entries[index],currentline); index++;
		pm.cyberSetup = GetBoolFromString(entries[index]); index++;
		pm.cyberDesetup = GetBoolFromString(entries[index]); index++;
		pm.oldBodyState = GetIntFromString(entries[index],currentline ); index++;
		pm.ConsoleDisable();
		pm.leanTarget = GetFloatFromString(entries[index],currentline); index++;
		pm.leanShift = GetFloatFromString(entries[index],currentline); index++;
		pm.jumpSFXFinished = GetFloatFromString(entries[index],currentline); index++;
		pm.jumpLandSoundFinished = GetFloatFromString(entries[index],currentline); index++;
		pm.jumpJetEnergySuckTickFinished = GetFloatFromString(entries[index],currentline); index++;
		pm.fatigueWarned = GetBoolFromString(entries[index]); index++;
		pm.turboFinished = GetFloatFromString(entries[index],currentline); index++;
		pm.ressurectingFinished = GetFloatFromString(entries[index],currentline); index++;
		pm.doubleJumpFinished = GetFloatFromString(entries[index],currentline); index++;
		pp.berserkFinishedTime = GetFloatFromString(entries[index],currentline); index++;
		pp.berserkIncrementFinishedTime = GetFloatFromString(entries[index],currentline); index++;
		pp.detoxFinishedTime = GetFloatFromString(entries[index],currentline); index++;
		pp.geniusFinishedTime = GetFloatFromString(entries[index],currentline); index++;
		pp.mediFinishedTime = GetFloatFromString(entries[index],currentline); index++;
		pp.reflexFinishedTime = GetFloatFromString(entries[index],currentline); index++;
		pp.sightFinishedTime = GetFloatFromString(entries[index],currentline); index++;
		pp.sightSideEffectFinishedTime = GetFloatFromString(entries[index],currentline); index++;
		pp.staminupFinishedTime = GetFloatFromString(entries[index],currentline); index++;
		pp.berserkIncrement = GetIntFromString(entries[index],currentline ); index++;
		pp.patchActive = GetIntFromString(entries[index],currentline ); index++;
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
		// readFloatx = GetFloatFromString(entries[index],currentline); index++;
		// readFloaty = GetFloatFromString(entries[index],currentline); index++;
		// readFloatz = GetFloatFromString(entries[index],currentline); index++;
		//trml.localPosition = new Vector3(readFloatx,readFloaty,readFloatz);
		// readFloatx = GetFloatFromString(entries[index],currentline); index++;
		// readFloaty = GetFloatFromString(entries[index],currentline); index++;
		// readFloatz = GetFloatFromString(entries[index],currentline); index++;
		// readFloatw = GetFloatFromString(entries[index],currentline); index++;
		//trml.localRotation = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);
		// readFloatx = GetFloatFromString(entries[index],currentline); index++;
		// readFloaty = GetFloatFromString(entries[index],currentline); index++;
		// readFloatz = GetFloatFromString(entries[index],currentline); index++;
		index += 10;
		//trml.localScale = new Vector3(readFloatx,readFloaty,readFloatz);
		ml.inventoryMode = !GetBoolFromString(entries[index]); index++; // take opposite because we are about to opposite again
		ml.ToggleInventoryMode(); // correctly set cursor lock state, and opposite again, now it is what was saved
		ml.holdingObject = GetBoolFromString(entries[index]); index++;
		ml.heldObjectIndex = GetIntFromString(entries[index],currentline ); index++;
		ml.heldObjectCustomIndex = GetIntFromString(entries[index],currentline ); index++;
		ml.heldObjectAmmo = GetIntFromString(entries[index],currentline ); index++;
		ml.heldObjectAmmo2 = GetIntFromString(entries[index],currentline ); index++;
		ml.firstTimePickup = GetBoolFromString(entries[index]); index++;
		ml.firstTimeSearch = GetBoolFromString(entries[index]); index++;
		ml.grenadeActive = GetBoolFromString(entries[index]); index++;
		ml.inCyberSpace = GetBoolFromString(entries[index]); index++;
		ml.yRotation = GetFloatFromString(entries[index],currentline); index++;
		ml.geniusActive = GetBoolFromString(entries[index]); index++;
		ml.xRotation = GetFloatFromString(entries[index],currentline); index++;
		ml.vmailActive = GetBoolFromString(entries[index]); index++;
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		ml.cyberspaceReturnPoint = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		ml.cyberspaceReturnCameraLocalRotation = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		ml.cyberspaceReturnPlayerCapsuleLocalRotation = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		ml.cyberspaceRecallPoint = new Vector3(readFloatx,readFloaty,readFloatz);
		ml.cyberspaceReturnLevel = GetIntFromString(entries[index],currentline ); index++;
		ml.currentSearchItem = null; // Prevent picking up first item immediately. Not currently possible to save references.
		hm.health = GetFloatFromString(entries[index],currentline); index++; // how much health we have
		hm.cyberHealth = GetFloatFromString(entries[index],currentline); index++;
		hm.deathDone = GetBoolFromString(entries[index]); index++; // bool - are we dead yet?
		hm.god = GetBoolFromString(entries[index]); index++; // Are we invincible? - we can save cheats?? OH WOW!
		hm.teleportDone = GetBoolFromString(entries[index]); index++; // did we already teleport? //hm.AwakeFromLoad();  Nothing done for isPlayer
		GUIState.ButtonType bt = (GUIState.ButtonType) Enum.Parse(typeof(GUIState.ButtonType), entries[index]);
		if (Enum.IsDefined(typeof(GUIState.ButtonType),bt)) GUIState.a.overButtonType = bt;
		index++;
		GUIState.a.overButton = GetBoolFromString(entries[index]); index++;
		for (j=0;j<7;j++) { inv.weaponInventoryIndices[j] = GetIntFromString(entries[index],currentline ); index++; }
		for (j=0;j<7;j++) { inv.weaponInventoryAmmoIndices[j] = GetIntFromString(entries[index],currentline ); index++; }
		//for (j=0;j<7;j++) { inv.weaponInventoryText[j] = inv.weaponInvTextSource[(WeaponFire.Get16WeaponIndexFromConstIndex(inv.weaponInventoryIndices[j]))]; } // derived from the above
		inv.numweapons = GetIntFromString(entries[index],currentline ); index++;
		for (j=0;j<16;j++) { inv.wepAmmo[j] = GetIntFromString(entries[index],currentline ); index++; }
		for (j=0;j<16;j++) { inv.wepAmmoSecondary[j] = GetIntFromString(entries[index],currentline ); index++; }
		for (j=0;j<7;j++) { inv.currentEnergyWeaponHeat[j] = GetFloatFromString(entries[index],currentline); index++; }
		for (j=0;j<7;j++) { inv.wepLoadedWithAlternate[j] = GetBoolFromString(entries[index]); index++; }
		wc.weaponCurrent = GetIntFromString(entries[index],currentline ); index++;
		wc.weaponIndex = GetIntFromString(entries[index],currentline ); index++;
		for (j=0;j<7;j++) { wc.weaponEnergySetting[j] = GetFloatFromString(entries[index],currentline); index++; }
		for (j=0;j<7;j++) { wc.currentMagazineAmount[j] = GetIntFromString(entries[index],currentline ); index++; }
		for (j=0;j<7;j++) { wc.currentMagazineAmount2[j] = GetIntFromString(entries[index],currentline ); index++; }
		wc.justChangedWeap = GetBoolFromString(entries[index]); index++;
		WeaponCurrent.WepInstance.SetAllViewModelsDeactive();
		wc.lastIndex = GetIntFromString(entries[index],currentline ); index++;
		wc.bottomless = GetBoolFromString(entries[index]); index++;
		wc.redbull = GetBoolFromString(entries[index]); index++;
		wc.reloadFinished = GetFloatFromString(entries[index],currentline); index++;
		wc.reloadLerpValue = GetFloatFromString(entries[index],currentline); index++;
		wc.lerpStartTime = GetFloatFromString(entries[index],currentline); index++;
		wc.targetY = GetFloatFromString(entries[index],currentline); index++;
		wf.waitTilNextFire = GetFloatFromString(entries[index],currentline); index++;
		wf.overloadEnabled = GetBoolFromString(entries[index]); index++;
		wf.sparqSetting = GetFloatFromString(entries[index],currentline); index++;
		wf.ionSetting = GetFloatFromString(entries[index],currentline); index++;
		wf.blasterSetting = GetFloatFromString(entries[index],currentline); index++;
		wf.plasmaSetting = GetFloatFromString(entries[index],currentline); index++;
		wf.stungunSetting = GetFloatFromString(entries[index],currentline); index++;
		wf.recoiling = GetBoolFromString(entries[index]); index++;
		wf.justFired = GetFloatFromString(entries[index],currentline); index++;
		wf.energySliderClickedTime = GetFloatFromString(entries[index],currentline); index++;
		wf.cyberWeaponAttackFinished = GetFloatFromString(entries[index],currentline); index++;
		inv.grenadeCurrent = GetIntFromString(entries[index],currentline ); index++;
		inv.grenadeIndex = GetIntFromString(entries[index],currentline ); index++;
		inv.nitroTimeSetting = GetFloatFromString(entries[index],currentline); index++;
		inv.earthShakerTimeSetting = GetFloatFromString(entries[index],currentline); index++;
		for (j=0;j<7;j++) { inv.grenAmmo[j] = GetIntFromString(entries[index],currentline ); index++; }
		inv.patchCurrent = GetIntFromString(entries[index],currentline ); index++;
		inv.patchIndex = GetIntFromString(entries[index],currentline ); index++;
		for (j=0;j<7;j++) { inv.patchCounts[j] = GetIntFromString(entries[index],currentline ); index++; }
		for (j=0;j<134;j++) { inv.hasLog[j] = GetBoolFromString(entries[index]); index++; }
		for (j=0;j<134;j++) { inv.readLog[j] = GetBoolFromString(entries[index]); index++; }
		for (j=0;j<10;j++) { inv.numLogsFromLevel[j] = GetIntFromString(entries[index],currentline ); index++; }
		inv.lastAddedIndex = GetIntFromString(entries[index],currentline ); index++;
		inv.beepDone = GetBoolFromString(entries[index]); index++;
		for (j=0;j<13;j++) { inv.hasHardware[j] = GetBoolFromString(entries[index]); index++; }
		if (Inventory.a.hasHardware[1]) {
			ml.compassContainer.SetActive(true);
			ml.automapContainerLH.SetActive(true);
			ml.automapContainerRH.SetActive(true);
		}
		for (j=0;j<13;j++) { inv.hardwareVersion[j] = GetIntFromString(entries[index],currentline ); index++; }
		for (j=0;j<13;j++) { inv.hardwareVersionSetting[j] = GetIntFromString(entries[index],currentline ); index++; }
		inv.hardwareInvCurrent = GetIntFromString(entries[index],currentline ); index++;
		inv.hardwareInvIndex = GetIntFromString(entries[index],currentline ); index++;
		for (j=0;j<13;j++) { inv.hardwareIsActive[j] = GetBoolFromString(entries[index]); index++; }
		for (j=0;j<32;j++) {
			int cardType = GetIntFromString(entries[index],currentline );
			switch (cardType) {
				case 0: inv.accessCardsOwned[j] = Door.accessCardType.None; break;
				case 1: inv.accessCardsOwned[j] = Door.accessCardType.Standard; break;
				case 2: inv.accessCardsOwned[j] = Door.accessCardType.Medical; break;
				case 3: inv.accessCardsOwned[j] = Door.accessCardType.Science; break;
				case 4: inv.accessCardsOwned[j] = Door.accessCardType.Admin; break;
				case 5: inv.accessCardsOwned[j] = Door.accessCardType.Group1; break;
				case 6: inv.accessCardsOwned[j] = Door.accessCardType.Group2; break;
				case 7: inv.accessCardsOwned[j] = Door.accessCardType.Group3; break;
				case 8: inv.accessCardsOwned[j] = Door.accessCardType.Group4; break;
				case 9: inv.accessCardsOwned[j] = Door.accessCardType.GroupA; break;
				case 10: inv.accessCardsOwned[j] = Door.accessCardType.GroupB; break;
				case 11: inv.accessCardsOwned[j] = Door.accessCardType.Storage; break;
				case 12: inv.accessCardsOwned[j] = Door.accessCardType.Engineering; break;
				case 13: inv.accessCardsOwned[j] = Door.accessCardType.Maintenance; break;
				case 14: inv.accessCardsOwned[j] = Door.accessCardType.Security; break;
				case 15: inv.accessCardsOwned[j] = Door.accessCardType.Per1; break;
				case 16: inv.accessCardsOwned[j] = Door.accessCardType.Per2; break;
				case 17: inv.accessCardsOwned[j] = Door.accessCardType.Per3; break;
				case 18: inv.accessCardsOwned[j] = Door.accessCardType.Per4; break;
				case 19: inv.accessCardsOwned[j] = Door.accessCardType.Per5; break;
				case 20: inv.accessCardsOwned[j] = Door.accessCardType.Command; break;
			}
			index++;
		}
		for (j=0;j<14;j++) { inv.generalInventoryIndexRef[j] = GetIntFromString(entries[index],currentline ); index++; }
		inv.generalInvCurrent = GetIntFromString(entries[index],currentline ); index++;
		inv.generalInvIndex = GetIntFromString(entries[index],currentline ); index++;
		inv.currentCyberItem = GetIntFromString(entries[index],currentline ); index++;
		inv.isPulserNotDrill = GetBoolFromString(entries[index]); index++;
		for (j=0;j<7;j++) { inv.softVersions[j] = GetIntFromString(entries[index],currentline ); index++; }
		for (j=0;j<7;j++) { inv.hasSoft[j] = GetBoolFromString(entries[index]); index++; }
		inv.emailCurrent = GetIntFromString(entries[index],currentline ); index++;
		inv.emailIndex = GetIntFromString(entries[index],currentline ); index++;	
		mfd.lastWeaponSideRH = GetBoolFromString(entries[index]); index++;
		mfd.lastItemSideRH = GetBoolFromString(entries[index]); index++;
		mfd.lastAutomapSideRH = GetBoolFromString(entries[index]); index++;
		mfd.lastTargetSideRH = GetBoolFromString(entries[index]); index++;
		mfd.lastDataSideRH = GetBoolFromString(entries[index]); index++;
		mfd.lastSearchSideRH = GetBoolFromString(entries[index]); index++;
		mfd.lastLogSideRH = GetBoolFromString(entries[index]); index++;
		mfd.lastLogSecondarySideRH = GetBoolFromString(entries[index]); index++;
		mfd.lastMinigameSideRH = GetBoolFromString(entries[index]); index++;
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		mfd.objectInUsePos = new Vector3(readFloatx,readFloaty,readFloatz);
		// tetheredPGP
		// tetheredPWP
		// tetheredSearchable
		// tetheredKeypadElevator
		// tetheredKeypadKeycode
		mfd.paperLogInUse = GetBoolFromString(entries[index]); index++;
		mfd.usingObject = GetBoolFromString(entries[index]); index++;
		mfd.logReaderContainer.SetActive(GetBoolFromString(entries[index])); index++;
		mfd.DataReaderContentTab.SetActive(GetBoolFromString(entries[index])); index++;
		mfd.logTable.SetActive(GetBoolFromString(entries[index])); index++;
		mfd.logLevelsFolder.SetActive(GetBoolFromString(entries[index])); index++;
		mfd.logFinished = GetFloatFromString(entries[index],currentline); index++;
		mfd.logActive = GetBoolFromString(entries[index]); index++;
		mfd.logType = GetIntFromString(entries[index],currentline ); index++;
		mfd.cyberTimer.GetComponent<CyberTimer>().timerFinished = GetFloatFromString(entries[index],currentline); index++;
	}

	void LoadObjectDataToObject(GameObject currentGameObject, string[] entries, int currentline, int index) {
		float readFloatx;
		float readFloaty;
		float readFloatz;
		float readFloatw;
		Vector3 tempvec;
		SaveObject so = currentGameObject.GetComponent<SaveObject>();

		// Index starts at 3 here for SetActive.
		// Set active state of GameObject in Hierarchy
		bool setToActive = GetBoolFromString(entries[index]); index++;
		if (setToActive) {
			if (!currentGameObject.activeSelf) currentGameObject.SetActive(true); 
		} else {
			if (currentGameObject.activeSelf) currentGameObject.SetActive(false); 
		}

		// Get transform
		readFloatx = GetFloatFromString(entries[index],currentline); index++;
		readFloaty = GetFloatFromString(entries[index],currentline); index++;
		readFloatz = GetFloatFromString(entries[index],currentline); index++;
		tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
		if (currentGameObject.transform.localPosition != tempvec) currentGameObject.transform.localPosition = tempvec;

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
			index = index + 3; // at 15 here, moving along and ignoring the zeros
		}

		HealthManager hm = currentGameObject.GetComponent<HealthManager>(); // used multiple times below
		SearchableItem se = currentGameObject.GetComponent<SearchableItem>(); // used multiple times below
		so.levelParentID = GetIntFromString(entries[index],currentline ); index++; // 16
		if (index >= entries.Length) return;
		switch (so.saveType) {
			case SaveObject.SaveableType.Useable:
				UseableObjectUse uou = currentGameObject.GetComponent<UseableObjectUse>();
				if (uou != null) {
					uou.useableItemIndex = GetIntFromString(entries[index],currentline ); index++;
					uou.customIndex = GetIntFromString(entries[index],currentline ); index++;
					uou.ammo = GetIntFromString(entries[index],currentline ); index++;
					uou.ammo2 = GetIntFromString(entries[index],currentline ); index++;
				}
				break;
			case SaveObject.SaveableType.Grenade:
				GrenadeActivate ga = currentGameObject.GetComponent<GrenadeActivate>();
				if (ga != null) {
					ga.constIndex = GetIntFromString(entries[index],currentline ); index++; // const lookup table index
					ga.useTimer = GetBoolFromString(entries[index]); index++; // do we have a timer going?
					ga.timeFinished = GetFloatFromString(entries[index],currentline); index++; // float - how much time left before the fun part?
					ga.explodeOnContact = GetBoolFromString(entries[index]); index++; // bool - or not a landmine
					ga.useProx = GetBoolFromString(entries[index]); index++; // bool - is this a landmine?
				}
				break;
			case SaveObject.SaveableType.NPC:
				AIController aic = currentGameObject.GetComponent<AIController>();
				AIAnimationController aiac = currentGameObject.GetComponentInChildren<AIAnimationController>();
				if (aic != null) {
					aic.index = GetIntFromString(entries[index],currentline ); index++; // int - NPC const lookup table index for instantiating
					int state = GetIntFromString(entries[index],currentline ); index++;
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
						case 10: aic.currentState = Const.aiState.Dead; break;
						default: aic.currentState = Const.aiState.Idle; break;
					}
					int enemIDRead = GetIntFromString(entries[index],currentline ); index++;
					if (enemIDRead >= 0) {
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
					}
					aic.gracePeriodFinished = GetFloatFromString(entries[index],currentline); index++; // float - time before applying pain damage on attack2
					aic.meleeDamageFinished = GetFloatFromString(entries[index],currentline); index++; // float - time before applying pain damage on attack2
					aic.inSight = GetBoolFromString(entries[index]); index++; // bool
					aic.infront = GetBoolFromString(entries[index]); index++; // bool
					aic.inProjFOV = GetBoolFromString(entries[index]); index++; // bool
					aic.LOSpossible = GetBoolFromString(entries[index]); index++; // bool
					aic.goIntoPain = GetBoolFromString(entries[index]); index++; // bool
					aic.rangeToEnemy = GetFloatFromString(entries[index],currentline); index++; // float
					aic.firstSighting = GetBoolFromString(entries[index]); index++; // bool - or are we dead?
					aic.dyingSetup = GetBoolFromString(entries[index]); index++; // bool - or are we dead?
					aic.ai_dying = GetBoolFromString(entries[index]); index++; // bool - are we dying the slow painful death
					aic.ai_dead = GetBoolFromString(entries[index]); index++; // bool - or are we dead?
					aic.currentWaypoint = GetIntFromString(entries[index],currentline ); index++; // int
					readFloatx = GetFloatFromString(entries[index],currentline); index++; // float
					readFloaty = GetFloatFromString(entries[index],currentline); index++; // float
					readFloatz = GetFloatFromString(entries[index],currentline); index++; // float
					aic.currentDestination = new Vector3(readFloatx,readFloaty,readFloatz);
					aic.timeTillEnemyChangeFinished = GetFloatFromString(entries[index],currentline); index++; // float
					aic.timeTillDeadFinished = GetFloatFromString(entries[index],currentline); index++; // float
					aic.timeTillPainFinished = GetFloatFromString(entries[index],currentline); index++; // float
					aic.tickFinished = GetFloatFromString(entries[index],currentline); index++; // float
					aic.raycastingTickFinished = GetFloatFromString(entries[index],currentline); index++; // float
					aic.huntFinished = GetFloatFromString(entries[index],currentline); index++; // float
					aic.hadEnemy = GetBoolFromString(entries[index]); index++; // bool
					readFloatx = GetFloatFromString(entries[index],currentline); index++; // float
					readFloaty = GetFloatFromString(entries[index],currentline); index++; // float
					readFloatz = GetFloatFromString(entries[index],currentline); index++; // float
					aic.lastKnownEnemyPos = new Vector3(readFloatx,readFloaty,readFloatz);
					readFloatx = GetFloatFromString(entries[index],currentline); index++; // float
					readFloaty = GetFloatFromString(entries[index],currentline); index++; // float
					readFloatz = GetFloatFromString(entries[index],currentline); index++; // float
					aic.tempVec = new Vector3(readFloatx,readFloaty,readFloatz);
					aic.shotFired = GetBoolFromString(entries[index]); index++; // bool
					aic.randomWaitForNextAttack1Finished = GetFloatFromString(entries[index],currentline); index++; // float
					aic.randomWaitForNextAttack2Finished = GetFloatFromString(entries[index],currentline); index++; // float
					aic.randomWaitForNextAttack3Finished = GetFloatFromString(entries[index],currentline); index++; // float
					readFloatx = GetFloatFromString(entries[index],currentline); index++; // float
					readFloaty = GetFloatFromString(entries[index],currentline); index++; // float
					readFloatz = GetFloatFromString(entries[index],currentline); index++; // float
					aic.idealTransformForward = new Vector3(readFloatx,readFloaty,readFloatz);
					readFloatx = GetFloatFromString(entries[index],currentline); index++; // float
					readFloaty = GetFloatFromString(entries[index],currentline); index++; // float
					readFloatz = GetFloatFromString(entries[index],currentline); index++; // float
					aic.idealPos = new Vector3(readFloatx,readFloaty,readFloatz);
					aic.attackFinished = GetFloatFromString(entries[index],currentline); index++; // float
					aic.attack2Finished = GetFloatFromString(entries[index],currentline); index++; // float
					aic.attack3Finished = GetFloatFromString(entries[index],currentline); index++; // float
					readFloatx = GetFloatFromString(entries[index],currentline); index++; // float
					readFloaty = GetFloatFromString(entries[index],currentline); index++; // float
					readFloatz = GetFloatFromString(entries[index],currentline); index++; // float
					aic.targettingPosition = new Vector3(readFloatx,readFloaty,readFloatz);
					aic.deathBurstFinished = GetFloatFromString(entries[index],currentline); index++; // float
					aic.deathBurstDone = GetBoolFromString(entries[index]); index++; // bool
					aic.asleep = GetBoolFromString(entries[index]); index++; // bool - are we sleepnir? vague reference alert
					aic.tranquilizeFinished = GetFloatFromString(entries[index],currentline); index++; // float
					aic.hopDone = GetBoolFromString(entries[index]); index++; // bool
					aic.wandering = GetBoolFromString(entries[index]); index++; // bool
					aic.wanderFinished = GetFloatFromString(entries[index],currentline); index++; // float
					if (hm != null) {
						hm.health = GetFloatFromString(entries[index],currentline); index++; // how much health we have
						if (hm.health > 0) {
							if (aic.boxCollider != null) { aic.boxCollider.enabled = true; }
							if (aic.sphereCollider != null) { aic.sphereCollider.enabled = true; }
							if (aic.meshCollider != null) { aic.meshCollider.enabled = true; }
							if (aic.capsuleCollider != null) { aic.capsuleCollider.enabled = true; }
							if (Const.a.moveTypeForNPC[aic.index] != Const.aiMoveType.Fly) {
								rbody.useGravity = true;
								rbody.isKinematic = false;
							}
						}
						hm.cyberHealth = GetFloatFromString(entries[index],currentline); index++;
						hm.deathDone = GetBoolFromString(entries[index]); index++; // bool - are we dead yet?
						hm.god = GetBoolFromString(entries[index]); index++; // are we invincible? - we can save cheats?? OH WOW!
						hm.teleportDone = GetBoolFromString(entries[index]); index++; // did we already teleport?
						hm.AwakeFromLoad();
					} else {
						index += 5;
					}
					if (aiac != null) {
						aiac.currentClipPercentage = GetFloatFromString(entries[index],currentline); index++; // float
						aiac.dying = GetBoolFromString(entries[index]); index++; // bool
						aiac.animSwapFinished = GetFloatFromString(entries[index],currentline); index++; // float
						if (!aic.ai_dead) {
							if (aiac.anim != null) aiac.anim.speed = 1f;
						}
					} else {
						index += 2;
					}
					// if (so.levelParentID >= 0 && so.levelParentID < 14) {
						// currentGameObject.transform.SetParent(LevelManager.a.npcsm[so.levelParentID].transform);
						// currentGameObject.transform.localPosition = tempV;
					// }
				} else {
					index += 11;
				}
				break;
			case SaveObject.SaveableType.Destructable:
				if (hm != null) {
					hm.health = GetFloatFromString(entries[index],currentline); index++; // how much health we have
					hm.deathDone = GetBoolFromString(entries[index]); index++; // bool - are we dead yet?
					hm.god = GetBoolFromString(entries[index]); index++; // are we invincible? - we can save cheats?? OH WOW!
					hm.teleportDone = GetBoolFromString(entries[index]); index++; // did we already teleport?
					hm.AwakeFromLoad();
				} else {
					index = index + 4;
				}
				break;
			case SaveObject.SaveableType.SearchableStatic:
				if (se != null) {
					se.contents[0] = GetIntFromString(entries[index],currentline ); index++; // int main lookup index
					se.contents[1] = GetIntFromString(entries[index],currentline ); index++; // int main lookup index
					se.contents[2] = GetIntFromString(entries[index],currentline ); index++; // int main lookup index
					se.contents[3] = GetIntFromString(entries[index],currentline ); index++; // int main lookup index
					se.customIndex[0] = GetIntFromString(entries[index],currentline ); index++; // int custom index
					se.customIndex[1] = GetIntFromString(entries[index],currentline ); index++; // int custom index
					se.customIndex[2] = GetIntFromString(entries[index],currentline ); index++; // int custom index
					se.customIndex[3] = GetIntFromString(entries[index],currentline ); index++; // int custom index
					se.searchableInUse = false;
				} else {
					index += 8;
				}
				break;
			case SaveObject.SaveableType.SearchableDestructable:
				if (se != null) {
					se.contents[0] = GetIntFromString(entries[index],currentline ); index++;; // int main lookup index
					se.contents[1] = GetIntFromString(entries[index],currentline ); index++; // int main lookup index
					se.contents[2] = GetIntFromString(entries[index],currentline ); index++; // int main lookup index
					se.contents[3] = GetIntFromString(entries[index],currentline ); index++; // int main lookup index
					se.customIndex[0] = GetIntFromString(entries[index],currentline ); index++; // int custom index
					se.customIndex[1] = GetIntFromString(entries[index],currentline ); index++; // int custom index
					se.customIndex[2] = GetIntFromString(entries[index],currentline ); index++; // int custom index
					se.customIndex[3] = GetIntFromString(entries[index],currentline ); index++; // int custom index
					se.searchableInUse = false;
				} else {
					index += 8;
				}
				if (hm != null) {
					hm.health = GetFloatFromString(entries[index],currentline); index++; // how much health we have
					hm.cyberHealth = GetFloatFromString(entries[index],currentline); index++;
					hm.deathDone = GetBoolFromString(entries[index]); index++; // bool - are we dead yet?
					hm.god = GetBoolFromString(entries[index]); index++; // are we invincible? - we can save cheats?? OH WOW!
					hm.teleportDone = GetBoolFromString(entries[index]); index++; // did we already teleport?
					hm.AwakeFromLoad();
				} else {
					index = index + 4;
				}
				break;
			case SaveObject.SaveableType.Door:
				Door dr = currentGameObject.GetComponent<Door>();
				if (dr != null) {
					dr.targetAlreadyDone = GetBoolFromString(entries[index]); index++; // bool - have we already ran targets
					dr.locked = GetBoolFromString(entries[index]); index++; // bool - is this locked?
					dr.ajar = GetBoolFromString(entries[index]); index++; // bool - is this ajar?
					dr.useFinished = GetFloatFromString(entries[index],currentline); index++;
					dr.waitBeforeClose = GetFloatFromString(entries[index],currentline); index++;
					dr.lasersFinished = GetFloatFromString(entries[index],currentline); index++;
					dr.blocked = GetBoolFromString(entries[index]); index++; // bool - is the door blocked currently?
					dr.accessCardUsedByPlayer = GetBoolFromString(entries[index]); index++; // bool - is the door blocked currently?
					int state = GetIntFromString(entries[index],currentline ); index++;
					string clipName = "IdleClosed";
					switch (state) {
						case 0: dr.doorOpen = Door.doorState.Closed; clipName = "IdleClosed"; break;
						case 1: dr.doorOpen = Door.doorState.Open; clipName = "IdleOpen"; break;
						case 2: dr.doorOpen = Door.doorState.Closing; clipName = "DoorClose"; break;
						case 3: dr.doorOpen = Door.doorState.Opening; clipName = "DoorOpen"; break;
					}
					dr.animatorPlaybackTime = GetFloatFromString(entries[index],currentline); index++;
					dr.SetAnimFromLoad(clipName,0,dr.animatorPlaybackTime);
				} else {
					index += 4;
				}
				break;
			case SaveObject.SaveableType.ForceBridge:
				ForceBridge fb = currentGameObject.GetComponent<ForceBridge>();
				if (fb != null) {
					fb.activated = GetBoolFromString(entries[index]); index++; // bool - is the bridge on?
					fb.lerping = GetBoolFromString(entries[index]); index++; // bool - are we currently lerping one way or tother
					fb.tickFinished = GetFloatFromString(entries[index],currentline); index++; // float - time before thinking
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
					bs.tickFinished = GetFloatFromString(entries[index],currentline); index++; // float - time before thinking
				} else {
					index += 4;
				}
				break;
			case SaveObject.SaveableType.FuncWall:
				FuncWall fw = currentGameObject.GetComponent<FuncWall>(); // actually this is on movertarget gameObjects
				if (fw != null) {
					int state = GetIntFromString(entries[index],currentline ); index++;
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
					sm.numberActive = GetIntFromString(entries[index],currentline ); index++; // int - number spawned
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
						pwp.currentPositionsLeft[i] = GetIntFromString(entries[index],currentline ); index++; // int - get the current wire positions
					}
					for (int i=0;i<pwp.currentPositionsRight.Length;i++) {
						pwp.currentPositionsRight[i] = GetIntFromString(entries[index],currentline ); index++;  // int - get the current wire positions
					}
					pwp.locked = GetBoolFromString(entries[index]); index++; // bool - is this locked?
				} else {
					index += 16; // number of wire positions is always 7 for each side
				}
				break;
			case SaveObject.SaveableType.TCounter:
				TriggerCounter tc = currentGameObject.GetComponent<TriggerCounter>();
				if (tc != null) {
					tc.counter = GetIntFromString(entries[index],currentline ); index++; // int - how many counts we have
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
					rad.numPlayers = GetIntFromString(entries[index],currentline ); index++; // int - how many players we are affecting
				} else {
					index += 2;
				}
				break;
			case SaveObject.SaveableType.GravPad:
				TextureChanger tex = currentGameObject.GetComponent<TextureChanger>();
				if (tex != null) {
					tex.currentTexture = GetBoolFromString(entries[index]); index++; // bool - is this gravlift on?
					tex.currentTexture = !tex.currentTexture; // gets done again in Toggle()
					tex.Toggle(); // set it again to be sure, does other stuff than just change the bool
				}
				break;
			case SaveObject.SaveableType.ChargeStation:
				ChargeStation chg = currentGameObject.GetComponent<ChargeStation>();
				if (chg != null) {
					chg.nextthink = GetFloatFromString(entries[index],currentline); index++; // float - time before recharged
				}
				break;
			case SaveObject.SaveableType.Light:
				LightAnimation la = currentGameObject.GetComponent<LightAnimation>();
				if (la != null) {
					la.lightOn = GetBoolFromString(entries[index]); index++;
					la.lerpOn = GetBoolFromString(entries[index]); index++;
					la.currentStep = GetIntFromString(entries[index],currentline ); index++;
					la.lerpValue = GetFloatFromString(entries[index],currentline); index++;
					la.lerpTime = GetFloatFromString(entries[index],currentline); index++;
					la.stepTime = GetFloatFromString(entries[index],currentline); index++;
					la.lerpStartTime = GetFloatFromString(entries[index],currentline); index++;
				}
				break;
			case SaveObject.SaveableType.Camera:
				Camera cm = currentGameObject.GetComponent<Camera>();
				UnityStandardAssets.ImageEffects.BerserkEffect bzk = currentGameObject.GetComponent<UnityStandardAssets.ImageEffects.BerserkEffect>();
				Grayscale gsc = currentGameObject.GetComponent<Grayscale>();
				if (cm != null) { cm.enabled = GetBoolFromString(entries[index]); index++;
				} else index++;
				if (bzk != null) { bzk.enabled = GetBoolFromString(entries[index]); index++;
				} else index++;
				if (gsc != null) { gsc.enabled = GetBoolFromString(entries[index]); index++;
				} else index++;
				break;
		}
		//objectLoadTimer.Stop();
		//UnityEngine.Debug.Log("LoadObjectData to Object savetype of " + so.saveType + " took time of " + objectLoadTimer.Elapsed.ToString());
	}

	public void Load(int saveFileIndex) {
		loadingScreen.SetActive(true);
		loadPercentText.text = "--.----";
		PauseScript.a.mainMenu.SetActive(false);
		PauseScript.a.PauseEnable();
		PauseScript.a.Loading();
		MFDManager.a.TabReset(true);
		MFDManager.a.TabReset(false);
		StartCoroutine(Const.a.LoadRoutine(saveFileIndex));
	}

	public IEnumerator LoadRoutine(int saveFileIndex) {
		Stopwatch loadTimer = new Stopwatch();
		//Stopwatch matchTimer = new Stopwatch();
		loadTimer.Start();
		Cursor.visible = true;
		yield return null;

		player1CapsuleMainCameragGO.GetComponent<Camera>().enabled = false;
		if (saveFileIndex < 0) {
			//WriteDatForNewGame(true,false); // set bit to know to deactivate main menu when we reload
			//SceneManager.LoadScene(0); // reload. it. all.
			loadingScreen.SetActive(false);
			PauseScript.a.PauseDisable();
			yield break;
		}
		startingNewGame = false;
		freshRun = false;
		//WriteDatForNewGame(false,false); // reset
		//SceneManager.LoadScene(0);
		yield return null;

		string readline;
		int currentline = 0;
		int numPlayers = 0;
		int numSaveablesFromSavefile = 0;
		int i,j;
		GameObject currentGameObject = null;
		sprint(stringTable[196]); // Loading...
		yield return null; // to update the sprint
		if (player1 != null) numPlayers++;
		if (player2 != null) numPlayers++;
		if (player3 != null) numPlayers++;
		if (player4 != null) numPlayers++;
		// Find all gameobjects with SaveObject script attached
		List<GameObject> saveableGameObjects = new List<GameObject>();
		FindAllSaveObjectsGOs(saveableGameObjects); // Find and add GameObjects to the list of saveables.
		List<string> readFileList = new List<string>();
		char csplit = '|'; // caching since it will be iterated over in a loop
		int index = 0; // caching since...I just said this
		string[] entries = new string[2048]; // hold | delimited strings on individual lines
		StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/sav"+saveFileIndex.ToString()+".txt");
		if (sr != null) {
			// Read the file into a list, line by line
			using (sr) {
				do {
					readline = sr.ReadLine();
					if (readline != null) {
						readFileList.Add(readline);
					}
				} while (!sr.EndOfStream);
				sr.Close();
			}

			numSaveablesFromSavefile = readFileList.Count;

			// readFileList[currentline] == saveName;  Not important, we are loading already now
			currentline++; // line is over, now we are at 1
			index = 0; // I already did this but just to be sure

			// Read in global time and pause data
			entries = readFileList[currentline].Split(csplit);
			PauseScript.a.relativeTime = GetFloatFromString(entries[index],currentline); // the global time from which everything checks it's somethingerotherFinished timer states
			currentline++; // line is over, now we are at 2
			index = 0; // reset before starting next line

			// Read in global states and quest mission bits in questData and difficulty indices
			entries = readFileList[currentline].Split(csplit);
			int levelNum = GetIntFromString(entries[index],currentline ); index++;
			LevelManager.a.LoadLevelFromSave(levelNum);
			for (i=0;i<14;i++) {
				LevelManager.a.levelSecurity[i] = GetIntFromString(entries[index],currentline); index++;
			}
			for (i=0;i<14;i++) {
				LevelManager.a.levelCameraDestroyedCount[i] = GetIntFromString(entries[index],currentline); index++;
			}
			for (i=0;i<14;i++) {
				LevelManager.a.levelSmallNodeDestroyedCount[i] = GetIntFromString(entries[index],currentline); index++;
			}
			for (i=0;i<14;i++) {
				LevelManager.a.levelLargeNodeDestroyedCount[i] = GetIntFromString(entries[index],currentline); index++;
			}
			for (i=0;i<14;i++) {
				LevelManager.a.ressurectionActive[i] = GetBoolFromString(entries[index]); index++;
			}
			questData.lev1SecCode = GetIntFromString(entries[index],currentline); index++;
			questData.lev2SecCode = GetIntFromString(entries[index],currentline); index++;
			questData.lev3SecCode = GetIntFromString(entries[index],currentline); index++;
			questData.lev4SecCode = GetIntFromString(entries[index],currentline); index++;
			questData.lev5SecCode = GetIntFromString(entries[index],currentline); index++;
			questData.lev6SecCode = GetIntFromString(entries[index],currentline); index++;
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
			difficultyCombat = GetIntFromString(entries[index],currentline); index++;
			difficultyMission = GetIntFromString(entries[index],currentline); index++;
			difficultyPuzzle = GetIntFromString(entries[index],currentline); index++;
			difficultyCyber = GetIntFromString(entries[index],currentline); index++;
			currentline++; // line is over, now we are at 3

			// Load player 1 data
			index = 2; // reset before starting next line, skipping savetype and ID number
			entries = readFileList[currentline].Split(csplit); // read in Quest bits
			if (entries[0] != "!" && player1 != null) LoadPlayerDataToPlayer(player1,entries,currentline,index);
			currentline++; // line is over, now we are at 4
			index = 2; // reset before starting next line, skipping savetype and ID number

			// Load player 2 data
			entries = readFileList[currentline].Split(csplit); // read in Quest bits
			if (entries[0] != "!" && player2 != null) LoadPlayerDataToPlayer(player2,entries,currentline,index);
			currentline++; // line is over, now we are at 5
			index = 2; // reset before starting next line, skipping savetype and ID number

			// Load player 3 data
			entries = readFileList[currentline].Split(csplit); // read in Quest bits
			if (entries[0] != "!" && player3 != null) LoadPlayerDataToPlayer(player3,entries,currentline,index);
			currentline++; // line is over, now we are at 6
			index = 2; // reset before starting next line, skipping savetype and ID number

			// Load player 4 data
			entries = readFileList[currentline].Split(csplit); // read in Quest bits
			if (entries[0] != "!" && player4 != null) LoadPlayerDataToPlayer(player4,entries,currentline,index);
			currentline++; // line is over, now we are at 7
			index = 0; // reset before starting next line, last one before I start doing an array, promise!

			if (currentline != 7) UnityEngine.Debug.Log("ERROR: currentline wasn't 7 before iterating through saveObjects!");
			//int[] lookupList = new int[(numSaveablesFromSavefile-currentline)];
			//SaveObject[] sos = new SaveObject[saveableGameObjects.Count];
			loadPercentText.text = "0.0001";
			yield return null;

			// get the existing IDs
			 // for (int i=currentline;i<saveableGameObjects.Count;i++) {
				// sos[i] = saveableGameObjects[i].GetComponent<SaveObject>();
			 // }

			//int largerCount = numSaveablesFromSavefile-7;
			//bool moreObjectsInSave = false;
			//bool moreObjectsInGame = false;
			//if ((saveableGameObjects.Count-1) > largerCount) {
			//	largerCount = (saveableGameObjects.Count-1);
			//	moreObjectsInSave = true;
			//}

			//if (largerCount > (saveableGameObjects.Count-1)) moreObjectsInGame = true;
			//UnityEngine.Debug.Log("moreObjectsInSave: " + moreObjectsInSave.ToString());
			//UnityEngine.Debug.Log("moreObjectsInGame: " + moreObjectsInGame.ToString());
			//UnityEngine.Debug.Log("largerCount: " + largerCount.ToString());

			int[] readIDs = new int[(numSaveablesFromSavefile-currentline)];
			List<int> instantiatedFound = new List<int>();
			bool instantiatedCheck;
			bool instantiatedActive;
			for (i=currentline;i<(numSaveablesFromSavefile-7);i++) {
				entries = readFileList[i].Split(csplit);
				if (entries.Length > 1) {
					instantiatedCheck = instantiatedActive = false;
					readIDs[i] = GetIntFromString(entries[1],i); // int - get saveID from 2nd slot
					instantiatedActive = GetBoolFromString(entries[3]); // bool - get activeSelf value of the gameObject
					instantiatedCheck = GetBoolFromString(entries[2]); // bool - get instantiated from 3rd slot
					if (instantiatedCheck && instantiatedActive) instantiatedFound.Add(i);
				}
			}
			// UnityEngine.Debug.Log("Number of instantiated objects to instantiate later: " + instantiatedFound.Count.ToString());

			//matchTimer.Start();
			index = 5; // 0 = Saveable type, 1 = SaveID, 2 = instantiated boolean, 3 = constLookupTable, 4 = constLookupIndex 
			bool[] alreadyCheckedThisSaveableGameObject = new bool[saveableGameObjects.Count];
			for (i=0;i<alreadyCheckedThisSaveableGameObject.Length;i++) {
				alreadyCheckedThisSaveableGameObject[i] = false; // Reset the list
			}

			SaveObject so;
			// Ok, so we have a list of all saveableGameObjects and a list of all saveables from the savefile.
			// Main iteration loops through all lines in the savefile.
			// Second iteration loops through all saveableGameObjects to find a match.
			// The save file will always have more objects in it than in the level since the level is fresh.
			// When we come across an instantiated object in the saveable file, we need to remember it for later and instantiate them all.
			for (i=currentline;i<(numSaveablesFromSavefile-7);i++) {
				for (j=0;j<(saveableGameObjects.Count);j++) {
					if (alreadyCheckedThisSaveableGameObject[j]) continue; // skip checking this and doing GetComponent
					currentGameObject = saveableGameObjects[j];
					so = currentGameObject.GetComponent<SaveObject>();
					if(so.SaveID == readIDs[i]) {
						entries = readFileList[i].Split(csplit);
						//UnityEngine.Debug.Log("Loading line: " + i.ToString() + " to GameObject named: " + currentGameObject.name + " with SaveID " + so.SaveID.ToString());
						LoadObjectDataToObject(currentGameObject,entries,i,index);
						alreadyCheckedThisSaveableGameObject[j] = true; // Huge time saver right here!
						break;
					}
				}
				loadPercentText.text = currentline.ToString("00.00");
				if (UnityEngine.Random.Range(0f,1.0f) < 0.01f) yield return null;
			}

			int numberOfMissedObjects = 0;
			for (i=0;i<saveableGameObjects.Count;i++) {
				if (alreadyCheckedThisSaveableGameObject[i]) continue;
				numberOfMissedObjects++;
				if (saveableGameObjects[i].name != "Player") Destroy(saveableGameObjects[i]);
			}

			// Now time to instantiate anything left that is supposed to be here
			if (instantiatedFound.Count > 0) {
				int constdex = -1;
				int consttable = 0;
				GameObject instantiatedObject = null;
				GameObject prefabReferenceGO = null;
				for (i=0;i<instantiatedFound.Count;i++) {
					entries = readFileList[i].Split(csplit);
					if (entries.Length > 1) {
						consttable = GetIntFromString(entries[3],i); // int - get the prefab table type to use for lookups in Const
						constdex = GetIntFromString(entries[4],i); // int - get the index into the Const table of prefabs
						if (constdex >= 0 && (consttable == 0 || consttable == 1)) {
							if (consttable == 0) prefabReferenceGO = Const.a.useableItems[constdex];
							else if (consttable == 1) prefabReferenceGO = Const.a.npcPrefabs[constdex];
							if (prefabReferenceGO != null) instantiatedObject = Instantiate(prefabReferenceGO,Const.a.vectorZero,quaternionIdentity) as GameObject; // Instantiate at generic location
							if (instantiatedObject != null) LoadObjectDataToObject(instantiatedObject, entries, i, index); // Load it
						}
					}
					loadPercentText.text = (i / instantiatedFound.Count).ToString("00.0000");
					yield return null;
				}
			}
			//matchTimer.Stop();
			//UnityEngine.Debug.Log("Loading done!");
		}
		player1Capsule.SetActive(true);
		player1CapsuleMainCameragGO.transform.parent.gameObject.SetActive(true);
		player1CapsuleMainCameragGO.SetActive(true);
		player1CapsuleMainCameragGO.GetComponent<Camera>().enabled = true;
		yield return null;
		loadingScreen.SetActive(false);
		PauseScript.a.PauseDisable();
		sprint(stringTable[197]); // Loading...Done!
		loadTimer.Stop();
		//UnityEngine.Debug.Log("Matching index loop de loop time: " + matchTimer.Elapsed.ToString());
		UnityEngine.Debug.Log("Loading time: " + loadTimer.Elapsed.ToString());
	}

	public void SetFOV() {
		if (player1 != null) player1CapsuleMainCameragGO.GetComponent<Camera>().fieldOfView = GraphicsFOV;
		if (player2 != null) player2.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().fieldOfView = GraphicsFOV;
		if (player3 != null) player3.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().fieldOfView = GraphicsFOV;
		if (player4 != null) player4.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().fieldOfView = GraphicsFOV;
	}

	public void SetBloom() {
		if (player1 != null) player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.bloom.enabled = GraphicsBloom;
		if (player2 != null) player2.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.bloom.enabled = GraphicsBloom;
		if (player3 != null) player3.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.bloom.enabled = GraphicsBloom;
		if (player4 != null) player4.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.bloom.enabled = GraphicsBloom;
	}


	void SetFXAA(AntialiasingModel.FxaaPreset preset) {
		AntialiasingModel.Settings amS = AntialiasingModel.Settings.defaultSettings;
		amS.fxaaSettings.preset = preset;
		amS.method = AntialiasingModel.Method.Fxaa;
		if (player1 != null) {
			player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = true;
			player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings = amS;
		}
		if (player2 != null) {
			player2.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = true;
			player2.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings = amS;
		}
		if (player3 != null) {
			player3.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = true;
			player3.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings = amS;
		}
		if (player4 != null) {
			player4.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = true;
			player4.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings = amS;
		}
	}

	// None
	// ExtremePerformance,
	// Performance,
	// Default,
	// Quality,
	// ExtremeQuality
	public void SetAA() {
		switch (GraphicsAAMode) {
			case 0: // No Antialiasing
				if (player1 != null) player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = false;
				if (player2 != null) player2.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = false;
				if (player3 != null) player3.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = false;
				if (player4 != null) player4.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = false;
				break;
			case 1: // FXAA Extreme Performance
				SetFXAA(AntialiasingModel.FxaaPreset.ExtremePerformance);
				break;
			case 2: // FXAA Performance
				SetFXAA(AntialiasingModel.FxaaPreset.Performance);
				break;
			case 3: // FXAA Default
				SetFXAA(AntialiasingModel.FxaaPreset.Default);
				break;
			case 4: // FXAA Extreme Quality
				SetFXAA(AntialiasingModel.FxaaPreset.Quality);
				break;
			case 5: // FXAA Extreme Quality
				SetFXAA(AntialiasingModel.FxaaPreset.ExtremeQuality);
				break;
			case 6: // TAA Default
				if (player1 != null) {
					player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = true;
					AntialiasingModel.Settings amS = player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings;
					amS.method = AntialiasingModel.Method.Taa;
					player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings = amS;
				}
				if (player2 != null) {
					player2.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = true;
					AntialiasingModel.Settings amS = player2.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings;
					amS.method = AntialiasingModel.Method.Taa;
					player2.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings = amS;
				}
				if (player3 != null) {
					player3.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = true;
					AntialiasingModel.Settings amS = player3.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings;
					amS.method = AntialiasingModel.Method.Taa;
					player3.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings = amS;
				}
				if (player4 != null) {
					player4.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = true;
					AntialiasingModel.Settings amS = player4.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings;
					amS.method = AntialiasingModel.Method.Taa;
					player4.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings = amS;
				}
				break;
		}
	}

	public void SetSSAO() {
		if (player1 != null) player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.ambientOcclusion.enabled = GraphicsSSAO;
		if (player2 != null) player2.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.ambientOcclusion.enabled = GraphicsSSAO;
		if (player3 != null) player3.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.ambientOcclusion.enabled = GraphicsSSAO;
		if (player4 != null) player4.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.ambientOcclusion.enabled = GraphicsSSAO;
	}

	public void SetBrightness() {
		float tempf = Const.a.GraphicsGamma;
		if (tempf < 1) tempf = 0;
		else tempf = tempf/100;
		tempf = (tempf * 8f) - 4f;
		PostProcessingProfile ppf = player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile;
		ColorGradingModel.Settings cgms = ppf.colorGrading.settings;
		cgms.basic.postExposure = tempf;
		if (player1 != null) ppf.colorGrading.settings = cgms;
	}

	public void SetVolume() {
		if (MainMenuHandler.a.dataFound) {
			AudioListener.volume = (AudioVolumeMaster/100f);
			mainmenuMusic.volume = (AudioVolumeMusic/100f);
			if (Music.a != null) {
				if (Music.a.SFXMain != null) Music.a.SFXMain.volume = (AudioVolumeMusic/100f);
				if (Music.a.SFXOverlay != null) Music.a.SFXOverlay.volume = (AudioVolumeMusic/100f);
			}
		} else {
			AudioListener.volume = 0f;
		}
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
			if (i == (healthObjectsRegistration.Length - 1)) UnityEngine.Debug.Log("WARNING: Could not register object with health.  Hit limit of " + healthObjectsRegistration.Length.ToString() + ".");
		}
	}

	private CultureInfo en_US_Culture = new CultureInfo("en-US");

	private int AssignConfigInt(string section, string keyname) {
		int inputInt = -1;
		string inputCapture = System.String.Empty;
		inputCapture = INIWorker.IniReadValue(section,keyname);
		if (inputCapture == null) inputCapture = "NULL";
		bool parsed = Int32.TryParse(inputCapture, NumberStyles.Integer, en_US_Culture, out inputInt);
		if (parsed) return inputInt; else sprint("Warning: Could not parse config key " + keyname + " as integer: " + inputCapture);
		return 0;
	}

	private bool AssignConfigBool(string section, string keyname) {
		int inputInt = -1;
		string inputCapture = System.String.Empty;
		inputCapture = INIWorker.IniReadValue(section,keyname);
		if (inputCapture == null) inputCapture = "NULL";
		bool parsed = Int32.TryParse(inputCapture, NumberStyles.Integer, en_US_Culture, out inputInt);
		if (parsed) {
			if (inputInt > 0) return true; else return false;
		} else sprint("Warning: Could not parse config key " + keyname + " as bool: " + inputCapture);
		return false;
	}

	public bool GetBoolFromString(string val) {
		return val.Equals("1");
	}

	private string boolFalseString = "0";
	private string boolTrueString = "1";
	public string BoolToString(bool inputValue) {
		if (inputValue) return boolTrueString;
		return boolFalseString;
	}

	private bool getValparsed;
	private int getValreadInt;
	private float getValreadFloat;
	public int GetIntFromString(string val, int currentline) {
		if (val == "0") return 0;
		getValparsed = Int32.TryParse(val, NumberStyles.Integer, en_US_Culture, out getValreadInt);
		if (!getValparsed) {
			UnityEngine.Debug.Log("BUG: Could not parse int from `" + val + "` on line " + currentline.ToString());
			return 0;
		}
		return getValreadInt;
	}

	public int GetIntFromString(string val) {
		if (val == "0") return 0;
		getValparsed = Int32.TryParse(val, NumberStyles.Integer, en_US_Culture, out getValreadInt);
		if (!getValparsed) { UnityEngine.Debug.Log("BUG: Could not parse int from `" + val + "`"); return 0; }
		return getValreadInt;
	}

	public float GetFloatFromString(string val, int currentline) {
		getValparsed = Single.TryParse(val, NumberStyles.Float, en_US_Culture, out getValreadFloat);
		if (!getValparsed) {
			UnityEngine.Debug.Log("BUG: Could not parse float from `" + val + "` on line " + currentline.ToString());
			return 0.0f;
		}
		return getValreadFloat;
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

	public aiMoveType GetMoveTypeFromInt(int movetype_i) { 
		switch (movetype_i) {
			case 0: return aiMoveType.None;
			case 1: return aiMoveType.Walk;
			case 2: return aiMoveType.Fly;
			case 3: return aiMoveType.Swim;
			case 4: return aiMoveType.Cyber;
		}
		return aiMoveType.None;
	}

	public static DamageData SetNPCDamageData (int NPCindex, int attackNum, GameObject ownedBy) {
		if (NPCindex < 0 || NPCindex > 23) { NPCindex = 0; UnityEngine.Debug.Log("BUG: NPCindex set incorrectly on NPC.  Not 0 to 23 on NPC at: " + ownedBy.transform.position.x.ToString() + ", " + ownedBy.transform.position.y.ToString() + ", " + ownedBy.transform.position.z + "."); }
		if (attackNum < 1 || attackNum > 3) attackNum = 1;
		DamageData dd = new DamageData(); 
		// Attacker (self [a]) data
		dd.owner = ownedBy;
		switch (attackNum) {
		case 1:
			dd.damage = Const.a.damageForNPC[NPCindex];
			break;
		case 2:
			dd.damage = Const.a.damageForNPC2[NPCindex];
			break;
		case 3:
			dd.damage = Const.a.damageForNPC3[NPCindex];
			break;
		default: UnityEngine.Debug.Log("BUG: attackIndex not 0,1, or 2 on NPC! Damage set to 1."); dd.damage = 1f; break;
		}
		dd.penetration = 0;
		dd.offense = 0;
		return dd;
	}

	// Check if particular bit is 1 (ON/TRUE) in binary format of given integer
	public bool CheckFlags (int checkInt, int flag) { return ((checkInt & flag) != 0); }

	public static float AngleInDeg(Vector3 vec1, Vector3 vec2) { return ((Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x)) * (180 / Mathf.PI)); }

	public void UseTargets (UseData ud, string targetname) {
		if (string.IsNullOrWhiteSpace(targetname)) return; // First check if targetname is valid.  This is fine if not, some triggers we just want to play the trigger's SFX and do nothing else.


		UseData tempUD = new UseData();
		float numtargetsfound = 0;
		// Find each gameobject with matching targetname in the register, then call Use for each
		for (int i=0;i<TargetRegister.Length;i++) {
			if (TargetnameRegister[i] == targetname) {
				if (TargetRegister[i] != null) {
					numtargetsfound++;
					tempUD.CopyBitsFromUseData(ud);
					if (tempUD.GOSetActive && !TargetRegister[i].activeSelf) TargetRegister[i].SetActive(true); //added activeSelf bit to keep from spamming SetActive when running targets through a trigger_multiple
					if (tempUD.GOSetDeactive && TargetRegister[i].activeSelf) TargetRegister[i].SetActive(false); // diddo for activeSelf to prevent spamming SetActive
					if (tempUD.GOToggleActive) TargetRegister[i].SetActive(!TargetRegister[i].activeSelf); // if I abuse this with a trigger_multiple someone should shoot me
					TargetIO tio = TargetRegister[i].GetComponent<TargetIO>();
					tio.Targetted(tempUD);
				} else {
					UnityEngine.Debug.Log("WARNING: null TargetRegister GameObject linked to targetname of " + targetname + ". Could not run Targetted.");
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

	public void Shake(bool effectIsWorldwide, float distance, float force) {
		if (distance == -1) distance = globalShakeDistance;
		if (force == -1) force = globalShakeForce;
		if (player1 == null) { UnityEngine.Debug.Log("WARNING: Shake() check - no host player1."); return; }  // No host player

		if (effectIsWorldwide) {
			// The whole station is a shakin' and a movin'!
			if (player1 != null) { if (player1.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player1.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
			// if (player2 != null) { if (player2.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player2.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
			// if (player3 != null) { if (player3.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player3.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
			// if (player4 != null) { if (player4.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player4.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
		} else {
			// check if player is close enough and shake em' up!
			if (player1 != null) {
				if (Vector3.Distance(transform.position, player1.GetComponent<PlayerReferenceManager>().playerCapsule.transform.position) < distance) {
					if (player1.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player1.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force);
				}
			}
			// if (Vector3.Distance(transform.position, player2.GetComponent<PlayerReferenceManager>().playerCapsule.transform.position) < distance) {
				// if (player2 != null) { if (player2.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player2.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
			// }
			// if (Vector3.Distance(transform.position, player3.GetComponent<PlayerReferenceManager>().playerCapsule.transform.position) < distance) {
				// if (player3 != null) { if (player3.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player3.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
			// }
			// if (Vector3.Distance(transform.position, player4.GetComponent<PlayerReferenceManager>().playerCapsule.transform.position) < distance) {
				// if (player4 != null) { if (player4.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>() != null) player4.GetComponent<PlayerReferenceManager>().GetComponent<MouseLookScript>().ScreenShake(force); }
			// }
		}
	}

	public int GetNPCConstIndexFromIndex23(int ref23) {
		switch (ref23) {
			case 0: return 160; // Autobomb
			case 1: return 161; // Cyborg Assassin
			case 2: return 162; // Avian Mutant
			case 3: return 163; // Exec Bot
			case 4: return 164; // Cyborg Drone
			case 5: return 165; // Cortex Reaver
			case 6: return 166; // Cyborg Warrior
			case 7: return 167; // Cyborg Enforcer
			case 8: return 168; // Cyborg Elite Guard
			case 9: return 169; // Cyborg of Edward Diego
			case 10: return 170; // Security-1 Robot
			case 11: return 171; // Security-2 Robot
			case 12: return 172; // Maintenance Bot
			case 13: return 173; // Mutated Cyborg
			case 14: return 174; // Hopper
			case 15: return 175; // Humanoid Mutant
			case 16: return 176; // Invisible Mutant
			case 17: return 177; // Virus Mutant
			case 18: return 178; // Serv-Bot
			case 19: return 179; // Flier Bot
			case 20: return 180; // Zero-G Mutant
			case 21: return 181; // Gorilla Tiger Mutant
			case 22: return 182; // Repair Bot
			case 23: return 183; // Plant Mutant
		}
		return -1;
	}

	public string CreditsStats() {
		string retval = Const.a.creditsText[0];
		int index = 0;
		char[] checkCharacters = retval.ToCharArray();
		char[] updatedCharacters = new char[checkCharacters.Length + 106];
		for (int i=0;i<updatedCharacters.Length;i++) {
			if (checkCharacters[i] == '#') {
				char[] tempChar = null;
				switch(index) {
					case 0: // Straight Time: #h
						tempChar = new char[2];
						float t = PauseScript.a.relativeTime;
						t = Mathf.Floor(t/3600f); // hours
						string s_t = t.ToString("00");
						char[] s_t_c = s_t.ToCharArray();
						tempChar[0] = s_t_c[0];
						tempChar[1] = s_t_c[1];
						break;

				}
				if (tempChar != null) {
					for (int j=0;j<tempChar.Length;j++) {
						updatedCharacters[i] = tempChar[j];
						i++;
					}
				} else {
					updatedCharacters[i] = checkCharacters[i];
				}
				index++;
			} else {
				updatedCharacters[i] = checkCharacters[i];
			}
		}

		return retval;
	}

	public void ApplyImpactForce(GameObject hitObject, float impactVelocity, Vector3 attackNormal, Vector3 hitPoint) {
		Rigidbody rbody = hitObject.GetComponent<Rigidbody>();
		if (rbody != null && impactVelocity > 0) rbody.AddForceAtPosition((attackNormal*impactVelocity*1.5f),hitPoint);
	}

	public int SafeIndex(ref int[] array, int index, int max, int failvalue) {
		if (index < 0 || index > max || index > array.Length) { UnityEngine.Debug.Log("Unexpected situation, index out of bounds passed to SafeIndex!"); return failvalue; }
		if (array.Length < 1) { UnityEngine.Debug.Log("Unexpected situation, array passed to SafeIndex was empty!"); return failvalue; }
		return array[index]; // Safe to pass the index value into the array space.
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
