using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReferenceManager : MonoBehaviour {
	// External references, required
	public GameObject playerCapsule;
	public GameObject playerCapsuleHardwareLantern;
	public GameObject playerCapsuleHardwareEReader;
	public GameObject playerCapsuleMainCamera;
	public GameObject playerCapsuleMainCameraGunCamera;
	public GameObject playerInventory;
	public GameObject playerCanvas;
	public GameObject playerCursor;
	public GameObject playerStatusBar;
	public GameObject playerTextWarningManager;
	public GameObject playerDeathRessurectEffect;
	public GameObject playerDeathEffect;
	public GameObject playerRadiationTreatmentFlash;

	// Internal references
	[HideInInspector] public int playerCurrentLevel;

	public static PlayerReferenceManager a;

	void Awake() {
		a = this;
		a.playerCurrentLevel = LevelManager.a.currentLevel;
	}
}
