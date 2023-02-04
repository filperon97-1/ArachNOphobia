using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AskingState : State
{
    private GameObject personaggio;
    private Robottino _robottino;
    private AudioSource _tabletAudioSource;

    private Therapist _therapist;
    
    private Renderer renderer;

    private Texture[] _spidersTextures;

    private int _countSpiderSelection;
    private float actualSpeed;
    private float normalSpeed = 2F;
    private Renderer rend;
    private bool visible;
    private float assaiSpeed = 3F;
    private float minHeight = 1F;      //altezza minima dal terreno
    private float distanza = 0.8F;             //distanza minima tra robottino e personaggio 
    private Vector3 targetPosition;
    private Vector3 direzione;

    public AskingState(string name, Robottino robottino, Therapist therapist) : base(name)
    {
        _robottino = robottino;
        _therapist = therapist;
        if (_robottino != null)
        {
            _spidersTextures = new[]
                {_robottino.plastic_little, _robottino.plastic_medium, _robottino.plastic_tarantula};
            _countSpiderSelection = 0;
        }
    }

    public override void Enter()
    {
        renderer = _robottino.Screen.GetComponent<Renderer>();
        _robottino.vr14Animator.SetBool("tablet", true);
        switch (_therapist.currentStatus)
        {
            case Therapist.MachineStatus.Puzzle:
                renderer.material.SetTexture("_MainTex", _spidersTextures[_countSpiderSelection]);
                _robottino.currentAskingTexture = _spidersTextures[_countSpiderSelection].name;
                break;
                
            case Therapist.MachineStatus.Corridor:
                renderer.material.SetTexture("_MainTex", _robottino.secondTabletTexture);
                _robottino.currentAskingTexture = "secondTexture";
                break;
        }
        personaggio = _robottino.cam;
        actualSpeed = normalSpeed;
        rend = _robottino.GetComponent<MeshRenderer>();
        visible = true;
        _tabletAudioSource = _robottino.Screen.GetComponent<AudioSource>();
        if(_tabletAudioSource != null)
        {
            _tabletAudioSource.enabled = true;
        }
        distanza = 0.6F;
    }

    public override void Exit()
    {
        _robottino.vr14Animator.SetBool("tablet", false);
    }

    public override void Tik()
    {
        visible = true;
        NotTooClose();
        FollowUser();
        rend.enabled = visible;

        if(_tabletAudioSource.clip == _robottino._audioConfirmSelection)
        {
            if (!_tabletAudioSource.isPlaying)
            {
                _tabletAudioSource.enabled = false;
            }
        }

        if (Input.GetKeyDown("q"))
        {
            switch (_therapist.currentStatus)
            {
                case Therapist.MachineStatus.Puzzle:
                    if (_robottino.GetIsSpiderChosen())
                    {
                        break;
                    }
                    _countSpiderSelection--;
                    if (_countSpiderSelection < 0) _countSpiderSelection = 2;
                    var texture = _spidersTextures[_countSpiderSelection];
                    if (texture!=null)
                    {
                        renderer.material.SetTexture("_MainTex", texture);
                        _robottino.currentAskingTexture = texture.name;
                    }
                    PlayTabletSound(_robottino._audioSelection);
                    break;
                
                case Therapist.MachineStatus.Corridor:
                    if (_robottino.currentAskingTexture == "firstTexture")
                    {
                        renderer.material.SetTexture("_MainTex", _robottino.secondTabletTexture);
                        _robottino.currentAskingTexture = "secondTexture";
                    }
                    else
                    {
                        renderer.material.SetTexture("_MainTex", _robottino.firstTabletTexture);
                        _robottino.currentAskingTexture = "firstTexture";
                    }
                    PlayTabletSound(_robottino._audioSelection);
                    break;
            }
            
        }
        
        if (Input.GetKeyDown("e"))
        {
            switch (_therapist.currentStatus)
            {
                case Therapist.MachineStatus.Puzzle:
                    if (_robottino.GetIsSpiderChosen())
                    {
                        break;
                    }
                    _countSpiderSelection++;
                    if (_countSpiderSelection > 2) _countSpiderSelection = 0;
                    var texture = _spidersTextures[_countSpiderSelection];
                    if (texture != null)
                    {
                        renderer.material.SetTexture("_MainTex", texture);
                        _robottino.currentAskingTexture = texture.name;
                    }
                    PlayTabletSound(_robottino._audioSelection);
                    break;
                
                case Therapist.MachineStatus.Corridor:
                    if (_robottino.currentAskingTexture == "firstTexture")
                    {
                        renderer.material.SetTexture("_MainTex", _robottino.secondTabletTexture);
                        _robottino.currentAskingTexture = "secondTexture";
                    }
                    else
                    {
                        renderer.material.SetTexture("_MainTex", _robottino.firstTabletTexture);
                        _robottino.currentAskingTexture = "firstTexture";
                    }
                    PlayTabletSound(_robottino._audioSelection);
                    break;
            }
        }
        if (_robottino.GetIsSpiderChosen() && _therapist.currentStatus== Therapist.MachineStatus.Puzzle)
        {
            distanza = 0.8F;
        }
    }

    void NotTooClose()
    {
        if ((personaggio.transform.position - _robottino.transform.position).sqrMagnitude < Mathf.Pow(distanza, 2))
        {
            actualSpeed = assaiSpeed;
        }
        else
        {
            actualSpeed = normalSpeed;
        }
    }

    void FollowUser()
    {
        _robottino.transform.forward = personaggio.transform.position - _robottino.transform.position;                         // guarda sempre l'utente
        targetPosition = personaggio.transform.position + personaggio.transform.forward * distanza;      // posizione in cui deve spostarsi
        if (_robottino.GetIsSpiderChosen() && _therapist.currentStatus == Therapist.MachineStatus.Puzzle)
        {
            targetPosition = targetPosition + personaggio.transform.right * 0.5F;
        }
        targetPosition.y = Mathf.Max(targetPosition.y, minHeight);                                       // deve stare sollevato da terra 
        direzione = targetPosition - _robottino.transform.position;
        bool cannotMove = false;
        if (!NearObstacle(targetPosition))  //controlla se c'� un ostacolo vicino
        {
            _robottino.transform.position += direzione * actualSpeed * Time.deltaTime;
        }
        else if (!TryDifferentPositions(targetPosition, 0.2F)) //se c'� ostacolo vicino prova altre posizioni (non faccio stessa cosa anche col muro perch� altrimenti vicino al muro ha movimenti strani)
        {
            cannotMove = true;
        }
        // se robottino non riesce a muoversi ed � molto vicino a utente lo rendo invisibile (risulterebbe troppo vicino alla camera)
        if (cannotMove && (personaggio.transform.position - _robottino.transform.position).sqrMagnitude < Mathf.Pow(distanza / 2, 2))
        {
            visible = false;
        }
    }

    // destinazione � troppo vicina a un ostacolo?
    private bool NearObstacle(Vector3 dest)
    {
        float radius = 0.2F;
        Collider[] hitColliders = Physics.OverlapSphere(dest, radius);
        if (hitColliders.Length >= 1)
        {
            foreach (Collider hit in hitColliders)
            {
                if (hit.gameObject.name != "Screen")
                {
                    return true;
                }
            }
        }
        return false;
    }

    // prova destinazioni alternative per il robottino (destinazione pi� in alto, pi� in basso, pi� a destra e pi� a sinistra rispetto alla camera)
    private bool TryDifferentPositions(Vector3 targetPosition, float dist)
    {
        Vector3 up = personaggio.transform.up;
        Vector3 destra = Vector3.Cross(personaggio.transform.forward, up);
        Vector3[] possibleDestinations = new Vector3[] { targetPosition + up * dist, targetPosition - up * dist, targetPosition + destra * dist, targetPosition - destra * dist };
        for (int i = 0; i < 4; i++)
        {
            Vector3 dest = possibleDestinations[i];
            dest.y = Mathf.Max(dest.y, minHeight);
            if (!NearObstacle(dest))
            {
                direzione = dest - _robottino.transform.position;
                _robottino.transform.position += direzione * actualSpeed * Time.deltaTime;
                return true;
            }
        }
        return false;
    }

    private void PlayTabletSound(AudioClip clip)
    {
        _tabletAudioSource.enabled = true;
        _tabletAudioSource.clip = clip;
        _tabletAudioSource.Play();
    }

}
