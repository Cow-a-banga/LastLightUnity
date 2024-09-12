using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ActionInteractor : MonoBehaviour
{
    [Tooltip("JSON файл диалога")]
    public TextAsset InkJson;
    
    [Tooltip("Имя персонажа для диалогов")]
    public string CharacterName;
    
    public GameObject InteractionIcon;
    public DialogueManager DialogueManager;
    
    private Camera _camera;
    private BoxCollider _collider;
    
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        InteractionIcon.transform.LookAt(_camera.transform);
        InteractionIcon.transform.Rotate(0, 180, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractionIcon.SetActive(true);
        DialogueManager.InkJson = InkJson;
        DialogueManager.CharacterName = CharacterName;
    }

    private void OnTriggerExit(Collider other)
    {
        InteractionIcon.SetActive(false);
        DialogueManager.InkJson = null;
        DialogueManager.CharacterName = null;
    }
}
