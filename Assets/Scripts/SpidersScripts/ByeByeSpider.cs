using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByeByeSpider : MonoBehaviour
{
    public GameObject spider;
    [SerializeField] private GameObject jump;
    [SerializeField] private GameObject end;
    public bool jumped;
    [SerializeField] private OpenWindow close;
    // Start is called before the first frame update
    void Start()
    {
        jumped = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!jumped)
        {
            spider.transform.LookAt(jump.transform);
            spider.transform.parent = null;
            spider.transform.position = Vector3.MoveTowards(spider.transform.position, jump.transform.position, 1*Time.deltaTime);
        }
        else
        {
            spider.transform.LookAt(end.transform);
            spider.transform.position = Vector3.MoveTowards(spider.transform.position, end.transform.position, 0.2f*Time.deltaTime);
            spider.transform.rotation = Quaternion.Slerp(spider.transform.rotation, end.transform.rotation, 0.5f);
        }
        if(spider.transform.position == jump.transform.position)
        {
            jumped = !jumped;
        }
        if(spider.transform.position == end.transform.position && close.enterCounter == 1)
        {
            close.enterCounter --;
            close.closeWindow();
        }
    }
}
