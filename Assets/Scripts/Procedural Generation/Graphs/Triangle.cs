using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Graphs;

public class Triangle
{
    public Vertex U { get; set; }
    public Vertex V { get; set; }
    public Vertex W { get; set; }

    public bool IsBad { get; set; }

    public Triangle()
    {

    }

    public Triangle(Vertex u, Vertex v, Vertex w)
    {
        U = u;
        V = v;
        W = w;
    }

    public static bool operator ==(Triangle left, Triangle right)
    {
        return (left.U == right.U || left.U == right.V || left.U == right.W)
            && (left.V == right.U || left.V == right.V || left.V == right.W)
            && (left.W == right.U || left.W == right.V || left.W == right.W);
    }

    public static bool operator !=(Triangle left, Triangle right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj is Triangle e)
        {
            return this == e;
        }

        return false;
    }

    public bool Equals(Triangle e)
    {
        return this == e;
    }

    public override int GetHashCode()
    {
        return U.GetHashCode() ^ V.GetHashCode() ^ W.GetHashCode();
    }

    public static bool AlmostEqual(Triangle left, Triangle right)
    {
        return (DelaunayTriangulation.AlmostEqual(left.U, right.U) || DelaunayTriangulation.AlmostEqual(left.U, right.V) || DelaunayTriangulation.AlmostEqual(left.U, right.W))
            && (DelaunayTriangulation.AlmostEqual(left.V, right.U) || DelaunayTriangulation.AlmostEqual(left.V, right.V) || DelaunayTriangulation.AlmostEqual(left.V, right.W))
            && (DelaunayTriangulation.AlmostEqual(left.W, right.U) || DelaunayTriangulation.AlmostEqual(left.W, right.V) || DelaunayTriangulation.AlmostEqual(left.W, right.W));
    }
}
