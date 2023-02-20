using CodeBase.Infrastructure.AssetManagement;
using UnityEngine;

namespace Infrastructure.Factory
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssetProvider _assets;

        public GameFactory(IAssetProvider assetsProvider) =>
            _assets = assetsProvider;

        public void Cleanup()
        {
        }

        public GameObject Instantiate(string prefabPath)
        {
            GameObject gameObject = Object.Instantiate(_assets.Instantiate(prefabPath));

            return gameObject;
        }

        public GameObject Instantiate(string prefabPath, Vector3 position) =>
            _assets.Instantiate(prefabPath, position);
    }
}