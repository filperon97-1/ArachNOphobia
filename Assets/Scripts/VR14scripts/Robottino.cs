using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static VariabiliGlobali;

public class Robottino : MonoBehaviour
{

    private FiniteStateMachine<Robottino> finiteStateMachine;

    public enum RbMachineStatus
    {
        Flying,
        Asking
    }

    
    public String eventType;
    public String currentAskingTexture;
    public AudioSource robottinoAudioSource;

    //Spider choice textures
    public Texture plastic_little;
    public Texture plastic_medium;
    public Texture plastic_tarantula;

    //Chosen spider textures
    public Texture chosen_little;
    public Texture chosen_medium;
    public Texture chosen_tarantula;

    //Feedback textures
    public Texture firstTabletTexture;
    public Texture secondTabletTexture;

    public RbMachineStatus currentStatus;

    public Transform targetPosition;

    public GameObject Screen;

    public Therapist _therapist;

    public event EventHandler<CustomEventArgs> onAnswer;

    public Animator vr14Animator;

    public GameObject personaggio;

    public GameObject cam;

    private bool isSpiderChosen;
    private Renderer _renderer;
    private AudioSource _tabletAudioSource;

    [SerializeField] public AudioClip _audioSelection;
    [SerializeField] public AudioClip _audioConfirmSelection;



    // Start is called before the first frame update
    void Start()
    {
        isSpiderChosen = false;
        _renderer = this.Screen.GetComponent<Renderer>();
        _tabletAudioSource = this.Screen.GetComponent<AudioSource>();
        robottinoAudioSource = GetComponent<AudioSource>();
        vr14Animator = GetComponent<Animator>();
        finiteStateMachine = new FiniteStateMachine<Robottino>(this);

        if(_therapist!=null)
            _therapist.OnAsking += ToAskingStatus;

        //STATES

        State flyState = new FlyState2("fly", this);
        State askingState = new AskingState("asking", this, _therapist);

        //TRANSITIONS

        finiteStateMachine.AddTransition(flyState, askingState, () => currentStatus == RbMachineStatus.Asking);
        finiteStateMachine.AddTransition(askingState, flyState, () => currentStatus == RbMachineStatus.Flying);

        //INITIAL STATUS

        if(_therapist!=null)
            switch (_therapist.currentStatus)
            {
                case Therapist.MachineStatus.Begin:
                    currentStatus = RbMachineStatus.Flying;
                    finiteStateMachine.SetState(flyState);
                    break;
                case Therapist.MachineStatus.Puzzle:
                    currentStatus = RbMachineStatus.Asking;
                    finiteStateMachine.SetState(askingState);
                    break;
                    
                case Therapist.MachineStatus.Corridor:
                    currentStatus = RbMachineStatus.Flying;
                    finiteStateMachine.SetState(flyState);
                    break;
                case Therapist.MachineStatus.Boxes:
                    currentStatus = RbMachineStatus.Flying;
                    finiteStateMachine.SetState(flyState);
                    break;
                default:
                    currentStatus = RbMachineStatus.Flying;
                    finiteStateMachine.SetState(flyState);
                    break;
            }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.robottinoAudioSource.isPlaying == false)
        {
            vr14Animator.SetBool("talk",false);
        }
        finiteStateMachine.Tik();

        //quando l'utente seleziona la texture lancio l'evento che fa reagire terapista e spiderGenerator
        if (currentStatus==RbMachineStatus.Asking && Input.GetKeyDown(KeyCode.Return))
        {
            switch (_therapist.currentStatus)
            {
                case Therapist.MachineStatus.Begin:
                    currentStatus = RbMachineStatus.Flying;
                    break;
                case Therapist.MachineStatus.Puzzle:

                    if (_tabletAudioSource != null && !isSpiderChosen)
                    {
                        PlayTabletSound(_audioConfirmSelection);
                    }

                    isSpiderChosen = true;
                    var chosenSpiders = FindObjectsOfType<PlasticSpider>();
                    foreach (var spider in chosenSpiders)
                    {
                        if (spider.spiderType == currentAskingTexture)
                            spider.chosen = true;
                    }

                    //aggiorna variabile globale ragno scelto
                    switch (currentAskingTexture)
                    {
                        case "plastic_tarantula":
                            VariabiliGlobali.ragnoScelto = VariabiliGlobali.Ragno.Tarantula;
                            _renderer.material.SetTexture("_MainTex", chosen_tarantula);
                            break;
                        case "plastic_medium":
                            VariabiliGlobali.ragnoScelto = VariabiliGlobali.Ragno.Medium;
                            _renderer.material.SetTexture("_MainTex", chosen_medium);
                            break;
                        case "plastic_little":
                            VariabiliGlobali.ragnoScelto = VariabiliGlobali.Ragno.Little;
                            _renderer.material.SetTexture("_MainTex", chosen_little);
                            break;
                    }                
                    break;
                
                case Therapist.MachineStatus.Corridor:
                    onAnswer(this, new CustomEventArgs(eventType, currentAskingTexture));
                    currentStatus = RbMachineStatus.Flying;
                    PlayTabletSound(_audioConfirmSelection);
                    break;
            }
            //currentStatus = RbMachineStatus.Flying;
        }

    }

    private void ToAskingStatus(object therapist, CustomEventArgs eventArgs)
    {
        currentStatus = RbMachineStatus.Asking;
        eventType = eventArgs.eventType;
        _tabletAudioSource.enabled = true;
    }

    public void Parla(string tracciaAudio)
    {
        if(vr14Animator.GetBool("tablet")==false)
            vr14Animator.SetBool("talk",true);
        var clip = Resources.Load(tracciaAudio) as AudioClip;
        robottinoAudioSource.clip = clip;
        robottinoAudioSource.Play();
    }

    public bool GetIsSpiderChosen()
    {
        return isSpiderChosen;
    }

    private void PlayTabletSound(AudioClip clip)
    {
        _tabletAudioSource.enabled = true;
        _tabletAudioSource.clip = clip;
        _tabletAudioSource.Play();
    }
}
