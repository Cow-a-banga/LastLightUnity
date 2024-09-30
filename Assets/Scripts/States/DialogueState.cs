using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueState : MonoBehaviour, IState
{
    public DialogueManager _dialogueManager;
    
    void Start()
    {
        
    }
    
    public State Name => State.Dialogue;

    public bool Check()
    {
        return _dialogueManager.Story != null;
    }

    public void Initialize()
    {
        Debug.Log("Dialogue init");
    }

    public void Do(float deltaTime) {}

    public void End()
    {
        Debug.Log("Dialogue end");
    }
}
