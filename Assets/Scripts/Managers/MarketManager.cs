using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MarketManager : MonoBehaviour
{
    public List<GameObject> myCapturedPieces = new List<GameObject>();
    //public List<GameObject> myPieces= new List<GameObject>();
    public List<GameObject> opponentCapturedPieces = new List<GameObject>();
    //public List<GameObject> opponentPieces = new List<GameObject>();
    public List<Chessman> selectedPieces = new List<Chessman>();
    public int totalCost;
    private Player hero;
    public PieceColor selectedColor = PieceColor.None;
    [SerializeField] GameObject dropInSprite;
    private Dictionary<Chessman, GameObject> sprites = new Dictionary<Chessman, GameObject>();
    public bool killingField;
    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void OpenMarket(Board board)
    {
        hero = board.Hero;
        totalCost = 0;
        //Debug.Log("Opening market");
        gameObject.SetActive(true);
        myCapturedPieces = hero.capturedPieces;
        opponentCapturedPieces = board.Opponent.capturedPieces;


        //board.CurrentMatch.black.pieces.AddRange(myCapturedPieces);
        foreach (GameObject piece in hero.pieces) { piece.SetActive(false); if(killingField){ piece.GetComponent<Chessman>().blood *= 2; }}
        foreach (GameObject piece in board.Opponent.pieces) { piece.SetActive(false); if(killingField){ piece.GetComponent<Chessman>().blood *= 2; }}

        if (myCapturedPieces.Count > 0)
            foreach (GameObject piece in myCapturedPieces)
            {
                piece.SetActive(true);
            }
        var decimatedPieces = new List<GameObject>();
        if (opponentCapturedPieces.Count > 0)
            foreach (GameObject piece in opponentCapturedPieces)
            {
                piece.SetActive(true);
                Chessman chessman = piece.GetComponent<Chessman>();
                if (chessman.owner == hero)
                {
                    if (chessman.abilities.Count > chessman.diplomacy)
                    {
                        Debug.Log("checking diplomacy for " + piece.name);
                        int survive = Random.Range(1, 10);
                        Debug.Log("Rolled " + survive + " and diplomacy is " + chessman.diplomacy);
                        if (survive <= ((chessman.abilities.Count - chessman.diplomacy) * 2))
                        {
                            Debug.Log("decimated from diplomacy check");
                            decimatedPieces.Add(piece);
                            chessman.DestroyPiece();
                        }
                    }
                }


            }
        opponentCapturedPieces.RemoveAll(x => decimatedPieces.Contains(x));
    }

    public void CloseMarket(Board board)
    {
        selectedColor = PieceColor.None;
        killingField = false;
        opponentCapturedPieces = board.Opponent.capturedPieces;
        if (opponentCapturedPieces.Count > 0)
            foreach (GameObject piece in opponentCapturedPieces)
            {
                if (piece != null)
                {
                    Chessman cm = piece.GetComponent<Chessman>();
                    cm.DestroyPiece();
                    hero.AbandonedPieces++;
                }

            }
        if (myCapturedPieces.Count > 0)
            foreach (GameObject piece in myCapturedPieces)
            {
                if (piece != null)
                    piece.GetComponent<Chessman>().DestroyPiece();
            }
        myCapturedPieces.Clear();
        opponentCapturedPieces.Clear();
        selectedPieces.Clear();
        gameObject.SetActive(false);
    }

    public void ReleasePieces()
    {
        foreach (Chessman item in selectedPieces)
        {
            //hero.playerCoins += item.releaseCost;
            item.highlightedParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            item.gameObject.SetActive(false);
            item.DestroyPiece();
        }
        selectedPieces.Clear();
        ClearPanel();
    }

    public void ReturnMyPieces()
    {
        foreach (Chessman piece in selectedPieces)
        {
            if (piece.owner != hero)
                break;
            
            hero.playerCoins -= piece.releaseCost;
            piece.highlightedParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            piece.gameObject.SetActive(false);
            hero.pieces.Add(piece.gameObject);
            opponentCapturedPieces.Remove(piece.gameObject);
            myCapturedPieces.Remove(piece.gameObject);
        }
        totalCost = 0;
        selectedPieces.Clear();
        ClearPanel();
    }

    public void KillPieces()
    {

        foreach (Chessman item in selectedPieces)
        {
            hero.playerBlood += item.blood;
            myCapturedPieces.Remove(item.gameObject);
            opponentCapturedPieces.Remove(item.gameObject);
            item.gameObject.SetActive(false);
            item.DestroyPiece();
        }
        selectedPieces.Clear();
        ClearPanel();
    }

    public void AddPieceToSelection(Chessman piece)
    {
        if (selectedPieces.Count == 0 || selectedColor == PieceColor.None)
            selectedColor = piece.color;
        if (totalCost + piece.releaseCost > hero.playerCoins && piece.color == PieceColor.White && !selectedPieces.Contains(piece))
            return;
        if (piece.color != selectedColor)
            return;

        if (selectedPieces.Contains(piece))
        {
            selectedPieces.Remove(piece);
            piece.highlightedParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            DropOut(piece);
            if (piece.color == PieceColor.White)
                totalCost -= piece.releaseCost;
        }
        else
        {
            selectedPieces.Add(piece);
            piece.highlightedParticles.Play();
            DropIn(piece);
            SoundManager.Instance.PlaySoundFXClip(SoundManager.Instance.dropIn, 1f, Settings.Instance.SfxVolume);
            if (piece.color == PieceColor.White)
                totalCost += piece.releaseCost;
        }
    }

    public void DropIn(Chessman piece)
    {
        GameObject sprite = piece.droppingSprite;
        GameObject newSprite = Instantiate(dropInSprite, this.transform);
        newSprite.transform.localPosition = new Vector3(55 - (100 * (selectedPieces.Count - 1)), 0, 0);
        newSprite.GetComponent<SpriteRenderer>().sprite = sprite.GetComponent<SpriteRenderer>().sprite;
        newSprite.GetComponent<Animator>().runtimeAnimatorController = sprite.GetComponent<Animator>().runtimeAnimatorController;

        if (piece.color == PieceColor.White)
            newSprite.GetComponent<SpriteRenderer>().color = Color.white;
        else
            newSprite.GetComponent<SpriteRenderer>().color = new Color32(69, 69, 69, 255);

        sprites.Add(piece, newSprite);






    }
    public void DropOut(Chessman piece)
    {
        Destroy(sprites[piece]);
        sprites.Remove(piece);
    }

    public void ClearPanel()
    {
        foreach (GameObject sprite in sprites.Values)
        {
            Destroy(sprite);
        }
        sprites.Clear();
    }
    


}
