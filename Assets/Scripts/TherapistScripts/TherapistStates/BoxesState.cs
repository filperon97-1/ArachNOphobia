using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;

public class BoxesState : State
{
    private Therapist _therapist;
    private Grab _grab;
    private ApritoreDiScatole _apritoreDiScatole;
    private GameObject _player;

    private bool isWalking;
    private bool isGrabbing;
    private float thresholdNotGrabbing;
    private float thresholdNotOpening;
    private float notGrabbingCounter;
    private float notOpeningCounter;
    private bool unableToGrab;


    public BoxesState(string name, Therapist therapist , Grab grab , ApritoreDiScatole apritoreDiScatole, GameObject player) : base(name)
    {
        _therapist = therapist;
        _grab = grab;
        _apritoreDiScatole = apritoreDiScatole;
        _player = player;
    }

    public override void Enter()
    {
        //iscrizione a eventi
        if (_grab != null)
        {
            _grab.OnGrab += Grabbato;
            _grab.OnDeGrab += DeGrabbato;
        }

        if (_apritoreDiScatole != null)
        {
            _apritoreDiScatole.OnOpen += ScatolaAperta;
            _apritoreDiScatole.OnLastOpen += Fine;
        }

        //inizializzo variabili
        isWalking = true;
        isGrabbing = false;
        thresholdNotGrabbing = 30;
        thresholdNotOpening = 30;
        notGrabbingCounter = 0;
        notOpeningCounter = 0;
        unableToGrab = true;
        var vr14 = GameObject.FindObjectOfType<Robottino>();
        if (vr14 != null)
        {
            vr14.GetComponent<Robottino>().currentStatus = Robottino.RbMachineStatus.Flying;
        }

        _player.GetComponent<Grab>().enabled = false;
        StartExplanation();
    }

    public override void Tik()
    {
        _therapist.therapistAnimator.SetBool("talking", _therapist.therapistAudioSource.isPlaying);
        LookAtPlayer();
        if(!isWalking && !_therapist.therapistAudioSource.isPlaying)
        {
            CheckIfUserOk();
        }

        if(!_therapist.therapistAudioSource.isPlaying && unableToGrab)
        {
            unableToGrab = false;
            _player.GetComponent<Grab>().enabled = true;
        }
    }

    public override void Exit()
    {
        _therapist.therapistAnimator.SetBool("talking",true);
        Say("TherapistAudio/Fase3_fineNew");
        _player.GetComponent<Grab>().enabled = false;
    }



    private void LookAtPlayer()
    {
        if (!isWalking)
        {
            float speed = 3.0f;
            Vector3 targetDirection = _therapist.user.transform.position - _therapist.transform.position;
            targetDirection.y = _therapist.transform.forward.y;
            float singleStep = speed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(_therapist.transform.forward, targetDirection, singleStep, 0.0f);
            _therapist.transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    private void CheckIfUserOk()
    {
        //se non sta prendendo le scatole da un po'
        if (!isGrabbing)
        {
            notOpeningCounter = 0;
            notGrabbingCounter += Time.deltaTime;
            if (notGrabbingCounter >= thresholdNotGrabbing)
            {
                Say("TherapistAudio/Fase3_2");        // "trova le scatole nella stanza"
                notGrabbingCounter = 0;
                thresholdNotGrabbing *= 2;
            }
        }

        //se non sta aprendo la scatola che sta portando da un po'
        else
        {
            notOpeningCounter += Time.deltaTime;
            if (notOpeningCounter >= thresholdNotOpening)
            {
                Say("TherapistAudio/Fase3_3");        // "porta la scatola sulla scrivania e aprila"
                notOpeningCounter = 0;
                thresholdNotOpening *= 2;
            }
        }
    }

    private void Say(string path)
    {
        var clip = Resources.Load(path) as AudioClip;
        _therapist.therapistAudioSource.clip = clip;
        _therapist.therapistAudioSource.Play();
    }

    private void StartExplanation()
    {
        _therapist.setAnimator(Therapist.AnimatorWays.WalkingToIdle);
        isWalking = false;
        Say("TherapistAudio/Fase3_Join");
    }

    //risposte agli eventi
    private void Grabbato(object obj, CustomEventArgs eventArgs)
    {
        isGrabbing = true;
        notGrabbingCounter = 0;
    }

    private void DeGrabbato(object obj, CustomEventArgs eventArgs)
    {
        isGrabbing = false;
    }

    private void ScatolaAperta(object obj, CustomEventArgs eventArgs)
    {
        notOpeningCounter = 0;
    }

    private void Fine(object obj, CustomEventArgs eventArgs)
    {
        _therapist.currentStatus = Therapist.MachineStatus.FreeSpider;
    }
}
