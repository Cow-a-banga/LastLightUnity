using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueInteractManager : MonoBehaviour
{

    [Tooltip("Объект с UI для диалога")]
    public GameObject DialogueUI;

    [Tooltip("Клавиша включения")]
    public KeyCode switchKey = KeyCode.E;

    [Tooltip("Имя главного героя")]
    public string PlayerName = "Ария";
    
    private UIDocument _document;
    private DialogueManager _manager;
    private Label _textLabel;
    private VisualElement _choiceContainer;
    private List<Choice> _choices;

    private void ShowPhrase(string lineFromInk)
    {
        var parts = lineFromInk.Split(':');
        string phraseName = null;

        if (parts.Length == 2)
        {
            phraseName = parts[0].Trim() switch
            {
                "Me" => PlayerName,
                "They" => _manager.CharacterName,
                _ => null
            };
        }
        
        var message = (parts.Length == 2 ? parts[1] : parts[0]).Trim();

        if (phraseName is not null)
        {
            Debug.Log(phraseName);
        }
        
        _textLabel.text = message;
    }

    
    void OnEnable()
    {
        _document = DialogueUI.GetComponent<UIDocument>();
        _manager = GetComponent<DialogueManager>();
        _textLabel = _document.rootVisualElement.Q<Label>("TextLabel");
        _choiceContainer = _document.rootVisualElement.Q("Choices");
        _document.rootVisualElement.style.display = DisplayStyle.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(switchKey) && _manager.InkJson is not null)
        {
            if (_document.rootVisualElement.style.display == DisplayStyle.None){
                _manager.StartStory();
                _document.rootVisualElement.style.display = DisplayStyle.Flex;
                ShowPhrase(_manager.Continue());
            }
            else if (_manager.isOver)
            {
                _manager.StopStory();
                _document.rootVisualElement.style.display = DisplayStyle.None;
            }
            else
            {
                if (_manager.canContinue)
                {
                    ShowPhrase(_manager.Continue());
                    ClearChoices();
                }
                else if (_manager.hasChoices)
                {
                    _choices = _manager.GetChoices();
                    DisplayChoices(_choices);
                }   
                
            }
        } 
        
        if (_choices is { Count: > 0 })
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && _choices.Count >= 1)
            {
                OnChoiceSelected(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && _choices.Count >= 2)
            {
                OnChoiceSelected(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && _choices.Count >= 3)
            {
                OnChoiceSelected(2);
            }
        }
    }
    
    private void DisplayChoices(IReadOnlyList<Choice> choices)
    {
        ClearChoices();

        for (var i = 0; i < choices.Count; i++)
        {
            var index = i;
            var button = new Button(() => OnChoiceSelected(index)) { text = choices[i].text};
            _choiceContainer.Add(button);
        }
    }
    
    private void ClearChoices()
    {
        _choiceContainer.Clear();
    }
    
    private void OnChoiceSelected(int index)
    {
        _manager.Choice(index);
        var value = _manager.Continue();
        _textLabel.text = value;
        ClearChoices();
    }
}
