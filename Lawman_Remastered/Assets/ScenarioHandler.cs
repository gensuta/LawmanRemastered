using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class ScenarioHandler : MonoBehaviour
{

    DialogueRunner dr;
    InMemoryVariableStorage yarnVars;

    [SerializeField]
    float timeTillContinue, maxTime, timeTillPostIntro, timer;

    public bool isOptionsVisible;

    [Space]
    [Header("Vars for story")]
    public int warnings;
    public int numPresses,opponentBullets = 3;
    public bool saidYoMama, saidBlam,startedIntro,startedMonologue;

    [Space]
    [Header("Node Names")]
    [SerializeField] string intro;
    public string introFinal, startMonologue, startBanter, startDrink, startSob, yoMamaStart, noMoreBullets,noMoreDialogue,fancyFootworks, farDialogue,closeDialogue;

    public bool gameOver;
    [SerializeField] bool _isTyping;

    [SerializeField] GameObject endPanel;
    [SerializeField] PlayerBehavior player;

    bool shouldChooseRand;
    public string nodeBeforeWarning;

    bool didBanter, didMonologue, didSob, didDrink;

    // Start is called before the first frame update
    void Start()
    {
        yarnVars = GetComponent<InMemoryVariableStorage>();

        dr = FindObjectOfType<DialogueRunner>();
    }



    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SceneManager.LoadScene(0);

            endPanel.SetActive(true);
        }
        else
        {
            opponentBullets = (int)GetValue("$bullets").AsNumber;
            gameOver = GetValue("$gameOver").AsBool;
            shouldChooseRand = GetValue("$endNode").AsBool;

            if (!isTyping() && timer == 0) timeTillContinue -= Time.deltaTime;

            if (GetCurrentNode() == introFinal) startedIntro = true;
            if (startedIntro && !isTyping()) timer += Time.deltaTime;

            if (!dr.IsDialogueRunning) _isTyping = false;

            if (timeTillContinue <= 0 && !isOptionsVisible)
            {
                Continue();
            }

            if (timer > timeTillPostIntro) 
                ChooseRandDialogue();

            if(shouldChooseRand && !isTyping())
            {
                startedIntro = true;

                var endBool = new Yarn.Value(true);
                SetValue("endNode", endBool);
                shouldChooseRand = false;
            }
        }
    }

    public void ChooseRandDialogue()
    {
        timer = 0;
        startedIntro = false;
        if (opponentBullets == 0)
        {
            StartNewNode(noMoreBullets);
            return;
        }

        if (player.distanceFromEnemy < player.warningDist)
        {
            StartNewNode(closeDialogue);
        }
        else if (player.distanceFromEnemy >25)
        {
            StartNewNode(farDialogue);
        }

        else if (numPresses > 5)
        {
            StartNewNode(fancyFootworks);
        }

        else
        {

            int rand = Random.Range(0, 4);
            if (rand == 0)
            {
                if (didMonologue)
                {
                    if (!didSob)
                    {
                        StartNewNode(startSob);
                        didSob = true;
                        return;
                    }
                    else if (!didDrink)
                    {
                        StartNewNode(startDrink);
                        didMonologue = true;
                        return;
                    }
                    else if (!didBanter)
                    {
                        StartNewNode(startBanter);
                        didBanter = true;
                        return;
                    }
                    else
                    {
                        StartNewNode(noMoreDialogue);
                    }
                }
                else
                {
                    startedMonologue = true;
                    StartNewNode(startMonologue);
                    didMonologue = true;
                }
            }
            if (rand == 1)
            {
                if (didBanter)
                {
                    if (!didMonologue)
                    {
                        StartNewNode(startMonologue);
                        didMonologue = true;
                        return;
                    }
                    else if (!didDrink)
                    {
                        StartNewNode(startDrink);
                        didMonologue = true;
                        return;
                    }
                    else if(!didSob)
                    {
                        StartNewNode(startSob);
                        didSob = true;
                        return;
                    }
                    else
                    {
                        StartNewNode(noMoreDialogue);
                    }
                }
                else
                {
                    StartNewNode(startBanter);
                    didBanter = true;
                }
            }
            if (rand == 2)
            {
                if (didDrink)
                {
                    if (!didMonologue)
                    {
                        StartNewNode(startMonologue);
                        didMonologue = true;
                        return;
                    }
                    else if (!didSob)
                    {
                        StartNewNode(startSob);
                        didSob = true;
                        return;
                    }
                    else if (!didBanter)
                    {
                        StartNewNode(startBanter);
                        didBanter = true;
                        return;
                    }
                    else
                    {
                        StartNewNode(noMoreDialogue);
                    }
                }
                else
                {
                    StartNewNode(startDrink);
                    didDrink = true;
                }
            }
            if (rand == 3)
            {
                if (didSob)
                {
                    if (!didMonologue)
                    {
                        StartNewNode(startMonologue);
                        didMonologue = true;
                        return;
                    }
                    else if (!didDrink)
                    {
                        StartNewNode(startDrink);
                        didMonologue = true;
                        return;
                    }
                    else if (!didBanter)
                    {
                        StartNewNode(startBanter);
                        didBanter = true;
                        return;
                    }
                    else
                    {
                        StartNewNode(noMoreDialogue);
                    }
                }
                else
                {
                    StartNewNode(startSob);
                    didSob = true;
                }

            }
        }
    }

    public string GetCurrentNode()
    {
        return dr.CurrentNodeName;
    }

    public bool isRunning()
    {
        return dr.IsDialogueRunning;
    }

    public bool isTyping()
    {
        return _isTyping;
    }

    public void OnComplete()
    {
        _isTyping = false;
    }

    public void OnStart()
    {
        _isTyping = true;
    }

    public void StartNewNode(string s)
    {
        dr.Stop();
        dr.StartDialogue(s);
    }

    void Continue()
    {
        OnStart();

        dr.Dialogue.Continue();
        ResetTimer();

        if (!saidYoMama && GetCurrentNode() == yoMamaStart)
        {
            saidYoMama = true;
        }

    }

    public void SetOptionsVisible(bool b)
    {
        isOptionsVisible = b;
    }
    public void ResetTimer()
    {
        timeTillContinue = maxTime;
    }


    public void SetValue(string variableName, Yarn.Value value)
    {
        yarnVars.SetValue(variableName, value);
    }

    // Return a value, given a variable name
    public Yarn.Value GetValue(string variableName)
    {
        return yarnVars.GetValue(variableName);
      
    }

}
