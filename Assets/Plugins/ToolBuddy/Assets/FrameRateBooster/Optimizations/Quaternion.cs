namespace ToolBuddy.FrameRateBooster.Optimizations
{
    public struct Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public static Quaternion op_Multiply(Quaternion lhs, Quaternion rhs)
        {
            //TODO why the Unity's code has a double conversion but the build assembly don't have it?
            Quaternion result;
            result.x = (lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y);
            result.y = (lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z);
            result.z = (lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x);
            result.w = (lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
            return result;
        }
    }
}
