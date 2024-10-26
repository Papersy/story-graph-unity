﻿using CodeBase.Infrastructure.Services;

namespace CodeBase.StaticData
{
  public interface IStaticDataService : IService
  {
    LevelStaticData ForLevel(string sceneKey);
  }
}