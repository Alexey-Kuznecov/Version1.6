
namespace AlexeyKuznecov.Library.Converters
{
    using System;

    /// <summary>
    /// The unit.
    /// </summary>
    public enum Unit : long
    {
        /// <summary>
        /// The kigabyte.
        /// </summary>
        Kigabyte = 1048576,

        /// <summary>
        /// The megabyte.
        /// </summary>
        Megabyte = 1073741824,

        /// <summary>
        /// The gigabyte.
        /// </summary>
        Gigabyte = 1099511627776
    }

    /// <summary>
    /// Convert bytes to KiB, MiB, GiB, kB, MB, GB
    /// </summary>
    public class ConverterBytes
    {
        /// <summary>
        /// Gets the total.
        /// </summary>
        public static decimal Total => 0;

        /// <summary>
        /// Automatically detects and formats bytes into other units.
        /// </summary>
        /// <param name="bytes"> Initial bytes. </param>
        /// <returns> The value is in the form of KB MB or GB. </returns>
        public static string AutoConvertFormatBytes(decimal bytes)
        {
            if (bytes < 1024)
            {
                return $"{bytes} b";
            }
            else if (bytes < 1048576)
            {
                return $"{BytesToKibiBytes(bytes):f2} Kb";
            }
            else if (bytes < 1073741824)
            {
                return $"{BytesToMebiBytes(bytes):f2} Mb";
            }
            else if (bytes < 1099511627776)
            {
                return $"{BytesToGibiBytes(bytes):f2} Gb";
            }
            else
            {
                return $"{BytesToGibiBytes(bytes) / 1024:f2} Tb";
            }
        }

        /// <summary>
        /// Automatically detects and converts bytes into other units.
        /// </summary>
        /// <param name="bytes"> Initial bytes. </param>
        /// <returns> The value is in the form of KB MB or GB. </returns>
        public static decimal AutoConvertBytes(decimal bytes)
        {
            if (bytes < 1024)
            {
                return bytes;
            }
            else if (bytes < 1048576)
            {
                return BytesToKibiBytes(bytes);
            }
            else if (bytes < 1073741824)
            {
                return BytesToMebiBytes(bytes);
            }
            else if (bytes < 1099511627776)
            {
                return BytesToGibiBytes(bytes);
            }
            else
            {
                return BytesToGibiBytes(bytes) / 1024;
            }
        }

        /// <summary>
        /// Converts a byte to other units such as KB, MB, or GB.
        /// </summary>
        /// <param name="bytes"> Initial bytes. </param>
        /// <param name="unitMeasure"> Select unit of measurement. </param>
        /// <returns> The value is in the form of KB MB or GB. </returns>
        public static decimal ConvertBytesTo(decimal bytes, Unit unitMeasure)
        {
            switch (unitMeasure)
            {
                case Unit.Kigabyte:
                    return BytesToKibiBytes(bytes);
                case Unit.Megabyte:
                    return BytesToMebiBytes(bytes);
                default:
                    return BytesToGibiBytes(bytes);
            }
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="bytes"> The bytes. </param>
        public static void Show(decimal bytes)
        {
            Console.WriteLine("Units established by the International Electrotechnical Commission (IEC) in 1998");
            Console.WriteLine("{0} Bytes => {1:f2} kiB -> JEDEC Standards KB - used in Microsoft Windows", bytes, BytesToKibiBytes(bytes));
            Console.WriteLine("{0} Bytes => {1:f2} MiB -> JEDEC Standards MB - used in Microsoft Windows", bytes, BytesToMebiBytes(bytes));
            Console.WriteLine("{0} Bytes => {1:f2} GiB -> JEDEC Standards GB - used in Microsoft Windows", bytes, BytesToGibiBytes(bytes));

            Console.WriteLine();

            Console.WriteLine("By International System of Units (SI):");
            Console.WriteLine(" kilo = 1 000,");
            Console.WriteLine(" mega = 1 000 000,");
            Console.WriteLine(" giga = 1 000 000 000.");

            Console.WriteLine();

            Console.WriteLine("{0} Bytes => {1:f2} kB", bytes, BytesToKiloBytes(bytes));
            Console.WriteLine("{0} Bytes => {1:f2} MB", bytes, BytesToMegaBytes(bytes));
            Console.WriteLine("{0} Bytes => {1:f2} GB", bytes, BytesToGigaBytes(bytes));

            Console.ReadKey();
        }

        #region Convrters

        /// <summary>
        /// Convert bytes to <c>kibi</c> bytes.
        /// </summary>
        /// <param name="bytes"> The bytes. </param>
        /// <returns> The <see cref="decimal"/>. </returns>
        public static decimal BytesToKibiBytes(decimal bytes) 
        {
            return bytes / 1024;
        }

        /// <summary>
        /// Convert bytes to <c>mebi</c> bytes.
        /// </summary>
        /// <param name="bytes"> The bytes. </param>
        /// <returns> The <see cref="decimal"/>. </returns>
        public static decimal BytesToMebiBytes(decimal bytes) 
        {
            return bytes / 1024 / 1024;
        }

        /// <summary>
        /// Convert bytes to <c>gibi</c> bytes.
        /// </summary>
        /// <param name="bytes"> The bytes. </param>
        /// <returns> The <see cref="decimal"/>. </returns>
        public static decimal BytesToGibiBytes(decimal bytes) 
        {
            return bytes / 1024 / 1024 / 1024;
        }

        /// <summary>
        /// KiloBytes => kB (k in lowercase! K in upper case means Kelvin)
        /// </summary>
        /// <param name="bytes"> The bytes. </param>
        /// <returns> The <see cref="decimal"/>. </returns>
        public static decimal BytesToKiloBytes(decimal bytes) 
        {
            return bytes / 1000;
        }

        /// <summary>
        /// Convert bytes to mega bytes.
        /// </summary>
        /// <param name="bytes"> The bytes. </param>
        /// <returns> The <see cref="decimal"/>. </returns>
        public static decimal BytesToMegaBytes(decimal bytes) 
        {
            return BytesToKiloBytes(bytes) / 1000;
        }

        /// <summary>
        /// Convert bytes to giga bytes.
        /// </summary>
        /// <param name="bytes"> The bytes. </param>
        /// <returns> The <see cref="decimal"/>. </returns>
        public static decimal BytesToGigaBytes(decimal bytes) 
        {
            return BytesToKiloBytes(bytes) / 1000;
        }

        #endregion
    }
}
