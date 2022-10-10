using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class SplashScreenFading : MonoBehaviour
{
    [SerializeField]
    private Color32 _colorGrey = new Color32(76, 76, 76, 255); // Setting starting grey color from Unity starting logo

    [SerializeField]
    private Color32 _colorBlack = new Color32(0, 0, 0, 255); // Setting starting black color after Unity starting logo

    [SerializeField]
    private float _startingFadeDuration = 1.5f; // Setting starting fade effect duration

    [SerializeField]
    private Image _therapistScreenHider; // Used to make UI image disappear ("Therapist UI -> HidingCanvas -> FadeFromGrey")

    // [starting VR functions]
    // This function is used at start to do Therapist screen fade-in fade-out effect
    public IEnumerator FadeOutTherapistScreen(float delay = 0.0f)
    {
        if (delay != 0)
        {
            yield return new WaitForSeconds(delay);
        }

        while (_therapistScreenHider.color.a > 0)
        {
            Color objectColor = _therapistScreenHider.color;
            float fadeAmount = objectColor.a - (Time.deltaTime * 0.6f);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            _therapistScreenHider.color = objectColor;

            yield return null;
        }

        _therapistScreenHider.gameObject.SetActive(false); // Disabling the grey screen for therapist so he can click on his interface
    }

    // This function uses Steam_VR built-in way to do fading effects
    public IEnumerator FadeFromGreyToBlackPatient(Color32 colorS, Color32 colorE, float duration) // Used for VR headset fade starting effect
    {
        SteamVR_Fade.View(colorS, 0f); // set start color
        SteamVR_Fade.View(colorE, duration); // set duration start fade to normal

        yield return null;
    }

    // This function uses Steam_VR built-in way to do fading effects
    public IEnumerator FadeFromBlackToNormalPatient(float duration) // Used for VR headset fade starting effect
    {
        SteamVR_Fade.View(Color.black, 0f);  // set start color (black)
        SteamVR_Fade.View(Color.clear, duration); // set duration and start fade to normal

        yield return null;
    }

    // This function is used at start to do Patient screen fade-in fade-out effect
    public IEnumerator StartingCoroutine(Color32 colorS, Color32 colorE, float duration, float delay = 0f)
    {
        yield return FadeFromGreyToBlackPatient(colorS, colorE, duration);

        if (delay != 0)
        {
            yield return new WaitForSeconds(delay);
        }

        yield return FadeFromBlackToNormalPatient(duration);
    }

    public void Start()
    {
        StartCoroutine(StartingCoroutine(_colorGrey, _colorBlack, _startingFadeDuration, 1f)); // Patient screen fade-in fade-out
        StartCoroutine(FadeOutTherapistScreen(0.1f)); // Therapist screen fade-in fade-out
    }

}
