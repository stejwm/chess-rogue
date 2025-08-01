using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    public IInteractable CurrentInteractable { get; private set; }
    [SerializeField] private Board board;

    public void SetWorldPosition(Vector3 worldPos)
    {
        transform.position = new(worldPos.x, worldPos.y, 0);
        UpdateTile();
    }

    private void UpdateTile()
    {
        Vector2 position = transform.position;
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(position));
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            Debug.Log($"Hit {hit.collider.name}");
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (CurrentInteractable != interactable)
            {
                CurrentInteractable?.OnHoverExit(board);
                CurrentInteractable = interactable;
                CurrentInteractable?.OnHover(board);
            }

        }
        else
        {
            CurrentInteractable?.OnHoverExit(board);
            CurrentInteractable = null;
        }
        CheckForUIButtonUnderSelector();
    }

    public void CheckForUIButtonUnderSelector()
    {
        // Find all active canvases and sort by sortingOrder descending
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        List<Canvas> sortedCanvases = new List<Canvas>();
        foreach (var canvas in canvases)
        {
            if (canvas.isActiveAndEnabled && canvas.gameObject.activeInHierarchy)
            {
                sortedCanvases.Add(canvas);
            }
        }
        sortedCanvases.Sort((a, b) => b.sortingOrder.CompareTo(a.sortingOrder));

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Camera.main.WorldToScreenPoint(transform.position);

        foreach (var canvas in sortedCanvases)
        {
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
                continue;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                Button button = result.gameObject.GetComponent<Button>();
                if (button != null && button.interactable)
                {
                    EventSystem.current.SetSelectedGameObject(button.gameObject);
                    return;
                }
            }
        }
        // If no button found in any canvas
        EventSystem.current.SetSelectedGameObject(null);
    }





    public void SnapToNearestTile(Board board)
    {
        // Find nearest tile based on selector's current position
        float minDist = float.MaxValue;
        Tile nearestTile = null;
        Vector3 selectorPos = transform.position;

        foreach (var tile in board.tiles)
        {
            Vector3 tilePos = tile.transform.position;
            float dist = Vector3.Distance(selectorPos, tilePos);
            if (dist < minDist)
            {
                minDist = dist;
                nearestTile = tile;
            }
        }

        if (nearestTile != null)
            SetWorldPosition(nearestTile.transform.position);
    }


    public void MoveToAdjacentTile(Board board, Vector2Int direction)
    {
        // Snap to nearest tile if not already on one
        SnapToNearestTile(board);

        // Find current tile indices
        Vector3 selectorPos = transform.position;
        int closestX = 0, closestY = 0;
        float minDist = float.MaxValue;
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                Vector3 tilePos = board.tiles[x, y].transform.position;
                float dist = Vector3.Distance(selectorPos, tilePos);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestX = x;
                    closestY = y;
                }
            }
        }

        // Calculate new position
        int newX = Mathf.Clamp(closestX + direction.x, 0, board.Width - 1);
        int newY = Mathf.Clamp(closestY + direction.y, 0, board.Height - 1);

        SetWorldPosition(board.tiles[newX, newY].transform.position);
    }
}