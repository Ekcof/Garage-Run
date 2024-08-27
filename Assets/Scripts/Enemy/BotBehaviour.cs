using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

/// <summary>
/// Responsible for movement of enemy
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class BotBehaviour : MonoBehaviour
{
    [Inject] private SC_FPSController _player;
    [Inject] private OutcomeManager _outcomeManager;
    [SerializeField] private float deletionDelay = 5f;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;

    private bool isMoving;
    private CancellationTokenSource _cts = new();

    private void Start()
    {
        _ = StartMovement(_cts.Token);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            _outcomeManager.OnFail();
    }

    public void OnGet()
    {
        _cts?.CancelAndDispose();
        _cts = new();
        _agent.enabled = true;
        _ = StartMovement(_cts.Token);
    }

    public void OnPool()
    {
        _cts?.CancelAndDispose();
    }

    private async UniTask StartMovement(CancellationToken token)
    {
        if (gameObject == null)
            return;
        isMoving = true;
        if (_animator != null)
            _animator.SetBool("Run", true);

        while (isMoving && !token.IsCancellationRequested)
        {
            MoveToPlayerDestinationAsync();
            try
            {
                await Task.Delay(200, token);
            }
            catch
            {
                return;
            }
        }
    }

    private void MoveToPlayerDestinationAsync()
    {
        if (_player.transform == null)
        {
            Debug.Log("Failed to find _playerTransform.position");
            return;
        }
        Vector3 destination = _player.transform.position;
        _agent.SetDestination(destination);
    }

    public void StopMovement()
    {
        isMoving = false;
    }

    private void OnDestroy()
    {
        _cts?.CancelAndDispose();
    }
}