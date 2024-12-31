using UnityEngine;

namespace CSM.Runtime.Utilities {
  /// <summary>
  ///
  /// </summary>
  public static class ShadowMapChecking {
    /// <summary>
    ///
    /// </summary>
    /// <param name="rt"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static bool SanityCheck(RenderTexture rt, int width = 1, int height = 1) {
      // Fetch its pixels and set it to rgb texture
      var rgb_tex = new Texture2D(width : width, height : height);

      RenderTexture.active = rt;
      rgb_tex.ReadPixels(source : new Rect(0,
                                           0,
                                           width : width,
                                           height : height),
                         0,
                         0);
      rgb_tex.Apply();
      RenderTexture.active = null;

      var c = rgb_tex.GetPixels32();

      return c[0].a > 0;
    }
  }
}