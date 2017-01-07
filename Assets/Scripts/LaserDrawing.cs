using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDrawing : MonoBehaviour {
	public GameObject followStarter;
	public Vector3 startPoint;
	public Vector3 endPoint;

	LineRenderer line;

	void Start () {
		line = GetComponent<LineRenderer>();
		line.startWidth = 0.2f;
		line.endWidth = 0.2f;
	}

	void Update () {
		//if (followStarter != null) {
		//	endPoint = followStarter.transform.position;

		//}
		line.SetPosition(0,startPoint);
		line.SetPosition(1,endPoint);
	}
}
