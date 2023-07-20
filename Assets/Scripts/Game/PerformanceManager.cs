using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private float fastestSpeed = 0f;
        private float slowestSpeed = 0f;
        private void Awake()
        {
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
        }


        public void UpdatePointerData(BasicPointer pointer)
        {
            // Now you have access to all public variables and methods of the BasicPointer instance
            pointerData = pointer;

        }

        public void UpdateMoleData(DiskMole mole)
        {
           
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
                speed = lastDistance / timeSinceLastShot;
            }
        }

        public float GetFeedback()
        {
            return feedback;
        }

        public void CalculateFeedback()
        {
            // Mettre à jour les vitesses les plus rapides et les plus lentes
            if (speed > fastestSpeed)
            {
                fastestSpeed = speed;
            }
            if (speed < slowestSpeed)
            {
                slowestSpeed = speed;
            }

            // Calculer le feedback en fonction de la proximité de la vitesse actuelle avec les vitesses les plus rapides et les plus lentes
            float range = fastestSpeed - slowestSpeed;
            if (range == 0)
            {
                feedback = 1; // Si toutes les vitesses sont les mêmes, le feedback est de 1
            }
            else
            {
                feedback = (speed - slowestSpeed) / range;

            }

        ResetShoot();
        }
    }
}