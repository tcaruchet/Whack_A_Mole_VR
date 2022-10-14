using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.ComponentModel.Design;

// CALLBACKS ?
// FADING UTILS ?

public class FadingHelper : MonoBehaviour
{
    [Header("Game Object To Fade:")]
    [SerializeField]
    [Tooltip("Drag and drop any text, images, sprites, canvas groups, 2D/3D objects to fade")]
    private GameObject objToFade; // Used to make disappear all elements linked to a canvas

    [SerializeField]
    [Tooltip("Should this object fades automatically when enabled/disabled")]
    private bool playAutomatically = true;

    [Header("Speed and Delay")]
    [SerializeField]
    [Range(.01f, 2f)]
    [Tooltip("Speed at which the object should appear")]
    private float fadeInSpeed = 1f;

    [SerializeField]
    [Range(.01f, 2f)]
    [Tooltip("Speed at which the object should disappear")]
    private float fadeOutSpeed = 1f;

    [SerializeField]
    [Tooltip("Delay between the fading")]
    private float fadeDelay = 1f;

    [SerializeField]
    [Tooltip("Choose if you want the object to fade in, fade out or both")]
    private Effect fadeAction = Effect.Both;

    enum Effect // Enum to state what kind of effect we want for our object : fadingIn, fadingOut, none
    {
        FadeIn,
        FadeOut,
        Both
    }

    enum FadeStatus // Three different states : Fading In, Fading out, or nothing
    {
        fadingIn,
        fadingOut,
        none
    }

    private FadeStatus currentFadingStatus = FadeStatus.none; // Gives the current state of the game object

    private string _componentToUse = string.Empty; // Used in Awake() and identifies the component that will be used for the fading effect

    private CanvasGroup cGToFade; // Used to make disappear all elements linked to a canvas

    private Text txtToFade; // Used to make UI text disappear

    private Image imgToFade; // Used to make UI image disappear

    private SpriteRenderer spriteRendererToFade; // Used to make any 2D gameObject with a sprite renderer disappear

    private MeshRenderer meshRendererToFade; // Used to make 3D gameobject with a mesh renderer disappear

    private Material materialToFade; // Used to make any 3D gameObject with a renderer disappear (need material's shader to Fade)

    private Component[] _listComponents; // Used in Start() to recover all different components of the game object to fade

    private float baseAlpha; // Used to set alpha to base value of an object

    private float currentAlpha; // Used to compare with base alpha and triggering end of coroutines / transitions

    private int fadeInCount; // Used to count the number of time the object faded in

    private int fadeOutCount; // Used to count the number of time the object faded out

    private int enabledCount; // Used to count the number of time the object was enabled

    private int disabledCount; // Used to count the number of time the object was disabled

    // Getters & Setters & Constructors (can be used in FadingUtils static class to get the correct parameters)
    public float GetBaseAlpha()
    {
        return baseAlpha;
    }

    public float GetCurrentAlpha()
    {
        return currentAlpha;
    }

    public float GetFadeDelay()
    {
        return fadeDelay;
    }

    public GameObject GetObjectToFade()
    {
        return objToFade;
    }

    public FadingHelper GetFadingHelper()
    {
        return this;
    }

    private void SetAlphaToZero()
    {
        switch (_componentToUse)
        {
            case "CanvasGroup":
                cGToFade.alpha = 0;
                break;
            case "Image":
                imgToFade.color = new Color(imgToFade.color.r, imgToFade.color.g, imgToFade.color.b, 0);
                break;
            case "SpriteRenderer":
                spriteRendererToFade.color = new Color(spriteRendererToFade.color.r, spriteRendererToFade.color.g, spriteRendererToFade.color.b, 0);
                break;
            case "Text":
                txtToFade.color = new Color(txtToFade.color.r, txtToFade.color.g, txtToFade.color.b, 0);
                break;
            case "MeshRenderer":
                materialToFade.color = new Color(materialToFade.color.r, materialToFade.color.g, materialToFade.color.b, 0);
                break;
            default:
                Debug.Log("No element of type CanvasGroup/Image/Renderer/Text");
                break;
        }
    }

