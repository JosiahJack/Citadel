using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildren : MonoBehaviour {
	void Start() {
		Matrix4x4 parentTransform = transform.worldToLocalMatrix;
		Dictionary<string, List<CombineInstance>> combines = new Dictionary<string, List<CombineInstance>>();
		Dictionary<string , Material> namedMaterials = new Dictionary<string, Material>();
		MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
		int i = 0;
		int j = 0;
		while (i < meshRenderers.Length) {
		//foreach (var meshRenderer in meshRenderers) {
			while (j < meshRenderers[i].sharedMaterials.Length) {
			//foreach (var material in meshRenderers.sharedMaterials) {
				Material material = meshRenderers[i].sharedMaterials[j];
				if (material != null && !combines.ContainsKey(material.name)) {
					combines.Add(material.name, new List<CombineInstance>());
					namedMaterials.Add(material.name, material);
				}
				j++;
			}
			i++;
		}

		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		i = 0;
		while(i < meshFilters.Length) {
		//foreach(var filter in meshFilters) {
			if (meshFilters[i].sharedMesh == null) continue;
			Renderer filterRenderer = meshFilters[i].GetComponent<Renderer>();
			if (filterRenderer.sharedMaterial == null) continue;
			if (filterRenderer.sharedMaterials.Length > 1) continue;
			CombineInstance ci = new CombineInstance {mesh = meshFilters[i].sharedMesh,transform = parentTransform*meshFilters[i].transform.localToWorldMatrix};
			combines[filterRenderer.sharedMaterial.name].Add(ci);
			Destroy(filterRenderer);
			i++;
		}

		foreach (Material m in namedMaterials.Values) {
			GameObject go = new GameObject("Combined mesh");
			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;

			MeshFilter filter = go.AddComponent<MeshFilter>();
			filter.mesh.CombineMeshes(combines[m.name].ToArray(), true, true);

			MeshRenderer arenderer = go.AddComponent<MeshRenderer>();
			arenderer.material = m;
		}
	}
}