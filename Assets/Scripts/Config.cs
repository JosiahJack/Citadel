using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.PostProcessing;

// Handles configuration parsing for user settings.
public class Config {
	public static void LoadConfig() {
		if (Application.platform == RuntimePlatform.Android) {
			// Android Config (no read/write permissions.  Stupid security.
			// ----------------------------------------------------------------

	        // Graphics Configurations
    		Const.a.GraphicsResWidth = Screen.width;
    		Const.a.GraphicsResHeight = Screen.height;
    		Const.a.GraphicsFullscreen = true;
    		Const.a.GraphicsSSAO = false;
    		Const.a.GraphicsBloom = false;
			Const.a.GraphicsSEGI = false;
    		Const.a.GraphicsFOV = 65;
    		Const.a.GraphicsAAMode = 1;
    		Const.a.GraphicsShadowMode = 0;
    		Const.a.GraphicsSSRMode = 1;
    		Const.a.GraphicsGamma = 50;
			Const.a.GraphicsModelDetail = 0; // No detail, mobile
    		Const.a.GraphicsVSync = false;
    
    		// Audio Configurations
    		Const.a.AudioSpeakerMode = 1;
    		Const.a.AudioReverb = true;
    		Const.a.AudioVolumeMaster = 100;
    		Const.a.AudioVolumeMusic = 20;
    		Const.a.AudioVolumeMessage = 100;
    		Const.a.AudioVolumeEffects = 100;
    		Const.a.AudioLanguage = 0;
    		Const.a.DynamicMusic = true;
			Const.a.Footsteps = true;

			// Input
    		Const.a.MouseSensitivity = 20;
     		Const.a.InputInvertLook = false;
    		Const.a.InputInvertCyberspaceLook = false;
    		Const.a.InputInvertInventoryCycling = false;
    		Const.a.InputQuickItemPickup = true;
    		Const.a.InputQuickReloadWeapons = true;
    		// NoShootMode is irrelevant on Android due to touch widgets.
			Const.a.HeadBob = true;

    		// Apply settings effects
    		SetVolume();
    		Const.sprint(Const.a.stringTable[1016] // "Setting screen resolution to "
    				     + Const.a.GraphicsResWidth.ToString()
    				     + ", " + Const.a.GraphicsResHeight.ToString()
    				     + ", " + Const.a.stringTable[1017] + ": "
    				     + Const.a.GraphicsFullscreen.ToString());
    		SetShadows();
			SetModelDetail();
    		SetBloom();
			SetSEGI();
    		SetSSR();
    		SetBrightness();
    		SetSSAO();
    		SetFOV();
    		SetAA();
    		SetVSync();
	        return;
	    }

	    // Normal Windows/Linux/Mac Config
	    // --------------------------------------------------------------------
	    
		// The currently used config is always Config.ini.
		string basePath = Utils.GetAppropriateDataPath();
		Utils.ConfirmExistsMakeIfNot(basePath,"Config.ini");

		// Graphics Configurations
		Const.a.GraphicsResWidth = AssignConfigInt("Graphics","ResolutionWidth");
		Const.a.GraphicsResHeight = AssignConfigInt("Graphics","ResolutionHeight");
		Const.a.GraphicsFullscreen = AssignConfigBool("Graphics","Fullscreen");
		Const.a.GraphicsSSAO = AssignConfigBool("Graphics","SSAO");
		Const.a.GraphicsBloom = AssignConfigBool("Graphics","Bloom");
		Const.a.GraphicsSEGI = AssignConfigBool("Graphics","SEGI");
		Const.a.GraphicsFOV = AssignConfigInt("Graphics","FOV");
		Const.a.GraphicsAAMode = AssignConfigInt("Graphics","AA");
		Const.a.GraphicsShadowMode = AssignConfigInt("Graphics", "Shadows");
		Const.a.GraphicsSSRMode = AssignConfigInt("Graphics", "SSR");
		Const.a.GraphicsGamma = AssignConfigInt("Graphics","Gamma");
		Const.a.GraphicsModelDetail = AssignConfigInt("Graphics","ModelDetail");
		Const.a.GraphicsVSync = AssignConfigBool("Graphics","VSync");

		// Audio Configurations
		Const.a.AudioSpeakerMode = AssignConfigInt("Audio","SpeakerMode");
		Const.a.AudioReverb = AssignConfigBool("Audio","Reverb");
		Const.a.AudioVolumeMaster = AssignConfigInt("Audio","VolumeMaster");
		Const.a.AudioVolumeMusic = AssignConfigInt("Audio","VolumeMusic");
		Const.a.AudioVolumeMessage = AssignConfigInt("Audio","VolumeMessage");
		Const.a.AudioVolumeEffects = AssignConfigInt("Audio","VolumeEffects");
		Const.a.AudioLanguage = AssignConfigInt("Audio","Language");  // defaults to 0 = english
		Const.a.DynamicMusic = AssignConfigBool("Audio","DynamicMusic");
		Const.a.Footsteps = AssignConfigBool("Audio","Footsteps");
		Const.a.HeadBob = AssignConfigBool("Input","HeadBob");

		Const.a.MouseSensitivity = ((AssignConfigInt("Input","MouseSensitivity")/100f) * 2f) + 0.01f;

		string inputCapture;
		// Input Configurations
		for (int i=0;i<40;i++) {
			inputCapture = INIWorker.IniReadValue("Input",Const.a.InputCodes[i]);
			for (int j=0;j<159;j++) {
				if (Const.a.InputValues[j] == inputCapture) Const.a.InputCodeSettings[i] = j;
			}
		}
		Const.a.InputInvertLook = AssignConfigBool("Input","InvertLook");
		Const.a.InputInvertCyberspaceLook = AssignConfigBool("Input","InvertCyberspaceLook");
		Const.a.InputInvertInventoryCycling = AssignConfigBool("Input","InvertInventoryCycling");
		Const.a.InputQuickItemPickup = AssignConfigBool("Input","QuickItemPickup");
		Const.a.InputQuickReloadWeapons = AssignConfigBool("Input","QuickReloadWeapons");
		Const.a.NoShootMode = AssignConfigBool("Input","NoShootMode");
		SetVolume();
		Const.sprint("Setting screen resolution to "
				     + Const.a.GraphicsResWidth.ToString()
				     + ", " + Const.a.GraphicsResHeight.ToString()
				     + ", Fullscreen: "
				     + Const.a.GraphicsFullscreen.ToString());
		Screen.SetResolution(Const.a.GraphicsResWidth,Const.a.GraphicsResHeight,true);
		Screen.fullScreen = Const.a.GraphicsFullscreen;
		SetShadows();
		SetModelDetail();
		SetBloom();
		SetSEGI();
		SetSSR();
		SetBrightness();
		SetSSAO();
		SetFOV();
		SetAA();
		SetVSync();
		SetLanguage();
		SetAudioMode();
	}

