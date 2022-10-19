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
using System.Diagnostics; // Stopwatch

// Global types
public enum BodyState : byte {Standing,Crouch,CrouchingDown,StandingUp,Prone,ProningDown,ProningUp};
public enum Handedness : byte {Center,LH,RH};
public enum AttackType : byte {None,Melee,MeleeEnergy,EnergyBeam,Magnetic,Projectile,ProjectileNeedle,ProjectileEnergyBeam,ProjectileLaunched,Gas,Tranq,Drill};
public enum NPCType : byte {Mutant,Supermutant,Robot,Cyborg,Supercyborg,Cyber,MutantCyborg};
public enum PerceptionLevel : byte {Low,Medium,High,Omniscient};
public enum AIState : byte {Idle,Walk,Run,Attack1,Attack2,Attack3,Pain,Dying,Dead,Inspect,Interacting};
public enum AIMoveType : byte {Walk,Fly,Swim,Cyber,None};

//Pool references
public enum PoolType {None,SparqImpacts,CameraExplosions,ProjEnemShot2,SparksSmall,BloodSpurtSmall,
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
	public AttackType[] attackTypeForWeapon;

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
	[HideInInspector] public AIMoveType[] moveTypeForNPC;
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
	public GameObject player1;

	// Layer masks
	[HideInInspector] public int layerMaskPlayerFrob;
	[HideInInspector] public int layerMaskPlayerAttack;
	[HideInInspector] public int layerMaskNPCSight;
	[HideInInspector] public int layerMaskNPCAttack;
	[HideInInspector] public int layerMaskNPCCollision;

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
	public StatusBarTextDecay statusBar;
   
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
	[HideInInspector] public int GraphicsShadowMode;
	[HideInInspector] public int GraphicsSSRMode;
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
	public GameObject[] chunkPrefabs;
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
	[HideInInspector] public List<PauseRigidbody> prb;
	[HideInInspector] public List<PauseParticleSystem> psys;
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
	private CultureInfo en_US_Culture = new CultureInfo("en-US");

	//Instance container variable
	public static Const a;

	void Awake() {
		Application.targetFrameRate = TARGET_FPS;
		a = this; // Create a new instance so that it can be accessed globally. MOST IMPORTANT PART!!
		a.justSavedTimeStamp = Time.time - a.savedReminderTime;
		a.player1CapsuleMainCameragGO = a.player1.GetComponent<PlayerReferenceManager>().playerCapsuleMainCamera;
		a.player1TargettingPos = a.player1CapsuleMainCameragGO.transform;
		a.player1Capsule = a.player1.GetComponent<PlayerReferenceManager>().playerCapsule;
		a.player1PlayerMovementScript = a.player1Capsule.GetComponent<PlayerMovement>();
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
		PauseRigidbody[] prbTemp = FindObjectsOfType<PauseRigidbody>();
		for (int i=0;i<prbTemp.Length;i++) prb.Add(prbTemp[i]);
		PauseParticleSystem[] psysTemp = FindObjectsOfType<PauseParticleSystem>();
		for (int i=0;i<psysTemp.Length;i++) psys.Add(psysTemp[i]);
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
		GraphicsShadowMode = AssignConfigInt("Graphics", "Shadows");
		GraphicsSSRMode = AssignConfigInt("Graphics", "SSR");
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
		INIWorker.IniWriteValue("Graphics","Fullscreen",Utils.BoolToString(GraphicsFullscreen));
		INIWorker.IniWriteValue("Graphics","SSAO",Utils.BoolToString(GraphicsSSAO));
		INIWorker.IniWriteValue("Graphics","Bloom",Utils.BoolToString(GraphicsBloom));
		INIWorker.IniWriteValue("Graphics","FOV",GraphicsFOV.ToString());
		INIWorker.IniWriteValue("Graphics","AA",GraphicsAAMode.ToString());
		INIWorker.IniWriteValue("Graphics","Shadows",GraphicsShadowMode.ToString());
		INIWorker.IniWriteValue("Grpahics","SSR",GraphicsSSRMode.ToString());
		INIWorker.IniWriteValue("Graphics","Gamma",GraphicsGamma.ToString());
		INIWorker.IniWriteValue("Audio","SpeakerMode",AudioSpeakerMode.ToString());
		INIWorker.IniWriteValue("Audio","Reverb",Utils.BoolToString(AudioReverb));
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
		INIWorker.IniWriteValue("Input","InvertLook",Utils.BoolToString(InputInvertLook));
		INIWorker.IniWriteValue("Input","InvertCyberspaceLook",Utils.BoolToString(InputInvertCyberspaceLook));
		INIWorker.IniWriteValue("Input","InvertInventoryCycling",Utils.BoolToString(InputInvertInventoryCycling));
		INIWorker.IniWriteValue("Input","QuickItemPickup",Utils.BoolToString(InputQuickItemPickup));
		INIWorker.IniWriteValue("Input","QuickReloadWeapons",Utils.BoolToString(InputQuickReloadWeapons));
		SetBloom();
		SetSSAO();
		SetAA();
	}

