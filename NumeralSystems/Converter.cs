using System;

namespace NumeralSystems
{
    /// <summary>
    /// Converts a string representations of a numbers to its integer equivalent.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Converts the string representation of a positive number in the octal numeral system to its 32-bit signed integer equivalent.
        /// </summary>
        /// <param name="source">The string representation of a positive number in the octal numeral system.</param>
        /// <returns>A positive decimal value.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if source string presents a negative number
        /// - or
        /// contains invalid symbols (non-octal alphabetic characters).
        /// Valid octal alphabetic characters: 0,1,2,3,4,5,6,7.
        /// </exception>
        public static int ParsePositiveFromOctal(this string source)
        {
            if ((ParseByRadix(source, 8) ^ -1) > 0)
            {
                throw new ArgumentException("source string presents a negative number", nameof(source));
            }

            return ParseByRadix(source, 8);
        }

        /// <summary>
        /// Converts the string representation of a positive number in the decimal numeral system to its 32-bit signed integer equivalent.
        /// </summary>
        /// <param name="source">The string representation of a positive number in the decimal numeral system.</param>
        /// <returns>A positive decimal value.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if source string presents a negative number
        /// - or
        /// contains invalid symbols (non-decimal alphabetic characters).
        /// Valid decimal alphabetic characters: 0,1,2,3,4,5,6,7,8,9.
        /// </exception>
        public static int ParsePositiveFromDecimal(this string source)
        {
            if ((ParseByRadix(source, 10) ^ -1) > 0)
            {
                throw new ArgumentException("source string presents a negative number", nameof(source));
            }

            return ParseByRadix(source, 10);
        }

        /// <summary>
        /// Converts the string representation of a positive number in the hex numeral system to its 32-bit signed integer equivalent.
        /// </summary>
        /// <param name="source">The string representation of a positive number in the hex numeral system.</param>
        /// <returns>A positive decimal value.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if source string presents a negative number
        /// - or
        /// contains invalid symbols (non-hex alphabetic characters).
        /// Valid hex alphabetic characters: 0,1,2,3,4,5,6,7,8,9,A(or a),B(or b),C(or c),D(or d),E(or e),F(or f).
        /// </exception>
        public static int ParsePositiveFromHex(this string source)
        {
            // на отрицательность проверсяем и на переполнения 32 битов
            if ((ParseByRadix(source, 16) ^ -1) > 0 || source.Length > 8)
            {
                throw new ArgumentException("source string presents a negative number", nameof(source));
            }

            return ParseByRadix(source, 16);
        }

        /// <summary>
        /// Converts the string representation of a positive number in the octal, decimal or hex numeral system to its 32-bit signed integer equivalent.
        /// </summary>
        /// <param name="source">The string representation of a positive number in the the octal, decimal or hex numeral system.</param>
        /// <param name="radix">The radix.</param>
        /// <returns>A positive decimal value.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if source string presents a negative number
        /// - or
        /// contains invalid for given numeral system symbols
        /// -or-
        /// the radix is not equal 8, 10 or 16.
        /// </exception>
        public static int ParsePositiveByRadix(this string source, int radix)
        {
            if (radix == 8)
            {
                return ParsePositiveFromOctal(source);
            }
            else if (radix == 10)
            {
                return ParsePositiveFromDecimal(source);
            }
            else if (radix == 16)
            {
                return ParsePositiveFromHex(source);
            }
            else
            {
                throw new ArgumentException("invalid for given numeral system symbols", nameof(radix));
            }
        }

