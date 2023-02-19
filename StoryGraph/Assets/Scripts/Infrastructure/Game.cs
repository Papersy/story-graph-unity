using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.States;


namespace CodeBase.Infrastructure
{
  public class Game
  {
    public readonly GameStateMachine StateMachine;

    public Game(ICoroutineRunner coroutineRunner)
    {
      StateMachine = new GameStateMachine(new SceneLoader(coroutineRunner), AllServices.Container);
    }
  }
}