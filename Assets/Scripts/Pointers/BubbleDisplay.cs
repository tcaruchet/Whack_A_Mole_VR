using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Assets.Scripts.HUD;
using static OutOfBoundsArrow;

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

    // Start is called before the first frame update
    void Awake()
    {

        ownPosition = transform.position;

        // Disable OutOfBound Animations onthe motorspace for the time being.
        //for (int i = 0; i < numberOfObjects; i++)
        //{
        //    Instantiate(OutOfBoundPrefab, ownPosition, Quaternion.identity, OutOfBoundContainer.transform);
        //}
    }

    void Start()
    {
        bubbleRender.SetActive(true);
        laserRender.enabled = showBubble;
        bubbleOutline.SetActive(showBubble);
        bubbleSphere.SetActive(showBubble);
        controllerRender.SetActive(true);
        motorSpaceRender.color = motorDisabledColor;
        outOfBoundIndicatorManager = staticArrowIndicator;
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

        if (laserMapper.CoordinateWithinMotorSpace(newPos))
        {
            if (action == MotorAction.Outside || action == MotorAction.None)
            {
                action = MotorAction.Enter;
                Debug.Log("Enter");
                bubbleRender.SetActive(true);
                laserRender.enabled = showBubble;
                StartCoroutine(OutOfBoundAnimation(false));
                bubbleOutline.SetActive(showBubble);
                bubbleSphere.SetActive(showBubble);
                controllerRender.SetActive(true);
                motorSpaceRender.color = motorActiveColor;

                // Hide indicator
                outOfBoundIndicatorManager.HideIndicator();

                enterMotorStateEvent.Invoke(new EnterMotorSpaceInfo
                {
                    side = laserMapper.NearestSide(newPos),
                    enter = true,
                    motorLastPos = newPos,
                    wallLastPos = laserMapper.ConvertMotorSpaceToWallSpace(newPos),
                });
                action = MotorAction.Inside;
            }
        }
        else
        {
            if (action == MotorAction.Inside || action == MotorAction.None)
            {
                Debug.Log("Exit");
                action = MotorAction.Exit;

                bubbleRender.SetActive(true);
                laserRender.enabled = showBubble;
                bubbleOutline.SetActive(showBubble);
                bubbleSphere.SetActive(showBubble);
                controllerRender.SetActive(true);
                motorSpaceRender.color = motorDisabledColor;

                // Show indicator
                outOfBoundIndicatorManager.ShowIndicator(newPos, laserMapper.NearestSide(newPos));

                enterMotorStateEvent.Invoke(new EnterMotorSpaceInfo
                {
                    side = laserMapper.NearestSide(newPos),
                    enter = false,
                    motorLastPos = newPos,
                    wallLastPos = laserMapper.ConvertMotorSpaceToWallSpace(newPos),
                });
                action = MotorAction.Outside;
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

    private IEnumerator OutOfBoundAnimation(bool isOutOfBound)
    {
        while (isOutOfBound)
        {
            InstantiateInCircle(OutOfBoundContainer, OutOfBoundContainer.transform.childCount, laserMapper.GetMotorSpace().width + 0.1f);

            foreach (Transform child in OutOfBoundContainer.transform)
            {
                var motorSpaceCenter = laserMapper.transform.position;

                var currentPos = child.position;
                var pointA = new Vector3(currentPos.x, currentPos.y, currentPos.z);
                child.position = Vector3.Lerp(pointA, motorSpaceCenter, Mathf.PingPong(Time.time * 0.3f, 0.15f));

                //Change the rotation of the objects to point them all to the center of the MotorSpace
                var difference = motorSpaceCenter - child.position;
                float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                child.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
            }
            yield return null;
        }
    }

    public void ChangeIndicator(ArrowType arrowType)
    {
        switch (arrowType)
        {
            case ArrowType.SideAligned:
                outOfBoundIndicatorManager = staticArrowIndicator;
                break;
            case ArrowType.PointingCenter:
                outOfBoundIndicatorManager = dynamicCenterPointingIndicator;
                break;
            default:
                outOfBoundIndicatorManager = staticArrowIndicator;
                break;
        }
    }


    public IEnumerator FadeOutObject(GameObject gameObjects, float fadeSpeed)
    {
        foreach (Transform o in gameObjects.transform)
        {
            while (o.GetComponent<Renderer>().material.color.a > 0)
            {
                Color objectColor = o.GetComponent<Renderer>().material.color;
                float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                o.GetComponent<Renderer>().material.color = objectColor;
                yield return null;
            }
        }
        OutOfBoundContainer.SetActive(false);
    }

    public IEnumerator FadeInObject(GameObject gameObjects, float fadeSpeed)
    {
        OutOfBoundContainer.SetActive(true);

        foreach (Transform o in gameObjects.transform)
        {
            while (o.GetComponent<Renderer>().material.color.a < 1)
            {
                Color objectColor = o.GetComponent<Renderer>().material.color;
                float fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                o.GetComponent<Renderer>().material.color = objectColor;
                yield return null;
            }
        }
    }
}