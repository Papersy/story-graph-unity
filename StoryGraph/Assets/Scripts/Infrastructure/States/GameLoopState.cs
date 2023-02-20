using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.States;
using Infrastructure.Services;

namespace Infrastructure.States
{
    public class GameLoopState : IState
    {
        public GameLoopState(GameStateMachine gameStateMachine)
        {
        }

        public void Enter()
        {
            AllServices.Container.Single<IUIService>().HudContainer.ShowUI();
        }

        public void Exit()
        {
            AllServices.Container.Single<IUIService>().HudContainer.HideUI();
        }
    }
}