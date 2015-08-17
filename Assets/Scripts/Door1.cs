using UnityEngine;
using System.Collections;

public class Door1 : MonoBehaviour {

	Animator animator;
	bool doorOpen;
	
	void Start () {
		doorOpen = false;
		animator = GetComponent<Animator>();
	}

	void Use () {
		print("Used!");
	}
}
