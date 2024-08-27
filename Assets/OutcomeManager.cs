using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class OutcomeManager : MonoBehaviour
{
    [Inject] private PlayerParams _playerParams;
    [SerializeField] private GameObject _outComeMenu;
    [SerializeField] private Image _panelImage;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Text _panelText;

    private void Awake()
    {
        _restartButton.onClick.RemoveAllListeners();
        _restartButton.onClick.AddListener(RestartScene);
        _playerParams.CurrentLevel.Subscribe(OnLevelUp).AddTo(this);
    }

    private void OnDestroy()
    {
        _restartButton.onClick?.RemoveAllListeners();
    }

    private void OnLevelUp(int level)
    {
        if (level > 4)
        {
            OnWin();
        }
    }

    public void OnFail()
    {
        ShowWindow(new Color(1, 0, 0, 0.5f), "FAILED");
    }

    public void OnWin()
    {
        ShowWindow(new Color(0, 0, 1, 0.5f), "SUCCESS");
    }

    private void ShowWindow(Color color, string text)
    {
        _outComeMenu.SetActive(true);
        _panelImage.color = color;
        _panelText.text = text;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}
