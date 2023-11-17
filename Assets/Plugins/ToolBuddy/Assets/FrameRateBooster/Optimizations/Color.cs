
namespace ToolBuddy.FrameRateBooster.Optimizations
{
    internal struct Color
    {
        public float r;
        public float g;
        public float b;
        public float a;

        static public Color get_black()
        {
            Color result;
            result.r = 0.0f;
            result.g = 0.0f;
            result.b = 0.0f;
            result.a = 1f;
            return result;
        }

        static public Color get_blue()
        {
            Color result;
            result.r = 0.0f;
            result.g = 0.0f;
            result.b = 1f;
            result.a = 1f;
            return result;
        }

        static public Color get_clear()
        {
            Color result;
            result.r = 0.0f;
            result.g = 0.0f;
            result.b = 0.0f;
            result.a = 0.0f;
            return result;
        }

        static public Color get_cyan()
        {
            Color result;
            result.r = 0.0f;
            result.g = 1f;
            result.b = 1f;
            result.a = 1f;
            return result;
        }

        static public Color get_gray()
        {
            Color result;
            result.r = 0.5f;
            result.g = 0.5f;
            result.b = 0.5f;
            result.a = 1f;
            return result;
        }

        static public Color get_green()
        {
            Color result;
            result.r = 0.0f;
            result.g = 1f;
            result.b = 0.0f;
            result.a = 1f;
            return result;
        }

        static public Color get_grey()
        {
            Color result;
            result.r = 0.5f;
            result.g = 0.5f;
            result.b = 0.5f;
            result.a = 1f;
            return result;
        }

        static public Color get_magenta()
        {
            Color result;
            result.r = 1f;
            result.g = 0.0f;
            result.b = 1f;
            result.a = 1f;
            return result;
        }

        static public Color get_red()
        {
            Color result;
            result.r = 1f;
            result.g = 0.0f;
            result.b = 0.0f;
            result.a = 1f;
            return result;
        }

        static public Color get_white()
        {
            Color result;
            result.r = 1f;
            result.g = 1f;
            result.b = 1f;
            result.a = 1f;
            return result;
        }

        static public Color get_yellow()
        {
            Color result;
            result.r = 1f;
            result.g = 0.9215686f;
            result.b = 0.01568628f;
            result.a = 1f;
            return result;
        }

        static public Color operator +(Color a, Color b)
        {
            a.r = a.r + b.r;
            a.g = a.g + b.g;
            a.b = a.b + b.b;
            a.a = a.a + b.a;
            return a;
        }

        static public Color operator /(Color a, float b)
        {
            float inverseb = 1 / b;
            a.r *= inverseb;
            a.g *= inverseb;
            a.b *= inverseb;
            a.a *= inverseb;
            return a;
        }

        ////TODO test
        //public Color get_linear()
        //{
        //    Color result;
        //    result.r = Mathf.GammaToLinearSpace(r);
        //    result.g = Mathf.GammaToLinearSpace(g);
        //    result.b = Mathf.GammaToLinearSpace(b);
        //    result.a = a;
        //    return result;
        //}

        ////TODO test
        //public Color get_gamma()
        //{
        //    Color result;
        //    result.r = Mathf.LinearToGammaSpace(r);
        //    result.g = Mathf.LinearToGammaSpace(g);
        //    result.b = Mathf.LinearToGammaSpace(b);
        //    result.a = a;
        //    return result;
        //}


        static public implicit operator Vector4(Color c)
        {
            Vector4 result;
            result.x = c.r;
            result.y = c.g;
            result.z = c.b;
            result.w = c.a;
            return result;
        }

        static public implicit operator Color(Vector4 v)
        {
            Color result;
            result.r = v.x;
            result.g = v.y;
            result.b = v.z;
            result.a = v.w;
            return result;
        }

        static public Color operator *(Color a, Color b)
        {
            a.r *= b.r;
            a.g *= b.g;
            a.b *= b.b;
            a.a *= b.a;
            return a;
        }

        static public Color operator *(Color a, float b)
        {
            a.r *= b;
            a.g *= b;
            a.b *= b;
            a.a *= b;
            return a;
        }

        static public Color operator *(float b, Color a)
        {
            a.r *= b;
            a.g *= b;
            a.b *= b;
            a.a *= b;
            return a;
        }

        static public Color operator -(Color a, Color b)
        {
            a.r -= b.r;
            a.g -= b.g;
            a.b -= b.b;
            a.a -= b.a;
            return a;
        }


        public static Color LerpUnclamped(Color a, Color b, float t)
        {
            a.r += (b.r - a.r) * t;
            a.g += (b.g - a.g) * t;
            a.b += (b.b - a.b) * t;
            a.a += (b.a - a.a) * t;
            return a;
        }

        internal Color RGBMultiplied(float multiplier)
        {
            Color result;
            result.r = this.r * multiplier;
            result.g = this.g * multiplier;
            result.b = this.b * multiplier;
            result.a = this.a;
            return result;
        }

        internal Color AlphaMultiplied(float multiplier)
        {
            Color result;
            result.r = this.r;
            result.g = this.g;
            result.b = this.b;
            result.a = this.a * multiplier;
            return result;
        }

        internal Color RGBMultiplied(Color multiplier)
        {
            multiplier.r = this.r * multiplier.r;
            multiplier.g = this.g * multiplier.g;
            multiplier.b = this.b * multiplier.b;
            multiplier.a = this.a;
            return multiplier;
        }
    }
}