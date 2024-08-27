using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class PlayerScaleController : MonoBehaviour
{
    [Inject] private PlayerParams _playerParams;

    private void Awake()
    {
        _playerParams.Scale.Subscribe(OnChangeScale).AddTo(this);
    }

    private void OnChangeScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
