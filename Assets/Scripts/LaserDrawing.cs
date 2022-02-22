using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDrawing : MonoBehaviour {
	[HideInInspector] public GameObject followStarter;
	[HideInInspector] public Vector3 startPoint;
	[HideInInspector] public Vector3 endPoint;
    public float lineLife = 0.15f;
	[HideInInspector] public LineRenderer line;

	void Awake () {
		line = GetComponent<LineRenderer>();
		line.startWidth = 0.2f;
		line.endWidth = 0.2f;
        line.enabled = true;
	}

	void Update() {
		line.SetPosition(0,startPoint);
		line.SetPosition(1,endPoint);
	}

    void OnEnable() {
        StartCoroutine(DelayedDestroy());
        line.enabled = true;
    }

	IEnumerator DelayedDestroy () {
		yield return new WaitForSeconds (lineLife);
		Destroy(this.gameObject);
	}
}
