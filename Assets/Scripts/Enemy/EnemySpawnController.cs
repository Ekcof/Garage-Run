using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.AI.Navigation;
using UnityEngine;
using Zenject;

/// <summary>
/// Responsible for spawning and destroying of enemies
/// </summary>
public class EnemySpawnController : MonoBehaviour
{
    [Inject] private DiContainer _container;
    [Inject] private PlayerParams _playerParams;
    [SerializeField] private NavMeshSurface currentSurface;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private BotBehaviour _enemyPrefab;

    private List<BotBehaviour> _openList = new();
    private List<BotBehaviour> _closeList = new();

    private void Awake()
    {
        _playerParams.CurrentLevel.Subscribe(OnCurrentLevel).AddTo(this);
    }

    private void OnCurrentLevel(int level)
    {
        RemoveAllEnemies();
        SpawnEnemiesOnLevel(level, _spawnPoints);
    }

    private void OnDestroy()
    {
        // Cleanup or unsubscribe if necessary
    }

    private void RemoveAllEnemies()
    {
        if (_openList.Count > 0)
        {
            for (int i = 0; i < _openList.Count; i++)
            {
                var enemy = _openList[i];
                Pool(enemy);
            }
            _openList.Clear();
        }
        else
        {
            Debug.Log("No enemies to remove!");
        }
    }

    private void Pool(BotBehaviour bot)
    {
        bot.OnPool();
        _closeList.Add(bot);
    }

    private BotBehaviour Get()
    {
        BotBehaviour bot;

        if (_closeList.Count > 0)
        {
            bot = _closeList[_closeList.Count - 1];
            _closeList.RemoveAt(_closeList.Count - 1);
        }
        else
        {
            bot = _container.InstantiatePrefabForComponent<BotBehaviour>(_enemyPrefab);
        }
        bot.OnGet();
        _openList.Add(bot);

        return bot;
    }

    public void SpawnEnemiesOnLevel(int level, List<Transform> spawnPoints)
    {
        var availableSpawnPoints = new List<Transform>(spawnPoints);
        int enemiesToSpawn = Mathf.Min(level + 1, availableSpawnPoints.Count);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            BotBehaviour enemy = Get();

            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform selectedSpawnPoint = availableSpawnPoints[randomIndex];

            Vector3 finalPosition = selectedSpawnPoint.position;

            enemy.transform.position = finalPosition;
            availableSpawnPoints.RemoveAt(randomIndex);
        }

        // Bake the NavMesh surface to update it for the new enemy positions
        //currentSurface.BuildNavMeshAsync().Forget();
    }

}
