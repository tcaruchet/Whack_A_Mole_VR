using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Timers;
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
        private bool isTimerRunning = false;
        private Vector3 lastPosition = Vector3.zero;
        private float speed = 0f;
        private float lastDistance = 0f;
        private float feedback = 0f;
        private float averageSpeed = 0f;
        private int nbShoot = 0;
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
            if (isTimerRunning)
            {
                timeSinceLastShot += Time.deltaTime;
            }

            CalculateSpeed();
        }

        private void ResetShoot()
        {
            // Réinitialisez le timer lorsque l'utilisateur tire
            timeSinceLastShot = 0f;
            speed = 0f;
            lastDistance = 0f;
        }

        public void OnPointerShoot()
        {
            // Arrêter le timer
            isTimerRunning = false;
            CalculateFeedback();
        }

        public void onMoleActivated()
        {
            // Démarrer le timer
            isTimerRunning = true;
            timeSinceLastShot = 0f;
            lastDistance= 0f;
            Debug.Log("Timer startedMOLE POP");
        }


        public void UpdatePointerData(BasicPointer pointer)
        {
            // Now you have access to all public variables and methods of the BasicPointer instance
            pointerData = pointer;
            // pointerData.onPointerShoot.AddListener(CalculateFeedback);
            
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
            if (isTimerRunning)
            {
                float distance = Vector3.Distance(position, lastPosition);
                lastPosition = position;
                lastDistance = lastDistance + distance;
                Debug.Log("Distance : " + lastDistance);
                Debug.Log("Time POUR LE CALCULE : " + timeSinceLastShot);
                speed = lastDistance / timeSinceLastShot;
            }
        }

        public float GetFeedback()
        {
            return feedback;
        }

        public void CalculateFeedback()
        {
            if (averageSpeed == 0f)
            {
                averageSpeed = speed;
                feedback = 1f;
            }
            else if (nbShoot < 5)
            {
                averageSpeed = (averageSpeed * nbShoot + speed) / (nbShoot + 1);
                feedback = 1f;
            }
            else
            {
                averageSpeed = (averageSpeed * nbShoot + speed) / (nbShoot + 1);
                float difference = speed - averageSpeed;
                Debug.Log("Speed : " + speed);
                Debug.Log("Average Speed : " + averageSpeed);
                Debug.Log("Difference : " + difference);
                feedback = 1 / (1 + Mathf.Exp(-2*difference));
                Debug.Log("Feedback : " + feedback);
            }
            nbShoot++;
            ResetShoot();
        }
    }
}