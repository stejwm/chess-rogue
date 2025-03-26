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

public enum EncounterType
    {
        EscapedPawn,
        Blacksmith
    }

public class DialogueManager : MonoBehaviour
{
    
    //current turn
    public static DialogueManager _instance;
    [SerializeField] private TMP_Text dialogueBox;
    [SerializeField] private GameObject optionsContainer;
    [SerializeField] private GameObject optionButtonPrefab;
    [SerializeField] private SpriteRenderer speakerSprite;
    private int dialogueIndex=0;
    private Dialogue dialogue;
    private Dialogue.DialogueMessage currentMessage;



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
        WriterEffect.CompleteTextRevealed+=ShowOptions;
    }

    private void OnDisable(){
        WriterEffect.CompleteTextRevealed-=ShowOptions;
    }

    public void Start()
    {
        gameObject.SetActive(false);
    }


    public void ShowDialogue(Dialogue.DialogueMessage message)
    {
        dialogueBox.maxVisibleCharacters = 0;
        currentMessage = message;
        dialogueBox.text = message.message.Replace("{name}", Game._instance.hero.name);
        
    }

    public void ShowOptions(){
        List<TMP_Text> optionTexts = new List<TMP_Text>();
        if (currentMessage.options != null && currentMessage.options.Count > 0)
        {
            foreach (var option in currentMessage.options)
            {

                var buttonObj = Instantiate(optionButtonPrefab, optionsContainer.transform);
                var button = buttonObj.GetComponent<Button>();
                var text = buttonObj.GetComponentInChildren<TMP_Text>();
                buttonObj.SetActive(true);
                text.text = option.optionText;
                optionTexts.Add(text);
                button.onClick.AddListener(() => {
                    DialogueEventRouter._instance.TriggerEvent(option.eventName);
                    if (option.nextDialogue !=null)
                    {
                        // Load and start new dialogue
                        StartDialogue(option.nextDialogue);
                    }
                    else
                    {
                        NextDialogue();
                    }
                });
            }
            StartCoroutine(AdjustFontSizes(optionTexts));
        }
    }
    public void ClearOptions()
    {
        foreach (Transform child in optionsContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    public void NextDialogue()
    {
        ClearOptions();
        dialogueIndex++;
        if (dialogue.messages.Count <= dialogueIndex)
        {
            EndDialogue();
        }
        else
        {
            ShowDialogue(dialogue.messages[dialogueIndex]);
        }
    }

    public void EndDialogue()
    {
        gameObject.SetActive(false);
        Game._instance.isInMenu = false;
    }

    public void StartDialogue(Dialogue dialogue){
        this.gameObject.SetActive(true);
        ClearOptions();
        Game._instance.isInMenu=true;
        speakerSprite.sprite = dialogue.sprite;
        dialogueIndex=0;
        this.dialogue=dialogue;
        ShowDialogue(dialogue.messages[dialogueIndex]);
    }


    public void EscapeeTeachings(){
        if(Game._instance.hero.orders.Count>0){
            Game._instance.hero.orders.RemoveAt(UnityEngine.Random.Range(0,Game._instance.hero.orders.Count));
            foreach (var pieceObj in Game._instance.hero.pieces){
                var piece = pieceObj.GetComponent<Chessman>();
                if (piece.type != PieceType.King && piece.type != PieceType.Queen)
                {
                    piece.support++;
                }
        }
        }
        
    }

    public void UpgradeAttack(){
        if(Game._instance.hero.playerCoins>=75){
            Game._instance.hero.playerCoins-=75;
            foreach (var pieceObj in Game._instance.hero.pieces){
                var piece = pieceObj.GetComponent<Chessman>();
                piece.attack++;
            }
            EndDialogue();
        }
        else{
            StartDialogue(Resources.Load<Dialogue>("Objects/Dialogues/BlacksmithNotEnoughCoins"));
        }
    }

    public void UpgradeAttackCost(){
        Game._instance.hero.playerCoins=0;
        foreach (var pieceObj in Game._instance.hero.pieces){
            var piece = pieceObj.GetComponent<Chessman>();
            piece.attack++;
        }
        EndDialogue();
    }

    public void DowngradeAttack(){
        
        foreach (var pieceObj in Game._instance.hero.pieces){
            var piece = pieceObj.GetComponent<Chessman>();
            piece.attack--;
        }
        EndDialogue();
    }

    public void DowngradeAttackCost(){
        
        Game._instance.hero.playerCoins=0;
        foreach (var pieceObj in Game._instance.hero.pieces){
            var piece = pieceObj.GetComponent<Chessman>();
            piece.attack--;
        }
        EndDialogue();
    }

    public void AddEscapee(){
        var hero = Game._instance.hero;
        var pieceObj = PieceFactory._instance.CreateAbilityPiece(
                PieceType.Pawn, "s", -1, -1, PieceColor.White, Team.Hero, hero, Game._instance.AllAbilities[18].Clone());
        hero.inventoryPieces.Add(pieceObj);
        Chessman piece = pieceObj.GetComponent<Chessman>();
        piece.LevelUp(Game._instance.level+2);
    }

    public void LaunchEncounterDialogue(EncounterType encounterType){
        switch(encounterType){
            case EncounterType.EscapedPawn:
                StartDialogue(Resources.Load<Dialogue>("Objects/Dialogues/EscapedPeasant"));
                break;
            case EncounterType.Blacksmith:
                StartDialogue(Resources.Load<Dialogue>("Objects/Dialogues/Blacksmith"));
                break;
        }

    }
    public IEnumerator AdjustFontSizes(List<TMP_Text> optionTexts)
    {
        float minFontSize = Mathf.Infinity;
        yield return null; 

        foreach (var text in optionTexts)
        {
            
            text.ForceMeshUpdate(); // Ensure TMP updates the text size
            minFontSize = Mathf.Min(minFontSize, text.fontSize);
        }

        // Apply the smallest font size to all options
        foreach (var text in optionTexts)
        {
            text.enableAutoSizing = false; // Disable auto-sizing after setting
            text.fontSize = minFontSize;
        }
    }


}
