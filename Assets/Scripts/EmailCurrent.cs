using UnityEngine;
using System.Collections;

public class EmailCurrent : MonoBehaviour {
	[SerializeField] public int emailCurrent = new int();
	[SerializeField] public int emailIndex = new int();
	public static EmailCurrent Instance;
	
	void Awake() {
		Instance = this;
		Instance.emailCurrent = 0; // Current slot in the email inventory (7 slots)
		Instance.emailIndex = 0; // Current index to the email look-up tables
	}
}