using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSceneStarter : MonoBehaviour
{
    public Gamemanager gamemanager;
    void Start()
    {
        gamemanager.StartTutorialGame();
    }
}
