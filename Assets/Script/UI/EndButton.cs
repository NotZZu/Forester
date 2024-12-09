using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndButton : MonoBehaviour
{
    [SerializeField] Text _text;
    [SerializeField] Button _endButton;
    void Awake()
    {
        _endButton.onClick.AddListener(EndButtonClick);
        float passedTime = PlayerPrefs.GetFloat("passedTime");

        _text.text += passedTime.ToString();
    }
    void EndButtonClick()
    {
        Debug.Log("End Button Clicked"); // ��ư Ŭ�� ���� Ȯ��
        SceneManager.LoadScene("StartScene");
    }
}
