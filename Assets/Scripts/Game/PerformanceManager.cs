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

public enum JudgementType {
    Speed,
    Distance,
    Time
}

// Data Structures
public class PerfData {
    public JudgementType judgementType = JudgementType.Speed;
    public Queue<float> lastVals = new Queue<float>();
    public Queue<float> lastJudges = new Queue<float>();
    public float movingAverage = -1f;
    public float upperThreshold = -1f;
    public float lowerThreshold = -1f;
    public Queue<float> meanMemoryVals = new Queue<float>();
    
    // State
    public float perfPrev = -1f;
    public float perf = -1f;
    public float judge = -1f;
    public Vector3 posPrev = Vector3.zero;
    public Vector3 pos = Vector3.zero;

    public Vector3 actionStartPos = Vector3.zero;
    public Vector3 actionEndPos = Vector3.zero;
    public float actionStartTimestamp = -1f;
    public float actionEndTimestamp = -1f;
}

public class PerformanceManager : MonoBehaviour
{
    [SerializeField]
    private JudgementType judgementType = JudgementType.Speed;

    // Action data
    private Dictionary<ControllerName, PerfData> perfData = new Dictionary<ControllerName, PerfData>();
    private PerfData perfR = new PerfData();
    private PerfData perfL = new PerfData();

    // Average configuration
    private float meanMemoryLimit = 20; // use the last 20 values for calculating mean
    private int minimumJudgeThreshold = 5;
    private float MultiplierUp = 1.50f; // Upper/Lower Threshold multipliers
    private float MultiplierDown = 0.50f;

    private void Awake()
    {
        perfData[ControllerName.Left] = perfL;
        perfData[ControllerName.Right] = perfR;
    }

    // Data Consumption
    public PerfData GetPerfData(ControllerName controllerName) {
        return perfData[controllerName];
    }

    public float GetInstantJudgement(ControllerName controllerName) {
        return perfData[controllerName].judge;
    }

    public float GetActionJudgement(ControllerName controllerName) {
        return perfData[controllerName].lastVals.LastOrDefault();
    }

    // Data Control
    public void ResetPerfHistory() {
        foreach(KeyValuePair<ControllerName, PerfData> entry in perfData) {
            entry.Value.lastVals.Clear();
            entry.Value.lastJudges.Clear();
        }       
    }

    public void ResetPerfData() {
        // Resets History, but maintains moving average.
        perfR = new PerfData();
        perfL = new PerfData();
        perfData[ControllerName.Left] = perfL;
        perfData[ControllerName.Right] = perfR;
    }

    // Data Feeders
    // BasicPointer: OnPointerShoot and OnPointerMove
    public void OnPointerShoot(ShootData shootData) {
        RaycastHit hit = shootData.hit;
        ControllerName controllerName = shootData.name;

        bool moleHit = false;
        Mole mole;

        if (hit.collider) { 
            if (hit.collider.gameObject.TryGetComponent<Mole>(out mole)) {
                moleHit = true;
            }
        }
        
        // if we dont hit a mole, don't count as an action.
        if (!moleHit) return;

        PerfData perf = perfData[controllerName];

        perf.actionEndPos = perf.actionStartPos;
        perf.actionStartPos = hit.point;
        perf.actionStartTimestamp = perf.actionEndTimestamp;
        perf.actionEndTimestamp = Time.deltaTime;

        float newVal;

        if (judgementType == JudgementType.Speed) {
            newVal = CalculateActionSpeed(perf);
        } else if (judgementType == JudgementType.Distance) {
            newVal = CalculateActionDistance(perf);
        } else if (judgementType == JudgementType.Time) {
            newVal = CalculateActionTime(perf);
        } else {
            newVal = -1f;
        }

        UpdateMovingAverage(newVal, perf);
        float judgement = MakeJudgement(newVal, perf);

        // Store Results
        perf.lastVals.Enqueue(newVal);
        perf.lastJudges.Enqueue(judgement);
    }

