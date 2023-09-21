#undef UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System;

// Cor01
public class CameraImageAccess : MonoBehaviour
{
    #region PUBLIC_MEMBERS

    public Color[] colorArray = null;

    public int redWidth, redHeight; // reduzido

    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS
    private PixelFormat mPixelFormat = PixelFormat.UNKNOWN_FORMAT;
    private Vuforia.Image image;
    private byte[] pixels;
    private bool mAccessCameraImage = true;
    private bool mFormatRegistered = false;
    private int mPixelSize = 1;
    private bool firstFrame = true;
    private int reductionFactor;

    private int internalSpace;
    private int externalSpace;
    private int topMargin;
    private int diffY;
    private int diffX;
    private float yFactorL = 1; // olho esquerda
    private float yFactorR = 1; // olho direita
    private float xFactorR = 1; // olho direita

    private int oX, oY; // original
    private int nX_L, nY_L; // new left
    private int nX_R, nY_R; // new right
    private int lineStride;
    private int ind, incStep;
    private int size;
    private int newWidth;
    private float ratioX_L, ratioX_R, ratioY_L, ratioY_R;
    private int noSkinBorderX, noSkinBorderY;
    private byte R, G, B;
    private Color c = Color.red;
    //private Color c = Color.green;

    #endregion // PRIVATE_MEMBERS

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        mPixelFormat = PixelFormat.GRAYSCALE; // Need Grayscale for Editor
        mPixelSize = 1;
#else
        mPixelFormat = PixelFormat.RGB888; // Use RGB888 for mobile
        mPixelSize = 3;
        reductionFactor = 1;
#endif
        // Register Vuforia life-cycle callbacks (forma antiga):
        //VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        //VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
        //VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);
        // Register Vuforia life-cycle callbacks (forma nova):
        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
        VuforiaApplication.Instance.OnVuforiaPaused += OnPause;
        VuforiaBehaviour.Instance.World.OnStateUpdated += OnTrackablesUpdated;

        //c.a = 0.2f; // muito transparente.
        //c.a = 0.5f; // medio transparente.
        //c.a = 0.8f; // pouco transparente.

        // Conf. com a camera em scale 1.0. E Image com escale 1.0. OK
        //int internalSpace = 190;
        //int externalSpace = 72;
        //int topMargin = 313;
        //int diffX = 3;
        //int diffY = -3;

        // Conf. com a camera em scale 1.24. E Image com escale 1.026
        //int internalSpace = 0;
        //int externalSpace = 5;
        //int topMargin = 265;
        //int diffY = -3;
        //int diffX = 12;

        // Conf. com a camera em scale 1.8. E Image com escale 1.115
        //int internalSpace = 0;
        //int externalSpace = -310;
        //int topMargin = 170;
        //int diffX = 0;
        //int diffY = 3;

