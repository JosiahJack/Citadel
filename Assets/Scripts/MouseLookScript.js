#pragma strict

var lookSensitivity : float = 5;
@HideInInspector
var yRotation : float;
@HideInInspector
var xRotation : float;
@HideInInspector
var zRotation : float;
@HideInInspector
var currentYRotation : float;
@HideInInspector
var currentXRotation : float;
@HideInInspector
var currentZRotation : float;
@HideInInspector
var yRotationV : float;
@HideInInspector
var xRotationV : float;
var lookSmoothDamp : float = 0.1;
@HideInInspector
var zRotationV : float;
//@HideInInspector
//var ray : Ray;

var testScreenSpaceObject : GameObject;

@HideInInspector
var mlookstring1 : String = "";
@HideInInspector
var mlookstring2 : String = "";
@HideInInspector
var mlookstring3 : String = "";
@HideInInspector
var mlookstring4 : String = "";

//var headbobSpeed : float = 1;
//var headbobStepCounter : float;
//var headbobAmountX : float = 1;
//var headbobAmountY : float = 1;
//var parentLastPos : Vector3;
//var eyeHeightRatio : float = 0.9;

//function Awake () {
	//parentLastPos = transform.parent.position;
//}

function Update () {
	//if (transform.parent.GetComponent(PlayerMovement).grounded)
		//headbobStepCounter += (Vector3.Distance(parentLastPos, transform.parent.position) * headbobSpeed);
	
	//transform.localPosition.x = (Mathf.Sin(headbobStepCounter) * headbobAmountX);
	//transform.localPosition.y = (Mathf.Cos(headbobStepCounter * 2) * headbobAmountY * -1) + (transform.localScale.y * eyeHeightRatio) - (transform.localScale.y / 2);
	//parentLastPos = transform.parent.position;

	yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
	xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
	
	xRotation = Mathf.Clamp(xRotation, -90, 90);  // Prevents you from looking up too far and breaking your neck.  Need to disable for Cyberspace!
	
	currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, xRotationV, lookSmoothDamp);  // Not really necessary, helps with smoothness
	currentYRotation = Mathf.SmoothDamp(currentYRotation, yRotation, yRotationV, lookSmoothDamp);  // "     "       "         "    "      "
	
	transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);  // If removing the currentXRotation calls above, remove "current" in arguments
	
	if(Input.GetMouseButtonDown(1)) {
		var hit : RaycastHit = new RaycastHit();
		// TIP: Use Camera.main.ViewportPointToRay for center of screen
		if (Physics.Raycast(GetComponent.<Camera>().ScreenPointToRay(Input.mousePosition), hit, 100)) {
			var rendererObj : Renderer = hit.collider.GetComponent.<MeshRenderer>();
			if (rendererObj != null && rendererObj.material != null && rendererObj.material.mainTexture != null && hit.collider != null) {
				var meshCollider = hit.collider as MeshCollider;
				if (meshCollider == null || meshCollider.sharedMesh == null)
					return;
					
				var mesh : Mesh = hit.collider.gameObject.GetComponent.<MeshFilter>().sharedMesh;
				//var mesh : Mesh = hit.collider.gameObject.GetComponent(MeshFilter).mesh;
				//var submeshTris = new Array();
				//var hittedTriangle = new Array();
				//var subMeshIndex : int = -1;
				//var subtri : int = -1;
				//mlookstring1 = "hit.triangleIndex = " + hit.triangleIndex.ToString();
				//hittedTriangle[0] = mesh.triangles[hit.triangleIndex * 3];
				//hittedTriangle[1] = mesh.triangles[hit.triangleIndex * 3 + 1];
				//hittedTriangle[2] = mesh.triangles[hit.triangleIndex * 3 + 2];
				//var p0: Vector3 = mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 0]];
				//var p1: Vector3 = mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 1]];
				//var p2: Vector3 = mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 2]];
				//var t0 : Vector3;
				//var t1 : Vector3;
				//var t2 : Vector3;
				//var dist1 : float = 100000;
				//var dist2 : float = 100000;
				//var dist3 : float = 100000;
				//var side1 : float = 100000;
				//var side2 : float = 100000;
				//var side3 : float = 100000;
				//var numTriangles = mesh.triangles.Length/3;
/*
				for(var i=0;i<numTriangles;i+=3) {
					t0 = mesh.vertices[mesh.triangles[i * 3 + 0]];
					t1 = mesh.vertices[mesh.triangles[i * 3 + 1]];
					t2 = mesh.vertices[mesh.triangles[i * 3 + 2]];
					side1 = Vector3.Distance(t0, t1);
					side2 = Vector3.Distance(t1, t2);
					side3 = Vector3.Distance(t2, t0);
					dist1 = Vector3.Distance(hit.point, t0);
					dist2 = Vector3.Distance(hit.point, t1);
					dist3 = Vector3.Distance(hit.point, t2);
					
					if (dist1 < side1 && dist2 < side1 && dist3 < side1) {
						if (dist1 < side2 && dist2 < side2 && dist3 < side2) {
							if (dist1 < side3 && dist2 < side3 && dist3 < side3) {
								subtri = i;
								break;
							}
						}
					}
				}
*/
/*
				for(i=0;i<mesh.subMeshCount;i++) {
					submeshTris = mesh.GetTriangles(i);
					for (var j=0;j<submeshTris.length;j += 3) {
						if(submeshTris[j] == hittedTriangle[0] && submeshTris[j+1] == hittedTriangle[1] && submeshTris[j+2] == hittedTriangle[2]) {
							subMeshIndex = i;
							mlookstring2 = "Submesh Index = " + subMeshIndex.ToString() + "\n";
							break;
						}
					}
					if (subMeshIndex != -1)
						break;
				}
*/
				//if (subtri != -1) {
					//Draw green lines on edges of hit tri
					var p0: Vector3 = mesh.vertices[mesh.triangles[(hit.triangleIndex * 3) + 0]];
					var p1: Vector3 = mesh.vertices[mesh.triangles[(hit.triangleIndex * 3) + 1]];
					var p2: Vector3 = mesh.vertices[mesh.triangles[(hit.triangleIndex * 3) + 2]];
					var hitTransform: Transform = hit.collider.gameObject.transform;
					p0 = hitTransform.TransformPoint(p0);
					p1 = hitTransform.TransformPoint(p1);
					p2 = hitTransform.TransformPoint(p2);
					Debug.DrawLine(p0, p1, Color.green, 999, false);
					Debug.DrawLine(p1, p2, Color.green, 999, false);
					Debug.DrawLine(p2, p0, Color.green, 999, false);
					
					//Tell player that we can't use "suchnsuch" wall
					//mlookstring3 = "Can't use " + rendererObj.materials[0].name + "\n";
					//print("MouseLookScript: Player tried to use wall.  Materials on object are: \n");
					//for (i=0; i<mesh.subMeshCount;i++) {
					//	print("Submesh/Material " + i.ToString() + " " + rendererObj.materials[i].name + "\n");
					//}
				//}
			}
			//var beakerInstance : GameObject;
			//beakerInstance = Instantiate(testScreenSpaceObject, hit.point, Quaternion.LookRotation(transform.position - hit.point));
			
			// Draws a line from the camera to the raycast hit.point
			Debug.DrawLine(transform.position, hit.point, Color.green, 999, false);
			
			//mlookstring4 = rendererObj.name;
			//print("MouseLookScript: " + mlookstring1 + " " + mlookstring2 + " " + mlookstring3 + " " + mlookstring4 + "\n"); //rendererObj.name + "\n");
		}
	}
}