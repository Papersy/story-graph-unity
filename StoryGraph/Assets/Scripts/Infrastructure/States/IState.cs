namespace CodeBase.Infrastructure.States
{
  public interface IState : IExitableState
  {
    void Enter();
  }

  public interface IPayloadedState<TPayload> : IExitableState
  {
    void Enter(TPayload sceneName);
  }

  public interface IExitableState
  {
    void Exit();
  }
}