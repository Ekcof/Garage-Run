using UnityEngine;
using Zenject;

public class ObtainZone : MonoBehaviour
{
    [Inject] private PlayerParams _playerParams;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Destroy(other.gameObject);
            _playerParams.AddLevel();
        }
    }
}
