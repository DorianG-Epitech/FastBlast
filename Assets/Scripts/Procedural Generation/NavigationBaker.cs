using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationBaker : MonoBehaviour
{

    public List<NavMeshSurface> surfaces;

    // Use this for initialization
    void Update()
    {
    }

    public void BakeNavMesh()
    {
        foreach (NavMeshSurface surf in surfaces)
        {
            surf.BuildNavMesh();
        }
    }
}
