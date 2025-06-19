using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public interface IInteractable
{
    void OnClick(Board board);
    void OnRightClick(Board board);
    void OnHover(Board board);
    void OnHoverExit(Board board);
}