using System.Collections;
using System.Collections.Generic;
using CI.QuickSave;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static PauseMenuManager _instance;
    public AudioSource audioSource;
    void Awake()
    {
        
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    // Update is called once per frame
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenMenu(){
        gameObject.SetActive(true);
        Time.timeScale=0;
    }

    public void CloseMenu(){
        Time.timeScale=1;
        gameObject.SetActive(false);
    }

    public void StartGame(){
        SceneManager.LoadScene(1);
    }

    public void SaveGame(){

        PlayerData playerData = new PlayerData
        {
            coins = Game._instance.hero.playerCoins,
            blood = Game._instance.hero.playerBlood,
            pieces = new List<PieceData>()
        };

        foreach (var pieceObj in Game._instance.hero.pieces)
        {
            Chessman piece = pieceObj.GetComponent<Chessman>();
            PieceData pieceData = new PieceData
            {
                pieceType = piece.type.ToString(),
                attack = piece.attack,
                defense = piece.defense,
                support = piece.support,
                posX = piece.xBoard,
                posY = piece.yBoard,
                abilities = new List<AbilityData>()
            };

            foreach (Ability ability in piece.abilities)  // Assuming `abilities` is a List<Ability>
            {
                pieceData.abilities.Add(new AbilityData
                {
                    abilityName = ability.abilityName,  // Use a unique identifier for each ability
                    abilityDescription = ability.description    // Store any relevant data
                });
            }
            playerData.pieces.Add(pieceData);
        }
        
        var writer = QuickSaveWriter.Create("Game");
            writer.Write("Player", playerData);
            writer.Write("State", Game._instance.state);
            writer.Write("Level", Game._instance.level);
            //writer.Write("MapNodes", MapManager._instance.mapNodes);
            writer.Commit();
    }

    public void QuitGame(){
        Time.timeScale=1;
        SceneManager.LoadScene(0);
    }
}
