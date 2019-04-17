using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp19
{
    class Program
    {
        static void Main(string[] args)
        {
            var ranges = new List<Range<string>>
            {
                new StringRange("W", 5,8),
                new StringRange("S",1,4),
                new StringRange("L",2,6),
                new StringRange("X",9,20),
                new StringRange("T",21, 30)
            };


            var finder = new OverlappingRangeFinder();

            c4(ranges);

            var results = finder.FindOverlaps(ranges).ToList();



            foreach (var result in results)
            {

                Console.WriteLine("{0} overlaps with: {1}", result.Range, string.Join(", ", result.OverlappingRanges.Select(r => r)));
            }

        }

        static void c4(IEnumerable<IRange> ranges)
        {
            var finder = new OverlappingRangeFinder();


            var results = finder.FindOverlaps(ranges);
        }


    }

    public class OverlappingRangeFinder
    {
        public IRangeOverlaList<TRange> FindOverlaps<TRange>(IEnumerable<TRange> ranges) where TRange : IRange
        {
            var orderedRanges = ranges.OrderBy(range => range.From).ToList();

            var dic = new Dictionary<TRange, List<TRange>>();

            Action<TRange, TRange> saveOverlap = (main, overlapping) =>
            {
                if (!dic.ContainsKey(main))
                {
                    dic.Add(main, new List<TRange>());
                }

                dic[main].Add(overlapping);
            };

            Action<TRange, int> findOverlaps = (current, position) =>
            {
                for (int i = position; i < orderedRanges.Count; i++)
                {
                    if (orderedRanges[i].From > current.To)
                    {
                        break;
                    }

                    saveOverlap(current, orderedRanges[i]);
                    saveOverlap(orderedRanges[i], current);
                }
            };

            for (int i = 0; i < orderedRanges.Count(); i++)
            {
                findOverlaps(orderedRanges[i], i + 1);
            }

            return
                new RangeOverlapList<TRange>(
                    dic.Select(c => new RangeOverlap<TRange>(c.Key, c.Value)).Cast<IRangeOverlap<TRange>>());
        }
    }
    public class RangeOverlapList<TRange> : 
        List<IRangeOverlap<TRange>>,
        IRangeOverlaList<TRange> where TRange: IRange
    {
        public RangeOverlapList(IEnumerable<IRangeOverlap<TRange>> range) : base(range)

        {

        }
    }

    public interface IRangeOverlaList<TRange> : IList<IRangeOverlap<TRange>> where TRange: IRange
    {

    }

    public interface IRangeOverlap<TRange> where TRange : IRange
    {
        TRange Range { get; }

        List<TRange> OverlappingRanges { get; }
    }

    public class RangeOverlap<T> : IRangeOverlap<T> where T: IRange
    {
        public RangeOverlap(T range, List<T> overlappingRanges)
        {
            Range = range;
            OverlappingRanges = overlappingRanges;
        }
        public T Range { get; }
        public List<T> OverlappingRanges { get; }


    }

    public class StringRange : Range<string>
    {
        public StringRange(string value, int from, int to) : base(value, from, to)
        {
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class Range<T> 
        : Range,
            IRange<T>

    {
        public Range(T value, int from, int to) : base(from, to)
        {
            Value = value;
        }

        public T Value
        {
            get;
        }
    }

    public interface IRange<TValue> : IRange
    {
        TValue Value { get; }
    }

    public interface IRange
    {
        int From { get; }

        int To { get; }
    }

    public class Range : IRange
    {
        public Range(int from, int to)
        {
            From = from;
            To = to;
        }
        public int From { get; }

        public int To { get; }
    }
}
