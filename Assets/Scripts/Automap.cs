﻿using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class Automap : MonoBehaviour {
	public Camera automapCamera;
	public GameObject automapCanvasGO;
	public GameObject automapContainerLH;
	public GameObject automapContainerRH;
	public GameObject automapTabLH;
	public GameObject automapTabRH;
	public GameObject automapFull;
	public Image[] automapFoWTiles;
	public Image automapBaseImage;
	public Image automapInnerCircle;
	public Image automapOuterCircle;
	public Sprite[] automapsBaseImages;
	public Image[] automapsHazardOverlays;
	public Transform automapFullPlayerIcon;
	public Transform automapNormalPlayerIconLH;
	public Transform automapNormalPlayerIconRH;
	public GameObject[] levelOverlayContainer;
	public GameObject levelOverlayContainerR;
	public GameObject levelOverlayContainer1;
	public GameObject levelOverlayContainer2;
	public GameObject levelOverlayContainer3;
	public GameObject levelOverlayContainer4;
	public GameObject levelOverlayContainer5;
	public GameObject levelOverlayContainer6;
	public GameObject levelOverlayContainer7;
	public GameObject levelOverlayContainer8;
	public GameObject levelOverlayContainer9;
	public GameObject levelOverlayContainerG1;
	public GameObject levelOverlayContainerG2;
	public GameObject levelOverlayContainerG4;
	public Vector2[] automapLevelHomePositions;
	public GameObject poolContainerAutomapBotOverlays;
	public GameObject poolContainerAutomapMutantOverlays;
	public GameObject poolContainerAutomapCyborgOverlays;
	public bool[] automapExploredR; // save
	public bool[] automapExplored1; // save
	public bool[] automapExplored2; // save
	public bool[] automapExplored3; // save
	public bool[] automapExplored4; // save
	public bool[] automapExplored5; // save
	public bool[] automapExplored6; // save
	public bool[] automapExplored7; // save
	public bool[] automapExplored8; // save
	public bool[] automapExplored9; // save
	public bool[] automapExploredG1; // save
	public bool[] automapExploredG2; // save
	public bool[] automapExploredG4; // save

	[HideInInspector] public bool inFullMap;
	[HideInInspector] public int currentAutomapZoomLevel = 0;
	private float automapUpdateFinished; // save
	private bool[] automapExplored;
	private float automapZoom0 = 1.2f;
	private float automapZoom1 = 0.75f;
	private float automapZoom2 = 0.55f;
	// private float circleInnerRangev1 = 7.679999f; //(2.5f * 2.56f) + 1.28f;
	// private float circleOuterRangev1 = 11.52f; //(4f * 2.56f) + 1.28f;
	// private float circleInnerRangev2 = 8.96f; //(3f * 2.56f) + 1.28f;
	// private float circleOuterRangev2 = 12.8f; //(4.5f * 2.56f) + 1.28f;
	// private float circleInnerRangev3 = 14.08f; //(5f * 2.56f) + 1.28f;
	// private float circleOuterRangev3 = 20.48f; //(7.5f * 2.56f) + 1.28f;
	// private float automapFactorx = 1.25f;
	// private float automapFactory = 1.135f;
	private float automapCorrectionX = -0.008f;
	private float automapCorrectionY = 0.099f;
	private float automapTileCorrectionX = -516;
	private float automapTileCorrectionY = -516;
	private float automapFoWRadius = 30f;
	private float automapTileBCorrectionX = 0f;
	private float automapTileBCorrectionY = 0f;
	private float icoZAdj;
	private float updateTime;
	private Vector3 tempVec;
	private Vector3 tempVec2;
	private Vector2 tempVec2b;
	private Transform automapCameraTransform;
	private RectTransform[] automapFoWTilesRects;
	private bool initialized = false;

	public static Automap a;

	void Awake() {
		a = this;
		a.initialized = false;
	}

	void Start() {
		automapExplored = new bool[4096];
		automapUpdateFinished = PauseScript.a.relativeTime;
		AutomapZoomAdjust();
		icoZAdj = 0f;
		automapCameraTransform = automapCamera.transform;
		automapExploredR = new bool[4096];
		automapExplored1 = new bool[4096];
		automapExplored2 = new bool[4096];
		automapExplored3 = new bool[4096];
		automapExplored4 = new bool[4096];
		automapExplored5 = new bool[4096];
		automapExplored6 = new bool[4096];
		automapExplored7 = new bool[4096];
		automapExplored8 = new bool[4096];
		automapExplored9 = new bool[4096];
		automapExploredG1 = new bool[4096];
		automapExploredG2 = new bool[4096];
		automapExploredG4 = new bool[4096];
		automapFoWTilesRects = new RectTransform[automapFoWTiles.Length];
		for (int i=0;i<automapFoWTiles.Length;i++) {
			automapFoWTilesRects[i] = automapFoWTiles[i].rectTransform;
			automapExploredR[i] = false;
			automapExplored1[i] = false;
			automapExplored2[i] = false;
			automapExplored3[i] = false;
			automapExplored4[i] = false;
			automapExplored5[i] = false;
			automapExplored6[i] = false;
			automapExplored7[i] = false;
			automapExplored8[i] = false;
			automapExplored9[i] = false;
			automapExploredG1[i] = false;
			automapExploredG2[i] = false;
			automapExploredG4[i] = false;
		}

		automapLevelHomePositions[0] =  new Vector2(  43.97f,  85.66f); // R
		automapLevelHomePositions[1] =  new Vector2(  -8.53f,  85.99f);
		automapLevelHomePositions[2] =  new Vector2(  10.20f,  44.80f);
		automapLevelHomePositions[3] =  new Vector2(   9.40f,  63.83f);
		automapLevelHomePositions[4] =  new Vector2( -55.65f, 116.80f);
		automapLevelHomePositions[5] =  new Vector2(  -9.40f,  71.80f);
		automapLevelHomePositions[6] =  new Vector2(  29.70f,  85.50f);
		automapLevelHomePositions[7] =  new Vector2(   5.00f,  76.55f);
		automapLevelHomePositions[8] =  new Vector2(  25.10f,  84.40f);
		automapLevelHomePositions[9] =  new Vector2(  39.80f,  72.60f);
		automapLevelHomePositions[10] = new Vector2( 440.80f, 200.60f); // G1
		automapLevelHomePositions[11] = new Vector2(  80.16f,-196.68f); // G2
		automapLevelHomePositions[12] = new Vector2(  99.50f, 416.90f); // G4
		automapLevelHomePositions[13] = new Vector2(   0.00f,   0.00f);
		initialized = true;

		if (LevelManager.a != null)
			SetAutomapExploredReference(LevelManager.a.currentLevel);
		else
			SetAutomapExploredReference(1);
	}

	public void UpdateAutomap(Vector3 playerPosition) {
		if (PlayerMovement.a.inCyberSpace) return;
		if (!initialized) Start();

		if (AutoMapDisplayActive()) {
			Utils.Activate(automapCamera.gameObject);
			Utils.EnableCamera(automapCamera);
		} else {
			Utils.Deactivate(automapCamera.gameObject);
			Utils.DisableCamera(automapCamera);
		}

		if (Inventory.a.hardwareVersion[1] < 2) {
			Utils.Deactivate(poolContainerAutomapBotOverlays);
		} else {
			Utils.Activate(poolContainerAutomapBotOverlays);
		}

		if (Inventory.a.hardwareVersion[1] < 3) {
			Utils.Deactivate(poolContainerAutomapCyborgOverlays);
			Utils.Deactivate(poolContainerAutomapMutantOverlays);
		} else {
			Utils.Activate(poolContainerAutomapCyborgOverlays);
			Utils.Activate(poolContainerAutomapMutantOverlays);
		}

		if (automapUpdateFinished < PauseScript.a.relativeTime) {
			Utils.EnableImage(automapBaseImage);
			Utils.AssignImageOverride(automapBaseImage,
				automapsBaseImages[LevelManager.a.currentLevel]);

			float mapWidth = (Const.a.mapWorldMaxW - Const.a.mapWorldMaxE);
			float mapHeight = (Const.a.mapWorldMaxN - Const.a.mapWorldMaxS);
			float ewOffset = (playerPosition.z - Const.a.mapWorldMaxE);
			float nsOffset = (playerPosition.x - Const.a.mapWorldMaxS);

			tempVec.x = ((ewOffset / mapWidth) * (Const.a.camMaxAmount * 2f))
						+ (Const.a.camMaxAmount * -1f);

			tempVec.y = ((nsOffset / mapHeight) * (Const.a.camMaxAmount * 2f))
						+ (Const.a.camMaxAmount * -1f);

			tempVec.z = automapCameraTransform.localPosition.z;
			tempVec.x = (tempVec.x * -1f) + automapCorrectionX;
			tempVec.y += automapCorrectionY;

			// private float mapTileMinX = 8; // top left corner
			// private float mapTileMaxY = -8; // top left corner
			// private float mapTileMinY = -1016; // bottom right corner
			// private float mapTileMaxX = 1016; // bottom right corner
			tempVec2b.x = ((ewOffset/mapWidth) * 1008f)
						  + Const.a.mapTileMinX + automapTileBCorrectionX;

			tempVec2b.y = (((nsOffset/mapHeight) * 1008f)
						  + Const.a.mapTileMinY + automapTileBCorrectionY);

			if (inFullMap) {
				tempVec2b.x -= automapTileBCorrectionX;
				tempVec2b.y -= automapTileBCorrectionY;
				automapFullPlayerIcon.localPosition = tempVec2b;

				// Move the map to center.
				tempVec.x = 0;
				tempVec.y = 0;
				automapCameraTransform.localPosition = tempVec; 
			} else {
				// Move the map to reflect player movement.
				automapCameraTransform.localPosition = tempVec;
			}

			// Update player icon rotation.
			// Rotation is adjusted for player view and direction vs UI space.
			icoZAdj = (PlayerMovement.a.transform.eulerAngles.y * (-1) + 90);

			float zLH = automapNormalPlayerIconLH.localRotation.z;
			float zRH = automapNormalPlayerIconRH.localRotation.z;
			float zFull = automapFullPlayerIcon.localRotation.z;
			Quaternion icoQ = Quaternion.Euler(0,0,icoZAdj);
			if (Mathf.Abs(zLH - icoZAdj) > 0.5f) {
				automapNormalPlayerIconLH.localRotation = icoQ;
			}

			if (Mathf.Abs(zRH - icoZAdj) > 0.5f) {
				automapNormalPlayerIconRH.localRotation = icoQ;
			}

			if (Mathf.Abs(zFull - icoZAdj) > 0.5f) {
				automapFullPlayerIcon.localRotation = icoQ;
			}

			// Update explored tiles
			for (int i=0;i<4096;i++) {
				if (automapExplored[i]) {
					Utils.DisableImage(automapFoWTiles[i]);
					Utils.Deactivate(automapFoWTiles[i].gameObject);
				} else {
					tempVec2.x = automapFoWTilesRects[i].localPosition.x * -1f
								 - automapTileCorrectionX;
					tempVec2.y = automapFoWTilesRects[i].localPosition.y
								 + automapTileCorrectionY;
					if (Vector2.Distance(tempVec2,tempVec2b) < automapFoWRadius) {
						automapExplored[i] = true;
						SetAutomapTileExplored(LevelManager.a.currentLevel,i);
						Utils.DisableImage(automapFoWTiles[i]);
						Utils.Deactivate(automapFoWTiles[i].gameObject);
					} else {
						Utils.EnableImage(automapFoWTiles[i]);
						Utils.Activate(automapFoWTiles[i].gameObject);
					}
				}
			}

			updateTime = 0.2f;
			if (Inventory.a.hardwareVersion[1] > 1) updateTime = 0.1f;
			if (Inventory.a.hardwareVersion[1] > 2) {
				updateTime = 0.05f;

				// Display hazards
				for (int j=0;j<13;j++) {
					if (j != LevelManager.a.currentLevel) {
						Utils.DisableImage(automapsHazardOverlays[j]);
						Utils.Deactivate(automapsHazardOverlays[j].gameObject);
					} else {
						Utils.EnableImage(automapsHazardOverlays[j]);
						Utils.Activate(automapsHazardOverlays[j].gameObject);
					}
				}

				// Display cyborg and mutant overlays - Handled by AIController
				// since it updates it anyways.
			}
			automapUpdateFinished = PauseScript.a.relativeTime + updateTime;
		}

		SetAutomapActiveState();
	}

	void ActivateAutomapUI() {
		Utils.EnableCamera(automapCamera);
		Utils.Activate(automapCanvasGO);
		ActivateLevelOverlayContainer(LevelManager.a.currentLevel);
	}

	void DeactivateAutomapUI() {
		Utils.DisableCamera(automapCamera);
		Utils.Deactivate(automapCanvasGO);
		ActivateLevelOverlayContainer(-1);
	}

	void SetAutomapActiveState() {
		if (Inventory.a.hasHardware[1]) {
			if (AutoMapDisplayActive()) {
				ActivateAutomapUI();
			} else {
				DeactivateAutomapUI();
			}
		} else {
			DeactivateAutomapUI();
		}
	}

	// True if, full map is displayed or either left or right automap.
	bool AutoMapDisplayActive() {
		if (automapTabLH.activeInHierarchy) return true;
		if (automapTabRH.activeInHierarchy) return true;
		if (automapFull.activeInHierarchy) return true;
		return false;
	}

	void ActivateLevelOverlayContainer(int current) {
		if (!initialized) Start();
		for (int i=0;i<levelOverlayContainer.Length;i++) {
			if (i == current) continue;

			Utils.Deactivate(levelOverlayContainer[i]);
		}

		if (current < 0 || current >= levelOverlayContainer.Length) return;

		Utils.Activate(levelOverlayContainer[current]);
	}

	public void SetAutomapExploredReference(int currentLevel) {
		if (!initialized) Start();
		switch(currentLevel) {
			case 0: for (int i=0;i<4096;i++) { automapExplored[i] = automapExploredR[i]; } break;
			case 1: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored1[i]; } break;
			case 2: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored2[i]; } break;
			case 3: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored3[i]; } break;
			case 4: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored4[i]; } break;
			case 5: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored5[i]; } break;
			case 6: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored6[i]; } break;
			case 7: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored7[i]; } break;
			case 8: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored8[i]; } break;
			case 9: for (int i=0;i<4096;i++) { automapExplored[i] = automapExplored9[i]; } break;
			case 10:for (int i=0;i<4096;i++) { automapExplored[i] = automapExploredG1[i];} break;
			case 11:for (int i=0;i<4096;i++) { automapExplored[i] = automapExploredG2[i];} break;
			case 12:for (int i=0;i<4096;i++) { automapExplored[i] = automapExploredG4[i];} break;
		}
	}

	void SetAutomapTileExplored(int currentLevel, int index) {
		if (!initialized) Start();
		switch(currentLevel) {
			case 0: automapExploredR[index] = true; break;
			case 1: automapExplored1[index] = true; break;
			case 2: automapExplored2[index] = true; break;
			case 3: automapExplored3[index] = true; break;
			case 4: automapExplored4[index] = true; break;
			case 5: automapExplored5[index] = true; break;
			case 6: automapExplored6[index] = true; break;
			case 7: automapExplored7[index] = true; break;
			case 8: automapExplored8[index] = true; break;
			case 9: automapExplored9[index] = true; break;
			case 10:automapExploredG1[index]= true; break;
			case 11:automapExploredG2[index]= true; break;
			case 12:automapExploredG4[index]= true; break;
		}
	}

	public void AutomapZoomOut() {
		if (Inventory.a.hardwareVersion[1] < 2) {
			// Map hardware version doesn't support zoom.
			Const.sprint(Const.a.stringTable[465],Const.a.player1);
			return;
		}

		currentAutomapZoomLevel++;
		if (currentAutomapZoomLevel > 2) {
			currentAutomapZoomLevel = 2;

			// zoom at max
			Const.sprint(Const.a.stringTable[316],Const.a.player1);
			return;
		}
		AutomapZoomAdjust();
	}

	public void AutomapZoomIn() {
		if (Inventory.a.hardwareVersion[1] < 2) {
			// Map hardware version doesn't support zoom.
			Const.sprint(Const.a.stringTable[465],Const.a.player1);
			return;
		}

		currentAutomapZoomLevel--;
		if (currentAutomapZoomLevel < 0) {
			currentAutomapZoomLevel = 0;

			// zoom at min
			Const.sprint(Const.a.stringTable[317],Const.a.player1);
			return;
		}
		AutomapZoomAdjust();
	}

	void AutomapZoomAdjust() {
		float zoom = automapZoom0;
		switch (currentAutomapZoomLevel) {
			case 0:  zoom = automapZoom0; break;
			case 1:  zoom = automapZoom1; break;
			case 2:  zoom = automapZoom2; break;
			default: zoom = automapZoom0; break;
		}

		Vector3 scaleVec = new Vector3(zoom,zoom,zoom);
		automapContainerLH.transform.localScale = scaleVec;
		automapContainerRH.transform.localScale = scaleVec;
	}

	public void AutomapGoSide() {
		Const.sprint("Unable to connect to side map, try updating to version "
					 + " 1.10",Const.a.player1);
	}

	public void AutomapGoFull() {
		Utils.Activate(automapFull);
		inFullMap = true;
		MFDManager.a.AutomapGoFull();
	}

	public void CloseFullmap() {
		Utils.Deactivate(automapFull);
		inFullMap = false;
		MFDManager.a.CloseFullmap();
	}

	// Convert from Worldspace into Automapspace
	public Vector3 GetMapPos(Vector3 worldPos) {
		float mapWidth = (Const.a.mapWorldMaxW - Const.a.mapWorldMaxE);
		float mapHeight = (Const.a.mapWorldMaxN - Const.a.mapWorldMaxS);
		float ewOffset = (worldPos.z - Const.a.mapWorldMaxE);
		float nsOffset = (worldPos.x - Const.a.mapWorldMaxS);
		// private float camMaxAmount = 0.2548032f;
		// private float mapWorldMaxN = 85.83999f;
		// private float mapWorldMaxS = -78.00001f;
		// private float mapWorldMaxE = -70.44f;
		// private float mapWorldMaxW = 93.4f;

		// 34.16488, -45.08, 0.4855735
		// x = ((0.6384575295) * 1008f) + 8;
		// x = 651
		tempVec = new Vector3(0f,0f);
		tempVec.y = ((ewOffset/mapWidth) * 1008f) + Const.a.mapTileMinX;
		tempVec.x = ((nsOffset/mapHeight) * 1008f) + Const.a.mapTileMinY;
		tempVec.z = -0.03f; // Always moved to be behind the fog of war tiles.
		return tempVec;
	}

	public Image LinkOverlay(Vector3 worldPos, Transform prnt, PoolType type) {
		SecurityCameraRotate scr = null;
		if (prnt != null) scr = prnt.gameObject.GetComponent<SecurityCameraRotate>();
		if (scr != null) worldPos = scr.transform.position;
		GameObject overlay = Const.a.GetObjectFromPool(type);
		if (overlay != null) {
			Utils.Activate(overlay);
			Image over = overlay.GetComponent<Image>();
			if (over != null) {
				Utils.EnableImage(over);
				over.rectTransform.anchoredPosition = GetMapPos(worldPos);
				return over;
			} else {
				Debug.Log("BUG: No automap icon type " + type.ToString());
			}
		}

		return null;
	}

	public static string Save(GameObject go) {
		Automap amp = go.GetComponent<Automap>();
		int j = 0;
		StringBuilder s1 = new StringBuilder();
		s1.Clear();
		if (amp == null) {
			Debug.Log("Automap missing!  GameObject.name: " + go.name);
			for (j=0;j<(4096 * 13);j++) s1.Append("b");
			s1.Append("ut");
			return Utils.DTypeWordToSaveString(s1.ToString());
		}

		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(amp.automapExploredR[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(amp.automapExplored1[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(amp.automapExplored2[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(amp.automapExplored3[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(amp.automapExplored4[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(amp.automapExplored5[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(amp.automapExplored6[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(amp.automapExplored7[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(amp.automapExplored8[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(amp.automapExplored9[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(amp.automapExploredG1[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(amp.automapExploredG2[j])); s1.Append(Utils.splitChar); } // bool
		for (j=0;j<4096;j++) { s1.Append(Utils.BoolToString(amp.automapExploredG4[j])); s1.Append(Utils.splitChar); } // bool
		s1.Append(Utils.UintToString(amp.currentAutomapZoomLevel));
		s1.Append(Utils.splitChar);
		s1.Append(Utils.SaveRelativeTimeDifferential(amp.automapUpdateFinished));
		return s1.ToString();
	}

	public static int Load(GameObject go, ref string[] entries, int index) {
		Automap amp = go.GetComponent<Automap>();
		int j = 0;
		if (amp == null) {
			Debug.Log("Automap.Load failure, amp == null");
			return index + 2 + (4096 * 13);
		}

		if (index < 0) {
			Debug.Log("Automap.Load failure, index < 0");
			return index + 2 + (4096 * 13);
		}

		if (entries == null) {
			Debug.Log("Automap.Load failure, entries == null");
			return index + 2 + (4096 * 13);
		}

		for (j=0;j<4096;j++) { amp.automapExploredR[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { amp.automapExplored1[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { amp.automapExplored2[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { amp.automapExplored3[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { amp.automapExplored4[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { amp.automapExplored5[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { amp.automapExplored6[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { amp.automapExplored7[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { amp.automapExplored8[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { amp.automapExplored9[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { amp.automapExploredG1[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { amp.automapExploredG2[j] = entries[index].Equals("1"); index++; }
		for (j=0;j<4096;j++) { amp.automapExploredG4[j] = entries[index].Equals("1"); index++; }
		if (LevelManager.a != null) amp.SetAutomapExploredReference(LevelManager.a.currentLevel);
		else amp.SetAutomapExploredReference(1);

		amp.currentAutomapZoomLevel = Utils.GetIntFromString(entries[index]); index++;
		if (amp.currentAutomapZoomLevel < 0) amp.currentAutomapZoomLevel = 0;
		if (amp.currentAutomapZoomLevel > 2) amp.currentAutomapZoomLevel = 2;
		amp.AutomapZoomAdjust();
		amp.automapUpdateFinished = Utils.LoadRelativeTimeDifferential(entries[index]); index++;
		return index;
	}
}