    public void OnPointerMove(MoveData moveData) {
        PerfData perf = perfData[moveData.name];

        perf.posPrev = perf.pos;
        perf.pos = moveData.cursorPos;
        perf.perfPrev = perf.perf;

        float newPerf;

        if (judgementType == JudgementType.Speed) {
            newPerf = CalculateInstantSpeed(perf);
        } else if (judgementType == JudgementType.Distance) {
            newPerf = CalculateInstantDistance(perf);
        } else if (judgementType == JudgementType.Time) {
            newPerf = CalculateInstantTime(perf);
        } else {
            newPerf = -1f;
        }

        float newJudge = MakeJudgement(newPerf, perf);
        perf.perf = newPerf;
        perf.judge = newJudge;
    }

    // Average Calculator
    private void UpdateMovingAverage(float val, PerfData perf) {

        // Update moving average
        perf.meanMemoryVals.Enqueue(val);
        if (perf.meanMemoryVals.Count > meanMemoryLimit)
        {
            perf.meanMemoryVals.Dequeue();
        }

        perf.movingAverage = perf.meanMemoryVals.Average();
        perf.upperThreshold = MultiplierUp * perf.movingAverage;
        perf.lowerThreshold = MultiplierDown * perf.movingAverage;
    }


    // Calculators
    private float CalculateInstantSpeed(PerfData perf) {
        // TODO: Should we calculate the instant speed (frame by frame), or should we calculate speed
        // based on the distance accumulated since the beginning?

        // if we don't have a previous position, abort calculation.
        if (perf.posPrev == Vector3.zero) return -1f;

        //Debug.Log("lastPosition: " + lastPositionSpeed);
        float distance = Vector3.Distance(perf.pos, perf.posPrev);
        float speed = distance / Time.deltaTime;
        return speed;
    }

    private float CalculateInstantDistance(PerfData perf) {
        // TODO: Should we calculate the instant speed (frame by frame), or should we calculate speed
        // based on the distance accumulated since the beginning?

        // if this is our first action, we don't have enough information to calculate speed.
        if (perf.actionStartPos == Vector3.zero) return -1f;

        // if we don't have a previous position, abort calculation.
        if (perf.posPrev == Vector3.zero) return -1f;

        //Debug.Log("lastPosition: " + lastPositionSpeed);
        float distance = Vector3.Distance(perf.actionStartPos, perf.pos);
        return distance;
    }

    private float CalculateInstantTime(PerfData perf) {
        // TODO: Should we calculate the instant speed (frame by frame), or should we calculate speed
        // based on the distance accumulated since the beginning?

        // if this is our first action, we don't have enough information to calculate speed.
        if (perf.actionStartTimestamp == -1f) return -1f;

        // if we don't have a previous position, abort calculation.
        if (perf.posPrev == Vector3.zero) return -1f;

        float time = Time.deltaTime - perf.actionStartTimestamp;
        return time;
    }

    private float CalculateActionDistance(PerfData perf) {
        if (perf.actionEndTimestamp == -1f || perf.actionEndPos == Vector3.zero) {
            // if this is our first action, we don't have enough information to calculate speed.
            return -1f;
        }

        float distance = Vector3.Distance(perf.actionStartPos, perf.actionEndPos);
        return distance;
    }

    private float CalculateActionTime(PerfData perf) {
    if (perf.actionEndTimestamp == -1f || perf.actionEndPos == Vector3.zero) {
        // if this is our first action, we don't have enough information to calculate speed.
        return -1f;
    }

    float time = perf.actionEndTimestamp - perf.actionStartTimestamp;
    return time;
    }

    private float CalculateActionSpeed(PerfData perf) {
        if (perf.actionEndTimestamp == -1f || perf.actionEndPos == Vector3.zero) {
            // if this is our first action, we don't have enough information to calculate speed.
            return -1f;
        }

        float distance = Vector3.Distance(perf.actionStartPos, perf.actionEndPos);
        float time = perf.actionEndTimestamp - perf.actionStartTimestamp;
        float speed = distance / time;

        return speed;
    }

