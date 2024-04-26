using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
    internal class Score
    {
        private int[] breakPoints = { 350, 300, 250, 150, 100, 50};
        private int[] breakPointIndex = { 0, 0, 0, 0, 0, 0};
        private int score;

        public Score() 
        {
            score = 0;
        }

        public void SetMovementScore(int y)
        { 
            for (int i = 0; i < breakPoints.Length; i++)
            {
                if (y <= breakPoints[i] && breakPointIndex[i] == 0)
                {
                    score += 10;
                    breakPointIndex[i] = 1;
                }
            }
        }

        public void SetCellScore()
        {
            score += 500;

        }
        public void Increment(int amount = 10) 
        {
            score += amount;
        }

        public void Decrement(int amount = 10) 
        {
            score -= amount;
        }


        public int GetScore()
        {
            return score;
        }

        public void BreakPointReset()
        {
            for (int i = 0; i < breakPoints.Length; i++)
            {
                breakPointIndex[i] = 0;
            }
        }
        public void FullReset()
        {
            BreakPointReset();
            score = 0;
        }
    }
}
