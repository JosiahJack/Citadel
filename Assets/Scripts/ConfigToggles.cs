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
		}
	}

	public void ToggleFullscreen () { Const.a.GraphicsFullscreen = self.isOn; Config.WriteConfig(); }
	public void ToggleSSAO () { Const.a.GraphicsSSAO = self.isOn; Config.WriteConfig(); }
	public void ToggleBloom () { Const.a.GraphicsBloom = self.isOn; Config.WriteConfig(); }
	public void ToggleReverb () { Const.a.AudioReverb = self.isOn; Config.WriteConfig(); }
	public void ToggleInvertLook () { Const.a.InputInvertLook = self.isOn; Config.WriteConfig(); }
	public void ToggleInvertCyberLook () { Const.a.InputInvertCyberspaceLook = self.isOn; Config.WriteConfig(); }
	public void ToggleInvertInventoryCycling () { Const.a.InputInvertInventoryCycling = self.isOn; Config.WriteConfig(); }
	public void ToggleQuickItemPickup () { Const.a.InputQuickItemPickup = self.isOn; Config.WriteConfig(); }
	public void ToggleQuickReloadWeapon () { Const.a.InputQuickReloadWeapons = self.isOn; Config.WriteConfig(); }
	public void ToggleVSync () { Const.a.GraphicsVSync = self.isOn; Config.WriteConfig(); }
}
