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

// GLOBAL SCRIPT EXECUTION ORDER (set in Unity Project Settings, here for ref)
// UnityEngine.EventSystems.EventSystems.EventSystem -1000
// Music           -950
// Const           -900
// PauseScript     -899
// Level           -800
// LevelManager    -700
// MainMenuHandler -500
// UnityEngine.InputSystem.PlayerInput -100
//                 --   Default Time   --
// UnityEngine.UI.ToggleGroup 10
// PlayerReferenceManager 100
// HealthManager    400
// MFDManager       600
// AIController     700
// CameraView       750
// DynamicCulling   800
// TargetIO         950
// SEGI            1100
// UnityStandardAssets.ImageEffects.ScreenSpaceAmbientOcclusion 1200
// TextLocalization 1300

public class Const : MonoBehaviour {
	//Item constants
	public QuestBits questData;
	public Texture2D[] useableItemsFrobIcons;
    public Sprite[] useableItemsIcons;

	//Audiolog constants
	public string[] audiologNames;
	[HideInInspector] public string[] audiologSenders;
	public string[] audiologSubjects;
	public AudioClip[] audioLogs;
	[HideInInspector] public AudioLogType[] audioLogType; // 0 = text only
														  // 1 = normal
														  // 2 = email
														  // 3 = papers
														  // 4 = vmail
	public string[] audioLogSpeech2Text;
	[HideInInspector] public int[] audioLogLevelFound;
	public AudioClip[] sounds;

	//Weapon constants
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
	public int[] magazinePitchCountForWeapon;
	public int[] magazinePitchCountForWeapon2;
	public float[] recoilForWeapon;
	public AttackType[] attackTypeForWeapon;

	//NPC constants
	[HideInInspector] public string[] nameForNPC;
	[HideInInspector] public AttackType[] attackTypeForNPC;
	[HideInInspector] public AttackType[] attackTypeForNPC2;
	[HideInInspector] public AttackType[] attackTypeForNPC3;
	[HideInInspector] public float[] damageForNPC; // Primary attack damage
	[HideInInspector] public float[] damageForNPC2; // Secondary attack damage
	[HideInInspector] public float[] damageForNPC3; // Grenade attack damage
	public float[] rangeForNPC; // Primary attack range
	public float[] rangeForNPC2; // Secondary attack range
	public float[] rangeForNPC3; // Grenade throw range
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
	public float[] runSpeedForNPC;
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

	// System constants
	[HideInInspector] public string[] creditsText;
	[HideInInspector] public HealthManager[] healthObjectsRegistration; // List of objects with health, used for fast application of damage in explosions
	public GameObject player1;

	// Layer masks
	[HideInInspector] public int layerMaskPlayerFrob;
	[HideInInspector] public int layerMaskPlayerTargetIDFrob;
	[HideInInspector] public int layerMaskPlayerAttack;
	[HideInInspector] public int layerMaskNPCSight;
	[HideInInspector] public int layerMaskNPCAttack;
	[HideInInspector] public int layerMaskNPCCollision;
	[HideInInspector] public int layerMaskPlayerFeet;
	[HideInInspector] public int layerMaskExplosion;

	public GameObject Pool_CameraExplosions;
	public GameObject Pool_BloodSpurtSmall;
	public GameObject Pool_SparksSmall;
	public GameObject Pool_SparksSmallBlue;
	public GameObject Pool_BloodSpurtSmallYellow;
	public GameObject Pool_BloodSpurtSmallGreen;
	public GameObject Pool_HopperImpact;
	public GameObject Pool_GrenadeFragExplosions;
    public GameObject Pool_Vaporize;
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
	public GameObject Pool_CyberDissolve;
	public GameObject Pool_AutomapBotOverlays;
	public GameObject Pool_AutomapCyborgOverlays;
	public GameObject Pool_AutomapMutantOverlays;
	public GameObject Pool_AutomapCameraOverlays;

	//Global object references
	public GameObject loadingScreen;
	public GameObject mainMenuInit; // Used to force mainMenuOn before Start().
	public StatusBarTextDecay statusBar;
   
	//Config constants
	public int difficultyCombat;
	public int difficultyMission;
	public int difficultyPuzzle;
	public int difficultyCyber;
	[HideInInspector] public string playerName;
	public AudioSource mainmenuMusic;
	[HideInInspector] public int GraphicsResWidth;
	[HideInInspector] public int GraphicsResHeight;
	[HideInInspector] public bool GraphicsFullscreen;
	[HideInInspector] public bool GraphicsSSAO;
	[HideInInspector] public bool GraphicsBloom;
	[HideInInspector] public bool GraphicsSEGI;
	[HideInInspector] public int GraphicsAAMode;
	[HideInInspector] public int GraphicsShadowMode;
	[HideInInspector] public int GraphicsSSRMode;
	[HideInInspector] public int GraphicsFOV;
	[HideInInspector] public int GraphicsGamma;
	[HideInInspector] public int GraphicsModelDetail;	
	[HideInInspector] public bool GraphicsVSync;
	[HideInInspector] public bool NoShootMode;
	[HideInInspector] public bool DynamicMusic;
	[HideInInspector] public const float HeadBobRate = 0.2f;
	[HideInInspector] public const float HeadBobAmount = 0.08f;
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
	/*[DTValidator.Optional] */public List<GameObject> TargetRegister; // Doesn't need to be full, available space for maps and mods made by the community to use tons of objects
	public List<string> TargetnameRegister;
    public string[] stringTable;
	[HideInInspector] public bool stringTableLoaded = false;
	[HideInInspector] public bool loading = false;
	public HashSet<TextLocalization> TextLocalizationRegister;
	public float[] reloadTime;

	public Material[] screenCodes;
	public Sprite[] logImages;

	public GameObject eventSystem;
	private GameObject[] prefabs; // Everything
	public GameObject prefabFallback;
	public Texture2D[] textures;
	public Texture[] sequenceTextures;
	public Text loadPercentText;
	public Material[] genericMaterials;
	public GameObject[] ReverbRegister;
	public int nextFreeSaveID = 2000000;
	public bool editMode = false;
	public bool noHUD = false;
	
	public Mesh sphereMesh;
	public Mesh cubeMesh;
	public Material segiEmitterMaterial1;
	public Material segiEmitterMaterialRed;
	public Material segiEmitterMaterialGreen;
	public Material segiEmitterMaterialBlue;
	public Material segiEmitterMaterialPurple;
	public Material segiEmitterMaterialRedFaint;
	public const float segiVoxelSize = 0.4f;
	public const float segiReducedExposure = 0.52f;
	public GameObject testSphere;

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
	[HideInInspector] public bool Footsteps;
	[HideInInspector] public bool HeadBob;
	[HideInInspector] public int[] npcCount;
	[HideInInspector] public int[] audioLogImagesRefIndicesLH;
	[HideInInspector] public int[] audioLogImagesRefIndicesRH;
	[HideInInspector] public string versionString;
	[HideInInspector] public bool gameFinished = false; // Global constants
	[HideInInspector] public float justSavedTimeStamp;
	[HideInInspector] public float savedReminderTime = 7f; // human short-term memory length
	[HideInInspector] public bool startingNewGame = false;
	[HideInInspector] public bool introNotPlayed = false;
	[HideInInspector] public const float doubleClickTime = 0.500f;
	[HideInInspector] public const float frobDistance = 4.9f;
	[HideInInspector] public const float elevatorPadUseDistance = 2f;
	[HideInInspector] public int creditsLength;
	[HideInInspector] public Transform player1TargettingPos;
	[HideInInspector] public GameObject player1Capsule;
	[HideInInspector] public PlayerMovement player1PlayerMovementScript;
	[HideInInspector] public PlayerHealth player1PlayerHealthScript;
	[HideInInspector] public GameObject player1CapsuleMainCameragGO;
	[HideInInspector] public List<PauseRigidbody> prb;
	[HideInInspector] public List<PauseParticleSystem> psys;
	[HideInInspector] public List<PauseAnimation> panimsList;
	[HideInInspector] public float playerCameraOffsetY = 0.84f; //Vertical camera offset from player 0,0,0 position (mid-body)
	[HideInInspector] public Color ssYellowText = new Color(0.8902f, 0.8745f, 0f); // Yellow, e.g. for current inventory text
	[HideInInspector] public Color ssDarkYellowText = new Color(0.8902f * 0.7f, 0.8745f * 0.7f, 0f); // Dark Yellow, e.g. for changing items transition
	[HideInInspector] public Color ssGreenText = new Color(0.3725f, 0.6549f, 0.1686f); // Green, e.g. for inventory text
	[HideInInspector] public Color ssRedText = new Color(0.9176f, 0.1373f, 0.1686f); // Red, e.g. for inventory text
	[HideInInspector] public Color ssWhiteText = new Color(1f, 1f, 1f); // White, e.g. for warnings text
	[HideInInspector] public Color ssOrangeText = new Color(1f, 0.498f, 0f); // Orange, e.g. for map buttons text
	[HideInInspector] public const float camMaxAmount = 0.2548032f;
	[HideInInspector] public const float mapWorldMaxN = 85.83999f;
	[HideInInspector] public const float mapWorldMaxS = -78.00001f;
	[HideInInspector] public const float mapWorldMaxE = -70.44f;
	[HideInInspector] public const float mapWorldMaxW = 93.4f;
	[HideInInspector] public const float mapTileMinX = 8; // top left corner
	[HideInInspector] public const float mapTileMinY = -1016; // bottom right corner
	public bool decoyActive = false;
	[HideInInspector] public const float berserkTime = 20f; //Patch constants
	[HideInInspector] public const float detoxTime = 60f;
	[HideInInspector] public const float geniusTime = 180f;
	[HideInInspector] public const float mediTime = 35f;
	[HideInInspector] public const float reflexTime = 155f;
	[HideInInspector] public const float sightTime = 40f;
	[HideInInspector] public const float sightSideEffectTime = 17f;
	[HideInInspector] public const float staminupTime = 60f;
	[HideInInspector] public const float reflexTimeScale = 0.25f;
	[HideInInspector] public const float defaultTimeScale = 1.0f;
	[HideInInspector] public const float berserkDamageMultiplier = 4.0f;
	[HideInInspector] public const float nitroMinTime = 1.0f; //Grenade constants
	[HideInInspector] public const float nitroMaxTime = 60.0f;
	[HideInInspector] public const float nitroDefaultTime = 7.0f;
	[HideInInspector] public const float earthShMinTime = 4.0f;
	[HideInInspector] public const float earthShMaxTime = 60.0f;
	[HideInInspector] public const float earthShDefaultTime = 10.0f;
	[HideInInspector] public const float globalShakeDistance = 0.3f;
	[HideInInspector] public const float globalShakeForce = 1f;
	[HideInInspector] public Quaternion quaternionIdentity;
	[HideInInspector] public Vector3 vectorZero;
	[HideInInspector] public Vector3 vectorOne;
	[HideInInspector] public int numberOfRaycastsThisFrame = 0;
	[HideInInspector] public const int maxRaycastsPerFrame = 20;
	[HideInInspector] public const float raycastTick = 0.2f;
	[HideInInspector] public const float aiTickTime = 0.1f;
	
	// Credit stats
	[HideInInspector] public int kills = 0;
	[HideInInspector] public int cyberkills = 0;
	[HideInInspector] public int shotsFired = 0;
	[HideInInspector] public int grenadesThrown = 0;
	[HideInInspector] public float damageDealt = 0f;
	[HideInInspector] public float damageReceived = 0f;
	[HideInInspector] public int savesScummed = 0;
	private int lastTargetRegistrySize = 0;

	public static RaycastHit hitNull;	

	// Private CONSTANTS
	[HideInInspector] public int TARGET_FPS = 240;
	private StringBuilder s1;
	private StringBuilder s2;

	//Instance container variable
	public static Const a;
	
	public void SetA() {
		if (a == null) a = this;
	}

	public void Awake() {
		Application.targetFrameRate = TARGET_FPS;
		SetA(); // Create a new instance so that it can be accessed globally.
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
		// Force Initialize all TextLocalization so language loaded from config
		// is set properly.
		a.TextLocalizationRegister = new HashSet<TextLocalization>();
		TextLocalization texloc = null;
		List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
		int i,k, found;
		for (i=0;i<allParents.Count;i++) {
			found = 0;
			Component[] compArray = allParents[i].GetComponentsInChildren(typeof(TextLocalization),true); // find all TextLocalization components, including inactive (hence the true here at the end)
			for (k=0;k<compArray.Length;k++) {
				texloc = compArray[k].gameObject.GetComponent<TextLocalization>();
				if (texloc != null) {
					texloc.Awake();
					found++;
				}
			}
			
// 			UnityEngine.Debug.Log("Found " + found.ToString()
// 								  + " TextLocalizations for "
// 								  + allParents[i].name);
		}
		
		a.lastTargetRegistrySize = 0;
		a.TargetRegister = new List<GameObject>();
		a.TargetnameRegister = new List<string>();
		for (i=0;i<allParents.Count;i++) {
			found = 0;
			Component[] compArray = allParents[i].GetComponentsInChildren(typeof(TargetIO),true); // find all TargetIO components, including inactive (hence the true here at the end)
			for (k=0;k<compArray.Length;k++) {
				TargetIO tio = compArray[k].gameObject.GetComponent<TargetIO>();
				if (tio != null) {
					tio.RemoteStart(a.gameObject,"Awake()"); // Reregister
					found++;
				}
			}
			
// 			UnityEngine.Debug.Log("Found " + found.ToString() + " TargetIO's "
// 								  + "for " + allParents[i].name);
		}

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
		a.LoadDamageTablesData();
		a.LoadEnemyTablesData(); // Doing earlier, needed by AIController Start
		a.LoadTextures();
		a.versionString = "v0.99.92"; // Global CITADEL PROJECT VERSION
		UnityEngine.Debug.Log("Citadel " + versionString
							  + ": " + System.Environment.NewLine
							  + "Start of C# Game Code, Welcome back Hacker!");
	}

