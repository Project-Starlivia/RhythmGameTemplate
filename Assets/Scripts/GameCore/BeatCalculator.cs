using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GameCore
{
    public class BeatCalculator
    {
        public static IEnumerable<Beat> FindRangeBeats(Beat[] beats, double from, double to)
        {
            return beats
                .Where(beat => beat.Tick > from && beat.Tick <= to);
        }
        
        [CanBeNull]
        public static IEnumerable<Beat> FindOffsetBeats(Beat[] beats, double previousTick, double currentTick, double offsetTick)
        {
            var previousAppearThreshold = previousTick + offsetTick;
            var currentAppearThreshold = currentTick + offsetTick;

            return FindRangeBeats(beats, previousAppearThreshold, currentAppearThreshold);
        }

        public static IEnumerable<Beat> FindHitBeats(Beat[] beats, double currentTick, double rangeMs, int bpm)
        {
            var rangeTick = TickCalculator.ElapsedTimeToTick(rangeMs / 1000, bpm);

            return FindRangeBeats(beats, currentTick - rangeTick, currentTick + rangeTick);
        }
    }
}