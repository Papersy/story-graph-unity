namespace Infrastructure.Services
{
    public class GameService : IGameService
    {
        private GameController _gameController = new GameController();

        public void GenerateMap()
        {
            
            _gameController.DeserializeFile();
            
            // HttpClientController httpClientController = new HttpClientController();
            // List<World> lSide;

            // string json = await httpClientController.GetNewWorld();
            // lSide = DeserializeFile(json);
            // DeserializeFile();

        }


        public GameController GetGameController() => _gameController;
    }
}