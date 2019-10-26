using System.Collections;
using UnityEngine;

namespace DTValidator.Internal {
	public class MathUtil {
		public const double epsilon = 0.0001;

		/// <summary>
		/// Pass in the relative position between two points, get QuadInOut (ref: easing.net)
		/// </summary>
		/// <param name="a">Start Point</param>
		/// <param name="b">End Point</param>
		/// <param name="r">0.0...1.0, Relative Position</param>
		public static float QuadInOut(float a, float b, float r) {
			if (r < 0.0f || r > 1.0f) {
				Debug.LogError("Invalid r (" + r + ") value for QuadInOut!");
				return 0.0f;
			}

			float m = (a + b) / 2.0f;
			r *= 2.0f;
			if (r < 1.0f) {
				return a + (Mathf.Pow(r, 2.0f) * (m - a));
			} else {
				return m + (Mathf.Pow((r - 1.0f), 1.0f / 2.0f) * (b - m));
			}
		}

		/// <summary>
		/// Returns a gaussian distributed value centered around mean and with a
		/// standard deviation curve
		/// </summary>
		public static float SampleGaussian(float mean, float standardDeviation) {
			float min = mean - (3.5f * standardDeviation);
			float max = mean + (3.5f * standardDeviation);

			float val;
			do {
				val = mean + (SampleNormalizedGaussian() * standardDeviation);
			} while (val < min || val > max);

			return val;
		}

		/// <summary>
		/// Returns a normalized gaussian value (0.0 --- 1.0 (one standardDeviation) ---- ...)
		/// </summary>
		public static float SampleNormalizedGaussian() {
			float v1, v2, s;
			do {
				v1 = 2.0f * Random.Range(0.0f, 1.0f) - 1.0f;
				v2 = 2.0f * Random.Range(0.0f, 1.0f) - 1.0f;
				s = (v1 * v1) + (v2 * v2);
			} while (s >= 1.0f || s == 0.0f);

			s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

			return v1 * s;
		}

		public static float RandomSign() {
			return Random.Range(0.0f, 1.0f) > 0.5f ? 1.0f : -1.0f;
		}

		/// <summary>
		/// Wraps a value between the range of min and max
		/// </summary>
		public static int Wrap(int input, int min, int max) {
			int newVal = input % max;
			if (newVal < min) {
				newVal += max;
			}
			return newVal;
		}

		public static int Clamp(int val, int min, int max) {
			if (val > max) {
				return max;
			} else if (val < min) {
				return min;
			} else {
				return val;
			}
		}

		/// <summary>
		/// Evaluates a sine function with the given amplitude, wavelength, and offset
		/// </summary>
		public static float EvaluateSine(float val, float amplitude, float wavelength, float offset = 0.0f) {
			if (wavelength == 0) {
				wavelength = 1.0f;
			}
			return amplitude * Mathf.Sin((val * 2.0f * Mathf.PI) / wavelength) + offset;
		}

		/// <summary>
		/// Example: RoundToDecimalPlace(12.345f, 1) == 12.3f
		/// </summary>
		public static float RoundToDecimalPlace(float val, int decimalPlace) {
			float conversion = Mathf.Pow(10.0f, decimalPlace);
			float raisedValue = val * conversion;
			return Mathf.Round(raisedValue) / conversion;
		}

		public static byte ConvertToByte(float f) {
			f = Mathf.Clamp01(f);
			return (byte)(f * 255);
		}
	}
}