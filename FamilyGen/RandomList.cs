using System;
using System.Collections.Generic;

namespace FamilyGen {
    class RandomList {
        private static Random rnd = new Random();
        private List<string> data = new List<string>();
        private List<double> weight = new List<double>();
        private double total = 0.0;

        public void Add(string str, double wt)
        {
            data.Add(str);
            weight.Add(wt);
            total += wt;
        }

        public string Get() {
            if (data.Count == 0)
                throw new InvalidOperationException("The list is empty");

            double n = rnd.NextDouble() * total;
            int p = 0;

            while (true) {
                n -= weight[p];
                if (n <= 0.0)
                    return data[p];

                ++p;

                if (p >= data.Count)
                    throw new IndexOutOfRangeException("Bad number generated");
            }
        }
    }
}
