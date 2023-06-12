namespace Infrastructure.Services
{
    public class GameService : IGameService
    {
        private GameController _gameController = new GameController();

        public void GenerateMap()
        {
            _gameController.DeserializeFile();
        }


        public GameController GetGameController() => _gameController;
    }
}