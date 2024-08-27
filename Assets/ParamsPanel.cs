using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ParamsPanel : MonoBehaviour
{
    [Inject] private PlayerParams _playerParams;
    [SerializeField] private Slider _scaleSlider;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Text _pointsText;

    private void Awake()
    {
        _scaleSlider.minValue = _playerParams.ShrinkLimit;
        _staminaSlider.maxValue = _playerParams.StaminaLimit;

        _playerParams.Scale.Subscribe(OnChangeScale).AddTo(this);
        _playerParams.Stamina.Subscribe(OnChangeStamina).AddTo(this);
        _playerParams.CurrentLevel.Subscribe(OnLevelUpdate).AddTo(this);
    }

    private void OnLevelUpdate(int level)
    {
        _pointsText.text = $"{_playerParams.CurrentLevel.Value}/5";
    }


    private void OnChangeScale(float obj)
    {
        _scaleSlider.value = obj;
    }

    private void OnChangeStamina(float obj)
    {
        _staminaSlider.value = obj;
    }
}
