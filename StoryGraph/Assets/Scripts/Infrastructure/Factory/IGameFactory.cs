using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.Infrastructure.Factory
{
  public interface IGameFactory : IService
  {
    void Cleanup();
    GameObject Instantiate(string prefabPath);
    GameObject Instantiate (string prefabPath, Vector3 position);
  }
}