using UnityEngine;

namespace CodeBase.Infrastructure.AssetManagement
{
  public class AssetProvider : IAssetProvider
  {
    public GameObject Instantiate(string address) => 
      Resources.Load<GameObject>(address);

    public GameObject Instantiate(string address, Vector3 spawnPoint)
    {
      GameObject go = Resources.Load<GameObject>(address);
      go.transform.position = spawnPoint;
      return go;
    }
  }
}