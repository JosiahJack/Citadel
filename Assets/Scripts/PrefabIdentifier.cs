using UnityEngine;
//using System.Collections;

// Allows a way to mark a prefab with a unique index value for looking this up
// in an array stored in Const.  This could differ depending on the type of
// prefab this is on, such as level geometry chunk, grenade, etc.
public class PrefabIdentifier : MonoBehaviour {
    public int constIndex;
}