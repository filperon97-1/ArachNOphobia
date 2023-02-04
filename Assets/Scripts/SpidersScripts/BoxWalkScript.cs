using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VariabiliGlobali;

public class BoxWalkScript : MonoBehaviour
{
    public GameObject ragno;
    public Transform spider;
    private GameObject start;
    private GameObject end;
    private GameObject center;
    private bool arrived;
    private float x;
    private float y;
    private float z;

    void Start()
    {
        arrived = false;
        center = ragno.transform.root.transform.Find("Center").gameObject;
        start = ragno.transform.root.transform.Find("Start").gameObject;
        end = ragno.transform.root.transform.Find("End").gameObject;
        spider.LookAt(center.transform);
        spider.position = center.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(spider.position == start.transform.position || spider.position == end.transform.position)
            arrived = !arrived;

        if(!arrived)
        {
            spider.LookAt(start.transform);
            spider.position = Vector3.MoveTowards(spider.position, start.transform.position, 0.001f);
        }
        else
        {
            spider.LookAt(end.transform);
            spider.position = Vector3.MoveTowards(spider.position, end.transform.position, 0.001f);
        }
    }
}
