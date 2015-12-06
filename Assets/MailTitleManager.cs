using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MailTitleManager : MonoBehaviour {
	[SerializeField] public string[] mailTitle;
	
	public void SetMailTitle (int index) {
		if (index >= 0)
			GetComponent<Text>().text = mailTitle[index];
	}
}