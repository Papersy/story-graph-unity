using CodeBase.Infrastructure.Services;
using UI;

namespace Infrastructure.Services
{
    public interface IUIService : IService
    {
        HUDContainer HudContainer { get; }
    }
}