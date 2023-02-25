using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics; // Stopwatch
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
	[HideInInspector] public AudioLogType[] audioLogType; // 0 = text only
														  // 1 = normal
														  // 2 = email
														  // 3 = papers
														  // 4 = vmail
	[HideInInspector] public string[] audioLogSpeech2Text;
	[HideInInspector] public int[] audioLogLevelFound;
	public AudioClip[] sounds;

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
												      // NPC Sounds       0,   1,   2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28
	[HideInInspector] public int[] sfxIdleForNPC =         new   int[]{  -1,  -1,  -1, -1, 58, -1, 59, -1, 59, 52, -1, -1, -1, -1, -1, -1,121, -1, -1, -1,121,118, -1, -1, -1, -1, -1, -1, -1};
	[HideInInspector] public int[] sfxSightSoundForNPC =   new   int[]{  -1,  -1, 111,150, 58,150, 59,152,152, -1,150,150,151,152,150, -1,121, -1,151,150,121,119,151, -1, -1, -1, -1, -1, -1};
	[HideInInspector] public int[] sfxAttack1ForNPC =      new   int[]{  -1,  -1, 108, -1, -1,146, -1,146,252,247, -1, -1, -1, -1, -1,122, -1,108,146, -1, -1,118, -1,125,258,258,258,258,258};
	[HideInInspector] public int[] sfxAttack2ForNPC =      new   int[]{  -1, 256,  -1,148, 50, 50, 50, 50, 50,250, 50, 50,146,259,148, -1,121, -1, -1,147, -1, -1,146, -1,258,258,258,258,258};
	[HideInInspector] public int[] sfxAttack3ForNPC =      new   int[]{  -1,  -1,  -1, -1, -1,244,244,244,245, -1, -1,149, -1, -1, -1, -1, -1, -1, -1,244, -1, -1, -1, -1,258,258,258,258,258};
	[HideInInspector] public int[] sfxDeathForNPC =        new   int[]{  -1,  48, 110,143, 48,145, 48, 51, 47, 47,142,143,144, 47,162,123,120,134,144,144,120,117,144,124, -1, -1, -1, -1, -1};
	[HideInInspector] public float[] deathBurstTimerForNPC=new float[]{0.0f,0.0f, 0.1f,0.0f,0.1f,0.1f,0.2f,0.1f,0.1f,0.1f,0.0f,0.45f,0.75f,0.1f,0.0f,0.0f,0.1f,0.224f,0.9f,0.0f,0.1f,0.1f,0.1f,0.2f,0.1f,0.1f,0.1f,0.1f,0.1f};
	[HideInInspector] public NPCType[] typeForNPC;
	[HideInInspector] public int[] projectile1PrefabForNPC;
	[HideInInspector] public int[] projectile2PrefabForNPC;
	[HideInInspector] public int[] projectile3PrefabForNPC;

	public GameObject[] projectilesLaunched;

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
    public GameObject Pool_MagpulseImpacts;
    public GameObject Pool_StungunImpacts;
    public GameObject Pool_RailgunImpacts;
    public GameObject Pool_PlasmaImpacts;
	public GameObject Pool_ProjEnemShot6Impacts;
	public GameObject Pool_ProjEnemShot2Impacts;
	public GameObject Pool_ProjSeedPodsImpacts;
	public GameObject Pool_TempAudioSources;
	public GameObject Pool_GrenadeEMPExplosions;
	public GameObject Pool_ProjEnemShot4Impacts;
	public GameObject Pool_CrateExplosions;
	public GameObject Pool_GrenadeFragLive;
	public GameObject Pool_ConcussionLive;
	public GameObject Pool_EMPLive;
	public GameObject Pool_GasLive;
	public GameObject Pool_GasExplosions;
	public GameObject Pool_CorpseHit;
	public GameObject Pool_LeafBurst;
	public GameObject Pool_MutationBurst;
	public GameObject Pool_GraytationBurst;
	public GameObject Pool_BarrelExplosions;
	public GameObject Pool_BulletHoleLarge;
	public GameObject Pool_BulletHoleScorchLarge;
	public GameObject Pool_BulletHoleScorchSmall;
	public GameObject Pool_BulletHoleSmall;
	public GameObject Pool_BulletHoleTiny;
	public GameObject Pool_BulletHoleTinySpread;
	public GameObject Pool_CyberDissolve;
	public GameObject Pool_TargetIDInstances;
	public GameObject Pool_AutomapBotOverlays;
	public GameObject Pool_AutomapCyborgOverlays;
	public GameObject Pool_AutomapMutantOverlays;
	public GameObject Pool_AutomapCameraOverlays;

	//Global object references
	public GameObject loadingScreen;
	public GameObject mainMenuInit; // Used to force mainMenuOn before Start().
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
	[HideInInspector] public bool GraphicsVSync;
	[HideInInspector] public bool GraphicsSSAO;
	[HideInInspector] public bool GraphicsBloom;
	[HideInInspector] public int GraphicsAAMode;
	[HideInInspector] public int GraphicsShadowMode;
	[HideInInspector] public int GraphicsSSRMode;
	[HideInInspector] public int GraphicsFOV;
	[HideInInspector] public int GraphicsGamma;
	[HideInInspector] public bool NoShootMode;
	public float HeadBobRate = 10.0f;
	public float HeadBobAmount = 0.02f;
	public int[] InputCodeSettings;	  // The integer index values
	public string[] InputCodes;		  // The readable mapping names used as
									  //   labels on the configuration page
	public string[] InputValues;	  // The list of all valid keys: letters,
									  //   numbers, etc.
	public string[] InputConfigNames; // The readable keys used as text
									  //   representations on the configuration
									  //   page for set values.

    public Font mainFont1; // Used to force Point filter mode.
	public Font mainFont2; // Used to force Point filter mode.
	/*[DTValidator.Optional] */public GameObject[] TargetRegister; // Doesn't need to be full, available space for maps and mods made by the community to use tons of objects
	public string[] TargetnameRegister;
    public string[] stringTable;
	public float[] reloadTime;

	public Material[] screenCodes;
	public Sprite[] logImages;

	public GameObject eventSystem;
	public Texture[] sequenceTextures;
	public GameObject[] chunkPrefabs;
	public Text loadPercentText;
	public GameObject[] cyberItemPrefabs;
	public GameObject[] miscellaneousPrefabs;
	public GameObject[] doorPrefabs;
	public Material[] genericMaterials;
	public GameObject[] ReverbRegister;

	// Irrelevant to inspector constants; automatically assigned during initialization or play.
	[HideInInspector] public int AudioSpeakerMode;
	[HideInInspector] public bool AudioReverb;
	[HideInInspector] public int AudioVolumeMaster;
	[HideInInspector] public int AudioVolumeMusic;
	[HideInInspector] public int AudioVolumeMessage;
	[HideInInspector] public int AudioVolumeEffects;
	[HideInInspector] public int AudioLanguage;			// The language index. Used for choosing which text to display on-screen.
	[HideInInspector] public float MouseSensitivity;		// The responsiveness of the mouse. Used for scaling slow mice up and fast mice down.
	[HideInInspector] public bool InputInvertLook;
	[HideInInspector] public bool InputInvertCyberspaceLook;
	[HideInInspector] public bool InputInvertInventoryCycling;
	[HideInInspector] public bool InputQuickItemPickup;
	[HideInInspector] public bool InputQuickReloadWeapons;
	[HideInInspector] public int[] npcCount;
	[HideInInspector] public int[] audioLogImagesRefIndicesLH;
	[HideInInspector] public int[] audioLogImagesRefIndicesRH;
	[HideInInspector] public string versionString = "v0.99"; // Global CITADEL PROJECT VERSION
	[HideInInspector] public bool gameFinished = false; // Global constants
	[HideInInspector] public float justSavedTimeStamp;
	[HideInInspector] public float savedReminderTime = 7f; // human short-term memory length
	[HideInInspector] public bool startingNewGame = false;
	[HideInInspector] public bool introNotPlayed = false;
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
	[HideInInspector] public Color ssDarkYellowText = new Color(0.8902f * 0.7f, 0.8745f * 0.7f, 0f); // Dark Yellow, e.g. for changing items transition
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
	[HideInInspector] public float globalShakeForce = 1f;
	[HideInInspector] public Quaternion quaternionIdentity;
	[HideInInspector] public Vector3 vectorZero;
	[HideInInspector] public Vector3 vectorOne;
	[HideInInspector] public int numberOfRaycastsThisFrame = 0;
	[HideInInspector] public int maxRaycastsPerFrame = 20;
	[HideInInspector] public float raycastTick = 0.2f;
	[HideInInspector] public float aiTickTime = 0.1f;

	// Private CONSTANTS
	[HideInInspector] public int TARGET_FPS = 60;
	private StringBuilder s1;
	private StringBuilder s2;

	//Instance container variable
	public static Const a;

	public void Awake() {
		Application.targetFrameRate = TARGET_FPS;
		a = this; // Create a new instance so that it can be accessed globally.
				  // MOST IMPORTANT PART!!


		// Cache values needed by awake prior to the .a instances of others.
		PlayerReferenceManager prm =
							  a.player1.GetComponent<PlayerReferenceManager>();

		if (prm != null) {
			a.player1Capsule = prm.playerCapsule;
			a.player1CapsuleMainCameragGO = prm.playerCapsuleMainCamera;
		}

		a.player1TargettingPos = a.player1CapsuleMainCameragGO.transform;
		a.player1PlayerMovementScript = 
							   a.player1Capsule.GetComponent<PlayerMovement>();

		a.CheckIfNewGame();
		a.LoadTextForLanguage(0); // Initialize with US English (index 0)
		a.s1 = new StringBuilder();
		a.s2 = new StringBuilder();
		if (a.mainMenuInit != null) {
			if (!a.mainMenuInit.activeSelf) a.mainMenuInit.SetActive(true);
		}

		a.justSavedTimeStamp = Time.time - a.savedReminderTime;
		a.quaternionIdentity = Quaternion.identity;
		a.vectorZero = Vector3.zero;
		a.vectorOne = Vector3.one;
		a.LoadAudioLogMetaData();
		a.LoadItemNamesData();
		a.LoadDamageTablesData();
		a.LoadEnemyTablesData(); // Doing earlier, needed by AIController Start
	}

	public bool RaycastBudgetExceeded() {
		return (numberOfRaycastsThisFrame > maxRaycastsPerFrame);
	}

	// From the Unity Documentation on Resources.Load:
	// Note: All asset names and paths in Unity use forward slashes, paths
	//       using backslashes will not work.
	// Using Utils.SafePathCombine with `true` to force / separator.
	// Removes the file extension as well per the docs.
	// docs.unity3d.com/2019.4/Documentation/ScriptReference/Resources.Load.html
	string ResourcesPathCombine(string folderInResources, string fileName) {
		string fname = Path.GetFileNameWithoutExtension(fileName);
		return Utils.SafePathCombine(folderInResources,true,fname);
	}

	public void ConfirmExistsInStreamingAssetsMakeIfNot(string fileName) {
		if (string.IsNullOrWhiteSpace(fileName)) {
			UnityEngine.Debug.Log("fileName was null or whitespace passed to "
								  + "ConfirmExistsInStreamingAssetsMakeIfNot");
			return;
		}

		string strmAstPth =
			Utils.SafePathCombine(Application.streamingAssetsPath,fileName);

		if (File.Exists(strmAstPth)) {
			return; // Already exists, all good!
		}

		 // Recreate StreamingAssets if it doesn't exist.
        if (!Directory.Exists(Application.streamingAssetsPath)) {
			Directory.CreateDirectory(Application.streamingAssetsPath);
		}

		string rsrc = ResourcesPathCombine("StreamingAssetsRecovery",fileName);
		//UnityEngine.Debug.Log("rsrc = " + rsrc);
        TextAsset resourcesFile = (TextAsset)Resources.Load(rsrc);
		//UnityEngine.Debug.Log(resourcesFile);
        if (resourcesFile != null) {
			// Recreate from Resources/StreamingAssetsRecovery/*
			File.WriteAllText(strmAstPth, resourcesFile.text, // new, contents
							  Encoding.ASCII);
			if (File.Exists(strmAstPth)) {
				UnityEngine.Debug.Log("File " + strmAstPth
									       + " recreated");
			} else {
				UnityEngine.Debug.Log("File " + strmAstPth
									       + " failed to be created by "
										   + "File.WriteAllText!");
			}
        } else {
			UnityEngine.Debug.Log("File " + strmAstPth
								  + " not found in the Resources folder");
		}
	}

    public void LoadTextForLanguage(int lang) {
        string readline; // variable to hold each string read in from the file
        int currentline = 0;
        string tF = "text_english.txt";
        switch (lang) {
            case 0: tF = "text_english.txt"; break;
            case 1: tF = "text_espanol.txt"; break; // UPKEEP: Other languages
        }

		ConfirmExistsInStreamingAssetsMakeIfNot(tF);
		string fp = Utils.SafePathCombine(Application.streamingAssetsPath,tF);
        StreamReader dataReader = new StreamReader(fp, Encoding.ASCII);
        using (dataReader) {
            do {
                // Read the next line
                readline = dataReader.ReadLine();
                if (currentline < stringTable.Length) {
                    stringTable[currentline] = readline;
				} else {
					UnityEngine.Debug.Log("WARNING: Ran out of slots in "
										  + "stringTable at "
										  + currentline.ToString());
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
		Config.LoadConfig();
		layerMaskNPCSight = LayerMask.GetMask("Default","Geometry","Corpse",
											  "Door","InterDebris",
											  "PhysObjects","Player","Player2",
											  "Player3","Player4");
		layerMaskNPCAttack = LayerMask.GetMask("Default","Geometry","Corpse",
											   "Door","InterDebris",
											   "PhysObjects","Player","Player2",
											   "Player3","Player4");

		// Not including "Bullets" as this is merely used for spawning, not
		// setting level-wide NPC collisions.
		layerMaskNPCCollision = LayerMask.GetMask("Default","TransparentFX",
												  "IgnoreRaycast","Geometry",
												  "NPC","Door","InterDebris",
												  "Player","Clip","NPCClip",
												  "PhysObjects");

		// Water is a hidden layer that prevents the player frobbing through
		// gratings, X-doors, etc.  Oh and also water...if that were a thing.
		layerMaskPlayerFrob = LayerMask.GetMask("Default","Geometry","Water",
												"Corpse","Door","InterDebris",
												"PhysObjects","Player2","Player3",
												"Player4","NPC");

		layerMaskPlayerAttack = LayerMask.GetMask("Default","Geometry","NPC",
												  "Bullets","Corpse","Door",
												  "InterDebris","PhysObjects",
												  "Player2","Player3","Player4");
		LoadCreditsData();
		StartCoroutine(InitializeEventSystem());
		questData = new QuestBits ();
		// Integer overload is maximum exclusive.  Confirmed maximum return
		// value is 9 and not 10.
		questData.lev1SecCode = UnityEngine.Random.Range(0,10); 
		questData.lev2SecCode = UnityEngine.Random.Range(0,10);
		questData.lev3SecCode = UnityEngine.Random.Range(0,10);
		questData.lev4SecCode = UnityEngine.Random.Range(0,10);
		questData.lev5SecCode = UnityEngine.Random.Range(0,10);
		questData.lev6SecCode = UnityEngine.Random.Range(0,10);
		if (mainFont1 != null) { // Ensure text is crisp and readable.
			mainFont1.material.mainTexture.filterMode = FilterMode.Point;
		}

		if (mainFont2 != null) { // Ensure text is crisp and readable.
			mainFont2.material.mainTexture.filterMode = FilterMode.Point;
		}

		PauseRigidbody[] prbTemp = FindObjectsOfType<PauseRigidbody>();
		for (int i=0;i<prbTemp.Length;i++) prb.Add(prbTemp[i]);

		 // P.P.S. PP. That's funny right there.  ...What?  I have kids.
		PauseParticleSystem[] ppses = FindObjectsOfType<PauseParticleSystem>();
		for (int i=0;i<ppses.Length;i++) psys.Add(ppses[i]);

		ObjectContainmentSystem.FindAllFloorGOs();
		ObjectContainmentSystem.UpdateActiveFlooring();

		GameObject newGameIndicator = GameObject.Find("NewGameIndicator");
		GameObject loadGameIndicator = GameObject.Find("LoadGameIndicator");
		if (loadGameIndicator != null) {
			// OK OK ok ok, so we aren't actually using this now and are just wiping out
			// dynamic object containers and relying on loading all static object data to
			// all static objects with manually generated indices now instead of Unity's
			// guid since the guid IS DIFFERENT when you reload the scene because the guids
			// are recreated as both scenes need loaded at once due to Unity's horrible no
			// good uncontrollable way of doing scene reloads (which could be round-about
			// worked around by having an empty dummy scene but poses other problems with
			// DontDestroyOnLoad stuff.  Regardless the guids are wiped and thus breaks the
			// old method of correlating saved objects in savefile to objects in the scene.
			//
			// Hopefully I've successfully marked and do save every static object to restore
			// it to its former glory as-is when saved.  Hopefully.
			MainMenuHandler.a.IntroVideo.SetActive(false);
			MainMenuHandler.a.IntroVideoContainer.SetActive(false);
			PauseScript.a.mainMenu.SetActive(false);
			SceneTransitionHandler sth = loadGameIndicator.GetComponent<SceneTransitionHandler>();
			sth.Load();
		} else if (newGameIndicator != null) {
			UnityEngine.Debug.Log("newGameIndicator.name: " + newGameIndicator.name);
			Utils.SafeDestroy(newGameIndicator);
			GoIntoGame();				  // Start of the game!!
		}
	}


	IEnumerator InitializeEventSystem () {
		yield return new WaitForSeconds(1f);
		if (eventSystem != null) eventSystem.SetActive(true);
	}

	private void LoadAudioLogMetaData () {
		// The following to be assigned to the arrays in the Unity Const data structure
		int readIndexOfLog, readLogImageLHIndex, readLogImageRHIndex; // look-up index for assigning the following data on the line in the file to the arrays
		string readLogText; // loaded into string audioLogSpeech2Text[]
		string readline; // variable to hold each string read in from the file
		char logSplitChar = ',';
		ConfirmExistsInStreamingAssetsMakeIfNot("logs_text.txt");
		string logPath = Utils.SafePathCombine(Application.streamingAssetsPath,
											   "logs_text.txt");
		StreamReader dataReader = new StreamReader(logPath,Encoding.ASCII);
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
				audioLogType[readIndexOfLog] = Utils.GetAudioLogTypeFromInt(Utils.GetIntFromString(entries[i])); i++;
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
		ConfirmExistsInStreamingAssetsMakeIfNot("damage_tables.txt");
		string dtP = Utils.SafePathCombine(Application.streamingAssetsPath,
										   "damage_tables.txt");
		StreamReader dataReader = new StreamReader(dtP,Encoding.ASCII);
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
		ConfirmExistsInStreamingAssetsMakeIfNot("credits.txt");
		string dr = Utils.SafePathCombine(Application.streamingAssetsPath,
										  "credits.txt");

		StreamReader dataReader = new StreamReader(dr,Encoding.ASCII);
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

				if (pagenum >= creditsText.Length) {
					UnityEngine.Debug.Log("Credits pagenum was too large at "
										  + pagenum.ToString());
					return;
				}

                creditsText[pagenum] += readline + System.Environment.NewLine;
			} while (!dataReader.EndOfStream);

			dataReader.Close();
			return;
		}
	}

	private void CheckIfNewGame () {
		string readline; // variable to hold each string read in from the file
		int currentline = 0;

		ConfirmExistsInStreamingAssetsMakeIfNot("ng.dat");
		string dr = Utils.SafePathCombine(Application.streamingAssetsPath,
										  "ng.dat");

		if (!File.Exists(dr)) {
			UnityEngine.Debug.Log("ng.dat not found nor recreated");
			return;
		}

		StreamReader dataReader = new StreamReader(dr,Encoding.ASCII);
		using (dataReader) {
			do {
				readline = dataReader.ReadLine(); // Read the next line
				if (currentline == 1) {
					a.introNotPlayed = Utils.GetBoolFromString(readline);
				}

				currentline++;
			} while (!dataReader.EndOfStream);

			dataReader.Close();
			return;
		}
	}

	public void WriteDatForIntroPlayed(bool setIntroNotPlayed) {
		// Write bit to file
		// No need to confirm it exists as StreamWriter will make it if not.
		string dr = Utils.SafePathCombine(Application.streamingAssetsPath,
										  "ng.dat");
		StreamWriter sw = new StreamWriter(dr,false,Encoding.ASCII);
		if (sw != null) {
			using (sw) {
				sw.WriteLine(Utils.BoolToString(setIntroNotPlayed));
				sw.Close();
			}
		}
		a.introNotPlayed = setIntroNotPlayed;
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
		typeForNPC = new NPCType[numberOfNPCs];
		projectile1PrefabForNPC = new int[numberOfNPCs];
		projectile2PrefabForNPC = new int[numberOfNPCs];
		projectile3PrefabForNPC = new int[numberOfNPCs];

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
		ConfirmExistsInStreamingAssetsMakeIfNot("enemy_tables.csv");
		string dr = Utils.SafePathCombine(Application.streamingAssetsPath,
										  "enemy_tables.csv");

		StreamReader dataReader = new StreamReader(dr,Encoding.ASCII);
		using (dataReader) {
			do {
				i = 0;
				refIndex = 0;
				readline = dataReader.ReadLine(); // Read the next line
				if (!skippedFirstLine) { skippedFirstLine = true; continue; }

				string[] entries = readline.Split(',');
				char[] commentCheck = entries[i].ToCharArray();
				if (commentCheck[0] == '/' && commentCheck[1] == '/') {
					continue; // Skip lines that start with '//'
				}

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
				readInt = Utils.GetIntFromString(entries[i].Trim()); typeForNPC[refIndex] = Utils.GetNPCTypeFromInt(readInt); i++;
				projectile1PrefabForNPC[refIndex] = Utils.GetIntFromString(entries[i].Trim()); i++;
				projectile2PrefabForNPC[refIndex] = Utils.GetIntFromString(entries[i].Trim()); i++;
				projectile3PrefabForNPC[refIndex] = Utils.GetIntFromString(entries[i].Trim()); i++;

				currentline++;
				if (currentline > 29) break;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}

	public static string GetTargetID(int npcIndex) {
		if (npcIndex > Const.a.npcPrefabs.Length) {
			return "BUG: npcIndex passed to GetTargetID too large!";
		}

		Const.a.npcCount[npcIndex]++;
		return Const.a.nameForNPC[npcIndex] + Const.a.npcCount[npcIndex].ToString();
	}

	public static string GetCyberTargetID(int cyberNPCIndex) {
		switch(cyberNPCIndex) {
			case 0: return Const.a.stringTable[499];
			case 1: return Const.a.stringTable[500];
			case 2: return Const.a.stringTable[501];
			case 3: return Const.a.stringTable[502];
		}
		return Const.a.stringTable[503];
	}

	public static void sprintByIndexOrOverride(int index, string overrideString,
											   GameObject playerPassed) {
		if (string.IsNullOrWhiteSpace(overrideString)) {
			if (index >= 0) {
				sprint(Const.a.stringTable[index],playerPassed);
			}
		} else sprint(overrideString,playerPassed);
	}

	// StatusBar Print
	public static void sprint(string input, GameObject player) {
		#if UNITY_EDITOR
			UnityEngine.Debug.Log(input); // Print to Editor console.
		#endif
		System.Console.WriteLine(input);
		a.statusBar.SendText(input);
	}

	public static void sprint(string input) { Const.sprint(input,null); }

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
        case PoolType.MagpulseImpacts:
            poolContainer = Pool_MagpulseImpacts;
            poolName = "MagpulseImpacts ";
            break;
        case PoolType.StungunImpacts:
            poolContainer = Pool_StungunImpacts;
            poolName = "StungunImpacts ";
            break;
        case PoolType.RailgunImpacts:
            poolContainer = Pool_RailgunImpacts;
            poolName = "RailgunImpacts ";
            break;
        case PoolType.PlasmaImpacts:
            poolContainer = Pool_PlasmaImpacts;
            poolName = "PlasmaImpacts ";
            break;
		case PoolType.ProjEnemShot6Impacts:
            poolContainer = Pool_ProjEnemShot6Impacts;
            poolName = "ProjEnemShot6Impacts ";
            break;
		case PoolType.ProjEnemShot2Impacts:
            poolContainer = Pool_ProjEnemShot2Impacts;
            poolName = "ProjEnemShot2Impacts ";
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
			if (child.gameObject.activeSelf == false) return child.gameObject;
			if (i == (poolContainer.transform.childCount - 1)) UnityEngine.Debug.Log("Ran out of items for " + poolName);
		}

		return null;
	}

	public void ClearActiveAutomapOverlays() {
		for (int i = 0; i < Pool_AutomapCameraOverlays.transform.childCount; i++) {
			Transform child = Pool_AutomapCameraOverlays.transform.GetChild(i);
			Image img = child.gameObject.GetComponent<Image>();
			if (img != null) img.enabled = false;
			if (child.gameObject.activeSelf == false) child.gameObject.SetActive(false);
		}

		for (int i = 0; i < Pool_AutomapMutantOverlays.transform.childCount; i++) {
			Transform child = Pool_AutomapMutantOverlays.transform.GetChild(i);
			Image img = child.gameObject.GetComponent<Image>();
			if (img != null) img.enabled = false;
			if (child.gameObject.activeSelf == false) child.gameObject.SetActive(false);
		}

		for (int i = 0; i < Pool_AutomapCyborgOverlays.transform.childCount; i++) {
			Transform child = Pool_AutomapCyborgOverlays.transform.GetChild(i);
			Image img = child.gameObject.GetComponent<Image>();
			if (img != null) img.enabled = false;
			if (child.gameObject.activeSelf == false) child.gameObject.SetActive(false);
		}

		for (int i = 0; i < Pool_AutomapBotOverlays.transform.childCount; i++) {
			Transform child = Pool_AutomapBotOverlays.transform.GetChild(i);
			Image img = child.gameObject.GetComponent<Image>();
			if (img != null) img.enabled = false;
			if (child.gameObject.activeSelf == false) child.gameObject.SetActive(false);
		}
	}

    public GameObject GetImpactType(HealthManager hm) {
        if (hm == null) return GetObjectFromPool(PoolType.SparksSmall);
        switch (hm.bloodType) {
            case BloodType.None: return GetObjectFromPool(PoolType.SparksSmall);
            case BloodType.Red: return GetObjectFromPool(PoolType.BloodSpurtSmall);
            case BloodType.Yellow: return GetObjectFromPool(PoolType.BloodSpurtSmallYellow);
            case BloodType.Green: return GetObjectFromPool(PoolType.BloodSpurtSmallGreen);
            case BloodType.Robot: return GetObjectFromPool(PoolType.SparksSmallBlue);
			case BloodType.Leaf: return GetObjectFromPool(PoolType.LeafBurst);
			case BloodType.Mutation: return GetObjectFromPool(PoolType.MutationBurst);
			case BloodType.GrayMutation: return GetObjectFromPool(PoolType.GraytationBurst);
        }

        return GetObjectFromPool(PoolType.SparksSmall);
	}

	void FindAllSaveObjectsGOs(ref List<GameObject> gos) {
		List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
		for (int i=0;i<allParents.Count;i++) {
			Component[] compArray = allParents[i].GetComponentsInChildren(typeof(SaveObject),true); // find all SaveObject components, including inactive (hence the true here at the end)
			for (int k=0;k<compArray.Length;k++) {
				gos.Add(compArray[k].gameObject); //add the gameObject associated with all SaveObject components in the scene
			}
		}
	}

	// Wrapper function to enable Save to be a coroutine so we can display
	// progress.  We don't though, currently we just haul off and get it with
	// top speed, no pausing momentarily to draw any progress bar since it is
	// plenty fast enough.
	public void StartSave(int index, string savename) {
		if (PlayerHealth.a.hm.health < 1.0f) return; // Can't save while dead!

		StartCoroutine(SaveRoutine(index,savename));
	}

	// Save the Game
	// ========================================================================
	public IEnumerator SaveRoutine(int saveFileIndex,string savename) {
		sprint(stringTable[194]); // Indicate we are saving "Saving..."
		yield return null; // Update to show this sprint.

		Stopwatch saveTimer = new Stopwatch();
		saveTimer.Start();
		string[] saveData = new string[16000]; // Found 2987 saveable objects on main level - should be enough for any instantiated dropped items...maybe
		int i,j;
		int index = 0;

		// All saveable classes
		List<GameObject> saveableGameObjects = new List<GameObject>();
		FindAllSaveObjectsGOs(ref saveableGameObjects);
		//UnityEngine.Debug.Log("Found "
		//					  + saveableGameObjects.Count.ToString()
		//					  + " total saveables for save.");

		if (string.IsNullOrWhiteSpace(savename)) {
			savename = "Unnamed " + Utils.UintToString(saveFileIndex); // int
		}

		saveData[index] = savename; index++;
		saveData[index] = Utils.FloatToString(PauseScript.a.relativeTime); index++; // float - pausable game time
		s1.Append(LevelManager.Save(LevelManager.a.gameObject));
		s1.Append(Utils.splitChar);
		s1.Append(questData.Save());
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(difficultyCombat));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(difficultyMission));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(difficultyPuzzle));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(difficultyCyber));
		saveData[index] = s1.ToString(); index++;
		s1.Clear();

		// Save all the objects data
		for (i=0;i<saveableGameObjects.Count;i++) {
			// Take this object's data and add it to the array.
			saveData[index] = SaveObject.Save(saveableGameObjects[i]); // <<< THIS IS IT <<<
			index++; // Each item saved on a separate line.
		}

		// Write to file
		string sName = "sav" + saveFileIndex.ToString() + ".txt";
		string sPath = Utils.SafePathCombine(Application.streamingAssetsPath,
											 sName);

		StreamWriter sw = new StreamWriter(sPath,false,Encoding.ASCII);
		if (sw != null) {
			using (sw) {
				for (j=0;j<saveData.Length;j++) {
					if (!string.IsNullOrWhiteSpace(saveData[j])) {
						sw.WriteLine(saveData[j]);
					}
				}
				sw.Close();
			}
		}

		// Make "Done!" appear at the end of the line after "Saving..." is finished, concept from Halo's "Checkpoint...Done!"
		saveTimer.Stop();
		sprint(stringTable[195] + " (" + saveTimer.Elapsed.ToString() + ")");
		if (saveFileIndex < 7) Const.a.justSavedTimeStamp = Time.time + Const.a.savedReminderTime; // using normal run time, don't ask again to save for next 7 seconds
	}

	// Start a New Game
	// ========================================================================
	// Sequence is as follows
	// 1. Button on New Game page sets difficulty, player name, and calls this.
	// 2. Write ng.dat file to show value of false to prevent intro playing.
	// 3. Look for GameNotYetStarted GameObject existence.
	// 4a. If fresh
	//   1. Destroy the GameNotYetStarted indicator GameObject.
	//   2. then simply turn off menu.
	// 4b. If not fresh
	//   1. Create NewGameIndicator GameObject, with transition handler.
	//   2. Flag it as DontDestroyOnLoad so it is preserved across scenes.
	//   3. Unload current scene.
	//   4. SceneTransitionHandler on DontDestroyOnLoad GameObject loads scene.
	// 5. Const.a.Start() checks for NewGameIndicator existence.
	// 6a. If present, simply turn off main menu.
	// 6b. Else same as if game was started from scratch.
	// 7. Go into the game.  Player now has normal control.
	public void NewGame() {
		//UnityEngine.Debug.Log("Starting new game!");
		WriteDatForIntroPlayed(false); // 2. Prevent intro from playing on subsequent sessions; only play intro video once ever after install (require menu option later).
		GameObject freshGame = GameObject.Find("GameNotYetStarted"); // 3.
		if (freshGame == null) { // 4b.
			GameObject newGameIndicator = new GameObject(); // 4b.1.
			newGameIndicator.name = "NewGameIndicator";
			SceneTransitionHandler sth =
			  newGameIndicator.AddComponent<SceneTransitionHandler>();
			sth.saveGameIndex = -1;
			DontDestroyOnLoad(newGameIndicator); // 4b.2.
			ReloadScene(sth); // 4b.3.
		} else { // 4a.
			//UnityEngine.Debug.Log("freshGame.name: " + freshGame.name);
			Utils.SafeDestroy(freshGame); // 4a.1. Destroy GameNotYetStarted. Game is started now.
			GoIntoGame(); // 4a.2. Ok now it's actually started.
		}
	}

	// Going into the game removes the helper GameObjects for these reasons:
	// - GameNotYetStarted, Game is now started, mark it as such.  This happens
	//                      only on game entry at beginning of session (first
	//                      time after launching the game).
	// - NewGameIndicator,  Game is no longer a new game, because it's started.
	// - LoadGameIndicator, Game should have been loaded prior to entry.
	public void GoIntoGame(Stopwatch loadTimer) {
		GameObject freshGame = GameObject.Find("GameNotYetStarted");
		if (freshGame != null) Utils.SafeDestroy(freshGame);
		GameObject saveIndicator = GameObject.Find("NewGameIndicator");
		if (saveIndicator != null)  Utils.SafeDestroy(saveIndicator);
		GameObject loadIndicator = GameObject.Find("LoadGameIndicator");
		if (loadIndicator != null)  Utils.SafeDestroy(loadIndicator);
		Cursor.visible = true;
		loadingScreen.SetActive(false);
		MainMenuHandler.a.IntroVideo.SetActive(false);
		MainMenuHandler.a.IntroVideoContainer.SetActive(false);
		PauseScript.a.mainMenu.SetActive(false);
		PauseScript.a.PauseDisable();
		if (PlayerHealth.a != null) {
			if (PlayerHealth.a.hm != null) PlayerHealth.a.hm.ClearOverlays();
		}

		if (Const.a.NoShootMode) MouseLookScript.a.ForceInventoryMode();

		loadingScreen.SetActive(false);
		if (player1Capsule != null) player1Capsule.SetActive(true);
		else UnityEngine.Debug.Log("BUG: Missing player1Capsule on GoIntoGame");
		player1CapsuleMainCameragGO.transform.parent.gameObject.SetActive(true);
		player1CapsuleMainCameragGO.SetActive(true);
		if (MouseLookScript.a != null) {
			MouseLookScript.a.playerCamera.enabled = true;
		}

		WriteDatForIntroPlayed(false);
		if (loadTimer == null) {
			sprint(stringTable[197]);
		} else {
			sprint(stringTable[197] + " ("
				   + loadTimer.Elapsed.ToString()
				   + ")"); // Loading...Done!
		}
	}

	public void GoIntoGame() {
		GoIntoGame(null);
	}


	public void ShowLoading() {
		MouseLookScript.a.playerCamera.enabled = false; // Hide world changes.
		PauseScript.a.mainMenu.SetActive(false); // Ensure that main menu is 
												 // off if came from Load page.
 		PauseScript.a.PauseEnable(); // Enable pause to make sure that nothing 
									 // goes on during couroutine as it happens
									 // over multiple frames.
		PauseScript.a.DisablePauseUI(); // Enable loading texts and unlock cursor.
		sprint(stringTable[196]); // Loading...
		if (PlayerHealth.a != null) PlayerHealth.a.hm.ClearOverlays();
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		loadPercentText.text = "(1) --.--";

		// Clear the HUD
		MFDManager.a.TabReset(true);
		MFDManager.a.TabReset(false);
		MFDManager.a.DisableAllCenterTabs();
		loadingScreen.SetActive(true);
		AutoSplitterData.isLoading = true;
	}

	public void ReloadScene(SceneTransitionHandler sth) {
		int index = SceneManager.GetActiveScene().buildIndex; // CitadelScene
        SceneManager.CreateScene("LoadScene");
		Scene loadScene = SceneManager.GetSceneByName("LoadScene");
        SceneManager.SetActiveScene(loadScene);
		AsyncOperation aso = SceneManager.UnloadSceneAsync(index,
							  UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
		sth.Reload(index, ref aso);
	}

	// Load the Game
	// ========================================================================
	// Sequence is as follows
	// 1. Player clicks on a button in load game menu or presses Quick Load.
	// 2. This function Load() is called with index -1 thru 7 and actual=false.
	// 3. Load then creates a DontDestroyOnLoad gameobject
	// 4. Current scene is unloaded.
	// 5. SceneTransitionHandler on DontDestroyOnLoad gameobject loads scene.
	// 6. Const Start() detects DontDestroyOnLoad object, uses Load actual=true
	// 7. Load then does actual load.
	//    a. Iterate over and destroy all dynamic objects in level containers.
	//    b. Find all remaining saveables to load static objects to.
	//    c. Load to static saveable objects.
	//    d. Iterate over dynamic object containers instantiating from save.
	public void Load(int saveFileIndex, bool actual) {
		ShowLoading();
		//if (!actual) {
		//	UnityEngine.Debug.Log("Initial Load() with saveFileIndex of " 
		//						  + saveFileIndex.ToString());
		//	GameObject loadGameIndicator = new GameObject();
		//	loadGameIndicator.name = "LoadGameIndicator";
		//	SceneTransitionHandler sth =
		//	  loadGameIndicator.AddComponent<SceneTransitionHandler>();
		//	sth.saveGameIndex = saveFileIndex;
		//	DontDestroyOnLoad(loadGameIndicator);
		//	ReloadScene(sth);
		//	return;
		//} else {
		//	UnityEngine.Debug.Log("Second pass Actual Load() with "
		//						  + "saveFileIndex of "
		//						  + saveFileIndex.ToString());
		//}

		GameObject freshGame = GameObject.Find("GameNotYetStarted");
		if (freshGame != null) Utils.SafeDestroy(freshGame);
		startingNewGame = false;
		introNotPlayed = false;
		WriteDatForIntroPlayed(introNotPlayed); // reset
		StartCoroutine(Const.a.LoadRoutine(saveFileIndex,false));
	}

	// LOAD 2. Called from Load menu or Quick Load.
	// LOAD 6. Called from Const.a.Start().
	public IEnumerator LoadRoutine(int saveFileIndex, bool actual) {
		Stopwatch loadTimer = new Stopwatch();
		Stopwatch loadUpdateTimer = new Stopwatch(); // For loading % indicator.
		loadTimer.Start();
		UnityEngine.Debug.Log("Start of Load with player at " + PlayerMovement.a.transform.localPosition.ToString());
		yield return null; // Update the view to show ShowLoading changes.

		string readline; 					// Initialize temporary variables.
		int numSaveablesFromSavefile = 0;
		int i,j;
		GameObject currentGameObjectInScene = null;
		List<GameObject> saveableGameObjectsInScene = new List<GameObject>();
		loadPercentText.text = "Preparing...";
		yield return null; // Update progress text.

		for (i=0;i<14;i++) {
			LevelManager.a.UnloadLevelDynamicObjects(i); // Delete them all!
			LevelManager.a.UnloadLevelNPCs(i); // Delete them all!
			loadPercentText.text = "Preparing level " + i.ToString();
			yield return new WaitForSeconds(0.1f); // Update progress text.
		}

		loadPercentText.text = "Open Save File         ";
		yield return null; // Update progress text.

		List<string> readFileList = new List<string>();
		int index = 0; // Caching since it will be iterated over in a loop.
		string[] entries = new string[2048]; // Holds pipe | delimited strings
											 // on individual lines.
		string lName = "sav" + saveFileIndex.ToString() + ".txt";
		string lPath = Utils.SafePathCombine(Application.streamingAssetsPath,
											 lName);
		StreamReader sr = new StreamReader(lPath);
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

			loadPercentText.text = "Load Quest Data...     ";
			yield return null; // to update the sprint
			int numSaveFileLines = readFileList.Count;
			numSaveablesFromSavefile = numSaveFileLines - 3;

			// readFileList[0] == saveName;  Not important, we are loading already now
			//index = 0; // Uncomment this if we pull in the saveName from this line for something.

			// Read in global time and pause data
			entries = readFileList[1].Split(Utils.splitChar.ToCharArray()[0]);
			PauseScript.a.relativeTime = Utils.GetFloatFromString(entries[index]); // the global time from which everything checks it's somethingerotherFinished timer states
			index = 0; // reset before starting next line

			// Read in global states, difficulties, and quest mission bits.
			entries = readFileList[2].Split(Utils.splitChar.ToCharArray()[0]);
			index = LevelManager.Load(LevelManager.a.gameObject,ref entries,index);
			index = questData.Load(ref entries,index);
			difficultyCombat = Utils.GetIntFromString(entries[index]); index++;
			difficultyMission = Utils.GetIntFromString(entries[index]); index++;
			difficultyPuzzle = Utils.GetIntFromString(entries[index]); index++;
			difficultyCyber = Utils.GetIntFromString(entries[index]); index++;

			loadPercentText.text = "Preprocess Save File...";
			yield return null;

			// First pass to initialize tracking arrays:
			// - saveFile_Line_SaveID, This holds the full list of all unique IDs.
			// - saveableIsInstantiated, True if object is instantiated prefab.
			int[] saveFile_Line_SaveID = new int[numSaveFileLines];
			bool[] saveFile_Line_IsInstantiated = new bool[numSaveFileLines];
			bool[] alreadyLoadedLineFromSaveFile = new bool[numSaveFileLines];
			Utils.BlankBoolArray(ref alreadyLoadedLineFromSaveFile,false); // Fill with false.
			for (i = 3; i < numSaveFileLines; i++) {
				entries = readFileList[i].Split(Utils.splitChar.ToCharArray()[0]);
				if (entries.Length > 1) {
					saveFile_Line_SaveID[i] = Utils.GetIntFromString(entries[1]); // int - get saveID from 2nd slot
					saveFile_Line_IsInstantiated[i] = Utils.GetBoolFromString(entries[2]); // bool - get instantiated from 3rd slot
				}
			}

			loadPercentText.text = "Preprocess Arrays...   ";
			yield return null;
			index = 3;
			SaveObject currentSaveObjectInScene;

			// LOAD 7b. FIND ALL STATIC SAVEABLES
			// DO THIS AFTER BLANKING TO ENSURE WE HAVE UP-TO-DATE LIST!!
			// Find all gameobjects with SaveObject script attached.
			// This assumes every prefab and static GameObject has only one
			// SaveObject script attached at top parent for that object.
			// Exceptions:
			// - func_wall has its SaveObject on first child
			// - doorE has its SaveObject on first child
			// - se_corpse_eaten has its SearchableItem on first child
			saveableGameObjectsInScene.Clear();
			FindAllSaveObjectsGOs(ref saveableGameObjectsInScene); // ref to avoid boxing.
			//UnityEngine.Debug.Log("Found " 
			//					  + saveableGameObjectsInScene.Count.ToString()
			//					  + " total static saveables remaining in "
			//					  + "scene after blanking out dynamic "
			//					  + "containers and NPC containers.");

			bool[] alreadyCheckedThisSaveableGameObjectInScene = new bool[saveableGameObjectsInScene.Count];
			Utils.BlankBoolArray(ref alreadyCheckedThisSaveableGameObjectInScene,false); // Fill with false.

			bool[] alreadyCheckedThisInstantiableGameObjectInScene = new bool[saveableGameObjectsInScene.Count];
			Utils.BlankBoolArray(ref alreadyCheckedThisInstantiableGameObjectInScene,false); // Fill with false.

			// LOAD 7c. LOAD TO STATIC SAVEABLES
			// Ok, so we have a list of all saveableGameObjectsInScene and a list of
			//   all saveables from the savefile.
			// Main iteration loops through all lines in the savefile.
			// Second iteration loops through all saveableGameObjectsInScene to find a match.
			// The save file will always have more objects in it than in the
			//   level since we removed the instantiables.
			// When we come across an instantiated object in the saveable file,
			//   we need to skip it for later and instantiate them all.
			loadPercentText.text = "Loading Static Objects: 0.0% (    0 / " + numSaveablesFromSavefile.ToString() + ")";
			yield return null;
			loadUpdateTimer.Start(); // For loading update
			for (i = 3; i < numSaveFileLines; i++) {
				if (saveFile_Line_IsInstantiated[i]) continue; // Skip instantiables.

				alreadyLoadedLineFromSaveFile[i] = true;
				for (j=0;j<(saveableGameObjectsInScene.Count);j++) {
					if (alreadyCheckedThisSaveableGameObjectInScene[j]) continue; // skip checking this and doing GetComponent
					if (saveableGameObjectsInScene[j] == null) continue;

					currentGameObjectInScene = saveableGameObjectsInScene[j];
					currentSaveObjectInScene = currentGameObjectInScene.GetComponent<SaveObject>();
					if (!currentSaveObjectInScene.instantiated) alreadyCheckedThisInstantiableGameObjectInScene[j] = true; // Huge time saver right here!

					// Static Objects all have unique ID.
					if (currentSaveObjectInScene.SaveID == 999999) UnityEngine.Debug.Log("Checking player during load");
					if (currentSaveObjectInScene.SaveID == saveFile_Line_SaveID[i]
						&& currentSaveObjectInScene.SaveID != 0) {
						if (currentSaveObjectInScene.SaveID == 999999) UnityEngine.Debug.Log("Found player in savefile on line " + i.ToString() + " during load");

						//if (!saveableGameObjectsInScene[j].isStatic // EDITOR ONLY!!!
						if (currentSaveObjectInScene.instantiated
							&& currentSaveObjectInScene.saveType != SaveableType.Light) {
							UnityEngine.Debug.Log("For some reason, attempting "
												  + "to load to dynamic object "
												  + saveableGameObjectsInScene[j].name);
						}

						entries = readFileList[i].Split(Utils.splitChar.ToCharArray()[0]);
						SaveObject.Load(currentGameObjectInScene,ref entries);
						alreadyCheckedThisSaveableGameObjectInScene[j] = true; // Huge time saver right here!
						break;
					}
				}

				loadPercentText.text = "Loading Static Objects: " + ((float)i/(float)numSaveablesFromSavefile*100f).ToString("0.0") + "% (" + i.ToString() + " / " + numSaveablesFromSavefile.ToString() + ")";
				if (loadUpdateTimer.ElapsedMilliseconds > 500) {
					loadUpdateTimer.Reset();
					loadUpdateTimer.Start();
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					yield return null;
				}
			}
			loadUpdateTimer.Stop();

			// Check if we missed a static non-instantiable object to load to.
			int numberOfMissedObjects = 0;
			SaveObject sobQuickCheck;
			for (i=0;i<saveableGameObjectsInScene.Count;i++) {
				if (alreadyCheckedThisInstantiableGameObjectInScene[i]) continue;

				string staticResult; // Immutable nonsense so no bother doing outside the loop, blech!
				sobQuickCheck = saveableGameObjectsInScene[i].GetComponent<SaveObject>();
 				staticResult = "is not static";
				if (sobQuickCheck != null) {
					if (!sobQuickCheck.instantiated) staticResult = "is static";
				}
				//if (saveableGameObjectsInScene[i].isStatic) staticResult = "is static"; // EDITOR ONLY!!!!!!!!!!
				

				UnityEngine.Debug.Log(saveableGameObjectsInScene[i].name 
									  + " not loaded during Static Pass and "
									  + staticResult);
				numberOfMissedObjects++;
			}
			if (numberOfMissedObjects > 0) {
				UnityEngine.Debug.Log("numberOfMissedObjects: "
									+ numberOfMissedObjects.ToString());
			}

			// LOAD 7d. INSTANTIATE AND LOAD TO INSTANTIATED SAVEABLES
			// Now time to instantiate anything left that is supposed to be here
			loadUpdateTimer.Start(); // For loading update
			int constdex = -1;
			int levID = -1;
			//int consttable = 0;
			GameObject instantiatedObject = null;
			//GameObject prefabReferenceGO = null;
			//for (i=0;i<instantiatedFound.Count;i++) {
			for (i = 3 ; i < numSaveFileLines; i++) {
				if (alreadyLoadedLineFromSaveFile[i]) continue;

				entries = readFileList[i].Split(Utils.splitChar.ToCharArray()[0]);
				if (entries.Length > 1) {
					levID = Utils.GetIntFromString(entries[18]); // int - get the level this was in
					if (levID < 0 || levID > 13) levID = 1; // Default to med.
					constdex = Utils.GetIntFromString(entries[19]); // int - get the index into the Master table of all prefabs
					if (constdex < 0 || constdex > 517) continue;

					instantiatedObject = ConsoleEmulator.SpawnDynamicObject(constdex,levID,false,null);
					SaveObject.Load(instantiatedObject,ref entries); // Load it.
				}

				loadPercentText.text = "Loading Dynamic Objects: " + ((float)i/(float)numSaveablesFromSavefile*100f).ToString("0.0") + "% (" + i.ToString() + " / " + numSaveablesFromSavefile.ToString() + ")";
				if (loadUpdateTimer.ElapsedMilliseconds > 500) {
					loadUpdateTimer.Reset();
					loadUpdateTimer.Start();
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					yield return null;
				}
			}
			loadUpdateTimer.Stop();
		}


		loadPercentText.text = "Cleaning Up...";
		yield return new WaitForSeconds(0.1f);
 		System.GC.Collect(); // Collect it all!
		AutoSplitterData.isLoading = false;
		loadTimer.Stop();
		GoIntoGame(loadTimer);
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
				return; // Ok, gameobject added to the register.
			}
		}
	}

	public void ReverbOn() {
		for (int i=0;i<ReverbRegister.Length;i++) {
			if (ReverbRegister[i] != null) {
				AudioReverbZone arz = ReverbRegister[i].GetComponent<AudioReverbZone>();
				if (arz != null) arz.enabled = true;
			}
		}
	}

	public void ReverbOff() {
		for (int i=0;i<ReverbRegister.Length;i++) {
			if (ReverbRegister[i] != null) {
				AudioReverbZone arz = ReverbRegister[i].GetComponent<AudioReverbZone>();
				if (arz != null) arz.enabled = false;
			}
		}
	}

	public void AddToReverbRegister (GameObject go) {
		for (int i=0;i<ReverbRegister.Length;i++) {
			if (ReverbRegister[i] == null) {
				ReverbRegister[i] = go;
				return; // Ok, gameobject added to the register.
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
}
