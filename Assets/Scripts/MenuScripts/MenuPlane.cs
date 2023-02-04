using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MenuPlane : MonoBehaviour
{

    public Spider spider;

    // Start is called before the first frame update
    void Start()
    {

    }

    public Vector3 findPoint(Vector3 center, float maxDistance)
    {
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

        NavMeshHit hit;

        bool foundPosition = NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

        if (!foundPosition)
        {
            //Debug.Log("returned center");
            return center;
        }

        return hit.position;
    }

    
   
}