        /// <summary>
        /// Converts the string representation of a signed number in the octal, decimal or hex numeral system to its 32-bit signed integer equivalent.
        /// </summary>
        /// <param name="source">The string representation of a signed number in the the octal, decimal or hex numeral system.</param>
        /// <param name="radix">The radix.</param>
        /// <returns>A signed decimal value.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if source contains invalid for given numeral system symbols
        /// -or-
        /// the radix is not equal 8, 10 or 16.
        /// </exception>
        public static int ParseByRadix(this string source, int radix)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (radix == 8)
            {
                if (string.IsNullOrEmpty(source))
                {
                    throw new ArgumentNullException(nameof(source));
                }

                // 0 - 48 символ, 7 - 55
                int result = 0;
                for (int i = 0; i < source.Length; i++)
                {
                    if (source[i] < 48 || source[i] > 55)
                    {
                        throw new ArgumentException("invalid symbols", nameof(source));
                    }

                    result += (source[i] - '0') * (int)Math.Pow(8, source.Length - i - 1);
                }

                return result;
            }
            else if (radix == 10)
            {
                if (string.IsNullOrEmpty(source))
                {
                    throw new ArgumentNullException(nameof(source));
                }

                int result = 0;

                // для возвращение в случае чего отрицательного значения, если будет > 0 то зашло отрицательное значение и мы вернем значение с -
                int minus = 0;
                for (int i = 0; i < source.Length; i++)
                {
                    if (source[i] < 48 || source[i] > 57)
                    {
                        if (source[i] == '-')
                        {
                            i++;
                            minus++;
                        }
                        else
                        {
                            throw new ArgumentException("invalid symbols", nameof(source));
                        }
                    }

                    result += (source[i] - '0') * (int)Math.Pow(10, source.Length - i - 1);
                }

                if (minus > 0)
                {
                    return -result;
                }
                else
                {
                    return result;
                }
            }
            else if (radix == 16)
            {
                if (string.IsNullOrEmpty(source))
                {
                    throw new ArgumentNullException(nameof(source));
                }

                int result = 0;

                // проверяем значения из сурса, чтобы они совпадали от 0 до 9 и от А-а до F-f по таблице ASCII
                int count = 0;
                int[] arrwithascii = new int[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 65, 66, 67, 68, 69, 70, 97, 98, 99, 100, 101, 102 };
                for (int k = 0; k < source.Length; k++)
                {
                    for (int j = 0; j < arrwithascii.Length; j++)
                    {
                        count += source[k] == arrwithascii[j] ? 1 : 0;
                    }
                }

                if (count < source.Length)
                {
                    throw new ArgumentException("invalid symbols", nameof(source));
                }

                for (int i = 0; i < source.Length; i++)
                {
                    switch (source[i])
                    {
                        case 'a':
                            result += 10 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'A':
                            result += 10 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'b':
                            result += 11 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'B':
                            result += 11 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'c':
                            result += 12 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'C':
                            result += 12 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'd':
                            result += 13 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'D':
                            result += 13 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'e':
                            result += 14 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'E':
                            result += 14 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'f':
                            result += 15 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'F':
                            result += 15 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        default:
                            result += (source[i] - '0') * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                    }
                }

                return result;
            }
            else
            {
                throw new ArgumentException("invalid for given numeral system symbols", nameof(radix));
            }
        }

        /// <summary>
        /// Converts the string representation of a positive number in the octal numeral system to its 32-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="source">The string representation of a positive number in the octal numeral system.</param>
        /// <param name="value">A positive decimal value.</param>
        /// <returns>true if s was converted successfully; otherwise, false.</returns>
        public static bool TryParsePositiveFromOctal(this string source, out int value)
        {
            TryParseByRadix(source, 8, out value);
            if ((value ^ -1) > 0)
            {
                return false;
            }

            return TryParseByRadix(source, 8, out value);
        }

        /// <summary>
        /// Converts the string representation of a positive number in the decimal numeral system to its 32-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="source">The string representation of a positive number in the decimal numeral system.</param>
        /// <returns>A positive decimal value.</returns>
        /// <param name="value">A pos1itive decimal value.</param>
        /// <returns>true if s was converted successfully; otherwise, false.</returns>
        public static bool TryParsePositiveFromDecimal(this string source, out int value)
        {
            TryParseByRadix(source, 10, out value);
            if ((value ^ -1) > 0)
            {
                return false;
            }

            return TryParseByRadix(source, 10, out value);
        }

        /// <summary>
        /// Converts the string representation of a positive number in the hex numeral system to its 32-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="source">The string representation of a positive number in the hex numeral system.</param>
        /// <returns>A positive decimal value.</returns>
        /// <param name="value">A positiv1e decimal value.</param>
        /// <returns>true if s was converted successfully; otherwise, false.</returns>
        public static bool TryParsePositiveFromHex(this string source, out int value)
        {
            TryParseByRadix(source, 16, out value);
            if ((value ^ -1) > 0)
            {
                return false;
            }

            return TryParseByRadix(source, 16, out value);
        }

        /// <summary>
        /// Converts the string representation of a positive number in the octal, decimal or hex numeral system to its 32-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="source">The string representation of a positive number in the the octal, decimal or hex numeral system.</param>
        /// <param name="radix">The radix.</param>
        /// <returns>A positive decimal value.</returns>
        /// <param name="value">A positiv1e decimal value.</param>
        /// <returns>true if s was converted successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Thrown the radix is not equal 8, 10 or 16.</exception>
        public static bool TryParsePositiveByRadix(this string source, int radix, out int value)
        {
            if (radix == 8)
            {
                return TryParseByRadix(source, 8, out value);
            }
            else if (radix == 10)
            {
                return TryParseByRadix(source, 10, out value);
            }
            else if (radix == 16)
            {
                return TryParseByRadix(source, 16, out value);
            }
            else
            {
                throw new ArgumentException("invalid for given numeral system symbols", nameof(radix));
            }
        }

        /// <summary>
        /// Converts the string representation of a signed number in the octal, decimal or hex numeral system to its 32-bit signed integer equivalent.
        /// A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="source">The string representation of a signed number in the the octal, decimal or hex numeral system.</param>
        /// <param name="radix">The radix.</param>
        /// <returns>A positive decimal value.</returns>
        /// <param name="value">A positive decimal valu1e.</param>
        /// <returns>true if s was converted successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Thrown the radix is not equal 8, 10 or 16.</exception>
        public static bool TryParseByRadix(this string source, int radix, out int value)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (radix == 8)
            {
                if (string.IsNullOrEmpty(source))
                {
                    throw new ArgumentNullException(nameof(source));
                }

                // 0 - 48 символ, 7 - 55
                int result = 0;
                for (int i = 0; i < source.Length; i++)
                {
                    if (source[i] < 48 || source[i] > 55)
                    {
                        value = 0;
                        return false;
                    }

                    result += (source[i] - '0') * (int)Math.Pow(8, source.Length - i - 1);
                }

                value = result;
                return true;
            }
            else if (radix == 10)
            {
                if (string.IsNullOrEmpty(source))
                {
                    throw new ArgumentNullException(nameof(source));
                }

                int result = 0;

                // для возвращение в случае чего отрицательного значения, если будет > 0 то зашло отрицательное значение и мы вернем значение с -
                int minus = 0;
                for (int i = 0; i < source.Length; i++)
                {
                    if (source[i] < 48 || source[i] > 57)
                    {
                        if (source[i] == '-')
                        {
                            i++;
                            minus++;
                        }
                        else
                        {
                            value = 0;
                            return false;
                        }
                    }

                    result += (source[i] - '0') * (int)Math.Pow(10, source.Length - i - 1);
                }

                if (minus > 0)
                {
                    value = -result;
                    return true;
                }
                else
                {
                    value = result;
                    return true;
                }
            }
            else if (radix == 16)
            {
                if (string.IsNullOrEmpty(source))
                {
                    throw new ArgumentNullException(nameof(source));
                }

                int result = 0;

                // проверяем значения из сурса, чтобы они совпадали от 0 до 9 и от А-а до F-f по таблице ASCII
                int count = 0;
                int[] arrwithascii = new int[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 65, 66, 67, 68, 69, 70, 97, 98, 99, 100, 101, 102 };
                for (int k = 0; k < source.Length; k++)
                {
                    for (int j = 0; j < arrwithascii.Length; j++)
                    {
                        count += source[k] == arrwithascii[j] ? 1 : 0;
                    }
                }

                if (count < source.Length || source.Length > 8)
                {
                    value = 0;
                    return false;
                }

                for (int i = 0; i < source.Length; i++)
                {
                    switch (source[i])
                    {
                        case 'a':
                            result += 10 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'A':
                            result += 10 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'b':
                            result += 11 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'B':
                            result += 11 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'c':
                            result += 12 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'C':
                            result += 12 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'd':
                            result += 13 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'D':
                            result += 13 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'e':
                            result += 14 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'E':
                            result += 14 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'f':
                            result += 15 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        case 'F':
                            result += 15 * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                        default:
                            result += (source[i] - '0') * (int)Math.Pow(16, source.Length - i - 1);
                            break;
                    }
                }

                value = result;
                return true;
            }
            else
            {
                throw new ArgumentException("invalid for given numeral system symbols", nameof(radix));
            }
        }
    }
}
