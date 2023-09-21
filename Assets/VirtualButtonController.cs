using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

/// <summary>
/// This class implements the IVirtualButtonEventHandler interface and
/// contains the logic to start animations depending on what 
/// virtual button has been pressed.
/// </summary>
public class VirtualButtonController : MonoBehaviour
{
    #region PUBLIC_MEMBERS
    public Animator animDoor;
    public Animator animWindow;
    public Animator animBird;
    public Animator animFrontWheel;
    public Animator animBackWheel;

    public Animator animCircle1;
    public Animator animCircle2;
    public Animator animCircle3;

    public AudioSource soundDoor;
    public AudioSource soundWindow;
    public AudioSource soundCircles;
    public AudioSource soundSlide;
    public AudioSource soundWheel1;
    public AudioSource soundWheel2;

    public image_event IE;

    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS
    VirtualButtonBehaviour[] virtualButtonBehaviours;
    VirtualButtonBehaviour lastButtonPressed;
    private int quantPlays;
    #endregion // PRIVATE_MEMBERS

    #region MONOBEHAVIOUR_METHODS
    void Awake()
    {
        //Debug.Log("Awake: " );

        // Register with the virtual buttons TrackableBehaviour
        IE = GameObject.Find("Image").GetComponent<image_event>();
        virtualButtonBehaviours = GetComponentsInChildren<VirtualButtonBehaviour>();
        animDoor.GetComponent<Animator>();
        animBird.GetComponent<Animator>();
        animFrontWheel.GetComponent<Animator>();
        animBackWheel.GetComponent<Animator>();
        animCircle1.GetComponent<Animator>();
        animCircle2.GetComponent<Animator>();
        animCircle3.GetComponent<Animator>();
        soundDoor.GetComponent<AudioSource>();
        soundWindow.GetComponent<AudioSource>();
        soundCircles.GetComponent<AudioSource>();
        soundSlide.GetComponent<AudioSource>();
        soundWheel1.GetComponent<AudioSource>();
        soundWheel2.GetComponent<AudioSource>();

        quantPlays = 0;

        //Debug.Log("Awake: len: " + virtualButtonBehaviours.Length);
        for (int i = 0; i < virtualButtonBehaviours.Length; ++i)
        {
            //Debug.Log("Awake name: " + virtualButtonBehaviours[i].VirtualButtonName);
            virtualButtonBehaviours[i].RegisterOnButtonPressed(OnButtonPressed);
            virtualButtonBehaviours[i].RegisterOnButtonReleased(OnButtonReleased);
        }
    }

    void Destroy()
    {
        // Register with the virtual buttons TrackableBehaviour
        virtualButtonBehaviours = GetComponentsInChildren<VirtualButtonBehaviour>();

        for (int i = 0; i < virtualButtonBehaviours.Length; ++i)
        {
            virtualButtonBehaviours[i].UnregisterOnButtonPressed(OnButtonPressed);
            virtualButtonBehaviours[i].UnregisterOnButtonReleased(OnButtonReleased);
        }
    }



    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    /// <summary>
    /// Called when the virtual button has just been pressed:
    /// </summary>
    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        //Debug.Log("OnButtonPressed: " + vb.VirtualButtonName);

        StopAllCoroutines();

        // Deixar rodar no maximo 2 anims simultaneas
        if (quantPlays >= 2) {
            // Esses 2 tem prioridade
            if ((vb.VirtualButtonName == "bird") ||
                // so vai no circles se o anterior nao for o bird. A prioridade do birds eh maior
                ((vb.VirtualButtonName == "circles") && (lastButtonPressed.VirtualButtonName != "bird"))) {
                OnButtonReleased(lastButtonPressed);
            } else {
                return;
            }
        }

        lastButtonPressed = vb;

        if (vb.VirtualButtonName == "circles")
        {
            animCircle1.Play("circulo1_anim");
            animCircle2.Play("circulo2_anim");
            animCircle3.Play("circulo3_anim");
            soundCircles.Play();
            quantPlays++;
        }
        else if (vb.VirtualButtonName == "door")
        {
            animDoor.Play("porta_anim1");
            soundDoor.Play();
            quantPlays++;
        }
        else if (vb.VirtualButtonName == "window")
        {
            animWindow.Play("janela_anim");
            soundWindow.Play();
            quantPlays++;
        }
        else if (vb.VirtualButtonName == "bird")
        {
            animBird.Play("passaro_anim");
            soundSlide.Play();
            quantPlays++;
        }
        else if (vb.VirtualButtonName == "front")
        {
            animFrontWheel.Play("roda_frente_anim");
            soundWheel1.Play();
            quantPlays++;
        }
        else if (vb.VirtualButtonName == "back")
        {
            animBackWheel.Play("roda_tras_anim");
            soundWheel2.Play();
            quantPlays++;
        }

        //BroadcastMessage("HandleVirtualButtonPressed", SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Called when the virtual button has just been released:
    /// </summary>
    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        //Debug.Log("OnButtonReleased: " + vb.VirtualButtonName);

        if (vb.VirtualButtonName == "door") {
            animDoor.Play("porta_anim2");
            if (soundDoor.isPlaying) {
                soundDoor.Stop();
            }
            quantPlays--;
        }
        else if (vb.VirtualButtonName == "window")
        {
            animWindow.Play("janela_anim2");
            if (soundWindow.isPlaying) {
                soundWindow.Stop();
            }
            quantPlays--;
        }
        else if (vb.VirtualButtonName == "bird")
        {
            animBird.Play("none");
            if (soundSlide.isPlaying) {
                soundSlide.Stop();
            }
            quantPlays--;
        }
        else if (vb.VirtualButtonName == "front") {
            animFrontWheel.Play("none");
            if (soundWheel1.isPlaying){
                soundWheel1.Stop();
            }
            quantPlays--;
        }
        else if (vb.VirtualButtonName == "back") {
            animBackWheel.Play("none");
            if (soundWheel2.isPlaying) {
                soundWheel2.Stop();
            }
            quantPlays--;
        }
        else if (vb.VirtualButtonName == "circles")
        {
            animCircle1.Play("none");
            animCircle2.Play("none");
            animCircle3.Play("none");
            if (soundCircles.isPlaying) {
                soundCircles.Stop();
            }
            quantPlays--;
        }
    }
    #endregion //PUBLIC_METHODS
}
