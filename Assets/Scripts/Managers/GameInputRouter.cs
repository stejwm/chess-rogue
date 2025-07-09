using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputRouter : MonoBehaviour
{
    private NewControls input;
    [SerializeField] private Board board;
    [SerializeField] private Selector selector;
    private IInteractable target;

    private void Awake()
    {
        input = new NewControls();


        input.ChessMatchInput.Click.performed += ctx => OnClick(ctx);
        input.ChessMatchInput.RightClick.performed += ctx => OnRightClick(ctx);
        input.ChessMatchInput.Point.performed += ctx => OnMouseMove(ctx);
    }

    private void OnEnable()
    {
        input.ChessMatchInput.Enable();
    }

    private void OnDisable()
    {
        input.ChessMatchInput.Disable();
    }
    private void OnMouseMove(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = input.ChessMatchInput.Point.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        selector.SetWorldPosition(Camera.main.ScreenToWorldPoint(screenPosition));

        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        if (hit.collider != null)
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (target != interactable)
            {
                target?.OnHoverExit(board);
                target = interactable;
                interactable?.OnHover(board);
            }

        }
        else
        {
            target?.OnHoverExit(board);
        }
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = input.ChessMatchInput.Point.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        if (hit.collider != null)
        {
            Debug.Log("Clicked on: "+hit.collider.name);
        }
        var tile = selector.CurrentTile;
        if (tile != null)
            tile.OnClick(board);
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        var tile = selector.CurrentTile;
        if (tile != null)
            tile.OnRightClick(board);
    }

    private void OnMoveCursor(Vector2 direction)
    {
        Debug.Log("Cursor moved: " + direction);
    }
}