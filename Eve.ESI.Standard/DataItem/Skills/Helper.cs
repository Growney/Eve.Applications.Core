using System;
using System.Collections.Generic;
using System.Linq;

namespace Eve.ESI.Standard.DataItem.Skills
{
    public static class Helper
    {
        private static readonly string[] c_numerals = { "0","I", "II", "III", "IV", "V" };

        public static string GetRomanLevel(int level)
        {
            if(level > 5 || level < 0)
            {
                throw new ArgumentException("Level must be between 0 and 5");
            }
            return c_numerals[level];
        }

        public static SkillQueueItem GetFirstSkillInQueue(this IEnumerable<SkillQueueItem> queue)
        {
            SkillQueueItem min = null;
            if (queue != null)
            {
                DateTime now = DateTime.UtcNow;
                List<SkillQueueItem> active = queue.Where(x => x.FinishDate > now).ToList();
                if (active.Count > 0)
                {
                    min = active[0];
                    for (int i = 1; i < active.Count; i++)
                    {
                        if (active[i].QueuePosition < min.QueuePosition)
                        {
                            min = active[i];
                        }
                    }
                }
            }
            return min;
        }
    }
}
