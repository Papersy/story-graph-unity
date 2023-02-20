using Infrastructure.Services;

namespace UI
{
    public class UIService : IUIService
    {
        public HUDContainer HudContainer { get; }

        public UIService(HUDContainer hudContainer)
        {
            HudContainer = hudContainer;
        }
    }
}