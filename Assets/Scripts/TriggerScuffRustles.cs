using UnityEngine;

public class TriggerScuffRustles : MonoBehaviour {
    public AudioSource SFX;
    public int clip;
    private float finished;

    void OnTriggerEnter (Collider col) {
        if (clip < 0 || clip >= Const.a.sounds.Length) return;
        if (!(col.gameObject.CompareTag("Player"))) return;
        if (finished > Time.time) return;

		finished = Time.time + Random.Range(3f,5f);
        SFX.PlayOneShot(Const.a.sounds[clip],Random.Range(0.5f,0.75f));
	}
}