    private void SetAlphaToNormal()
    {
        switch (_componentToUse)
        {
            case "CanvasGroup":
                cGToFade.alpha = baseAlpha;
                break;
            case "Image":
                imgToFade.color = new Color(imgToFade.color.r, imgToFade.color.g, imgToFade.color.b, baseAlpha);
                break;
            case "SpriteRenderer":
                spriteRendererToFade.color = new Color(spriteRendererToFade.color.r, spriteRendererToFade.color.g, spriteRendererToFade.color.b, baseAlpha);
                break;
            case "Text":
                txtToFade.color = new Color(txtToFade.color.r, txtToFade.color.g, txtToFade.color.b, baseAlpha);
                break;
            case "MeshRenderer":
                materialToFade.color = new Color(materialToFade.color.r, materialToFade.color.g, materialToFade.color.b, baseAlpha);
                break;
            default:
                Debug.Log("No element of type CanvasGroup/Image/Renderer/Text");
                break;
        }
    }

    private void SetEntryParameters(GameObject gameObj)
    {
        objToFade = gameObj; // getting object to fade
        _listComponents = objToFade.GetComponents(typeof(Component)); // create a list of all components located inside the object to fade

        bool objectIdentified = false; // Will be set to true if a compatible component is found
        int iterator = 0; // helps getting outside of the loop if no compatible component is found, once all components have been verified

        while (objectIdentified == false || iterator < _listComponents.Length)
        {
            switch (_listComponents[iterator].GetType().Name) // we look at the name of each component of the game object
            {
                case "CanvasGroup":
                    _componentToUse = "CanvasGroup";
                    cGToFade = objToFade.GetComponent<CanvasGroup>();
                    baseAlpha = cGToFade.alpha;
                    if (playAutomatically && (fadeAction == Effect.FadeIn || fadeAction == Effect.Both))
                    {
                        SetAlphaToZero();
                    }
                    else if (playAutomatically && fadeAction == Effect.FadeOut)
                    {
                        SetAlphaToNormal();
                    }
                    objectIdentified = true;
                    break;
                case "Image":
                    _componentToUse = "Image";
                    imgToFade = objToFade.GetComponent<Image>();
                    baseAlpha = imgToFade.color.a;
                    if (playAutomatically && (fadeAction == Effect.FadeIn || fadeAction == Effect.Both))
                    {
                        SetAlphaToZero();
                    }
                    else if (playAutomatically && fadeAction == Effect.FadeOut)
                    {
                        SetAlphaToNormal();
                    }
                    objectIdentified = true;
                    break;
                case "SpriteRenderer":
                    _componentToUse = "SpriteRenderer";
                    spriteRendererToFade = objToFade.GetComponent<SpriteRenderer>();
                    baseAlpha = spriteRendererToFade.color.a;
                    if (playAutomatically && (fadeAction == Effect.FadeIn || fadeAction == Effect.Both))
                    {
                        SetAlphaToZero();
                    }
                    else if (playAutomatically && fadeAction == Effect.FadeOut)
                    {
                        SetAlphaToNormal();
                    }
                    objectIdentified = true;
                    break;
                case "Text":
                    _componentToUse = "Text";
                    txtToFade = objToFade.GetComponent<Text>();
                    baseAlpha = txtToFade.color.a;
                    if (playAutomatically && (fadeAction == Effect.FadeIn || fadeAction == Effect.Both))
                    {
                        SetAlphaToZero();
                    }
                    else if (playAutomatically && fadeAction == Effect.FadeOut)
                    {
                        SetAlphaToNormal();
                    }
                    objectIdentified = true;
                    break;
                case "MeshRenderer":
                    _componentToUse = "MeshRenderer";
                    meshRendererToFade = objToFade.GetComponent<MeshRenderer>();
                    materialToFade = meshRendererToFade.material;
                    baseAlpha = meshRendererToFade.material.color.a;
                    if (playAutomatically && (fadeAction == Effect.FadeIn || fadeAction == Effect.Both))
                    {
                        SetAlphaToZero();
                    }
                    else if (playAutomatically && fadeAction == Effect.FadeOut)
                    {
                        SetAlphaToNormal();
                    }
                    objectIdentified = true;
                    break;
                default:
                    Debug.Log("Element " + _listComponents[iterator].GetType().Name + " is not CanvasGroup/Image/Renderer/Text");
                    break;
                case null:
                    Debug.Log("No components detected");
                    throw new ArgumentNullException(nameof(objToFade));
            }
            iterator++;
        }
        if (objectIdentified == false && objToFade != null) // No compatible items plus no game object linked
        {
            Debug.Log("NO COMPATIBLE COMPONENT FOUND, HAVE YOU LINKED A GAME OBJECT TO THE SCRIPT ?");
        }
    }