	private void LoadAudioLogMetaData () {
		// The following to be assigned to the arrays in the Unity Const data structure
		int readIndexOfLog, readLogImageLHIndex, readLogImageRHIndex; // look-up index for assigning the following data on the line in the file to the arrays
		string readLogText; // loaded into string audioLogSpeech2Text[]
		string readline; // variable to hold each string read in from the file
		char logSplitChar = ',';
		StreamReader dataReader = new StreamReader(Application.dataPath + "/StreamingAssets/logs_text.txt",Encoding.ASCII);
		using (dataReader) {
			do {
				int i = 0;
				// Read the next line
				readline = dataReader.ReadLine();
				if (readline == null) continue; // just in case
				string[] entries = readline.Split(logSplitChar);
				readIndexOfLog = Utils.GetIntFromString(entries[i]); i++;
				readLogImageLHIndex = Utils.GetIntFromString(entries[i]); i++;
				readLogImageRHIndex = Utils.GetIntFromString(entries[i]); i++;
				audioLogImagesRefIndicesLH[readIndexOfLog] = readLogImageLHIndex;
				audioLogImagesRefIndicesRH[readIndexOfLog] = readLogImageRHIndex;
				audiologNames[readIndexOfLog] = entries[i]; i++;
				audiologSenders[readIndexOfLog] = entries[i]; i++;
				audiologSubjects[readIndexOfLog] = entries[i]; i++;
				audioLogType[readIndexOfLog] = Utils.GetIntFromString(entries[i]); i++;
				audioLogLevelFound[readIndexOfLog] = Utils.GetIntFromString(entries[i]); i++;
				readLogText = entries[i]; i++;
				// handle extra commas within the body text and append remaining portions of the line
				if (entries.Length > 8) {
					for (int j=9;j<entries.Length;j++) {
						readLogText = (readLogText +"," + entries[j]);  // combine remaining portions of text after other commas and add comma back
					}
				}
				audioLogSpeech2Text[readIndexOfLog] = readLogText;
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
				delayBetweenShotsForWeapon[currentline] = Utils.GetFloatFromString(entries[i]); i++;
				delayBetweenShotsForWeapon2[currentline] = Utils.GetFloatFromString(entries[i]); i++;
				damagePerHitForWeapon[currentline] = Utils.GetFloatFromString(entries[i]); i++;
				damagePerHitForWeapon2[currentline] = Utils.GetFloatFromString(entries[i]); i++;
				damageOverloadForWeapon[currentline] = Utils.GetFloatFromString(entries[i]); i++;
				energyDrainLowForWeapon[currentline] = Utils.GetFloatFromString(entries[i]); i++;
				energyDrainHiForWeapon[currentline] = Utils.GetFloatFromString(entries[i]); i++;
				energyDrainOverloadForWeapon[currentline] = Utils.GetFloatFromString(entries[i]); i++;
				penetrationForWeapon[currentline] = Utils.GetFloatFromString(entries[i]); i++;
				penetrationForWeapon2[currentline] = Utils.GetFloatFromString(entries[i]); i++;
				offenseForWeapon[currentline] = Utils.GetFloatFromString(entries[i]); i++;
				offenseForWeapon2[currentline] = Utils.GetFloatFromString(entries[i]); i++;
				rangeForWeapon[currentline] = Utils.GetFloatFromString(entries[i]); i++;
				readInt = Utils.GetIntFromString(entries[i]); i++;
				attackTypeForWeapon[currentline] = Utils.GetAttackTypeFromInt(readInt);
				currentline++;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}

	private void LoadCreditsData () {
		creditsText = new string[21];
		string readline; // variable to hold each string read in from the file
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
				if (currentline == 0) a.startingNewGame = Utils.GetBoolFromString(readline);
				if (currentline == 1) a.freshRun = Utils.GetBoolFromString(readline);
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
				sw.WriteLine(Utils.BoolToString(bitStartingNew));
				sw.WriteLine(Utils.BoolToString(bitFreshRun));
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
		moveTypeForNPC = new AIMoveType[numberOfNPCs];
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

		//case 0: return AIMoveType.None;
		//case 1: return AIMoveType.Walk;
		//case 2: return AIMoveType.Fly;
		//case 3: return AIMoveType.Swim;
		//case 4: return AIMoveType.Cyber;

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

				refIndex = Utils.GetIntFromString(entries[i+1]); // Index is stored at 2nd spot
				if (refIndex < 0 || refIndex > 28) continue; // Invalid value, skip
				nameForNPC[refIndex] = entries[i].Trim(); i++;
				i++; // No need to read the index again so we skip over it.
				readInt = Utils.GetIntFromString(entries[i].Trim()); attackTypeForNPC[refIndex] = Utils.GetAttackTypeFromInt(readInt); i++; 
				readInt = Utils.GetIntFromString(entries[i].Trim()); attackTypeForNPC2[refIndex] = Utils.GetAttackTypeFromInt(readInt); i++; 
				readInt = Utils.GetIntFromString(entries[i].Trim()); attackTypeForNPC3[refIndex] = Utils.GetAttackTypeFromInt(readInt); i++; 
				damageForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				damageForNPC2[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				damageForNPC3[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				rangeForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				rangeForNPC2[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				rangeForNPC3[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				healthForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				healthForCyberNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				readInt = Utils.GetIntFromString(entries[i].Trim()); perceptionForNPC[refIndex] = Utils.GetPerceptionLevelFromInt(readInt); i++;
				disruptabilityForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				armorvalueForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				defenseForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				moveTypeForNPC[refIndex] = Utils.GetMoveTypeFromInt(Utils.GetIntFromString(entries[i].Trim())); i++;
				yawSpeedForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				fovForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				fovAttackForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				fovStartMovementForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				distToSeeBehindForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				sightRangeForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				walkSpeedForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				runSpeedForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				attack1SpeedForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				attack2SpeedForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				attack3SpeedForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				attack3ForceForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				attack3RadiusForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeToPainForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeBetweenPainForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeTillDeadForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeToActualAttack1ForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeToActualAttack2ForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeToActualAttack3ForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeBetweenAttack1ForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeBetweenAttack2ForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeBetweenAttack3ForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeToChangeEnemyForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeIdleSFXMinForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeIdleSFXMaxForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeAttack1WaitMinForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeAttack1WaitMaxForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeAttack1WaitChanceForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeAttack2WaitMinForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeAttack2WaitMaxForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeAttack2WaitChanceForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeAttack3WaitMinForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeAttack3WaitMaxForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeAttack3WaitChanceForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				readInt = Utils.GetIntFromString(entries[i].Trim()); i++; //attack1ProjectileLaunchedTypeForNPC[refIndex] = GetPoolType(readInt);
				readInt = Utils.GetIntFromString(entries[i].Trim()); i++; //attack2ProjectileLaunchedTypeForNPC[refIndex] = GetPoolType(readInt);
				readInt = Utils.GetIntFromString(entries[i].Trim()); i++; //attack3ProjectileLaunchedTypeForNPC[refIndex] = GetPoolType(readInt); // Not worrying about projectile type for now, would require indexing all of the pools.
				projectileSpeedAttack1ForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				projectileSpeedAttack2ForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				projectileSpeedAttack3ForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				hasLaserOnAttack1ForNPC[refIndex] = Utils.GetBoolFromString(entries[i].Trim()); i++;
				hasLaserOnAttack2ForNPC[refIndex] = Utils.GetBoolFromString(entries[i].Trim()); i++;
				hasLaserOnAttack3ForNPC[refIndex] = Utils.GetBoolFromString(entries[i].Trim()); i++;
				explodeOnAttack3ForNPC[refIndex] = Utils.GetBoolFromString(entries[i].Trim()); i++;
				preactivateMeleeCollidersForNPC[refIndex] = Utils.GetBoolFromString(entries[i].Trim()); i++;
				huntTimeForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				flightHeightForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				flightHeightIsPercentageForNPC[refIndex] = Utils.GetBoolFromString(entries[i].Trim()); i++;
				switchMaterialOnDeathForNPC[refIndex] = Utils.GetBoolFromString(entries[i].Trim()); i++;
				hearingRangeForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				timeForTranquilizationForNPC[refIndex] = Utils.GetFloatFromString(entries[i].Trim()); i++;
				hopsOnMoveForNPC[refIndex] = Utils.GetBoolFromString(entries[i].Trim()); i++;
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
		#if UNITY_EDITOR
			UnityEngine.Debug.Log(input); // Print to Editor console.
		#endif
		a.statusBar.SendText(input);
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
		PlayerHealth ph = PlayerHealth.a;
		PlayerEnergy pe = PlayerEnergy.a;
		PlayerMovement pm = PlayerMovement.a;
		PlayerPatch pp = PlayerPatch.a;
		HealthManager hm = pCap.GetComponent<HealthManager>();
		tr = pCap.transform;
		MouseLookScript ml = MouseLookScript.a;
		trml = MouseLookScript.a.transform;
		Inventory inv = Inventory.a;
		MFDManager mfd = MFDManager.a;
		SaveObject sav = plyr.GetComponent<SaveObject>();

		line = sav.saveableType;
		line += Utils.splitChar + sav.SaveID.ToString();
		string pname = string.IsNullOrEmpty(Const.a.playerName) ? "Hacker" : Const.a.playerName;
		line += Utils.splitChar + pname;
		line += Utils.splitChar + Utils.FloatToString(ph.radiated); // float
		line += Utils.splitChar + Utils.FloatToString(ph.timer); // float, not relative timer
		line += Utils.splitChar + Utils.BoolToString(ph.playerDead); // bool
		line += Utils.splitChar + Utils.BoolToString(ph.radiationArea); // bool
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(ph.mediPatchPulseFinished); // float
		line += Utils.splitChar + ph.mediPatchPulseCount.ToString(); // int
		line += Utils.splitChar + Utils.BoolToString(ph.makingNoise); // bool
		line += Utils.splitChar + Utils.FloatToString(ph.lastHealth); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(ph.painSoundFinished); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(ph.radSoundFinished); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(ph.radFXFinished); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pe.tickFinished); // float
		line += Utils.splitChar + Utils.FloatToString(pm.playerSpeed); // float
		line += Utils.splitChar + Utils.BoolToString(pm.grounded); // bool
		line += Utils.splitChar + Utils.FloatToString(pm.currentCrouchRatio); // float
		line += Utils.splitChar + Utils.BodyStateToInt(pm.bodyState).ToString(); // int
		line += Utils.splitChar + Utils.BoolToString(pm.ladderState); // bool
		line += Utils.splitChar + Utils.BoolToString(pm.gravliftState); // bool
		line += Utils.splitChar + Utils.BoolToString(pm.inCyberSpace); // bool
		for (j=0;j<4096;j++) { line += Utils.splitChar + Utils.BoolToString(pm.automapExploredR[j]); } // bool
		for (j=0;j<4096;j++) { line += Utils.splitChar + Utils.BoolToString(pm.automapExplored1[j]); } // bool
		for (j=0;j<4096;j++) { line += Utils.splitChar + Utils.BoolToString(pm.automapExplored2[j]); } // bool
		for (j=0;j<4096;j++) { line += Utils.splitChar + Utils.BoolToString(pm.automapExplored3[j]); } // bool
		for (j=0;j<4096;j++) { line += Utils.splitChar + Utils.BoolToString(pm.automapExplored4[j]); } // bool
		for (j=0;j<4096;j++) { line += Utils.splitChar + Utils.BoolToString(pm.automapExplored5[j]); } // bool
		for (j=0;j<4096;j++) { line += Utils.splitChar + Utils.BoolToString(pm.automapExplored6[j]); } // bool
		for (j=0;j<4096;j++) { line += Utils.splitChar + Utils.BoolToString(pm.automapExplored7[j]); } // bool
		for (j=0;j<4096;j++) { line += Utils.splitChar + Utils.BoolToString(pm.automapExplored8[j]); } // bool
		for (j=0;j<4096;j++) { line += Utils.splitChar + Utils.BoolToString(pm.automapExplored9[j]); } // bool
		for (j=0;j<4096;j++) { line += Utils.splitChar + Utils.BoolToString(pm.automapExploredG1[j]); } // bool
		for (j=0;j<4096;j++) { line += Utils.splitChar + Utils.BoolToString(pm.automapExploredG2[j]); } // bool
		for (j=0;j<4096;j++) { line += Utils.splitChar + Utils.BoolToString(pm.automapExploredG4[j]); } // bool
		line += Utils.splitChar + Utils.BoolToString(pm.CheatWallSticky); // bool
		line += Utils.splitChar + Utils.BoolToString(pm.CheatNoclip); // bool
		line += Utils.splitChar + Utils.FloatToString(pm.jumpTime); // float, not a timer
		line += Utils.splitChar + Utils.FloatToString(pm.oldVelocity.x) + Utils.splitChar
						  + Utils.FloatToString(pm.oldVelocity.y) + Utils.splitChar
						  + Utils.FloatToString(pm.oldVelocity.z); // Vector3 (float|float|float)
		line += Utils.splitChar + Utils.FloatToString(pm.fatigue); // float
		line += Utils.splitChar + Utils.BoolToString(pm.justJumped); // bool
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pm.fatigueFinished); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pm.fatigueFinished2); // float
		line += Utils.splitChar + Utils.BoolToString(pm.cyberSetup); // bool
		line += Utils.splitChar + Utils.BoolToString(pm.cyberDesetup); // bool
		line += Utils.splitChar + Utils.BodyStateToInt(pm.oldBodyState).ToString(); // int
		line += Utils.splitChar + Utils.FloatToString(pm.leanTarget); // float
		line += Utils.splitChar + Utils.FloatToString(pm.leanShift); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pm.jumpSFXFinished); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pm.jumpLandSoundFinished); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pm.jumpJetEnergySuckTickFinished); // float
		line += Utils.splitChar + Utils.BoolToString(pm.fatigueWarned); // bool
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pm.ressurectingFinished); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pm.doubleJumpFinished); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pm.turboFinished); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.berserkFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.berserkIncrementFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.detoxFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.geniusFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.mediFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.reflexFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.sightFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.sightSideEffectFinishedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(pp.staminupFinishedTime); // float
		line += Utils.splitChar + pp.berserkIncrement.ToString(); // int
		line += Utils.splitChar + pp.patchActive.ToString(); // int
		line += Utils.splitChar + Utils.FloatToString(tr.localPosition.x) + Utils.splitChar
						  + Utils.FloatToString(tr.localPosition.y) + Utils.splitChar
						  + Utils.FloatToString(tr.localPosition.z); // Vector3 (float|float|float)
		line += Utils.splitChar + Utils.FloatToString(tr.localRotation.x) + Utils.splitChar
						  + Utils.FloatToString(tr.localRotation.y) + Utils.splitChar
						  + Utils.FloatToString(tr.localRotation.z) + Utils.splitChar
						  + Utils.FloatToString(tr.localRotation.w); // Quaternion (float|float|float|float)
		line += Utils.splitChar + Utils.FloatToString(tr.localScale.x) + Utils.splitChar
						  + Utils.FloatToString(tr.localScale.y) + Utils.splitChar
						  + Utils.FloatToString(tr.localScale.z); // Vector3 (float|float|float)
		line += Utils.splitChar + Utils.FloatToString(trml.localPosition.x) + Utils.splitChar
						  + Utils.FloatToString(trml.localPosition.y) + Utils.splitChar
						  + Utils.FloatToString(trml.localPosition.z); // Vector3 (float|float|float)
		line += Utils.splitChar + Utils.FloatToString(trml.localRotation.x) + Utils.splitChar
						  + Utils.FloatToString(trml.localRotation.y) + Utils.splitChar
						  + Utils.FloatToString(trml.localRotation.z) + Utils.splitChar
						  + Utils.FloatToString(trml.localRotation.w); // Quaternion (float|float|float|float)
		line += Utils.splitChar + Utils.FloatToString(trml.localScale.x) + Utils.splitChar
						  + Utils.FloatToString(trml.localScale.y) + Utils.splitChar
						  + Utils.FloatToString(trml.localScale.z); // Vector3 (float|float|float)
		line += Utils.splitChar + Utils.BoolToString(ml.inventoryMode); // bool
		line += Utils.splitChar + Utils.BoolToString(ml.holdingObject); // bool
		line += Utils.splitChar + ml.heldObjectIndex.ToString(); // int
		line += Utils.splitChar + ml.heldObjectCustomIndex.ToString(); // int
		line += Utils.splitChar + ml.heldObjectAmmo.ToString(); // int
		line += Utils.splitChar + ml.heldObjectAmmo2.ToString(); // int
		line += Utils.splitChar + Utils.BoolToString(ml.firstTimePickup); // bool
		line += Utils.splitChar + Utils.BoolToString(ml.firstTimeSearch); // bool
		line += Utils.splitChar + Utils.BoolToString(ml.grenadeActive); // bool
		line += Utils.splitChar + Utils.BoolToString(ml.inCyberSpace); // bool
		line += Utils.splitChar + Utils.FloatToString(ml.yRotation); // float
		line += Utils.splitChar + Utils.BoolToString(ml.geniusActive); // bool
		line += Utils.splitChar + Utils.FloatToString(ml.xRotation); // float
		line += Utils.splitChar + Utils.BoolToString(ml.vmailActive); // bool
		line += Utils.splitChar + Utils.FloatToString(ml.cyberspaceReturnPoint.x) + Utils.splitChar
						  + Utils.FloatToString(ml.cyberspaceReturnPoint.y) + Utils.splitChar
						  + Utils.FloatToString(ml.cyberspaceReturnPoint.z);
		line += Utils.splitChar + Utils.FloatToString(ml.cyberspaceReturnCameraLocalRotation.x) + Utils.splitChar
						  + Utils.FloatToString(ml.cyberspaceReturnCameraLocalRotation.y) + Utils.splitChar
						  + Utils.FloatToString(ml.cyberspaceReturnCameraLocalRotation.z);
		line += Utils.splitChar + Utils.FloatToString(ml.cyberspaceReturnPlayerCapsuleLocalRotation.x) + Utils.splitChar
						  + Utils.FloatToString(ml.cyberspaceReturnPlayerCapsuleLocalRotation.y) + Utils.splitChar
						  + Utils.FloatToString(ml.cyberspaceReturnPlayerCapsuleLocalRotation.z);
		line += Utils.splitChar + Utils.FloatToString(ml.cyberspaceRecallPoint.x) + Utils.splitChar
						  + Utils.FloatToString(ml.cyberspaceRecallPoint.y) + Utils.splitChar
						  + Utils.FloatToString(ml.cyberspaceRecallPoint.z); // Vector3 (float|float|float)
		line += Utils.splitChar + ml.cyberspaceReturnLevel.ToString(); // int
		line += Utils.splitChar + Utils.FloatToString(hm.health); // float
		line += Utils.splitChar + Utils.FloatToString(hm.cyberHealth); // float
		line += Utils.splitChar + Utils.BoolToString(hm.deathDone); // bool
		line += Utils.splitChar + Utils.BoolToString(hm.god); // bool
		line += Utils.splitChar + Utils.BoolToString(hm.teleportDone); // bool
		line += Utils.splitChar + GUIState.a.overButtonType.ToString(); // int
		line += Utils.splitChar + Utils.BoolToString(GUIState.a.overButton); // bool
		for (j=0;j<7;j++) { line += Utils.splitChar + inv.weaponInventoryIndices[j].ToString(); } // int
		for (j=0;j<7;j++) { line += Utils.splitChar + inv.weaponInventoryAmmoIndices[j].ToString(); } // int
		line += Utils.splitChar + inv.numweapons.ToString(); // int
		for (j=0;j<16;j++) { line += Utils.splitChar + inv.wepAmmo[j].ToString(); } // int
		for (j=0;j<16;j++) { line += Utils.splitChar + inv.wepAmmoSecondary[j].ToString(); } // int
		for (j=0;j<7;j++) { line += Utils.splitChar + Utils.FloatToString(inv.currentEnergyWeaponHeat[j]); } // float
		for (j=0;j<7;j++) { line += Utils.splitChar + Utils.BoolToString(inv.wepLoadedWithAlternate[j]); } // bool
		line += Utils.splitChar + WeaponCurrent.WepInstance.weaponCurrent.ToString(); // int
		line += Utils.splitChar + WeaponCurrent.WepInstance.weaponIndex.ToString(); // int
		for (j=0;j<7;j++) { line += Utils.splitChar + Utils.FloatToString(WeaponCurrent.WepInstance.weaponEnergySetting[j]); } // float
		for (j=0;j<7;j++) { line += Utils.splitChar + WeaponCurrent.WepInstance.currentMagazineAmount[j].ToString(); } // int
		for (j=0;j<7;j++) { line += Utils.splitChar + WeaponCurrent.WepInstance.currentMagazineAmount2[j].ToString(); } // int
		line += Utils.splitChar + Utils.BoolToString(WeaponCurrent.WepInstance.justChangedWeap); // bool
		line += Utils.splitChar + WeaponCurrent.WepInstance.lastIndex.ToString(); // int
		line += Utils.splitChar + Utils.BoolToString(WeaponCurrent.WepInstance.bottomless); // bool
		line += Utils.splitChar + Utils.BoolToString(WeaponCurrent.WepInstance.redbull); // bool
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(WeaponCurrent.WepInstance.reloadFinished); // float
		line += Utils.splitChar + Utils.FloatToString(WeaponCurrent.WepInstance.reloadLerpValue); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(WeaponCurrent.WepInstance.lerpStartTime); // float
		line += Utils.splitChar + Utils.FloatToString(WeaponCurrent.WepInstance.targetY); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(WeaponFire.a.waitTilNextFire); // float
		line += Utils.splitChar + Utils.BoolToString(WeaponFire.a.overloadEnabled); // bool
		line += Utils.splitChar + Utils.FloatToString(WeaponFire.a.sparqSetting); // float
		line += Utils.splitChar + Utils.FloatToString(WeaponFire.a.ionSetting); // float
		line += Utils.splitChar + Utils.FloatToString(WeaponFire.a.blasterSetting); // float
		line += Utils.splitChar + Utils.FloatToString(WeaponFire.a.plasmaSetting); // float
		line += Utils.splitChar + Utils.FloatToString(WeaponFire.a.stungunSetting); // float
		line += Utils.splitChar + Utils.BoolToString(WeaponFire.a.recoiling); // bool
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(WeaponFire.a.justFired); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(WeaponFire.a.energySliderClickedTime); // float
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(WeaponFire.a.cyberWeaponAttackFinished); // float
		line += Utils.splitChar + inv.grenadeCurrent.ToString(); // int
		line += Utils.splitChar + inv.grenadeIndex.ToString(); // int
		line += Utils.splitChar + Utils.FloatToString(inv.nitroTimeSetting); // float
		line += Utils.splitChar + Utils.FloatToString(inv.earthShakerTimeSetting); // float
		for (j=0;j<7;j++) { line += Utils.splitChar + inv.grenAmmo[j].ToString(); } // int
		line += Utils.splitChar + inv.patchCurrent.ToString(); // int
		line += Utils.splitChar + inv.patchIndex.ToString(); // int
		for (j=0;j<7;j++) { line += Utils.splitChar + inv.patchCounts[j].ToString(); } // int
		for (j=0;j<134;j++) { line += Utils.splitChar + Utils.BoolToString(inv.hasLog[j]); } // bool
		for (j=0;j<134;j++) { line += Utils.splitChar + Utils.BoolToString(inv.readLog[j]); } // bool
		for (j=0;j<10;j++) { line += Utils.splitChar + inv.numLogsFromLevel[j].ToString(); } // int
		line += Utils.splitChar + inv.lastAddedIndex.ToString(); // int
		line += Utils.splitChar + Utils.BoolToString(inv.beepDone); // bool
		for (j=0;j<13;j++) { line += Utils.splitChar + Utils.BoolToString(inv.hasHardware[j]); } // bool
		for (j=0;j<13;j++) { line += Utils.splitChar + inv.hardwareVersion[j].ToString(); } // int
		for (j=0;j<13;j++) { line += Utils.splitChar + inv.hardwareVersionSetting[j].ToString(); } // int
		line += Utils.splitChar + inv.hardwareInvCurrent.ToString(); // int
		line += Utils.splitChar + inv.hardwareInvIndex.ToString(); // int
		for (j=0;j<13;j++) { line += Utils.splitChar + Utils.BoolToString(inv.hardwareIsActive[j]); } // bool
		for (j=0;j<32;j++) {
			switch (inv.accessCardsOwned[j]) {
				case Door.accessCardType.None: line+= Utils.splitChar + "0"; break;
				case Door.accessCardType.Standard: line+= Utils.splitChar + "1"; break;
				case Door.accessCardType.Medical: line+= Utils.splitChar + "2"; break;
				case Door.accessCardType.Science: line+= Utils.splitChar + "3"; break;
				case Door.accessCardType.Admin: line+= Utils.splitChar + "4"; break;
				case Door.accessCardType.Group1: line+= Utils.splitChar + "5"; break;
				case Door.accessCardType.Group2: line+= Utils.splitChar + "6"; break;
				case Door.accessCardType.Group3: line+= Utils.splitChar + "7"; break;
				case Door.accessCardType.Group4: line+= Utils.splitChar + "8"; break;
				case Door.accessCardType.GroupA: line+= Utils.splitChar + "9"; break;
				case Door.accessCardType.GroupB: line+= Utils.splitChar + "10"; break;
				case Door.accessCardType.Storage: line+= Utils.splitChar + "11"; break;
				case Door.accessCardType.Engineering: line+= Utils.splitChar + "12"; break;
				case Door.accessCardType.Maintenance: line+= Utils.splitChar + "13"; break;
				case Door.accessCardType.Security: line+= Utils.splitChar + "14"; break;
				case Door.accessCardType.Per1: line+= Utils.splitChar + "15"; break;
				case Door.accessCardType.Per2: line+= Utils.splitChar + "16"; break;
				case Door.accessCardType.Per3: line+= Utils.splitChar + "17"; break;
				case Door.accessCardType.Per4: line+= Utils.splitChar + "18"; break;
				case Door.accessCardType.Per5: line+= Utils.splitChar + "19"; break;
				case Door.accessCardType.Command: line+= Utils.splitChar + "20"; break;
			}
		}
		for (j=0;j<14;j++) { line += Utils.splitChar + inv.generalInventoryIndexRef[j].ToString(); } // int
		line += Utils.splitChar + inv.generalInvCurrent.ToString(); // int
		line += Utils.splitChar + inv.generalInvIndex.ToString(); // int
		line += Utils.splitChar + inv.currentCyberItem.ToString(); // int
		line += Utils.splitChar + Utils.BoolToString(inv.isPulserNotDrill); // bool
		for (j=0;j<7;j++) { line += Utils.splitChar + inv.softVersions[j].ToString(); } // int 
		for (j=0;j<7;j++) { line += Utils.splitChar + Utils.BoolToString(inv.hasSoft[j]); } // bool
		line += Utils.splitChar + inv.emailCurrent.ToString(); // int
		line += Utils.splitChar + inv.emailIndex.ToString(); // int
		line += Utils.splitChar + Utils.BoolToString(mfd.lastWeaponSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastItemSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastAutomapSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastTargetSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastDataSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastSearchSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastLogSideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastLogSecondarySideRH); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.lastMinigameSideRH); // bool
		line += Utils.splitChar + Utils.FloatToString(mfd.objectInUsePos.x) + Utils.splitChar
						  + Utils.FloatToString(mfd.objectInUsePos.y) + Utils.splitChar
						  + Utils.FloatToString(mfd.objectInUsePos.z);
		// tetheredPGP
		// tetheredPWP
		// tetheredSearchable
		// tetheredKeypadElevator
		// tetheredKeypadKeycode
		line += Utils.splitChar + Utils.BoolToString(mfd.paperLogInUse); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.usingObject); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.logReaderContainer.activeSelf); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.DataReaderContentTab.activeSelf); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.logTable.activeSelf); // bool
		line += Utils.splitChar + Utils.BoolToString(mfd.logLevelsFolder.activeSelf); // bool
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(mfd.logFinished);
		line += Utils.splitChar + Utils.BoolToString(mfd.logActive); // bool
		line += Utils.splitChar + mfd.logType.ToString(); // int
		line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(mfd.cyberTimer.GetComponent<CyberTimer>().timerFinished);
		return line;
	}

	// Info about the enemy's current state
	public string SaveNPCData(GameObject go) {
		s2.Clear();
		if (go == null) { UnityEngine.Debug.Log("BUG: attempting to SaveNPCData for null GameObject go"); }
		//string line = System.String.Empty;
		AIController aic = go.GetComponent<AIController>();
		AIAnimationController aiac = go.GetComponentInChildren<AIAnimationController>();
		if (aic != null) {
			if (!aic.startInitialized) aic.Start();
			s2.Append(aic.index.ToString()); // int
			s2.Append(Utils.splitChar);
			switch (aic.currentState) {
				case AIState.Walk: s2.Append("1"); break;
				case AIState.Run: s2.Append("2"); break;
				case AIState.Attack1: s2.Append("3");  break;
				case AIState.Attack2: s2.Append("4");  break;
				case AIState.Attack3: s2.Append("5");  break;
				case AIState.Pain: s2.Append("6");  break;
				case AIState.Dying: s2.Append("7");  break;
				case AIState.Inspect: s2.Append("8");  break;
				case AIState.Interacting: s2.Append("9");  break;
				case AIState.Dead: s2.Append("10");  break;
				default: s2.Append("0");  break;
			}
			s2.Append(Utils.splitChar);
			if (aic.enemy != null) {
				SaveObject so = aic.enemy.GetComponent<SaveObject>();
				if (so != null) {
					s2.Append(so.SaveID.ToString());
					//line += Utils.splitChar + so.SaveID.ToString(); // saveID of NPC's enemy
				} else {
					s2.Append("-1");
					UnityEngine.Debug.Log("BUG: SaveNPCData missing SaveObject on aic.enemy with index " + aic.index.ToString());
					//line += Utils.splitChar + "-1";
				}
			} else {
				s2.Append("-1");
				//line += Utils.splitChar + "-1";
			}
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.gracePeriodFinished));
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.meleeDamageFinished));
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.inSight)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.infront)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.inProjFOV)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.LOSpossible)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.goIntoPain)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.rangeToEnemy));
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.firstSighting)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.dyingSetup)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.ai_dying)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.ai_dead)); // bool
			s2.Append(Utils.splitChar); s2.Append(aic.currentWaypoint.ToString()); // int
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.currentDestination.x));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.currentDestination.y));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.currentDestination.z));
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.timeTillEnemyChangeFinished));
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.timeTillDeadFinished));
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.timeTillPainFinished));
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.tickFinished));
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.raycastingTickFinished));
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.huntFinished));
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.hadEnemy)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.lastKnownEnemyPos.x));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.lastKnownEnemyPos.y));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.lastKnownEnemyPos.z));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.tempVec.x));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.tempVec.y));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.tempVec.z));
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.shotFired)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.randomWaitForNextAttack1Finished));
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.randomWaitForNextAttack2Finished));
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.randomWaitForNextAttack3Finished));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.idealTransformForward.x));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.idealTransformForward.y));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.idealTransformForward.z));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.idealPos.x));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.idealPos.y));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.idealPos.z));
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.attackFinished));
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.attack2Finished));
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.attack3Finished));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.targettingPosition.x));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.targettingPosition.y));
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aic.targettingPosition.z));
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.deathBurstFinished));
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.deathBurstDone)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.asleep)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.tranquilizeFinished));
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.hopDone)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aic.wandering)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aic.wanderFinished));
			s2.Append(Utils.splitChar); s2.Append(HealthManager.Save(aic.healthManager.gameObject));
		} else {
			UnityEngine.Debug.Log("BUG: SaveNPCData missing AIController");
		}
		if (aiac != null) {
			s2.Append(Utils.splitChar); s2.Append(Utils.FloatToString(aiac.currentClipPercentage));
			s2.Append(Utils.splitChar); s2.Append(Utils.BoolToString(aiac.dying)); // bool
			s2.Append(Utils.splitChar); s2.Append(Utils.SaveRelativeTimeDifferential(aiac.animSwapFinished));
		}
		//12
		if (s2 != null) return s2.ToString();
		return System.String.Empty;
	}

	public string SaveSpawnerData(GameObject go) {
		string line = System.String.Empty;
		SpawnManager sm = go.GetComponent<SpawnManager>();
		if (sm != null) {
			line = Utils.BoolToString(sm.active); // bool - is this enabled
			line += Utils.splitChar + sm.numberActive.ToString(); // int - number spawned
			line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(sm.delayFinished); // float - time that we need to spawn next
		} else {
			UnityEngine.Debug.Log("SpawnManager missing on savetype of SpawnManager! GameObject.name: " + go.name);
		}
		//3
		return line;
	}	

	public string SaveInteractablePanelData(GameObject go) {
		string line = System.String.Empty;
		InteractablePanel ip = go.GetComponent<InteractablePanel>();
		if (ip != null) {
			line = Utils.BoolToString(ip.open); // bool - is the panel opened
			line += Utils.splitChar + Utils.BoolToString(ip.installed); // bool - is the item installed, MAKE SURE YOU ENABLE THE INSTALL ITEM GameObject IN LOAD
		} else {
			UnityEngine.Debug.Log("InteractablePanel missing on savetype of InteractablePanel! GameObject.name: " + go.name);
		}
		//2
		return line;
	}		

	public string SaveElevatorPanelData(GameObject go) {
		string line = System.String.Empty;
		KeypadElevator ke = go.GetComponent<KeypadElevator>();
		if (ke != null) {
			line = Utils.BoolToString(ke.padInUse); // bool - is the pad being used by a player
			line += Utils.splitChar + Utils.BoolToString(ke.locked); // bool - locked?
		} else {
			UnityEngine.Debug.Log("KeypadElevator missing on savetype of KeypadElevator! GameObject.name: " + go.name);
		}
		//2
		return line;
	}	

	public string SaveKeypadData(GameObject go) {
		string line = System.String.Empty;
		KeypadKeycode kk = go.GetComponent<KeypadKeycode>();
		if (kk != null) {
			line = Utils.BoolToString(kk.padInUse); // bool - is the pad being used by a player
			line += Utils.splitChar + Utils.BoolToString(kk.locked); // bool - locked?
			line += Utils.splitChar + Utils.BoolToString(kk.solved); // bool - already entered correct keycode?
		} else {
			UnityEngine.Debug.Log("KeypadKeycode missing on savetype of KeypadKeycode! GameObject.name: " + go.name);
		}
		//3
		return line;
	}	

	public string SavePuzzleGridData(GameObject go) {
		string line = System.String.Empty;
		PuzzleGridPuzzle pgp = go.GetComponent<PuzzleGridPuzzle>();
		if (pgp != null) {
			line = Utils.BoolToString(pgp.puzzleSolved); // bool - is this puzzle already solved?
			for (int i=0;i<35;i++) { line += Utils.splitChar + Utils.BoolToString(pgp.grid[i]); } // bool - get the current grid states + or X
			line += Utils.splitChar + Utils.BoolToString(pgp.fired); // bool - have we already fired yet?
			line += Utils.splitChar + Utils.BoolToString(pgp.locked); // bool - is this locked?
		} else {
			UnityEngine.Debug.Log("PuzzleGridData missing on savetype of PuzzleGrid! GameObject.name: " + go.name);
		}
		//38
		return line;
	}	

	public string SavePuzzleWireData(GameObject go) {
		string line = System.String.Empty;
		PuzzleWirePuzzle pwp = go.GetComponent<PuzzleWirePuzzle>();
		if (pwp != null) {
			line = Utils.BoolToString(pwp.puzzleSolved); // bool - is this puzzle already solved?
			for (int i=0;i<7;i++) { line += Utils.splitChar + pwp.currentPositionsLeft[i].ToString(); } // int - get the current wire positions
			for (int i=0;i<7;i++) { line += Utils.splitChar + pwp.currentPositionsRight[i].ToString(); } // int - get the current wire positions
			line += Utils.splitChar + Utils.BoolToString(pwp.locked); // bool - is this locked?
		} else {
			UnityEngine.Debug.Log("PuzzleWirePuzzle missing on savetype of PuzzleWire! GameObject.name: " + go.name);
		}
		//16
		return line;
	}

	public string SaveTCounterData(GameObject go) {
		string line = System.String.Empty;
		TriggerCounter tc = go.GetComponent<TriggerCounter>();
		if (tc != null) {
			line = tc.counter.ToString(); // int - how many counts we have
		} else {
			UnityEngine.Debug.Log("TriggerCounter missing on savetype of TriggerCounter! GameObject.name: " + go.name);
		}
		//1
		return line;	
	}

	public string SaveTGravityData(GameObject go) {
		string line = System.String.Empty;
		GravityLift gl = go.GetComponent<GravityLift>();
		if (gl != null) {
			line = Utils.BoolToString(gl.active); // bool - is this gravlift on?
		} else {
			UnityEngine.Debug.Log("GravityLift missing on savetype of GravityLift! GameObject.name: " + go.name);
		}
		//1
		return line;
	}

	public string SaveMChangerData(GameObject go) {
		string line = System.String.Empty;
		MaterialChanger mch = go.GetComponent<MaterialChanger>();
		if (mch != null) {
			line = Utils.BoolToString(mch.alreadyDone); // bool - is this gravlift on?  Much is already done.
		} else {
			UnityEngine.Debug.Log("MaterialChanger missing on savetype of MaterialChanger! GameObject.name: " + go.name);
		}
		//1
		return line;
	}

	public string SaveTRadiationData(GameObject go) {
		string line = System.String.Empty;
		Radiation rad = go.GetComponent<Radiation>();
		if (rad != null) {
			line = Utils.BoolToString(rad.isEnabled); // bool - hey is this on? hello?
			line += Utils.splitChar + rad.numPlayers.ToString(); // int - how many players we are affecting
		} else {
			UnityEngine.Debug.Log("Radiation missing on savetype of Radiation! GameObject.name: " + go.name);
		}
		//2
		return line;
	}

	public string SaveGravLiftPadTextureData(GameObject go) {
		string line = System.String.Empty;
		TextureChanger tex = go.GetComponent<TextureChanger>();
		if (tex != null) {
			line = Utils.BoolToString(tex.currentTexture); // bool - is this gravlift on?
		} else {
			UnityEngine.Debug.Log("TextureChanger missing on savetype of TextureChanger! GameObject.name: " + go.name);
		}
		//1
		return line;
	}

	public string SaveChargeStationData(GameObject go) {
		string line = System.String.Empty;
		ChargeStation chg = go.GetComponent<ChargeStation>();
		if (chg != null) {
			line = Utils.SaveRelativeTimeDifferential(chg.nextthink); // float - time before recharged
		} else {
			UnityEngine.Debug.Log("ChargeStation missing on savetype of ChargeStation! GameObject.name: " + go.name);
		}
		//1
		return line;
	}

	public string SaveLightAnimationData(GameObject go) {
		string line = System.String.Empty;
		LightAnimation la = go.GetComponent<LightAnimation>();
		if (la != null) {
			line = Utils.BoolToString(la.lightOn); // bool
			line += Utils.splitChar + Utils.BoolToString(la.lerpOn); // bool
			line += Utils.splitChar + la.currentStep.ToString(); // int
			line += Utils.splitChar + Utils.FloatToString(la.lerpValue); // %
			line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(la.lerpTime);
			line += Utils.splitChar + Utils.FloatToString(la.stepTime); // Not a timer, current time amount
			line += Utils.splitChar + Utils.SaveRelativeTimeDifferential(la.lerpStartTime);
		} else {
			UnityEngine.Debug.Log("LightAnimation missing on savetype of Light! GameObject.name: " + go.name);
		}
		return line;
	}

	public string SaveCameraData(GameObject go) {
		string line = System.String.Empty;
		Camera cm = go.GetComponent<Camera>();
		UnityStandardAssets.ImageEffects.BerserkEffect bzk = go.GetComponent<UnityStandardAssets.ImageEffects.BerserkEffect>();
		Grayscale gsc = go.GetComponent<Grayscale>();
		if (cm != null) {
			line = Utils.BoolToString(cm.enabled); // bool
			if (bzk != null) line += Utils.splitChar + Utils.BoolToString(bzk.enabled);
			else line += Utils.splitChar + "0";

			if (gsc != null) line += Utils.splitChar + Utils.BoolToString(gsc.enabled);
			else line += Utils.splitChar + "0";
		} else {
			UnityEngine.Debug.Log("Camera missing on savetype of Camera! GameObject.name: " + go.name);
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

	// Wrapper function to enable Save to be a coroutine...and now we can
	// enable all the levels before a save for a tiny bit.  Except we don't
	// since I need to figure out how to save as fast as possible while still
	// taking the time to display feedback at some point.  Currently we just
	// haul off and get it with top speed, no pausing momentarily to draw any
	// progress bar.  Left here just in case I try this later.
	public void StartSave(int index, string savename) {
		if (PlayerHealth.a.hm.health < 1.0f) return; // Can't save while dead!

		//StartCoroutine(Save(index,savename));
		Save(index,savename);
	}

	// Save the Game
	// ============================================================================
	//public IEnumerator Save(int saveFileIndex,string savename) {
	public void Save(int saveFileIndex,string savename) {
		Stopwatch saveTimer = new Stopwatch();
		saveTimer.Start();
		string[] saveData = new string[16000]; // Found 2987 saveable objects on main level - should be enough for any instantiated dropped items...maybe
		int i,j;
		int index = 0;

		// All saveable classes
		List<GameObject> saveableGameObjects = new List<GameObject>();
		FindAllSaveObjectsGOs(saveableGameObjects);
		sprint(stringTable[194]); // Indicate we are saving "Saving..."
		if (string.IsNullOrWhiteSpace(savename)) {
			savename = "Unnamed " + saveFileIndex.ToString(); // int
		}

		saveData[index] = savename; index++;
		saveData[index] = Utils.FloatToString(PauseScript.a.relativeTime); index++; // float - pausable game time
		s1.Clear(); // keep reusing s1
		// Global states and Difficulties
		s1.Append(LevelManager.a.currentLevel.ToString()); // int
		for (i=0;i<14;i++) {
			s1.Append(Utils.splitChar);
			s1.Append(LevelManager.a.levelSecurity[i].ToString()); // int
		}

		for (i=0;i<14;i++) {
			s1.Append(Utils.splitChar);
			s1.Append(LevelManager.a.levelCameraDestroyedCount[i].ToString()); // int
		}

		for (i=0;i<14;i++) {
			s1.Append(Utils.splitChar);
			s1.Append(LevelManager.a.levelSmallNodeDestroyedCount[i].ToString()); // int
		}

		for (i=0;i<14;i++) {
			s1.Append(Utils.splitChar);
			s1.Append(LevelManager.a.levelLargeNodeDestroyedCount[i].ToString()); // int
		}

		for (i=0;i<14;i++) {
			s1.Append(Utils.splitChar);
			s1.Append(Utils.BoolToString(LevelManager.a.ressurectionActive[i]));
		}

		s1.Append(Utils.splitChar);
		s1.Append(questData.Save());
		s1.Append(Utils.splitChar);
		s1.Append(difficultyCombat.ToString());
		s1.Append(Utils.splitChar);
		s1.Append(difficultyMission.ToString());
		s1.Append(Utils.splitChar);
		s1.Append(difficultyPuzzle.ToString());
		s1.Append(Utils.splitChar);
		s1.Append(difficultyCyber.ToString());
		saveData[index] = s1.ToString(); index++;
		s1.Clear();

		// Save all the players' data
		saveData[index] = SavePlayerData(player1); index++; // saves as "!" if null
		saveData[index] = "!"; index++; // Null player2.
		saveData[index] = "!"; index++; // saves as "!" if null
		saveData[index] = "!"; index++; // saves as "!" if null

		// Save all the objects data
		for (i=0;i<saveableGameObjects.Count;i++) {
			SaveObject sav = saveableGameObjects[i].GetComponent<SaveObject>();
			if (sav.saveableType == "Player") continue;

			saveData[index] = sav.Save(saveableGameObjects[i]); index++; // Take this objects data and add it to the array.
		}

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

		// Make "Done!" appear at the end of the line after "Saving..." is finished, concept from Halo's "Checkpoint...Done!"
		sprint(stringTable[195]);
		if (saveFileIndex < 7) Const.a.justSavedTimeStamp = Time.time + Const.a.savedReminderTime; // using normal run time, don't ask again to save for next 7 seconds
		saveTimer.Stop();
		UnityEngine.Debug.Log("Saved to file in " + saveTimer.Elapsed.ToString());
	}

	void LoadPlayerDataToPlayer(GameObject currentPlayer, string[] entries,int index) {
		int j;
		float readFloatx;
		float readFloaty;
		float readFloatz;
		float readFloatw;
		PlayerReferenceManager PRman = currentPlayer.GetComponent<PlayerReferenceManager>();
		GameObject pCap = PRman.playerCapsule;
		PlayerHealth ph = PlayerHealth.a;
		PlayerEnergy pe = PlayerEnergy.a;
		PlayerMovement pm = PlayerMovement.a;
		PlayerPatch pp = PlayerPatch.a;
		HealthManager hm = pCap.GetComponent<HealthManager>();
		Transform tr = pCap.transform;
		MouseLookScript ml = MouseLookScript.a;
		Transform trml = MouseLookScript.a.transform;
		Inventory inv = Inventory.a;
		MFDManager mfd = MFDManager.a;
		// Already parsed saveableType and ID number in main Load() function, skipping ahead to index 2 (3rd slot).
		Const.a.playerName = entries[index]; index++; 
		ph.radiated = Utils.GetFloatFromString(entries[index]); index++;
		ph.timer = Utils.GetFloatFromString(entries[index]); index++; // Not relative time
		ph.playerDead = Utils.GetBoolFromString(entries[index]); index++;
		ph.radiationArea = Utils.GetBoolFromString(entries[index]); index++;
		ph.mediPatchPulseFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		ph.mediPatchPulseCount = Utils.GetIntFromString(entries[index] ); index++;
		ph.makingNoise = Utils.GetBoolFromString(entries[index]); index++;
		ph.lastHealth = Utils.GetFloatFromString(entries[index]); index++;
		ph.painSoundFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		ph.radSoundFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		ph.radFXFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pe.energy = Utils.GetFloatFromString(entries[index]); index++;
		pe.tickFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.playerSpeed = Utils.GetFloatFromString(entries[index]); index++;
		pm.grounded = Utils.GetBoolFromString(entries[index]); index++;
		pm.currentCrouchRatio = Utils.GetFloatFromString(entries[index]); index++;
		pm.bodyState = Utils.IntToBodyState(Utils.GetIntFromString(entries[index])); index++;
		pm.ladderState = Utils.GetBoolFromString(entries[index]); index++;
		pm.gravliftState = Utils.GetBoolFromString(entries[index]); index++;
		pm.inCyberSpace = Utils.GetBoolFromString(entries[index]); index++;
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
		pm.CheatWallSticky = Utils.GetBoolFromString(entries[index]); index++;
		pm.CheatNoclip = Utils.GetBoolFromString(entries[index]); index++;
		pm.jumpTime = Utils.GetFloatFromString(entries[index]); index++; // not a timer
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		pm.oldVelocity = new Vector3(readFloatx,readFloaty,readFloatz);
		pm.fatigue = Utils.GetFloatFromString(entries[index]); index++;
		pm.justJumped = Utils.GetBoolFromString(entries[index]); index++;
		pm.fatigueFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.fatigueFinished2 = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.cyberSetup = Utils.GetBoolFromString(entries[index]); index++;
		pm.cyberDesetup = Utils.GetBoolFromString(entries[index]); index++;
		pm.oldBodyState = Utils.IntToBodyState(Utils.GetIntFromString(entries[index])); index++;
		pm.ConsoleDisable();
		pm.leanTarget = Utils.GetFloatFromString(entries[index]); index++;
		pm.leanShift = Utils.GetFloatFromString(entries[index]); index++;
		pm.jumpSFXFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.jumpLandSoundFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.jumpJetEnergySuckTickFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.fatigueWarned = Utils.GetBoolFromString(entries[index]); index++;
		pm.turboFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.ressurectingFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pm.doubleJumpFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.berserkFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.berserkIncrementFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.detoxFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.geniusFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.mediFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.reflexFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.sightFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.sightSideEffectFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.staminupFinishedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		pp.berserkIncrement = Utils.GetIntFromString(entries[index]); index++;
		pp.patchActive = Utils.GetIntFromString(entries[index]); index++;
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		tr.localPosition = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		readFloatw = Utils.GetFloatFromString(entries[index]); index++;
		tr.localRotation = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		tr.localScale = new Vector3(readFloatx,readFloaty,readFloatz);
		// readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		// readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		// readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		//trml.localPosition = new Vector3(readFloatx,readFloaty,readFloatz);
		// readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		// readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		// readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		// readFloatw = Utils.GetFloatFromString(entries[index]); index++;
		//trml.localRotation = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);
		// readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		// readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		// readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		index += 10;
		//trml.localScale = new Vector3(readFloatx,readFloaty,readFloatz);
		ml.inventoryMode = !Utils.GetBoolFromString(entries[index]); index++; // take opposite because we are about to opposite again
		ml.ToggleInventoryMode(); // correctly set cursor lock state, and opposite again, now it is what was saved
		ml.holdingObject = Utils.GetBoolFromString(entries[index]); index++;
		ml.heldObjectIndex = Utils.GetIntFromString(entries[index]); index++;
		ml.heldObjectCustomIndex = Utils.GetIntFromString(entries[index]); index++;
		ml.heldObjectAmmo = Utils.GetIntFromString(entries[index]); index++;
		ml.heldObjectAmmo2 = Utils.GetIntFromString(entries[index]); index++;
		ml.firstTimePickup = Utils.GetBoolFromString(entries[index]); index++;
		ml.firstTimeSearch = Utils.GetBoolFromString(entries[index]); index++;
		ml.grenadeActive = Utils.GetBoolFromString(entries[index]); index++;
		ml.inCyberSpace = Utils.GetBoolFromString(entries[index]); index++;
		ml.yRotation = Utils.GetFloatFromString(entries[index]); index++;
		ml.geniusActive = Utils.GetBoolFromString(entries[index]); index++;
		ml.xRotation = Utils.GetFloatFromString(entries[index]); index++;
		ml.vmailActive = Utils.GetBoolFromString(entries[index]); index++;
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		ml.cyberspaceReturnPoint = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		ml.cyberspaceReturnCameraLocalRotation = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		ml.cyberspaceReturnPlayerCapsuleLocalRotation = new Vector3(readFloatx,readFloaty,readFloatz);
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		ml.cyberspaceRecallPoint = new Vector3(readFloatx,readFloaty,readFloatz);
		ml.cyberspaceReturnLevel = Utils.GetIntFromString(entries[index]); index++;
		ml.currentSearchItem = null; // Prevent picking up first item immediately. Not currently possible to save references.
		hm.health = Utils.GetFloatFromString(entries[index]); index++; // how much health we have
		hm.cyberHealth = Utils.GetFloatFromString(entries[index]); index++;
		hm.deathDone = Utils.GetBoolFromString(entries[index]); index++; // bool - are we dead yet?
		hm.god = Utils.GetBoolFromString(entries[index]); index++; // Are we invincible? - we can save cheats?? OH WOW!
		hm.teleportDone = Utils.GetBoolFromString(entries[index]); index++; // did we already teleport? //hm.AwakeFromLoad();  Nothing done for isPlayer
		GUIState.ButtonType bt = (GUIState.ButtonType) Enum.Parse(typeof(GUIState.ButtonType), entries[index]);
		if (Enum.IsDefined(typeof(GUIState.ButtonType),bt)) GUIState.a.overButtonType = bt;
		index++;
		GUIState.a.overButton = Utils.GetBoolFromString(entries[index]); index++;
		for (j=0;j<7;j++) { inv.weaponInventoryIndices[j] = Utils.GetIntFromString(entries[index] ); index++; }
		for (j=0;j<7;j++) { inv.weaponInventoryAmmoIndices[j] = Utils.GetIntFromString(entries[index] ); index++; }
		//for (j=0;j<7;j++) { inv.weaponInventoryText[j] = inv.weaponInvTextSource[(WeaponFire.Get16WeaponIndexFromConstIndex(inv.weaponInventoryIndices[j]))]; } // derived from the above
		inv.numweapons = Utils.GetIntFromString(entries[index] ); index++;
		for (j=0;j<16;j++) { inv.wepAmmo[j] = Utils.GetIntFromString(entries[index] ); index++; }
		for (j=0;j<16;j++) { inv.wepAmmoSecondary[j] = Utils.GetIntFromString(entries[index] ); index++; }
		for (j=0;j<7;j++) { inv.currentEnergyWeaponHeat[j] = Utils.GetFloatFromString(entries[index]); index++; }
		for (j=0;j<7;j++) { inv.wepLoadedWithAlternate[j] = Utils.GetBoolFromString(entries[index]); index++; }
		WeaponCurrent.WepInstance.weaponCurrent = Utils.GetIntFromString(entries[index] ); index++;
		WeaponCurrent.WepInstance.weaponIndex = Utils.GetIntFromString(entries[index] ); index++;
		for (j=0;j<7;j++) { WeaponCurrent.WepInstance.weaponEnergySetting[j] = Utils.GetFloatFromString(entries[index]); index++; }
		for (j=0;j<7;j++) { WeaponCurrent.WepInstance.currentMagazineAmount[j] = Utils.GetIntFromString(entries[index] ); index++; }
		for (j=0;j<7;j++) { WeaponCurrent.WepInstance.currentMagazineAmount2[j] = Utils.GetIntFromString(entries[index] ); index++; }
		WeaponCurrent.WepInstance.justChangedWeap = Utils.GetBoolFromString(entries[index]); index++;
		WeaponCurrent.WepInstance.SetAllViewModelsDeactive();
		WeaponCurrent.WepInstance.lastIndex = Utils.GetIntFromString(entries[index] ); index++;
		WeaponCurrent.WepInstance.bottomless = Utils.GetBoolFromString(entries[index]); index++;
		WeaponCurrent.WepInstance.redbull = Utils.GetBoolFromString(entries[index]); index++;
		WeaponCurrent.WepInstance.reloadFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		WeaponCurrent.WepInstance.reloadLerpValue = Utils.GetFloatFromString(entries[index]); index++; // %
		WeaponCurrent.WepInstance.lerpStartTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		WeaponCurrent.WepInstance.targetY = Utils.GetFloatFromString(entries[index]); index++;
		WeaponFire.a.waitTilNextFire = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		WeaponFire.a.overloadEnabled = Utils.GetBoolFromString(entries[index]); index++;
		WeaponFire.a.sparqSetting = Utils.GetFloatFromString(entries[index]); index++;
		WeaponFire.a.ionSetting = Utils.GetFloatFromString(entries[index]); index++;
		WeaponFire.a.blasterSetting = Utils.GetFloatFromString(entries[index]); index++;
		WeaponFire.a.plasmaSetting = Utils.GetFloatFromString(entries[index]); index++;
		WeaponFire.a.stungunSetting = Utils.GetFloatFromString(entries[index]); index++;
		WeaponFire.a.recoiling = Utils.GetBoolFromString(entries[index]); index++;
		WeaponFire.a.justFired = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		WeaponFire.a.energySliderClickedTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		WeaponFire.a.cyberWeaponAttackFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		inv.grenadeCurrent = Utils.GetIntFromString(entries[index]); index++;
		inv.grenadeIndex = Utils.GetIntFromString(entries[index]); index++;
		inv.nitroTimeSetting = Utils.GetFloatFromString(entries[index]); index++;
		inv.earthShakerTimeSetting = Utils.GetFloatFromString(entries[index]); index++;
		for (j=0;j<7;j++) { inv.grenAmmo[j] = Utils.GetIntFromString(entries[index] ); index++; }
		inv.patchCurrent = Utils.GetIntFromString(entries[index]); index++;
		inv.patchIndex = Utils.GetIntFromString(entries[index]); index++;
		for (j=0;j<7;j++) { inv.patchCounts[j] = Utils.GetIntFromString(entries[index]); index++; }
		for (j=0;j<134;j++) { inv.hasLog[j] = Utils.GetBoolFromString(entries[index]); index++; }
		for (j=0;j<134;j++) { inv.readLog[j] = Utils.GetBoolFromString(entries[index]); index++; }
		for (j=0;j<10;j++) { inv.numLogsFromLevel[j] = Utils.GetIntFromString(entries[index]); index++; }
		inv.lastAddedIndex = Utils.GetIntFromString(entries[index]); index++;
		inv.beepDone = Utils.GetBoolFromString(entries[index]); index++;
		for (j=0;j<13;j++) { inv.hasHardware[j] = Utils.GetBoolFromString(entries[index]); index++; }
		if (Inventory.a.hasHardware[1]) {
			ml.compassContainer.SetActive(true);
			ml.automapContainerLH.SetActive(true);
			ml.automapContainerRH.SetActive(true);
		}
		for (j=0;j<13;j++) { inv.hardwareVersion[j] = Utils.GetIntFromString(entries[index]); index++; }
		for (j=0;j<13;j++) { inv.hardwareVersionSetting[j] = Utils.GetIntFromString(entries[index]); index++; }
		inv.hardwareInvCurrent = Utils.GetIntFromString(entries[index]); index++;
		inv.hardwareInvIndex = Utils.GetIntFromString(entries[index]); index++;
		for (j=0;j<13;j++) { inv.hardwareIsActive[j] = Utils.GetBoolFromString(entries[index]); index++; }
		for (j=0;j<32;j++) {
			int cardType = Utils.GetIntFromString(entries[index]);
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
		for (j=0;j<14;j++) { inv.generalInventoryIndexRef[j] = Utils.GetIntFromString(entries[index]); index++; }
		inv.generalInvCurrent = Utils.GetIntFromString(entries[index]); index++;
		inv.generalInvIndex = Utils.GetIntFromString(entries[index]); index++;
		inv.currentCyberItem = Utils.GetIntFromString(entries[index]); index++;
		inv.isPulserNotDrill = Utils.GetBoolFromString(entries[index]); index++;
		for (j=0;j<7;j++) { inv.softVersions[j] = Utils.GetIntFromString(entries[index]); index++; }
		for (j=0;j<7;j++) { inv.hasSoft[j] = Utils.GetBoolFromString(entries[index]); index++; }
		inv.emailCurrent = Utils.GetIntFromString(entries[index]); index++;
		inv.emailIndex = Utils.GetIntFromString(entries[index]); index++;	
		mfd.lastWeaponSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastItemSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastAutomapSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastTargetSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastDataSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastSearchSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastLogSideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastLogSecondarySideRH = Utils.GetBoolFromString(entries[index]); index++;
		mfd.lastMinigameSideRH = Utils.GetBoolFromString(entries[index]); index++;
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		mfd.objectInUsePos = new Vector3(readFloatx,readFloaty,readFloatz);
		// tetheredPGP
		// tetheredPWP
		// tetheredSearchable
		// tetheredKeypadElevator
		// tetheredKeypadKeycode
		mfd.paperLogInUse = Utils.GetBoolFromString(entries[index]); index++;
		mfd.usingObject = Utils.GetBoolFromString(entries[index]); index++;
		mfd.logReaderContainer.SetActive(Utils.GetBoolFromString(entries[index])); index++;
		mfd.DataReaderContentTab.SetActive(Utils.GetBoolFromString(entries[index])); index++;
		mfd.logTable.SetActive(Utils.GetBoolFromString(entries[index])); index++;
		mfd.logLevelsFolder.SetActive(Utils.GetBoolFromString(entries[index])); index++;
		mfd.logFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		mfd.logActive = Utils.GetBoolFromString(entries[index]); index++;
		mfd.logType = Utils.GetIntFromString(entries[index]); index++;
		mfd.cyberTimer.GetComponent<CyberTimer>().timerFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
	}

	void LoadObjectDataToObject(GameObject currentGameObject, string[] entries, int index) {
		float readFloatx;
		float readFloaty;
		float readFloatz;
		float readFloatw;
		Vector3 tempvec;
		SaveObject so = currentGameObject.GetComponent<SaveObject>();

		// Index starts at 3 here for SetActive.
		// Set active state of GameObject in Hierarchy
		bool setToActive = Utils.GetBoolFromString(entries[index]); index++;
		if (setToActive) {
			if (!currentGameObject.activeSelf) currentGameObject.SetActive(true); 
		} else {
			if (currentGameObject.activeSelf) currentGameObject.SetActive(false); 
		}

		// Get transform
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
		if (currentGameObject.transform.localPosition != tempvec) currentGameObject.transform.localPosition = tempvec;

		// Get rotation
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		readFloatw = Utils.GetFloatFromString(entries[index]); index++;
		Quaternion tempquat = new Quaternion(readFloatx,readFloaty,readFloatz,readFloatw);
		currentGameObject.transform.localRotation = tempquat;

		// Get scale
		readFloatx = Utils.GetFloatFromString(entries[index]); index++;
		readFloaty = Utils.GetFloatFromString(entries[index]); index++;
		readFloatz = Utils.GetFloatFromString(entries[index]); index++;
		tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
		currentGameObject.transform.localScale = tempvec;

		// Get rigidbody velocity
		Rigidbody rbody = currentGameObject.GetComponent<Rigidbody>();
		if (rbody != null) {
			readFloatx = Utils.GetFloatFromString(entries[index]); index++;
			readFloaty = Utils.GetFloatFromString(entries[index]); index++;
			readFloatz = Utils.GetFloatFromString(entries[index]); index++;
			tempvec = new Vector3(readFloatx,readFloaty,readFloatz);
			rbody.velocity = tempvec;
			rbody.isKinematic = Utils.GetBoolFromString(entries[index]); index++;
		} else {
			index = index + 4; // At 15 here, moving along and ignoring the zeros
		}

		HealthManager hm = currentGameObject.GetComponent<HealthManager>(); // used multiple times below
		SearchableItem se = currentGameObject.GetComponent<SearchableItem>(); // used multiple times below
		so.levelParentID = Utils.GetIntFromString(entries[index]); index++; // 16
		if (index >= entries.Length) return;
		switch (so.saveType) {
			case SaveObject.SaveableType.Useable:
				UseableObjectUse uou = currentGameObject.GetComponent<UseableObjectUse>();
				if (uou != null) {
					uou.useableItemIndex = Utils.GetIntFromString(entries[index]); index++;
					uou.customIndex = Utils.GetIntFromString(entries[index]); index++;
					uou.ammo = Utils.GetIntFromString(entries[index]); index++;
					uou.ammo2 = Utils.GetIntFromString(entries[index]); index++;
				}
				break;
			case SaveObject.SaveableType.Grenade:
				GrenadeActivate ga = currentGameObject.GetComponent<GrenadeActivate>();
				if (ga != null) {
					ga.constIndex = Utils.GetIntFromString(entries[index]); index++; // const lookup table index
					ga.useTimer = Utils.GetBoolFromString(entries[index]); index++; // do we have a timer going?
					ga.timeFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - how much time left before the fun part?
					ga.explodeOnContact = Utils.GetBoolFromString(entries[index]); index++; // bool - or not a landmine
					ga.useProx = Utils.GetBoolFromString(entries[index]); index++; // bool - is this a landmine?
				}
				break;
			case SaveObject.SaveableType.NPC:
				AIController aic = currentGameObject.GetComponent<AIController>();
				AIAnimationController aiac = currentGameObject.GetComponentInChildren<AIAnimationController>();
				if (aic != null) {
					aic.index = Utils.GetIntFromString(entries[index]); index++; // int - NPC const lookup table index for instantiating
					int state = Utils.GetIntFromString(entries[index]); index++;
					switch (state) {
						case 0: aic.currentState = AIState.Idle; break;
						case 1: aic.currentState = AIState.Walk; break;
						case 2: aic.currentState = AIState.Run; break;
						case 3: aic.currentState = AIState.Attack1; break;
						case 4: aic.currentState = AIState.Attack2; break;
						case 5: aic.currentState = AIState.Attack3; break;
						case 6: aic.currentState = AIState.Pain; break;
						case 7: aic.currentState = AIState.Dying; break;
						case 8: aic.currentState = AIState.Inspect; break;
						case 9: aic.currentState = AIState.Interacting; break;
						case 10:aic.currentState = AIState.Dead; break;
						default:aic.currentState = AIState.Idle; break;
					}
					int enemIDRead = Utils.GetIntFromString(entries[index]); index++;
					if (enemIDRead >= 0) {
						if (player1.GetComponent<SaveObject>().SaveID == enemIDRead) {
							aic.enemy = player1;
						}
					}
					aic.gracePeriodFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time before applying pain damage on attack2
					aic.meleeDamageFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time before applying pain damage on attack2
					aic.inSight = Utils.GetBoolFromString(entries[index]); index++; // bool
					aic.infront = Utils.GetBoolFromString(entries[index]); index++; // bool
					aic.inProjFOV = Utils.GetBoolFromString(entries[index]); index++; // bool
					aic.LOSpossible = Utils.GetBoolFromString(entries[index]); index++; // bool
					aic.goIntoPain = Utils.GetBoolFromString(entries[index]); index++; // bool
					aic.rangeToEnemy = Utils.GetFloatFromString(entries[index]); index++; // float
					aic.firstSighting = Utils.GetBoolFromString(entries[index]); index++; // bool - or are we dead?
					aic.dyingSetup = Utils.GetBoolFromString(entries[index]); index++; // bool - or are we dead?
					aic.ai_dying = Utils.GetBoolFromString(entries[index]); index++; // bool - are we dying the slow painful death
					aic.ai_dead = Utils.GetBoolFromString(entries[index]); index++; // bool - or are we dead?
					aic.currentWaypoint = Utils.GetIntFromString(entries[index]); index++; // int
					readFloatx = Utils.GetFloatFromString(entries[index]); index++; // float
					readFloaty = Utils.GetFloatFromString(entries[index]); index++; // float
					readFloatz = Utils.GetFloatFromString(entries[index]); index++; // float
					aic.currentDestination = new Vector3(readFloatx,readFloaty,readFloatz);
					aic.timeTillEnemyChangeFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					aic.timeTillDeadFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					aic.timeTillPainFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					aic.tickFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					aic.raycastingTickFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					aic.huntFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					aic.hadEnemy = Utils.GetBoolFromString(entries[index]); index++; // bool
					readFloatx = Utils.GetFloatFromString(entries[index]); index++; // float
					readFloaty = Utils.GetFloatFromString(entries[index]); index++; // float
					readFloatz = Utils.GetFloatFromString(entries[index]); index++; // float
					aic.lastKnownEnemyPos = new Vector3(readFloatx,readFloaty,readFloatz);
					readFloatx = Utils.GetFloatFromString(entries[index]); index++; // float
					readFloaty = Utils.GetFloatFromString(entries[index]); index++; // float
					readFloatz = Utils.GetFloatFromString(entries[index]); index++; // float
					aic.tempVec = new Vector3(readFloatx,readFloaty,readFloatz);
					aic.shotFired = Utils.GetBoolFromString(entries[index]); index++; // bool
					aic.randomWaitForNextAttack1Finished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					aic.randomWaitForNextAttack2Finished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					aic.randomWaitForNextAttack3Finished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					readFloatx = Utils.GetFloatFromString(entries[index]); index++; // float
					readFloaty = Utils.GetFloatFromString(entries[index]); index++; // float
					readFloatz = Utils.GetFloatFromString(entries[index]); index++; // float
					aic.idealTransformForward = new Vector3(readFloatx,readFloaty,readFloatz);
					readFloatx = Utils.GetFloatFromString(entries[index]); index++; // float
					readFloaty = Utils.GetFloatFromString(entries[index]); index++; // float
					readFloatz = Utils.GetFloatFromString(entries[index]); index++; // float
					aic.idealPos = new Vector3(readFloatx,readFloaty,readFloatz);
					aic.attackFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					aic.attack2Finished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					aic.attack3Finished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					readFloatx = Utils.GetFloatFromString(entries[index]); index++; // float
					readFloaty = Utils.GetFloatFromString(entries[index]); index++; // float
					readFloatz = Utils.GetFloatFromString(entries[index]); index++; // float
					aic.targettingPosition = new Vector3(readFloatx,readFloaty,readFloatz);
					aic.deathBurstFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					aic.deathBurstDone = Utils.GetBoolFromString(entries[index]); index++; // bool
					aic.asleep = Utils.GetBoolFromString(entries[index]); index++; // bool - are we sleepnir? vague reference alert
					aic.tranquilizeFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					aic.hopDone = Utils.GetBoolFromString(entries[index]); index++; // bool
					aic.wandering = Utils.GetBoolFromString(entries[index]); index++; // bool
					aic.wanderFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
					if (hm != null) {
						hm.health = Utils.GetFloatFromString(entries[index]); index++; // how much health we have
						if (hm.health > 0) {
							if (aic.boxCollider != null) { aic.boxCollider.enabled = true; }
							if (aic.sphereCollider != null) { aic.sphereCollider.enabled = true; }
							if (aic.meshCollider != null) { aic.meshCollider.enabled = true; }
							if (aic.capsuleCollider != null) { aic.capsuleCollider.enabled = true; }
							if (Const.a.moveTypeForNPC[aic.index] != AIMoveType.Fly) {
								rbody.useGravity = true;
								rbody.isKinematic = false;
							}
						}
						hm.cyberHealth = Utils.GetFloatFromString(entries[index]); index++;
						hm.deathDone = Utils.GetBoolFromString(entries[index]); index++; // bool - are we dead yet?
						hm.god = Utils.GetBoolFromString(entries[index]); index++; // are we invincible? - we can save cheats?? OH WOW!
						hm.teleportDone = Utils.GetBoolFromString(entries[index]); index++; // did we already teleport?
						hm.AwakeFromLoad();
					} else {
						index += 5;
					}
					if (aiac != null) {
						aiac.currentClipPercentage = Utils.GetFloatFromString(entries[index]); index++; // float
						aiac.dying = Utils.GetBoolFromString(entries[index]); index++; // bool
						aiac.animSwapFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float
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
					hm.health = Utils.GetFloatFromString(entries[index]); index++; // how much health we have
					hm.deathDone = Utils.GetBoolFromString(entries[index]); index++; // bool - are we dead yet?
					hm.god = Utils.GetBoolFromString(entries[index]); index++; // are we invincible? - we can save cheats?? OH WOW!
					hm.teleportDone = Utils.GetBoolFromString(entries[index]); index++; // did we already teleport?
					hm.AwakeFromLoad();
				} else {
					index = index + 4;
				}
				break;
			case SaveObject.SaveableType.SearchableStatic:
				if (se != null) {
					se.contents[0] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
					se.contents[1] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
					se.contents[2] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
					se.contents[3] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
					se.customIndex[0] = Utils.GetIntFromString(entries[index]); index++; // int custom index
					se.customIndex[1] = Utils.GetIntFromString(entries[index]); index++; // int custom index
					se.customIndex[2] = Utils.GetIntFromString(entries[index]); index++; // int custom index
					se.customIndex[3] = Utils.GetIntFromString(entries[index]); index++; // int custom index
					se.searchableInUse = false;
				} else {
					index += 8;
				}
				break;
			case SaveObject.SaveableType.SearchableDestructable:
				if (se != null) {
					se.contents[0] = Utils.GetIntFromString(entries[index]); index++;; // int main lookup index
					se.contents[1] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
					se.contents[2] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
					se.contents[3] = Utils.GetIntFromString(entries[index]); index++; // int main lookup index
					se.customIndex[0] = Utils.GetIntFromString(entries[index]); index++; // int custom index
					se.customIndex[1] = Utils.GetIntFromString(entries[index]); index++; // int custom index
					se.customIndex[2] = Utils.GetIntFromString(entries[index]); index++; // int custom index
					se.customIndex[3] = Utils.GetIntFromString(entries[index]); index++; // int custom index
					se.searchableInUse = false;
				} else {
					index += 8;
				}
				if (hm != null) {
					hm.health = Utils.GetFloatFromString(entries[index]); index++; // how much health we have
					hm.cyberHealth = Utils.GetFloatFromString(entries[index]); index++;
					hm.deathDone = Utils.GetBoolFromString(entries[index]); index++; // bool - are we dead yet?
					hm.god = Utils.GetBoolFromString(entries[index]); index++; // are we invincible? - we can save cheats?? OH WOW!
					hm.teleportDone = Utils.GetBoolFromString(entries[index]); index++; // did we already teleport?
					hm.AwakeFromLoad();
				} else {
					index = index + 4;
				}
				break;
			case SaveObject.SaveableType.Door:
				Door dr = currentGameObject.GetComponent<Door>();
				if (dr != null) {
					dr.targetAlreadyDone = Utils.GetBoolFromString(entries[index]); index++; // bool - have we already ran targets
					dr.locked = Utils.GetBoolFromString(entries[index]); index++; // bool - is this locked?
					dr.ajar = Utils.GetBoolFromString(entries[index]); index++; // bool - is this ajar?
					dr.useFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
					dr.waitBeforeClose = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
					dr.lasersFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
					dr.blocked = Utils.GetBoolFromString(entries[index]); index++; // bool - is the door blocked currently?
					dr.accessCardUsedByPlayer = Utils.GetBoolFromString(entries[index]); index++; // bool - is the door blocked currently?
					int state = Utils.GetIntFromString(entries[index]); index++;
					string clipName = "IdleClosed";
					switch (state) {
						case 0: dr.doorOpen = Door.doorState.Closed; clipName = "IdleClosed"; break;
						case 1: dr.doorOpen = Door.doorState.Open; clipName = "IdleOpen"; break;
						case 2: dr.doorOpen = Door.doorState.Closing; clipName = "DoorClose"; break;
						case 3: dr.doorOpen = Door.doorState.Opening; clipName = "DoorOpen"; break;
					}
					dr.animatorPlaybackTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
					dr.SetAnimFromLoad(clipName,0,dr.animatorPlaybackTime);
				} else {
					index += 4;
				}
				break;
			case SaveObject.SaveableType.ForceBridge:
				ForceBridge fb = currentGameObject.GetComponent<ForceBridge>();
				if (fb != null) {
					fb.activated = Utils.GetBoolFromString(entries[index]); index++; // bool - is the bridge on?
					fb.lerping = Utils.GetBoolFromString(entries[index]); index++; // bool - are we currently lerping one way or tother
					fb.tickFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time before thinking
				} else {
					index += 2;
				}
				break;
			case SaveObject.SaveableType.Switch:
				ButtonSwitch bs = currentGameObject.GetComponent<ButtonSwitch>();
				if (bs != null) {
					// bs?  null?  that's bs
					bs.locked = Utils.GetBoolFromString(entries[index]); index++; // bool - is this switch locked
					bs.active = Utils.GetBoolFromString(entries[index]); index++; // bool - is the switch flashing?
					bs.alternateOn = Utils.GetBoolFromString(entries[index]); index++; // bool - is the flashing material on?
					bs.delayFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time before firing targets
					bs.tickFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time before thinking
				} else {
					index += 4;
				}
				break;
			case SaveObject.SaveableType.FuncWall:
				FuncWall fw = currentGameObject.GetComponent<FuncWall>(); // actually this is on movertarget gameObjects
				if (fw != null) {
					int state = Utils.GetIntFromString(entries[index]); index++;
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
					tt.justUsed = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // bool - is the player still touching it?
				} else {
					index++;
				}
				break;
			case SaveObject.SaveableType.LBranch:
				LogicBranch lb = currentGameObject.GetComponent<LogicBranch>();
				if (lb != null) {
					lb.relayEnabled = Utils.GetBoolFromString(entries[index]); index++; // bool - is this enabled
					lb.onSecond = Utils.GetBoolFromString(entries[index]); index++; // bool - which one are we on?
				} else {
					index += 2;
				}
				break;
			case SaveObject.SaveableType.LRelay:
				LogicRelay lr = currentGameObject.GetComponent<LogicRelay>();
				if (lr != null) {
					lr.relayEnabled = Utils.GetBoolFromString(entries[index]); index++; // bool - is this enabled
				} else {
					index++;
				}
				break;
			case SaveObject.SaveableType.LSpawner:
				SpawnManager sm = currentGameObject.GetComponent<SpawnManager>();
				if (sm != null) {
					sm.active = Utils.GetBoolFromString(entries[index]); index++; // bool - is this enabled
					sm.numberActive = Utils.GetIntFromString(entries[index]); index++; // int - number spawned
					sm.delayFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time that we need to spawn next
				} else {
					index += 3;
				}
				break;
			case SaveObject.SaveableType.InteractablePanel:
				InteractablePanel ip = currentGameObject.GetComponent<InteractablePanel>();
				if (ip != null) {
					ip.open = Utils.GetBoolFromString(entries[index]); index++; // bool - is the panel opened
					ip.installed = Utils.GetBoolFromString(entries[index]); index++; // bool - is the item installed, MAKE SURE YOU ENABLE THE INSTALL ITEM GameObject IN LOAD
				} else {
					index += 2;
				}
				break;
			case SaveObject.SaveableType.ElevatorPanel:
				KeypadElevator ke = currentGameObject.GetComponent<KeypadElevator>();
				if (ke != null) {
					ke.padInUse = Utils.GetBoolFromString(entries[index]); index++; // bool - is the pad being used by a player
					ke.locked = Utils.GetBoolFromString(entries[index]); index++; // bool - locked?
				} else {
					index += 2;
				}
				break;
			case SaveObject.SaveableType.Keypad:
				KeypadKeycode kk = currentGameObject.GetComponent<KeypadKeycode>();
				if (kk != null) {
					kk.padInUse = Utils.GetBoolFromString(entries[index]); index++; // bool - is the pad being used by a player
					kk.locked = Utils.GetBoolFromString(entries[index]); index++; // bool - locked?
					kk.solved = Utils.GetBoolFromString(entries[index]); index++; // bool - already entered correct keycode?
				} else {
					index += 3;
				}
				break;
			case SaveObject.SaveableType.PuzzleGrid:
				PuzzleGridPuzzle pgp = currentGameObject.GetComponent<PuzzleGridPuzzle>();
				if (pgp != null) {
					pgp.puzzleSolved = Utils.GetBoolFromString(entries[index]); index++; // bool - is this puzzle already solved?
					for (int i=0;i<pgp.grid.Length;i++) {
						pgp.grid[i] = Utils.GetBoolFromString(entries[index]); index++;  // bool - get the current grid states + or X
					}
					pgp.fired = Utils.GetBoolFromString(entries[index]); index++; // bool - have we already fired yet?
					pgp.locked = Utils.GetBoolFromString(entries[index]); index++; // bool - is this locked?
				} else {
					index += 38; // grid length is always 35
				}
				break;
			case SaveObject.SaveableType.PuzzleWire:
				PuzzleWirePuzzle pwp = currentGameObject.GetComponent<PuzzleWirePuzzle>();
				if (pwp != null) {
					pwp.puzzleSolved = Utils.GetBoolFromString(entries[index]); index++; // bool - is this puzzle already solved?
					for (int i=0;i<pwp.currentPositionsLeft.Length;i++) {
						pwp.currentPositionsLeft[i] = Utils.GetIntFromString(entries[index]); index++; // int - get the current wire positions
					}
					for (int i=0;i<pwp.currentPositionsRight.Length;i++) {
						pwp.currentPositionsRight[i] = Utils.GetIntFromString(entries[index]); index++;  // int - get the current wire positions
					}
					pwp.locked = Utils.GetBoolFromString(entries[index]); index++; // bool - is this locked?
				} else {
					index += 16; // number of wire positions is always 7 for each side
				}
				break;
			case SaveObject.SaveableType.TCounter:
				TriggerCounter tc = currentGameObject.GetComponent<TriggerCounter>();
				if (tc != null) {
					tc.counter = Utils.GetIntFromString(entries[index]); index++; // int - how many counts we have
				} else {
					index++;
				}
				break;
			case SaveObject.SaveableType.TGravity:
				GravityLift gl = currentGameObject.GetComponent<GravityLift>();
				if (gl != null) {
					gl.active = Utils.GetBoolFromString(entries[index]); index++; // bool - is this gravlift on?
				} else {
					index++;
				}
				break;
			case SaveObject.SaveableType.MChanger:
				MaterialChanger mch = currentGameObject.GetComponent<MaterialChanger>();
				if (mch != null) {
					mch.alreadyDone = Utils.GetBoolFromString(entries[index]); index++; // bool - is this gravlift on?
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
					rad.isEnabled = Utils.GetBoolFromString(entries[index]); index++; // bool - hey is this on? hello?
					rad.numPlayers = Utils.GetIntFromString(entries[index]); index++; // int - how many players we are affecting
				} else {
					index += 2;
				}
				break;
			case SaveObject.SaveableType.GravPad:
				TextureChanger tex = currentGameObject.GetComponent<TextureChanger>();
				if (tex != null) {
					tex.currentTexture = Utils.GetBoolFromString(entries[index]); index++; // bool - is this gravlift on?
					tex.currentTexture = !tex.currentTexture; // gets done again in Toggle()
					tex.Toggle(); // set it again to be sure, does other stuff than just change the bool
				}
				break;
			case SaveObject.SaveableType.ChargeStation:
				ChargeStation chg = currentGameObject.GetComponent<ChargeStation>();
				if (chg != null) {
					chg.nextthink = Utils.LoadRelativeTimeDifferential(entries[index]); index++; // float - time before recharged
				}
				break;
			case SaveObject.SaveableType.Light:
				LightAnimation la = currentGameObject.GetComponent<LightAnimation>();
				if (la != null) {
					la.lightOn = Utils.GetBoolFromString(entries[index]); index++;
					la.lerpOn = Utils.GetBoolFromString(entries[index]); index++;
					la.currentStep = Utils.GetIntFromString(entries[index]); index++;
					la.lerpValue = Utils.GetFloatFromString(entries[index]); index++; // %
					la.lerpTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
					la.stepTime = Utils.GetFloatFromString(entries[index]); index++; // Not a timer, current time amount
					la.lerpStartTime = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
				}
				break;
			case SaveObject.SaveableType.Camera:
				Camera cm = currentGameObject.GetComponent<Camera>();
				UnityStandardAssets.ImageEffects.BerserkEffect bzk = currentGameObject.GetComponent<UnityStandardAssets.ImageEffects.BerserkEffect>();
				Grayscale gsc = currentGameObject.GetComponent<Grayscale>();
				if (cm != null) { cm.enabled = Utils.GetBoolFromString(entries[index]); index++;
				} else index++;
				if (bzk != null) { bzk.enabled = Utils.GetBoolFromString(entries[index]); index++;
				} else index++;
				if (gsc != null) { gsc.enabled = Utils.GetBoolFromString(entries[index]); index++;
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
		int numSaveablesFromSavefile = 0;
		int i,j;
		GameObject currentGameObject = null;
		sprint(stringTable[196]); // Loading...
		yield return null; // to update the sprint

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
			PauseScript.a.relativeTime = Utils.GetFloatFromString(entries[index]); // the global time from which everything checks it's somethingerotherFinished timer states
			currentline++; // line is over, now we are at 2
			index = 0; // reset before starting next line

			// Read in global states and quest mission bits in questData and difficulty indices
			entries = readFileList[currentline].Split(csplit);
			int levelNum = Utils.GetIntFromString(entries[index]); index++;
			LevelManager.a.LoadLevelFromSave(levelNum);
			for (i=0;i<14;i++) {
				LevelManager.a.levelSecurity[i] = Utils.GetIntFromString(entries[index]); index++;
			}
			for (i=0;i<14;i++) {
				LevelManager.a.levelCameraDestroyedCount[i] = Utils.GetIntFromString(entries[index]); index++;
			}
			for (i=0;i<14;i++) {
				LevelManager.a.levelSmallNodeDestroyedCount[i] = Utils.GetIntFromString(entries[index]); index++;
			}
			for (i=0;i<14;i++) {
				LevelManager.a.levelLargeNodeDestroyedCount[i] = Utils.GetIntFromString(entries[index]); index++;
			}
			for (i=0;i<14;i++) {
				LevelManager.a.ressurectionActive[i] = Utils.GetBoolFromString(entries[index]); index++;
			}
			questData.lev1SecCode = Utils.GetIntFromString(entries[index]); index++;
			questData.lev2SecCode = Utils.GetIntFromString(entries[index]); index++;
			questData.lev3SecCode = Utils.GetIntFromString(entries[index]); index++;
			questData.lev4SecCode = Utils.GetIntFromString(entries[index]); index++;
			questData.lev5SecCode = Utils.GetIntFromString(entries[index]); index++;
			questData.lev6SecCode = Utils.GetIntFromString(entries[index]); index++;
			questData.RobotSpawnDeactivated = Utils.GetBoolFromString(entries[index]); index++;
			questData.IsotopeInstalled = Utils.GetBoolFromString(entries[index]); index++;
			questData.ShieldActivated = Utils.GetBoolFromString(entries[index]); index++;
			questData.LaserSafetyOverriden = Utils.GetBoolFromString(entries[index]); index++;
			questData.LaserDestroyed = Utils.GetBoolFromString(entries[index]); index++;
			questData.BetaGroveCyberUnlocked = Utils.GetBoolFromString(entries[index]); index++;
			questData.GroveAlphaJettisonEnabled = Utils.GetBoolFromString(entries[index]); index++;
			questData.GroveBetaJettisonEnabled = Utils.GetBoolFromString(entries[index]); index++;
			questData.GroveDeltaJettisonEnabled = Utils.GetBoolFromString(entries[index]); index++;
			questData.MasterJettisonBroken = Utils.GetBoolFromString(entries[index]); index++;
			questData.Relay428Fixed = Utils.GetBoolFromString(entries[index]); index++;
			questData.MasterJettisonEnabled = Utils.GetBoolFromString(entries[index]); index++;
			questData.BetaGroveJettisoned = Utils.GetBoolFromString(entries[index]); index++;
			questData.AntennaNorthDestroyed = Utils.GetBoolFromString(entries[index]); index++;
			questData.AntennaSouthDestroyed = Utils.GetBoolFromString(entries[index]); index++;
			questData.AntennaEastDestroyed = Utils.GetBoolFromString(entries[index]); index++;
			questData.AntennaWestDestroyed = Utils.GetBoolFromString(entries[index]); index++;
			questData.SelfDestructActivated = Utils.GetBoolFromString(entries[index]); index++;
			questData.BridgeSeparated = Utils.GetBoolFromString(entries[index]); index++;
			questData.IsolinearChipsetInstalled = Utils.GetBoolFromString(entries[index]); index++;
			difficultyCombat = Utils.GetIntFromString(entries[index]); index++;
			difficultyMission = Utils.GetIntFromString(entries[index]); index++;
			difficultyPuzzle = Utils.GetIntFromString(entries[index]); index++;
			difficultyCyber = Utils.GetIntFromString(entries[index]); index++;
			currentline++; // line is over, now we are at 3

			// Load player 1 data
			index = 2; // reset before starting next line, skipping savetype and ID number
			entries = readFileList[currentline].Split(csplit); // read in Quest bits
			if (entries[0] != "!" && player1 != null) LoadPlayerDataToPlayer(player1,entries,index);
			currentline++; // line is over, now we are at 4
			index = 2; // reset before starting next line, skipping savetype and ID number

			// Load player 2 data
			entries = readFileList[currentline].Split(csplit); // read in Quest bits
			//if (entries[0] != "!" && player2 != null) LoadPlayerDataToPlayer(player2,entries,index);
			currentline++; // line is over, now we are at 5
			index = 2; // reset before starting next line, skipping savetype and ID number

			// Load player 3 data
			entries = readFileList[currentline].Split(csplit); // read in Quest bits
			//if (entries[0] != "!" && player3 != null) LoadPlayerDataToPlayer(player3,entries,index);
			currentline++; // line is over, now we are at 6
			index = 2; // reset before starting next line, skipping savetype and ID number

			// Load player 4 data
			entries = readFileList[currentline].Split(csplit); // read in Quest bits
			//if (entries[0] != "!" && player4 != null) LoadPlayerDataToPlayer(player4,entries,index);
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
					readIDs[i] = Utils.GetIntFromString(entries[1]); // int - get saveID from 2nd slot
					instantiatedActive = Utils.GetBoolFromString(entries[3]); // bool - get activeSelf value of the gameObject
					instantiatedCheck = Utils.GetBoolFromString(entries[2]); // bool - get instantiated from 3rd slot
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
						LoadObjectDataToObject(currentGameObject,entries,index);
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
						consttable = Utils.GetIntFromString(entries[3]); // int - get the prefab table type to use for lookups in Const
						constdex = Utils.GetIntFromString(entries[4]); // int - get the index into the Const table of prefabs
						if (constdex >= 0 && (consttable == 0 || consttable == 1)) {
							if (consttable == 0) prefabReferenceGO = Const.a.useableItems[constdex];
							else if (consttable == 1) prefabReferenceGO = Const.a.npcPrefabs[constdex];
							if (prefabReferenceGO != null) instantiatedObject = Instantiate(prefabReferenceGO,Const.a.vectorZero,quaternionIdentity) as GameObject; // Instantiate at generic location
							if (instantiatedObject != null) LoadObjectDataToObject(instantiatedObject, entries, index); // Load it
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
		player1CapsuleMainCameragGO.GetComponent<Camera>().fieldOfView = GraphicsFOV;
	}

	public void SetBloom() {
		player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.bloom.enabled = GraphicsBloom;
	}


	void SetFXAA(AntialiasingModel.FxaaPreset preset) {
		AntialiasingModel.Settings amS = AntialiasingModel.Settings.defaultSettings;
		amS.fxaaSettings.preset = preset;
		amS.method = AntialiasingModel.Method.Fxaa;
		player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = true;
		player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings = amS;
	}

	// None
	// ExtremePerformance,
	// Performance,
	// Default,
	// Quality,
	// ExtremeQuality
	public void SetAA() {
		if (GraphicsAAMode < 0) GraphicsAAMode = 0;
		if (GraphicsAAMode > 5) GraphicsAAMode = 5;
		switch (GraphicsAAMode) {
			case 0: // No Antialiasing, turn off the profile's antialiasing entirely.
				player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = false;
				break;
			case 1: // FXAA Extreme Performance, FXAA is a bit different so we call a helper function to set it.
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
			//case 6: // TAA Default  -- Too ugly, removed.  Might add back later to test if there's better quality settings when I have a graphics card that can handle it.
		//		player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = true;
		//		AntialiasingModel.Settings amS = player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings;
		//		amS.method = AntialiasingModel.Method.Taa;
		//		player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings = amS;
		//		break;
		}
	}

	// No Shadows
	// Hard Shadows,
	// Soft Shadows
	public void SetShadows() {
		if (GraphicsShadowMode > 2) GraphicsShadowMode = 2;
		if (GraphicsShadowMode < 0) GraphicsShadowMode = 0;
		switch (GraphicsShadowMode) {
			case 0: // No Shadows
				QualitySettings.shadows = ShadowQuality.Disable;
				break;
			case 1: // Hard Shadows
				QualitySettings.shadows = ShadowQuality.HardOnly;
				QualitySettings.shadowResolution = ShadowResolution.Low;
				QualitySettings.shadowDistance = 35.0f;
				break;
			case 2: // Soft Shadows
				QualitySettings.shadows = ShadowQuality.All;
				QualitySettings.shadowResolution = ShadowResolution.Low;
				QualitySettings.shadowDistance = 35.0f;
				break;
		}
	}

	void SetSSRPreset(ScreenSpaceReflectionModel.SSRResolution preset) {
		ScreenSpaceReflectionModel.Settings ssr = ScreenSpaceReflectionModel.Settings.defaultSettings;
		ssr.reflection.blendType = ScreenSpaceReflectionModel.SSRReflectionBlendType.Additive;
		ssr.reflection.reflectionQuality = preset;
		ssr.reflection.maxDistance = 100f;
		ssr.reflection.iterationCount = 256;
		ssr.reflection.stepSize = 12;
		ssr.reflection.widthModifier = 0.5f;
		ssr.reflection.reflectionBlur = 1.0f;
		ssr.reflection.reflectBackfaces = false;
		ssr.intensity.reflectionMultiplier = 0.25f;
		ssr.intensity.fadeDistance = 100f;
		ssr.intensity.fresnelFade = 1.0f;
		ssr.intensity.fresnelFadePower = 1.0f;
		ssr.screenEdgeMask.intensity = 0.03f;
		player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.screenSpaceReflection.enabled = true;
		player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.screenSpaceReflection.settings = ssr;
	}

	// No SSR
	// Low SSR,
	// High SSR
	public void SetSSR() {
		if (GraphicsSSRMode > 2) GraphicsSSRMode = 2;
		if (GraphicsSSRMode < 0) GraphicsSSRMode = 0;
		switch (GraphicsSSRMode) {
			case 0: // No SSR
				player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.screenSpaceReflection.enabled = false;
				break;
			case 1: // Low SSR
				SetSSRPreset(ScreenSpaceReflectionModel.SSRResolution.Low);
				break;
			case 2: // High SSR
				SetSSRPreset(ScreenSpaceReflectionModel.SSRResolution.High);
				break;
		}
	}

	public void SetSSAO() {
		player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.ambientOcclusion.enabled = GraphicsSSAO;
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

		if (effectIsWorldwide) {
			// The whole station is a shakin' and a movin'!
			MouseLookScript.a.ScreenShake(force);
		} else {
			// check if player is close enough and shake em' up!
			if (Vector3.Distance(transform.position,player1Capsule.transform.position) < distance) {
				MouseLookScript.a.ScreenShake(force);
			}
		}
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
		if (rbody != null && impactVelocity > 0) rbody.AddForceAtPosition((attackNormal*impactVelocity*30f),hitPoint);
	}
}
