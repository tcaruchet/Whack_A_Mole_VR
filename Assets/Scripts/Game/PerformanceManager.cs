using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using static WallManager;

namespace Assets.Scripts.Game
{
    public class PerformanceManager : MonoBehaviour
    {

        private BasicPointer pointerData;
        private DiskMole moleData;
        private WallManager wallData;
        private PatternPlayer patternData;
        private float timeSinceLastShot = 0f;
        private Vector3 lastPosition = Vector3.zero;
        private float speed = 0f;
        private float lastDistance = 0f;
        public static PerformanceManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            timeSinceLastShot += Time.deltaTime;
            CalculateSpeed();
        }

        private void ResetShoot()
        {
            // Réinitialisez le timer lorsque l'utilisateur tire
            Debug.Log("timeSinceLastShot : " + timeSinceLastShot + "speed : " + speed + "LastDistance : " + lastDistance);
            timeSinceLastShot = 0f;
            speed = 0f;
            lastDistance = 0f;
        }


        public void UpdatePointerData(BasicPointer pointer)
        {
            // Now you have access to all public variables and methods of the BasicPointer instance
            pointerData = pointer;
            pointerData.onPointerShoot.AddListener(ResetShoot);
            
        }

        public void UpdateMoleData(DiskMole mole)
        {
            moleData = mole;
        } 

        public void UpdateWallData(WallManager wall)
        {
            // Update information based on the wall
        }

        public void UpdatePatternData(PatternPlayer pattern)
        {
            // Update information based on the pattern
        }

        public void CalculateSpeed()
        {

            Vector3 position = pointerData.MappedPosition;
            if (lastPosition == Vector3.zero)
            {
                  lastPosition = position;
            }
            float distance = Vector3.Distance(position, lastPosition);
            ///speed = Vector3.Distance(position, lastPosition) / Time.deltaTime;
            lastPosition = position;
            lastDistance = lastDistance + distance;
            speed = lastDistance / timeSinceLastShot;
           // Debug.Log("Speed J'espère c'est bon jpp: " + speed);

        }

        public float CalculateFeedback()
        {
            float feedback = 0f;
            // Calculate feedback based on the data 
            // scale 0-1

            return feedback;
        }
    }
}