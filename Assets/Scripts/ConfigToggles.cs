using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigToggles : MonoBehaviour {
	// External references
	public ConfigToggleType ToggleType;

	// Internal references
	private Toggle self;

	void Start () { // Wait for Const.a. to initialize.
		AlignWithConfigFile();
	}

	void OnEnable() {
		AlignWithConfigFile();
	}

	public void AlignWithConfigFile() {
		if (self == null) self = GetComponent<Toggle>();
		switch (ToggleType) {
			case ConfigToggleType.Fullscreen: self.isOn = Const.a.GraphicsFullscreen; break;
			case ConfigToggleType.SSAO: self.isOn = Const.a.GraphicsSSAO; break;
			case ConfigToggleType.Bloom: self.isOn = Const.a.GraphicsBloom; break;
			case ConfigToggleType.Reverb: self.isOn = Const.a.AudioReverb; break;
			case ConfigToggleType.InvertLook: self.isOn = Const.a.InputInvertLook; break;
			case ConfigToggleType.InvertCyber: self.isOn = Const.a.InputInvertCyberspaceLook; break;
			case ConfigToggleType.InvertInventoryCycling: self.isOn = Const.a.InputInvertInventoryCycling; break;
			case ConfigToggleType.QuickPickup: self.isOn = Const.a.InputQuickItemPickup; break;
			case ConfigToggleType.QuickReload: self.isOn = Const.a.InputQuickReloadWeapons; break;
			case ConfigToggleType.Vsync: self.isOn = Const.a.GraphicsVSync; break;
			case ConfigToggleType.NoShootMode: self.isOn = Const.a.NoShootMode; break;
			case ConfigToggleType.DynamicMusic: self.isOn = Const.a.DynamicMusic; break;
			case ConfigToggleType.HeadBob: self.isOn = Const.a.HeadBob; break;
			case ConfigToggleType.Footsteps: self.isOn = Const.a.Footsteps; break;
		}
	}

	public void ToggleFullscreen () { Const.a.GraphicsFullscreen = self.isOn; Config.WriteConfig(); }
	public void ToggleSSAO () { Const.a.GraphicsSSAO = self.isOn; Config.WriteConfig(); }
	public void ToggleBloom () { Const.a.GraphicsBloom = self.isOn; Config.WriteConfig(); }
	public void ToggleReverb () {
		Const.a.AudioReverb = self.isOn;
		if (Const.a.AudioReverb) Const.a.ReverbOn();
		else Const.a.ReverbOff();

		Config.WriteConfig();
	}
	public void ToggleInvertLook () { Const.a.InputInvertLook = self.isOn; Config.WriteConfig(); }
	public void ToggleInvertCyberLook () { Const.a.InputInvertCyberspaceLook = self.isOn; Config.WriteConfig(); }
	public void ToggleInvertInventoryCycling () { Const.a.InputInvertInventoryCycling = self.isOn; Config.WriteConfig(); }
	public void ToggleQuickItemPickup () { Const.a.InputQuickItemPickup = self.isOn; Config.WriteConfig(); }
	public void ToggleQuickReloadWeapon () { Const.a.InputQuickReloadWeapons = self.isOn; Config.WriteConfig(); }
	public void ToggleVSync () { Const.a.GraphicsVSync = self.isOn; Config.WriteConfig(); }
	public void ToggleNoShootMode () { Const.a.NoShootMode = self.isOn; Config.WriteConfig(); }
	public void ToggleDynamicMusic () { Const.a.DynamicMusic = self.isOn; Config.WriteConfig(); Music.a.Stop(); }
	public void ToggleHeadBob () { Const.a.HeadBob = self.isOn; Config.WriteConfig(); }
	public void ToggleFootsteps () { Const.a.Footsteps = self.isOn; Config.WriteConfig(); }
}
