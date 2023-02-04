using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VariabiliGlobali;
using UnityEngine.SceneManagement;

public class FreeSpiderState : State
{
    private Therapist _therapist;
    private Robottino _robottino;
    private AudioSource _robottinoAudioSource;

    public bool isGrabbing;
    private float thresholdNotGrabbing;
    private float notGrabbingCounter;
    private bool _endTalking;
    private bool _endIntroRobottino;

    private GameObject _player;

    private FreeSpiderScript _freeSpiderScript;


    public FreeSpiderState(string name, Therapist therapist, GameObject player) : base(name)
    {
        _therapist = therapist;
        _player = player;
        _freeSpiderScript = _player.GetComponent<FreeSpiderScript>();
    }


    public override void Enter()
    {
        isGrabbing = false;
        _endTalking = false;
        _endIntroRobottino = true;
        thresholdNotGrabbing = 30;
        notGrabbingCounter = 0;
        _player.GetComponent<Grab>().enabled = false;
        _robottino = GameObject.FindObjectOfType<Robottino>();
        if(_robottino != null)
        {
            _robottinoAudioSource = _robottino.GetComponent<AudioSource>();
        }

        var window = GameObject.FindObjectOfType<OpenWindow>();
        if(window != null)
        {
            window.GetComponent<BoxCollider>().isTrigger = true;
        }
    }

    public override void Exit()
    {
        Say("TherapistAudio/Conclusione2");
    }

    public override void Tik()
    {
        if (_therapist.therapistAudioSource.isPlaying == false && !_endTalking)
        {
            _therapist.therapistAnimator.SetBool("talking",false);
            _robottino.Parla("VR14Audio/VR14Robot/VR14GrabSpider_Robot");
            _endTalking = true;
            
        }

        if(_robottinoAudioSource != null && _endIntroRobottino)
        {
            if(!_robottinoAudioSource.isPlaying && _endTalking)
            {
                _player.GetComponent<GrabSpider>().enabled = true;
                _endIntroRobottino = false;
            }
        }
            
        LookAtPlayer();
        if(!isGrabbing && !_therapist.therapistAudioSource.isPlaying)
        {
            CheckIfUserOk();
            if (!_therapist.therapistAudioSource.isPlaying)
            {
                _therapist.therapistAnimator.SetBool("talking", false);
            }
        }
        if(_player.GetComponent<FreeSpiderScript>().enabled && !isGrabbing)
        {
            isGrabbing = true;
            _robottino.Parla("VR14Audio/VR14Robot/VR14FreeSpider_Robot");
        }
        if(_freeSpiderScript.liberato == true)
        {
            _freeSpiderScript.liberato = false;
            Say("TherapistAudio/Conclusione2");
        }
        if(_therapist.therapistAudioSource.clip == Resources.Load("TherapistAudio/Conclusione2"))
        {
            if (_therapist.therapistAudioSource.isPlaying == false)
            {
                SceneManager.LoadScene("Credits");
            }
        }
    }

   private void LookAtPlayer()
    {
        float speed = 3.0f;
        Vector3 targetDirection = _therapist.user.transform.position - _therapist.transform.position;
        targetDirection.y = _therapist.transform.forward.y;
        float singleStep = speed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(_therapist.transform.forward, targetDirection, singleStep, 0.0f);
        _therapist.transform.rotation = Quaternion.LookRotation(newDirection);
    }

    private void CheckIfUserOk()
    {
        //se non sta prendendo il ragno da un po'
        if (!isGrabbing)
        {
            notGrabbingCounter += Time.deltaTime;
            if (notGrabbingCounter >= thresholdNotGrabbing)
            {
                Say("TherapistAudio/Incoraggiamento");        // Esorta a prendere il ragno
                notGrabbingCounter = 0;
                thresholdNotGrabbing *= 2;
            }
        }
    }

    private void Say(string path)
    {
        if (!_therapist.therapistAudioSource.isPlaying)
        {
            _therapist.therapistAnimator.SetBool("talking", true);
            var clip = Resources.Load(path) as AudioClip;
            _therapist.therapistAudioSource.clip = clip;
            _therapist.therapistAudioSource.Play();
        }
    }

}
