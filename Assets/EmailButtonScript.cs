using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EmailButtonScript : MonoBehaviour {

	[SerializeField] private GameObject iconman;
	[SerializeField] private GameObject mailtitleman;
	[SerializeField] private int mailButtonIndex;

	[SerializeField] private AudioSource SFX = null; // assign in the editor
	[SerializeField] private AudioClip SFXClip = null; // assign in the editor
	[SerializeField] private CenterTabButtonsScript CenterTabButtonsManager = null; // assign in the editor

	private int invslot;

	void MailInvClick () {

		invslot = EmailTitle.Instance.emailInventoryIndices[mailButtonIndex];
		if (invslot < 0)
			return;

		//mailtitleman.GetComponent<MailTitleManager>().SetMailTitle(invslot); //Set Mail Title Text for MFD
		EmailCurrent.Instance.emailCurrent = mailButtonIndex;				//Set current mail to be played

			EmailTitle.Instance.SFX.clip = EmailTitle.Instance.emailVoiceClips[EmailCurrent.Instance.emailCurrent];
			EmailTitle.Instance.SFX.Play ();

		CenterTabButtonsManager.TabButtonClick (5);
	}

	[SerializeField] private Button MailButton = null; // assign in the editor
	void Start() {
		MailButton.onClick.AddListener(() => { MailInvClick();});
	}
}
