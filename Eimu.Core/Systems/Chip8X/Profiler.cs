using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Eimu.Core.Systems.Chip8X
{
    public class Profiler
    {
        private Dictionary<ChipOpCode, int> m_Counts;
        private bool m_Stop;

        public Profiler()
        {
            m_Stop = false;
            m_Counts = new Dictionary<ChipOpCode, int>();
        }

        public void CountOpcode(ChipOpCode code)
        {
            if (m_Stop)
                return;

            int count = 0;

            if (m_Counts.TryGetValue(code, out count))
            {
                m_Counts[code] = ++count;
            }
            else
            {
                m_Counts.Add(code, 1);
            }
        }

        public void DumpStats()
        {
            m_Stop = true;
            foreach (KeyValuePair<ChipOpCode, int> val in m_Counts)
            {
                Console.WriteLine(val.Key.ToString() + " : " + val.Value.ToString());
            }
        }
    }
}
