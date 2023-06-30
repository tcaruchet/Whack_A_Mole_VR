using System;
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

        // Reference to the main camera
        private Camera mainCamera;

        // Store the last MotorSpace center position and the exited side
        private Vector3 lastMotorSpaceCenter;
        private Side lastSide;

        private void Awake()
        {
            // Get a reference to the main camera
            mainCamera = Camera.main;

            // Spawn the arrows and parent them to the container
            for (int i = 0; i < numberOfObjects; i++)
            {
                Instantiate(OutOfBoundPrefab, OutOfBoundContainer.transform);
            }
        }

        internal override void ShowIndicator(Vector3 position, Vector3 motorSpaceCenter, Side side)
        {
            OutOfBoundContainer.SetActive(true);

            lastMotorSpaceCenter = motorSpaceCenter;
            lastSide = side;

            PositionArrows(position);
            OrientArrowsTowardsCenter(lastSide);
        }

        private void Update()
        {
            if (OutOfBoundContainer.activeSelf)
            {
                // If the container is active, keep updating the orientation of the arrows
                OrientArrowsTowardsCenter(lastSide);
            }
        }

        // A separate method to position the arrows around the given position
        private void PositionArrows(Vector3 position)
        {
            int i = 0;
            float angleSection = (Mathf.PI * 2f / numberOfObjects);
            foreach (Transform child in OutOfBoundContainer.transform)
            {
                float angle = i * angleSection;
                Vector3 newPos = position + new Vector3(Mathf.Cos(angle) * arrowDistance, Mathf.Sin(angle) * arrowDistance, 0);
                child.position = newPos;
                i++;
            }
        }

        private void OrientArrowsTowardsCenter(Side side)
        {
            foreach (Transform child in OutOfBoundContainer.transform)
            {
                child.rotation = mainCamera.transform.rotation;

                Vector3 rotation;
                switch (side)
                {
                    case Side.Right:
                        rotation = new Vector3(0, 0, 180); // or whatever value represents "right" in your game
                        break;
                    case Side.Left:
                        rotation = new Vector3(0, 0, 0); // or whatever value represents "left" in your game
                        break;
                    case Side.Top:
                        rotation = new Vector3(0, 0, -90); // or whatever value represents "top" in your game
                        break;
                    case Side.Bottom:
                        rotation = new Vector3(0, 0, 90); // or whatever value represents "bottom" in your game
                        break;
                    default:
                        throw new ArgumentException("Unexpected side: " + side);
                }

                child.rotation *= Quaternion.Euler(rotation);
            }
        }




        internal override void HideIndicator()
        {
            // Disable the container (and all its child objects)
            OutOfBoundContainer.SetActive(false);
        }
    }
}
