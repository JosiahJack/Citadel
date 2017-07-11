using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

[RequireComponent (typeof (ColorCorrectionCurves))]
[ExecuteInEditMode]
[AddComponentMenu ("Image Effects/Color Adjustments/Dynamic Color Correction (Curves, Saturation)")]
/*
* Color Curves Manager. A script to manage the Unity 5 Color Correction Curves script, found among the standard assets.
* Color Curves Manager allows to set two set of curves / configurations and dynamically interpolate between each set.
*/
public class ColorCurvesManager : MonoBehaviour
{

	public float Factor = 0;

	// A configuration
	public float SaturationA = 1;
	public AnimationCurve RedA = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));
	public AnimationCurve GreenA = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));
	public AnimationCurve BlueA = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));

	public AnimationCurve RedADepth = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));
	public AnimationCurve GreenADepth = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));
	public AnimationCurve BlueADepth = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));

	public AnimationCurve ZCurveA = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));

	public Color SelectiveFromColorA = Color.white;
	public Color SelectiveToColorA = Color.white;

	// B configuration
	public float SaturationB = 1;
	public AnimationCurve RedB = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));
	public AnimationCurve GreenB = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));
	public AnimationCurve BlueB = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));

	public AnimationCurve RedBDepth = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));
	public AnimationCurve GreenBDepth = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));
	public AnimationCurve BlueBDepth = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));

	public AnimationCurve ZCurveB = new AnimationCurve(new Keyframe(0f,0f), new Keyframe(1f,1f));

	public Color SelectiveFromColorB = Color.white;
	public Color SelectiveToColorB = Color.white;

	// Logic
	private List<Keyframe[]> RedPairedKeyframes;
	private List<Keyframe[]> GreenPairedKeyframes;
	private List<Keyframe[]> BluePairedKeyframes;
	private List<Keyframe[]> DepthRedPairedKeyframes;
	private List<Keyframe[]> DepthGreenPairedKeyframes;
	private List<Keyframe[]> DepthBluePairedKeyframes;
	private List<Keyframe[]> ZCurvePairedKeyframes;

	private ColorCorrectionCurves CurvesScript;

	private const float PAIRING_DISTANCE = 0.01f;
	private const float TANGENT_DISTANCE = 0.0012f;

	/*These properties are necessary to detect changes in variables by code*/
	private bool ChangesInEditor = true;
	private float LastFactor;
	private float LastSaturationA;
	private float LastSaturationB;

	/*These function are necessary to acces public values from some UI components.*/
	public void SetFactor(float factor) {Factor = factor;}
	public void SetSaturationA(float saturationA) {SaturationA = saturationA;}
	public void SetSaturationB(float saturationB) {SaturationB = saturationB;}

	void Start()
	{
		LastFactor = Factor;
		LastSaturationA = SaturationA;
		LastSaturationB = SaturationB;

		CurvesScript = GetComponent<ColorCorrectionCurves>();

		PairCurvesKeyframes();
	}

	void Update()
	{ 
		UpdateScript();
	}

	
	private void UpdateScript()
	{

		if (!PairedListsInitiated())
			PairCurvesKeyframes();

		//If parameters has changed from the editor
		if(ChangesInEditor)
		{
			PairCurvesKeyframes();//The curves could have been changed in the editor.
			UpdateScriptParameters();
			
			CurvesScript.UpdateParameters();
			
			ChangesInEditor = false;
		}
		//If parameters has changed from another script
		else if (Factor != LastFactor || SaturationA != LastSaturationA || SaturationB != LastSaturationB)
		{
			UpdateScriptParameters();
			CurvesScript.UpdateParameters();

			LastFactor = Factor;
			LastSaturationA = SaturationA;
			LastSaturationB = SaturationB;
		}

	}

	void EditorHasChanged ()
	{
		ChangesInEditor = true;
		UpdateScript();
	}

	/*
	 *Given two curves, this function returns a list of paired keyframes. Each pair represent the same keyframe in both curves,
	 *so the script can interpolate between those two smoothly. To determine if two keyframes in each curve actually represent
	 *the same keyframe, and thus has to be paired in the output list, the fields PAIRING_DISTANCE and are used. If a keyframe
	 *in one curve does not have a pair in the other curve, a new keyframe is created. To calculate the tangents of this new
	 *keyframe, TANGENT_DISTANCE is used.
	 */
	public static List<Keyframe[]> PairKeyframes(AnimationCurve curveA, AnimationCurve curveB)
	{
		//If both curves has the same ammounts of keyframes, pairing is pretty straitfordward.
		if (curveA.length == curveB.length)
			return SimplePairKeyframes(curveA, curveB);

		//Else, we have to pair closest points and create new points.
		var pairList = new List<Keyframe[]>();//TODO

		//Make two temporal list with all the points in both curves.
		List<Keyframe> tempA = new List<Keyframe>();
		List<Keyframe> tempB = new List<Keyframe>();

		tempA.AddRange(curveA.keys);
		tempB.AddRange(curveB.keys);

		//Iterate through one list finding pairs in the other
		int i = 0 ; 
		while (i < tempA.Count)
		{
			Keyframe aKeyframe = tempA[i];
			int pairIndex = tempB.FindIndex(bKeyframe => (Mathf.Abs(aKeyframe.time - bKeyframe.time) < PAIRING_DISTANCE));

			if(pairIndex >= 0)
			{
				//The point i has a pair. Let add both to the list and remove them from the temp lists.
				Keyframe[] pair = new Keyframe[] {tempA[i], tempB[pairIndex]};
				pairList.Add(pair);
				tempA.RemoveAt(i);
				tempB.RemoveAt(pairIndex);
			}
			else
			{
				//No pair found for this keyframe. Let's move on.
				i++;
			}
		}

		//Create new keyframes for the keyframes that has no pairs in the other curve.
		foreach(Keyframe k in tempA)
		{
			Keyframe newPair = CreatePair(k, curveB);
			pairList.Add(new Keyframe[] {k, newPair});
		}

		foreach(Keyframe k in tempB)
		{
			Keyframe newPair = CreatePair(k, curveA);
			pairList.Add(new Keyframe[] {newPair,k});
		}

		return pairList;
	}

	private static List<Keyframe[]> SimplePairKeyframes(AnimationCurve curveA, AnimationCurve curveB)
	{
		
		var pairList = new List<Keyframe[]>();//TODO
		
		if (curveA.length != curveB.length)
		{
			throw new UnityException("Simple Pair cannot work with curves with different number of Keyframes.");
		}
		
		for(int i = 0; i< curveA.length; i++)
		{
			pairList.Add(new Keyframe[] {curveA.keys[i], curveB.keys[i]});
		}
		
		return pairList;
	}

	private static Keyframe CreatePair(Keyframe kf, AnimationCurve curve)
	{
		Keyframe newPair = new Keyframe();

		newPair.time = kf.time;
		newPair.value = curve.Evaluate(kf.time);

		if(kf.time >= TANGENT_DISTANCE)
		{
			float x = kf.time - TANGENT_DISTANCE;

			newPair.inTangent = (curve.Evaluate(x) - curve.Evaluate(kf.time)) / (x - kf.time);
		}

		if(kf.time + TANGENT_DISTANCE <= 1)
		{
			float x = kf.time + TANGENT_DISTANCE;
			
			newPair.outTangent = (curve.Evaluate(x) - curve.Evaluate(kf.time)) / (x - kf.time);
		}

		return newPair;
	}

	/*
	 *Create a new curve, interpolating between keyframes defined in the paired keyframe list, pondering by factor.
	 */
	public static AnimationCurve CreateCurveFromKeyframes(IList<Keyframe[]> keyframePairs, float factor)
	{
		Keyframe[] finalFrames = new Keyframe[keyframePairs.Count];
		
		for(int i = 0; i< keyframePairs.Count; i++)
		{
			Keyframe[] pair = keyframePairs[i];
			finalFrames[i] = AverageKeyframe(pair[0],pair[1],factor);
		}
		
		return new AnimationCurve(finalFrames);
	}

	public static Keyframe AverageKeyframe(Keyframe a, Keyframe b, float factor)
	{
		Keyframe frame = new Keyframe();
		frame.time = a.time*(1-factor) + b.time*factor;
		frame.value = a.value*(1-factor) + b.value*factor;
		frame.inTangent = (a.inTangent*(1-factor) + b.inTangent*factor);
		frame.outTangent = (a.outTangent*(1-factor) + b.outTangent*factor);
		
		return frame;
	}
		
	private void PairCurvesKeyframes()
	{
		RedPairedKeyframes = PairKeyframes(RedA, RedB);
		GreenPairedKeyframes = PairKeyframes(GreenA, GreenB);
		BluePairedKeyframes = PairKeyframes(BlueA, BlueB);

		if(ScriptAdvancedMode() || !PairedListsInitiated())
		{
			DepthRedPairedKeyframes = PairKeyframes(RedADepth,RedBDepth);
			DepthGreenPairedKeyframes = PairKeyframes(GreenADepth,GreenBDepth);
			DepthBluePairedKeyframes = PairKeyframes(BlueADepth,BlueBDepth);

			ZCurvePairedKeyframes = PairKeyframes(ZCurveA,ZCurveB);
		}
	}

	private void UpdateScriptParameters()
	{
		Factor = Mathf.Clamp01(Factor);

		SaturationA = Mathf.Clamp(SaturationA, 0f, 5f);
		SaturationB = Mathf.Clamp(SaturationB, 0f, 5f);

		CurvesScript.saturation = Mathf.Lerp(SaturationA, SaturationB, Factor);
		CurvesScript.redChannel = CreateCurveFromKeyframes(RedPairedKeyframes, Factor);
		CurvesScript.greenChannel = CreateCurveFromKeyframes(GreenPairedKeyframes, Factor);
		CurvesScript.blueChannel = CreateCurveFromKeyframes(BluePairedKeyframes, Factor);

		if(ScriptAdvancedMode())
		{
			CurvesScript.depthRedChannel = CreateCurveFromKeyframes(DepthRedPairedKeyframes, Factor);
			CurvesScript.depthGreenChannel = CreateCurveFromKeyframes(DepthGreenPairedKeyframes, Factor);
			CurvesScript.depthBlueChannel = CreateCurveFromKeyframes(DepthBluePairedKeyframes, Factor);

			CurvesScript.zCurve = CreateCurveFromKeyframes(ZCurvePairedKeyframes, Factor);
		}

		if (ScriptSelective())
		{
			CurvesScript.selectiveFromColor = Color.Lerp(SelectiveFromColorA,SelectiveFromColorB,Factor);
			CurvesScript.selectiveToColor = Color.Lerp(SelectiveToColorA,SelectiveToColorB,Factor);
		}
	}

	private bool PairedListsInitiated()
	{
		return !(RedPairedKeyframes == null || GreenPairedKeyframes == null || BluePairedKeyframes == null
		         || DepthRedPairedKeyframes == null || DepthGreenPairedKeyframes == null || DepthBluePairedKeyframes == null);
	}

	/*Way to observe the script configuration*/
	public bool ScriptAdvancedMode()
	{return CurvesScript.mode == (ColorCorrectionCurves.ColorCorrectionMode.Advanced);
	}
	public bool ScriptSelective()
	{return CurvesScript.selectiveCc;
	}
}