	public static void WriteConfig() {
		INIWorker.IniWriteValue("Graphics","ResolutionWidth",Const.a.GraphicsResWidth.ToString());
		INIWorker.IniWriteValue("Graphics","ResolutionHeight",Const.a.GraphicsResHeight.ToString());
		INIWorker.IniWriteValue("Graphics","Fullscreen",Utils.BoolToStringConfig(Const.a.GraphicsFullscreen));
		INIWorker.IniWriteValue("Graphics","SSAO",Utils.BoolToStringConfig(Const.a.GraphicsSSAO));
		INIWorker.IniWriteValue("Graphics","Bloom",Utils.BoolToStringConfig(Const.a.GraphicsBloom));
		INIWorker.IniWriteValue("Graphics","SEGI",Utils.BoolToStringConfig(Const.a.GraphicsSEGI));
		INIWorker.IniWriteValue("Graphics","FOV",Const.a.GraphicsFOV.ToString());
		INIWorker.IniWriteValue("Graphics","AA",Const.a.GraphicsAAMode.ToString());
		INIWorker.IniWriteValue("Graphics","Shadows",Const.a.GraphicsShadowMode.ToString());
		INIWorker.IniWriteValue("Graphics","SSR",Const.a.GraphicsSSRMode.ToString());
		INIWorker.IniWriteValue("Graphics","Gamma",Const.a.GraphicsGamma.ToString());
		INIWorker.IniWriteValue("Graphics","ModelDetail",Const.a.GraphicsModelDetail.ToString());
		INIWorker.IniWriteValue("Graphics","VSync",Utils.BoolToStringConfig(Const.a.GraphicsVSync));
		INIWorker.IniWriteValue("Audio","SpeakerMode",Const.a.AudioSpeakerMode.ToString());
		INIWorker.IniWriteValue("Audio","Reverb",Utils.BoolToStringConfig(Const.a.AudioReverb));
		INIWorker.IniWriteValue("Audio","VolumeMaster",Const.a.AudioVolumeMaster.ToString());
		INIWorker.IniWriteValue("Audio","VolumeMusic",Const.a.AudioVolumeMusic.ToString());
		INIWorker.IniWriteValue("Audio","VolumeMessage",Const.a.AudioVolumeMessage.ToString());
		INIWorker.IniWriteValue("Audio","VolumeEffects",Const.a.AudioVolumeEffects.ToString());
		INIWorker.IniWriteValue("Audio","Language",Const.a.AudioLanguage.ToString());
		INIWorker.IniWriteValue("Audio","DynamicMusic",Utils.BoolToStringConfig(Const.a.DynamicMusic));
		INIWorker.IniWriteValue("Audio","Footsteps",Utils.BoolToStringConfig(Const.a.Footsteps));

		int ms = (int)(Const.a.MouseSensitivity/2f*100f);
		INIWorker.IniWriteValue("Input","MouseSensitivity",ms.ToString());
		for (int i=0;i<40;i++) {
			INIWorker.IniWriteValue("Input",Const.a.InputCodes[i],Const.a.InputValues[Const.a.InputCodeSettings[i]]);
		}
		INIWorker.IniWriteValue("Input","InvertLook",Utils.BoolToStringConfig(Const.a.InputInvertLook));
		INIWorker.IniWriteValue("Input","InvertCyberspaceLook",Utils.BoolToStringConfig(Const.a.InputInvertCyberspaceLook));
		INIWorker.IniWriteValue("Input","InvertInventoryCycling",Utils.BoolToStringConfig(Const.a.InputInvertInventoryCycling));
		INIWorker.IniWriteValue("Input","QuickItemPickup",Utils.BoolToStringConfig(Const.a.InputQuickItemPickup));
		INIWorker.IniWriteValue("Input","QuickReloadWeapons",Utils.BoolToStringConfig(Const.a.InputQuickReloadWeapons));
		INIWorker.IniWriteValue("Input","NoShootMode",Utils.BoolToStringConfig(Const.a.NoShootMode));
		INIWorker.IniWriteValue("Input","HeadBob",Utils.BoolToStringConfig(Const.a.HeadBob));

		SetBloom();
		SetSEGI();
		SetSSAO();
		SetFOV();
		SetAA();
		SetVSync();
		if (MainMenuHandler.a != null) MainMenuHandler.a.RenderConfigView();
		SaveConfigToPlayerPrefs();
		if (Const.a.difficultyMission < 3) {
			MissionTimer.a.text.text = System.String.Empty;
			MissionTimer.a.timerTypeText.text = System.String.Empty;
			MFDManager.a.overallMissionTimerT.SetActive(false);
			MFDManager.a.overallMissionTimer.SetActive(false);
		} else {
			MFDManager.a.overallMissionTimerT.SetActive(true);
			MFDManager.a.overallMissionTimer.SetActive(true);
		}
	}

