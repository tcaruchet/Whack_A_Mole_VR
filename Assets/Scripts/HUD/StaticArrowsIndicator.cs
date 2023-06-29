using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HUD
{
    using UnityEngine;

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

        /// <summary>
        ///     Show the indicator at the given position and orient the arrows to face the motorspace center
        /// </summary>
        /// <param name="position">Position where motorspace exiting occurs</param>
        /// <param name="motorSpaceCenter">Position of the motorspace center</param>
        /// <param name="side">The side of the motorspace that is being exited</param>
        internal override void ShowIndicator(Vector3 position, Vector3 motorSpaceCenter, Side side)
        {
            // Enable the container (and all its child objects)
            OutOfBoundContainer.SetActive(true);

            // Position the arrows in a circle around the given position
            int i = 0;
            float angleSection = (Mathf.PI * 2f / numberOfObjects);
            foreach (Transform child in OutOfBoundContainer.transform)
            {
                // Calculate the position to form a circle
                float angle = i * angleSection;
                Vector3 newPos = position + new Vector3(Mathf.Cos(angle) * arrowDistance, Mathf.Sin(angle) * arrowDistance, 0);
                child.position = newPos;

                // Orient the arrow to face the camera
                child.rotation = mainCamera.transform.rotation;

                // Rotate to point to the center of the MotorSpace
                var difference = motorSpaceCenter - child.position;
                float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                child.rotation *= Quaternion.Euler(0.0f, 0.0f, rotationZ - 90); // -90 because we're aligning the tip of the arrow

                i++;
            }
        }





        internal override void HideIndicator()
        {
            // Disable the container (and all its child objects)
            OutOfBoundContainer.SetActive(false);
        }
    }
}
