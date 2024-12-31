using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSM.Runtime.Utilities {
  /// <summary>
  /// Utilities for creating meshes
  /// </summary>
  public static class MeshUtility {
    /// <summary>
    ///
    /// </summary>
    struct TriangleIndices {
      public int _V1;
      public int _V2;
      public int _V3;

      public TriangleIndices(int v1, int v2, int v3) {
        this._V1 = v1;
        this._V2 = v2;
        this._V3 = v3;
      }
    }

    // return index of point in the middle of p1 and p2
    static int GetMiddlePoint(int p1,
                              int p2,
                              ref List<Vector3> vertices,
                              ref Dictionary<long, int> cache,
                              float radius) {
      // first check if we have it already
      var first_is_smaller = p1 < p2;
      long smaller_index = first_is_smaller ? p1 : p2;
      long greater_index = first_is_smaller ? p2 : p1;
      var key = (smaller_index << 32) + greater_index;

      if (cache.TryGetValue(key : key, value : out var ret)) {
        return ret;
      }

      // not in cache, calculate it
      var point1 = vertices[index : p1];
      var point2 = vertices[index : p2];
      var middle = new Vector3(x : (point1.x + point2.x) / 2f,
                               y : (point1.y + point2.y) / 2f,
                               z : (point1.z + point2.z) / 2f);

      // add vertex makes sure point is on unit sphere
      var i = vertices.Count;
      vertices.Add(item : middle.normalized * radius);

      // store it, return index
      cache.Add(key : key, value : i);

      return i;
    }

    /// <summary>
    /// Create an ico sphere
    /// </summary>
    /// <param name="recursion_level"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static Mesh CreateIcoSphere(int recursion_level, float radius) {
      var mesh = new Mesh();

      var vert_list = new List<Vector3>();
      var middle_point_index_cache = new Dictionary<long, int>();

      // create 12 vertices of a icosahedron
      var t = (1f + Mathf.Sqrt(5f)) / 2f;

      vert_list.Add(item : new Vector3(-1f, y : t, 0f).normalized * radius);
      vert_list.Add(item : new Vector3(1f, y : t, 0f).normalized * radius);
      vert_list.Add(item : new Vector3(-1f, y : -t, 0f).normalized * radius);
      vert_list.Add(item : new Vector3(1f, y : -t, 0f).normalized * radius);

      vert_list.Add(item : new Vector3(0f, -1f, z : t).normalized * radius);
      vert_list.Add(item : new Vector3(0f, 1f, z : t).normalized * radius);
      vert_list.Add(item : new Vector3(0f, -1f, z : -t).normalized * radius);
      vert_list.Add(item : new Vector3(0f, 1f, z : -t).normalized * radius);

      vert_list.Add(item : new Vector3(x : t, 0f, -1f).normalized * radius);
      vert_list.Add(item : new Vector3(x : t, 0f, 1f).normalized * radius);
      vert_list.Add(item : new Vector3(x : -t, 0f, -1f).normalized * radius);
      vert_list.Add(item : new Vector3(x : -t, 0f, 1f).normalized * radius);

      // create 20 triangles of the icosahedron
      var faces = new List<TriangleIndices> {
                                                // 5 faces around point 0
                                                new TriangleIndices(0, 11, 5),
                                                new TriangleIndices(0, 5, 1),
                                                new TriangleIndices(0, 1, 7),
                                                new TriangleIndices(0, 7, 10),
                                                new TriangleIndices(0, 10, 11),
                                                // 5 adjacent faces
                                                new TriangleIndices(1, 5, 9),
                                                new TriangleIndices(5, 11, 4),
                                                new TriangleIndices(11, 10, 2),
                                                new TriangleIndices(10, 7, 6),
                                                new TriangleIndices(7, 1, 8),
                                                // 5 faces around point 3
                                                new TriangleIndices(3, 9, 4),
                                                new TriangleIndices(3, 4, 2),
                                                new TriangleIndices(3, 2, 6),
                                                new TriangleIndices(3, 6, 8),
                                                new TriangleIndices(3, 8, 9),
                                                // 5 adjacent faces
                                                new TriangleIndices(4, 9, 5),
                                                new TriangleIndices(2, 4, 11),
                                                new TriangleIndices(6, 2, 10),
                                                new TriangleIndices(8, 6, 7),
                                                new TriangleIndices(9, 8, 1)
                                            };

      // refine triangles
      for (var i = 0; i < recursion_level; i++) {
        var faces2 = new List<TriangleIndices>();
        for (var index = 0; index < faces.Count; index++) {
          var tri = faces[index : index];
// replace triangle by 4 triangles
          var a = GetMiddlePoint(p1 : tri._V1,
                                 p2 : tri._V2,
                                 vertices : ref vert_list,
                                 cache : ref middle_point_index_cache,
                                 radius : radius);
          var b = GetMiddlePoint(p1 : tri._V2,
                                 p2 : tri._V3,
                                 vertices : ref vert_list,
                                 cache : ref middle_point_index_cache,
                                 radius : radius);
          var c = GetMiddlePoint(p1 : tri._V3,
                                 p2 : tri._V1,
                                 vertices : ref vert_list,
                                 cache : ref middle_point_index_cache,
                                 radius : radius);

          faces2.Add(item : new TriangleIndices(v1 : tri._V1, v2 : a, v3 : c));
          faces2.Add(item : new TriangleIndices(v1 : tri._V2, v2 : b, v3 : a));
          faces2.Add(item : new TriangleIndices(v1 : tri._V3, v2 : c, v3 : b));
          faces2.Add(item : new TriangleIndices(v1 : a, v2 : b, v3 : c));
        }

        faces = faces2;
      }

      mesh.vertices = vert_list.ToArray();

      var tri_list = new List<int>();
      for (var i = 0; i < faces.Count; i++) {
        tri_list.Add(item : faces[index : i]._V1);
        tri_list.Add(item : faces[index : i]._V2);
        tri_list.Add(item : faces[index : i]._V3);
      }

      mesh.triangles = tri_list.ToArray();
      mesh.uv = new Vector2[vert_list.Count];

      var normals = new Vector3[vert_list.Count];
      for (var i = 0; i < normals.Length; i++) {
        normals[i] = vert_list[index : i].normalized;
      }

      mesh.normals = normals;

      mesh.RecalculateBounds();

      return mesh;
    }

    /// <summary>
    /// Creates a screen space quad mesh given camera parameterisation
    /// </summary>
    /// <param name="main"></param>
    /// <returns></returns>
    public static Mesh CreateScreenSpaceQuad(Camera main) {
      if (!main) {
        return null;
      }

      var mesh = new Mesh();
      var vertices = new Vector3[4];

      vertices[0] = new Vector3(0, 0, 0);
      vertices[1] = new Vector3(0, 1, 0);
      vertices[2] = new Vector3(1, 0, 0);
      vertices[3] = new Vector3(1, 1, 0);

      mesh.vertices = vertices;

      var normals = new Vector3[4];
      var x = Mathf.Tan(f : main.fieldOfView * 0.5f * Mathf.Deg2Rad) * main.aspect * main.nearClipPlane;
      var y = Mathf.Tan(f : main.fieldOfView * 0.5f * Mathf.Deg2Rad) * main.nearClipPlane;
      var z = main.nearClipPlane;
      normals[0] = new Vector3(x : -x, y : -y, z : z);
      normals[1] = new Vector3(x : -x, y : y, z : z);
      normals[2] = new Vector3(x : x, y : -y, z : z);
      normals[3] = new Vector3(x : x, y : y, z : z);
      mesh.normals = normals;

      var indices = new int[6];

      indices[0] = 0;
      indices[1] = 1;
      indices[2] = 2;

      indices[3] = 2;
      indices[4] = 1;
      indices[5] = 3;

      mesh.triangles = indices;
      mesh.RecalculateBounds();

      return mesh;
    }

    /// <summary>
    /// Creates a pyramid mesh for a spot light
    /// </summary>
    /// <returns></returns>
    public static Mesh CreateSpotlightPyramid() {
      var mesh = new Mesh {
                              vertices = new[] {
                                                   new Vector3(0.0f, 0.0f, 0.0f),
                                                   new Vector3(1.0f, 1.0f, 1.0f),
                                                   new Vector3(1.0f, -1.0f, 1.0f),
                                                   new Vector3(-1.0f, -1.0f, 1.0f),
                                                   new Vector3(-1.0f, 1.0f, 1.0f)
                                               },
                              triangles = new[] {
                                                    0,
                                                    1,
                                                    2,
                                                    0,
                                                    4,
                                                    1,
                                                    0,
                                                    3,
                                                    4,
                                                    0,
                                                    2,
                                                    3,
                                                    1,
                                                    3,
                                                    2,
                                                    1,
                                                    4,
                                                    3
                                                }
                          };

      mesh.RecalculateNormals();
      mesh.RecalculateBounds();
      mesh.Optimize();

      return mesh;
    }

    public static Mesh CreateSpotlightCone() {
      const Int32 num_vertices = 32;
      var radius_top = 0f;
      var radius_bottom = 1f;
      const Single length = 1f;

      var opening_angle =
          0f; // if >0, create a cone with this angle by setting radiusTop to 0, and adjust radiusBottom according to length;

      var outside = true;

      var inside = false;

      if (opening_angle > 0 && opening_angle < 180) {
        radius_top = 0;
        radius_bottom = length * Mathf.Tan(f : opening_angle * Mathf.Deg2Rad / 2);
      }

      var mesh = new Mesh();

      var multiplier = (outside ? 1 : 0) + (inside ? 1 : 0);
      var offset = (outside && inside ? 2 * num_vertices : 0);
      var vertices = new Vector3[2 * multiplier * num_vertices]; // 0..n-1: top, n..2n-1: bottom
      var normals = new Vector3[2 * multiplier * num_vertices];
      var uvs = new Vector2[2 * multiplier * num_vertices];
      int[] tris;
      var slope = Mathf.Atan(f : (radius_bottom - radius_top) / length); // (rad difference)/height
      var slope_sin = Mathf.Sin(f : slope);
      var slope_cos = Mathf.Cos(f : slope);

      int i;

      for (i = 0; i < num_vertices; i++) {
        var angle = 2 * Mathf.PI * i / num_vertices;
        var angle_sin = Mathf.Sin(f : angle);
        var angle_cos = Mathf.Cos(f : angle);
        var angle_half = 2 * Mathf.PI * (i + 0.5f) / num_vertices; // for degenerated normals at cone tips
        var angle_half_sin = Mathf.Sin(f : angle_half);
        var angle_half_cos = Mathf.Cos(f : angle_half);

        vertices[i] = new Vector3(x : radius_top * angle_cos, y : radius_top * angle_sin, 0);
        vertices[i + num_vertices] =
            new Vector3(x : radius_bottom * angle_cos, y : radius_bottom * angle_sin, z : length);

        if (radius_top == 0) {
          normals[i] = new Vector3(x : angle_half_cos * slope_cos, y : angle_half_sin * slope_cos, z : -slope_sin);
        } else {
          normals[i] = new Vector3(x : angle_cos * slope_cos, y : angle_sin * slope_cos, z : -slope_sin);
        }

        if (radius_bottom == 0) {
          normals[i + num_vertices] =
              new Vector3(x : angle_half_cos * slope_cos, y : angle_half_sin * slope_cos, z : -slope_sin);
        } else {
          normals[i + num_vertices] = new Vector3(x : angle_cos * slope_cos, y : angle_sin * slope_cos, z : -slope_sin);
        }

        uvs[i] = new Vector2(x : 1.0f * i / num_vertices, 1);
        uvs[i + num_vertices] = new Vector2(x : 1.0f * i / num_vertices, 0);

        if (outside && inside) {
          // vertices and uvs are identical on inside and outside, so just copy
          vertices[i + 2 * num_vertices] = vertices[i];
          vertices[i + 3 * num_vertices] = vertices[i + num_vertices];
          uvs[i + 2 * num_vertices] = uvs[i];
          uvs[i + 3 * num_vertices] = uvs[i + num_vertices];
        }

        if (inside) {
          // invert normals
          normals[i + offset] = -normals[i];
          normals[i + num_vertices + offset] = -normals[i + num_vertices];
        }
      }

      mesh.vertices = vertices;
      mesh.normals = normals;
      mesh.uv = uvs;

      // create triangles
      // here we need to take care of point order, depending on inside and outside
      var cnt = 0;
      if (radius_top == 0) {
        // top cone
        tris = new int[num_vertices * 3 * multiplier];
        if (outside) {
          for (i = 0; i < num_vertices; i++) {
            tris[cnt++] = i + num_vertices;
            tris[cnt++] = i;
            if (i == num_vertices - 1) {
              tris[cnt++] = num_vertices;
            } else {
              tris[cnt++] = i + 1 + num_vertices;
            }
          }
        }

        if (inside) {
          for (i = offset; i < num_vertices + offset; i++) {
            tris[cnt++] = i;
            tris[cnt++] = i + num_vertices;
            if (i == num_vertices - 1 + offset) {
              tris[cnt++] = num_vertices + offset;
            } else {
              tris[cnt++] = i + 1 + num_vertices;
            }
          }
        }
      } else if (radius_bottom == 0) {
        // bottom cone
        tris = new int[num_vertices * 3 * multiplier];
        if (outside) {
          for (i = 0; i < num_vertices; i++) {
            tris[cnt++] = i;
            if (i == num_vertices - 1) {
              tris[cnt++] = 0;
            } else {
              tris[cnt++] = i + 1;
            }

            tris[cnt++] = i + num_vertices;
          }
        }

        if (inside) {
          for (i = offset; i < num_vertices + offset; i++) {
            if (i == num_vertices - 1 + offset) {
              tris[cnt++] = offset;
            } else {
              tris[cnt++] = i + 1;
            }

            tris[cnt++] = i;
            tris[cnt++] = i + num_vertices;
          }
        }
      } else {
        // truncated cone
        tris = new int[num_vertices * 6 * multiplier];
        if (outside) {
          for (i = 0; i < num_vertices; i++) {
            var ip1 = i + 1;
            if (ip1 == num_vertices) {
              ip1 = 0;
            }

            tris[cnt++] = i;
            tris[cnt++] = ip1;
            tris[cnt++] = i + num_vertices;

            tris[cnt++] = ip1 + num_vertices;
            tris[cnt++] = i + num_vertices;
            tris[cnt++] = ip1;
          }
        }

        if (inside) {
          for (i = offset; i < num_vertices + offset; i++) {
            var ip1 = i + 1;
            if (ip1 == num_vertices + offset) {
              ip1 = offset;
            }

            tris[cnt++] = ip1;
            tris[cnt++] = i;
            tris[cnt++] = i + num_vertices;

            tris[cnt++] = i + num_vertices;
            tris[cnt++] = ip1 + num_vertices;
            tris[cnt++] = ip1;
          }
        }
      }

      mesh.triangles = tris;

      //mesh.RecalculateNormals();
      mesh.RecalculateBounds();
      //mesh.Optimize();

      return mesh;
    }

    public static Mesh GenerateSpotlightCircleMesh(Camera main) {
      if (!main) {
        return null;
      }

      const int circle_segment_count = 64;
      const int circle_vertex_count = circle_segment_count + 2;
      const int circle_index_count = circle_segment_count * 3;
      const Single segment_width = Mathf.PI * 2f / circle_segment_count;

      var circle = new Mesh();
      var vertices = new List<Vector3>(capacity : circle_vertex_count);
      var indices = new int[circle_index_count];

      var angle = 0f;
      vertices.Add(item : Vector3.zero);
      for (var i = 1; i < circle_vertex_count; ++i) {
        vertices.Add(item : new Vector3(x : Mathf.Cos(f : angle), y : Mathf.Sin(f : angle), 0f));
        angle -= segment_width;
        if (i > 1) {
          var j = (i - 2) * 3;
          indices[j + 0] = 0;
          indices[j + 1] = i - 1;
          indices[j + 2] = i;
        }
      }

      circle.SetVertices(inVertices : vertices);
      circle.SetIndices(indices : indices, topology : MeshTopology.Triangles, 0);


      //var x1 = Mathf.Tan(main.fieldOfView * 0.5f * Mathf.Deg2Rad) * main.aspect * main.nearClipPlane;
      //var y1 = Mathf.Tan(main.fieldOfView * 0.5f * Mathf.Deg2Rad) * main.nearClipPlane;
      //var z1 = main.nearClipPlane;

      //normals
      var normals_list = new List<Vector3>();
      for (var i = 0; i < vertices.Count; i++) {
        normals_list.Add(item : Vector3.forward);
      }

      circle.SetNormals(inNormals : normals_list);
      //circle.RecalculateNormals();
      circle.RecalculateBounds();
      //circle.Optimize();
      return circle;
    }

    public static Mesh GenerateSpotlightCircleMesh2( Camera main, float radius = 1f, int n = 60) {
      if (!main) {
        return null;
      }
      var mesh = new Mesh();

      var vertices_list = new List<Vector3> { };
      float x;
      float y;
      for (var i = 0; i < n; i++) {
        x = radius * Mathf.Sin(f : (2 * Mathf.PI * i) / n);
        y = radius * Mathf.Cos(f : (2 * Mathf.PI * i) / n);
        vertices_list.Add(item : new Vector3(x : x, y : y, 0f));
      }

      var vertices = vertices_list.ToArray();

      //triangles
      var triangles_list = new List<int> { };
      for (var i = 0; i < (n - 2); i++) {
        triangles_list.Add(0);
        triangles_list.Add(item : i + 1);
        triangles_list.Add(item : i + 2);
      }

      var triangles = triangles_list.ToArray();

      var x1 = Mathf.Tan(f : main.fieldOfView * 0.5f * Mathf.Deg2Rad) * main.aspect * main.nearClipPlane;
      var y1 = Mathf.Tan(f : main.fieldOfView * 0.5f * Mathf.Deg2Rad) * main.nearClipPlane;
      var z1 = main.nearClipPlane;

      //normals
      var normals_list = new List<Vector3> { };
      for (var i = 0; i < vertices.Length; i++) {
        normals_list.Add(item : Vector3.forward);
      }

      var normals = normals_list.ToArray();

      //initialise
      mesh.vertices = vertices;
      mesh.triangles = triangles;
      mesh.normals = normals;

      return mesh;
    }
  }
}