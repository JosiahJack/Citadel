using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.PostProcessing;

// Handles configuration parsing for user settings.
public class Config {
	public static void LoadConfig() {
		// Graphics Configurations
		Const.a.GraphicsResWidth = AssignConfigInt("Graphics","ResolutionWidth");
		Const.a.GraphicsResHeight = AssignConfigInt("Graphics","ResolutionHeight");
		Const.a.GraphicsFullscreen = AssignConfigBool("Graphics","Fullscreen");
		Const.a.GraphicsSSAO = AssignConfigBool("Graphics","SSAO");
		Const.a.GraphicsBloom = AssignConfigBool("Graphics","Bloom");
		Const.a.GraphicsFOV = AssignConfigInt("Graphics","FOV");
		Const.a.GraphicsAAMode = AssignConfigInt("Graphics","AA");
		Const.a.GraphicsShadowMode = AssignConfigInt("Graphics", "Shadows");
		Const.a.GraphicsSSRMode = AssignConfigInt("Graphics", "SSR");
		Const.a.GraphicsGamma = AssignConfigInt("Graphics","Gamma");

		// Audio Configurations
		Const.a.AudioSpeakerMode = AssignConfigInt("Audio","SpeakerMode");
		Const.a.AudioReverb = AssignConfigBool("Audio","Reverb");
		Const.a.AudioVolumeMaster = AssignConfigInt("Audio","VolumeMaster");
		Const.a.AudioVolumeMusic = AssignConfigInt("Audio","VolumeMusic");
		Const.a.AudioVolumeMessage = AssignConfigInt("Audio","VolumeMessage");
		Const.a.AudioVolumeEffects = AssignConfigInt("Audio","VolumeEffects");
		Const.a.AudioLanguage = AssignConfigInt("Audio","Language");  // defaults to 0 = english

		Const.a.MouseSensitivity = ((AssignConfigInt("Input","MouseSensitivity")/100f) * 2f) + 0.01f;;

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
		SetVolume();
		Screen.SetResolution(Const.a.GraphicsResWidth,Const.a.GraphicsResHeight,true);
		Screen.fullScreen = Const.a.GraphicsFullscreen;
	}

	public static void WriteConfig() {
		INIWorker.IniWriteValue("Graphics","ResolutionWidth",Const.a.GraphicsResWidth.ToString());
		INIWorker.IniWriteValue("Graphics","ResolutionHeight",Const.a.GraphicsResHeight.ToString());
		INIWorker.IniWriteValue("Graphics","Fullscreen",Utils.BoolToString(Const.a.GraphicsFullscreen));
		INIWorker.IniWriteValue("Graphics","SSAO",Utils.BoolToString(Const.a.GraphicsSSAO));
		INIWorker.IniWriteValue("Graphics","Bloom",Utils.BoolToString(Const.a.GraphicsBloom));
		INIWorker.IniWriteValue("Graphics","FOV",Const.a.GraphicsFOV.ToString());
		INIWorker.IniWriteValue("Graphics","AA",Const.a.GraphicsAAMode.ToString());
		INIWorker.IniWriteValue("Graphics","Shadows",Const.a.GraphicsShadowMode.ToString());
		INIWorker.IniWriteValue("Grpahics","SSR",Const.a.GraphicsSSRMode.ToString());
		INIWorker.IniWriteValue("Graphics","Gamma",Const.a.GraphicsGamma.ToString());
		INIWorker.IniWriteValue("Audio","SpeakerMode",Const.a.AudioSpeakerMode.ToString());
		INIWorker.IniWriteValue("Audio","Reverb",Utils.BoolToString(Const.a.AudioReverb));
		INIWorker.IniWriteValue("Audio","VolumeMaster",Const.a.AudioVolumeMaster.ToString());
		INIWorker.IniWriteValue("Audio","VolumeMusic",Const.a.AudioVolumeMusic.ToString());
		INIWorker.IniWriteValue("Audio","VolumeMessage",Const.a.AudioVolumeMessage.ToString());
		INIWorker.IniWriteValue("Audio","VolumeEffects",Const.a.AudioVolumeEffects.ToString());
		INIWorker.IniWriteValue("Audio","Language",Const.a.AudioLanguage.ToString());
		int ms = (int)(Const.a.MouseSensitivity/2f*100f);
		INIWorker.IniWriteValue("Input","MouseSensitivity",ms.ToString());
		for (int i=0;i<40;i++) {
			INIWorker.IniWriteValue("Input",Const.a.InputCodes[i],Const.a.InputValues[Const.a.InputCodeSettings[i]]);
		}
		INIWorker.IniWriteValue("Input","InvertLook",Utils.BoolToString(Const.a.InputInvertLook));
		INIWorker.IniWriteValue("Input","InvertCyberspaceLook",Utils.BoolToString(Const.a.InputInvertCyberspaceLook));
		INIWorker.IniWriteValue("Input","InvertInventoryCycling",Utils.BoolToString(Const.a.InputInvertInventoryCycling));
		INIWorker.IniWriteValue("Input","QuickItemPickup",Utils.BoolToString(Const.a.InputQuickItemPickup));
		INIWorker.IniWriteValue("Input","QuickReloadWeapons",Utils.BoolToString(Const.a.InputQuickReloadWeapons));
		SetBloom();
		SetSSAO();
		SetAA();
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


	public static void SetFXAA(AntialiasingModel.FxaaPreset preset) {
		AntialiasingModel.Settings amS = AntialiasingModel.Settings.defaultSettings;
		amS.fxaaSettings.preset = preset;
		amS.method = AntialiasingModel.Method.Fxaa;
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
				SetSSRPreset(ScreenSpaceReflectionModel.SSRResolution.High);
				break;
		}
	}

	public static void SetSSRPreset(ScreenSpaceReflectionModel.SSRResolution preset) {
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
		Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.screenSpaceReflection.enabled = true;
		Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.screenSpaceReflection.settings = ssr;
	}

	public static void SetSSAO() {
		Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile.ambientOcclusion.enabled = Const.a.GraphicsSSAO;
	}

	public static void SetBrightness() {
		float tempf = Const.a.GraphicsGamma;
		if (tempf < 1) tempf = 0;
		else tempf = tempf/100;
		tempf = (tempf * 8f) - 4f;
		PostProcessingProfile ppf = Const.a.player1CapsuleMainCameragGO.GetComponent<Camera>().GetComponent<PostProcessingBehaviour>().profile;
		ColorGradingModel.Settings cgms = ppf.colorGrading.settings;
		cgms.basic.postExposure = tempf;
		ppf.colorGrading.settings = cgms;
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