	public static void SaveConfigToPlayerPrefs() {
		#if UNITY_EDITOR
			// Don't bother with PlayerPrefs from Editor.
		#else
			// Force to potato PlayerPref settings for faster startup, and to prevent users with potato systems from being able to run initially.
			PlayerPrefs.SetInt("Screenmanager Resolution Width", Const.a.GraphicsResWidth);
			PlayerPrefs.SetInt("Screenmanager Resolution Height", Const.a.GraphicsResHeight);
			PlayerPrefs.SetInt("Screenmanager Is Fullscreen", Const.a.GraphicsFullscreen ? 1 : 0); // 0 = windowed
			PlayerPrefs.Save();
		#endif
	}
	
	public static void SetVolume() {
		if (MainMenuHandler.a.dataFound) {
			AudioListener.volume = (Const.a.AudioVolumeMaster/100f);
			Const.a.mainmenuMusic.volume = (Const.a.AudioVolumeMusic/100f);
			if (Music.a != null) {
				if (Music.a.SFXMain != null) Music.a.SFXMain.volume = (Const.a.AudioVolumeMusic/100f);
				if (Music.a.SFXOverlay != null) Music.a.SFXOverlay.volume = (Const.a.AudioVolumeMusic/100f);
			}
		} else {
			AudioListener.volume = 0f;
		}
	}

	public static void SetFOV() {
		Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().fieldOfView = Const.a.GraphicsFOV;
	}

	public static void SetBloom() {
		Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.bloom.enabled = Const.a.GraphicsBloom;
	}
	
	public static void SetSEGI() {
		Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<SEGI>().enabled = Const.a.GraphicsSEGI;
		SetBrightness();
	}

