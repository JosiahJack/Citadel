using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLayers : MonoBehaviour {
	public float defaultDistance = 50f;
	public float skyDistance = 1100f;

	void Awake () {
		Camera cam = GetComponent<Camera>();
		cam.layerCullSpherical = true;
		float[] distPerLayer = new float[32];







		distPerLayer[8] = defaultDistance;
		distPerLayer[9] = defaultDistance;
		distPerLayer[10] = defaultDistance;
		distPerLayer[11] = defaultDistance;
		distPerLayer[12] = defaultDistance;
		distPerLayer[13] = defaultDistance;

		distPerLayer[15] = skyDistance;
		cam.layerCullDistances = distPerLayer;
	}
}
