using CodeBase.Infrastructure.Services;
using Infrastructure.Services;
using Newtonsoft.Json.Linq;

namespace Player
{
    public class PlayerStats
    {
        public static int Damage = 20;
        public static int Health = 0;
        public static JToken NpcBattleInfo = null;

        // public static void UpdateHealth(int count)
        // {
        //     Health += count;
        //
        //     if (Health <= 0)
        //     {
        //         var playerId = AllServices.Container.Single<IGameService>().GetGameController().GetMainPlayerId();
        //
        //         AllServices.Container.Single<IUIService>().HudContainer.GameCanvas.DiePanel.SetActive(true);
        //         AllServices.Container.Single<IGameService>().GetGameController().HeroDeath();
        //     }
        // }
    }
}