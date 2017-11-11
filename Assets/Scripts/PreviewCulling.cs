using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[ExecuteInEditMode]
public class PreviewCulling : MonoBehaviour {

	#if UNITY_EDITOR

	static Camera sceneReferenceCamera;

	Vector3 pos;
	Quaternion rot;
	float near;
	float far;

	new Camera camera;

	bool IsSceneViewCamera(Camera cam)
	{
		Camera[] sceneCameras = SceneView.GetAllSceneCameras();
		for (int i = 0; i < sceneCameras.Length; i++){
			if (cam == sceneCameras[i]) { return true; }
		}
		return false;
	}

	void OnEnable() {
		camera = GetComponent<Camera>();
		if (camera && !IsSceneViewCamera(camera)) {
			sceneReferenceCamera = camera;
			UpdateSceneCameras();
		}
	}

	void OnDisable() {
		if (camera && sceneReferenceCamera == camera) {
			CleanSceneCameras();
			sceneReferenceCamera = null;
		}
	}

	private void UpdateSceneCameras() {
		//add a PreviewCulling script to the scene camera if not done already
		Camera[] sceneCameras = SceneView.GetAllSceneCameras();
		for (int i = 0; i < sceneCameras.Length; i++) {
			if (!sceneCameras[i].GetComponent<PreviewCulling>()) {
				sceneCameras[i].gameObject.AddComponent<PreviewCulling>();
			}
		}
	}

	void CleanSceneCameras() {
		Camera[] sceneCameras = SceneView.GetAllSceneCameras();
		for (int i = 0; i < sceneCameras.Length; i++) {
			PreviewCulling pc = sceneCameras[i].GetComponent<PreviewCulling>();
			if (pc) {
				DestroyImmediate(pc);
			}
		}
	}

	void OnPreCull() {
		//if the scene camera is about to cull, save current transform values and copy from the reference camera
		if (camera && sceneReferenceCamera && IsSceneViewCamera(camera)) { 
			pos  = transform.position;
			rot  = transform.rotation;
			near = camera.nearClipPlane;
			far  = camera.farClipPlane;
			camera.projectionMatrix = sceneReferenceCamera.projectionMatrix;
			camera.transform.position = sceneReferenceCamera.transform.position;
			camera.transform.rotation = sceneReferenceCamera.transform.rotation;
			camera.nearClipPlane = sceneReferenceCamera.nearClipPlane;
			camera.farClipPlane = sceneReferenceCamera.farClipPlane;
		}
	}

	void OnPreRender() {
		//if the scene camera is about to render reset its transform to saved values
		if (camera && IsSceneViewCamera(camera)) {
			transform.position = pos;
			transform.rotation = rot;
			camera.nearClipPlane = near;
			camera.farClipPlane = far;
			camera.ResetProjectionMatrix();
		}
	}
	#endif
}