using UnityEngine;
using System.Collections;

public class TouchHurt : MonoBehaviour {
	public float damage = 1; // assign in the editor
	
	void  OnCollisionEnter ( Collision col  ){
		if (col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
		}
	}
}