using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneStarter : MonoBehaviour
{
    public Gamemanager gamemanager;
    void Start()
    {
        gamemanager.StartNormalGame();
    }
}
