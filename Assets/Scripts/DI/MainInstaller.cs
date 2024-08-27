using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private SubmitController _submitController;
    [SerializeField] private SC_FPSController _fpsController;
    [SerializeField] private PlayerParams _params;
    [SerializeField] private EnemySpawnController _enemySpawnController;
    [SerializeField] private OutcomeManager _outcomeManager;

    public override void InstallBindings()
    {
        Container.BindInstance(_mainCamera).WithId("mainCamera");
        Bind(_submitController);
        Bind(_fpsController);
        Bind(_params);
        Bind(_enemySpawnController);
        Bind(_outcomeManager);
    }

    private void Bind<T>(T instance)
    {
        Container.BindInterfacesAndSelfTo<T>().FromInstance(instance).AsSingle().NonLazy();
    }
}
