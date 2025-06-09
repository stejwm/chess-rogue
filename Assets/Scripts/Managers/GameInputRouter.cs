using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public interface IGameInputReceiver
{
    void HandleClick(GameObject clickedObject);
}
public class GameInputRouter : MonoBehaviour
{
    private IGameInputReceiver currentReceiver;

    public void SetReceiver(IGameInputReceiver receiver)
    {
        currentReceiver = receiver;
    }

    public void OnClick(GameObject clicked)
    {
        currentReceiver?.HandleClick(clicked);
    }
}