using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFarPlaneOcclussion : MonoBehaviour {
	public bool makeRaysVisible = false;
	public int defaultFarPlane = 100;
	public int minDistance = 20;
	public int maxDistance = 200;
	public int farPlaneBuffer = 10;
	public int rateOfReceding = 16;
	public Camera cameraAffected;
	public int distance;

	private float[] rayGridY = new float[] {1.00f, 0.60f, 0.59f, 0.58f, 0.57f, 0.56f, 0.55f, 0.54f, 0.53f, 0.52f, 0.51f, 0.50f, 0.49f, 0.48f, 0.47f, 0.46f, 0.45f, 0.44f, 0.43f, 0.42f, 0.41f, 0.40f, 0.00f};
	private float[] rayGridX = new float[] {0.00f, 0.01f, 0.06f, 0.11f, 0.16f, 0.21f, 0.26f, 0.31f, 0.36f, 0.41f, 0.43f, 0.45f, 0.47f, 0.48f, 0.49f, 0.50f, 0.51f, 0.52f, 0.53f, 0.55f, 0.57f, 0.59f, 0.64f, 0.69f, 0.74f, 0.79f, 0.84f, 0.89f, 0.94f, 0.99f, 1.00f};
	void Start () {
		cameraAffected.farClipPlane = defaultFarPlane;
		StartCoroutine (AdjustFarPlane());
	}

	IEnumerator AdjustFarPlane () {
		while (true) {
			int farPlane = (int) cameraAffected.farClipPlane + farPlaneBuffer;
			distance = minDistance;
			bool ExtendFarPlane = false;

			foreach (float y in rayGridY) {
				foreach (float x in rayGridX) {
					int tempDist = CastOcclusionRay (x, y);
					if (tempDist >= farPlane) {
						distance = tempDist;
						ExtendFarPlane = true;
						goto SHIFT_FAR_PLANE;
					}
				}

				yield return 0;
			}

			SHIFT_FAR_PLANE:
			// Far plane should extend instantly, but recede slowly.
			if (ExtendFarPlane == false) {
				cameraAffected.farClipPlane -= rateOfReceding;
				if (cameraAffected.farClipPlane < minDistance) cameraAffected.farClipPlane = minDistance;
			} else {
				cameraAffected.farClipPlane = distance;
			}
		}
	}

	int CastOcclusionRay (float graphX, float graphY) {
		Ray ray = cameraAffected.ViewportPointToRay (new Vector3 (graphX, graphY, 0));

		if (makeRaysVisible == true) Debug.DrawRay (ray.origin, ray.direction*20, Color.red);

		RaycastHit hit;
		if (Physics.Raycast (ray, out hit) && hit.distance < maxDistance) {
			return (int) hit.distance + farPlaneBuffer;
		} else {
			return (int) maxDistance;
		}
	}
}