    // Setting up
    void Awake()
    {
        objToFade = gameObject; // getting object to fade
        _listComponents = objToFade.GetComponents(typeof(Component)); // create a list of all components located inside the object to fade

        bool objectIdentified = false; // Will be set to true if a compatible component is found
        int iterator = 0; // helps getting outside of the loop if no compatible component is found, once all components have been verified

        while (objectIdentified == false || iterator < _listComponents.Length)
        {
            switch (_listComponents[iterator].GetType().Name) // we look at the name of each component of the game object
            {
                case "CanvasGroup":
                    _componentToUse = "CanvasGroup";
                    cGToFade = objToFade.GetComponent<CanvasGroup>();
                    baseAlpha = cGToFade.alpha;
                    if (playAutomatically && (fadeAction == Effect.FadeIn || fadeAction == Effect.Both))
                    {
                        SetAlphaToZero();
                    }
                    else if (playAutomatically && fadeAction == Effect.FadeOut)
                    {
                        SetAlphaToNormal();
                    }
                    objectIdentified = true;
                    break;
                case "Image":
                    _componentToUse = "Image";
                    imgToFade = objToFade.GetComponent<Image>();
                    baseAlpha = imgToFade.color.a;
                    if (playAutomatically && (fadeAction == Effect.FadeIn || fadeAction == Effect.Both))
                    {
                        SetAlphaToZero();
                    }
                    else if (playAutomatically && fadeAction == Effect.FadeOut)
                    {
                        SetAlphaToNormal();
                    }
                    objectIdentified = true;
                    break;
                case "SpriteRenderer":
                    _componentToUse = "SpriteRenderer";
                    spriteRendererToFade = objToFade.GetComponent<SpriteRenderer>();
                    baseAlpha = spriteRendererToFade.color.a;
                    if (playAutomatically && (fadeAction == Effect.FadeIn || fadeAction == Effect.Both))
                    {
                        SetAlphaToZero();
                    }
                    else if (playAutomatically && fadeAction == Effect.FadeOut)
                    {
                        SetAlphaToNormal();
                    }
                    objectIdentified = true;
                    break;
                case "Text":
                    _componentToUse = "Text";
                    txtToFade = objToFade.GetComponent<Text>();
                    baseAlpha = txtToFade.color.a;
                    if (playAutomatically && (fadeAction == Effect.FadeIn || fadeAction == Effect.Both))
                    {
                        SetAlphaToZero();
                    }
                    else if (playAutomatically && fadeAction == Effect.FadeOut)
                    {
                        SetAlphaToNormal();
                    }
                    objectIdentified = true;
                    break;
                case "MeshRenderer":
                    _componentToUse = "MeshRenderer";
                    meshRendererToFade = objToFade.GetComponent<MeshRenderer>();
                    materialToFade = meshRendererToFade.material;
                    baseAlpha = meshRendererToFade.material.color.a;
                    if (playAutomatically && (fadeAction == Effect.FadeIn || fadeAction == Effect.Both))
                    {
                        SetAlphaToZero();
                    }
                    else if (playAutomatically && fadeAction == Effect.FadeOut)
                    {
                        SetAlphaToNormal();
                    }
                    objectIdentified = true;
                    break;
                default:
                    Debug.Log("Element " + _listComponents[iterator].GetType().Name + " is not CanvasGroup/Image/Renderer/Text");
                    break;
                case null:
                    Debug.Log("No components detected");
                    throw new ArgumentNullException(nameof(objToFade));
            }
            iterator++;
        }
        if (objectIdentified == false && objToFade != null) // No compatible items plus no game object linked
        {
            Debug.Log("NO COMPATIBLE COMPONENT FOUND, HAVE YOU LINKED A GAME OBJECT TO THE SCRIPT ?");
        }
    }

