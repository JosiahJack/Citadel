using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public Material[] materials;
    public Transform thePlayer;
    Vector3 thePosition;
    void Start()
    {
        StartCoroutine("writeToMaterial");
    }

    IEnumerator writeToMaterial()
    {
        while (true)
        {
            thePosition = thePlayer.transform.position;
            for(int i=0; i<materials.Length; i++)
            {
                materials[i].SetVector("_position", thePosition);
            }
            yield return null;
        }
    }
}
