using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System.IO;

public class DialogueVariables
{
    public Dictionary<string, Ink.Runtime.Object> Variables {get; private set;}

    public DialogueVariables(string globalsPath)
    {
        string inkContent = File.ReadAllText(globalsPath);
        Ink.Compiler compiler = new Ink.Compiler(inkContent);
        Story globals = compiler.Compile();

        Variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (var name in globals.variablesState)
        {
            var value = globals.variablesState.GetVariableWithName(name);
            Variables.Add(name, value);
        }

    }

    public void StartListening(Story story)
    {
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += SetVariable;
    }

    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= SetVariable;
    }

    public void SetVariable(string name, Ink.Runtime.Object value)
    {
        if(Variables.ContainsKey(name))
        {
            Variables.Remove(name);
            Variables.Add(name, value);
        }
    }

    private void VariablesToStory(Story story)
    {
        foreach (var variable in Variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }

    public Ink.Runtime.Object GetVariable(string name)
    {
        if(Variables.ContainsKey(name))
        {
            return Variables[name];
        }
        return null;
    }
}
