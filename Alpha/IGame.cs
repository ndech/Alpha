using System;

namespace Alpha
{
    interface IGame
    {
        ServiceContainer Services { get; }
        void Exit();
        void Save(String fileName);
        void Load(String fileName, Action<String> feedback);

        void Draw();
    }
}
