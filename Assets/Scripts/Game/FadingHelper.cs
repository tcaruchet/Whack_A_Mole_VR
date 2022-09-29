using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingHelper : MonoBehaviour
{
    [Header("Canvas Group fading parameters")]
    [SerializeField]
    [Tooltip("Drag and drop canvas group here")]
    private CanvasGroup CGToFade; // Used to make disappear all elements linked to a canvas

    [SerializeField]
    [Range(.01f, 2f)]
    [Tooltip("Canvas group fade in speed ratio")]
    private float CGFadeInSpeed = 1f; 

    [SerializeField]
    [Range(.01f, 2f)]
    [Tooltip("Canvas group fade out speed ratio")]
    private float CGFadeOutSpeed = 1f;

    [Header("Text fading parameters")]
    [SerializeField]
    [Tooltip("Drag and drop texts elements from UI and Canvas here")]
    private Text txtToFade; //  Used to make UI text disappear

    [SerializeField]
    [Range(.01f, 2f)]
    [Tooltip("Text fade in speed ratio")]
    private float txtFadeInSpeed = 1f;

    [SerializeField]
    [Range(.01f, 2f)]
    [Tooltip("Text fade out speed ratio")]
    private float txtFadeOutSpeed = 1f;

    [Header("Image fading parameters")]
    [SerializeField]
    [Tooltip("Drag and drop image from UI elements or sprites here")]
    private Image imgToFade; //  Used to make UI image disappear

    [SerializeField]
    [Range(.01f, 2f)]
    [Tooltip("Image fade in speed ratio")]
    private float imgFadeInSpeed = 1f;

    [SerializeField]
    [Range(.01f, 2f)]
    [Tooltip("Image fade out speed ratio")]
    private float imgFadeOutSpeed = 1f;

    [Header("2D object fading parameters")]
    [SerializeField]
    [Tooltip("Drag and drop 2D object equipped with a renderer here")]
    private Renderer rendererToFade2D; //  Used to make any 2D gameObject with a renderer disappear

    [SerializeField]
    [Range(.01f, 2f)]
    [Tooltip("2D renderer fade in speed ratio")]
    private float r2DFadeInSpeed = 1f;

    [SerializeField]
    [Range(.01f, 2f)]
    [Tooltip("2D renderer fade out speed ratio")]
    private float r2DFadeOutSpeed = 1f;

    [Header("3D object fading parameters")]
    [SerializeField]
    [Tooltip("To fade a 3D object we need to access its renderer and material")]
    private Renderer rendererToFade3D; // Used to make disappear 3D objects from the scene

    [SerializeField]
    [Tooltip("To fade a 3D object we need to access its renderer and material")]
    private Material materialToFade3D; //  Used to make any 3D gameObject with a renderer disappear (need material's shader to Fade)

    [SerializeField]
    [Range(.01f, 2f)]
    [Tooltip("3D object fade in speed ratio")]
    private float r3DFadeInSpeed = 1f;

    [SerializeField]
    [Range(.01f, 2f)]
    [Tooltip("3D object fade out speed ratio")]
    private float r3DFadeOutSpeed = 1f;

    // Different classes doing fading in & fading out effects for different game objects
    // Most of these functions uses the alpha to make them appear / disappear


    // [2D Object functions]
    public IEnumerator FadeInObject2D()
    {
        while (rendererToFade2D.material.color.a < 1)
        {
            Color objectColor = rendererToFade2D.material.color;
            float fadeAmount = objectColor.a + (r2DFadeInSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            rendererToFade2D.material.color = objectColor;

            yield return null;
        }
    }

    public IEnumerator FadeOutObject2D()
    {
        while (rendererToFade2D.material.color.a > 0)
        {
            Color objectColor = rendererToFade2D.material.color;
            float fadeAmount = objectColor.a - (r2DFadeOutSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            rendererToFade2D.material.color = objectColor;

            yield return null;
        }
    }

    // [3D Object functions]
    public IEnumerator FadeInObject3D()
    {
        while (materialToFade3D.color.a < 1)
        {
            Color newColor = materialToFade3D.color;
            newColor.a += (Time.deltaTime * r3DFadeInSpeed);

            materialToFade3D.color = newColor;
            rendererToFade3D.material = materialToFade3D;

            yield return null;
        }
    }

    public IEnumerator FadeOutObject3D()
    {
        while (materialToFade3D.color.a > 0)
        {
            Color newColor = materialToFade3D.color;
            newColor.a -= (Time.deltaTime * r3DFadeOutSpeed);

            materialToFade3D.color = newColor;
            rendererToFade3D.material = materialToFade3D;

            yield return null;
        }
    }

    // [Canvas / UI functions]
    public IEnumerator FadeInCanvasGroup()
    {
        while (CGToFade.alpha < 1)
        {
            CGToFade.alpha += (Time.deltaTime * CGFadeInSpeed);

            yield return null;
        }
    }

    public IEnumerator FadeOutCanvasGroup()
    {
        while (CGToFade.alpha > 0)
        {
            CGToFade.alpha -= (Time.deltaTime * CGFadeOutSpeed) ;

            yield return null;
        }
    }

    // [Txt / UI functions]
    public IEnumerator FadeInText()
    {
        while (txtToFade.color.a < 1)
        {
            Color objectColor = txtToFade.color;
            float fadeAmount = objectColor.a + (Time.deltaTime * txtFadeInSpeed);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            txtToFade.color = objectColor;

            yield return null;
        }
    }

    public IEnumerator FadeOutText()
    {
        while (txtToFade.color.a > 0)
        {
            Color objectColor = txtToFade.color;
            float fadeAmount = objectColor.a - (Time.deltaTime * txtFadeOutSpeed);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            txtToFade.color = objectColor;

            yield return null;
        }
    }

    // [2D Sprite / UI functions]
    public IEnumerator FadeInImage()
    {
        while (imgToFade.color.a < 1)
        {
            Color objectColor = imgToFade.color;
            float fadeAmount = objectColor.a + (Time.deltaTime * imgFadeInSpeed);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            imgToFade.color = objectColor;

            yield return null;
        }
    }

    public IEnumerator FadeOutImage()
    {
        while (imgToFade.color.a > 0)
        {
            Color objectColor = imgToFade.color;
            float fadeAmount = objectColor.a - (Time.deltaTime * imgFadeOutSpeed);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            imgToFade.color = objectColor;

            yield return null;
        }
    }
}
