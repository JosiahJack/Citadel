 #pragma strict
 
 function Start() {
         var mesh : Mesh = GetComponent(MeshFilter).mesh;
         Debug.Log(mesh.name + " has " + mesh.subMeshCount + " submeshes!");
 }