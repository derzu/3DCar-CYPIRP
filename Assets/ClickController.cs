using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickController: MonoBehaviour
{
    public Animator anim1;
    public Animator anim2;
    public Animator anim3;
    public AudioSource som;
    private bool isPlaying;


    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("TouchDetection::Start:: " + gameObject.name);

        isPlaying = false;

        //Debug.Log("Start::anim1::isActiveAndEnabled:: " + anim1.isActiveAndEnabled);
        //Debug.Log("Start::anim2:: " + anim2.isActiveAndEnabled);
        //Debug.Log("Start::anim3:: " + anim3.isActiveAndEnabled);
        //Debug.Log("Start::anim1::isInitialized:: " + anim1.isInitialized);
        //Debug.Log("Start::anim2:: " + anim2.isInitialized);
        //Debug.Log("Start::anim3:: " + anim3.isInitialized);
        //Debug.Log("Start::anim1::isMatchingTarget:: " + anim1.isMatchingTarget);
        //Debug.Log("Start::anim2:: " + anim2.isMatchingTarget);
        //Debug.Log("Start::anim3:: " + anim3.isMatchingTarget);
        //Debug.Log("Start::anim1::hasBoundPlayables:: " + anim1.hasBoundPlayables);
        //Debug.Log("Start::anim2:: " + anim2.hasBoundPlayables);
        //Debug.Log("Start::anim3:: " + anim3.hasBoundPlayables);

        anim1.GetComponent<Animator>();
        anim2.GetComponent<Animator>();
        anim3.GetComponent<Animator>();
        som.GetComponent<AudioSource>();

        //Debug.Log("TouchDetection::Start::anim2:: " + anim2.name);
        //Debug.Log("TouchDetection::Start::anim3:: " + anim3.name);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnMouseDown()
    {
        //Debug.Log("TouchDetection::OnMouseDown:: " + gameObject.name);
        //Debug.Log("TouchDetection::OnMouseDown::anim2:: " + anim2.name);
        //Debug.Log("TouchDetection::OnMouseDown::anim3:: " + anim3.name);
        if (!isPlaying)
        {
            if (gameObject.name == "carro_circulo1")
            {
                anim1.Play("circulo1_anim");
                anim2.Play("circulo2_anim");
                anim3.Play("circulo3_anim");
            }
            if (gameObject.name == "carro_porta_2")
            {
                anim1.Play("porta_anim");
                som.Play();
            }
            if (gameObject.name == "carro_rodas_frente_2")
            {
                anim1.Play("roda_frente_anim");
            }
            if (gameObject.name == "carro_rodas_tras_2")
            {
                anim1.Play("roda_tras_anim");
            }
        }
        else
        {
            anim1.Play("none");
            if (gameObject.name == "carro_circulo1")
            {
                anim2.Play("none");
                anim3.Play("none");
            }

            if (som != null && som.isPlaying)
            {
                som.Stop();
            }
        }

        isPlaying = !isPlaying;
    }
}
