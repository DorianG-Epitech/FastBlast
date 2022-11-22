using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Graphs;

public class Edge
{
    public Vertex U { get; set; }
    public Vertex V { get; set; }

    public bool IsBad { get; set; }

    public Edge()
    {

    }

    public Edge(Vertex u, Vertex v)
    {
        U = u;
        V = v;
    }

    public static bool operator ==(Edge left, Edge right)
    {
        return (left.U == right.U || left.U == right.V)
            && (left.V == right.U || left.V == right.V);
    }

    public static bool operator !=(Edge left, Edge right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj is Edge e)
        {
            return this == e;
        }

        return false;
    }

    public bool Equals(Edge e)
    {
        return this == e;
    }

    public override int GetHashCode()
    {
        return U.GetHashCode() ^ V.GetHashCode();
    }

    public static bool AlmostEqual(Edge left, Edge right)
    {
        return (DelaunayTriangulation.AlmostEqual(left.U, right.U) || DelaunayTriangulation.AlmostEqual(left.V, right.U))
            && (DelaunayTriangulation.AlmostEqual(left.U, right.V) || DelaunayTriangulation.AlmostEqual(left.V, right.U));
    }
}
