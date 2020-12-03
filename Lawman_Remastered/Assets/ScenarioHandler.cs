using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class ScenarioHandler : MonoBehaviour
{
    [SerializeField]
    string[] startingNodes;// the list of nodes we can start at
    int rand;


    bool nodesLoaded; // since dr doesn't load nodes on awake we need this =_=
    DialogueRunner dr;
    InMemoryVariableStorage yarnVars;

    [SerializeField]
    float timeTillContinue, maxTime;

    public bool isOptionsVisible;

    // Start is called before the first frame update
    void Start()
    {
        yarnVars = GetComponent<InMemoryVariableStorage>();

        rand = Random.Range(0, startingNodes.Length);
        dr = FindObjectOfType<DialogueRunner>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isOptionsVisible) timeTillContinue -= Time.deltaTime;

        if(dr.NodeExists(startingNodes[rand]) && !nodesLoaded)
        {
            dr.startNode = startingNodes[rand];
            dr.StartDialogue();
            nodesLoaded = true;
        }
        

        if(timeTillContinue <= 0 && !isOptionsVisible)
        {
            Continue();
        }
    }

    void Continue()
    {
        dr.Dialogue.Continue();
        ResetTimer();
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
