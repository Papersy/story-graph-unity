using CodeBase.Infrastructure;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.States;
using CodeBase.StaticData;
using Infrastructure.AssetManagement;
using Infrastructure.Factory;
using Infrastructure.Services;
using UI;
using UnityEngine;

namespace Infrastructure.States
{
    public class BootstrapState : IState
    {
        private const string Initial = "Boot";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly AllServices _services;

        public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader, AllServices services)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _services = services;

            RegisterServices();
        }

        public void Enter() =>
            _sceneLoader.Load(Initial, onLoaded: EnterLoadLevel);

        public void Exit()
        {
        }

        private void EnterLoadLevel() =>
            _stateMachine.Enter<LoadLevelState, string>("Game");

        private void RegisterServices()
        {
            _services.RegisterSingle<IStaticDataService>(new StaticDataService());
            _services.RegisterSingle<IGameStateMachine>(_stateMachine);
            _services.RegisterSingle<IAssetProvider>(new AssetProvider());
            _services.RegisterSingle<IGameFactory>(new GameFactory(
                _services.Single<IAssetProvider>()));
            _services.RegisterSingle<IGameService>(new GameService());

            GameObject hudContainerGO = _services.Single<IGameFactory>().Instantiate(ConstantsData.HUDAddress);
            HUDContainer hudContainer = hudContainerGO.GetComponent<HUDContainer>();

            _services.RegisterSingle<IUIService>(new UIService(hudContainer));
        }
    }
}