using System;

namespace formulas
{
    public struct Reroll
    {
        private int D;
        private int d;
        private int a;

        public Reroll(int D,int d,int a)
        {
            this.D = D;
            this.d = d;
            this.a = a;
        }

        public int GetReroll(int uso)
        {
            return (int)Math.Round((float)a / D + Math.Pow(((float)a / d), uso - 1));
        }
        
    }
}