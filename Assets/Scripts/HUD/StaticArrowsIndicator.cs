using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.HUD
{
    internal class StaticArrowsIndicator : OutOfBoundIndicator
    {
        // Reference to the arrow prefab
        [SerializeField]
        private GameObject OutOfBoundPrefab;

        // Reference to the parent container for the arrows
        [SerializeField]
        private GameObject OutOfBoundContainer;

        // The number of arrows to spawn
        [SerializeField]
        private int numberOfObjects;

        // Distance from the center to position the arrows
        [SerializeField]
        private float arrowDistance = 1.5f;

        [SerializeField]
        private WallManager wallManager;
        private WallInfo wallInfo;

        // Store the last MotorSpace center position and the exited side
        private Side lastSide;

        [SerializeField]
        public Color Color;

        [SerializeField]
        private float fadeTime = 0.35f;

        [SerializeField]
        private LaserMapper laserMapper;

        private Coroutine fadeCoroutine;
        private Coroutine fadeRoutine = null;
        private Coroutine animationRoutine = null;


        void OnEnable()
        {
            wallManager.stateUpdateEvent.AddListener(OnWallUpdated);
            OnWallUpdated(wallManager.CreateWallInfo());
        }

        void OnDisable()
        {
            wallManager.stateUpdateEvent.RemoveListener(OnWallUpdated);
        }

        private void OnWallUpdated(WallInfo w)
        {
            wallInfo = w;
        }

        private void Awake()
        {
            // Spawn the arrows and parent them to the container
            for (int i = 0; i < numberOfObjects; i++)
            {
                OutOfBoundPrefab.GetComponent<SpriteRenderer>().color = Color;
                var obj = Instantiate(OutOfBoundPrefab, OutOfBoundContainer.transform);
                obj.name = $"Arrow{i}";
            }
        }

        internal override void ShowIndicator(Vector3 position, Vector3 motorSpaceCenter, Side side)
        {
            // Stop any previous fade or animation routine
            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);
            if (animationRoutine != null)
                StopCoroutine(animationRoutine);

            // Start new fade in and animation routine
            fadeRoutine = StartCoroutine(FadeInObject(OutOfBoundContainer, 10f));
            animationRoutine = StartCoroutine(OutOfBoundAnimation(true));
            laserMapper.ShowMotorspace(true);
        }


        internal override void HideIndicator()
        {
            // Stop any previous fade or animation routine
            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);
            if (animationRoutine != null)
                StopCoroutine(animationRoutine);

            // Start new fade out routine
            fadeRoutine = StartCoroutine(FadeOutObject(OutOfBoundContainer, 10f));
            laserMapper.ShowMotorspace(false);
        }


        private IEnumerator OutOfBoundAnimation(bool isOutOfBound)
        {
            while (isOutOfBound)
            {
                InstantiateInCircle(OutOfBoundContainer, OutOfBoundContainer.transform.childCount, laserMapper.GetMotorSpace().width + 0.1f);

                foreach (Transform child in OutOfBoundContainer.transform)
                {
                    var motorSpaceCenter = laserMapper.transform.position;

                    var currentPos = child.position;
                    var pointA = new Vector3(currentPos.x, currentPos.y, currentPos.z);
                    child.position = Vector3.Lerp(pointA, motorSpaceCenter, Mathf.PingPong(Time.time * 0.3f, 0.15f));

                    var difference = motorSpaceCenter - child.position;
                    float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                    child.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
                }

                yield return null;
            }
        }

        public void InstantiateInCircle(GameObject container, Vector3 location, int howMany, float radius, float zPosition)
        {
            int i = 0;
            float angleSection = (Mathf.PI * 2f / howMany);
            foreach (Transform prefab in container.transform)
            {
                float angle = i * angleSection + (3 * Mathf.PI / 4);
                Vector3 newPos = location + new Vector3(Mathf.Cos(angle) * 1.5f, Mathf.Sin(angle), 0) * radius;
                newPos.z = zPosition;
                prefab.transform.position = newPos;
                i++;
            }
        }

        public void InstantiateInCircle(GameObject container, Vector3 location, int howMany, float radius)
        {
            InstantiateInCircle(container, location, howMany, radius, location.z);
        }

        public void InstantiateInCircle(GameObject container, int howMany, float radius)
        {
            InstantiateInCircle(container, laserMapper.transform.position, howMany, radius);
        }



        public IEnumerator FadeOutObject(GameObject gameObjects, float fadeSpeed)
        {
            foreach (Transform o in gameObjects.transform)
            {
                while (o.GetComponent<Renderer>().material.color.a > 0)
                {
                    Color objectColor = o.GetComponent<Renderer>().material.color;
                    float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

                    objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                    o.GetComponent<Renderer>().material.color = objectColor;
                    yield return null;
                }
            }

            gameObjects.SetActive(false);
        }

        public IEnumerator FadeInObject(GameObject gameObjects, float fadeSpeed)
        {
            gameObjects.SetActive(true);

            foreach (Transform o in gameObjects.transform)
            {
                while (o.GetComponent<Renderer>().material.color.a < 1)
                {
                    Color objectColor = o.GetComponent<Renderer>().material.color;
                    float fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

                    objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                    o.GetComponent<Renderer>().material.color = objectColor;
                    yield return null;
                }
            }
        }

    }
}
