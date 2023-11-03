using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShadowsIn2D.Visibility
{
    internal class EndPointComparer : IComparer<EndPoint>
    {
        internal EndPointComparer() { }
        
        // Helper: comparison function for sorting points by angle
        public int Compare(EndPoint a, EndPoint b)
        {          
            // Traverse in angle order
            if (a.Angle > b.Angle) { return 1; }
            if (a.Angle < b.Angle) { return -1; }
            // But for ties we want Begin nodes before End nodes
            if (!a.Begin && b.Begin) { return 1; }
            if (a.Begin && !b.Begin) { return -1; }

            return 0;
        }
    }
}
