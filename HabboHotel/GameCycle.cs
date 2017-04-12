using System.Threading;
using System.Threading.Tasks;

namespace ProjectHub.HabboHotel
{
    public class GameCycle
    {
        private bool CycleEnded;
        private bool CycleActive;
        private Task Cycle;
        private int CycleSleepTime = 25;

        public void StartLoop()
        {
            Cycle = new Task(Loop);
            Cycle.Start();
            CycleActive = true;
        }

        private void Loop()
        {
            while (CycleActive)
            {
                CycleEnded = false;
                CycleEnded = true;
                Thread.Sleep(CycleSleepTime);

                // OnCycle functions here!
            }
        }

        public void StopLoop()
        {
            CycleActive = false;

            while (!CycleEnded)
            {
                Thread.Sleep(CycleSleepTime);
            }
        }
    }
}