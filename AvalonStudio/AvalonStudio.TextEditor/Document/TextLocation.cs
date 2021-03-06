using System;
using System.ComponentModel;
using System.Globalization;

namespace AvalonStudio.TextEditor.Document
{
#if !NREFACTORY
    /// <summary>
    ///     A line/column position.
    ///     Text editor lines/columns are counted started from one.
    /// </summary>
    /// <remarks>
    ///     The document provides the methods <see cref="IDocument.GetLocation" /> and
    ///     <see cref="IDocument.GetOffset(TextLocation)" /> to convert between offsets and TextLocations.
    /// </remarks>

    [TypeConverter(typeof(TextLocationConverter))]
    public struct TextLocation : IComparable<TextLocation>, IEquatable<TextLocation>
    {
        /// <summary>
        ///     Represents no text location (0, 0).
        /// </summary>
        public static readonly TextLocation Empty = new TextLocation(0, 0);

        /// <summary>
        ///     Creates a TextLocation instance.
        /// </summary>
        public TextLocation(int line, int column)
        {
            Line = line;
            Column = column;
        }

        /// <summary>
        ///     Gets the line number.
        /// </summary>
        public int Line { get; }

        /// <summary>
        ///     Gets the column number.
        /// </summary>
        public int Column { get; }

        /// <summary>
        ///     Gets whether the TextLocation instance is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return Column <= 0 && Line <= 0; }
        }

        /// <summary>
        ///     Gets a string representation for debugging purposes.
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "(Line {1}, Col {0})", Column, Line);
        }

        /// <summary>
        ///     Gets a hash code.
        /// </summary>
        public override int GetHashCode()
        {
            return unchecked(191 * Column.GetHashCode() ^ Line.GetHashCode());
        }

        /// <summary>
        ///     Equality test.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is TextLocation)) return false;
            return (TextLocation)obj == this;
        }

        /// <summary>
        ///     Equality test.
        /// </summary>
        public bool Equals(TextLocation other)
        {
            return this == other;
        }

        /// <summary>
        ///     Equality test.
        /// </summary>
        public static bool operator ==(TextLocation left, TextLocation right)
        {
            return left.Column == right.Column && left.Line == right.Line;
        }

        /// <summary>
        ///     Inequality test.
        /// </summary>
        public static bool operator !=(TextLocation left, TextLocation right)
        {
            return left.Column != right.Column || left.Line != right.Line;
        }

        /// <summary>
        ///     Compares two text locations.
        /// </summary>
        public static bool operator <(TextLocation left, TextLocation right)
        {
            if (left.Line < right.Line)
                return true;
            if (left.Line == right.Line)
                return left.Column < right.Column;
            return false;
        }

        /// <summary>
        ///     Compares two text locations.
        /// </summary>
        public static bool operator >(TextLocation left, TextLocation right)
        {
            if (left.Line > right.Line)
                return true;
            if (left.Line == right.Line)
                return left.Column > right.Column;
            return false;
        }

        /// <summary>
        ///     Compares two text locations.
        /// </summary>
        public static bool operator <=(TextLocation left, TextLocation right)
        {
            return !(left > right);
        }

        /// <summary>
        ///     Compares two text locations.
        /// </summary>
        public static bool operator >=(TextLocation left, TextLocation right)
        {
            return !(left < right);
        }

        /// <summary>
        ///     Compares two text locations.
        /// </summary>
        public int CompareTo(TextLocation other)
        {
            if (this == other)
                return 0;
            if (this < other)
                return -1;
            return 1;
        }
    }

    /// <summary>
    ///     Converts strings of the form '0+[;,]0+' to a <see cref="TextLocation" />.
    /// </summary>
    public class TextLocationConverter : TypeConverter
    {
        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <inheritdoc />
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(TextLocation) || base.CanConvertTo(context, destinationType);
        }

        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var parts = ((string)value).Split(';', ',');
                if (parts.Length == 2)
                {
                    return new TextLocation(int.Parse(parts[0], culture), int.Parse(parts[1], culture));
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc />
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType)
        {
            if (value is TextLocation && destinationType == typeof(string))
            {
                var loc = (TextLocation)value;
                return loc.Line.ToString(culture) + ";" + loc.Column.ToString(culture);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    ///     An (Offset,Length)-pair.
    /// </summary>
    public interface ISegment
    {
        /// <summary>
        ///     Gets the start offset of the segment.
        /// </summary>
        int Offset { get; }

        /// <summary>
        ///     Gets the length of the segment.
        /// </summary>
        /// <remarks>For line segments (IDocumentLine), the length does not include the line delimeter.</remarks>
        int Length { get; }

        /// <summary>
        ///     Gets the end offset of the segment.
        /// </summary>
        /// <remarks>EndOffset = Offset + Length;</remarks>
        int EndOffset { get; }
    }

    /// <summary>
    ///     Extension methods for <see cref="ISegment" />.
    /// </summary>
    public static class ISegmentExtensions
    {
        /// <summary>
        ///     Gets whether <paramref name="segment" /> fully contains the specified segment.
        /// </summary>
        /// <remarks>
        ///     Use <c>segment.Contains(offset, 0)</c> to detect whether a segment (end inclusive) contains offset;
        ///     use <c>segment.Contains(offset, 1)</c> to detect whether a segment (end exclusive) contains offset.
        /// </remarks>
        public static bool Contains(this ISegment segment, int offset, int length)
        {
            return segment.Offset <= offset && offset + length <= segment.EndOffset;
        }

        /// <summary>
        ///     Gets whether <paramref name="thisSegment" /> fully contains the specified segment.
        /// </summary>
        public static bool Contains(this ISegment thisSegment, ISegment segment)
        {
            return segment != null && thisSegment.Offset <= segment.Offset && segment.EndOffset <= thisSegment.EndOffset;
        }
    }

#endif
}