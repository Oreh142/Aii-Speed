using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private bool isPaused;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameObject _pausePanel;

    public void OnPauseButtonClick()
    {
        Time.timeScale = 0;
        _gameManager.ChangeSoundStatus(false);
        isPaused = true;
        _pausePanel.SetActive(true);
    }

    public void ClosePausePanel()
    {
        Time.timeScale = 1f;
        _gameManager.ChangeSoundStatus(true);
        _pausePanel.SetActive(false);
        isPaused = false;
    }

    public void OnToMenuButtonClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public bool GetPausedStatus()
    {
        return isPaused;
    }
}