using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HUD
{
    using UnityEngine;

    internal class StaticArrowIndicator : OutOfBoundIndicator
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

        internal override void ShowIndicator(Vector3 position, Vector3 motorSpaceCenter, Side side)
        {
            // Enable the container (and all its child objects)
            OutOfBoundContainer.SetActive(true);

            // Position the arrows in a circle around the given position
            int i = 0;
            float angleSection = (Mathf.PI * 2f / numberOfObjects);
            foreach (Transform child in OutOfBoundContainer.transform)
            {
                //Calculate the position to form a circle
                float angle = i * angleSection;
                Vector3 newPos = position + new Vector3(Mathf.Cos(angle) * arrowDistance, Mathf.Sin(angle) * arrowDistance, 0);
                child.position = newPos;

                //// Orient the arrow to face the camera
                //child.LookAt(child.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);

                //var currentPos = child.position;
                //var pointA = new Vector3(currentPos.x, currentPos.y, currentPos.z);
                //child.position = Vector3.Lerp(pointA, motorSpaceCenter, Mathf.PingPong(Time.time * 0.3f, 0.15f));

                //Change the rotation of the objects to point them all to the center of the MotorSpace
                var difference = motorSpaceCenter - child.position;
                float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                child.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

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
