using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.Events;

public class HmdCalibration : MonoBehaviour
{

    [System.Serializable]
    public class CalibrationEvent : UnityEvent {}
    [SerializeField]
    public CalibrationEvent calibrationUpdate;

    [SerializeField]
    private SteamVR_Input_Sources controller;

    [SerializeField]
    private GameObject controllerGameObject;

    [SerializeField]
    private GameObject objToFade;

    private FadingHelper calibrationScreenFader;

    bool calibrated = false;
    int timeout = 100;
    int time = 0;

    // Start is called before the first frame update
    void Awake()
    {
        controllerGameObject.GetComponent<SteamVR_Behaviour_Pose>().onTransformUpdated.AddListener(delegate{PositionUpdated();});
    }

    void Start()
    {
        calibrationScreenFader = objToFade.GetComponent<FadingHelper>();
    }

    public void Update() {
        if (!calibrated) {
            if (Input.GetKeyDown(KeyCode.V))
            {
                bool finished = false;
                Debug.Log("Before Utils Func");
                finished = FadingUtils.FadingUtils.FadingOutPlusDisablingCB(calibrationScreenFader,2);

                Debug.Log("Before If statement");

                if (finished)
                {
                    CloseInstructionPanel();
                    Debug.Log("Inside If statement");
                } 
                


                Debug.Log("After If statement");

            }
        }
    }

    // Update is called once per frame
    public void PositionUpdated() {
        // HACK: for some reason we need invoke the calibrationUpdate a couple of times..
        // need to investigate why this is..
        if (time < timeout) {
            if(SteamVR.active)
            {
                if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown(controller))
                {
                    CloseInstructionPanel();
                }
            }
            if (calibrated) {
            time++;
            }
        }
    }

    //generic function if we need to change something when the instruction panel disappeared
    private void CloseInstructionPanel()
    {
        calibrationUpdate.Invoke(); 
        calibrated = true;
    }
}
