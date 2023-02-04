using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour
{

    public Animator buttonAnimator;
    // Start is called before the first frame update
    void Start()
    {
        buttonAnimator = GetComponent<Animator>();
        this.gameObject.GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TaskOnClick()
    {
        SceneManager.LoadScene("faseBase");
    }
}
