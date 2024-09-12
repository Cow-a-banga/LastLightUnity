using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using Ink.UnityIntegration;

public class DialogueManager : MonoBehaviour
{
    public TextAsset InkJson { get; set; }
    public string CharacterName { get; set; }

    [Tooltip("Файл с глобальными переменными ink")]
    public InkFile globalsInkFile;

    private Story _story;

    public bool isOver => !_story.canContinue && _story.currentChoices.Count == 0;
    
    public bool canContinue => _story.canContinue;
    public bool hasChoices => _story.currentChoices.Count > 0;
    public DialogueVariables DialogueVariables {get; private set;}

    public void Awake()
    {
        DialogueVariables = new(globalsInkFile.filePath);
    }

    public void StartStory()
    {
        _story = new Story(InkJson.text);
        DialogueVariables.StartListening(_story);
    }

    public void StopStory()
    {
        DialogueVariables.StopListening(_story);
        _story = null;
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