        // Conf.com a camera em scale 1.21 E Image com escale 1.05.
        if (reductionFactor == 1)
        {
            //internalSpace = 34; // conf 1
            //externalSpace = 38;
            //topMargin = 282;
            //diffX = 2;
            internalSpace = 98; // conf 2
            externalSpace = 64;
            topMargin = 210;
            diffY = -2;
            diffX = 99;
            //internalSpace = 30; // conf 3
            //externalSpace = 36;
            //diffX = 2;

            yFactorL = 1.007f; // olho esquerda
            yFactorR = 1; // olho direita
            xFactorR = 0.997f; // olho direita

            noSkinBorderX = 200;
            noSkinBorderY = 50;
        }
        if (reductionFactor == 2) // TOU NESSE
        {
            //internalSpace = 56;  // Conf 1 (maior)
            //externalSpace = 17;
            //topMargin = 139;
            //diffY = -1;
            //diffX = 13;

            //internalSpace = 82;  // Conf 2 (menor)
            //externalSpace = 21;
            //topMargin = 141;
            //diffY = -1;
            //diffX = 22;

            //internalSpace = 86;  // Conf 3 (menor ainda)
            //externalSpace = 23;
            //topMargin = 141;
            //diffY = -1;
            //diffX = 24;

            internalSpace = 162;  // Conf Cor 1
            externalSpace = 31;
            topMargin = 105;
            diffY = -1; // olho da direita
            diffX = 50; // olho da direita

            yFactorL = 0.995f; // olho esquerda
            yFactorR = 0.992f; // olho direita
            xFactorR = 0.992f; // olho direita

            noSkinBorderX = 100;
            noSkinBorderY = 25;
        }
#if UNITY_EDITOR
        noSkinBorderX = 0;
        noSkinBorderY = 0;
#endif
    }

    void OnVuforiaStarted()
    {
        //VuforiaBehaviour.Instance.CameraDevice.SetCameraMode(CameraMode.MODE_OPTIMIZE_SPEED);
        VuforiaBehaviour.Instance.CameraDevice.SetFocusMode(FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        //Unity.XR.MockHMD.MockHMD.SetEyeResolution(400, 300);

        // Try register camera image format
        if (VuforiaBehaviour.Instance.CameraDevice.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError(
                "\nFailed to register pixel format: " + mPixelFormat.ToString() +
                "\nThe format may be unsupported by your device." +
                "\nConsider using a different pixel format.\n");
            mFormatRegistered = false;
        }
    }

    void OnDestroy()
    {
        // Unregister Vuforia life-cycle callbacks:
        VuforiaApplication.Instance.OnVuforiaStarted -= RegisterFormat;
        VuforiaApplication.Instance.OnVuforiaPaused -= OnPause;
        if (VuforiaBehaviour.Instance.World != null)
            VuforiaBehaviour.Instance.World.OnStateUpdated -= OnTrackablesUpdated;
    }

    /// 
    /// Called each time the Vuforia state is updated
    /// 
    void OnTrackablesUpdated()
    {
        if (mFormatRegistered)
        {
            if (mAccessCameraImage)
            {
                image = VuforiaBehaviour.Instance.CameraDevice.GetCameraImage(mPixelFormat);
                redHeight = image.Height / reductionFactor;
                redWidth = image.Width / reductionFactor;

                if (!firstFrame)
                {
                    pixels = image.Pixels;
                    if (image.Pixels != null && image.Pixels.Length > 0)
                    {
                        // Limpa o colorArray antes de desenhar.
                        Array.Clear(colorArray, 0, colorArray.Length);
                        //desenhaBorda();
                        DetectFinger();
                    }
                }
                else if (firstFrame && image.Height > 0)
                {
                    Debug.Log("\nfirstFrame::Image Format: " + image.PixelFormat);
                    Debug.Log("\nImage Size:   " + image.Width + "x" + image.Height);
                    Debug.Log("\nBuffer Size:  " + image.BufferWidth + "x" + image.BufferHeight);
                    Debug.Log("\nImage Stride: " + image.Stride);
                    firstFrame = false;
                    colorArray = new Color[redWidth * redHeight];

                    lineStride = image.Width * mPixelSize;
                    incStep = mPixelSize * reductionFactor;
                    size = lineStride * image.Height;
                    newWidth = (int)(image.Width - internalSpace - externalSpace * 2) / (2 * reductionFactor);
                    ratioX_L = newWidth / (float)image.Width;
                    ratioX_R = ratioX_L * xFactorR;
                    ratioY_L = ratioX_L * yFactorL;
                    ratioY_R = ratioX_L * yFactorR;
                }
            }
        }
    }

    void desenhaBorda()
    {
        int y, x;
        // desenha 2 linhas horizontais em cima e duas embaixo
        for (x = 0; x < redWidth; x++)
        {
            y = 0;
            colorArray[x + y * redWidth] = Color.red;
            y = 1;
            colorArray[x + y * redWidth] = Color.red;

            y = redHeight - 1;
            colorArray[x + y * redWidth] = Color.red;
            y = redHeight - 2;
            colorArray[x + y * redWidth] = Color.red;
            if (x % 50 == 0)
                x += 50;
        }

        // desenha 2 linhas verticais na direita e esquerda
        for (y = 0; y < redHeight; y++)
        {
            x = 0;
            colorArray[x + y * redWidth] = Color.red;
            x = 1;
            colorArray[x + y * redWidth] = Color.red;

            x = redWidth - 1;
            colorArray[x + y * redWidth] = Color.red;
            x = redWidth - 2;
            colorArray[x + y * redWidth] = Color.red;
            if (y % 50 == 0)
                y += 50;
        }
    }

    // Fake function, just for testing on preview enviroment 
    bool isSkinGrayscale(byte pixel)
    {
        //if (pixel > 30 && pixel < 170)
        if (pixel > 60 && pixel < 100)
            return true;
        else
            return false;
    }

    bool isSkinColor()
    {
        R = pixels[ind];
        G = pixels[ind + 1];
        B = pixels[ind + 2];
        if ((R > 95 && G > 40 && B > 20 &&
            (Mathf.Max(Mathf.Max(R, G), B) - Mathf.Min(Mathf.Min(R, G), B) > 15) &&
            Mathf.Abs(R - G) > 15 &&
            R > G && R > B)
            ||
            (R > 220 && G > 210 && B > 170 &&
            Mathf.Abs(R - G) <= 15 &&
            R > B && G > B))
        {
            return true;
        } else {
            return false;
        }
    }

    void DetectFinger()
    {
        int linePixels;
        int maxWidth = image.Width - noSkinBorderX;
        int maxHeight = image.Height - noSkinBorderY;
        for (oY = noSkinBorderY; oY < maxHeight; oY++)
        {
            linePixels = oY * lineStride;
            for (oX = noSkinBorderX; oX < maxWidth; oX++)
            {
                ind = oX * mPixelSize + linePixels;
#if UNITY_EDITOR
                if (isSkinGrayscale(pixels[ind]))
                {
                    colorArray[ind] = c;
                }
#else
                if (isSkinColor())
                {
                    c.r = pixels[ind] / 255f;
                    c.g = pixels[ind + 1] / 255f;
                    c.b = pixels[ind + 2] / 255f;

                    nX_L = (int)(oX * ratioX_L);
                    nY_L = (int)(oY * ratioY_L);
                    nX_R = (int)(oX * ratioX_R);
                    nY_R = (int)(oY * ratioY_R);

                    // Olho da Esquerda (Left)
                    nX_L += externalSpace;
                    nY_L += topMargin;
                    colorArray[nX_L + nY_L * redWidth] = c;

                    // Olho da Direita (Right)
                    nX_R += (externalSpace + newWidth + diffX);
                    nY_R += (topMargin + diffY);
                    colorArray[nX_R + nY_R * redWidth] = c;
                }
#endif
            }
        }
    }

    void DetectFingerOld()
    {
        // TODO verificar se fizer um for dentro do outro, representando as linhas e colunas fica mais rapido.
        // Vai economiar no calculo do oX e oY sem precisar do mod ou div.
        for (ind = 0; ind < size; ind += incStep)
        {
#if UNITY_EDITOR
            if (isSkinGrayscale(pixels[ind]))
            {
                colorArray[ind] = c;
            }
#else
            if (ind % lineStride == 0)
            {
                ind += lineStride * (reductionFactor / 2);
            }

            if (isSkinColor())
            {
                //c = new Color(pixels[i] / 255f, pixels[i + 1] / 255f, pixels[i + 2] / 255f);
                c.r = pixels[ind] / 255f;
                c.g = pixels[ind + 1] / 255f;
                c.b = pixels[ind + 2] / 255f;
                // TODO se quiser testar com verde comenta essas 3 linhas anteriores

                oX = (ind % lineStride) / mPixelSize;
                oY = ind / lineStride;

                //nX_L  = (int)Mathf.Round(oX * ratioX_L);
                //nY_L  = (int)Mathf.Round(oY * ratioY_L);
                //nX_R = (int)Mathf.Round(oX * ratioX_R);
                //nY_R = (int)Mathf.Round(oY * ratioY_R);
                nX_L = (int)(oX * ratioX_L);
                nY_L = (int)(oY * ratioY_L);
                nX_R = (int)(oX * ratioX_R);
                nY_R = (int)(oY * ratioY_R);

                // Olho da Esquerda (Left)
                nX_L += externalSpace;
                nY_L += topMargin;
                colorArray[nX_L + nY_L * redWidth] = c;

                // Olho da Direita (Right)
                nX_R += (externalSpace + newWidth + diffX);
                nY_R += (topMargin + diffY);
                colorArray[nX_R + nY_R * redWidth] = c;
            }
#endif
        }
    }

    /// 
    /// Called when app is paused / resumed
    /// 
    void OnPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("App was paused");
            UnregisterFormat();
        } else {
            Debug.Log("App was resumed");
            RegisterFormat();

        }
    }

    /// 
    /// Register the camera pixel format
    /// 
    void RegisterFormat()
    {
        VuforiaBehaviour.Instance.CameraDevice.SetFocusMode(FocusMode.FOCUS_MODE_CONTINUOUSAUTO);

        if (VuforiaBehaviour.Instance.CameraDevice.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = false;
        }
    }

    /// 
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// 
    void UnregisterFormat()
    {
        Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        VuforiaBehaviour.Instance.CameraDevice.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
