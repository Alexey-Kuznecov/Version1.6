
namespace MultiColumns.Sized
{
    using System;

    public enum Unit : long
    {
        Kigabyte = 1048576,

        Megabyte = 1073741824,

        Gigabyte = 1099511627776
    }

    public class ConverterBytes
    {
        public static decimal Total => 0;

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

        public static decimal BytesToKibiBytes(decimal bytes) 
        {
            return bytes / 1024;
        }

        public static decimal BytesToMebiBytes(decimal bytes) 
        {
            return bytes / 1024 / 1024;
        }

        public static decimal BytesToGibiBytes(decimal bytes) 
        {
            return bytes / 1024 / 1024 / 1024;
        }

        public static decimal BytesToKiloBytes(decimal bytes) 
        {
            return bytes / 1000;
        }

        public static decimal BytesToMegaBytes(decimal bytes) 
        {
            return BytesToKiloBytes(bytes) / 1000;
        }
        public static decimal BytesToGigaBytes(decimal bytes) 
        {
            return BytesToKiloBytes(bytes) / 1000;
        }

        #endregion
    }
}
