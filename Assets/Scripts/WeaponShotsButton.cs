using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponShotsButton : MonoBehaviour {
	public GameObject iconman;
	public GameObject ammoiconman;
	public GameObject weptextman;
	public int WepButtonIndex;
	public AudioSource SFX = null; // assign in the editor
	public AudioClip SFXClip = null; // assign in the editor
	private int invslot;
	private float ix;
	private float iy;
	private Vector3 transMouse;
	private Matrix4x4 m;
	private Matrix4x4 inv;
	[SerializeField] private Button WepButton = null; // assign in the editor

	public void PtrEnter () {
		GUIState.a.isBlocking = true;
	}

	public void PtrExit () {
		GUIState.a.isBlocking = false;
	}

	void WeaponInvClick () {
		WepButton.GetComponent<WeaponButton>().WeaponInvClick();
	}

	void Start() {
		WepButton.onClick.AddListener(() => { WeaponInvClick();});
	}
}
