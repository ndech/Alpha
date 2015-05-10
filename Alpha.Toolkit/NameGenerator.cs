using System;
using System.Collections.Generic;
using System.IO;

namespace Alpha.Toolkit
{
    public static class NameGenerator
    {
        private static List<String> provinceNames;
        private static List<String> settlementNames;

        public static String GetRandomProvinceName()
        {
            return provinceNames.RandomItem();
        }
        public static String GetSettlementName()
        {
            return provinceNames.RandomItem();
        }

        static NameGenerator()
        {
            provinceNames = ReadFromFile(@"Data\ProvinceNames.txt");
            settlementNames = ReadFromFile(@"Data\SettlementNames.txt");
        }

        public static List<String> ReadFromFile(String filename)
        {
            List<String> result = new List<string>();
            String line;
            using (StreamReader file = new StreamReader(filename))
                while ((line = file.ReadLine()) != null)
                    result.Add(line);
            return result;
        }
    }
}
