using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ RequireComponent(typeof(SpriteRenderer))]
public class image_event : MonoBehaviour
{
    //public Text texto1;

    //public Text texto2;

    //public Text texto3;

    //public Text texto4;

    Image rend;

    //SpriteRenderer rend2;

    Texture2D tex = null;

    public bool image_state;

    public CameraImageAccess CA;

    private Sprite newSprite = null;

    private FilterMode FilterMode;

    //private Material newMaterial = null;

    public int altura, largura;

    //private int frame;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Image>();
        image_state = true;
        //frame = 0;
    }

    void makeTexture(int width, int height, Color[] colorArray)
    {
        //criar uma textura
        if (tex == null) {
            tex = new Texture2D(width, height);
            tex.filterMode = FilterMode.Point;
        }

        if (tex != null)
        {
            //transfere o array para a textura e aplica os pixels     
            tex.SetPixels(colorArray);
            tex.Apply();
        }
    }

    void makeSprite()
    {
        //criar um sprite com essa textura
        //if (newSprite == null && tex != null)
        if (tex != null)
        {
            newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
            rend.sprite = newSprite;
            //texto3.text = "camera x: " + largura.ToString();
            //texto4.text = "camera y: " + altura.ToString(); 
        }

        //designiar um sprite proceduralmente para rend.sprite
        rend.material.mainTexture = tex;
        rend.material.SetTexture("nome", tex);
    }

    // Update is called once per frame
    void Update()
    {
        //Screen.SetResolution(largura, altura, true);
        //texto1.text = "tela x: " + Screen.width.ToString();
        //texto2.text = "tela y: " + Screen.height.ToString();
        // pula os frames pares para economizar processamento.
        //if (((frame++) % 2) != 0)
        //    return;

        altura = CA.redHeight;
        largura = CA.redWidth;
        if (/*image_state && */CA != null && CA.colorArray != null && CA.redWidth > 0)
        {
            makeTexture(largura, altura, CA.colorArray);
            makeSprite();
        }
    }
}
