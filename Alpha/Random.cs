﻿using SystemRandom = System.Random;

namespace Alpha
{
    class Random
    {
        private readonly SystemRandom _generator;
        private Random()
        {
            _generator = new SystemRandom();
        }

        private static Random _instance;

        public int Get(int min, int max)
        {
            return _generator.Next(min, max);
        }

        public static Random Generator { get { return _instance ?? (_instance = new Random()); } }
    }
}
