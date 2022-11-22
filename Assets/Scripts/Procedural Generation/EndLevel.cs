using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    public bool endReached = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            endReached = true;
        }
    }
}
