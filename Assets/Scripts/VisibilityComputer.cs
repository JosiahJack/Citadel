using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// From https://roy-t.nl/2014/02/27/2d-lighting-and-shadows-preview
namespace ShadowsIn2D.Visibility {
    public class VisibilityComputer {
        // These represent the map and light location:        
        private List<EndPoint> endpoints;
        private List<Segment> segments;       
                
        private Vector2 origin;
        /// <summary>
        /// The origin, or position of the observer
        /// </summary>
        public Vector2 Origin { get { return origin; } }
        private EndPointComparer radialComparer;

        public VisibilityComputer(Vector2 origin)
        {
            segments = new List<Segment>();
            endpoints = new List<EndPoint>();
            radialComparer = new EndPointComparer();

            this.origin = origin;
            LoadBoundaries();
        }       

        public void AddSquareOccluder(Vector2 position) {
            Vector2[] corners = new Vector2[4];
            corners[0] = new Vector2(position.x + 0.5f,position.y + 0.5f);
            corners[1] = new Vector2(position.x - 0.5f,position.y + 0.5f);
            corners[2] = new Vector2(position.x + 0.5f,position.y - 0.5f);
            corners[3] = new Vector2(position.x - 0.5f,position.y - 0.5f);
            AddSegment(corners[0], corners[1]);
            AddSegment(corners[1], corners[2]);
            AddSegment(corners[2], corners[3]);
            AddSegment(corners[3], corners[0]);
        }            

        // Add a segment, where the first point shows up in the
        // visualization but the second one does not. (Every endpoint is
        // part of two segments, but we want to only show them once.)
        private void AddSegment(Vector2 p1, Vector2 p2)
        {
            Segment segment = new Segment();
            EndPoint endPoint1 = new EndPoint();
            EndPoint endPoint2 = new EndPoint();

            endPoint1.Position = p1;
            endPoint1.Segment = segment;

            endPoint2.Position = p2;
            endPoint2.Segment = segment;

            segment.P1 = endPoint1;
            segment.P2 = endPoint2;

            segments.Add(segment);
            endpoints.Add(endPoint1);
            endpoints.Add(endPoint2);
        }

        /// <summary>
        /// Remove all occluders
        /// </summary>
        public void ClearOccluders()
        {
            segments.Clear();
            endpoints.Clear();

            LoadBoundaries();
        }

        /// <summary>
        /// Helper function to construct segments along the outside perimiter
        /// in order to limit the radius of sight
        /// </summary>        
        private void LoadBoundaries() {
            float radius = 32f;

            //Top
            AddSegment(new Vector2(origin.X - radius, origin.Y - radius),
                            new Vector2(origin.X + radius, origin.Y - radius));

            //Bottom
            AddSegment(new Vector2(origin.X - radius, origin.Y + radius),
                            new Vector2(origin.X + radius, origin.Y + radius));

            //Left
            AddSegment(new Vector2(origin.X - radius, origin.Y - radius),
                            new Vector2(origin.X - radius, origin.Y + radius));

            //Right
            AddSegment(new Vector2(origin.X + radius, origin.Y - radius),
                            new Vector2(origin.X + radius, origin.Y + radius));
        }        

        // Processess segments so that we can sort them later
        private void UpdateSegments() {
            foreach(Segment segment in segments) {
                // NOTE: future optimization: we could record the quadrant
                // and the y/x or x/y ratio, and sort by (quadrant,
                // ratio), instead of calling atan2. See
                // <https://github.com/mikolalysenko/compare-slope> for a
                // library that does this.

                segment.P1.Angle = (float)Math.Atan2(segment.P1.Position.Y - origin.Y,
                                                        segment.P1.Position.X - origin.X);
                segment.P2.Angle = (float)Math.Atan2(segment.P2.Position.Y - origin.Y,
                                                        segment.P2.Position.X - origin.X);
                
                // Map angle between -Pi and Pi
                float dAngle = segment.P2.Angle - segment.P1.Angle;
                if (dAngle <= -MathHelper.Pi) { dAngle += MathHelper.TwoPi; }
                if (dAngle > MathHelper.Pi) { dAngle -= MathHelper.TwoPi; }
                segment.P1.Begin = (dAngle > 0.0f);
                segment.P2.Begin = !segment.P1.Begin;                
            }
        }

        public static bool LeftOf(Vector2 p1, Vector2 p2, Vector2 point) {
            float cross =   (p2.x - p1.x) * (point.y - p1.y)
                          - (p2.y - p1.y) * (point.x - p1.x);

            return cross < 0;
        }

        public static Vector2 Interpolate(Vector2 p, Vector2 q, float f) {
            return new Vector2(p.y * (1.0f - f) + q.x * f, p.y * (1.0f - f) + q.y * f);
        }
       