	public static void SetVSync() {
		if (Const.a.GraphicsVSync) {
			Application.targetFrameRate = Const.a.TARGET_FPS * 2;
			QualitySettings.vSyncCount = 1;
		} else {
			Application.targetFrameRate = Const.a.TARGET_FPS;
			QualitySettings.vSyncCount = 0;
		}
		
		Debug.Log("Set VSYNC to " + Const.a.GraphicsVSync.ToString() + ", target framerate is " + Application.targetFrameRate.ToString());
	}

	public static void SetFXAA(AntialiasingModel.FxaaPreset preset) {
		AntialiasingModel.Settings amS = AntialiasingModel.Settings.defaultSettings;
		amS.fxaaSettings.preset = preset;
		Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = true;
		Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings = amS;
	}

	// None
	// ExtremePerformance,
	// Performance,
	// Default,
	// Quality,
	// ExtremeQuality
	public static void SetAA() {
		if (Const.a.GraphicsAAMode < 0) Const.a.GraphicsAAMode = 0;
		if (Const.a.GraphicsAAMode > 5) Const.a.GraphicsAAMode = 5;
		switch (Const.a.GraphicsAAMode) {
			case 0: // No Antialiasing, turn off the profile's antialiasing entirely.
				Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = false;
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
		//		Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.enabled = true;
		//		AntialiasingModel.Settings amS = Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings;
		//		amS.method = AntialiasingModel.Method.Taa;
		//		Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.antialiasing.settings = amS;
		//		break;
		}
	}

	// No Shadows
	// Hard Shadows,
	// Soft Shadows
	public static void SetShadows() {
		if (Const.a.GraphicsShadowMode > 2) Const.a.GraphicsShadowMode = 2;
		if (Const.a.GraphicsShadowMode < 0) Const.a.GraphicsShadowMode = 0;
		switch (Const.a.GraphicsShadowMode) {
			case 0: // No Shadows
				QualitySettings.shadows = ShadowQuality.Disable;
				QualitySettings.shadowDistance = 1.0f;
				break;
			case 1: // Hard Shadows
				QualitySettings.shadows = ShadowQuality.HardOnly;
				QualitySettings.shadowResolution = ShadowResolution.Low;
				QualitySettings.shadowDistance = 50.0f; // Check layers in LevelManager shadCullArray
				break;
			case 2: // Soft Shadows
				QualitySettings.shadows = ShadowQuality.All;
				QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
				QualitySettings.shadowDistance = 50.0f; // Check layers in LevelManager shadCullArray
				break;
		}
	}
	
	// No Detail (aka flat cards, ala original)
	// High Detail (Citadel intended graphics)
	public static void SetModelDetail() {
		if (Const.a.GraphicsModelDetail == 0) {
			DynamicCulling.a.lodSqrDist = 0f;
		} else {
			DynamicCulling.a.lodSqrDist = DynamicCulling.lodSqrDistDefault;
		}
		
		DynamicCulling.a.forceRecull = true; // Recull to reapply meshes.
	}
	
	// No SSR
	// Low SSR,
	// High SSR
	public static void SetSSR() {
		if (Const.a.GraphicsSSRMode > 2) Const.a.GraphicsSSRMode = 2;
		if (Const.a.GraphicsSSRMode < 0) Const.a.GraphicsSSRMode = 0;
		switch (Const.a.GraphicsSSRMode) {
			case 0: // No SSR
				Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.screenSpaceReflection.enabled = false;
				break;
			case 1: // Low SSR
				SetSSRPreset(ScreenSpaceReflectionModel.SSRResolution.Low);
				break;
			case 2: // High SSR
				SetSSRPreset(ScreenSpaceReflectionModel.SSRResolution.Low); // Also low as it still looks good and prevents spikes every 0.25secs
				break;
		}
	}

	public static void SetSSRPreset(ScreenSpaceReflectionModel.SSRResolution preset) {
		ScreenSpaceReflectionModel.Settings ssr = ScreenSpaceReflectionModel.Settings.defaultSettings;
		ssr.reflection.reflectionQuality = preset;
		ssr.reflection.blendType = ScreenSpaceReflectionModel.SSRReflectionBlendType.Additive;
		ssr.reflection.maxDistance = 100f;
		if (preset == ScreenSpaceReflectionModel.SSRResolution.High) {
			ssr.reflection.iterationCount = 1024;
			ssr.reflection.stepSize = 1;
		} else {
			ssr.reflection.iterationCount = 512;
			ssr.reflection.stepSize = 6;
		}

		ssr.intensity.reflectionMultiplier = 0.25f;
		ssr.reflection.widthModifier = 0.5f;
		ssr.reflection.reflectionBlur = 1.0f;
		ssr.reflection.reflectBackfaces = false;
		ssr.intensity.fadeDistance = 100f;
		ssr.intensity.fresnelFade = 1.0f;
		ssr.intensity.fresnelFadePower = 1.0f;
		ssr.screenEdgeMask.intensity = 0.03f;
		Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.screenSpaceReflection.enabled = true;
		Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.screenSpaceReflection.settings = ssr;
	}

