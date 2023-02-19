using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeBase.StaticData
{
  public class StaticDataService : IStaticDataService
  {
    private const string StaticDataLevelsPath = "StaticData/Levels";
    private readonly Dictionary<string, LevelStaticData> _levels;

    public StaticDataService()
    {
      _levels = Resources
        .LoadAll<LevelStaticData>(StaticDataLevelsPath)
        .ToDictionary(x => x.LevelKey, x => x);
    }
      
    public LevelStaticData ForLevel(string sceneKey) =>
      _levels.TryGetValue(sceneKey,out LevelStaticData staticData) 
        ? staticData 
        : null;
  }
}