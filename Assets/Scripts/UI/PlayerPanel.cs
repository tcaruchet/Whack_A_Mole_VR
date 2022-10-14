using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FadingUtils;

/*
Class dedicated to the player panel. Updates the displayed information, hides/shows the panel and switches between the 
information panel and the "game paused" panel.
*/

public class PlayerPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject infoContainer;

    [SerializeField]
    private GameObject gamePausedContainer;

    [SerializeField]
    private GameObject gameCountDownContainer;

    [SerializeField]
    private GameObject instructionContainer;

    //[SerializeField]
    //private GameObject gameEndingContainer;

    private SoundManager soundManager;
    private LoggerNotifier loggerNotifier;

    [SerializeField]
    private Text timeText;

    [SerializeField]
    private Text participantText;

    [SerializeField]
    private Text testText;

    [SerializeField]
    private Text countDownText;
    private string countDownTextTemplate;

    [SerializeField]
    private Text instructionText;
    private string instructionTextDefault;

    private Canvas panelCanvas;

    private FadingHelper gamePausedFader;               // Handles gamePauseContainer
    private FadingHelper gameCountDownFader;            // Handles gameCountDownContainer
    private FadingHelper infoFader;                     // Handles infoContainer
    private FadingHelper instructionContainerFader;     // Handles instructionContainer
    private FadingHelper instructionTxtFader;           // Handles instructionText
    //private FadingHelper gameEndingFader;               // Handles instructionText

    void Awake()
    {
        panelCanvas = gameObject.GetComponentInChildren<Canvas>();
        countDownTextTemplate = countDownText.text;
        instructionTextDefault = instructionText.text;
    }

    void Start()
    {
        gamePausedFader = gamePausedContainer.GetComponent<FadingHelper>();
        gameCountDownFader = gameCountDownContainer.GetComponent<FadingHelper>();
        infoFader = infoContainer.GetComponent<FadingHelper>();
        instructionContainerFader = instructionContainer.GetComponent<FadingHelper>(); 
        //gameEndingFader = gameEndingContainer.GetComponent<FadingHelper>(); 

        loggerNotifier = new LoggerNotifier(persistentEventsHeadersDefaults: new Dictionary<string, string>(){
            {"GameMessage", "NULL"}
        });
        soundManager = FindObjectOfType<SoundManager>();
    }

    // Hides/shows the panel
    public void SetEnablePanel(bool enable = true)
    {
        panelCanvas.enabled = enable;
    }

    // Switches between the "game paused" panel and the information panel.
    public void SetPausedContainer(bool pausedContainer = true)
    {
        if (pausedContainer)
        {
            infoContainer.SetActive(false);
            FadingUtils.FadingUtils.FadingOutPlusDisabling(infoFader);
            FadingUtils.FadingUtils.FadingInPlusEnabling(instructionContainerFader);
            FadingUtils.FadingUtils.FadingInPlusEnabling(gamePausedFader);
        }
        else
        {
            FadingUtils.FadingUtils.FadingOutPlusDisabling(gamePausedFader);
            FadingUtils.FadingUtils.FadingInPlusEnabling(infoFader);
            infoContainer.SetActive(true);
        }
    }

    // Switches between the "countdown" panel and the information panel.
    public void SetCountDownContainer(bool countDownContainer = true) {

        if (countDownContainer)
        {
            FadingUtils.FadingUtils.FadingOutPlusDisabling(infoFader);
            FadingUtils.FadingUtils.FadingInPlusEnabling(gameCountDownFader);
        }
        else
        {
            FadingUtils.FadingUtils.FadingOutPlusDisabling(gameCountDownFader);
            FadingUtils.FadingUtils.FadingInPlusEnabling(infoFader);
            SetInstructionText(instructionTextDefault);
        }
    }

    // Updates the participant Id, game duration and test Id displayed on the information panel.
    public void UpdateDisplayedInfos(Dictionary<string, object> data)
    {
        foreach(KeyValuePair<string, object> entry in data)
        {
            switch(entry.Key)
            {
                case "ParticipantId":
                    participantText.text = "Participant " + entry.Value.ToString();
                    break;

                case "TestId":
                    testText.text = "Test " + entry.Value.ToString();
                    break;
                
                case "GameDuration":
                    timeText.text = entry.Value.ToString() + " minutes";
                    break;
            }
        }
    }

    // Updates the countdown in the countdown panel.
    public void UpdateCountDownInfo(int count) {
        countDownText.text = string.Format(countDownTextTemplate, count.ToString());
    }

    public void SetInstructionText(string text) {
        instructionText.text = text;
    }    

    public void SetMessage(string text, float time) {
        SetInstructionText(text);
        StartCoroutine(WaitShowMessage(time, text));
    }

    private IEnumerator WaitShowMessage(float duration, string text) {
        float currentCountDownLeft = duration;
        int currentCountDownLeftRounded = -1;
        int prevCount = -1;
        SetEnablePanel(true);
        SetCountDownContainer(true);

        while (currentCountDownLeft > -0.9) {
            prevCount = currentCountDownLeftRounded;

            currentCountDownLeft -= Time.deltaTime;
            currentCountDownLeftRounded = (int) Mathf.Ceil(currentCountDownLeft);
            
            if (currentCountDownLeftRounded != prevCount) {
                var data = new Dictionary<string, object>()
                {
                    {"CountDown", currentCountDownLeftRounded}
                };
                UpdateCountDownInfo(currentCountDownLeftRounded);
                soundManager.PlaySound(gameObject, SoundManager.Sound.countdown);
                loggerNotifier.NotifyLogger("CountDown " + currentCountDownLeftRounded.ToString(), EventLogger.EventType.GameEvent, new Dictionary<string, object>()
                {
                    {"GameMessage", text}
                });

            }
            yield return null;
        }
        SetEnablePanel(false);
        SetCountDownContainer(false);
    }

}
