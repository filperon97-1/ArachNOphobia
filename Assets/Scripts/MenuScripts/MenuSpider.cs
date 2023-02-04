using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MenuSpider : MonoBehaviour
{
    [SerializeField] MenuPlane generator;

    NavMeshAgent agent;
    public float reachedDistance;
    public float maxDistance;

    public Vector3 target;
    public bool hunting = false;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {

        if (!hunting || Vector3.Distance(transform.position, target) <= reachedDistance)
        {
            target = generator.findPoint(transform.position, maxDistance);
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(target, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(target);
                hunting = true;
            }
            else hunting = false;
        }
    }

}
