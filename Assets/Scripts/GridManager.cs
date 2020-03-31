using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
	public List<GameObject> MasterGrid1;
	public List<GameObject> MasterGrid2;
	public List<GameObject> MasterGrid3;
	public List<GameObject> MasterGrid4;
	public List<GameObject> MasterGrid5;
	public List<GameObject> MasterGrid6;
	public List<GameObject> MasterGrid7;
	public List<GameObject> MasterGrid8;
	public List<GameObject> MasterGrid9;
	public List<GameObject> MasterGrid10;
	public List<GameObject> MasterGrid11;
	public List<GameObject> MasterGrid12;
	public List<GameObject> MasterGrid13;
	public Texture[] levelMaps;

	public Texture blankMap;
	public GridManager a;

	void Awake() {
		a = this;
		levelMaps = new Texture[13]; //13 because the 14th Cyberspace doesn't get an automap
		for (int i=0;i<levelMaps.Length;i++) {
			levelMaps[i] = blankMap;
		}
	}
	
	public void RegisterGridObject (int lev, GameObject gridObj) {
		switch (lev) {
			case 0: MasterGrid1.Add(gridObj); break;
			case 1: MasterGrid2.Add(gridObj); break;
			case 2: MasterGrid3.Add(gridObj); break;
			case 3: MasterGrid4.Add(gridObj); break;
			case 4: MasterGrid5.Add(gridObj); break;
			case 5: MasterGrid6.Add(gridObj); break;
			case 6: MasterGrid7.Add(gridObj); break;
			case 7: MasterGrid8.Add(gridObj); break;
			case 8: MasterGrid9.Add(gridObj); break;
			case 9: MasterGrid10.Add(gridObj); break;
			case 10: MasterGrid11.Add(gridObj); break;
			case 11: MasterGrid12.Add(gridObj); break;
			case 12: MasterGrid13.Add(gridObj); break;
		}
		
	}


	/*
	private static int width = 128;
	private static int length = 128;
	private static int numLevels = 13;
	public S_GridOpen[] levelGridData = new S_GridOpen[13]; // reduced from 14 to 13 since we don't need to register the cyberspace grids to reduce memory usage

	void Start () {
		for (int i=0;i<levelGridData.Length;i++) {
			levelGridData[i] = new S_GridOpen(new bool[width*length], new Vector3(0f, 0f, 0f)); // EGAD!  We just added 26.6kb of bools and 156bytes of floats.  Well at least your honest about it
		}
	}

	public void RegisterGridLocationOpen (int level, Transform gt) {
		levelGridData[level].gridOpen[GetGridPosition(level,gt.position)] = true;
	}

	public void RegisterGridCenterForLevel (int level, Vector3 pos) {
		levelGridData[level].gridOrigin = pos; 
	}

	public int GetGridPosition(int levIndex,Vector3 refPos) {
		// first get the x,y coordinates (no you stupid Unity...X is X and Y is Y but we'll pretend z is y for now you idiots...bla bla 2D game support bla bla)
		int x = (levelGridData[levIndex].gridOrigin.x - refPos.x) / 2.56f;
		int y = (levelGridData[levIndex].gridOrigin.z - refPos.z) / 2.56f;
		//int xystringIndex = 
	}

	public struct S_GridOpen {
		public bool[] gridOpen;
		public Vector3 gridOrigin;

		public S_GridOpen(bool[] a, Vector3 b) {
			gridOpen = a;
			gridOrigin = b;
		}
	}
	*/
}