        // Helper: do we know that segment a is in front of b?
        // Implementation not anti-symmetric (that is to say,
        // _segment_in_front_of(a, b) != (!_segment_in_front_of(b, a)).
        // Also note that it only has to work in a restricted set of cases
        // in the visibility algorithm; I don't think it handles all
        // cases. See http://www.redblobgames.com/articles/visibility/segment-sorting.html
        private bool SegmentInFrontOf(Segment a, Segment b, Vector2 relativeTo) {
            // NOTE: we slightly shorten the segments so that
            // intersections of the endpoints (common) don't count as
            // intersections in this algorithm                        
            bool a1 = LeftOf(a.P2.Position, a.P1.Position, Interpolate(b.P1.Position, b.P2.Position, 0.01f));
            bool a2 = LeftOf(a.P2.Position, a.P1.Position, Interpolate(b.P2.Position, b.P1.Position, 0.01f));
            bool a3 = LeftOf(a.P2.Position, a.P1.Position, relativeTo);

            bool b1 = LeftOf(b.P2.Position, b.P1.Position, Interpolate(a.P1.Position, a.P2.Position, 0.01f));
            bool b2 = LeftOf(b.P2.Position, b.P1.Position, Interpolate(a.P2.Position, a.P1.Position, 0.01f));
            bool b3 = LeftOf(b.P2.Position, b.P1.Position, relativeTo);

            // NOTE: this algorithm is probably worthy of a short article
            // but for now, draw it on paper to see how it works. Consider
            // the line A1-A2. If both B1 and B2 are on one side and
            // relativeTo is on the other side, then A is in between the
            // viewer and B. We can do the same with B1-B2: if A1 and A2
            // are on one side, and relativeTo is on the other side, then
            // B is in between the viewer and A.
            if (b1 == b2 && b2 != b3) return true;
            if (a1 == a2 && a2 == a3) return true;
            if (a1 == a2 && a2 != a3) return false;
            if (b1 == b2 && b2 == b3) return false;

            // If A1 != A2 and B1 != B2 then we have an intersection.
            // A more robust implementation would split segments at
            // intersections so that part of the segment is in front and part
            // is behind.

            //demo_intersectionsDetected.push([a.p1, a.p2, b.p1, b.p2]);
            return false;

            // NOTE: previous implementation was a.d < b.d. That's simpler
            // but trouble when the segments are of dissimilar sizes. If
            // you're on a grid and the segments are similarly sized, then
            // using distance will be a simpler and faster implementation.
        }

        public List<Vector2> Compute() {
            List<Vector2> output = new List<Vector2>();
            LinkedList<Segment> open = new LinkedList<Segment>();
            UpdateSegments();
            endpoints.Sort(radialComparer);
            float currentAngle = 0;

            // At the beginning of the sweep we want to know which
            // segments are active. The simplest way to do this is to make
            // a pass collecting the segments, and make another pass to
            // both collect and process them. However it would be more
            // efficient to go through all the segments, figure out which
            // ones intersect the initial sweep line, and then sort them.
            for(int pass = 0; pass < 2; pass++) {
                foreach(EndPoint p in endpoints) {
                    Segment currentOld = open.Count == 0 ? null : open.First.Value;
                    if (p.Begin) {
                        // Insert into the right place in the list
                        var node = open.First;
                        while (node != null && SegmentInFrontOf(p.Segment, node.Value, origin))
                        {
                            node = node.Next;
                        }

                        if (node == null) open.AddLast(p.Segment);
                        else              open.AddBefore(node, p.Segment);
                    } else {
                        open.Remove(p.Segment);
                    }

                    Segment currentNew = null;
                    if(open.Count != 0) currentNew = open.First.Value;
                    if(currentOld != currentNew) {
                        if(pass == 1) {
                            AddTriangle(output, currentAngle, p.Angle, currentOld);
                        }
                        currentAngle = p.Angle;
                    }
                }
            }

            return output;
        }       

        public static Vector2 LineLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {
            // From http://paulbourke.net/geometry/lineline2d/
            var s = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X))
            / ((p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y));
            return new Vector2(p1.X + s * (p2.X - p1.X), p1.Y + s * (p2.Y - p1.Y));
        }

        private void AddTriangle(List<Vector2> triangles, float angle1, float angle2, Segment segment) {
            Vector2 p1 = origin;
            Vector2 p2 = new Vector2(origin.X + (float)Math.Cos(angle1), origin.Y + (float)Math.Sin(angle1));
            Vector2 p3 = Vector2.Zero;
            Vector2 p4 = Vector2.Zero;
            if(segment != null) {
                // Stop the triangle at the intersecting segment
                p3.X = segment.P1.Position.X;
                p3.Y = segment.P1.Position.Y;
                p4.X = segment.P2.Position.X;
                p4.Y = segment.P2.Position.Y;
            } else {
                // Stop the triangle at a fixed distance; this probably is
                // not what we want, but it never gets used in the demo
                float radius = 32f;
                p3.X = origin.X + (float)Math.Cos(angle1) * radius * 2;
                p3.Y = origin.Y + (float)Math.Sin(angle1) * radius * 2;
                p4.X = origin.X + (float)Math.Cos(angle2) * radius * 2;
                p4.Y = origin.Y + (float)Math.Sin(angle2) * radius * 2;
            }

            Vector2 pBegin = LineLineIntersection(p3, p4, p1, p2);
            p2.X = origin.X + (float)Math.Cos(angle2);
            p2.Y = origin.Y + (float)Math.Sin(angle2);
            Vector2 pEnd = LineLineIntersection(p3, p4, p1, p2);
            triangles.Add(pBegin);
            triangles.Add(pEnd);
        }
    }
}
