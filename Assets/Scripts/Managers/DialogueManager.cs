using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    //current turn
    public static DialogueManager _instance;
    [SerializeField] private TMP_Text dialogueBox;
    [SerializeField] private GameObject nextButton;
    private int dialogueIndex=0;
    private Dialogue dialogue;



    void Awake()
    {
        
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }
    private void OnEnable(){
        WriterEffect.CompleteTextRevealed+=ShowButton;
    }

    private void OnDisable(){
        WriterEffect.CompleteTextRevealed-=ShowButton;
    }


    public void ShowDialogue(String message){
        dialogueBox.maxVisibleCharacters=0;
        dialogueBox.text=message.Replace("{name}", Game._instance.hero.name);
        
    }

    public void NextDialogue(){
        HideButton();
        dialogueIndex++;
        if(dialogue.messages.Count<=dialogueIndex)
            gameObject.SetActive(false);
        else{
            ShowDialogue(dialogue.messages[dialogueIndex]);
        }
    }

    public void StartDialogue(Dialogue dialogue){
        dialogueIndex=0;
        this.dialogue=dialogue;
        ShowDialogue(dialogue.messages[dialogueIndex]);
    }

    private void ShowButton(){
        nextButton.SetActive(true); 
    }
    private void HideButton(){
        nextButton.SetActive(false); 
    }



}
