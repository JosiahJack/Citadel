using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigToggles : MonoBehaviour {
	private Toggle self;
	public enum ConfigToggleType{Fullscreen,SSAO,Bloom,Reverb,Subtitles,InvertLook,InvertCyber,InvertInventoryCycling,QuickPickup,QuickReload};
	public ConfigToggleType ToggleType;

	void Start () {
		self = GetComponent<Toggle>();
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
		}
	}

	public void ToggleFullscreen () { Const.a.GraphicsFullscreen = self.isOn; Const.a.WriteConfig(); }
	public void ToggleSSAO () { Const.a.GraphicsSSAO = self.isOn; Const.a.WriteConfig(); }
	public void ToggleBloom () { Const.a.GraphicsBloom = self.isOn; Const.a.WriteConfig(); }
	public void ToggleReverb () { Const.a.AudioReverb = self.isOn; Const.a.WriteConfig(); }
	public void ToggleInvertLook () { Const.a.InputInvertLook = self.isOn; Const.a.WriteConfig(); }
	public void ToggleInvertCyberLook () { Const.a.InputInvertCyberspaceLook = self.isOn; Const.a.WriteConfig(); }
	public void ToggleInvertInventoryCycling () { Const.a.InputInvertInventoryCycling = self.isOn; Const.a.WriteConfig(); }
	public void ToggleQuickItemPickup () { Const.a.InputQuickItemPickup = self.isOn; Const.a.WriteConfig(); }
	public void ToggleQuickReloadWeapon () { Const.a.InputQuickReloadWeapons = self.isOn; Const.a.WriteConfig(); }
}