	public bool RaycastBudgetExceeded() {
		return (numberOfRaycastsThisFrame > maxRaycastsPerFrame);
	}

    public void LoadTextForLanguage(int lang) {
// 		UnityEngine.Debug.Log("Loading language: " + lang.ToString());
        string readline; // variable to hold each string read in from the file
        int currentline = 0;
        string tF = "text_english.txt";
        switch (lang) {
            case 0: tF = "text_english.txt"; break;
			case 1: tF = "text_espanol.txt"; break; // UPKEEP: Other languages
			case 2: tF = "text_deutsch.txt"; break; // German
			case 3: tF = "text_francais.txt"; break; // French
			case 4: tF = "text_nihongo.txt"; break; // Japanese
			case 5: tF = "text_russkiy.txt"; break; // Russian
			case 6: tF = "text_italiano.txt"; break; // Italian
			case 7: tF = "text_portugues.txt"; break; // Portugese
        }

        StreamReader dataReader = Utils.ReadStreamingAsset(tF);
		if (stringTable.Length < 925) stringTable = new string[1024];
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
			stringTableLoaded = true;
            return;
        }
    }

	void Start() {
		Config.LoadConfig();
		layerMaskNPCSight = LayerMask.GetMask("Default","Geometry",
											  "Door","InterDebris",
											  "PhysObjects","Player","Player2",
											  "Player3","Player4");
		layerMaskNPCAttack = LayerMask.GetMask("Default","Geometry",
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
												"Door","InterDebris",
												"PhysObjects","Player2",
												"Player3","Player4",
												"CorpseSearchable");

		// Must have the geometry and default layers to prevent locking onto
		// NPCs through walls.
		layerMaskPlayerTargetIDFrob = LayerMask.GetMask("Default","Geometry",
														"Door",
														"Player2","Player3",
														"Player4","NPC",
														"CorpseSearchable");

		layerMaskPlayerAttack = LayerMask.GetMask("Default","Geometry","NPC",
												  "Bullets","Door",
												  "InterDebris","PhysObjects",
												  "Player2","Player3","Player4",
												  "CorpseSearchable");
			
		layerMaskExplosion = LayerMask.GetMask("Default","Geometry","NPC",
												  "Bullets","Door",
												  "InterDebris","PhysObjects",
												  "Player2","Player3","Player4",
											  	  "Player","CorpseSearchable");
			

		layerMaskPlayerFeet = LayerMask.GetMask("Default","Geometry");

		LoadCreditsData();
		StartCoroutine(InitializeEventSystem());
		questData = new QuestBits ();
// 		if (mainFont1 != null) { // Ensure text is crisp and readable.
			mainFont1.material.mainTexture.filterMode = FilterMode.Point;
// 		}

// 		if (mainFont2 != null) { // Ensure text is crisp and readable.
			mainFont2.material.mainTexture.filterMode = FilterMode.Point;
// 		}

		ResetPauseLists();
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
		} else if (newGameIndicator != null || Application.platform == RuntimePlatform.Android) {
			UnityEngine.Debug.Log("newGameIndicator.name: " + newGameIndicator.name);
			Utils.SafeDestroy(newGameIndicator);
			GoIntoGame();				  // Start of the game!!
		}
	}
	
	public void ResetPauseLists() {
		prb.Clear();
		psys.Clear();
		panimsList.Clear();
		PauseRigidbody[] prbTemp = FindObjectsOfType<PauseRigidbody>();
		for (int i=0;i<prbTemp.Length;i++) prb.Add(prbTemp[i]);

		 // P.P.S. PP. That's funny right there.  ...What?  I have kids.
		PauseParticleSystem[] ppses = FindObjectsOfType<PauseParticleSystem>();
		for (int i=0;i<ppses.Length;i++) psys.Add(ppses[i]);

		PauseAnimation[] panims = FindObjectsOfType<PauseAnimation>();
		for (int i=0;i<panims.Length;i++) panimsList.Add(panims[i]);

		ObjectContainmentSystem.FindAllFloorGOs();
		ObjectContainmentSystem.UpdateActiveFlooring();	
	}

	IEnumerator InitializeEventSystem () {
		yield return new WaitForSeconds(1f);
		if (eventSystem != null) eventSystem.SetActive(true);
	}

	public void LoadAudioLogMetaData() {
		// The following to be assigned to the arrays in the Unity Const data structure
		int readIndexOfLog, readLogImageLHIndex, readLogImageRHIndex; // look-up index for assigning the following data on the line in the file to the arrays
		string readLogText; // loaded into string audioLogSpeech2Text[]
		string readline; // variable to hold each string read in from the file
		char logSplitChar = ',';
		string tF = null;
		switch(Const.a.AudioLanguage) {
            case 0: tF = "logs_text_english.txt"; break;
			case 1: tF = "logs_text_espanol.txt"; break; // UPKEEP: Other languages
			case 2: tF = "logs_text_deutsch.txt"; break; // German
			case 3: tF = "logs_text_francais.txt"; break; // French
			case 4: tF = "logs_text_nihongo.txt"; break; // Japanese
			case 5: tF = "logs_text_russkiy.txt"; break; // Russian
			case 6: tF = "logs_text_italiano.txt"; break; // Italian
			case 7: tF = "logs_text_portugues.txt"; break; // Portugese
        }

		StreamReader dataReader = Utils.ReadStreamingAsset(tF);
		using (dataReader) {
			do {
				int i = 0;
				// Read the next line
				readline = dataReader.ReadLine();
				if (readline == null) continue; // just in case
				string[] entries = readline.Split(logSplitChar);
				readIndexOfLog = Utils.GetIntFromStringAudLogText(entries[i]); i++;
				readLogImageLHIndex = Utils.GetIntFromStringAudLogText(entries[i]); i++;
				readLogImageRHIndex = Utils.GetIntFromStringAudLogText(entries[i]); i++;
				audioLogImagesRefIndicesLH[readIndexOfLog] = readLogImageLHIndex;
				audioLogImagesRefIndicesRH[readIndexOfLog] = readLogImageRHIndex;
				audiologNames[readIndexOfLog] = entries[i]; i++;
				audiologSenders[readIndexOfLog] = entries[i]; i++;
				audiologSubjects[readIndexOfLog] = entries[i]; i++;
				audioLogType[readIndexOfLog] = Utils.GetAudioLogTypeFromInt(Utils.GetIntFromStringAudLogText(entries[i])); i++;
				audioLogLevelFound[readIndexOfLog] = Utils.GetIntFromStringAudLogText(entries[i]); i++;
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

	private void LoadDamageTablesData () {
		string readline; // variable to hold each string read in from the file
		int currentline = 0;
		int readInt = 0;
		StreamReader dataReader = Utils.ReadStreamingAsset("damage_tables.txt");
		using (dataReader) {
			do {
				int i = 0;
				// Read the next line
				readline = dataReader.ReadLine();
				string[] entries = readline.Split(',');
				delayBetweenShotsForWeapon[currentline] = Utils.GetFloatFromString(entries[i],"delayBetweenShotsForWeapon"); i++;
				delayBetweenShotsForWeapon2[currentline] = Utils.GetFloatFromString(entries[i],"delayBetweenShotsForWeapon2"); i++;
				damagePerHitForWeapon[currentline] = Utils.GetFloatFromString(entries[i],"damagePerHitForWeapon"); i++;
				damagePerHitForWeapon2[currentline] = Utils.GetFloatFromString(entries[i],"damagePerHitForWeapon2"); i++;
				damageOverloadForWeapon[currentline] = Utils.GetFloatFromString(entries[i],"damageOverloadForWeapon"); i++;
				energyDrainLowForWeapon[currentline] = Utils.GetFloatFromString(entries[i],"energyDrainLowForWeapon"); i++;
				energyDrainHiForWeapon[currentline] = Utils.GetFloatFromString(entries[i],"energyDrainHiForWeapon"); i++;
				energyDrainOverloadForWeapon[currentline] = Utils.GetFloatFromString(entries[i],"energyDrainOverloadForWeapon"); i++;
				penetrationForWeapon[currentline] = Utils.GetFloatFromString(entries[i],"penetrationForWeapon"); i++;
				penetrationForWeapon2[currentline] = Utils.GetFloatFromString(entries[i],"penetrationForWeapon2"); i++;
				offenseForWeapon[currentline] = Utils.GetFloatFromString(entries[i],"offenseForWeapon"); i++;
				offenseForWeapon2[currentline] = Utils.GetFloatFromString(entries[i],"offenseForWeapon2"); i++;
				readInt = Utils.GetIntFromString(entries[i],"attackTypeForWeapon"); i++;
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
		//Utils.ConfirmExistsInStreamingAssetsMakeIfNot("credits.txt");
		//string dr = Utils.SafePathCombine(Application.streamingAssetsPath,
		//								  "credits.txt");

		//StreamReader dataReader = new StreamReader(dr,Encoding.ASCII);
		StreamReader dataReader = Utils.ReadStreamingAsset("credits.txt");
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
		string dr;
		if (Application.platform == RuntimePlatform.Android) {
		    a.introNotPlayed = false;
			return;
		} else {
			Utils.ConfirmExistsInStreamingAssetsMakeIfNot("ng.dat");
			dr = Utils.SafePathCombine(Application.streamingAssetsPath,
										      "ng.dat");
		}

		if (!File.Exists(dr)) {
			UnityEngine.Debug.Log("ng.dat not found nor recreated");
			return;
		}

		StreamReader dataReader = new StreamReader(dr,Encoding.ASCII);
		using (dataReader) {
			do {
				readline = dataReader.ReadLine(); // Read the next line
				if (currentline == 1) a.introNotPlayed = readline.Equals("1");
				currentline++;
			} while (!dataReader.EndOfStream);

			dataReader.Close();
			return;
		}
	}

	public void WriteDatForIntroPlayed(bool setIntroNotPlayed) {
	    if (Application.platform == RuntimePlatform.Android) return;
	    
		// Write bit to file
		// No need to confirm it exists as StreamWriter will make it if not.
		string dr = Utils.SafePathCombine(Application.streamingAssetsPath,
										  "ng.dat");
		StreamWriter sw = new StreamWriter(dr,false,Encoding.ASCII);
		if (sw != null) {
			using (sw) {
				sw.WriteLine(Utils.BoolToStringConfig(setIntroNotPlayed));
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
		string readline; // variable to hold each string read in from the file
		bool skippedFirstLine = false;
		int currentline = 1;
		int readInt = 0;
		int i = 0;
		int refIndex = 0;
		StreamReader dataReader = Utils.ReadStreamingAsset("enemy_tables.csv");
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

				refIndex = Utils.GetIntFromStringAudLogText(entries[i+1]); // Index is stored at 2nd spot
				if (refIndex < 0 || refIndex > 28) continue; // Invalid value, skip

				nameForNPC[refIndex] = entries[i].Trim(); i++;
				i++; // No need to read the index again so we skip over it.
				readInt = Utils.GetIntFromStringAudLogText(entries[i].Trim()); attackTypeForNPC[refIndex] = Utils.GetAttackTypeFromInt(readInt); i++; 
				readInt = Utils.GetIntFromStringAudLogText(entries[i].Trim()); attackTypeForNPC2[refIndex] = Utils.GetAttackTypeFromInt(readInt); i++; 
				readInt = Utils.GetIntFromStringAudLogText(entries[i].Trim()); attackTypeForNPC3[refIndex] = Utils.GetAttackTypeFromInt(readInt); i++; 
				damageForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				damageForNPC2[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				damageForNPC3[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				rangeForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				rangeForNPC2[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				rangeForNPC3[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				healthForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				healthForCyberNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				readInt = Utils.GetIntFromStringAudLogText(entries[i].Trim()); perceptionForNPC[refIndex] = Utils.GetPerceptionLevelFromInt(readInt); i++;
				disruptabilityForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				armorvalueForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				defenseForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				moveTypeForNPC[refIndex] = Utils.GetMoveTypeFromInt(Utils.GetIntFromStringAudLogText(entries[i].Trim())); i++;
				yawSpeedForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				fovForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				fovAttackForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				fovStartMovementForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				distToSeeBehindForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				sightRangeForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				walkSpeedForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				runSpeedForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				attack1SpeedForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				attack2SpeedForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				attack3SpeedForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				attack3ForceForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				attack3RadiusForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeToPainForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeBetweenPainForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeTillDeadForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeToActualAttack1ForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeToActualAttack2ForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeToActualAttack3ForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeBetweenAttack1ForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeBetweenAttack2ForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeBetweenAttack3ForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeToChangeEnemyForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeIdleSFXMinForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeIdleSFXMaxForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeAttack1WaitMinForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeAttack1WaitMaxForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeAttack1WaitChanceForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeAttack2WaitMinForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeAttack2WaitMaxForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeAttack2WaitChanceForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeAttack3WaitMinForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeAttack3WaitMaxForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeAttack3WaitChanceForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				readInt = Utils.GetIntFromStringAudLogText(entries[i].Trim()); i++; //attack1ProjectileLaunchedTypeForNPC[refIndex] = GetPoolType(readInt);
				readInt = Utils.GetIntFromStringAudLogText(entries[i].Trim()); i++; //attack2ProjectileLaunchedTypeForNPC[refIndex] = GetPoolType(readInt);
				readInt = Utils.GetIntFromStringAudLogText(entries[i].Trim()); i++; //attack3ProjectileLaunchedTypeForNPC[refIndex] = GetPoolType(readInt); // Not worrying about projectile type for now, would require indexing all of the pools.
				projectileSpeedAttack1ForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				projectileSpeedAttack2ForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				projectileSpeedAttack3ForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				hasLaserOnAttack1ForNPC[refIndex] = Utils.GetBoolFromStringInTables(entries[i].Trim()); i++;
				hasLaserOnAttack2ForNPC[refIndex] = Utils.GetBoolFromStringInTables(entries[i].Trim()); i++;
				hasLaserOnAttack3ForNPC[refIndex] = Utils.GetBoolFromStringInTables(entries[i].Trim()); i++;
				explodeOnAttack3ForNPC[refIndex] = Utils.GetBoolFromStringInTables(entries[i].Trim()); i++;
				preactivateMeleeCollidersForNPC[refIndex] = Utils.GetBoolFromStringInTables(entries[i].Trim()); i++;
				huntTimeForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				flightHeightForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				flightHeightIsPercentageForNPC[refIndex] = Utils.GetBoolFromStringInTables(entries[i].Trim()); i++;
				switchMaterialOnDeathForNPC[refIndex] = Utils.GetBoolFromStringInTables(entries[i].Trim()); i++;
				hearingRangeForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				timeForTranquilizationForNPC[refIndex] = Utils.GetFloatFromStringDataTables(entries[i].Trim()); i++;
				hopsOnMoveForNPC[refIndex] = Utils.GetBoolFromStringInTables(entries[i].Trim()); i++;
				readInt = Utils.GetIntFromStringAudLogText(entries[i].Trim()); typeForNPC[refIndex] = Utils.GetNPCTypeFromInt(readInt); i++;
				projectile1PrefabForNPC[refIndex] = Utils.GetIntFromStringAudLogText(entries[i].Trim()); i++;
				projectile2PrefabForNPC[refIndex] = Utils.GetIntFromStringAudLogText(entries[i].Trim()); i++;
				projectile3PrefabForNPC[refIndex] = Utils.GetIntFromStringAudLogText(entries[i].Trim()); i++;

				currentline++;
				if (currentline > 29) break;
			} while (!dataReader.EndOfStream);
			dataReader.Close();
			return;
		}
	}
	
	Texture2D LoadTextureFromFile(string imgPath) {
		string path = Utils.SafePathCombine(Application.streamingAssetsPath,
											"textures/" + imgPath);
		
		Texture2D tex = new Texture2D(64, 64); // Default to world grid size for cases where DynamicCulling needs to index as [64,64].
		try {
			if (File.Exists(path)) {
				byte[] raw = File.ReadAllBytes(path);
				tex.LoadImage(raw);
				return tex;
			}
			goto CreateBlackTexture;
		} catch (Exception e) {
			UnityEngine.Debug.Log($"BUG: Failed to load texture at {path}: {e.Message}");
			goto CreateBlackTexture;
		}

CreateBlackTexture:
		Color[] pixels = new Color[64 * 64];
		for (int i = 0; i < pixels.Length; i++) {
			pixels[i] = Color.black;
		}
		tex.SetPixels(pixels);
		tex.Apply();
		return tex;
	}

	private void LoadTextures() {
        textures[0] =  LoadTextureFromFile("worldedgesclosed_0.png");
        textures[1] =  LoadTextureFromFile("worldedgesclosed_1.png");
        textures[2] =  LoadTextureFromFile("worldedgesclosed_2.png");
        textures[3] =  LoadTextureFromFile("worldedgesclosed_3.png");
        textures[4] =  LoadTextureFromFile("worldedgesclosed_4.png");
        textures[5] =  LoadTextureFromFile("worldedgesclosed_5.png");
        textures[6] =  LoadTextureFromFile("worldedgesclosed_6.png");
        textures[7] =  LoadTextureFromFile("worldedgesclosed_7.png");
        textures[8] =  LoadTextureFromFile("worldedgesclosed_8.png");
        textures[9] =  LoadTextureFromFile("worldedgesclosed_9.png");
        textures[10] = LoadTextureFromFile("worldedgesclosed_10.png");
        textures[11] = LoadTextureFromFile("worldedgesclosed_11.png");
        textures[12] = LoadTextureFromFile("worldedgesclosed_12.png");
        textures[13] = LoadTextureFromFile("worldcellopen_0.png");
        textures[14] = LoadTextureFromFile("worldcellopen_1.png");
        textures[15] = LoadTextureFromFile("worldcellopen_2.png");
        textures[16] = LoadTextureFromFile("worldcellopen_3.png");
        textures[17] = LoadTextureFromFile("worldcellopen_4.png");
        textures[18] = LoadTextureFromFile("worldcellopen_5.png");
        textures[19] = LoadTextureFromFile("worldcellopen_6.png");
        textures[20] = LoadTextureFromFile("worldcellopen_7.png");
        textures[21] = LoadTextureFromFile("worldcellopen_8.png");
        textures[22] = LoadTextureFromFile("worldcellopen_9.png");
        textures[23] = LoadTextureFromFile("worldcellopen_10.png");
        textures[24] = LoadTextureFromFile("worldcellopen_11.png");
        textures[25] = LoadTextureFromFile("worldcellopen_12.png");
		textures[26] = LoadTextureFromFile("worldcellskyvis_0.png");
        textures[27] = LoadTextureFromFile("worldcellskyvis_1.png");
        textures[28] = LoadTextureFromFile("worldcellskyvis_2.png");
        textures[29] = LoadTextureFromFile("worldcellskyvis_3.png");
        textures[30] = LoadTextureFromFile("worldcellskyvis_4.png");
        textures[31] = LoadTextureFromFile("worldcellskyvis_5.png");
        textures[32] = LoadTextureFromFile("worldcellskyvis_6.png");
        textures[33] = LoadTextureFromFile("worldcellskyvis_7.png");
        textures[34] = LoadTextureFromFile("worldcellskyvis_8.png");
        textures[35] = LoadTextureFromFile("worldcellskyvis_9.png");
        textures[36] = LoadTextureFromFile("worldcellskyvis_10.png");
        textures[37] = LoadTextureFromFile("worldcellskyvis_11.png");
        textures[38] = LoadTextureFromFile("worldcellskyvis_12.png");
	}

	public Sprite GetSpriteFromTexture(int useableItemIndex) {
		//Texture2D tex = textures[usableItemIndex + 13];
		Texture2D tex = useableItemsFrobIcons[useableItemIndex];
		return Sprite.Create(tex, new Rect(0,0,tex.width,tex.height),
							 new Vector2(0.5f,0.5f));
	}

	public static string GetTargetID(int npcIndex) {
		if (npcIndex > 29) return "BUG: npcIndex too large for GetTargetID!";

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

	// StatusBar Print
	public static void sprint(string input, GameObject player) {
		UnityEngine.Debug.Log(input);
		a.statusBar.SendText(input);
	}
	
	public static void sprint(int lingdex) {
		if (lingdex < 0) return;
		if (lingdex > Const.a.stringTable.Length) return;
		
		sprint(Const.a.stringTable[lingdex],null);
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
		case PoolType.CyberDissolve:
			poolContainer = Pool_CyberDissolve;
			poolName = "CyberDissolve ";
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

	void ClearAutomapOverlay(GameObject over) {
		Image img = over.GetComponent<Image>();
		Utils.DisableImage(img);
		Utils.Deactivate(over);
	}

	public void ClearActiveAutomapOverlays() {
		for (int i = 0; i < Pool_AutomapCameraOverlays.transform.childCount; i++) {
			Transform child = Pool_AutomapCameraOverlays.transform.GetChild(i);
			ClearAutomapOverlay(child.gameObject);
		}

		for (int i = 0; i < Pool_AutomapMutantOverlays.transform.childCount; i++) {
			Transform child = Pool_AutomapMutantOverlays.transform.GetChild(i);
			ClearAutomapOverlay(child.gameObject);
		}

		for (int i = 0; i < Pool_AutomapCyborgOverlays.transform.childCount; i++) {
			Transform child = Pool_AutomapCyborgOverlays.transform.GetChild(i);
			ClearAutomapOverlay(child.gameObject);
		}

		for (int i = 0; i < Pool_AutomapBotOverlays.transform.childCount; i++) {
			Transform child = Pool_AutomapBotOverlays.transform.GetChild(i);
			ClearAutomapOverlay(child.gameObject);
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
        if (Application.platform == RuntimePlatform.Android) return;
	    
		StartCoroutine(SaveRoutine(index,savename));
	}

	// Save the Game
	// ========================================================================
	public IEnumerator SaveRoutine(int saveFileIndex,string savename) {
		sprint(stringTable[194]); // Indicate we are saving "Saving..."
		yield return null; // Update to show this sprint.

		Stopwatch saveTimer = new Stopwatch();
		saveTimer.Start();
		List<string> saveData = new List<string>();
		int i,j;

		// All saveable classes
		List<GameObject> saveableGameObjects = new List<GameObject>();
		FindAllSaveObjectsGOs(ref saveableGameObjects);
		//UnityEngine.Debug.Log("Found "
		//					  + saveableGameObjects.Count.ToString()
		//					  + " total saveables for save.");

		if (string.IsNullOrWhiteSpace(savename)) {
			savename = "Unnamed " + saveFileIndex.ToString();
		}

		saveData.Add(savename);
		
		// Credit Stats and Times
		s1.Clear();
		s1.Append(Utils.FloatToString(PauseScript.a.relativeTime,"GameTime"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(PauseScript.a.absoluteTime,"TotalPlayTime"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(kills,"kills"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(kills,"cyberkills"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(shotsFired,"shotsFired"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(grenadesThrown,"grenadesThrown"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(damageDealt,"damageDealt"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.FloatToString(damageReceived,"damageReceived"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.IntToString(savesScummed,"savesScummed"));
		saveData.Add(s1.ToString());

		s1.Clear();
		s1.Append(LevelManager.Save(LevelManager.a.gameObject));
		s1.Append(Utils.splitChar);
		s1.Append(questData.Save());
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(difficultyCombat,"difficultyCombat"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(difficultyMission,"difficultyMission"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(difficultyPuzzle,"difficultyPuzzle"));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.UintToString(difficultyCyber,"difficultyCyber"));
		saveData.Add(s1.ToString());
		
		s1.Clear();

		// Save all the objects data
		for (i=0;i<saveableGameObjects.Count;i++) {
			// Take this object's data and add it to the array.
			saveData.Add(SaveObject.Save(saveableGameObjects[i])); // <<< THIS IS IT <<<
		}
		
		for (i=0;i<14;i++) {
			int dyncount = LevelManager.a.DynamicObjectsSavestrings[i].Count;
			for (j=0;j<dyncount;j++) {
				saveData.Add(LevelManager.a.DynamicObjectsSavestrings[i][j]);
			}
		}

		// Write to file
		string sName = "sav" + saveFileIndex.ToString() + ".txt";
		string sPath;
		sPath = Utils.SafePathCombine(Application.streamingAssetsPath,sName);
		StreamWriter sw = new StreamWriter(sPath,false,Encoding.ASCII);
		if (sw != null) {
			using (sw) {
				for (j=0;j<saveData.Count;j++) {
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
			sth.diffCombatCarryover = Const.a.difficultyCombat;
			sth.diffCyberCarryover = Const.a.difficultyCyber;
			sth.diffPuzzleCarryover = Const.a.difficultyPuzzle;
			sth.diffMissionCarryover = Const.a.difficultyMission;
			Cursor.lockState = CursorLockMode.None;
			DontDestroyOnLoad(newGameIndicator); // 4b.2.
			loadingScreen.SetActive(true);
			ReloadScene(sth); // 4b.3.
		} else { // 4a.
			//UnityEngine.Debug.Log("freshGame.name: " + freshGame.name);
			ClearActiveAutomapOverlays();
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
		if (saveIndicator != null) {
			SceneTransitionHandler sth = saveIndicator.GetComponent<SceneTransitionHandler>();
			UnityEngine.Debug.Log("Acquiring sth data");
			if (sth != null) {
				if (sth.setActiveAtNext) {
					Const.a.difficultyCombat = sth.diffCombatCarryover;
					Const.a.difficultyMission = sth.diffMissionCarryover;
					Const.a.difficultyPuzzle = sth.diffPuzzleCarryover;
					Const.a.difficultyCyber = sth.diffCyberCarryover;
				}
			}
			Utils.SafeDestroy(saveIndicator);
		}
		
		GameObject loadIndicator = GameObject.Find("LoadGameIndicator");
		if (loadIndicator != null) {
			SceneTransitionHandler sth = loadIndicator.GetComponent<SceneTransitionHandler>();
			UnityEngine.Debug.Log("Acquiring sth data");
			if (sth != null) {
				if (sth.setActiveAtNext) {
					Const.a.difficultyCombat = sth.diffCombatCarryover;
					Const.a.difficultyMission = sth.diffMissionCarryover;
					Const.a.difficultyPuzzle = sth.diffPuzzleCarryover;
					Const.a.difficultyCyber = sth.diffCyberCarryover;
				}
			}
			Utils.SafeDestroy(loadIndicator);
		}
		
		Cursor.visible = true;
		Utils.Deactivate(loadingScreen);
		Utils.Deactivate(MainMenuHandler.a.IntroVideo);
		Utils.Deactivate(MainMenuHandler.a.IntroVideoContainer);
		Automap.a.ActivateAutomapUI();
		Automap.a.DeactivateAutomapUI();
		Utils.Deactivate(PauseScript.a.mainMenu);
		PauseScript.a.PauseDisable();
		if (PlayerHealth.a != null) {
			if (PlayerHealth.a.hm != null) PlayerHealth.a.hm.ClearOverlays();
		}

		if (Const.a.NoShootMode) MouseLookScript.a.ForceInventoryMode();
		Utils.Activate(player1Capsule);
		Utils.Activate(player1CapsuleMainCameragGO.transform.parent.gameObject);
		Utils.Activate(player1CapsuleMainCameragGO);
		Utils.EnableCamera(MouseLookScript.a.playerCamera);
		WriteDatForIntroPlayed(false);
		if (loadTimer == null) {
			sprint(stringTable[197]);
		} else {
			sprint(stringTable[197] + " (" + loadTimer.Elapsed.ToString() + ")"); // Loading...Done!
		}
		
		DynamicCulling.a.Cull(false);
	}

	public void GoIntoGame() {
		GoIntoGame(null);
	}

	public void ShowLoading() {
		Utils.DisableCamera(MouseLookScript.a.playerCamera); // Hide changes.
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
	    if (Application.platform == RuntimePlatform.Android) return;
		ShowLoading();
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
		loading = true;
		UnityEngine.Debug.Log("Start of Load for index " + saveFileIndex.ToString());
		yield return null; // Update the view to show ShowLoading changes.

		string readline; 					// Initialize temporary variables.
		int numSaveablesFromSavefile = 0;
		int i,j,k;
		GameObject currentGameObjectInScene = null;
		List<GameObject> saveableGameObjectsInScene = new List<GameObject>();
		loadPercentText.text = "Preparing...";
		yield return null; // Update progress text.

		SaveObject.currentObjectInfo = "Start of Load...";

		// Remove and clear out everything and reset any lists.
		ClearActiveAutomapOverlays();
		TargetRegister.Clear();
		TargetnameRegister.Clear();
		for (i=0;i<healthObjectsRegistration.Length;i++) {
			healthObjectsRegistration[i] = null;
		}
		
		LevelManager.a.ResetSaveStrings();
		for (i=0;i<14;i++) {
			LevelManager.a.UnloadLevelDynamicObjects(i,false); // Delete them all!
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
		StreamReader sr;
		if (Application.platform == RuntimePlatform.Android) {
		    string fPath = Utils.SafePathCombine(Application.persistentDataPath,
										  lName);
										  
		    sr = new StreamReader(fPath, Encoding.ASCII);
		} else {
		    sr = Utils.ReadStreamingAsset(lName);
		}
		
		List<GameObject> allParents = 
			SceneManager.GetActiveScene().GetRootGameObjects().ToList();
			
		if (sr != null) {
			// Read the file into a list, line by line
			using (sr) {
				do {
					readline = sr.ReadLine();
					if (readline != null) readFileList.Add(readline);
				} while (!sr.EndOfStream);
				sr.Close();
			}

			loadPercentText.text = "Load Quest Data...     ";
			yield return null; // to update the sprint
			int numSaveFileLines = readFileList.Count;
			numSaveablesFromSavefile = numSaveFileLines - 3;

			// readFileList[0] == saveName;  Not important, we are loading already now
			//index = 0; // Uncomment this if we pull in the saveName from this line for something.

			// Read in global time, pause data, credit stats
			entries = readFileList[1].Split(Utils.splitCharChar);
			
			// The global time from which everything checks it's
			// somethingerotherFinished timer states.
			PauseScript.a.relativeTime = Utils.GetFloatFromString(entries[index],"GameTime"); index++;
			PauseScript.a.absoluteTime = Utils.GetFloatFromString(entries[index],"TotalPlayTime"); index++;
			kills = Utils.GetIntFromString(entries[index],"kills"); index++;
			cyberkills = Utils.GetIntFromString(entries[index],"cyberkills"); index++;
			shotsFired = Utils.GetIntFromString(entries[index],"shotsFired"); index++;
			grenadesThrown = Utils.GetIntFromString(entries[index],"grenadesThrown"); index++;
			damageDealt = Utils.GetFloatFromString(entries[index],"damageDealt"); index++;
			damageReceived = Utils.GetFloatFromString(entries[index],"damageReceived"); index++;
			savesScummed = 1 + Utils.GetIntFromString(entries[index],"savesScummed"); // 1+, you're doin' it now!
			index = 0; // reset before starting next line

			// Read in global states, difficulties, and quest mission bits.
			entries = readFileList[2].Split(Utils.splitCharChar);
			index = LevelManager.Load(LevelManager.a.gameObject,ref entries,index);
			index = questData.Load(ref entries,index);
			difficultyCombat = Utils.GetIntFromString(entries[index],"difficultyCombat"); index++;
			difficultyMission = Utils.GetIntFromString(entries[index],"difficultyMission"); index++;
			difficultyPuzzle = Utils.GetIntFromString(entries[index],"difficultyPuzzle"); index++;
			difficultyCyber = Utils.GetIntFromString(entries[index],"difficultyCyber"); index++;
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
				entries = readFileList[i].Split(Utils.splitCharChar);
				if (entries.Length < 1)  continue;

				saveFile_Line_SaveID[i] = Utils.GetIntFromString(entries[2],"SaveID");
				saveFile_Line_IsInstantiated[i] = Utils.GetBoolFromString(entries[3],"instantiated");
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
			loadPercentText.text = "Loading Static Objects: 0.0% (    0 / "
								   + numSaveablesFromSavefile.ToString() + ")";
			yield return null;
			loadUpdateTimer.Start(); // For loading update
			float perc = 0f;
			for (i = 3; i < numSaveFileLines; i++) {
				if (saveFile_Line_IsInstantiated[i]) continue; // Skip instantiables.

				alreadyLoadedLineFromSaveFile[i] = true;
				for (j=0;j<(saveableGameObjectsInScene.Count);j++) {
					if (alreadyCheckedThisSaveableGameObjectInScene[j]) continue; // skip checking this and doing GetComponent
					if (saveableGameObjectsInScene[j] == null) continue;

					currentGameObjectInScene = saveableGameObjectsInScene[j];
					currentSaveObjectInScene = SaveLoad.GetPrefabSaveObject(currentGameObjectInScene);
					if (!currentSaveObjectInScene.instantiated) alreadyCheckedThisInstantiableGameObjectInScene[j] = true; // Huge time saver right here!

					// Static Objects all have unique ID.
// 					if (currentSaveObjectInScene.SaveID == 999999) UnityEngine.Debug.Log("Checking player during load");
					if (currentSaveObjectInScene.SaveID == saveFile_Line_SaveID[i]
						&& currentSaveObjectInScene.SaveID != 0) {
						
// 						if (currentSaveObjectInScene.SaveID == 999999) UnityEngine.Debug.Log("Found player in savefile on line " + i.ToString() + " during load");

						//if (!saveableGameObjectsInScene[j].isStatic // EDITOR ONLY!!!
						if (currentSaveObjectInScene.instantiated
							&& currentSaveObjectInScene.saveType != SaveableType.Light) {
							UnityEngine.Debug.Log("For some reason, attempting "
												  + "to load to dynamic object "
												  + saveableGameObjectsInScene[j].name);
						}

						entries = readFileList[i].Split(Utils.splitCharChar);
						PrefabIdentifier prefID = SaveLoad.GetPrefabIdentifier(currentGameObjectInScene,true);
						SaveObject.Load(currentGameObjectInScene,ref entries,i,prefID);
						alreadyCheckedThisSaveableGameObjectInScene[j] = true; // Huge time saver right here!
						break;
					}
				}

				perc = (float)i/(float)numSaveablesFromSavefile*100f;
				loadPercentText.text = "Loading Static Objects: "
									   + perc.ToString("0.0") + "% ("
									   + i.ToString() + " / "
									   + numSaveablesFromSavefile.ToString()
									   + ")";
									   
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
			SaveObject sob;
			for (i=0;i<saveableGameObjectsInScene.Count;i++) {
				if (alreadyCheckedThisInstantiableGameObjectInScene[i]) {
					continue;
				}

				sob = SaveLoad.GetPrefabSaveObject(saveableGameObjectsInScene[i]);
				if (sob != null) {
					if (!sob.instantiated) {
						UnityEngine.Debug.Log(saveableGameObjectsInScene[i].name
						+ " not loaded during Static Pass and is static");
					} else {
						UnityEngine.Debug.Log(saveableGameObjectsInScene[i].name
						+ " not loaded during Static Pass and is not static");
					}
				} else {
					UnityEngine.Debug.Log(saveableGameObjectsInScene[i].name
						+ " not loaded during Static Pass and is not static");
				}
				numberOfMissedObjects++;
			}
			if (numberOfMissedObjects > 0) {
				UnityEngine.Debug.Log("numberOfMissedObjects: "
									  + numberOfMissedObjects.ToString());
			}

			// LOAD 7d. INSTANTIATE AND LOAD TO INSTANTIATED SAVEABLES
			// Now time to instantiate anything left that's supposed to be here
			loadUpdateTimer.Start(); // For loading update
			int constdex = -1; // To store the index of Master Index table.
			int levID = 1; // To store the level this was in.
			int savID = -1; // To store the SaveObject.SaveID.
			float percLoaded = 0f;
			GameObject instGO = null;
			GameObject contnr = null;
// 			UnityEngine.Debug.Log("numSaveFileLines: " + numSaveFileLines.ToString());
			for (i = 3 ; i < numSaveFileLines; i++) {
				if (alreadyLoadedLineFromSaveFile[i]) continue;

				entries = readFileList[i].Split(Utils.splitCharChar);
				if (entries.Length > 1) {
					constdex = Utils.GetIntFromString(entries[0],"constIndex");
					levID = Utils.GetIntFromString(entries[19],"levelID");
					if (!ConsoleEmulator.ConstIndexInBounds(constdex)) continue;

					// Already did LevelManager.a.LoadLevel above, and since its
					// savestrings lists were empty, safe to spawn dynamics now.
					savID = Utils.GetIntFromString(entries[2],"SaveID");
					if (ConsoleEmulator.ConstIndexIsNPC(constdex)) {
						contnr = LevelManager.a.GetRequestedLevelNPCContainer(levID);
						instGO = ConsoleEmulator.SpawnDynamicObject(constdex,levID,false,contnr,savID);
						PrefabIdentifier prefID = SaveLoad.GetPrefabIdentifier(instGO,true);
						SaveObject.Load(instGO,ref entries,i,prefID); // Load NPC.
					} else if (ConsoleEmulator.ConstIndexIsDynamicObject(constdex)) {
						// For DynamicObjects, if current level, go ahead and Instantiate new Prefabs, else add string to LevelManager's list for other levels.
						if (levID == LevelManager.a.currentLevel) {
							contnr = LevelManager.a.GetRequestedLevelDynamicContainer(levID);
							instGO = ConsoleEmulator.SpawnDynamicObject(constdex,levID,false,contnr,savID);
							PrefabIdentifier prefID = SaveLoad.GetPrefabIdentifier(instGO,true);
							SaveObject.Load(instGO,ref entries,i,prefID); // Load NPC.
						} else {
							if (levID < LevelManager.a.DynamicObjectsSavestrings.Length) { // levID < 14
								if (i < readFileList.Count && readFileList.Count > 0) {
									LevelManager.a.DynamicObjectsSavestrings[levID].Add(readFileList[i]);
								}
							}
						}
					}

				}

				percLoaded = ((float)i / (float)numSaveablesFromSavefile*100f);
				loadPercentText.text = "Loading Dynamic Objects: "
									   + percLoaded.ToString("0.0")
									   + "% (" + i.ToString() + " / "
									   + numSaveablesFromSavefile.ToString()
									   + ")";
				if (loadUpdateTimer.ElapsedMilliseconds > 500) {
					loadUpdateTimer.Reset();
					loadUpdateTimer.Start();
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					yield return null;
				}
			}
			
			// OK we read in all the dynamic objects above into the savestrings
			// list, now actaully instantiate them.
			LevelManager.a.LoadLevelDynamicObjects(LevelManager.a.currentLevel);
			loadUpdateTimer.Stop();

			// LOAD 8.  Repopulate registries as needed that were on Awake.
			for (i = 0; i < LevelManager.a.npcsm.Length; i++ ) {
				LevelManager.a.npcsm[i].RepopulateChildList();
			}
			
			if (Inventory.a.hasHardware[1]) {
				// Go through all HealthManagers in the game and initialize the
				// linked overlays now for Automap.  Done after instantiation.
				List<GameObject> hmGOs = new List<GameObject>();
				
				// Find all HealthManager components.
				bool includeInactive = true;
				for (i=0;i<allParents.Count;i++) {
					Component[] compArray =
						allParents[i].GetComponentsInChildren(
							typeof(HealthManager),includeInactive);

					// Add all gameObject with a HealthManager components.
					for (k=0;k<compArray.Length;k++) hmGOs.Add(compArray[k].gameObject);
				}

				for (i=0;i<hmGOs.Count;i++) {
					if (hmGOs[i] == null) continue;

					HealthManager hm = hmGOs[i].GetComponent<HealthManager>();
					if (hm == null) continue;

					if ((hm.isNPC || hm.isSecCamera)) {
						hm.Awake(); // Set up slots.
						hm.Start(); // Setup overlay.
					}
				}
			}
		}
		
		loadPercentText.text = "Re-register targets...";
		yield return new WaitForSeconds(0.01f);
		for (i=0;i<allParents.Count;i++) {
			Component[] compArray = allParents[i].GetComponentsInChildren(typeof(TargetIO),true); // find all SaveObject components, including inactive (hence the true here at the end)
			for (k=0;k<compArray.Length;k++) {
				TargetIO tio = compArray[k].gameObject.GetComponent<TargetIO>();
				if (tio != null) {
					tio.RemoteStart(this.gameObject,"LoadRoutine()"); // Reregister
				}
			}
		}
		ResetPauseLists();
		loadPercentText.text = "Re-init cull systems...";
		yield return new WaitForSeconds(0.01f);
		DynamicCulling.a.Cull_Init();
		DynamicCulling.a.CullCore();
		loadPercentText.text = "Cleaning Up...";
		yield return new WaitForSeconds(0.01f);

 		System.GC.Collect(); // Collect it all!
		System.GC.WaitForPendingFinalizers();
		AutoSplitterData.isLoading = false;
		loadTimer.Stop();
		loading = false;
		GoIntoGame(loadTimer);
	}

	public void NPCAudioOcclusion() {
		// Raytraced Audio Occlusion ;)
		int hitCount = 0;
		float newVolume = 1.0f;
		RaycastHit[] results = new RaycastHit[6];
		AIController aic = null;
		for (int i=0;i<healthObjectsRegistration.Length;i++) {
			if (healthObjectsRegistration[i] == null) continue;

			aic = healthObjectsRegistration[i].GetComponent<AIController>();
			if (aic == null) continue;
			if (aic.SFX == null) continue;
			if (aic.index < Const.a.sfxSightSoundForNPC.Length && aic.index >= 0) {
				if (Const.a.sfxSightSoundForNPC[aic.index] < Const.a.sounds.Length && Const.a.sfxSightSoundForNPC[aic.index] >= 0) {
					if (aic.SFX.clip == Const.a.sounds[Const.a.sfxSightSoundForNPC[aic.index]]) {
						aic.SFX.volume = aic.normalVolume;
						continue;
					}
				}
			}

			hitCount = Physics.RaycastNonAlloc(
						MouseLookScript.a.transform.position,
						aic.transform.position
						  - MouseLookScript.a.transform.position,
						results,32f,Const.a.layerMaskPlayerFrob,
						QueryTriggerInteraction.UseGlobal);

			aic.SFX.volume = aic.normalVolume;
			if (hitCount > 0) {
				if (hitCount > 5) {
					newVolume = aic.normalVolume * 0.65f;
				} else if (hitCount == 5) {
					newVolume = aic.normalVolume * 0.70f;
				} else if (hitCount == 4) {
					newVolume = aic.normalVolume * 0.75f;
				} else if (hitCount == 3) {
					newVolume = aic.normalVolume * 0.85f;
				} else if (hitCount == 2) {
					newVolume = aic.normalVolume * 0.90f;
				} else {
					newVolume = aic.normalVolume * 0.95f;
				}

				aic.SFX.volume = newVolume;
			}
		}
	}

	public void RegisterObjectWithHealth(HealthManager hm) {
		if (hm == null) return;

		for (int i=0;i<healthObjectsRegistration.Length;i++) {
			if (healthObjectsRegistration[i] != null) {
				if (healthObjectsRegistration[i] == hm) {
					return; // already in the list
				}
			}
		}

		int len = healthObjectsRegistration.Length;
		for (int i=0;i<len;i++) {
			if (healthObjectsRegistration[i] == null) {
				healthObjectsRegistration[i] = hm;
				return;
			}

			if (i == (len - 1)) {
				string msg = "WARNING: Could not register object with health. "
							 + " Hit limit of ";

				UnityEngine.Debug.Log(msg + len.ToString());
			}
		}
	}

	private void LockCPUScreenCode() {
		switch (LevelManager.a.currentLevel) {
			case 1:
				if (Const.a.questData.lev1SecCodeLocked) return;

				Const.a.questData.lev1SecCodeLocked = true;
				Const.a.questData.lev1SecCode = UnityEngine.Random.Range(0,10);
			break;
			case 2:
				if (Const.a.questData.lev2SecCodeLocked) return;

				Const.a.questData.lev2SecCodeLocked = true;
				Const.a.questData.lev2SecCode = UnityEngine.Random.Range(0,10);
				break;
			case 3:
				if (Const.a.questData.lev3SecCodeLocked) return;

				Const.a.questData.lev3SecCodeLocked = true;
				Const.a.questData.lev3SecCode = UnityEngine.Random.Range(0,10);
				break;
			case 4:
				if (Const.a.questData.lev4SecCodeLocked) return;

				Const.a.questData.lev4SecCodeLocked = true;
				Const.a.questData.lev4SecCode = UnityEngine.Random.Range(0,10);
				break;
			case 5:
				if (Const.a.questData.lev5SecCodeLocked) return;

				Const.a.questData.lev5SecCodeLocked = true;
				Const.a.questData.lev5SecCode = UnityEngine.Random.Range(0,10);
				break;
			case 6:
				if (Const.a.questData.lev6SecCodeLocked) return;

				Const.a.questData.lev6SecCodeLocked = true;
				Const.a.questData.lev6SecCode = UnityEngine.Random.Range(0,10);
				break;
		}
	}

	// Called by something's Use()
	public void UseTargets(GameObject go, UseData ud, string targetname) {
		if (loading) { UnityEngine.Debug.LogError("Attempted to UseTargets during loading!"); return; }
		
		if (go != null) {
			TargetIO tio = go.GetComponent<TargetIO>();
			if (tio != null) {
				ud.SetBits(tio);
			}
		}

		// First do things that don't actually need any other named object.
		if (ud.lockCodeToScreenMaterialChanger) LockCPUScreenCode();

		// Next check if targetname is valid.  This is fine if not, some
		// triggers we just want to play the trigger's SFX and do nothing else.
		if (string.IsNullOrWhiteSpace(targetname)) return;

		UseData tempUD = new UseData();
		float numtargetsfound = 0;
		// Find each gameobject with matching targetname in the register, then
		// call Use for each.
		bool succeeded = false;
		for (int i=0;i<TargetRegister.Count;i++) {
			if (TargetnameRegister.Count < 1) {
				UnityEngine.Debug.LogWarning("NO TARGETNAMES IN "
										   + "TargetnameRegister!!!");
				return;
			}

			if (TargetnameRegister[i] != targetname) continue;

			if (TargetRegister[i] != null) {
				numtargetsfound++;
				tempUD.CopyBitsFromUseData(ud);

				UnityEngine.Debug.Log("Running targets for " + targetname);

				// Added activeSelf bit to keep from spamming SetActive
				// when running targets through a trigger_multiple
				if (tempUD.GOSetActive && !TargetRegister[i].activeSelf) {
					//UnityEngine.Debug.Log("GOSetActive on " + targetname);
					TargetRegister[i].SetActive(true);
					succeeded = true;
				}

				// Diddo for activeSelf to prevent spamming SetActive.
				if (tempUD.GOSetDeactive && TargetRegister[i].activeSelf) {
					//UnityEngine.Debug.Log("GOSetDeactive on " + targetname);
					TargetRegister[i].SetActive(false);
					succeeded = true;
				}

				if (tempUD.GOToggleActive) {
					// If I abuse this with a trigger_multiple someone should
					// shoot me.
					TargetRegister[i].SetActive(!TargetRegister[i].activeSelf);
					succeeded = true;
				}

				TargetIO tio = TargetRegister[i].GetComponent<TargetIO>();
				tio.Targetted(tempUD);
				succeeded = true;
			}
		}

		if (!succeeded) {
			UnityEngine.Debug.LogWarning("Failed to find a matching targetname"
										 + " for " + targetname);
		}
	}

	// Should ONLY come from a TargetIO
	public void AddToTargetRegister(TargetIO tio) {
		string tn = tio.targetname;
		if (string.IsNullOrEmpty(tn)) return;

		GameObject go = tio.gameObject;		
// 		UnityEngine.Debug.Log("Target registering " + tn + " for " + go.name);
		int i = 0;
	    for (i=0;i<TargetRegister.Count; i++) {
	        if (TargetRegister[i] == null) continue;
	        if (TargetRegister[i] != go) continue; // Key check for whole loop.
			
	        // GameObject go is in registry already
            if (TargetnameRegister[i] == tn) {
// 				UnityEngine.Debug.Log(tn + " for " + go.name + " already in "
// 									  + "TargetRegister[], name and object");
				
                return; // Already in register, name and object.
            } else {
                TargetnameRegister[i] = tn; // Fix up partial registry.
//                 UnityEngine.Debug.Log(tn + " for " + go.name + " already in "
// 									  + "TargetRegister[], fix up partial");
				
                return; // Ok it's good now.
            }
	    }
	    
	    // GameObject isn't in registry, add fresh.
	    TargetRegister.Add(go);
		TargetnameRegister.Add(tn);
// 		if (TargetnameRegister.Count <= lastTargetRegistrySize) {
// 			UnityEngine.Debug.Log("Target register reset!");
// 		}
// 		
		lastTargetRegistrySize = TargetnameRegister.Count;
		
// 		UnityEngine.Debug.Log("Target registering " + tn + " for " + go.name
// 							  + "... complete! Registry now size of names["
// 							  + TargetnameRegister.Count.ToString() + "]/gos("
// 							  + TargetRegister.Count.ToString() + ")");
	}

	public void AddToTextLocalizationRegister(TextLocalization txtloc) {
		if (txtloc == null) return;

		TextLocalizationRegister.Add(txtloc);
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
			MouseLookScript.a.ScreenShake(force,1f);
		} else {
			// check if player is close enough and shake em' up!
			if (Vector3.Distance(transform.position,player1Capsule.transform.position) < distance) {
				MouseLookScript.a.ScreenShake(force,1f);
			}
		}
	}
	
	public float DifficultyIndex() {
	    float stupid = 0f;
	    stupid += (difficultyCombat * difficultyCombat);
        stupid += (difficultyPuzzle * difficultyPuzzle);
        stupid += (difficultyMission * difficultyMission);
        stupid += (difficultyCyber * difficultyCyber);
        return stupid;
	}
	
	public float GetScore(bool isFinal) {
        float stupid = DifficultyIndex();
        float score = 0f;
        float victories = (float)(kills + cyberkills);
        float secs = 0f;
        
        secs = Mathf.Floor(PauseScript.a.relativeTime / 3600f);
        if (!isFinal) { // Report score if no deaths.
            score = victories * 10000f;
            score -= Mathf.Min(score * 0.666f,secs * 100f);
            score *= ((stupid + 1f) / 37f);
            if (stupid > 35f) score += 2222222f; // secret kevin bonus
            return Mathf.Floor(score);
        }
        
        // Death is 10 anti-kills, but you always keep at least a third of your
        // kills.
        float deathPenalty = PlayerHealth.a.ressurections * 10f;
        score = victories - Mathf.Min(deathPenalty,victories * 0.666f);
        score *= 10000f;
        score -= Mathf.Min(score * 0.666f,secs * 100f);
        score *= ((stupid + 1f) / 37f); // 9 * 4 + 1 is best difficulty factor
        if (stupid > 35f) score += 2222222f; // secret kevin bonus
        return Mathf.Floor(score);
	}

	public string CreditsStats() {
	    StringBuilder s1 = new StringBuilder();
	    s1.Clear();
	    s1.Append("======================================================================");
        s1.Append("\n");
	    s1.Append("CITADEL");
	    s1.Append("\n");
	    s1.Append("======================================================================");
	    s1.Append("\n");
	    s1.Append("CONGRATULATIONS " + playerName);
	    s1.Append("\n");
	    
	    string hours, minutes, secs;
	    float t = PauseScript.a.relativeTime;
		float tb = (Mathf.Floor(t/3600f));
        hours = tb.ToString("0");
        t = t - (tb * 3600f);
        tb = Mathf.Floor(t / 60f); 
        minutes = tb.ToString("00");
        t = t - (tb * 60f);
        secs = t.ToString("00.000");
	    s1.Append("Straight Time: " + hours + "h " + minutes + "m " + secs
	              + "s");
	              
	    s1.Append("\n");
	    
	    t = PauseScript.a.absoluteTime;
		tb = (Mathf.Floor(t/3600f));
        hours = tb.ToString("0");
        t = t - (tb * 3600f);
        tb = Mathf.Floor(t / 60f); 
        minutes = tb.ToString("00");
        t = t - (tb * 60f);
        secs = t.ToString("00.000"); 
        s1.Append("Total Time (with reload from deaths): ");
	    s1.Append("\n");
	    
	    s1.Append("Kills: " + kills.ToString());
	    s1.Append("\n");
	    s1.Append("Kills in Cyberspace: " + cyberkills.ToString());
	    s1.Append("\n");
	    
	    s1.Append("Score Subtotal: " + GetScore(false).ToString("0"));
	    s1.Append("\n");
	    s1.Append("Deaths: " + PlayerHealth.a.deaths.ToString());
	    s1.Append("\n");
	    s1.Append("Ressurections: " + PlayerHealth.a.ressurections.ToString());
	    s1.Append("\n");
	    s1.Append("Combat: " + difficultyCombat.ToString()
	              + " | Puzzle: " + difficultyPuzzle.ToString()
	              + " | Mission: " + difficultyMission.ToString()
	              + " | Cyber: " + difficultyCyber.ToString());
	    s1.Append("\n");
	    s1.Append("Difficulty Index: " + DifficultyIndex().ToString());
	    s1.Append("\n");
	    s1.Append("Final Score: " + GetScore(true));
	    s1.Append("\n");
	    s1.Append("\n");
	    s1.Append("Shots Fired: " + shotsFired.ToString());
	    s1.Append("\n");
	    s1.Append("Grenades Thrown: " + grenadesThrown.ToString());
	    s1.Append("\n");
	    s1.Append("Damage Dealt: " + damageDealt.ToString());
	    s1.Append("\n");
	    s1.Append("Damage Received: " + damageReceived.ToString());
	    s1.Append("\n");
	    s1.Append("Saves Scummed: " + savesScummed.ToString());
	    s1.Append("\n");
	    s1.Append("\n");
	    s1.Append("Click to continue...");
		return s1.ToString();
	}
	
	void OnDestroy() {
		if (a == this) a = null;
		questData = null;
		useableItemsFrobIcons = null;
		useableItemsIcons = null;
		audiologNames = null;
		audiologSenders = null;
		audiologSubjects = null;
		audioLogs = null;
		audioLogType = null;
		audioLogSpeech2Text = null;
		audioLogLevelFound = null;
		sounds = null;
		attackTypeForWeapon = null;
		nameForNPC = null;
		attackTypeForNPC = null;
		attackTypeForNPC2 = null;
		attackTypeForNPC3 = null;
		perceptionForNPC = null;
		moveTypeForNPC = null;
		typeForNPC = null;
		creditsText = null;
		healthObjectsRegistration = null;
		player1 = null;
		Pool_CameraExplosions = null;
		Pool_BloodSpurtSmall = null;
		Pool_SparksSmall = null;
		Pool_SparksSmallBlue = null;
		Pool_BloodSpurtSmallYellow = null;
		Pool_BloodSpurtSmallGreen = null;
		Pool_HopperImpact = null;
		Pool_GrenadeFragExplosions = null;
		Pool_Vaporize = null;
		Pool_MagpulseImpacts = null;
		Pool_StungunImpacts = null;
		Pool_RailgunImpacts = null;
		Pool_PlasmaImpacts = null;
		Pool_ProjEnemShot6Impacts = null;
		Pool_ProjEnemShot2Impacts = null;
		Pool_ProjSeedPodsImpacts = null;
		Pool_TempAudioSources = null;
		Pool_GrenadeEMPExplosions = null;
		Pool_ProjEnemShot4Impacts = null;
		Pool_CrateExplosions = null;
		Pool_GrenadeFragLive = null;
		Pool_ConcussionLive = null;
		Pool_EMPLive = null;
		Pool_GasLive = null;
		Pool_GasExplosions = null;
		Pool_CorpseHit = null;
		Pool_LeafBurst = null;
		Pool_MutationBurst = null;
		Pool_GraytationBurst = null;
		Pool_BarrelExplosions = null;
		Pool_CyberDissolve = null;
		Pool_AutomapBotOverlays = null;
		Pool_AutomapCyborgOverlays = null;
		Pool_AutomapMutantOverlays = null;
		Pool_AutomapCameraOverlays = null;
		loadingScreen = null;
		mainMenuInit = null;
		statusBar = null;
		mainmenuMusic = null;
		InputCodeSettings = null;
		InputCodes = null;
		InputValues = null;
		InputConfigNames = null;
		mainFont1 = null;
		mainFont2 = null;
		TargetRegister = null;
		TargetnameRegister = null;
		stringTable = null;
		TextLocalizationRegister = null;
		reloadTime = null;
		screenCodes = null;
		logImages = null;
		eventSystem = null;
		prefabs = null;
		textures = null;
		sequenceTextures = null;
		loadPercentText = null;
		genericMaterials = null;
		ReverbRegister = null;
		sphereMesh = null;
		cubeMesh = null;
		segiEmitterMaterial1 = null;
		segiEmitterMaterialRed = null;
		segiEmitterMaterialGreen = null;
		segiEmitterMaterialBlue = null;
		segiEmitterMaterialPurple = null;
		segiEmitterMaterialRedFaint = null;
		testSphere = null;
		npcCount = null;
		audioLogImagesRefIndicesLH = null;
		audioLogImagesRefIndicesRH = null;
		player1TargettingPos = null;
		player1Capsule = null;
		player1PlayerMovementScript = null;
		player1PlayerHealthScript = null;
		player1CapsuleMainCameragGO = null;
		prb = null;
		psys = null;
		panimsList = null;
		s1 = null;
		s2 = null;
	}
	
	private string GetPrefabNameFromIndex(int constIndex) {
		switch (constIndex) {
			case 0: return "chunk_black";
			case 1: return "chunk_blocker";
			case 2: return "chunk_bridg1_1";
			case 3: return "chunk_bridg1_1flipx";
			case 4: return "chunk_bridg1_2";
			case 5: return "chunk_bridg1_3";
			case 6: return "chunk_bridg1_3_slice45";
			case 7: return "chunk_bridg1_3flipx";
			case 8: return "chunk_bridg1_4";
			case 9: return "chunk_bridg1_4_slice32";
			case 10: return "chunk_bridg1_4_slice32flipx";
			case 11: return "chunk_bridg1_5";
			case 12: return "chunk_bridg2_2";
			case 13: return "chunk_bridg2_3";
			case 14: return "chunk_bridg2_4";
			case 15: return "chunk_bridg2_5";
			case 16: return "chunk_bridg2_6";
			case 17: return "chunk_bridg2_7";
			case 18: return "chunk_bridg2_8";
			case 19: return "chunk_bridg2_9";
			case 20: return "chunk_crate_impenetrable";
			case 21: return "chunk_cyberpanel";
			case 22: return "chunk_cyberpanel_slice45";
			case 23: return "chunk_eng1_1";
			case 24: return "chunk_eng1_1d";
			case 25: return "chunk_eng1_2";
			case 26: return "chunk_eng1_2d";
			case 27: return "chunk_eng1_3";
			case 28: return "chunk_eng1_3d";
			case 29: return "chunk_eng1_4";
			case 30: return "chunk_eng1_5";
			case 31: return "chunk_eng1_5_slice45lh";
			case 32: return "chunk_eng1_5_slice45rh";
			case 33: return "chunk_eng1_5d";
			case 34: return "chunk_eng1_6";
			case 35: return "chunk_eng1_6d";
			case 36: return "chunk_eng1_7";
			case 37: return "chunk_eng1_7d";
			case 38: return "chunk_eng1_8";
			case 39: return "chunk_eng1_9";
			case 40: return "chunk_eng1_9d";
			case 41: return "chunk_eng2_1";
			case 42: return "chunk_eng2_1_slice45";
			case 43: return "chunk_eng2_1_slice384high";
			case 44: return "chunk_eng2_1_slice384highrh";
			case 45: return "chunk_eng2_1d";
			case 46: return "chunk_eng2_2";
			case 47: return "chunk_eng2_2d";
			case 48: return "chunk_eng2_3";
			case 49: return "chunk_eng2_3d";
			case 50: return "chunk_eng2_4";
			case 51: return "chunk_eng2_5";
			case 52: return "chunk_eng2_5_slice45";
			case 53: return "chunk_eng2_6";
			case 54: return "chunk_exec1_1";
			case 55: return "chunk_exec1_1d";
			case 56: return "chunk_exec1_2";
			case 57: return "chunk_exec1_2d";
			case 58: return "chunk_exec2_1";
			case 59: return "chunk_exec2_2";
			case 60: return "chunk_exec2_2d";
			case 61: return "chunk_exec2_3";
			case 62: return "chunk_exec2_4";
			case 63: return "chunk_exec2_4_slice45";
			case 64: return "chunk_exec2_5";
			case 65: return "chunk_exec2_6";
			case 66: return "chunk_exec2_7";
			case 67: return "chunk_exec3_1";
			case 68: return "chunk_exec3_1d";
			case 69: return "chunk_exec3_2";
			case 70: return "chunk_exec3_4";
			case 71: return "chunk_exec4_1";
			case 72: return "chunk_exec4_2";
			case 73: return "chunk_exec4_3";
			case 74: return "chunk_exec4_4";
			case 75: return "chunk_exec4_5";
			case 76: return "chunk_exec4_6";
			case 77: return "chunk_exec6_1";
			case 78: return "chunk_exteriorpanel1";
			case 79: return "chunk_fan1";
			case 80: return "chunk_flight1_1";
			case 81: return "chunk_flight1_1b";
			case 82: return "chunk_flight1_2";
			case 83: return "chunk_flight1_2_slice45rh";
			case 84: return "chunk_flight1_3";
			case 85: return "chunk_flight1_4";
			case 86: return "chunk_flight1_5";
			case 87: return "chunk_flight1_5_slice45lh";
			case 88: return "chunk_flight1_6";
			case 89: return "chunk_flight2_1";
			case 90: return "chunk_flight2_2";
			case 91: return "chunk_flight2_2_slice45";
			case 92: return "chunk_flight2_3";
			case 93: return "chunk_grove1_1";
			case 94: return "chunk_grove1_2";
			case 95: return "chunk_grove1_2_slice45";
			case 96: return "chunk_grove1_3";
			case 97: return "chunk_grove1_4";
			case 98: return "chunk_grove1_5";
			case 99: return "chunk_grove1_6";
			case 100: return "chunk_grove1_7";
			case 101: return "chunk_grove2_1";
			case 102: return "chunk_grove2_2";
			case 103: return "chunk_grove2_3";
			case 104: return "chunk_grove2_4";
			case 105: return "chunk_grove2_5";
			case 106: return "chunk_grove2_6";
			case 107: return "chunk_grove2_7";
			case 108: return "chunk_grove2_8";
			case 109: return "chunk_grove2_9";
			case 110: return "chunk_grove2_9b";
			case 111: return "chunk_grove2_9c";
			case 112: return "chunk_lift1";
			case 113: return "chunk_maint1_1";
			case 114: return "chunk_maint1_2";
			case 115: return "chunk_maint1_2d";
			case 116: return "chunk_maint1_3";
			case 117: return "chunk_maint1_3b";
			case 118: return "chunk_maint1_4";
			case 119: return "chunk_maint1_4b";
			case 120: return "chunk_maint1_5";
			case 121: return "chunk_maint1_6";
			case 122: return "chunk_maint1_7";
			case 123: return "chunk_blockerflightbay";
			case 124: return "chunk_maint1_9";
			case 125: return "chunk_maint1_9d";
			case 126: return "chunk_maint2_1";
			case 127: return "chunk_maint2_1b";
			case 128: return "chunk_maint2_1d";
			case 129: return "chunk_maint2_2";
			case 130: return "chunk_maint2_3";
			case 131: return "chunk_maint2_3d";
			case 132: return "chunk_maint2_4";
			case 133: return "chunk_maint2_4d";
			case 134: return "chunk_maint2_5";
			case 135: return "chunk_maint2_5d";
			case 136: return "chunk_maint2_6";
			case 137: return "chunk_maint2_6d";
			case 138: return "chunk_maint2_7";
			case 139: return "chunk_maint2_7d";
			case 140: return "chunk_maint2_8";
			case 141: return "chunk_maint2_9";
			case 142: return "chunk_maint2_9_slice45RH";
			case 143: return "chunk_maint2_9_slice128_top";
			case 144: return "chunk_maint3_1";
			case 145: return "chunk_maint3_1_slice32_lh";
			case 146: return "chunk_maint3_1_slice32_rh";
			case 147: return "chunk_maint3_1_slice45";
			case 148: return "chunk_maint3_1d";
			case 149: return "chunk_med1_1";
			case 150: return "chunk_med1_1_half_top";
			case 151: return "chunk_med1_1_slice128high";
			case 152: return "chunk_med1_1_slice192RH";
			case 153: return "chunk_med1_1_slice256";
			case 154: return "chunk_med1_1d";
			case 155: return "chunk_med1_2";
			case 156: return "chunk_med1_2d";
			case 157: return "chunk_med1_3";
			case 158: return "chunk_med1_3d";
			case 159: return "chunk_med1_4";
			case 160: return "chunk_med1_5";
			case 161: return "chunk_med1_6";
			case 162: return "chunk_med1_7";
			case 163: return "chunk_med1_7_slice14_64";
			case 164: return "chunk_med1_7_slice45_320lh";
			case 165: return "chunk_med1_7_slice45_320rh";
			case 166: return "chunk_med1_7_slice96high";
			case 167: return "chunk_med1_7d";
			case 168: return "chunk_med1_7d_slice128";
			case 169: return "chunk_med1_8";
			case 170: return "chunk_med1_8d";
			case 171: return "chunk_med1_9";
			case 172: return "chunk_black";
			case 173: return "chunk_black";
			case 174: return "chunk_med1_9d";
			case 175: return "chunk_black";
			case 176: return "chunk_med1_9d_ofs112_90";
			case 177: return "chunk_med1_9d_ofs144_90";
			case 178: return "chunk_med2_1";
			case 179: return "chunk_med2_1_slice32RH";
			case 180: return "chunk_med2_1d";
			case 181: return "chunk_med2_2";
			case 182: return "chunk_med2_2_half_bottom";
			case 183: return "chunk_med2_2d";
			case 184: return "chunk_med2_3";
			case 185: return "chunk_med2_3d";
			case 186: return "chunk_med2_4";
			case 187: return "chunk_med2_5";
			case 188: return "chunk_med2_6";
			case 189: return "chunk_med2_7";
			case 190: return "chunk_med2_8";
			case 191: return "chunk_med2_8_half_top";
			case 192: return "chunk_med2_8_slice32RH";
			case 193: return "chunk_med2_8_slice45";
			case 194: return "chunk_med2_9";
			case 195: return "chunk_med2_9d";
			case 196: return "chunk_med3_1";
			case 197: return "chunk_rad1_1";
			case 198: return "chunk_rad1_2";
			case 199: return "chunk_reac1_1";
			case 200: return "chunk_reac1_1_slice45";
			case 201: return "chunk_reac1_2";
			case 202: return "chunk_reac1_3";
			case 203: return "chunk_reac1_4";
			case 204: return "chunk_reac1_5";
			case 205: return "chunk_reac1_6";
			case 206: return "chunk_reac1_7";
			case 207: return "chunk_reac1_8";
			case 208: return "chunk_reac1_9";
			case 209: return "chunk_reac2_1";
			case 210: return "chunk_reac2_1_slice45LH";
			case 211: return "chunk_reac2_1_slice45LH_up";
			case 212: return "chunk_reac2_1_slice45RH";
			case 213: return "chunk_reac2_1_slice45RH_up";
			case 214: return "chunk_reac2_1b";
			case 215: return "chunk_reac2_1bmirror";
			case 216: return "chunk_reac2_1mirror";
			case 217: return "chunk_reac2_2";
			case 218: return "chunk_reac2_4";
			case 219: return "chunk_reac2_4_slice128lower";
			case 220: return "chunk_reac2_5";
			case 221: return "chunk_reac2_6";
			case 222: return "chunk_reac2_7";
			case 223: return "chunk_reac2_8";
			case 224: return "chunk_reac2_9";
			case 225: return "chunk_reac3_1";
			case 226: return "chunk_reac3_2";
			case 227: return "chunk_reac3_3";
			case 228: return "chunk_reac3_4";
			case 229: return "chunk_reac3_5";
			case 230: return "chunk_reac3_6";
			case 231: return "chunk_reac3_7";
			case 232: return "chunk_reac4_1";
			case 233: return "chunk_reac4_1_slice45lh";
			case 234: return "chunk_reac4_2";
			case 235: return "chunk_reac5_1";
			case 236: return "chunk_reac5_2";
			case 237: return "chunk_reac5_3";
			case 238: return "chunk_reac6_1";
			case 239: return "chunk_reac6_2";
			case 240: return "chunk_reac6_3";
			case 241: return "chunk_sci1_1";
			case 242: return "chunk_sci1_1_slice45_toplh";
			case 243: return "chunk_sci1_1_slice45_toprh";
			case 244: return "chunk_sci1_1d";
			case 245: return "chunk_sci1_2";
			case 246: return "chunk_sci1_2_slice45lh";
			case 247: return "chunk_sci1_2_slice45lh_up";
			case 248: return "chunk_sci1_2_slice45rh";
			case 249: return "chunk_sci1_2_slice45rh_up";
			case 250: return "chunk_sci1_2d";
			case 251: return "chunk_sci1_3";
			case 252: return "chunk_sci1_4";
			case 253: return "chunk_sci1_5";
			case 254: return "chunk_sci1_6";
			case 255: return "chunk_sci1_6_slice45";
			case 256: return "chunk_sci1_7";
			case 257: return "chunk_sci1_7d";
			case 258: return "chunk_sci1_8";
			case 259: return "chunk_sci1_8d";
			case 260: return "chunk_sci1_9";
			case 261: return "chunk_sci1_9d";
			case 262: return "chunk_sci2_1";
			case 263: return "chunk_sci2_1_slice45lh";
			case 264: return "chunk_sci2_1_slice45rh";
			case 265: return "chunk_sci2_1d";
			case 266: return "chunk_sci2_2";
			case 267: return "chunk_sci2_2d";
			case 268: return "chunk_sci2_3";
			case 269: return "chunk_sci2_4";
			case 270: return "chunk_sci2_5";
			case 271: return "chunk_sci2_5d";
			case 272: return "chunk_sci3_1";
			case 273: return "chunk_sci3_1d";
			case 274: return "chunk_sci3_2";
			case 275: return "chunk_sci3_3";
			case 276: return "chunk_sci3_4";
			case 277: return "chunk_sci3_5";
			case 278: return "chunk_sci3_6";
			case 279: return "chunk_screen";
			case 280: return "chunk_sec1_1";
			case 281: return "chunk_sec1_1b";
			case 282: return "chunk_sec1_1c";
			case 283: return "chunk_sec1_1c_slice45";
			case 284: return "chunk_sec1_1c_slice64highlh";
			case 285: return "chunk_sec1_1c_slice64highrh";
			case 286: return "chunk_black";
			case 287: return "chunk_black";
			case 288: return "chunk_sec1_2";
			case 289: return "chunk_sec1_2b";
			case 290: return "chunk_sec1_3";
			case 291: return "chunk_sec1_3_slice45";
			case 292: return "chunk_stor1_1";
			case 293: return "chunk_stor1_2";
			case 294: return "chunk_stor1_3";
			case 295: return "chunk_stor1_4";
			case 296: return "chunk_stor1_5";
			case 297: return "chunk_stor1_6";
			case 298: return "chunk_stor1_6_slice128_up_lh";
			case 299: return "chunk_stor1_6_slice128_up_rh";
			case 300: return "chunk_stor1_6_slice192lh";
			case 301: return "chunk_stor1_6_slice192rh";
			case 302: return "chunk_stor1_7";
			case 303: return "chunk_stor1_7_slice45";
			case 304: return "chunk_stor1_7d";
			case 305: return "chunk_teleporter";
			case 306: return "chunk_white";
			case 307: return "item_paper_wad";
			case 308: return "item_warecasing";
			case 309: return "item_beaker";
			case 310: return "item_beverage";
			case 311: return "item_skull";
			case 312: return "item_arm";
			case 313: return "item_audiolog";
			case 314: return "weapon_grenadefrag";
			case 315: return "weapon_grenadeconc";
			case 316: return "weapon_grenadeemp";
			case 317: return "weapon_grenadeearth";
			case 318: return "weapon_grenademine";
			case 319: return "weapon_grenadenitro";
			case 320: return "weapon_grenadegas";
			case 321: return "item_patch_berserk";
			case 322: return "item_patch_detox";
			case 323: return "item_patch_genius";
			case 324: return "item_patch_medi";
			case 325: return "item_patch_reflex";
			case 326: return "item_patch_sight";
			case 327: return "item_patch_staminup";
			case 328: return "item_hw_system";
			case 329: return "item_hw_navunit";
			case 330: return "item_hw_ereader";
			case 331: return "item_hw_sensaround";
			case 332: return "item_hw_targetid";
			case 333: return "item_hw_shield";
			case 334: return "item_hw_bio";
			case 335: return "item_hw_lantern";
			case 336: return "item_hw_envirosuit";
			case 337: return "item_hw_booster";
			case 338: return "item_hw_jumpjets";
			case 339: return "item_hw_infrared";
			case 340: return "item_fireextinguisher";
			case 341: return "item_access_card_admin";
			case 342: return "item_workerhelmet";
			case 343: return "weapon_mk3";
			case 344: return "weapon_blaster";
			case 345: return "weapon_dartgun";
			case 346: return "weapon_flechette";
			case 347: return "weapon_ionrifle";
			case 348: return "weapon_rapier";
			case 349: return "weapon_pipe";
			case 350: return "weapon_magnum";
			case 351: return "weapon_magpulse";
			case 352: return "weapon_pistol";
			case 353: return "weapon_plasma";
			case 354: return "weapon_railgun";
			case 355: return "weapon_riotgun";
			case 356: return "weapon_skorpion";
			case 357: return "weapon_sparqbeam";
			case 358: return "weapon_stungun";
			case 359: return "item_battery";
			case 360: return "item_battery_icad";
			case 361: return "item_logic_probe";
			case 362: return "item_healthkit";
			case 363: return "item_plastique";
			case 364: return "item_chipset_interfacedemod";
			case 365: return "item_flask";
			case 366: return "item_chipset_bitflag";
			case 367: return "item_ammo_rubber";
			case 368: return "item_isotopex22";
			case 369: return "item_testtube";
			case 370: return "weapon_grenadefrag_live";
			case 371: return "item_chipset_isolinear";
			case 372: return "weapon_grenadeconc_live";
			case 373: return "item_ammo_needle";
			case 374: return "item_ammo_tranq";
			case 375: return "item_ammo_standard";
			case 376: return "item_ammo_teflon";
			case 377: return "item_ammo_hollow";
			case 378: return "item_ammo_slug";
			case 379: return "item_ammo_magnesium";
			case 380: return "item_ammo_penetrator";
			case 381: return "item_ammo_hornet";
			case 382: return "item_ammo_splinter";
			case 383: return "item_ammo_rail";
			case 384: return "item_ammo_slag";
			case 385: return "item_ammo_slaglarge";
			case 386: return "item_ammo_magcart";
			case 387: return "weapon_grenadeemp_live";
			case 388: return "item_access_card_std";
			case 389: return "weapon_grenadeearth_live";
			case 390: return "item_access_card_group1";
			case 391: return "item_access_card_science";
			case 392: return "item_access_card_eng";
			case 393: return "item_access_card_groupB";
			case 394: return "item_access_card_security";
			case 395: return "item_access_card_per5diego";
			case 396: return "item_access_card_medi";
			case 397: return "item_access_card_group3";
			case 398: return "item_access_card_purple";
			case 399: return "item_head_male";
			case 400: return "item_head_female";
			case 401: return "item_severedhead";
			case 402: return "weapon_grenademine_live";
			case 403: return "weapon_grenadenitro_live";
			case 404: return "weapon_grenadegas_live";
			case 405: return "line_sparqbeam";
			case 406: return "line_blaster";
			case 407: return "line_ion";
			case 408: return "line_hopperbeam";
			case 409: return "red crosshair";
			case 410: return "orange crosshair";
			case 411: return "yellow crosshair";
			case 412: return "green crosshair small";
			case 413: return "teal crosshair";
			case 414: return "blue crosshair";
			case 415: return "cursor vmail";
			case 416: return "cursor cyberspace";
			case 417: return "item_access_card_perdarcy";
			case 418: return "null";
			case 419: return "npc_autobomb";
			case 420: return "npc_cyborg_assassin";
			case 421: return "npc_avian_mutant";
			case 422: return "npc_exec_bot";
			case 423: return "npc_cyborg_drone";
			case 424: return "npc_cortex_reaver";
			case 425: return "npc_cyborg_warrior";
			case 426: return "npc_cyborg_enforcer";
			case 427: return "npc_cyborg_elite";
			case 428: return "npc_cyborg_diego";
			case 429: return "npc_sec1_bot";
			case 430: return "npc_sec2_bot";
			case 431: return "npc_maint_bot";
			case 432: return "npc_mutant_cyborg";
			case 433: return "npc_hopper";
			case 434: return "npc_humanoid_mutant";
			case 435: return "npc_invisomut";
			case 436: return "npc_virus_mutant";
			case 437: return "npc_servbot";
			case 438: return "npc_flier_bot";
			case 439: return "npc_zerog_mutant";
			case 440: return "npc_gorilla_tiger_mutant";
			case 441: return "npc_repairbot";
			case 442: return "npc_plant_mutant";
			case 443: return "npc_cyberdog";
			case 444: return "npc_cyberguard";
			case 445: return "npc_cyberram";
			case 446: return "npc_cyber_reaver";
			case 447: return "npc_cybershodan";
			case 448: return "item_cyber_data";
			case 449: return "item_cyber_decoy";
			case 450: return "item_cyber_drill";
			case 451: return "item_cyber_game";
			case 452: return "item_cyber_integrity";
			case 453: return "item_cyber_keycard";
			case 454: return "item_cyber_pulser";
			case 455: return "item_cyber_recall";
			case 456: return "item_cyber_shield";
			case 457: return "item_cyber_turbo";
			case 458: return "prop_phys_barrel_chemical";
			case 459: return "prop_phys_barrel_radiation";
			case 460: return "prop_phys_barrel_toxic";
			case 461: return "prop_phys_cart";
			case 462: return "prop_phys_pot";
			case 463: return "prop_phys_toolcart";
			case 464: return "se_briefcase";
			case 465: return "se_corpse_blueshirt";
			case 466: return "se_corpse_brownshirt";
			case 467: return "se_corpse_eaten";
			case 468: return "se_corpse_labcoat";
			case 469: return "se_corpse_security";
			case 470: return "se_corpse_tan";
			case 471: return "se_corpse_torso";
			case 472: return "se_crate1";
			case 473: return "se_crate2";
			case 474: return "se_crate3";
			case 475: return "se_crate4";
			case 476: return "se_crate5";
			case 477: return "sec_camera";
			case 478: return "sec_cpunode";
			case 479: return "sec_cpunode_small";
			case 480: return "weapon_cyber_mine";
			case 481: return "proj_enemshot2";
			case 482: return "proj_magpulse_shot";
			case 483: return "proj_stungun_shot";
			case 484: return "proj_rail_shot";
			case 485: return "proj_plasmarifle_shot";
			case 486: return "proj_enemshot6";
			case 487: return "proj_enemshot5";
			case 488: return "proj_enemshot4";
			case 489: return "proj_throwingstar";
			case 490: return "proj_magpulsenpc_shot";
			case 491: return "proj_railnpc_shot";
			case 492: return "proj_cyberplayer_shot";
			case 493: return "proj_cyberdog_shot";
			case 494: return "proj_cyberreaver_shot";
			case 495: return "proj_cyberice_shot";
			case 496: return "doorA";
			case 497: return "doorB";
			case 498: return "doorC";
			case 499: return "doorD";
			case 500: return "doorE";
			case 501: return "doorF";
			case 502: return "doorG";
			case 503: return "doorH";
			case 504: return "doorI";
			case 505: return "doorJ";
			case 506: return "doorK";
			case 507: return "doorL";
			case 508: return "door_elevator1";
			case 509: return "door_elevator2";
			case 510: return "door_elevator3";
			case 511: return "door_elevator4";
			case 512: return "door_secret1";
			case 513: return "door_secret2";
			case 514: return "door_secret3";
			case 515: return "func_forcebridge";
			case 516: return "prop_lift2";
			case 517: return "func_wall";
			case 518: return "BulletHoleLarge";
			case 519: return "BulletHoleScorchLarge";
			case 520: return "BulletHoleScorchSmall";
			case 521: return "BulletHoleSmall";
			case 522: return "BulletHoleTiny";
			case 523: return "BulletHoleTinySpread";
			case 524: return "func_door_cyber";
			case 525: return "prop_console01";
			case 526: return "prop_console02";
			case 527: return "prop_grate1_1";
			case 528: return "prop_grate1_2";
			case 529: return "prop_grate1_3";
			case 530: return "se_cabinet";
			case 531: return "se_thermos";
			case 532: return "prop_beaker_holder";
			case 533: return "prop_bed";
			case 534: return "prop_bed_hospital";
			case 535: return "prop_bed_neurosurgery";
			case 536: return "prop_bonepile1";
			case 537: return "prop_bridgewall1";
			case 538: return "prop_broken_clock";
			case 539: return "prop_brokengun";
			case 540: return "prop_chair01";
			case 541: return "prop_chair02";
			case 542: return "prop_chair03";
			case 543: return "prop_chair04";
			case 544: return "prop_chair05";
			case 545: return "prop_chandelier";
			case 546: return "prop_charge_station";
			case 547: return "prop_clothes";
			case 548: return "prop_computer";
			case 549: return "prop_couch";
			case 550: return "prop_couch2";
			case 551: return "prop_cpuscreen";
			case 552: return "prop_cyber_datafrag";
			case 553: return "prop_cyber_decoy";
			case 554: return "prop_cyber_exit";
			case 555: return "prop_cyber_switch";
			case 556: return "prop_cyberport";
			case 557: return "prop_desk01";
			case 558: return "prop_desk02";
			case 559: return "prop_dexmissile";
			case 560: return "prop_foliage_fernpoison";
			case 561: return "prop_foliage_bush";
			case 562: return "prop_foliage_fern";
			case 563: return "prop_foliage_fernblueflower";
			case 564: return "prop_foliage_pinetreem";
			case 565: return "prop_foliage_poisonbush1";
			case 566: return "prop_gear_large";
			case 567: return "prop_gear_small";
			case 568: return "prop_grass1";
			case 569: return "prop_grass2";
			case 570: return "prop_grass3";
			case 571: return "prop_grass4";
			case 572: return "prop_grass5";
			case 573: return "prop_grate4";
			case 574: return "prop_healingbed";
			case 575: return "prop_lamp";
			case 576: return "prop_light_emergsignal";
			case 577: return "prop_microscope";
			case 578: return "prop_pipe";
			case 579: return "prop_puddle";
			case 580: return "prop_puddle_grease";
			case 581: return "prop_puddle_oil";
			case 582: return "prop_shelves";
			case 583: return "prop_skeleton";
			case 584: return "prop_sleeping_cables";
			case 585: return "prop_sparkingwire";
			case 586: return "prop_table";
			case 587: return "prop_tv_on_a_post";
			case 588: return "prop_vendingmachines1";
			case 589: return "prop_vendingmachines2";
			case 590: return "prop_weapon_rack";
			case 591: return "prop_xray";
			case 592: return "text_decal";
			case 593: return "text_decalStopDSS1";
			case 594: return "trigger_counter";
			case 595: return "trigger_cyberpush";
			case 596: return "trigger_gravitylift";
			case 597: return "trigger_ladder";
			case 598: return "trigger_multiple";
			case 599: return "trigger_music";
			case 600: return "trigger_once";
			case 601: return "trigger_radiation";
			case 602: return "us_isotopepanel";
			case 603: return "us_paperlog";
			case 604: return "us_puz_elevatorkeypad";
			case 605: return "us_puz_elevatorkeypad2";
			case 606: return "us_puz_elevatorkeypad3";
			case 607: return "us_puz_elevatorkeypad4";
			case 608: return "us_puz_keypad";
			case 609: return "us_puz_panel_blue_grid";
			case 610: return "us_puz_panel_brown_grid";
			case 611: return "us_puz_panel_gray_grid";
			case 612: return "us_puz_panel_red_grid";
			case 613: return "us_puz_panel_teal_grid";
			case 614: return "us_relaypanel";
			case 615: return "us_retinalscanner";
			case 616: return "prop_vending1_1";
			case 617: return "prop_vending1_2";
			case 618: return "prop_vending1_3";
			case 619: return "prop_vending2_1";
			case 620: return "prop_vending2_2";
			case 621: return "ambient_airhiss";
			case 622: return "ambient_clicker";
			case 623: return "ambient_compressor";
			case 624: return "ambient_dishwasher";
			case 625: return "ambient_drip_amb";
			case 626: return "ambient_fan";
			case 627: return "ambient_generator_gas";
			case 628: return "ambient_gurgle";
			case 629: return "ambient_icemaker";
			case 630: return "ambient_intake";
			case 631: return "ambient_lathe";
			case 632: return "ambient_lev3loop1";
			case 633: return "ambient_lev3loop2";
			case 634: return "ambient_lev3loop3";
			case 635: return "ambient_lev3loop4";
			case 636: return "ambient_liquid_bubble";
			case 637: return "ambient_liquid_lava2";
			case 638: return "ambient_looping";
			case 639: return "ambient_machgear_loop";
			case 640: return "ambient_machine_ambience";
			case 641: return "ambient_machine_go";
			case 642: return "ambient_machine_humamb7";
			case 643: return "ambient_machine_humlonoise";
			case 644: return "ambient_machine_loop1";
			case 645: return "ambient_machine_loop2";
			case 646: return "ambient_machinea1";
			case 647: return "ambient_machinevat_loop";
			case 648: return "ambient_mist";
			case 649: return "ambient_pipewater_loop";
			case 650: return "ambient_powerloom";
			case 651: return "ambient_pump";
			case 652: return "ambient_pump2";
			case 653: return "ambient_rain";
			case 654: return "ambient_steam_loop";
			case 655: return "ambient_washing_machine";
			case 656: return "decal_blood_die";
			case 657: return "decal_blood_resist";
			case 658: return "decal_blood_stayaway";
			case 659: return "decal_blood_words2";
			case 660: return "decal_bloodfonta";
			case 661: return "decal_bloodfonte";
			case 662: return "decal_bloodfontg";
			case 663: return "decal_bloodfonth";
			case 664: return "decal_bloodfontr";
			case 665: return "decal_bloodfonty";
			case 666: return "decal_bloodsplat2";
			case 667: return "decal_logo_antenna";
			case 668: return "decal_logo_armory";
			case 669: return "decal_logo_biohazard";
			case 670: return "decal_logo_bridge";
			case 671: return "decal_logo_cyborg";
			case 672: return "decal_logo_gears";
			case 673: return "decal_logo_medical";
			case 674: return "decal_logo_radhazard";
			case 675: return "decal_logo_research";
			case 676: return "decal_logo_security";
			case 677: return "decal_painting1";
			case 678: return "decal_painting2";
			case 679: return "decal_painting3";
			case 680: return "decal_posterbetterfuture";
			case 681: return "decal_postergenetics";
			case 682: return "decal_scorch1";
			case 683: return "decal_scorch2";
			case 684: return "decal_scorch3";
			case 685: return "decal_scorch4";
			case 686: return "decal_scorchtiny";
			case 687: return "decal_blood_splat";
			case 688: return "func_switch1";
			case 689: return "func_switch2";
			case 690: return "func_switch3";
			case 691: return "func_switch4";
			case 692: return "func_switch5";
			case 693: return "func_switch5broken";
			case 694: return "func_switch7";
			case 695: return "func_switch8";
			case 696: return "func_switchbroken1";
			case 697: return "clip_npc";
			case 698: return "clip_objects";
			case 699: return "logic_relay";
			case 700: return "logic_branch";
			case 701: return "logic_timer";
			case 702: return "logic_spawner";
			case 703: return "info_teleport_destination";
			case 704: return "prop_debris_panel";
			case 705: return "info_cyborgconversion";
			case 706: return "info_elev_destination";
			case 707: return "info_email";
			case 708: return "info_gameend";
			case 709: return "info_message";
			case 710: return "info_mission";
			case 711: return "info_note";
			case 712: return "info_playsound";
			case 713: return "info_ressurection_point";
			case 714: return "info_screenshake";
			case 715: return "info_spawnpoint";
			case 716: return "fx_reverbzone";
			case 717: return "ef_cyber_ice";
			case 718: return "ef_fragexplosion";
			case 719: return "ef_line_sparqbeam";
			case 720: return "ef_mist";
			case 721: return "ef_particle_bloodspurtsmall";
			case 722: return "ef_particle_bloodspurtsmallgreen";
			case 723: return "ef_particle_bloodspurtsmallyellow";
			case 724: return "ef_particle_bloodspurttiny";
			case 725: return "ef_particle_camerahit";
			case 726: return "ef_particle_darthit";
			case 727: return "ef_particle_sec2muzburst";
			case 728: return "ef_particle_sec2rotmuzburst";
			case 729: return "ef_particle_sparksmall";
			case 730: return "ef_particle_sparksmallblue";
			case 731: return "ef_particle_sparqhit";
			case 732: return "ef_sparkspits";
			case 733: return "ef_spraydrips";
			case 734: return "ef_steam";
			case 735: return "env_sparksmall";
			case 736: return "TargetIDInstance";
			case 737: return "prop_papers01";
			case 738: return "prop_papers02";
			case 739: return "ef_particle_blasterhit";
			case 740: return "ef_particle_ionhit";
			case 741: return "us_puz_panel_blue_wire";
			case 742: return "us_puz_panel_brown_wire";
			case 743: return "us_puz_panel_gray_wire";
			case 744: return "us_puz_panel_red_wire";
			case 745: return "us_puz_panel_teal_wire";
			case 746: return "weapon_grenadeenergmine_live";
			case 747: return "decal_logo_storage";
		}
		
		return "item_paper_wad";
	}
	
	public void ClearPrefabs() {
		prefabs = null;
	}
	
	// Must not return null.  Returns item_paper_wad as fallback (0 is fallback constIndex elsewhere).
	public GameObject GetPrefab(int constIndex) {
		if (prefabs == null) prefabs = new GameObject[748];
		if (prefabs.Length < 748) prefabs = new GameObject[748];
		if (constIndex < 0 || constIndex >= 748) { UnityEngine.Debug.LogWarning("Invalid constIndex of " + constIndex.ToString() + " passed to GetPrefab"); return prefabFallback; }
		
		if (prefabs[constIndex] == null) {
			string prefabName = GetPrefabNameFromIndex(constIndex);
			GameObject loadedPrefab = Resources.Load<GameObject>("Prefabs/" + prefabName);
			
			if (loadedPrefab == null) {
				UnityEngine.Debug.LogError("Failed to load prefab: Prefabs/" + prefabName + "; returning item_paper_wad.");
				return prefabFallback;
			}

			prefabs[constIndex] = loadedPrefab;
		}
		
		return prefabs[constIndex];
	}
}
