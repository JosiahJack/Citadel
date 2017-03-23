using UnityEngine;
using System.Collections;

public class GrenadeFrag : MonoBehaviour {
	public float nearforce;
	public float nearradius;
	public GameObject explosion;
	//private AudioSource SFXSource;
	//public AudioClip ExplosionSFX;

	/*void OnMouseDown () {
		Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
		foreach(Collider c in colliders) {
			if (c.GetComponent<Rigidbody>() == null) continue;

			c.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, radius, 0.5f, ForceMode.Impulse);
		}

		Destroy(this.gameObject);
	}*/

	void OnMouseDown () {
		GetComponent<ExplosionForce>().ExplodeOuter(transform.position);
		GetComponent<ExplosionForce>().ExplodeInner(transform.position, nearforce, nearradius);
        Instantiate(explosion, transform.position, Quaternion.identity);
        //GameObject expinst = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
		//SFXSource = expinst.GetComponent<AudioSource>();
		//SFXSource.PlayOneShot(ExplosionSFX);
		Destroy(this.gameObject);
	}
}
