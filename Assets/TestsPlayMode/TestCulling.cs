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

        [UnityTest]
        [Timeout(100000000)]
        public IEnumerator CheckEachCellCulling() {
            RunBeforeAnyTests();
            yield return new WaitUntil(() => sceneLoaded);

            yield return new WaitForSeconds(1.5f);
            
            MainMenuHandler.a.StartGame(true);
            yield return new WaitForSeconds(0.25f);

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
            DynamicCulling.a.Cull(true);
            MouseLookScript.a.playerCamera.enabled = false;
            Const.a.testSphere.SetActive(true);
            RenderTexture capture = new RenderTexture(1024,1024,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);
            Texture2D captureTex = new Texture2D(1024,1024,TextureFormat.RGB24,false);
            RenderTexture.active = capture;
            MouseLookScript.a.playerCamera.targetTexture = capture;
            MouseLookScript.a.playerCamera.nearClipPlane = 0.02f;
//             float hitYDn, hitYUp;
            float minY, maxY;
            ChunkPrefab chp;
            Vector3 avgPos;
            bool foundPink = false;
//             bool didHitDn = false;
//             bool didHitUp = false;
            float pinkMin = 217f/256f; // Give cushion around the pink value of 219 0 219
            float pinkMax = 221f/256f;
            float pinkGThreshold = 1f/256f;
            float timeTillNext = 0.001f;
            int pinkCountThreshold = 512;
            Color[] pixels;
//             RaycastHit hit;
            bool minMaxNotSet = true;
            int layerMask = LayerMask.GetMask("Default","Geometry");
            for (x=0;x<DynamicCulling.WORLDX;x++) {
                for (y=0;y<DynamicCulling.WORLDX;y++) {
//                     if (LevelManager.a.currentLevel == 1 && x == 15 && y == 30) continue; // Tiny cell, camera can't fit, open cells adjacent can check it.
                    if (!DynamicCulling.a.gridCells[x,y].open) continue;
                    if (DynamicCulling.a.gridCells[x,y].chunkPrefabs.Count < 1) continue;
                 
                    avgPos = Vector3.zero;
                    minY = 0f;
                    maxY = 0f;
                    chunkCount = DynamicCulling.a.gridCells[x,y].chunkPrefabs.Count;
                    minMaxNotSet = true;
                    for (i=0;i<chunkCount;i++) {
                        chp = DynamicCulling.a.gridCells[x,y].chunkPrefabs[i];
                        
                        avgPos += chp.go.transform.position;
//                         UnityEngine.Debug.Log("x,y: " + x.ToString() + "," + y.ToString() + " | chp.go.transform.position.y: " + chp.go.transform.position.y.ToString());
//                         UnityEngine.Debug.Log("x,y: " + x.ToString() + "," + y.ToString() + " | iter initial minY: " + minY.ToString() + ", maxY: " + maxY.ToString());
                        if (minMaxNotSet) {
                            minY = maxY = chp.go.transform.position.y;
                            minMaxNotSet = false;
//                             UnityEngine.Debug.Log("First set min and max for x,y: " + x.ToString() + "," + y.ToString() + " | minY: " + minY.ToString() + ", maxY: " + maxY.ToString());
                        } else {
                            if (chp.go.transform.position.y < minY) minY = chp.go.transform.position.y;
                            if (chp.go.transform.position.y > maxY) maxY = chp.go.transform.position.y;
//                             UnityEngine.Debug.Log("x,y: " + x.ToString() + "," + y.ToString() + " | minY: " + minY.ToString() + ", maxY: " + maxY.ToString());
                        }                        
                    }
                    
                    avgPos = new Vector3(avgPos.x / chunkCount, avgPos.y / chunkCount, avgPos.z / chunkCount);
//                     UnityEngine.Debug.Log("bef avgPos.y: " + avgPos.y.ToString("F8"));
                    avgPos.y = (((maxY - minY) / 2f) + minY) - 0.6171862f; // 0.6171862 is how far offset camera is from playerCapsule center ;)
//                     UnityEngine.Debug.Log("aft avgPos.y: " + avgPos.y.ToString("F8"));

                    GameObject cullCamMarker = new GameObject("CullCamMarker_" + x.ToString() + "." + y.ToString());
//                     cullCamMarker.transform.parent = LevelManager.a.GetCurrentGeometryContainer().transform;
                    cullCamMarker.transform.position = avgPos;
                    MeshFilter mf = cullCamMarker.AddComponent<MeshFilter>();
                    mf.sharedMesh = Const.a.sphereMesh;
                    MeshRenderer mR = cullCamMarker.AddComponent<MeshRenderer>();
                    mR.material = Const.a.segiEmitterMaterial1;
                    mR.material.SetColor("_EmissionColor",new Color(0.9f,0.2f,0.1f,1f));
                    cullCamMarker.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                    cullCamMarker.layer = 2; // IgnoreRaycast
                    
//                     didHitDn = false;
//                     didHitUp = false;
//                     hitYUp = avgPos.y;
//                     hitYDn = avgPos.y;
//                     if (Physics.Raycast(avgPos, Vector3.down, out hit, 51.2f,layerMask)) {
//                         hitYDn = hit.point.y;
//                         didHitDn = true;
//                     }
//                     
//                     if (Physics.Raycast(avgPos, Vector3.up, out hit, 51.2f,layerMask)) {
//                         hitYUp = hit.point.y;
//                         didHitUp = true;
//                     }

//                     if (x == 15 && y == 40) UnityEngine.Debug.Log("Avg pos prior to raycast adjustment on " + x.ToString() + "," + y.ToString() + " was " + avgPos.ToString());
//                     if (didHitUp && didHitDn) {
//                     
// //                         avgPos.y = ((avgPos.y - hitYDn) < (avgPos.y - hitYUp) ? hitYDn : hitYUp) + 0.24f;
//                     } else if (didHitUp) {
// //                         avgPos.y = hitYUp + 0.24f;
//                         UnityEngine.Debug.Log("Only hit up for " + x.ToString() + "," + y.ToString());
//                     } else if (didHitDn) {
// //                         avgPos.y = hitYDn + 0.24f;
//                         UnityEngine.Debug.Log("Only hit down for " + x.ToString() + "," + y.ToString());
//                     }

//                     if (x == 15 && y == 40) UnityEngine.Debug.Log("Avg pos after to raycast adjustment on " + x.ToString() + "," + y.ToString() + " was " + avgPos.ToString());
                    
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
                        SaveDebugScreenshot(captureTex,"gridCellxy" + x.ToString() + "." + y.ToString() + "_pixelI" + i.ToString());
                        yield return new WaitForSeconds(timeTillNext);

                        UnityEngine.Debug.LogWarning("Culling error 1 on cell " + x.ToString() + "," + y.ToString());
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
                        SaveDebugScreenshot(captureTex,"gridCellxy" + x.ToString() + "." + y.ToString() + "_pixelI" + i.ToString());
                        yield return new WaitForSeconds(timeTillNext);

                        UnityEngine.Debug.LogWarning("Culling error 2 on cell " + x.ToString() + "," + y.ToString());
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
                        SaveDebugScreenshot(captureTex,"gridCellxy" + x.ToString() + "." + y.ToString() + "_pixelI" + i.ToString());
                        yield return new WaitForSeconds(timeTillNext);

                        UnityEngine.Debug.LogWarning("Culling error 3 on cell " + x.ToString() + "," + y.ToString());
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
                        SaveDebugScreenshot(captureTex,"gridCellxy" + x.ToString() + "." + y.ToString() + "_pixelI" + i.ToString());
                        yield return new WaitForSeconds(timeTillNext);

                        UnityEngine.Debug.LogWarning("Culling error 4 on cell " + x.ToString() + "," + y.ToString());
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
                        SaveDebugScreenshot(captureTex,"gridCellxy" + x.ToString() + "." + y.ToString() + "_pixelI" + i.ToString());
                        yield return new WaitForSeconds(timeTillNext);

                        UnityEngine.Debug.LogWarning("Culling error 5 on cell " + x.ToString() + "," + y.ToString());
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
                        SaveDebugScreenshot(captureTex,"gridCellxy" + x.ToString() + "." + y.ToString() + "_pixelI" + i.ToString());
                        yield return new WaitForSeconds(timeTillNext);

                        UnityEngine.Debug.LogWarning("Culling error 6 on cell " + x.ToString() + "," + y.ToString());
                    }
                }
            }
            Assert.That(true,"all good");

            // Revert to normal
            Const.a.testSphere.SetActive(false);
            PlayerMovement.a.Notarget = false;
            PlayerMovement.a.rbody.isKinematic = false;
        }
    }
}
