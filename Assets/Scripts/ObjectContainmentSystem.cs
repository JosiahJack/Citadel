using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ObjectContainmentSystem {
    public static List<GameObject> FloorChunks;
    public static List<GameObject> ActiveFloorChunks;
    //public static bool[] OpenCell = new bool[4096];

    public static void FindAllFloorGOs() {
        if (FloorChunks == null) FloorChunks = new List<GameObject>();
		List<GameObject> allParents = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
		for (int i=0;i<allParents.Count;i++) {
			Component[] compArray = allParents[i].GetComponentsInChildren(typeof(MarkAsFloor),true);
			for (int k=0;k<compArray.Length;k++) {
				FloorChunks.Add(compArray[k].gameObject);
			}
		}
	}

    public static void UpdateActiveFlooring() {
        if (ActiveFloorChunks == null) ActiveFloorChunks = new List<GameObject>();
        if (FloorChunks == null) {
            FloorChunks = new List<GameObject>();
            FindAllFloorGOs();
        }
        ActiveFloorChunks.Clear();
        //for (int i=0; i<4096; i++) {
        //    OpenCell[i] = false;
        //}

        for (int i=0; i<FloorChunks.Count;i++) {
            if (FloorChunks[i].activeInHierarchy) ActiveFloorChunks.Add(FloorChunks[i]);
        }

        //float xMin = 0f;
        //float xMax = 0f;
        //float yMin = 0f;
        //float yMax = 0f;
        //Vector2 gridPos;
        //for (int i=0; i<ActiveFloorChunks.Count;i++) {
        //    gridPos = new Vector2(ActiveFloorChunks[i].transform.localPosition.x,
        //                          ActiveFloorChunks[i].transform.localPosition.y);
        //    if (gridPos.x < xMin) xMin = gridPos.x;
        //    if (gridPos.x > xMax) xMax = gridPos.x;
        //    if (gridPos.y < yMin) yMin = gridPos.y;
        //    if (gridPos.y > yMax) yMax = gridPos.y;
        //}

        //Debug.Log("Found extents of active flooring: "
        //          + xMin.ToString() + ", " + yMin.ToString()
        //          + " | "
        //          + xMax.ToString() + ", " + yMax.ToString());
    }

    public static Vector3 FindNearestFloor(float x, float y, float heightFallback) {
        Vector3 checkpos;
        float distMin = 1000000f;
        for (int i=0;i<ActiveFloorChunks.Count;i++) {
            checkpos = ActiveFloorChunks[i].transform.position;
            float deltax = x - checkpos.x;
            float deltay = y - checkpos.z; // Stupid Unity
            if (deltax < distMin) distMin = deltax;
            if (deltay < distMin) distMin = deltay;
            if (deltax < 1.28f && deltay < 1.28f) return checkpos; // Most likely the floor we fell through.
        }

        // Go back and find the floor closest to us.  Could have blasted away via explosion.
        for (int i=0;i<ActiveFloorChunks.Count;i++) {
            checkpos = ActiveFloorChunks[i].transform.position;
            float deltax = x - checkpos.x;
            float deltay = y - checkpos.z; // Stupid Unity
            if (deltax - distMin < 1.28f) return checkpos;
            if (deltay - distMin < 1.28f) return checkpos;
        }

        return new Vector3(x,heightFallback,y);
    }
}