    private float MakeJudgement(float val, PerfData perf) {
        float judgement;
        // If there is less than the threshold to judge threshold, default to 100% postive feedback.
        if (perf.meanMemoryVals.Count < minimumJudgeThreshold)
        {
            judgement = 1;
            return judgement;
        }

        if (val <= perf.lowerThreshold)
        {
            judgement = 0;
        }
        else if (val >= perf.upperThreshold)
        {
            judgement = 1;
        }
        else
        {
            judgement = (val - perf.lowerThreshold) / (perf.upperThreshold - perf.lowerThreshold);
        }
        return judgement;
    }
}




//     // old code
//     public void OnPointerShoot()
//     {
//         isTimerRunning = false;
//         CalculateAction();
//     }

//     private float timeSinceLastShot = 0f;
//     private bool isTimerRunning = false;
//     private float speed = 0f;
//     private float instantSpeed = 0f;
//     private Vector3 lastPosition = Vector3.zero;
//     private Vector3 lastPositionSpeed = Vector3.zero;
//     private float lastDistance = 0f;
//     private float feedback = 0f;
//     private float averageSpeed = 0f;
//     private int nbShoot = 0;

//     private void Awake()
//     {
//     }

//     private void Update()
//     {
//         if (isTimerRunning)
//         {
//             timeSinceLastShot += Time.deltaTime;
//         }

//         CalculateSpeed();
//         CalculateInstantSpeed();
//     }

//     private void ResetShoot()
//     {
//         timeSinceLastShot = 0f;
//         speed = 0f;
//         lastDistance = 0f;
//     }



//     public void onMoleActivated()
//     {
//         isTimerRunning = true;
//         timeSinceLastShot = 0f;
//         lastDistance = 0f;
//     }


//     public void UpdatePointerData(BasicPointer pointer)
//     {
//         // Now you have access to all public variables and methods of the BasicPointer instance
//         pointerData = pointer;

//     }

//     public void CalculateSpeed()
//     {

//         Vector3 position = pointerData.MappedPosition;
//         if (lastPosition == Vector3.zero)
//         {
//             lastPosition = position;
//         }
//         if (isTimerRunning)
//         {
//             float distance = Vector3.Distance(position, lastPosition);
//             lastPosition = position;
//             lastDistance = lastDistance + distance;
//             speed = lastDistance / timeSinceLastShot;
//         }
//     }

//     public void CalculateInstantSpeed()
//     {
//         Vector3 position = pointerData.MappedPosition;
//         if (lastPositionSpeed == Vector3.zero)
//         {
//             lastPositionSpeed = position;
//         }
//         if (lastPositionSpeed != Vector3.zero)
//         {
//             Debug.Log("lastPosition: " + lastPositionSpeed);
//             float distance = Vector3.Distance(position, lastPositionSpeed);
//             instantSpeed = distance / Time.deltaTime;
//         }
//         else
//         {
//             Debug.Log("FESSE " + lastPositionSpeed);
//         }
//         lastPositionSpeed = position;
//     }


//     public float GetSpeed()
//     {
//         return speed;
//     }

//     public float GetInstantSpeed()
//     {
//         return instantSpeed;
//     }


//     public float GetFeedback()
//     {
//         return feedback;
//     }

//     public Queue<float> GetTaskFeedbacks() {
//         return taskFeedbacks;
//     }

//     public void CalculateFeedback()
//     {
//         float minDistance = 0.3f;
//         lastSpeeds.Enqueue(speed);

//         if (lastSpeeds.Count > 20)
//         {
//             lastSpeeds.Dequeue();
//         }
//         if (nbShoot < 5)
//         {
//             feedback = 1;
//             averageSpeed = speed;
//             nbShoot++;
//         }
//         else if (lastDistance <= minDistance)
//         {
//             feedback = 1;
//         }
//         else
//         {
//             averageSpeed = lastSpeeds.Average();
//             nbShoot++;
//             float thresholdUp = 1.50f * averageSpeed;
//             float thresholdDown = 0.50f * averageSpeed;

//             if (speed <= thresholdDown)
//             {
//                 feedback = 0;
//             }
//             else if (speed >= thresholdUp)
//             {
//                 feedback = 1;
//             }
//             else
//             {
//                 feedback = (speed - thresholdDown) / (thresholdUp - thresholdDown);
//             }

//         }
//         taskFeedbacks.Enqueue(feedback);
//         ResetShoot();
//     }
// }