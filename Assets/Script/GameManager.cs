using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerAction _player;
    public static GameManager _instance;
    public void Awake()
    {
        _instance = this;
    }
}
