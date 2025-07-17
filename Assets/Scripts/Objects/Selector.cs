using UnityEngine;

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
    }

    public void MoveByOffset(Vector2Int offset)
    {
        // Move selector by grid offset if you're navigating with keyboard/controller
        Vector3 newPosition = transform.position + new Vector3(offset.x, 0, offset.y); // assuming flat XZ board
        SetWorldPosition(newPosition);
    }
}