using System;

namespace formulas
{
    public struct Experience
    {
        private int a;
        private float h;
        private int k;

        public Experience(int a, float h, int k)
        {
            this.a = a;
            this.h = h;
            this.k = k;
        }

        public int GetExperience(int N)
        {
            return (int)Math.Round(((float)a / k) * Math.Pow(N, h));
        }
        
    }
}