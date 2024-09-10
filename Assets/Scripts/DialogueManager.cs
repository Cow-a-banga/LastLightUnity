using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    [Tooltip("JSON файл диалога")]
    public TextAsset inkJson;

    private Story _story;

    public bool isOver => !_story.canContinue && _story.currentChoices.Count == 0;
    
    public bool canContinue => _story.canContinue;
    public bool hasChoices => _story.currentChoices.Count > 0;

    public void StartStory()
    {
        _story = new Story(inkJson.text);
    }

    public string Continue()
    {
        return _story.Continue();
    }
    
    public List<Choice> GetChoices()
    {
        return _story.currentChoices;
    }

    public void Choice(int index)
    {
        _story.ChooseChoiceIndex(index);
    }
}
