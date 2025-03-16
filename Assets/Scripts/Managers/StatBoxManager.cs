using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class StatBoxManager : MonoBehaviour
{
    
    public static StatBoxManager _instance;
    public List<StatBox> statBoxes;

    public StatBox EnemyStatBox;
    public bool lockView;
    public bool enemyLockView;
    public Chessman enemyLockedPiece;
    public Chessman lockedPiece;
    // Start is called before the first frame update
    void Awake()
    {
        if(_instance !=null && _instance !=this){
            Destroy(this.gameObject);
        }
        else{
            _instance=this;
        }
    }

    void Start(){
        Cursor.visible=true;
        foreach (var statBox in statBoxes)
        {
            statBox.gameObject.SetActive(false);
        }
    }

    public void LockView(Chessman piece){
        lockView=true;
        piece.flames.Play();
        lockedPiece=piece;
    }
    public void UnlockView(){
        lockView=false;
        if (lockedPiece){
            lockedPiece.flames.Stop();
            lockedPiece=null;
        }
    }
    public void SetAndShowStats(Chessman piece){
        if(!lockView)
        {
            foreach (StatBox statBox in statBoxes)
            {
                statBox.SetStats(piece);
            }
        }

    }

    public void SetAndShowEnemyStats(Chessman piece){
        if(!enemyLockView)
        {
            EnemyStatBox.SetStats(piece);
        }
        
    }

    public void LockEnemyView(Chessman piece){
        enemyLockView=true;
        piece.highlightedParticles.Play();
        enemyLockedPiece=piece;
    }
    public void UnlockEnemyView(){
        enemyLockView=false;
        if (enemyLockedPiece){
            enemyLockedPiece.highlightedParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            enemyLockedPiece=null;
        }
    }

    public void HideStats(){
        foreach (var statBox in statBoxes)
        {
            statBox.HideStats();
        }
    }
}