	public static void SetSSAO() {
// 		Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.ambientOcclusion.enabled = Const.a.GraphicsSSAO;
		Const.a.player1CapsuleMainCameragGO.GetComponent<UnityStandardAssets.ImageEffects.ScreenSpaceAmbientOcclusion>().enabled = Const.a.GraphicsSSAO;
	}

	public static void SetBrightness() {
		float tempf = Const.a.GraphicsGamma;
		if (tempf < 1) tempf = 0;
		else tempf = tempf/100;
		tempf = (tempf * 8f) - 4f;
		if (Const.a.GraphicsSEGI) tempf -= Const.segiReducedExposure;
		tempf += 2.2f;
		PostProcessingProfile ppf = Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile;
		ColorGradingModel.Settings cgms = ppf.colorGrading.settings;
		cgms.basic.postExposure = tempf;
		ppf.colorGrading.settings = cgms;
	}

	public static void SetLanguage() {
		Const.a.LoadTextForLanguage(Const.a.AudioLanguage);
		Const.a.LoadAudioLogMetaData();
		foreach(TextLocalization txtLoc in Const.a.TextLocalizationRegister) {
			txtLoc.UpdateText();
		}
		
		MainMenuHandler.a.aaaApply.SetOptionsText();
		MainMenuHandler.a.shadApply.SetOptionsText();
		MainMenuHandler.a.ssrApply.SetOptionsText();
		MainMenuHandler.a.audModeApply.SetOptionsText();
		MainMenuHandler.a.mdlDetApply.SetOptionsText();
	}
	
	private static readonly string AUDIO_MODE_KEY = "AudioSpeakerMode";
	private static int lastAudioMode = -1;
	
	public static void SetAudioMode() {
		if (lastAudioMode == -1) lastAudioMode = PlayerPrefs.GetInt(AUDIO_MODE_KEY, 1); // Stereo as default

		AudioConfiguration audconf = AudioSettings.GetConfiguration();
		AudioSpeakerMode targetMode = AudioSpeakerMode.Stereo;
		switch(Const.a.AudioSpeakerMode) {
			case 0: targetMode = AudioSpeakerMode.Mono; break;
			case 1: targetMode = AudioSpeakerMode.Stereo; break;
			case 2: targetMode = AudioSpeakerMode.Quad; break;
			case 3: targetMode = AudioSpeakerMode.Surround; break;
			case 4: targetMode = AudioSpeakerMode.Mode5point1; break;
			case 5: targetMode = AudioSpeakerMode.Mode7point1; break;
			case 6: targetMode = AudioSpeakerMode.Prologic; break;
		}
		
		if (audconf.speakerMode != targetMode) {
			audconf.speakerMode = targetMode;
			AudioSettings.Reset(audconf);
		}

		if (lastAudioMode != Const.a.AudioSpeakerMode) {
			PlayerPrefs.SetInt(AUDIO_MODE_KEY, Const.a.AudioSpeakerMode);
			PlayerPrefs.Save(); // Ensure it’s written to disk
		}
	}

	private static int AssignConfigInt(string section, string keyname) {
		int inputInt = -1;
		string inputCapture = System.String.Empty;
		inputCapture = INIWorker.IniReadValue(section,keyname);
		if (inputCapture == null) inputCapture = "NULL";
		bool parsed = Int32.TryParse(inputCapture, NumberStyles.Integer, Utils.en_US_Culture, out inputInt);
		if (parsed) return inputInt; else Const.sprint("Warning: Could not parse config key " + keyname + " as integer: " + inputCapture);
		return 0;
	}

	private static bool AssignConfigBool(string section, string keyname) {
		int inputInt = -1;
		string inputCapture = System.String.Empty;
		inputCapture = INIWorker.IniReadValue(section,keyname);
		if (inputCapture == null) inputCapture = "NULL";
		bool parsed = Int32.TryParse(inputCapture, NumberStyles.Integer, Utils.en_US_Culture, out inputInt);
		if (parsed) {
			if (inputInt > 0) return true; else return false;
		} else Const.sprint("Warning: Could not parse config key " + keyname + " as bool: " + inputCapture);
		return false;
	}
}
