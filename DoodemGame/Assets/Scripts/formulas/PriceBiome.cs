using System;

namespace formulas
{
    public struct PriceBiome
    {
        private int a;
        private int g;
        private int i;

        public PriceBiome(int a, int g, int i)
        {
            this.a = a;
            this.g = g;
            this.i = i;
        }

        public int GetPrice(int R,int B)
        {
            return (int)Math.Round((float)a / 5 + (R - 1) * g * B * i);
        }
    }
}