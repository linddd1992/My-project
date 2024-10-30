using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassView : MonoBehaviour
{
    public void OnPassButton(){
        UIManager.Instance.CloseWindow("PassView");
        GameManager.Instance.LoadMainMenu();
    }
}
