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

        public void OnPointerShoot()
        {
            CalculateFeedback();
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
            float distance = Vector3.Distance(position, lastPosition);
            ///speed = Vector3.Distance(position, lastPosition) / Time.deltaTime;
            lastPosition = position;
            lastDistance = lastDistance + distance;
            speed = lastDistance / timeSinceLastShot;
           // Debug.Log("Speed J'espère c'est bon jpp: " + speed);

        }

        public float Getfeedback()
        {
            return feedback;
        }

        public void CalculateFeedback()
        {
            if (averageSpeed == 0f)
            {
                // Si c'est le premier tir, on définit la vitesse moyenne à la vitesse actuelle
                Debug.Log("Premier tir");
                averageSpeed = speed;
                feedback = 1f;
                nbShoot++;
            }
            else
            {
                // Définir le feedback en fonction de la vitesse actuelle par rapport à la vitesse moyenne
                float speedDifference = speed - averageSpeed;
                Debug.Log("Speed : " + speed + "AverageSpeed : " + averageSpeed);
                Debug.Log("SpeedDifference : " + speedDifference);
                /*  if (speedDifference < 0) // Si la vitesse est en dessous de la moyenne
                  {
                     Debug.Log("VITESSE EN DESSOUS DE LA MOYENNE");
                      feedback = 0f;
                  }
                  else if (speedDifference >= 0 && speedDifference <= 0.8f) // Si la vitesse est proche de la moyenne
                  {
                      Debug.Log("VITESSE proche DE LA MOYENNE");
                      feedback = 0.5f; 
                  }
                  else // Si la vitesse est bien au-dessus de la moyenne
                  {
                      Debug.Log("VITESSE AU DESSUS DE LA MOYENNE aka le fast");
                      feedback = 1f;
                  }*/
                 speedDifference = speed / averageSpeed;

                // Utiliser une fonction sigmoïde pour obtenir un feedback entre 0 et 1
                feedback = 1 / (1 + Mathf.Exp(-speedDifference));
                Debug.Log("Feedbaaaaaaaaaaaaaaaaaaaaaaaaaack : " + feedback);
                averageSpeed = (averageSpeed * nbShoot + speed) / (nbShoot + 1);
            }
            
            ResetShoot();
        }
    }
}