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

    public Story Story;

    public bool isOver => !Story.canContinue && Story.currentChoices.Count == 0;
    
    public bool canContinue => Story.canContinue;
    public bool hasChoices => Story.currentChoices.Count > 0;
    public DialogueVariables DialogueVariables {get; private set;}

    public void Awake()
    {
        DialogueVariables = new(globalsInkFile.filePath);
    }

    public void StartStory()
    {
        Story = new Story(InkJson.text);
        DialogueVariables.StartListening(Story);
    }

    public void StopStory()
    {
        DialogueVariables.StopListening(Story);
        Story = null;
    }

    public string Continue()
    {
        return Story.Continue();
    }
    
    public List<Choice> GetChoices()
    {
        return Story.currentChoices;
    }

    public void Choice(int index)
    {
        Story.ChooseChoiceIndex(index);
    }
}
