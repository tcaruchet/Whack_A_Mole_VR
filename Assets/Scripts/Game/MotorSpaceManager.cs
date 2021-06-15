using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MotorSpaceManager : MonoBehaviour
{

    public enum ActiveMotorSpace { Right, Left, Both, Off };

    public ActiveMotorSpace motorspace = ActiveMotorSpace.Right;

    [SerializeField]
    public LaserMapper MotorSpaceRight;

    [SerializeField]
    public LaserMapper MotorSpaceLeft;

    [SerializeField]
    public MotorSpaceMirror MotorSpaceMirrorRight;

    [SerializeField]
    public MotorSpaceMirror MotorSpaceMirrorLeft;

    [SerializeField]
    private Slider motorSpaceSlider;

    bool isMirror = false;

    public void SetActiveMotorSpace(string newMotorSpace) {
        motorspace = (MotorSpaceManager.ActiveMotorSpace)System.Enum.Parse( typeof(MotorSpaceManager.ActiveMotorSpace), newMotorSpace);
        bool R = motorspace == ActiveMotorSpace.Right ? true : false;
        R = motorspace == ActiveMotorSpace.Both ? true : R;
        bool L = motorspace == ActiveMotorSpace.Left ? true : false;
        L = motorspace == ActiveMotorSpace.Both ? true : L;
        bool mirrorR = isMirror;
        mirrorR = motorspace == ActiveMotorSpace.Right ? mirrorR : false;
        mirrorR = motorspace == ActiveMotorSpace.Both ? false : mirrorR;
        bool mirrorL = isMirror;
        mirrorL = motorspace == ActiveMotorSpace.Left ? mirrorL : false;
        mirrorL = motorspace == ActiveMotorSpace.Both ? false : mirrorL;

        MotorSpaceRight.gameObject.SetActive(R);
        MotorSpaceLeft.gameObject.SetActive(L);
        MotorSpaceMirrorRight.gameObject.SetActive(mirrorR);
        MotorSpaceMirrorLeft.gameObject.SetActive(mirrorL);
    }

    public void SetMirror(bool setMirror) {
        isMirror = setMirror;
        if (!isMirror) {
            MotorSpaceMirrorLeft.gameObject.SetActive(false);
            MotorSpaceMirrorRight.gameObject.SetActive(false);
            return;
        }
        if (motorspace == ActiveMotorSpace.Both) {
            return;
        }

        if (motorspace == ActiveMotorSpace.Right) {
            MotorSpaceMirrorRight.gameObject.SetActive(true);
            MotorSpaceMirrorLeft.gameObject.SetActive(false);
            MotorSpaceMirrorRight.UpdateMotorSpaceToMirror(MotorSpaceRight);
        } else if (motorspace == ActiveMotorSpace.Left) {
            MotorSpaceMirrorLeft.gameObject.SetActive(true);
            MotorSpaceMirrorRight.gameObject.SetActive(false);
            MotorSpaceMirrorLeft.UpdateMotorSpaceToMirror(MotorSpaceLeft);
        }
        
    }

    public void CalibrateActiveMotorspace(bool calibrate) {
        if (motorspace == ActiveMotorSpace.Right) {
            MotorSpaceRight.ToggleMotorCalibration(calibrate);
        } else if (motorspace == ActiveMotorSpace.Left) {
            MotorSpaceLeft.ToggleMotorCalibration(calibrate);
        } else if (motorspace == ActiveMotorSpace.Both) {
            MotorSpaceRight.ToggleMotorCalibration(calibrate);
            MotorSpaceLeft.ToggleMotorCalibration(calibrate);
        }
    }

    public void ResetActiveMotorspace() {
        if (motorspace == ActiveMotorSpace.Right) {
            MotorSpaceRight.SetDefaultMotorSpace();
        } else if (motorspace == ActiveMotorSpace.Left) {
            MotorSpaceLeft.SetDefaultMotorSpace();
        } else if (motorspace == ActiveMotorSpace.Both) {
            MotorSpaceRight.SetDefaultMotorSpace();
            MotorSpaceLeft.SetDefaultMotorSpace();
        }
    }

    public void SetActiveMotorSpaceMultiplier() {
        if (motorspace == ActiveMotorSpace.Right) {

        }
    }

    public void SetMotorSpaceSmall() {
        SetMotorSpace(0.15f, 0.1f);
    }

    public void SetMotorSpaceMedium() {
        SetMotorSpace(0.6f, 0.4f);
    }

    public void SetMotorSpaceLarge() {
        SetMotorSpace(1.2f, 0.8f);
    }

    public void SetMotorSpace(float width, float height) {
        var mR = MotorSpaceRight.GetMotorSpace();
        var mL = MotorSpaceLeft.GetMotorSpace();

        MotorSpaceInfo mRNew = new MotorSpaceInfo { 
            width = width, 
            height = height, 
            pos = mR.pos
            };

        MotorSpaceInfo mLNew = new MotorSpaceInfo { 
            width = width, 
            height = height, 
            pos = mL.pos
            };

        bool mRState = MotorSpaceRight.gameObject.activeSelf;
        bool mLState = MotorSpaceLeft.gameObject.activeSelf;

        MotorSpaceRight.gameObject.SetActive(true);
        MotorSpaceRight.SetMotorSpace(mRNew);
        MotorSpaceRight.gameObject.SetActive(mRState);

        MotorSpaceLeft.gameObject.SetActive(true);
        MotorSpaceLeft.SetMotorSpace(mLNew);
        MotorSpaceLeft.gameObject.SetActive(mLState);

        if (isMirror) MotorSpaceMirrorRight.UpdateMotorSpaceToMirror(MotorSpaceRight);
        if (isMirror) MotorSpaceMirrorLeft.UpdateMotorSpaceToMirror(MotorSpaceLeft);
    }

    public void OnMultiplierSliderChanged() {
        var sliderValue = (float) motorSpaceSlider.value;
        var highVal = (float) motorSpaceSlider.maxValue;
        var lowVal = (float) motorSpaceSlider.minValue;
        float multiplier = (sliderValue - lowVal) / highVal;

        if (motorspace == ActiveMotorSpace.Right) {
            MotorSpaceRight.SetMultiplier(multiplier);
            if (isMirror) MotorSpaceMirrorRight.UpdateMotorSpaceToMirror(MotorSpaceRight);
        } else if (motorspace == ActiveMotorSpace.Left) {
            MotorSpaceLeft.SetMultiplier(multiplier);
            if (isMirror) MotorSpaceMirrorLeft.UpdateMotorSpaceToMirror(MotorSpaceLeft);
        } else if (motorspace == ActiveMotorSpace.Both) {
            MotorSpaceRight.SetMultiplier(multiplier);
            MotorSpaceLeft.SetMultiplier(multiplier);
        }
        
    }
}
