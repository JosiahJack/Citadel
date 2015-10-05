using UnityEngine;
using System.Collections;

public class EmailTitle : MonoBehaviour {
	public string[] emailInventoryTitle;
	public int[] emailInventoryIndices;
	[SerializeField] public string[] emailInvTitleSource;
	public static EmailTitle Instance;
	public AudioClip[] emailVoiceClips = null; // assign in the editor

	[SerializeField] public AudioSource SFX = null; // assign in the editor
	
	void Awake() {
		Instance = this;
		Instance.emailInventoryTitle = new string[]{"SHODAN-06.NOV.72","Rebecca-1","Dummy","","","",""};;
		Instance.emailInventoryIndices = new int[]{1,2,3,4,5,6,7};
		Instance.emailVoiceClips = emailVoiceClips;
	}
}
