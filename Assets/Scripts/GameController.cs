using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public enum GameState { Menu, Playing };
    public GameState gameState;

    public GameObject menuObject;
    public GameObject playObject;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeToPlayState()
    {
        menuObject.SetActive(false);
        gameState = GameState.Playing;
        playObject.SetActive(true);

    }
}
