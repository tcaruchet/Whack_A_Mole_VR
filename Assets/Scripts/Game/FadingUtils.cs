using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// This utility class is used to enable & disable game objects holding FadeHelper class script outside from the FadeHelper class itself

namespace FadingUtils
{
    public static class FadingUtils
    {
        public static void FadingOutPlusDisabling(FadingHelper fadeHelper, float fadeDelay=0f) // this function if called will make a fade out effect then disable the item
        {
            float fadeTime = 0;
            fadeHelper.FadeOutGameObject();

            while (fadeTime < fadeDelay)
            {
                fadeTime += Time.deltaTime;
            }

            fadeHelper.gameObject.SetActive(false);
        }

        public static bool FadingOutPlusDisablingCB(FadingHelper fadeHelper, float fadeDelay=0f) // this function if called will make a fade out effect then disable the item
        {
            float fadeTime = 0;
            fadeHelper.FadeOutGameObject();

            while (fadeTime < fadeDelay)
            {
                fadeTime += Time.deltaTime;
            }

            return true;//fadeHelper.gameObject.SetActive(false);
        }

        public static void FadingInPlusEnabling(FadingHelper fadeHelper) // this function if called will enable an item then make a fade in effect
        {
            fadeHelper.gameObject.SetActive(true);
            fadeHelper.FadeInGameObject();
        }

        public static void FadingRoutinePlusEnablingThenDisabling(FadingHelper fadeHelper, float fadeDelay = 0f)  // this function if called will enable an item, make a fade in effect, wait then make fade out effect, then disable the item
        {
            float fadeTime = 0f;

            fadeHelper.gameObject.SetActive(true); // Fading in
            fadeHelper.FadeInGameObject();

            while (fadeTime < fadeDelay)
            {
                fadeTime += Time.deltaTime;
            }

            fadeTime = 0f;
            fadeHelper.FadeOutGameObject(); // Fading out
            while (fadeTime < fadeDelay)
            {
                fadeTime += Time.deltaTime;
            }
            fadeHelper.gameObject.SetActive(false);
        }

        public static FadingHelper GetFadingHelper(GameObject usedGameObject)  // this function if called will enable an item, make a fade in effect, wait then make fade out effect, then disable the item
        {
            return usedGameObject.GetComponent<FadingHelper>();
        }
    
        /*public static FadingHelper SetFadingHelper(GameObject usedGameObject)
        {
            FadingHelper fadingHelper = new FadingHelper(usedGameObject);
            
            return fadingHelper;
        }*/
    }
}

