using System;

namespace Assets.Scripts
{
    internal class Range : IComparable<Range>
    {
        private int start;
        private int end;

        public Range(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public Range(string r)
        {
            string[] parts = r.Split('-');
            this.start = Int32.Parse(parts[1]);
            this.end = Int32.Parse(parts[2]);
        }

        public int CompareTo(Range other)
        {
            if (this < other)
            {
                return -1;
            }
            if (this > other)
            {
                return 1;
            }
            if (this <= other)
            {
                return -1;
            }
            if (other <= this)
            {
                return 1;
            }
            return 0;
        }

        public static bool operator >(Range r1, Range r2)
        {
            return r1.Start > r2.End;
        }

        public static bool operator <(Range r1, Range r2)
        {
            return r1.End < r2.Start;
        }

        public static bool operator >=(Range r1, Range r2)
        {
            return r1.End > r2.End;
        }

        public static bool operator <=(Range r1, Range r2)
        {
            return r1.Start < r2.Start;
        }

        public static bool operator ==(Range r1, Range r2)
        {
            if (ReferenceEquals(r2, null)) return false;
            if (ReferenceEquals(r1, null)) return false;
            return (r1.Start == r2.Start) && (r1.End == r2.End);
        }

        public static bool operator !=(Range r1, Range r2)
        {
            if (ReferenceEquals(r2, null)) return false;
            if (ReferenceEquals(r1, null)) return false;
            return (r1.Start != r2.Start) && (r1.End != r2.End);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + start.GetHashCode();
            hash = (hash * 7) + end.GetHashCode();
            return hash;
        }

        public override bool Equals(Object other)
        {
            if (other.GetType() == GetType())
            {
                var otherRange = other as Range;
                return (start == otherRange.Start) && (end == otherRange.End);
            }
            return false;
        }

        public int Start
        {
            get { return start; }
        }

        public int End
        {
            get { return end; }
        }

        public override string ToString()
        {
            return start.ToString() + "-" + end.ToString();
        }
    }
}