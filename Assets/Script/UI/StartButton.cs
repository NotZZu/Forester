using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField] Button _startButton;
    void Awake()
    {
        _startButton.onClick.AddListener(OnStartButtonClick);
    }
    void OnStartButtonClick()
    {
        SceneManager.LoadScene("MainScene");
    }
}
