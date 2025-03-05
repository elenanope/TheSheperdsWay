using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) Debug.Log("GameManager is missing!");
            return instance;
        }
    }
    public float totalLife = 1;
    public enum GameState { gameOver, gameStarted, gamePaused, gameCompleted }
    public GameState currentGameState = GameState.gameStarted;


    private void Awake()
    {
        instance = this; ;
    }

    private void Update()
    {
        //Condición para perder la partida
        if (totalLife <= 0)
        {
            totalLife = 0;
            currentGameState = GameState.gameOver;
            Debug.Log("Has perdido!");
        }
    }

}
