// From ASP.Net Web Stack [http://aspnetwebstack.codeplex.com/]
// Used under the Apache License

namespace RazorEngineHost
{
    using System;
    using System.Diagnostics;

    using RazorEngineHost.Common;

    /// <summary>PositionTagged</summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("({Position})\"{Value}\"")]
    public class PositionTagged<T>
    {
        /// <summary>The position.</summary>
        public int Position { get; }

        /// <summary>The value.</summary>
        public T Value { get; }

        private PositionTagged()
        {
            this.Position = 0;
            this.Value = default(T);
        }

        /// <summary>Creates a new PositionTagged instance</summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public PositionTagged(T value, int offset)
        {
            this.Position = offset;
            this.Value = value;
        }

        /// <summary>Checks if the given object equals the current object.</summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as PositionTagged<T>;
            return other != null &&
                   other.Position == this.Position &&
                   Equals(other.Value, this.Value);
        }

        /// <summary>Calculates a hash-code for the current instance.</summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCodeCombiner.Start()
                .Add(this.Position)
                .Add(this.Value)
                .CombinedHash;
        }

        /// <summary>Returns Value.ToString().</summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }

        /// <summary>convert implicitely to the value.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator T(PositionTagged<T> value)
        {
            return value.Value;
        }

        /// <summary>Convert from a tuple.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator PositionTagged<T>(Tuple<T, int> value)
        {
            return new PositionTagged<T>(value.Item1, value.Item2);
        }

        /// <summary>Checks if the given instances are equal.</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(PositionTagged<T> left, PositionTagged<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>Checks if the given instances are not equal.</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(PositionTagged<T> left, PositionTagged<T> right)
        {
            return !Equals(left, right);
        }
    }
}