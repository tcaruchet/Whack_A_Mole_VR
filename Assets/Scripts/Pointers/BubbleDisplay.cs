using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Assets.Scripts.HUD;
using static OutOfBoundsArrow;
using System;

public class EnterMotorSpaceInfo {
    public Side side; // side from which it entered/exited
    public bool enter = true; // enter (true), exit (false)
    public Vector3 motorLastPos; // motorspace last position
    public Vector3 wallLastPos; // wall space last position
}

public enum MotorAction {
    Enter,
    Inside,
    Exit,
    Outside,
    None,
}

public enum ArrowType
{
    DynamicCenter,
    StaticPointing
}

public class BubbleDisplay : MonoBehaviour
{
    [SerializeField]
    private LoggingManager loggingManager;
    
    [SerializeField]
    public SoundManager soundManager;
    // The parent we will follow in terms of object position.
    [SerializeField]
    private GameObject parent;

    [SerializeField]
    private GameObject bubbleRender;

    [SerializeField]
    private GameObject bubbleSphere;

    [SerializeField]
    private GameObject bubbleOutline;

    [SerializeField]
    private LineRenderer laserRender;

    [SerializeField]
    private GameObject controllerRender;

    [SerializeField]
    private GameObject OutOfBoundContainer;

    [SerializeField]
    private GameObject OutOfBoundPrefab;

    [SerializeField]
    private int numberOfObjects;

    [SerializeField]
    private bool parentX = true;

    [SerializeField]
    private bool parentY = true;

    [SerializeField]
    private bool parentZ = true;

    [SerializeField]
    private float offsetX = 0f;

    [SerializeField]
    private float offsetY = 0f;

    [SerializeField]
    private float offsetZ = 0f;

    [SerializeField]
    private bool showBubble = false;

    [SerializeField]
    private LaserMapper laserMapper;

    [SerializeField]
    private Image motorSpaceRender;

    [SerializeField]
    private Color motorActiveColor;

    [SerializeField]
    private Color motorDisabledColor;


    [SerializeReference]
    private OutOfBoundIndicator staticArrowIndicator;

    [SerializeReference]
    private OutOfBoundIndicator dynamicCenterPointingIndicator;

    private OutOfBoundIndicator outOfBoundIndicatorManager;  // The current active indicator
    public ArrowType CurrentArrowType { get; private set; }




    private float newPosX;
    private float newPosY;
    private float newPosZ;

    private float prevPosX;
    private float prevPosY;
    private float prevPosZ;

    private Vector3 ownPosition;

    [System.Serializable]
    public class EnterMotorSpaceEvent : UnityEvent<EnterMotorSpaceInfo> {}
    public EnterMotorSpaceEvent enterMotorStateEvent;
    
    private MotorAction action = MotorAction.None;

    
    private DateTime exitTime;
    private bool hasExited = false;

    // Start is called before the first frame update
    void Awake()
    {
        ownPosition = transform.position;
    }

    void Start()
    {
        bubbleRender.SetActive(true);
        laserRender.enabled = showBubble;
        bubbleOutline.SetActive(showBubble);
        bubbleSphere.SetActive(showBubble);
        controllerRender.SetActive(true);
        motorSpaceRender.color = motorDisabledColor;

        //TEMP:CHANGE THIS
        outOfBoundIndicatorManager = dynamicCenterPointingIndicator;
        CurrentArrowType = ArrowType.DynamicCenter;
    }

    // Update is called once per frame
    void Update()
    {
        // Update our world position to be equivalent to the parent, for the axis chosen.
        newPosX = parentX ? parent.transform.position.x : ownPosition.x;
        newPosY = parentY ? parent.transform.position.y : ownPosition.y;
        newPosZ = parentZ ? parent.transform.position.z : ownPosition.z;

        Vector3 newPos = new Vector3(newPosX, newPosY, newPosZ);

        Vector3 clipPos = laserMapper.RubberClipToMotorSpace(newPos);
        this.transform.position = new Vector3(clipPos.x + offsetX, clipPos.y + offsetY, clipPos.z + offsetZ);

        prevPosX = newPosX;
        prevPosY = newPosY;
        prevPosZ = newPosZ;

        Side side = laserMapper.NearestSide(newPos);
        if (laserMapper.CoordinateWithinMotorSpace(newPos))
        {
            if (action == MotorAction.Outside || action == MotorAction.None || action == MotorAction.Exit)
            {
                action = MotorAction.Enter;
                Debug.Log($"Entered MotorSpace at position {newPos}, side {side}");
                bubbleRender.SetActive(true);
                laserRender.enabled = showBubble;
                //StartCoroutine(OutOfBoundAnimation(false));
                bubbleOutline.SetActive(showBubble);
                bubbleSphere.SetActive(showBubble);
                controllerRender.SetActive(true);
                motorSpaceRender.color = motorActiveColor;

                // Hide indicator
                outOfBoundIndicatorManager.HideIndicator();

                enterMotorStateEvent.Invoke(new EnterMotorSpaceInfo
                {
                    side = side,
                    enter = true,
                    motorLastPos = newPos,
                    wallLastPos = laserMapper.ConvertMotorSpaceToWallSpace(newPos),
                });
                action = MotorAction.Inside;

                LogReturnTime();

            }
        }
        else
        {
            if (action == MotorAction.Inside || action == MotorAction.None || action == MotorAction.Enter)
            {
                Debug.Log($"Exited MotorSpace at position {newPos}, side {side}");
                action = MotorAction.Exit;

                bubbleRender.SetActive(true);
                laserRender.enabled = showBubble;
                bubbleOutline.SetActive(showBubble);
                bubbleSphere.SetActive(showBubble);
                controllerRender.SetActive(true);
                motorSpaceRender.color = motorDisabledColor;

                // Show indicator
                outOfBoundIndicatorManager.ShowIndicator(newPos, laserMapper.transform.position, side);

                enterMotorStateEvent.Invoke(new EnterMotorSpaceInfo
                {
                    side = side,
                    enter = false,
                    motorLastPos = newPos,
                    wallLastPos = laserMapper.ConvertMotorSpaceToWallSpace(newPos),
                });
                action = MotorAction.Outside;

                exitTime = DateTime.Now;
                hasExited = true;
            }
        }
    }


