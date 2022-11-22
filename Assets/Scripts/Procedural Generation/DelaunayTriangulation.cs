using System;
using System.Collections.Generic;
using UnityEngine;
using Graphs;

public class DelaunayTriangulation
{
    public List<Vertex> Vertices { get; private set; }
    public List<Edge> Edges { get; private set; }
    public List<Triangle> Triangles { get; private set; }
    public List<Tetrahedron> Tetrahedra { get; private set; }

    DelaunayTriangulation()
    {
        Edges = new List<Edge>();
        Triangles = new List<Triangle>();
        Tetrahedra = new List<Tetrahedron>();
    }

    public static DelaunayTriangulation Triangulate(List<Vertex> vertices)
    {
        DelaunayTriangulation delaunay = new DelaunayTriangulation();
        delaunay.Vertices = new List<Vertex>(vertices);
        delaunay.Triangulate();

        return delaunay;
    }

    void Triangulate()
    {
        float minX = Vertices[0].Position.x;
        float minY = Vertices[0].Position.y;
        float minZ = Vertices[0].Position.z;
        float maxX = minX;
        float maxY = minY;
        float maxZ = minZ;

        foreach (var vertex in Vertices)
        {
            if (vertex.Position.x < minX) minX = vertex.Position.x;
            if (vertex.Position.x > maxX) maxX = vertex.Position.x;
            if (vertex.Position.y < minY) minY = vertex.Position.y;
            if (vertex.Position.y > maxY) maxY = vertex.Position.y;
            if (vertex.Position.z < minZ) minZ = vertex.Position.z;
            if (vertex.Position.z > maxZ) maxZ = vertex.Position.z;
        }

        float dx = maxX - minX;
        float dy = maxY - minY;
        float dz = maxZ - minZ;
        float deltaMax = Mathf.Max(dx, dy, dz) * 2;

        Vertex p1 = new Vertex(new Vector3(minX - 1, minY - 1, minZ - 1));
        Vertex p2 = new Vertex(new Vector3(maxX + deltaMax, minY - 1, minZ - 1));
        Vertex p3 = new Vertex(new Vector3(minX - 1, maxY + deltaMax, minZ - 1));
        Vertex p4 = new Vertex(new Vector3(minX - 1, minY - 1, maxZ + deltaMax));

        Tetrahedra.Add(new Tetrahedron(p1, p2, p3, p4));

        foreach (var vertex in Vertices)
        {
            List<Triangle> triangles = new List<Triangle>();

            foreach (var t in Tetrahedra)
            {
                if (t.CircumCircleContains(vertex.Position))
                {
                    t.IsBad = true;
                    triangles.Add(new Triangle(t.A, t.B, t.C));
                    triangles.Add(new Triangle(t.A, t.B, t.D));
                    triangles.Add(new Triangle(t.A, t.C, t.D));
                    triangles.Add(new Triangle(t.B, t.C, t.D));
                }
            }

            for (int i = 0; i < triangles.Count; i++)
            {
                for (int j = i + 1; j < triangles.Count; j++)
                {
                    if (Triangle.AlmostEqual(triangles[i], triangles[j]))
                    {
                        triangles[i].IsBad = true;
                        triangles[j].IsBad = true;
                    }
                }
            }

            Tetrahedra.RemoveAll((Tetrahedron t) => t.IsBad);
            triangles.RemoveAll((Triangle t) => t.IsBad);

            foreach (var triangle in triangles)
            {
                Tetrahedra.Add(new Tetrahedron(triangle.U, triangle.V, triangle.W, vertex));
            }
        }

        Tetrahedra.RemoveAll((Tetrahedron t) => t.ContainsVertex(p1) || t.ContainsVertex(p2) || t.ContainsVertex(p3) || t.ContainsVertex(p4));

        HashSet<Triangle> triangleSet = new HashSet<Triangle>();
        HashSet<Edge> edgeSet = new HashSet<Edge>();

        foreach (var t in Tetrahedra)
        {
            var abc = new Triangle(t.A, t.B, t.C);
            var abd = new Triangle(t.A, t.B, t.D);
            var acd = new Triangle(t.A, t.C, t.D);
            var bcd = new Triangle(t.B, t.C, t.D);

            if (triangleSet.Add(abc))
            {
                Triangles.Add(abc);
            }

            if (triangleSet.Add(abd))
            {
                Triangles.Add(abd);
            }

            if (triangleSet.Add(acd))
            {
                Triangles.Add(acd);
            }

            if (triangleSet.Add(bcd))
            {
                Triangles.Add(bcd);
            }

            var ab = new Edge(t.A, t.B);
            var bc = new Edge(t.B, t.C);
            var ca = new Edge(t.C, t.A);
            var da = new Edge(t.D, t.A);
            var db = new Edge(t.D, t.B);
            var dc = new Edge(t.D, t.C);

            if (edgeSet.Add(ab))
            {
                Edges.Add(ab);
            }

            if (edgeSet.Add(bc))
            {
                Edges.Add(bc);
            }

            if (edgeSet.Add(ca))
            {
                Edges.Add(ca);
            }

            if (edgeSet.Add(da))
            {
                Edges.Add(da);
            }

            if (edgeSet.Add(db))
            {
                Edges.Add(db);
            }

            if (edgeSet.Add(dc))
            {
                Edges.Add(dc);
            }
        }
    }

    public static bool AlmostEqual(Vertex left, Vertex right)
    {
        return (left.Position - right.Position).sqrMagnitude < 0.01f;
    }
}
