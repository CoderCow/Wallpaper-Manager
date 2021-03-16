using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

using Common.IO.Serialization;

namespace Common {
  [Serializable]
  [XmlRoot]
  public struct Percentage: 
    IComparable, IComparable<Percentage>, IEquatable<Percentage>, IFormattable
  {
    #region Constants
    /// <summary>
    ///   Represents the smallest possible value of <see cref="Percentage" />.
    /// </summary>
    public const Single MinValue = 0f;
    /// <summary>
    ///   Represents the largest possible value of <see cref="Percentage" />.
    /// </summary>
    public const Single MaxValue = 1f;

    /// <summary>
    ///   Represents a <see cref="Percentage" /> with a value of zero.
    /// </summary>
    public static readonly Percentage Zero = new Percentage(0f);
    /// <summary>
    ///   Represents a <see cref="Percentage" /> with a value of 
    ///   <see cref="Single.NaN">Single.NaN</see>.
    /// </summary>
    public static readonly Percentage NaN = new Percentage(Single.NaN);
    /// <summary>
    ///   Represents a <see cref="Percentage" /> with a value of 
    ///   <see cref="Single.PositiveInfinity">Single.PositiveInfinity</see>.
    /// </summary>
    public static readonly Percentage PositiveInfinity = new Percentage(Single.PositiveInfinity);
    /// <summary>
    ///   Represents a <see cref="Percentage" /> with a value of 
    ///   <see cref="Single.NegativeInfinity">Single.NegativeInfinity</see>.
    /// </summary>
    public static readonly Percentage NegativeInfinity = new Percentage(Single.NegativeInfinity);
    /// <summary>
    ///   Represents a smallest possible <see cref="Percentage" /> value that is greater 
    ///   than zero.
    /// </summary>
    public static readonly Percentage Epsilon = new Percentage(Single.Epsilon);
    #endregion

    /// <summary>
    ///   The internal percentage value representation.
    /// </summary>
    private readonly Single value;


    #region Method: Constructor
    /// <summary>
    ///   Creates a new <see cref="Percentage" /> object using the given <see cref="Single" /> 
    ///   value.
    /// </summary>
    /// <param name="value">
    ///   The <see cref="Single" /> value which should be represented by this
    ///   <see cref="Percentage" />.
    /// </param>
    public Percentage(Single value) {
      Contract.Requires<ArgumentOutOfRangeException>(
        value >= Percentage.MinValue && value <= Percentage.MaxValue);

      this.value = value;
    }
    #endregion

    #region Static Methods: Parse, TryParse
    /// <inheritdoc cref="Parse(String, NumberStyles, IFormatProvider)" />
    public static Percentage Parse(String s) {
      Percentage result;
      Percentage.TryParse(s, out result);
      return result;
    }
    
    /// <summary>
    ///   Converts the string representation of a number to its percentage number equivalent.
    /// </summary>
    /// <inheritdoc cref="Single.Parse(String, NumberStyles, IFormatProvider)" />
    /// <returns>
    ///   A percentage that is equivalent to the numeric value or symbol specified in 
    ///   <paramref name="s" />.
    /// </returns>
    public static Percentage Parse(String s, NumberStyles style, IFormatProvider provider) {
      Percentage result;
      Percentage.TryParse(s, style, provider, out result);
      return result;
    }

    /// <inheritdoc cref="TryParse(String, NumberStyles, IFormatProvider, out Percentage)" />
    public static Percentage Parse(String s, IFormatProvider provider) {
      Percentage result;
      Percentage.TryParse(s, provider, out result);
      return result;
    }

    /// <inheritdoc cref="TryParse(String, NumberStyles, IFormatProvider, out Percentage)" />
    public static Boolean TryParse(String s, out Percentage result) {
      return Percentage.TryParse(
        s, NumberStyles.Float | NumberStyles.AllowThousands, null, out result);
    }

    /// <inheritdoc cref="TryParse(String, NumberStyles, IFormatProvider, out Percentage)" />
    public static Boolean TryParse(String s, IFormatProvider provider, out Percentage result) {
      return Percentage.TryParse(
        s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);
    }

    /// <summary>
    ///   Converts the string representation of a number to its percentage number equivalent. 
    ///   A return code indicates whether the conversion succeeded or failed.
    /// </summary>
    /// <inheritdoc cref="Single.TryParse(String, NumberStyles, IFormatProvider, out Single)" />
    /// <param name="result">
    ///   When this method returns, contains percentage number equivalent to the numeric 
    ///   value or symbol contained in <paramref name="s" />, if the conversion succeeded, 
    ///   or zero if the conversion failed. The conversion fails if the <paramref name="s" /> 
    ///   parameter is <c>null</c>, is not a number in a valid format, or represents a number 
    ///   less than <see cref="MinValue" /> or greater than <see cref="MaxValue" />. 
    ///   This parameter is passed uninitialized. 
    /// </param>
    public static Boolean TryParse(
      String s, NumberStyles style, IFormatProvider provider, out Percentage result
    ) {
      if (s == null) throw new ArgumentNullException();
      if ((style & NumberStyles.AllowHexSpecifier) != 0) throw new ArgumentException();

      NumberFormatInfo numberFormat = 
        (NumberFormatInfo)provider.GetFormat(typeof(NumberFormatInfo));

      if (numberFormat != null) {
        s = s.Replace(numberFormat.PercentSymbol, String.Empty);
      }
      
      Single singleResult;
      if (Single.TryParse(s, style, provider, out singleResult)) {
        result = new Percentage(singleResult / 100f);
        return true;
      }

      result = Percentage.NaN;
      return false;
    }
    #endregion

    #region Static Methods: IsInfinity, IsPositiveInfinity, IsNegativeInfinity, IsNaN
    /// <summary>
    ///   Returns a value indicating whether the specified number evaluates to 
    ///   <see cref="NegativeInfinity" /> or <see cref="PositiveInfinity" />.
    /// </summary>
    /// <param name="value">
    ///   The <see cref="Percentage" /> object to check.
    /// </param>
    /// <returns>
    ///   A value indicating whether the specified number evaluates to 
    ///   <see cref="NegativeInfinity" /> or <see cref="PositiveInfinity" />.
    /// </returns>
    public static Boolean IsInfinity(Percentage value) {
      return Single.IsInfinity(value.value);
    }

    /// <summary>
    ///   Returns a value indicating whether the specified number evaluates to 
    ///   <see cref="PositiveInfinity" />.
    /// </summary>
    /// <param name="value">
    ///   The <see cref="Percentage" /> object to check.
    /// </param>
    /// <returns>
    ///   A value indicating whether the specified number evaluates to 
    ///   <see cref="PositiveInfinity" />.
    /// </returns>
    public static Boolean IsPositiveInfinity(Percentage value) {
      return Single.IsPositiveInfinity(value.value);
    }

    /// <summary>
    ///   Returns a value indicating whether the specified number evaluates to 
    ///   <see cref="NegativeInfinity" />.
    /// </summary>
    /// <param name="value">
    ///   The <see cref="Percentage" /> object to check.
    /// </param>
    /// <returns>
    ///   A value indicating whether the specified number evaluates to 
    ///   <see cref="NegativeInfinity" />.
    /// </returns>
    public static Boolean IsNegativeInfinity(Percentage value) {
      return Single.IsNegativeInfinity(value.value);
    }

    /// <summary>
    ///   Returns a value indicating whether the specified number evaluates to 
    ///   <see cref="NaN" /> (Not a Number).
    /// </summary>
    /// <param name="value">
    ///   The <see cref="Percentage" /> object to check.
    /// </param>
    /// <returns>
    ///   A value indicating whether the specified number evaluates to 
    ///   <see cref="NaN" /> (Not a Number).
    /// </returns>
    public static Boolean IsNaN(Percentage value) {
      return Single.IsNaN(value.value);
    }
    #endregion

    #region Static Methods: Operator Overloads
    /// <summary>
    ///   Returns a value that indicates whether two specified <see cref="Percentage" /> 
    ///   values are equal. 
    /// </summary>
    /// <param name="left">
    ///   The first value to compare.
    /// </param>
    /// <param name="right">
    ///   The second value to compare.
    /// </param>
    /// <returns>
    ///   A value that indicates whether two specified <see cref="Percentage" /> 
    ///   values are equal. 
    /// </returns>
    public static Boolean operator ==(Percentage left, Percentage right) {
      return (left.CompareTo(right) == 0);
    }

    /// <summary>
    ///   Returns a value that indicates whether two specified <see cref="Percentage" /> 
    ///   values are not equal. 
    /// </summary>
    /// <param name="left">
    ///   The first value to compare.
    /// </param>
    /// <param name="right">
    ///   The second value to compare.
    /// </param>
    /// <returns>
    ///   A value that indicates whether two specified <see cref="Percentage" /> 
    ///   values are not equal. 
    /// </returns>
    public static Boolean operator !=(Percentage left, Percentage right) {
      return (left.CompareTo(right) != 0);
    }

    /// <summary>
    ///   Returns a value that indicates whether an specified <see cref="Percentage" /> 
    ///   value is equal or greater to another specified <see cref="Percentage" /> value. 
    /// </summary>
    /// <param name="left">
    ///   The first value to compare.
    /// </param>
    /// <param name="right">
    ///   The second value to compare.
    /// </param>
    /// <returns>
    ///   A value that indicates whether an specified <see cref="Percentage" /> 
    ///   value is equal or greater to another specified <see cref="Percentage" /> value. 
    /// </returns>
    public static Boolean operator >=(Percentage left, Percentage right) {
      return (left.CompareTo(right) >= 0);
    }

    /// <summary>
    ///   Returns a value that indicates whether an specified <see cref="Percentage" /> 
    ///   value is equal or lesser to another specified <see cref="Percentage" /> value. 
    /// </summary>
    /// <param name="left">
    ///   The first value to compare.
    /// </param>
    /// <param name="right">
    ///   The second value to compare.
    /// </param>
    /// <returns>
    ///   A value that indicates whether an specified <see cref="Percentage" /> 
    ///   value is equal or lesser to another specified <see cref="Percentage" /> value. 
    /// </returns>
    public static Boolean operator <=(Percentage left, Percentage right) {
      return (left.CompareTo(right) <= 0);
    }

    /// <summary>
    ///   Returns a value that indicates whether an specified <see cref="Percentage" /> 
    ///   value is greater to another specified <see cref="Percentage" /> value. 
    /// </summary>
    /// <param name="left">
    ///   The first value to compare.
    /// </param>
    /// <param name="right">
    ///   The second value to compare.
    /// </param>
    /// <returns>
    ///   A value that indicates whether an specified <see cref="Percentage" /> 
    ///   value is greater to another specified <see cref="Percentage" /> value. 
    /// </returns>
    public static Boolean operator >(Percentage left, Percentage right) {
      return (left.CompareTo(right) > 0);
    }

    /// <summary>
    ///   Returns a value that indicates whether an specified <see cref="Percentage" /> 
    ///   value is lesser to another specified <see cref="Percentage" /> value. 
    /// </summary>
    /// <param name="left">
    ///   The first value to compare.
    /// </param>
    /// <param name="right">
    ///   The second value to compare.
    /// </param>
    /// <returns>
    ///   A value that indicates whether an specified <see cref="Percentage" /> 
    ///   value is lesser to another specified <see cref="Percentage" /> value. 
    /// </returns>
    public static Boolean operator <(Percentage left, Percentage right) {
      Percentage t = left+=right;
      return (left.CompareTo(right) < 0);
    }

    public static Percentage operator +(Percentage left, Percentage right) {
      return new Percentage(left.value + right.value);
    }

    public static Percentage operator ++(Percentage p) {
      return new Percentage(p.value + 1f);
    }

    public static Percentage operator -(Percentage left, Percentage right) {
      return new Percentage(left.value - right.value);
    }

    public static Percentage operator --(Percentage p) {
      return new Percentage(p.value - 1f);
    }

    public static Percentage operator *(Percentage left, Percentage right) {
      return new Percentage(left.value * right.value);
    }

    public static Percentage operator /(Percentage left, Percentage right) {
      return new Percentage(left.value / right.value);
    }

    public static Percentage operator %(Percentage left, Percentage right) {
      return new Percentage(left.value % right.value);
    }
    #endregion

    #region Static Methods: Implicit/Explicit Casts
    public static implicit operator Single(Percentage percentage) {
      return percentage.value;
    }

    public static explicit operator Double(Percentage percentage) {
      return percentage.value;
    }

    public static explicit operator Decimal(Percentage percentage) {
      return (Decimal)percentage.value;
    }

    public static explicit operator Byte(Percentage percentage) {
      return (Byte)percentage.value;
    }

    public static explicit operator SByte(Percentage percentage) {
      return (SByte)percentage.value;
    }

    public static explicit operator Int16(Percentage percentage) {
      return (Int16)percentage.value;
    }

    public static explicit operator UInt16(Percentage percentage) {
      return (UInt16)percentage.value;
    }

    public static explicit operator Int32(Percentage percentage) {
      return (Int32)percentage.value;
    }

    public static explicit operator UInt32(Percentage percentage) {
      return (UInt32)percentage.value;
    }

    public static explicit operator Int64(Percentage percentage) {
      return (Int64)percentage.value;
    }

    public static explicit operator UInt64(Percentage percentage) {
      return (UInt64)percentage.value;
    }
    #endregion

    #region IEquatable Implementation
    /// <inheritdoc />
    [Pure]
    public override Boolean Equals(Object obj) {
      if (obj is Percentage) {
        return this.Equals((Percentage)obj);
      }
      if (obj is Single) {
        return this.value.Equals((Single)obj);
      }

      return false;
    }

    /// <inheritdoc />
    [Pure]
    public override Int32 GetHashCode() {
      return this.value.GetHashCode();
    }
    
    /// <summary>
    ///   Returns a value indicating whether this instance and a specified 
    ///   <see cref="Percentage" /> object represent the same value. 
    /// </summary>
    /// <param name="other">
    ///   The other <see cref="Percentage" /> object to compare with.
    /// </param>
    /// <returns>
    ///   A value indicating whether this instance and a specified <see cref="Percentage" /> 
    ///   object represent the same value.
    /// </returns>
    /// <seealso cref="IEquatable{T}.Equals(T)">IEquatable&lt;&gt;.Equals(T)</seealso>
    [Pure]
    public Boolean Equals(Percentage other) {
      return this.value.Equals(other.value);
    }
    #endregion

    #region IComparable Implementation
    /// <inheritdoc cref="IComparable.CompareTo(Object)" />
    [Pure]
    public Int32 CompareTo(Object obj) {
      if (!(obj is Percentage) && !(obj is Single)) throw new ArgumentException();
      Contract.Ensures(Contract.Result<Int32>() >= -1 || Contract.Result<Int32>() <= 1);

      if (obj is Percentage) {
        return this.CompareTo((Percentage)obj);
      }

      return this.CompareTo((Single)obj);
    }
    
    /// <inheritdoc cref="CompareTo(Object)" />
    [Pure]
    public Int32 CompareTo(Single other) {
      Contract.Ensures(Contract.Result<Int32>() >= -1 || Contract.Result<Int32>() <= 1);

      return this.value.CompareTo(other);
    }

    /// <inheritdoc cref="CompareTo(Object)" />
    [Pure]
    public Int32 CompareTo(Percentage other) {
      Contract.Ensures(Contract.Result<Int32>() >= -1 || Contract.Result<Int32>() <= 1);

      return this.value.CompareTo(other.value);
    }
    #endregion

    #region IFormattable Implementation
    /// <inheritdoc />
    [Pure]
    public override String ToString() {
      return this.ToString("P");
    }

    /// <inheritdoc cref="IConvertible.ToString(IFormatProvider)" />
    [Pure]
    public String ToString(IFormatProvider formatProvider) {
      return this.value.ToString("P", formatProvider);
    }

    /// <inheritdoc cref="IFormattable.ToString(String, IFormatProvider)" />
    [Pure]
    public String ToString(String format) {
      return this.value.ToString(format);
    }

    /// <inheritdoc cref="IFormattable.ToString(String, IFormatProvider)" />
    [Pure]
    public String ToString(String format, IFormatProvider formatProvider) {
      return this.value.ToString(format, formatProvider);
    }
    #endregion
  }
}
