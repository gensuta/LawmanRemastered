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
    public int opponentBullets = 3;
    public bool saidYoMama, saidBlam,startedIntro,startedMonologue;

    [Space]
    [Header("Node Names")]
    [SerializeField] string intro;
    public string introFinal, startMonologue, startBanter, yoMamaStart;

    public bool gameOver;
    [SerializeField] bool _isTyping;

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
        }
        else
        {
            opponentBullets = (int)GetValue("$bullets").AsNumber;
            gameOver = GetValue("$gameOver").AsBool;

            if (!isTyping() && timer == 0) timeTillContinue -= Time.deltaTime;

            if (GetCurrentNode() == introFinal) startedIntro = true;
            if (startedIntro && !isTyping()) timer += Time.deltaTime;

            if (timer > timeTillPostIntro) ChooseRandDialogue();

            if (!dr.IsDialogueRunning) _isTyping = false;

            if (timeTillContinue <= 0 && !isOptionsVisible)
            {
                Continue();
            }
        }
    }

    public void ChooseRandDialogue()
    {
        timer = 0;
        startedIntro = false;
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            startedMonologue = true;
            StartNewNode(startMonologue);
        }
        if (rand == 1) StartNewNode(startBanter);
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

        if(!saidYoMama && GetCurrentNode() == yoMamaStart)
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
