using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HUD
{
    internal class DynamicArrowIndicator : OutOfBoundIndicator
    {
        [SerializeField]
        private WallManager wallManager;

        [SerializeField]
        private CanvasGroup arrow;


        private Coroutine coroutine;
        private bool active = false;
        private bool lastEnter = true;
        private Side lastSide;
        private WallInfo wallInfo;
        private float fadeTime = 0.35f;

        private void Awake()
        {
            //arrow = GetComponent<CanvasGroup>();
            arrow.gameObject.SetActive(false); // Ensure the arrow is disabled by default
            wallInfo = wallManager.CreateWallInfo();
        }

        private void OnEnable()
        {
            wallManager.stateUpdateEvent.AddListener(OnWallUpdated);
            OnWallUpdated(wallManager.CreateWallInfo());
        }

        private void OnDisable()
        {
            wallManager.stateUpdateEvent.RemoveListener(OnWallUpdated);
        }

        private void Update()
        {
            if (arrow.gameObject.activeInHierarchy) // Only update rotation if the arrow is active
            {
                arrow.transform.right = new Vector3(wallInfo.meshCenter.x, wallInfo.meshCenter.y, arrow.transform.position.z) - arrow.transform.position;
            }
        }

        internal override void ShowIndicator(Vector3 position, Vector3 motorSpaceCenter, Side side)
        {
            if (!arrow.gameObject.activeInHierarchy) // Only show the indicator if it's not already shown
            {
                arrow.gameObject.SetActive(true); // Enable the arrow
                arrow.transform.right = new Vector3(wallInfo.meshCenter.x, wallInfo.meshCenter.y, arrow.transform.position.z) - arrow.transform.position;

                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                coroutine = FadingUtils.FadeRoutine(handler: this, Obj: arrow.gameObject, fadeTime: fadeTime, fadeDirection: FadeAction.In);
            }
        }

        internal override void HideIndicator()
        {
            if (arrow.gameObject.activeInHierarchy) // Only hide the indicator if it's currently shown
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                coroutine = FadingUtils.FadeRoutine(handler: this, Obj: arrow.gameObject, fadeTime: fadeTime, fadeDirection: FadeAction.Out);
                arrow.gameObject.SetActive(false); // Disable the arrow
            }
        }

        private void OnWallUpdated(WallInfo w)
        {
            wallInfo = w;
            active = wallInfo.active;
            if (!lastEnter && active)
            {
                ShowIndicator(Vector3.zero, w.wallCenter, lastSide);
            }
            else
            {
                HideIndicator();
            }
        }
    }
}
