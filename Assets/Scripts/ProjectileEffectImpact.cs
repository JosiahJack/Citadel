using UnityEngine;
using System.Collections;

public class ProjectileEffectImpact : MonoBehaviour {
    public Const.PoolType impactType;
	public GameObject host;
    [HideInInspector]
    public DamageData dd;
    [SerializeField] public int hitCountBeforeRemoval = 1;
    private Vector3 tempVec;
    private int numHits;
    //private Rigidbody rbody;

    private void OnEnable() {
        numHits = 0; // reset when pulled from pool
        //rbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter (Collision other) {
        if (other.gameObject != host) {
            numHits++;
            if (numHits >= hitCountBeforeRemoval) {
                // get an impact effect
                GameObject impact = Const.a.GetObjectFromPool(impactType);
                // enable the impact effect
                if (impact != null) {
                    impact.transform.position = other.contacts[0].point;
                    impact.SetActive(true);
                }

                HealthManager hm = other.contacts[0].otherCollider.gameObject.GetComponent<HealthManager>();
                if (hm != null)
                    hm.TakeDamage(dd);
                gameObject.SetActive(false); // disable the projectile
            }// else {
               // Vector3 dir = other.contacts[0].normal;
                //rbody.AddForce(dir * rbody.velocity.magnitude * 0.75f, ForceMode.Impulse); //buggy, added to physics material instead
            //}
        }
	}
}
