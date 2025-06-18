using UnityEngine;

public class Selector : MonoBehaviour
{
    public IInteractable CurrentTile { get; private set; }

    public void SetWorldPosition(Vector3 worldPos)
    {
        transform.position = worldPos;
        UpdateTile();
    }

    private void UpdateTile()
    {
        Vector2 position = transform.position;
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(position));
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        { 
            CurrentTile = hit.collider.GetComponent<IInteractable>();
        }
        else
        {
            CurrentTile = null;
        }
    }

    public void MoveByOffset(Vector2Int offset)
    {
        // Move selector by grid offset if you're navigating with keyboard/controller
        Vector3 newPosition = transform.position + new Vector3(offset.x, 0, offset.y); // assuming flat XZ board
        SetWorldPosition(newPosition);
    }
}