using System;
using System.Collections.Generic;
using System.IO;

namespace Alpha.Toolkit
{
    public static class NameGenerator
    {
        private static List<string> provinceNames;
        private static List<string> settlementNames;

        public static string GetRandomProvinceName()
        {
            return provinceNames.RandomItem();
        }
        public static string GetSettlementName()
        {
            return provinceNames.RandomItem();
        }

        static NameGenerator()
        {
            provinceNames = ReadFromFile(@"Data\ProvinceNames.txt");
            settlementNames = ReadFromFile(@"Data\SettlementNames.txt");
        }

        public static List<string> ReadFromFile(string filename)
        {
            List<string> result = new List<string>();
            string line;
            using (StreamReader file = new StreamReader(filename))
                while ((line = file.ReadLine()) != null)
                    result.Add(line);
            return result;
        }
    }
}
