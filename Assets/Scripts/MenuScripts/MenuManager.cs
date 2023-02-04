using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public CustomButton[] buttons = new CustomButton[3];
    private int buttonCounter=0;
    private bool check;

    [SerializeField] MenuPlane plane;

    // Start is called before the first frame update
    void Start()
    {
        buttons[buttonCounter].GetComponent<Animator>().SetBool("Selected", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && buttonCounter<2)
        {
            buttons[buttonCounter].buttonAnimator.SetBool("Selected", false);
            buttonCounter++;
            buttons[buttonCounter].buttonAnimator.SetBool("Selected", true);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && buttonCounter>0)
        {

            buttons[buttonCounter].buttonAnimator.SetBool("Selected", false);
            buttonCounter--;
            buttons[buttonCounter].buttonAnimator.SetBool("Selected", true);
        }
        if (Input.GetKeyDown("return"))
        {
            buttons[buttonCounter].buttonAnimator.SetBool("Clicked", true);
            switch (buttonCounter)
            {
                case 0:
                    SceneManager.LoadScene("faseBase");
                    break;
                case 1:
                    SceneManager.LoadScene("Credits");
                    break;
                case 2:
                    Application.Quit();
                    break;
                default:
                    break;
            }
        }

       

    }
}
