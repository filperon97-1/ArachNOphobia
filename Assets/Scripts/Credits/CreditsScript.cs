using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour
{
    [SerializeField] private Transform text;
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;
    [SerializeField] private Transform center;
    [SerializeField] private Transform unityLogo;
    public float speed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        text.position = start.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        text.position = Vector3.MoveTowards(text.position, end.transform.position, speed);
        if(text.position == end.transform.position)
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
