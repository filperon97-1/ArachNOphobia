using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spider: MonoBehaviour
{
    SpiderGenerator generator;

    NavMeshAgent agent;
    private Animator animator;

    public float maxDistance;
    public float reachedDistance;

    private Therapist _therapist;

    [SerializeField] private AudioClip _spiderWalk;

    Vector3 target;
    public bool hunting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        generator = FindObjectOfType<SpiderGenerator>();
        _therapist = FindObjectOfType<Therapist>();
        if(_therapist.currentStatus == Therapist.MachineStatus.Corridor)
            animator.Play("WalkingSpider");
    }

    void Update()
    {
        if(_therapist.currentStatus == Therapist.MachineStatus.Corridor)
        {
            if (!hunting || Vector3.Distance(transform.position, target) <= reachedDistance)
            {
                target = generator.RandomTarget(transform.position, maxDistance);
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(target, path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetDestination(target);
                    hunting = true;
                    playSoundWalk();
                }
                else hunting = false;
            }
        }
    }
    private void playSoundWalk()
    {
        var source = GetComponent<AudioSource>();
        if(source != null)
        {
            source.clip = _spiderWalk;
            source.Play();
            source.loop = true;
        }
    }

}
