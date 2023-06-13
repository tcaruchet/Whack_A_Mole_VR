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

        private void Awake()
        {
            // Spawn the arrows and parent them to the container
            for (int i = 0; i < numberOfObjects; i++)
            {
                Instantiate(OutOfBoundPrefab, OutOfBoundContainer.transform);
            }
        }

        internal override void ShowIndicator(Vector3 position, Side side)
        {
            // Enable the container (and all its child objects)
            OutOfBoundContainer.SetActive(true);

            // Position the arrows in a circle around the given position
            int i = 0;
            float angleSection = (Mathf.PI * 2f / numberOfObjects);
            foreach (Transform child in OutOfBoundContainer.transform)
            {
                float angle = i * angleSection + (3 * Mathf.PI / 4);
                Vector3 newPos = position + new Vector3(Mathf.Cos(angle) * 1.5f, Mathf.Sin(angle), 0);
                child.position = newPos;
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
