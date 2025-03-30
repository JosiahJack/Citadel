using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Tests {
    public class TestCulling {
        private bool sceneLoaded = false;

        private void SaveDebugScreenshot(Texture2D shot, string debugText) {
            // Only for testing locally, not on CI
            string sname = debugText + "_" + System.DateTime.UtcNow.ToString("ddMMMyyyy_HH_mm_ss")
                        + "_" + Const.a.versionString + ".png";
            string spath = Utils.SafePathCombine(Application.streamingAssetsPath,
                                                "Screenshots");

            // Check and recreate Screenshots folder if it was deleted.
            if (!Directory.Exists(spath)) Directory.CreateDirectory(spath);
            spath = Utils.SafePathCombine(spath,sname);
            byte[] bytes = shot.EncodeToPNG();
            File.WriteAllBytes(spath, bytes);
        }
        
        public void RunBeforeAnyTests() {
            if (sceneLoaded) return;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("CitadelScene");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.name == "CitadelScene") {
                SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the event to avoid handling it multiple times
                sceneLoaded = true; // Indicate that the scene is loaded.
            }
        }

        /*
        [UnityTest]
        [Timeout(100000000)]
        public IEnumerator CheckEachCellCulling() {
            RunBeforeAnyTests();
            yield return new WaitUntil(() => sceneLoaded);

            yield return new WaitForSeconds(1.5f);
            
            MainMenuHandler.a.StartGame(true);
            yield return new WaitForSeconds(0.1f);

            int chunkCount,x,y,i,pinkCount;
            PlayerMovement.a.hm.god = true;
            PlayerMovement.a.Notarget = true;
            PlayerMovement.a.rbody.isKinematic = true;
            PlayerMovement.a.rbody.useGravity = false;
            PlayerMovement.a.enabled = false;
            PlayerReferenceManager.a.playerCanvas.SetActive(false);
            MouseLookScript.a.enabled = false;
            DynamicCulling.a.useLODMeshes = true;
            DynamicCulling.a.overrideWindowsToBlack = true;
            DynamicCulling.a.outputDebugImages = false;
            DynamicCulling.a.Cull(true);
            MouseLookScript.a.playerCamera.enabled = false;
            Const.a.testSphere.SetActive(true);
            RenderTexture capture = new RenderTexture(1024,1024,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);
            Texture2D captureTex = new Texture2D(1024,1024,TextureFormat.RGB24,false);
            RenderTexture.active = capture;
            MouseLookScript.a.playerCamera.targetTexture = capture;
            MouseLookScript.a.playerCamera.nearClipPlane = 0.02f;
            float minY, maxY;
            ChunkPrefab chp;
            Vector3 avgPos;
            bool foundPink = false;
            float pinkMin = 217f/256f; // Give cushion around the pink value of 219 0 219
            float pinkMax = 221f/256f;
            float pinkGThreshold = 1f/256f;
            float timeTillNext = 0.0001f;
            int pinkCountThreshold = 128;
            float floorHeight;
            MarkAsFloor maf = null;
            Color[] pixels;
            bool minMaxNotSet = true;
            int layerMask = LayerMask.GetMask("Default","Geometry");
            
            int numLeaks = 0;
            
            for (int lev=0;lev<13;lev++) {
                ConsoleEmulator.CheatLoadLevel(lev);
                yield return new WaitForSeconds(0.1f);
                DynamicCulling.a.Cull(true);
                yield return new WaitForSeconds(timeTillNext);
                for (x=0;x<DynamicCulling.WORLDX;x++) {
                    for (y=0;y<DynamicCulling.WORLDX;y++) {
                        if (lev == 6) {
                            if (x == 38 && y == 21) continue; // These are too small to fit camera in, not needed to check.
                            if (x == 38 && y == 26) continue;
                            if (x == 40 && y == 21) continue;
                            if (x == 44 && y == 26) continue;
                            if (x == 40 && y == 26) continue;
                            if (x == 42 && y == 26) continue;
                        }
                        if (!DynamicCulling.a.gridCells[x,y].open) continue;
                        if (DynamicCulling.a.gridCells[x,y].open && DynamicCulling.a.gridCells[x,y].closedNorth && DynamicCulling.a.gridCells[x,y].closedSouth && DynamicCulling.a.gridCells[x,y].closedWest && DynamicCulling.a.gridCells[x,y].closedEast) continue;
                        if (DynamicCulling.a.gridCells[x,y].chunkPrefabs.Count < 1) continue;
                    
                        avgPos = Vector3.zero;
                        minY = 0f;
                        maxY = 0f;
                        chunkCount = DynamicCulling.a.gridCells[x,y].chunkPrefabs.Count;
                        minMaxNotSet = true;
                        int[] numAtHeightsForIndex = new int[chunkCount];
                        float[] heights = new float[chunkCount];
                        floorHeight = -1300f;
                        for (i=0;i<chunkCount;i++) {
                            chp = DynamicCulling.a.gridCells[x,y].chunkPrefabs[i];
                            maf = chp.go.GetComponent<MarkAsFloor>();
                            if (maf != null) {
                                floorHeight = maf.floorHeight - 1.28f;
                            }
                            
                            heights[i] = chp.go.transform.position.y;
                            avgPos += chp.go.transform.position;
                            if (minMaxNotSet) {
                                minY = maxY = chp.go.transform.position.y;
                                minMaxNotSet = false;
                            } else {
                                if (chp.go.transform.position.y < minY) minY = chp.go.transform.position.y;
                                if (chp.go.transform.position.y > maxY) maxY = chp.go.transform.position.y;
                            }                        
                        }
                        
                        if (floorHeight < -1000f) {
                            floorHeight = (((maxY - minY) / 2f) + minY) - 0.6171862f;
                        }
                        
                        avgPos = new Vector3(avgPos.x / chunkCount, floorHeight + 0.16f, avgPos.z / chunkCount);
                        GameObject cullCamMarker = new GameObject("CullCamMarker_" + x.ToString() + "." + y.ToString());
                        cullCamMarker.transform.position = avgPos;
                        MeshFilter mf = cullCamMarker.AddComponent<MeshFilter>();
                        mf.sharedMesh = Const.a.sphereMesh;
                        MeshRenderer mR = cullCamMarker.AddComponent<MeshRenderer>();
                        mR.material = Const.a.segiEmitterMaterial1;
                        mR.material.SetColor("_EmissionColor",new Color(0.9f,0.2f,0.1f,1f));
                        cullCamMarker.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                        cullCamMarker.layer = 2; // IgnoreRaycast

                        PlayerMovement.a.transform.position = avgPos;
                        yield return new WaitForSeconds(timeTillNext);
                        MouseLookScript.a.playerCamera.enabled = false;
                        DynamicCulling.a.Cull(true);
                        LevelManager.a.SetSkyVisible(false);
                        yield return new WaitForSeconds(timeTillNext);
                        
                        MouseLookScript.a.playerCamera.enabled = true;
                        PlayerMovement.a.transform.localRotation = Quaternion.Euler(0f,0f,0f); // Yaw 0, East
                        MouseLookScript.a.transform.localRotation = Quaternion.Euler(0f,0f,0f); // Pitch 0
                        RenderTexture.active = capture;
                        MouseLookScript.a.playerCamera.targetTexture = capture;
                        MouseLookScript.a.playerCamera.Render();
                        captureTex.ReadPixels(new Rect(0, 0, capture.width, capture.height), 0, 0);
                        MouseLookScript.a.playerCamera.targetTexture = null;
                        captureTex.Apply();
                        pixels = captureTex.GetPixels(0);
                        foundPink = false;
                        pinkCount = 0;
                        for (i=0;i<pixels.Length;i++) {
                            if ((pixels[i].r > pinkMin && pixels[i].r < pinkMax)
                                && (pixels[i].b > pinkMin && pixels[i].b < pinkMax)
                                && pixels[i].g < pinkGThreshold) {
                                
                                pinkCount++;
                                if (pinkCount > pinkCountThreshold) {
                                    foundPink = true;
                                    break;
                                }
                            }
                        }
                        
                        if (foundPink) {
                            SaveDebugScreenshot(captureTex,lev.ToString() + "_gridCellxy" + x.ToString() + "." + y.ToString() + "_pixelI" + i.ToString());
                            yield return new WaitForSeconds(timeTillNext);

                            UnityEngine.Debug.LogWarning(lev.ToString() + " Culling error 1 on cell " + x.ToString() + "," + y.ToString());
                            numLeaks++;
                            continue;
                        }
                    
                        PlayerMovement.a.transform.localRotation = Quaternion.Euler(0f,90f,0f); // Yaw 90, North
                        MouseLookScript.a.transform.localRotation = Quaternion.Euler(0f,0f,0f); // Pitch 0
                        yield return new WaitForSeconds(timeTillNext);
                        RenderTexture.active = capture;
                        MouseLookScript.a.playerCamera.targetTexture = capture;
                        MouseLookScript.a.playerCamera.Render();
                        captureTex.ReadPixels(new Rect(0, 0, capture.width, capture.height), 0, 0);
                        MouseLookScript.a.playerCamera.targetTexture = null;
                        captureTex.Apply();
                        pixels = captureTex.GetPixels(0);
                        foundPink = false;
                        pinkCount = 0;
                        for (i=0;i<pixels.Length;i++) {
                            if ((pixels[i].r > pinkMin && pixels[i].r < pinkMax)
                                && (pixels[i].b > pinkMin && pixels[i].b < pinkMax)
                                && pixels[i].g < pinkGThreshold) {
                                
                                pinkCount++;
                                if (pinkCount > pinkCountThreshold) {
                                    foundPink = true;
                                    break;
                                }
                            }
                        }
                        
                        if (foundPink) {
                            SaveDebugScreenshot(captureTex,lev.ToString() + "_gridCellxy" + x.ToString() + "." + y.ToString() + "_pixelI" + i.ToString());
                            yield return new WaitForSeconds(timeTillNext);

                            UnityEngine.Debug.LogWarning(lev.ToString() + " Leak detected facing North on cell " + x.ToString() + "," + y.ToString());
                            numLeaks++;
                            continue;
                        }
                                    
                        PlayerMovement.a.transform.localRotation = Quaternion.Euler(0f,180f,0f); // Yaw 180, West
                        MouseLookScript.a.transform.localRotation = Quaternion.Euler(0f,0f,0f);
                        yield return new WaitForSeconds(timeTillNext);
                        RenderTexture.active = capture;
                        MouseLookScript.a.playerCamera.targetTexture = capture;
                        MouseLookScript.a.playerCamera.Render();
                        captureTex.ReadPixels(new Rect(0, 0, capture.width, capture.height), 0, 0);
                        MouseLookScript.a.playerCamera.targetTexture = null;
                        captureTex.Apply();
                        pixels = captureTex.GetPixels(0);
                        foundPink = false;
                        pinkCount = 0;
                        for (i=0;i<pixels.Length;i++) {
                            if ((pixels[i].r > pinkMin && pixels[i].r < pinkMax)
                                && (pixels[i].b > pinkMin && pixels[i].b < pinkMax)
                                && pixels[i].g < pinkGThreshold) {
                                
                                pinkCount++;
                                if (pinkCount > pinkCountThreshold) {
                                    foundPink = true;
                                    break;
                                }
                            }
                        }
                        
                        if (foundPink) {
                            SaveDebugScreenshot(captureTex,lev.ToString() + "_gridCellxy" + x.ToString() + "." + y.ToString() + "_pixelI" + i.ToString());
                            yield return new WaitForSeconds(timeTillNext);

                            UnityEngine.Debug.LogWarning(lev.ToString() + " Leak detected facing West on cell " + x.ToString() + "," + y.ToString());
                            numLeaks++;
                            continue;
                        }
                    
                        PlayerMovement.a.transform.localRotation = Quaternion.Euler(0f,270f,0f); // Yaw 270, South
                        MouseLookScript.a.transform.localRotation = Quaternion.Euler(0f,0f,0f);
                        yield return new WaitForSeconds(timeTillNext);
                        RenderTexture.active = capture;
                        MouseLookScript.a.playerCamera.targetTexture = capture;
                        MouseLookScript.a.playerCamera.Render();
                        captureTex.ReadPixels(new Rect(0, 0, capture.width, capture.height), 0, 0);
                        MouseLookScript.a.playerCamera.targetTexture = null;
                        captureTex.Apply();
                        pixels = captureTex.GetPixels(0);
                        foundPink = false;
                        pinkCount = 0;
                        for (i=0;i<pixels.Length;i++) {
                            if ((pixels[i].r > pinkMin && pixels[i].r < pinkMax)
                                && (pixels[i].b > pinkMin && pixels[i].b < pinkMax)
                                && pixels[i].g < pinkGThreshold) {
                                
                                pinkCount++;
                                if (pinkCount > pinkCountThreshold) {
                                    foundPink = true;
                                    break;
                                }
                            }
                        }
                        
                        if (foundPink) {
                            SaveDebugScreenshot(captureTex,lev.ToString() + "_gridCellxy" + x.ToString() + "." + y.ToString() + "_pixelI" + i.ToString());
                            yield return new WaitForSeconds(timeTillNext);

                            UnityEngine.Debug.LogWarning(lev.ToString() + " Leak detected facing South on cell " + x.ToString() + "," + y.ToString());
                            numLeaks++;
                            continue;
                        }
                    
                        PlayerMovement.a.transform.localRotation = Quaternion.Euler(0f,0f,0f);
                        MouseLookScript.a.transform.localRotation = Quaternion.Euler(90f,0f,0f); // Down
                        yield return new WaitForSeconds(timeTillNext);
                        RenderTexture.active = capture;
                        MouseLookScript.a.playerCamera.targetTexture = capture;
                        MouseLookScript.a.playerCamera.Render();
                        captureTex.ReadPixels(new Rect(0, 0, capture.width, capture.height), 0, 0);
                        MouseLookScript.a.playerCamera.targetTexture = null;
                        captureTex.Apply();
                        pixels = captureTex.GetPixels(0);
                        foundPink = false;
                        pinkCount = 0;
                        for (i=0;i<pixels.Length;i++) {
                            if ((pixels[i].r > pinkMin && pixels[i].r < pinkMax)
                                && (pixels[i].b > pinkMin && pixels[i].b < pinkMax)
                                && pixels[i].g < pinkGThreshold) {
                                
                                pinkCount++;
                                if (pinkCount > pinkCountThreshold) {
                                    foundPink = true;
                                    break;
                                }
                            }
                        }
                        
                        if (foundPink) {
                            SaveDebugScreenshot(captureTex,lev.ToString() + "_gridCellxy" + x.ToString() + "." + y.ToString() + "_pixelI" + i.ToString());
                            yield return new WaitForSeconds(timeTillNext);

                            UnityEngine.Debug.LogWarning(lev.ToString() + " Leak detected facing down on cell " + x.ToString() + "," + y.ToString());
                            numLeaks++;
                            continue;
                        }
                    
                        PlayerMovement.a.transform.localRotation = Quaternion.Euler(0f,0f,0f);
                        MouseLookScript.a.transform.localRotation = Quaternion.Euler(-90f,0f,0f); // Up
                        yield return new WaitForSeconds(timeTillNext);
                        RenderTexture.active = capture;
                        MouseLookScript.a.playerCamera.targetTexture = capture;
                        MouseLookScript.a.playerCamera.Render();
                        captureTex.ReadPixels(new Rect(0, 0, capture.width, capture.height), 0, 0);
                        MouseLookScript.a.playerCamera.targetTexture = null;
                        captureTex.Apply();
                        pixels = captureTex.GetPixels(0);
                        foundPink = false;
                        pinkCount = 0;
                        for (i=0;i<pixels.Length;i++) {
                            if ((pixels[i].r > pinkMin && pixels[i].r < pinkMax)
                                && (pixels[i].b > pinkMin && pixels[i].b < pinkMax)
                                && pixels[i].g < pinkGThreshold) {
                                
                                pinkCount++;
                                if (pinkCount > pinkCountThreshold) {
                                    foundPink = true;
                                    break;
                                }
                            }
                        }
                        
                        if (foundPink) {
                            SaveDebugScreenshot(captureTex,lev.ToString() + "_gridCellxy" + x.ToString() + "." + y.ToString() + "_pixelI" + i.ToString());
                            yield return new WaitForSeconds(timeTillNext);

                            UnityEngine.Debug.LogWarning(lev.ToString() + " Leak detected facing up on cell " + x.ToString() + "," + y.ToString());
                            numLeaks++;
                        }
                    }
                }
            }
            Assert.That(numLeaks == 0,"Found " + numLeaks.ToString() + " leaks.  See warnings for details.");

            // Revert to normal
            Const.a.testSphere.SetActive(false);
            PlayerMovement.a.Notarget = false;
            PlayerMovement.a.rbody.isKinematic = false;
        }*/
    }
}
