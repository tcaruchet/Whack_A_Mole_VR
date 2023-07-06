using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditorInternal;
using UnityEngine;
using static WallManager;

namespace Assets.Scripts.Game
{
    public class PerformanceManager : MonoBehaviour
    {

        private BasicPointer pointerData;
        private DiskMole moleData;
        private WallManager wallData;
        private PatternPlayer patternData;


        public static PerformanceManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // listMolesInfo = new List<MoleInfo>();
            }
            else
            {
                Destroy(gameObject);
            }
        }


        public void UpdatePointerData(BasicPointer pointer)
        {
            // Now you have access to all public variables and methods of the BasicPointer instance
            pointerData = pointer;
            
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