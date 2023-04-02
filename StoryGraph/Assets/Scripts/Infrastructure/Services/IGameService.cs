﻿using System;
using CodeBase.Infrastructure.Services;

namespace Infrastructure.Services
{
    public interface IGameService : IService
    {
        // event Action<string> OnLocationChanged;
        //
        void GenerateMap();
        GameController GetGameController();
    }
}