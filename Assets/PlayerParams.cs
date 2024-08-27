using System.Threading;
using UniRx;
using UnityEngine;

public class PlayerParams : MonoBehaviour
{
    public const float SHRINK_LIMIT = 0.3f;
    public const float ENLARGE_LIMIT = 1.5f;
    public const float SCALING_TIME = 0.5f;

    public const float STAMINA_LIMIT = 6f;
    public const float STAMINA_RESTORE_TIME = 9f;

    private bool _isRestoringStamina;
    private float _previousStamina;

    private ReactiveProperty<float> _scale = new(1);
    private ReactiveProperty<float> _stamina = new(STAMINA_LIMIT);
    private ReactiveProperty<int> _currentLevel = new();

    private CancellationTokenSource _cts = new();
    public float ShrinkLimit => SHRINK_LIMIT;
    public float StaminaLimit => STAMINA_LIMIT;
    public IReadOnlyReactiveProperty<float> Scale => _scale;
    public IReadOnlyReactiveProperty<float> Stamina => _stamina;
    public IReadOnlyReactiveProperty<int> CurrentLevel => _currentLevel;
    public bool IsShrinked => _scale.Value < 0.95f;
    public bool CanLoseStamina => _stamina.Value > STAMINA_LIMIT / 2 || !_isRestoringStamina;

    private void Awake()
    {
        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                if (_stamina.Value >= _previousStamina && _stamina.Value != STAMINA_LIMIT)
                {
                    RestoreStamina();
                }

                _previousStamina = _stamina.Value;
            })
            .AddTo(this);
    }

    private void RestoreStamina()
    {
        _isRestoringStamina = true;
        _stamina.Value += (STAMINA_LIMIT / STAMINA_RESTORE_TIME) * Time.deltaTime;
        if (_stamina.Value >= STAMINA_LIMIT)
        {
            _stamina.Value = STAMINA_LIMIT;
            _isRestoringStamina = false;
        }
    }


    public void OnLoseStamina()
    {
        _isRestoringStamina = false;
        _stamina.Value = Mathf.Max(_stamina.Value - Time.deltaTime, 0f);
    }

    public void OnShrinking()
    {
        if (_scale.Value > SHRINK_LIMIT)
        {
            float scaleDecrementPerFrame = (1f / SCALING_TIME) * (Time.deltaTime * (1f - SHRINK_LIMIT));
            _scale.Value = Mathf.Max(_scale.Value - scaleDecrementPerFrame, SHRINK_LIMIT);
        }
    }

    public void OnEnlarging()
    {
        if (_scale.Value < ENLARGE_LIMIT)
        {
            float scaleIncrementPerFrame = (1f / SCALING_TIME) * (Time.deltaTime * (ENLARGE_LIMIT - 1f));
            _scale.Value = Mathf.Min(_scale.Value + scaleIncrementPerFrame, ENLARGE_LIMIT);
        }
    }

    public void OnNormalizingScale()
    {
        if (_scale.Value != 1f)
        {
            float targetScale = 1f;
            float scaleAdjustmentPerFrame = (1f / SCALING_TIME) * (Time.deltaTime * Mathf.Abs(_scale.Value - targetScale));
            if (_scale.Value > 1f)
            {
                _scale.Value = Mathf.Max(_scale.Value - scaleAdjustmentPerFrame, targetScale);
            }
            else if (_scale.Value < 1f)
            {
                _scale.Value = Mathf.Min(_scale.Value + scaleAdjustmentPerFrame, targetScale);
            }
        }
    }

    public void AddLevel()
    {
        _currentLevel.Value += 1;
    }
}
