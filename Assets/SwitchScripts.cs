using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScripts : MonoBehaviour
{
    public AnimationCode animationCode;
    public WebSocketClient webSocketClient;
    void Update()
    {
        // if space key is pressed then switch between the two scripts
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(animationCode.isActivated)
            {
                animationCode.isActivated = false;
                webSocketClient.isActivated = true;
            }
            else
            {
                animationCode.isActivated = true;
                webSocketClient.isActivated = false;
            }
        }
    }
}