    public void Show(bool show) {
        showBubble = show;
    }

    public void UpdateOwnPosition(Vector3 newPosition) {
        ownPosition = newPosition;
    }

    public void InstantiateInCircle(GameObject container, Vector3 location, int howMany, float radius, float zPosition)
    {
        int i = 0;
        float angleSection = (Mathf.PI * 2f / howMany);
        foreach (Transform prefab in container.transform)
        {
            float angle = i * angleSection + (3 * Mathf.PI / 4);
            Vector3 newPos = location + new Vector3(Mathf.Cos(angle) * 1.5f, Mathf.Sin(angle), 0) * radius;
            newPos.z = zPosition;
            prefab.transform.position = newPos;
            i++;
        }
    }

    public void InstantiateInCircle(GameObject container, Vector3 location, int howMany, float radius)
    {
        InstantiateInCircle(container, location, howMany, radius, location.z);
    }

    public void InstantiateInCircle(GameObject container, int howMany, float radius)
    {
        InstantiateInCircle(container, laserMapper.transform.position, howMany, radius);
    }

    

    public void ChangeIndicator(ArrowType arrowType)
    {
        outOfBoundIndicatorManager = arrowType switch
        {
            ArrowType.StaticPointing => staticArrowIndicator,
            ArrowType.DynamicCenter => dynamicCenterPointingIndicator,
            _ => staticArrowIndicator,
        };
        CurrentArrowType = arrowType;
    }

    // Log the time spent outside the MotorSpace and the type of indicator used
    private void LogReturnTime()
    {
        // Check if the user has already exited the MotorSpace
        if (hasExited)
        {
            TimeSpan timeSpentOutside = DateTime.Now - exitTime;
            // Log the time spent outside
            Debug.Log($"Time spent outside the MotorSpace: {timeSpentOutside.TotalSeconds} seconds. Indicator type: {outOfBoundIndicatorManager.GetType().Name}");
            // Reset the exit time and the hasExited flag
            hasExited = false;
            //}
            //else
            //{
            //    Debug.Log("Entered MotorSpace for the first time.");
            //}
        }

        //private IEnumerator OutOfBoundAnimation(bool isOutOfBound)
        //{
        //    while (isOutOfBound)
        //    {
        //        InstantiateInCircle(OutOfBoundContainer, OutOfBoundContainer.transform.childCount, laserMapper.GetMotorSpace().width + 0.1f);

        //        foreach (Transform child in OutOfBoundContainer.transform)
        //        {
        //            var motorSpaceCenter = laserMapper.transform.position;

        //            var currentPos = child.position;
        //            var pointA = new Vector3(currentPos.x, currentPos.y, currentPos.z);
        //            child.position = Vector3.Lerp(pointA, motorSpaceCenter, Mathf.PingPong(Time.time * 0.3f, 0.15f));

        //            //Change the rotation of the objects to point them all to the center of the MotorSpace
        //            var difference = motorSpaceCenter - child.position;
        //            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        //            child.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
        //        }
        //        yield return null;
        //    }
        //}



        //public IEnumerator FadeOutObject(GameObject gameObjects, float fadeSpeed)
        //{
        //    foreach (Transform o in gameObjects.transform)
        //    {
        //        while (o.GetComponent<Renderer>().material.color.a > 0)
        //        {
        //            Color objectColor = o.GetComponent<Renderer>().material.color;
        //            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

        //            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
        //            o.GetComponent<Renderer>().material.color = objectColor;
        //            yield return null;
        //        }
        //    }
        //    OutOfBoundContainer.SetActive(false);
        //}

        //public IEnumerator FadeInObject(GameObject gameObjects, float fadeSpeed)
        //{
        //    OutOfBoundContainer.SetActive(true);

        //    foreach (Transform o in gameObjects.transform)
        //    {
        //        while (o.GetComponent<Renderer>().material.color.a < 1)
        //        {
        //            Color objectColor = o.GetComponent<Renderer>().material.color;
        //            float fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

        //            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
        //            o.GetComponent<Renderer>().material.color = objectColor;
        //            yield return null;
        //        }
        //    }
        //}
    }
}