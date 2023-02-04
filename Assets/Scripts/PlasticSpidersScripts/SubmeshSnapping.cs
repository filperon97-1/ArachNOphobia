using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class SubmeshSnapping : MonoBehaviour
{
    public float radius = 1;
    public Transform target;
    public bool active;
    public float duration;

    private Transform[] children = new Transform[4];
    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            if (i < 5)
                children[i] = child;
            else break;
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null){
            //TODO: Check if user is grabbing a child to limit computation
            foreach (Transform source in children)
            {
                if (Vector3.Distance(source.position, target.position) < radius)
                {
                    if (active)
                    {
                        var direction = GameObject.FindObjectOfType<SpiderPlatform>();
                        if (direction != null)
                        {
                            if (source.gameObject.GetComponent<PlasticSpider>().chosen == true && Mathf.Abs(source.rotation.z - direction.transform.rotation.z) <= 0.01f)
                            {
                                //Move to target only if the child is the spider chosen by the user
                                source.DOMove(new Vector3(target.position.x, source.position.y, target.position.z), duration);
                                source.gameObject.GetComponent<PlasticSpider>().RightPosition = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