    // Different classes doing fading in & fading out effects for different game objects
    // Most of these functions uses the alpha to make them appear / disappear

    // [2D Object functions] works with Sprite renderer
    public IEnumerator FadeInObject2D()
    {
        float fadeTime = 0;
        SetAlphaToZero();

        while (spriteRendererToFade.material.color.a < baseAlpha || fadeTime < fadeDelay)
        {
            Color objectColor = spriteRendererToFade.material.color;
            float fadeAmount = objectColor.a + (fadeInSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            spriteRendererToFade.material.color = objectColor;

            fadeTime += Time.deltaTime;

            yield return null;
        }

        CleanCoroutine(MethodBase.GetCurrentMethod().Name);
    }

    public IEnumerator FadeOutObject2D()
    {
        float fadeTime = 0;
        SetAlphaToNormal();

        while (spriteRendererToFade.material.color.a > 0 || fadeTime < fadeDelay)
        {
            Color objectColor = spriteRendererToFade.material.color;
            float fadeAmount = objectColor.a - (fadeOutSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            spriteRendererToFade.material.color = objectColor;

            fadeTime += Time.deltaTime;

            yield return null;
        }

        CleanCoroutine(MethodBase.GetCurrentMethod().Name);
    }

    public IEnumerator FadeRoutineObject2D()
    {
        yield return FadeInObject2D();

        if (fadeDelay != 0)
        {
            yield return new WaitForSeconds(fadeDelay);
        }

        yield return FadeOutObject2D();
    }

    // [3D Object functions] works with Mesh renderer
    public IEnumerator FadeInObject3D()
    {
        float fadeTime = 0;
        SetAlphaToZero();

        while (materialToFade.color.a < baseAlpha || fadeTime < fadeDelay)
        {
            Color newColor = materialToFade.color;
            newColor.a += (Time.deltaTime * fadeInSpeed);

            materialToFade.color = newColor;
            meshRendererToFade.material = materialToFade;

            fadeTime += Time.deltaTime;

            yield return null;
        }

        CleanCoroutine(MethodBase.GetCurrentMethod().Name);
    }

    public IEnumerator FadeOutObject3D()
    {
        float fadeTime = 0;
        SetAlphaToNormal();

        while (materialToFade.color.a > 0 || fadeTime < fadeDelay)
        {
            Color newColor = materialToFade.color;
            newColor.a -= (Time.deltaTime * fadeOutSpeed);

            materialToFade.color = newColor;
            meshRendererToFade.material = materialToFade;

            fadeTime += Time.deltaTime;

            yield return null;
        }

        CleanCoroutine(MethodBase.GetCurrentMethod().Name);
    }

    public IEnumerator FadeRoutineObject3D()
    {
        yield return FadeInObject3D();

        if (fadeDelay != 0)
        {
            yield return new WaitForSeconds(fadeDelay);
        }

        yield return FadeOutObject3D();
    }

    // [Canvas / UI functions]
    public IEnumerator FadeInCanvasGroup()
    {
        float fadeTime = 0;
        SetAlphaToZero();

        while (cGToFade.alpha < baseAlpha || fadeTime < fadeDelay)
        {
            cGToFade.alpha += (Time.deltaTime * fadeInSpeed);
            fadeTime += Time.deltaTime;

            yield return null;
        }

        CleanCoroutine(MethodBase.GetCurrentMethod().Name);
    }

    public IEnumerator FadeOutCanvasGroup()
    {
        float fadeTime = 0;
        SetAlphaToNormal();

        while (cGToFade.alpha > 0 || fadeTime < fadeDelay)
        {
            cGToFade.alpha -= (Time.deltaTime * fadeOutSpeed);

            fadeTime += Time.deltaTime;

            yield return null;
        }

        CleanCoroutine(MethodBase.GetCurrentMethod().Name);
    }

    public IEnumerator FadeRoutineCanvasGroup()
    {
        yield return FadeInCanvasGroup();

        if (fadeDelay != 0)
        {
            yield return new WaitForSeconds(fadeDelay);
        }

        yield return FadeOutCanvasGroup();
    }

    // [Txt / UI functions]
    public IEnumerator FadeInText()
    {
        float fadeTime = 0;
        SetAlphaToZero();

        while (txtToFade.color.a < baseAlpha || fadeTime < fadeDelay)
        {
            Color objectColor = txtToFade.color;
            float fadeAmount = objectColor.a + (Time.deltaTime * fadeInSpeed);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            txtToFade.color = objectColor;

            fadeTime += Time.deltaTime;

            yield return null;
        }

        CleanCoroutine(MethodBase.GetCurrentMethod().Name);
    }

    public IEnumerator FadeOutText()
    {
        float fadeTime = 0;
        SetAlphaToNormal();

        while (txtToFade.color.a > 0 || fadeTime < fadeDelay)
        {
            Color objectColor = txtToFade.color;
            float fadeAmount = objectColor.a - (Time.deltaTime * fadeOutSpeed);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            txtToFade.color = objectColor;

            fadeTime += Time.deltaTime;

            yield return null;
        }

        CleanCoroutine(MethodBase.GetCurrentMethod().Name);
    }

    public IEnumerator FadeRoutineText()
    {
        yield return FadeInText();

        if (fadeDelay != 0)
        {
            yield return new WaitForSeconds(fadeDelay);
        }

        yield return FadeOutText();
    }

    // [2D Sprite / Images / UI functions]
    public IEnumerator FadeInImage()
    {
        float fadeTime = 0;
        SetAlphaToZero();

        while (imgToFade.color.a < baseAlpha || fadeTime < fadeDelay)
        {
            Color objectColor = imgToFade.color;
            float fadeAmount = objectColor.a + (Time.deltaTime * fadeInSpeed);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            imgToFade.color = objectColor;

            fadeTime += Time.deltaTime;

            yield return null;
        }

        CleanCoroutine(MethodBase.GetCurrentMethod().Name);
    }

    public IEnumerator FadeOutImage()
    {
        float fadeTime = 0;
        SetAlphaToNormal();

        while (imgToFade.color.a > 0 || fadeTime < fadeDelay)
        {
            Color objectColor = imgToFade.color;
            float fadeAmount = objectColor.a - (Time.deltaTime * fadeOutSpeed);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            imgToFade.color = objectColor;

            fadeTime += Time.deltaTime;

            yield return null;
        }

        CleanCoroutine(MethodBase.GetCurrentMethod().Name);
    }

    public IEnumerator FadeRoutineImage()
    {
        yield return FadeInImage();

        if (fadeDelay != 0)
        {
            yield return new WaitForSeconds(fadeDelay);
        }

        yield return FadeOutImage();
    }

    // General purpose Fade routines, will use the correct coroutine corresponding to the correct component
    public void FadeInGameObject() // will pick the right Fade In function depending on the component
    {
        switch (_componentToUse)
        {
            case "CanvasGroup":
                StartCoroutine(FadeInCanvasGroup());
                break;
            case "Image":
                StartCoroutine(FadeInImage());
                break;
            case "SpriteRenderer":
                StartCoroutine(FadeInObject2D());
                break;
            case "MeshRenderer":
                StartCoroutine(FadeInObject3D());
                break;
            case "Text":
                StartCoroutine(FadeInText());
                break;
            default:
                Debug.Log("Element " + _componentToUse + " is not CanvasGroup/Image/Renderer/Text");
                break;
            case null:
                Debug.Log("No components detected");
                throw new ArgumentNullException(nameof(objToFade));
        }
    }

    public void FadeOutGameObject() // will pick the right Fade Out function depending on the component
    {
        switch (_componentToUse)
        {
            case "CanvasGroup":
                StartCoroutine(FadeOutCanvasGroup());
                break;
            case "Image":
                StartCoroutine(FadeOutImage());
                break;
            case "SpriteRenderer":
                StartCoroutine(FadeOutObject2D());
                break;
            case "MeshRenderer":
                StartCoroutine(FadeOutObject3D());
                break;
            case "Text":
                StartCoroutine(FadeOutText());
                break;
            default:
                Debug.Log("Element " + _componentToUse + " is not CanvasGroup/Image/Renderer/Text");
                break;
            case null:
                Debug.Log("No components detected");
                throw new ArgumentNullException(nameof(objToFade));
        }
    }

    public void FadeRoutineGameObject() // will pick the Fade In & Out function depending on the component
    {
        switch (_componentToUse)
        {
            case "CanvasGroup":
                StartCoroutine(FadeRoutineCanvasGroup());
                break;
            case "Image":
                StartCoroutine(FadeRoutineImage());
                break;
            case "SpriteRenderer":
                StartCoroutine(FadeRoutineObject2D());
                break;
            case "MeshRenderer":
                StartCoroutine(FadeRoutineObject3D());
                break;
            case "Text":
                StartCoroutine(FadeRoutineText());
                break;
            default:
                Debug.Log("Element " + _componentToUse + " is not CanvasGroup/Image/Renderer/Text");
                break;
            case null:
                Debug.Log("No components detected");
                throw new ArgumentNullException(nameof(objToFade));
        }
    }

    // This next function calls the correct function depending of the enum type "Effect" (see inspector for setup)
    public void FadeFromAction()
    {
        switch (fadeAction)
        {
            case Effect.FadeIn:
                FadeInGameObject();
                break;
            case Effect.FadeOut:
                FadeOutGameObject();
                break;
            case Effect.Both:
                FadeRoutineGameObject();
                break;
        }
    }
    

    // Cleaning up function
    public void CleanCoroutine(string coroutineName)
    {
        StopCoroutine(coroutineName);
    }

    void Update()
    {
        //testInput();
    }

    // Test functions, here is all the different functions defined above and activable with differents keystrokes
    private void TestInput()
    {
 /*       if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(FadeOutObject2D());
            Debug.Log("FadeOutObject2D");
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(FadeInObject2D());
            Debug.Log("FadeInObject2D");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(FadeOutCanvasGroup());
            Debug.Log("FadeOutCanvasGroup");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(FadeInCanvasGroup());
            Debug.Log("FadeInCanvasGroup");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(FadeOutObject3D());
            Debug.Log("FadeOutObject3D");
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartCoroutine(FadeInObject3D());
            Debug.Log("FadeInObject3D");
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(FadeOutText());
            Debug.Log("FadeOutText");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            StartCoroutine(FadeInText());
            Debug.Log("FadeInText");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(FadeOutImage());
            Debug.Log("FadeOutImage");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(FadeInImage());
            Debug.Log("FadeInImage");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(FadeRoutineObject2D());
            Debug.Log("FadeRoutineObject2D");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(FadeRoutineImage());
            Debug.Log("FadeRoutineImage");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(FadeRoutineObject3D());
            Debug.Log("FadeRoutineObject3D");

        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(FadeRoutineText());
            Debug.Log("FadeRoutineObject3D");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(FadeRoutineCanvasGroup());
            Debug.Log("FadeRoutineCanvasGroup");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("FadeInGameObject");
            FadeInGameObject();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("FadeOutObject");
            FadeOutGameObject();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("FadeRoutineGameObject");
            FadeRoutineGameObject();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("FadeFromAction");
            FadeFromAction();
        }*/
    }
}
