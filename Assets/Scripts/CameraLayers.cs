using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLayers : MonoBehaviour {
	void Awake () {
		Camera cam = GetComponent<Camera>();
		cam.layerCullSpherical = true;
		float[] distPerLayer = new float[32];

		for (int i = 0; i < distPerLayer.Length; i++) {
			distPerLayer [i] = 0; // default to far plane distance
		}

		// Tweaked layer settings
		distPerLayer[15] = 1350f;
		cam.layerCullDistances = distPerLayer;
	}
}
