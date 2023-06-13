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

        private CanvasGroup arrow;
        private Coroutine coroutine;
        private bool showArrow = false;
        private bool active = false;
        private bool lastEnter = true;
        private Side lastSide;
        private WallInfo wallInfo;
        private float fadeTime = 0.35f;

        private void Awake()
        {
            arrow = GetComponent<CanvasGroup>();
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
            if (showArrow)
            {
                transform.right = new Vector3(wallInfo.meshCenter.x, wallInfo.meshCenter.y, arrow.transform.position.z) - arrow.transform.position;
            }
        }

        internal override void ShowIndicator(Vector3 positon, Side side)
        {
            if (coroutine != null) { StopCoroutine(coroutine); }
            arrow.transform.rotation = Quaternion.identity;
            showArrow = true;
            transform.right = new Vector3(wallInfo.meshCenter.x, wallInfo.meshCenter.y, arrow.transform.position.z) - arrow.transform.position;
            coroutine = FadingUtils.FadeRoutine(handler: this, Obj: arrow.gameObject, fadeTime: fadeTime, fadeDirection: FadeAction.In);
        }

        internal override void HideIndicator()
        {
            if (coroutine != null) { StopCoroutine(coroutine); }
            coroutine = FadingUtils.FadeRoutine(handler: this, Obj: arrow.gameObject, fadeTime: fadeTime, fadeDirection: FadeAction.Out);
            showArrow = false;
        }

        private void OnWallUpdated(WallInfo w)
        {
            wallInfo = w;
            active = wallInfo.active;
            if (!lastEnter && active) { ShowIndicator(Vector3.zero, lastSide); }
        }

        public void OnMotorSpaceEnter(EnterMotorSpaceInfo m)
        {
            lastEnter = m.enter;
            lastSide = m.side;
            Debug.Log("MotorSpaceEnter " + m.enter + "active " + active);
            if (m.enter && active)
            {
                Debug.Log("MotorSpaceEnter " + m.side);
                HideIndicator();
            }
            else if (!m.enter && active)
            {
                Debug.Log("MotorSpaceExit " + m.side);
                ShowIndicator(Vector3.zero, m.side);
            }
            else
            {
                HideIndicator();
            }
        }
    }


}
