#pragma strict
static var inventoryMode : boolean;
public var cursorTexture : Texture2D;
public var cursorHotspot : Vector2;
public var lookSensitivity : float = 5;

@HideInInspector
var yRotation : float;
@HideInInspector
var xRotation : float;
@HideInInspector
var zRotation : float;
@HideInInspector
var currentZRotation : float;
@HideInInspector
var yRotationV : float;
@HideInInspector
var xRotationV : float;
var lookSmoothDamp : float = 0.1;
@HideInInspector
var zRotationV : float;
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

function Start () {
	cursorHotspot = new Vector2 (cursorTexture.width/2, cursorTexture.height/2);
	Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
	Cursor.visible = true;
	Cursor.lockState = CursorLockMode.None;
	inventoryMode = true;  // Start with inventory mode turned on
}

function Update () {
	//if (transform.parent.GetComponent(PlayerMovement).grounded)
		//headbobStepCounter += (Vector3.Distance(parentLastPos, transform.parent.position) * headbobSpeed);
	
	//transform.localPosition.x = (Mathf.Sin(headbobStepCounter) * headbobAmountX);
	//transform.localPosition.y = (Mathf.Cos(headbobStepCounter * 2) * headbobAmountY * -1) + (transform.localScale.y * eyeHeightRatio) - (transform.localScale.y / 2);
	//parentLastPos = transform.parent.position;
	if (inventoryMode == false) {
		yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
		xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
		xRotation = Mathf.Clamp(xRotation, -90, 90);  // Limit up and down angle. TIP:: Need to disable for Cyberspace!
		transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
	}
	
	if(Input.GetKeyDown(KeyCode.Tab))
		ToggleInventoryMode();
	
	if(Input.GetMouseButtonDown(1)) {
		var hit : RaycastHit = new RaycastHit();
		if (Physics.Raycast(GetComponent.<Camera>().ScreenPointToRay(Input.mousePosition), hit, 3)) {
			// TIP: Use Camera.main.ViewportPointToRay for center of screen
			if (hit.collider == null)
				return;
			
			// Check if object is usable and then use it
			if (hit.collider.tag == "Usable")
				hit.transform.SendMessageUpwards("Use");
				
			var rendererObj : Renderer = hit.collider.GetComponent.<MeshRenderer>();
			if (rendererObj != null && rendererObj.material != null && rendererObj.material.mainTexture != null) {
				var meshCollider = hit.collider as MeshCollider;
				if (meshCollider == null || meshCollider.sharedMesh == null)
					return;
					
				var mesh : Mesh = hit.collider.gameObject.GetComponent.<MeshFilter>().sharedMesh;
				var submeshTris = new Array();
				var hittedTriangle = new Array();
				var subMeshIndex : int = -1;
				mlookstring1 = "hit.triangleIndex = " + hit.triangleIndex.ToString();
				hittedTriangle[0] = mesh.triangles[hit.triangleIndex * 3];
				hittedTriangle[1] = mesh.triangles[hit.triangleIndex * 3 + 1];
				hittedTriangle[2] = mesh.triangles[hit.triangleIndex * 3 + 2];
				for(var i=0;i<mesh.subMeshCount;i++) {
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
				//Draw green lines on edges of hit tri
				//var p0: Vector3 = mesh.vertices[mesh.triangles[(hit.triangleIndex * 3) + 0]];
				//var p1: Vector3 = mesh.vertices[mesh.triangles[(hit.triangleIndex * 3) + 1]];
				//var p2: Vector3 = mesh.vertices[mesh.triangles[(hit.triangleIndex * 3) + 2]];
				//var hitTransform: Transform = hit.collider.gameObject.transform;
				//p0 = hitTransform.TransformPoint(p0);
				//p1 = hitTransform.TransformPoint(p1);
				//p2 = hitTransform.TransformPoint(p2);
				//Debug.DrawLine(p0, p1, Color.green, 999, false);
				//Debug.DrawLine(p1, p2, Color.green, 999, false);
				//Debug.DrawLine(p2, p0, Color.green, 999, false);
				
				//Tell player that we can't use "suchnsuch" wall
				mlookstring3 = "Can't use " + rendererObj.materials[subMeshIndex].name + "\n";
			}
			// Draws a line from the camera to the raycast hit.point
			//Debug.DrawLine(transform.position, hit.point, Color.green, 999, false);
			
			//mlookstring4 = rendererObj.name;
			print("MouseLookScript: " + mlookstring1 + " " + mlookstring2 + " " + mlookstring3 + " " + mlookstring4 + "\n"); //rendererObj.name + "\n");
		}
	}
}

function ToggleInventoryMode () {
	if (inventoryMode) {
		Cursor.lockState = CursorLockMode.Locked;
		inventoryMode = false;
	} else {
		Cursor.lockState = CursorLockMode.None;
		inventoryMode = true;
	}
}

// Returns string for describing the walls/floors/etc. based on the material name
function GetTextureDescription (material_name : String) : String {
	var retval : String = "";
	
	// First handle the animated textures
	if (material_name.StartsWith("+"))
		retval = "normal screens";
		
	if (material_name.Contains("eng2_5"))
		retval = "power exchanger";

	if (material_name.Contains("fan"))
		retval = "field generation rotor";
		
	if (material_name.Contains("lift"))
		retval = "repulsor lift";
		
	if (material_name.Contains("bridg"))
		retval = "biological infestation";
		
	if (material_name.Contains("alert"))
		retval = "warning indicator";
		
	if (material_name.Contains("telepad"))
		retval = "jump disk";
	
	// Handle med* textures
	if (material_name.StartsWith("med")) {
		if (material_name == "med1_7" || material_name == "med2_6") {
			retval = "flourescent lighting";
		} else {
			retval = "medical panelling";
		}
	}
	
	if (material_name.Contains("crate"))
		retval = "storage crate";
	
	switch(material_name) {
	case "black32": retval = "";
	case "black64": retval = "";
	case "black128": retval = "";
	case "bridg1_2": retval = "biological infestation";
	case "bridg1_3": retval = "biological infestation";
	case "bridg1_3b": retval = "biological infestation";
	case "bridg1_4": retval = "biological infestation";
	case "bridg1_5": retval = "data transfer schematic";
	case "bridg2_1": retval = "monitoring port";
	case "bridg2_2": retval = "stone mosaic tiling";
	case "bridg2_3": retval = "monitoring port";
	case "bridg2_4": retval = "video observation screen";
	case "bridg2_5": retval = "cyber station";
	case "bridg2_6": retval = "burnished platinum panelling";
	case "bridg2_7": retval = "burnished platinum panelling";
	case "bridg2_8": retval = "SHODAN neural bud";
	case "bridg2_9": retval = "computer";
	case "cabinet": retval = "cabinet";
	case "charge_stat": retval = "energy charge station";
	case "citmat1_1": retval = "CPU node";
	case "citmat1_2": retval = "chair";
	case "citmat1_3": retval = "catwalk";
	case "citmat1_4": retval = "catwalk";
	case "citmat1_5": retval = "";
	case "citmat1_6": retval = "cabinet";
	case "citmat1_7": retval = "catwalk";
	case "citmat1_8": retval = "table top";
	case "citmat1_9": retval = "catwalk";
	case "citmat2_1": retval = "catwalk";
	case "citmat2_2": retval = "cabinet";
	case "citmat2_3": retval = "cabinet";
	case "citmat2_4": retval = "cabinet";
	case "console1_1": retval = "computer";
	case "console1_2": retval = "computer";
	case "console1_3": retval = "cart";
	case "console1_4": retval = "computer";
	case "console1_5": retval = "computer";
	case "console1_6": retval = "console";
	case "console1_7": retval = "console";
	case "console1_8": retval = "console";
	case "console1_9": retval = "console";
	case "console2_1": retval = "console panel";
	case "console2_2": retval = "desk";
	case "console2_3": retval = "computer panel";
	case "console2_4": retval = "computer panel";
	case "console2_5": retval = "computer console";
	case "console2_6": retval = "console controls";
	case "console2_7": retval = "console";
	case "console2_8": retval = "console controls";
	case "console2_9": retval = "console";
	case "console3_1": retval = "cyber space port";
	case "console3_2": retval = "computer";
	case "console3_3": retval = "computer";
	case "console3_4": retval = "keyboard";
	case "console3_5": retval = "computer panelling";
	case "console3_6": retval = "normal screens";
	case "console3_7": retval = "destroyed screen";
	case "console3_8": retval = "desk";
	case "console3_9": retval = "desk";
	case "console4_1": retval = "console controls";
	case "cyber": retval = "x-ray machine";
	case "d_arrow1": retval = "repulsor lights";
	case "d_arrow2": retval = "repulsor lights";
	}
	return retval;
}