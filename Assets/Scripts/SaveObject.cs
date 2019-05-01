using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveObject : MonoBehaviour {
	//public string SaveID;
	public int SaveID;

	void Start () {
		//string x,y,z;
		//x = transform.localPosition.x.ToString("0000.00000");
		//y = transform.localPosition.y.ToString("0000.00000");
		//z = transform.localPosition.z.ToString("0000.00000");
		//SaveID = (x + y + z);
		SaveID = gameObject.GetInstanceID();
		//Const.sprint("Saveable Object has ID# of: " + SaveID.ToString());
	}
}
