using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HardwareButtonScript : MonoBehaviour {
	//[SerializeField] private GameObject iconman;
	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
	[SerializeField] public GameObject centerTabManager;
	//private int invslot;
	//private float ix;
	//private float iy;
	//private Vector3 transMouse;
	//private Matrix4x4 m;
	//private Matrix4x4 inv;
	//private bool alternateAmmo = false;

	public void PtrEnter () {
		GUIState.a.isBlocking = true;
	}

	public void PtrExit () {
		GUIState.a.isBlocking = false;
	}

	void HardwareClick () {
		SFX.PlayOneShot(SFXClip);
		centerTabManager.GetComponent<CenterTabButtonsScript>().TabButtonClickSilent(4);
	}

	[SerializeField] private Button HwButton = null; // assign in the editor
	void Start() {
		HwButton.onClick.AddListener(() => { HardwareClick();});
	}
}
