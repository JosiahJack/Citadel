using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BiomonitorGraphSystem : MonoBehaviour {
    public float widthPerc = 0.4f;
    public float heightPerc = 0.1f;
    public RawImage OutputTexture;

	// Internal refernces
    private float[] max; // Value at the top of the graph
    private float[] min; // Value at the bottom of the graph
    private Color[] currentColors;
    private Color[][] colorsERG;
    private Color[][] colorsCHI;
    private Color[][] colorsECG;
    private int lastERG;
    private int lastCHI;
    private int lastECG;
    private Color backgroundColor = new Color(0.2f, 0.2f, 1f, 0.01f);
    private int graphWidth = 620;
    private int graphHeight = 36;
    private Color ergColor = new Color(0.0f, 0.5f, 1f, 1f);
    private Color chiColor = new Color(0.7f, 0.0f, 1f, 1f);
    private Color ecgColor = new Color(1.0f, 0.0f, 0f, 1f);
    private Texture2D tex;
    private Color col;
    private int ymax = 36;
	private float ecgValue = 0f;
	private float ergValue = 0f;
	private float chiValue = 0f;
	private float beatShift;
	private float beatThresh = 0.1f;
	private float beatVariation = 0.05f;
	private float beatFinished;
	private float freq = 35f;
    private float tick0Finished;
    private float tick1Finished;
    private float tick2Finished;
    private float tickFinished; // Overall marching.
    private float tick0 = 0.0211f;
    private float tick1 = 0.050f;
    private float tick2 = 0.0104f;
    private float tick = 0.02f;
    public int currentIndex0 = 0;
    public int currentIndex1 = 0;
    public int currentIndex2 = 0;
    private float distPerc;
    private float fadeDist;
    private Color col0;
    private Color col1;
    private Color col2;
	private float graphAdd = 20f;
	private float fatigueFactor = 0f;

	// Singleton instance
    public static BiomonitorGraphSystem a; // Ensure an instance is present

    void Awake() { a = this; }

    void Start() {
        min = new float[]{0f,-2f,-1f};
        max = new float[]{1f, 2f, 1f};
        OutputTexture.texture = (Texture)tex;
        ClearGraphs();
    }

    void OnEnable() {
        ClearGraphs();
    }

    public void ClearGraphs() {
        graphWidth = (int)((float)Screen.width * widthPerc);
        graphHeight = (int)((float)Screen.height * heightPerc);
        tex = new Texture2D(graphWidth,graphHeight);
        for (int x=0;x<graphWidth;x++) {
            for (int y=0; y<graphHeight;y++) tex.SetPixel(x,y,backgroundColor);
        }
        currentColors = new Color[graphHeight];
        for (int y=0;y<graphHeight;y++) currentColors[y] = backgroundColor;

        ymax = (currentColors.Length - 1);
		beatFinished = PauseScript.a.relativeTime;
        tick0Finished = PauseScript.a.relativeTime + tick0;
        tick1Finished = PauseScript.a.relativeTime + tick1;
        tick2Finished = PauseScript.a.relativeTime + tick2;
        tickFinished = PauseScript.a.relativeTime + tick;
        currentIndex0 = (int)(graphWidth * UnityEngine.Random.Range(0f,1f));
        currentIndex1 = (int)(graphWidth * UnityEngine.Random.Range(0f,1f));
        currentIndex2 = (int)(graphWidth * UnityEngine.Random.Range(0f,1f));
        colorsERG = new Color[graphWidth][];
        colorsCHI = new Color[graphWidth][];
        colorsECG = new Color[graphWidth][];
        for (int x=0;x<graphWidth;x++) {
            colorsERG[x] = new Color[graphHeight];
            colorsCHI[x] = new Color[graphHeight];
            colorsECG[x] = new Color[graphHeight];
            for (int y=0; y<graphHeight;y++) {
                colorsERG[x][y] = backgroundColor;
                colorsCHI[x][y] = backgroundColor;
                colorsECG[x][y] = backgroundColor;
            }
        }
    }

    public void IncrementERG() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

        currentIndex0++;
        if (currentIndex0 >= graphWidth) currentIndex0 = 0;
    }

    public void IncrementCHI() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

        currentIndex1++;
        if (currentIndex1 >= graphWidth) currentIndex1 = 0;
    }

    public void IncrementECG() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

        currentIndex2++;
        if (currentIndex2 >= graphWidth) currentIndex2 = 0;
    }

    public void Update() {
		if (PauseScript.a.Paused()) return;
		if (PauseScript.a.MenuActive()) return;

		// Energy Usage
		ergValue = (PlayerEnergy.a.drainJPM / 255f);
		if (ergValue < 0f) ergValue = 0f;
	    if (ergValue > 1f) ergValue = 1f;

		// Chi Brain Waves
        float brainFactor = 0.15f;
        if (PlayerPatch.a.geniusFinishedTime > PauseScript.a.relativeTime) {
            brainFactor = 0.35f + UnityEngine.Random.Range(-0.3f,0.3f);
        }

		chiValue = Mathf.Sin(PauseScript.a.relativeTime * 10f * brainFactor);

		// ECG: Create shifted sine wave for heart beat.
		// Apply percent fatigued to 200bpm max heart rate with baseline 50bpm.
		fatigueFactor = ((PlayerMovement.a.fatigue / 100f) * 120f) + graphAdd;
        fatigueFactor = fatigueFactor / 60f;
		if (beatFinished < PauseScript.a.relativeTime) {
            beatFinished = PauseScript.a.relativeTime + (1f/fatigueFactor);
        }

		beatShift = (beatFinished - PauseScript.a.relativeTime)
                    / (1f/fatigueFactor);
		if (beatShift > 0.94f) ecgValue = Mathf.Sin(beatShift * freq);
		else ecgValue = 0;

		 // Inject variation when beating
		if (ecgValue > beatThresh || ecgValue < (beatThresh * -1f)) {
			ecgValue += UnityEngine.Random.Range(-beatVariation,beatVariation);
		}

        if (tick0Finished < PauseScript.a.relativeTime) {
            tick0Finished = PauseScript.a.relativeTime + tick0;
            Push(0,ergValue);
            IncrementERG();
            Push(0,ergValue);
            IncrementERG();
            Push(0,ergValue);
        }

        if (tick1Finished < PauseScript.a.relativeTime) {
            tick1Finished = PauseScript.a.relativeTime + tick1;
            Push(1,chiValue);
            IncrementCHI();
            Push(1,chiValue);
            IncrementCHI();
            Push(1,chiValue);
            IncrementCHI();
            Push(1,chiValue);
        }

        if (tick2Finished < PauseScript.a.relativeTime) {
            tick2Finished = PauseScript.a.relativeTime + tick2;
            Push(2,ecgValue);
            IncrementECG();
            Push(2,ecgValue);
        }

        distPerc = 1f;
        fadeDist = 100f;
        for (int x=0;x<graphWidth;x++) {
            for (int y=0; y<graphHeight;y++) {
                col0 = colorsERG[x][y];
                col1 = colorsCHI[x][y];
                col2 = colorsECG[x][y];
                if (col0.a > 0.01f) {
                    fadeDist = 200f;
                    distPerc = (currentIndex0 - x);
                    if ((graphWidth - x) < fadeDist
                        && currentIndex0 < fadeDist) {
                        distPerc +=  graphWidth;
                    }

                    if (distPerc < 0f) distPerc = fadeDist;
                    else if (distPerc > fadeDist) distPerc = fadeDist;

                    distPerc = (fadeDist - distPerc) / fadeDist;
                    if (distPerc > 1f) distPerc = 1f;
                    else if (distPerc < 0f) distPerc = 0f;

                    if (distPerc == 0f) colorsERG[x][y] = backgroundColor;
                    col0.a = distPerc;
                    tex.SetPixel(x,y,col0);
                } else if (col1.a > 0.01f) {
                    fadeDist = 180f;
                    distPerc = (currentIndex1 - x);
                    if ((graphWidth - x) < fadeDist
                        && currentIndex1 < fadeDist) {
                        distPerc +=  graphWidth;
                    }

                    if (distPerc < 0f) distPerc = fadeDist;
                    else if (distPerc > fadeDist) distPerc = fadeDist;

                    distPerc = (fadeDist - distPerc) / fadeDist;
                    if (distPerc > 1f) distPerc = 1f;
                    else if (distPerc < 0f) distPerc = 0f;

                    if (distPerc == 0f) colorsCHI[x][y] = backgroundColor;
                    col1.a = distPerc;
                    tex.SetPixel(x,y,col1);
                } else if (col2.a > 0.01f) {
                    fadeDist = 275f;
                    distPerc = (currentIndex2 - x);
                    if ((graphWidth - x) < fadeDist
                        && currentIndex2 < fadeDist) {
                        distPerc +=  graphWidth;
                    }

                    if (distPerc < 0f) distPerc = fadeDist;
                    else if (distPerc > fadeDist) distPerc = fadeDist;

                    distPerc = (fadeDist - distPerc) / fadeDist;
                    if (distPerc > 1f) distPerc = 1f;
                    else if (distPerc < 0f) distPerc = 0f;

                    if (distPerc == 0f) colorsECG[x][y] = backgroundColor;
                    col2.a = distPerc;
                    tex.SetPixel(x,y,col2);
                } else {
                    tex.SetPixel(x,y,backgroundColor);
                }
            }
        }

        if (tickFinished < PauseScript.a.relativeTime) {
            tickFinished = PauseScript.a.relativeTime + tick;
            IncrementERG();
            IncrementCHI();
            IncrementECG();
        }

        tex.Apply();
        OutputTexture.texture = (Texture)tex;
    }

    public void EnergyPulse(float take) {
		Push(0,take);
		IncrementERG();
		Push(0,take);
		IncrementERG();
    }

    // Add a data point to the beginning of the graph
    public void Push(int index, float val) {
        if (currentColors.Length < 1) Start();

        float value = 0f;
        int dist = 1;
        int y0 = 0;
        bool down = false;
        for (int y=0;y<currentColors.Length;y++) {
            currentColors[y] = backgroundColor;
            switch(index) {
                case 0: colorsERG[currentIndex0][y] = backgroundColor; break;
                case 1: colorsCHI[currentIndex1][y] = backgroundColor; break;
                case 2: colorsECG[currentIndex2][y] = backgroundColor; break;
            }
        }

        switch(index) {
            case 0:
                value = Mathf.InverseLerp(min[0],max[0],val);
                y0 = (int)(value * graphHeight);
                if (y0 > ymax) y0 = ymax;
                if (y0 < 0) y0 = 0;
                currentColors[y0] = ergColor;
                colorsERG[currentIndex0][y0] = ergColor;
                if (Mathf.Abs(lastERG - y0) > 2) {
                    dist = lastERG;
                    if (lastERG > y0) {
                        dist = lastERG - 1;
                        down = true;
                    } else {
                        dist = lastERG + 1;
                        down = false;
                    }

                    while (dist != y0) {
                        if (dist > ymax || dist < 0) break;

                        currentColors[dist] = ergColor;
                        colorsERG[currentIndex0][dist] = ergColor;
                        if (down) dist--;
                        else dist++;
                    }
                }
                lastERG = y0;
                if (y0 > 0 && y0 < ymax) { // Increase thickness to 2 pixels
                    currentColors[y0 + 1] = ergColor;
                    colorsERG[currentIndex0][y0 + 1] = ergColor;
                }
                break;
            case 1:
                value = Mathf.InverseLerp(min[1],max[1],val);
                y0 = (int)(value * graphHeight);
                if (y0 > ymax) y0 = ymax;
                if (y0 < 0) y0 = 0;
                currentColors[y0] = chiColor;
                colorsCHI[currentIndex1][y0] = chiColor;
                if (Mathf.Abs(lastCHI - y0) > 2) {
                    dist = lastCHI;
                    if (lastCHI > y0) {
                        dist = lastCHI - 1;
                        down = true;
                    } else {
                        dist = lastCHI + 1;
                        down = false;
                    }

                    while (dist != y0) {
                        if (dist > ymax || dist < 0) break;

                        currentColors[dist] = chiColor;
                        colorsCHI[currentIndex1][dist] = chiColor;
                        if (down) dist--;
                        else dist++;
                    }
                }
                lastCHI = y0;
                if (y0 > 0 && y0 < ymax) { // Increase thickness to 3 pixels
                    currentColors[y0 - 1] = chiColor;
                    currentColors[y0 + 1] = chiColor;
                    colorsCHI[currentIndex1][y0 + 1] = chiColor;
                }
                break;
            case 2:
                value = Mathf.InverseLerp(min[2],max[2],val);
                y0 = (int)(value * graphHeight);
                if (y0 > ymax) y0 = ymax;
                if (y0 < 0) y0 = 0;
                currentColors[y0] = ecgColor;
                colorsECG[currentIndex2][y0] = ecgColor;
                if (Mathf.Abs(lastECG - y0) > 2) {
                    dist = lastECG;
                    if (lastECG > y0) {
                        dist = lastECG - 1;
                        down = true;
                    } else {
                        dist = lastECG + 1;
                        down = false;
                    }

                    while (dist != y0) {
                        if (dist > ymax || dist < 0) break;

                        currentColors[dist] = ecgColor;
                        colorsECG[currentIndex2][dist] = ecgColor;
                        if (down) dist--;
                        else dist++;
                    }
                }
                lastECG = y0;
                int half = (int)((max[2] - min[2]) / 2f);

                // Increase thickness to 3 pixels
                if (y0 > 0 && y0 < ymax && (Mathf.Abs(y0 - half) > 2)) {
                    currentColors[y0 - 1] = ecgColor;
                    currentColors[y0 + 1] = ecgColor;
                    colorsECG[currentIndex2][y0 - 1] = ecgColor;
                    colorsECG[currentIndex2][y0 + 1] = ecgColor;
                }
                break;
        }
     }
}