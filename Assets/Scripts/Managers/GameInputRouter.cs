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
    private Vector2 joystickInput;

    private void Awake()
    {
        input = new NewControls();


        input.ChessMatchInput.Click.performed += ctx => OnClick(ctx);
        input.ChessMatchInput.RightClick.performed += ctx => OnRightClick(ctx);
        input.ChessMatchInput.Point.performed += ctx => OnMouseMove(ctx);
        input.ChessMatchInput.Move.performed += ctx => OnJoystickMove(ctx);
        input.ChessMatchInput.Move.canceled += ctx => joystickInput = Vector2.zero;
        input.ChessMatchInput.DPadMove.performed += ctx => OnDPadMove(ctx);
    }
    void Update()
    {
        // Deadzone to prevent drift
        if (joystickInput.magnitude > 0.1f)
        {
            
            Vector3 direction = new Vector3(joystickInput.x, joystickInput.y, 0);
            Vector3 targetPosition = selector.transform.position + direction * Settings.Instance.JoystickSpeed * Time.deltaTime;

            // Smooth movement
            selector.SetWorldPosition(Vector3.Lerp(selector.transform.position, targetPosition, 0.2f));
        }
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
    }
    private void OnJoystickMove(InputAction.CallbackContext context)
    {
        joystickInput = context.ReadValue<Vector2>();
    }
    private void OnDPadMove(InputAction.CallbackContext context)
    {
        Vector2 screenPosition = input.ChessMatchInput.DPadMove.ReadValue<Vector2>();
        Vector2Int snap = Vector2Int.RoundToInt(screenPosition);
        selector.MoveToAdjacentTile(board, snap);
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        /* Vector2 screenPosition = input.ChessMatchInput.Point.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        if (hit.collider != null)
        {
            Debug.Log("Clicked on: "+hit.collider.name);
        } */
        var tile = selector.CurrentInteractable;
        if (tile != null)
            tile.OnClick(board);
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        var tile = selector.CurrentInteractable;
        if (tile != null)
            tile.OnRightClick(board);
    }

    private void OnMoveCursor(Vector2 direction)
    {
        //Debug.Log("Cursor moved: " + direction);
    }
}