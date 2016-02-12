using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LZ4pn;

namespace LZ4UnsafePerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: LZ4UnsafePerformanceTest.exe <file>");
                return;
            }

            string filename = args[0];

            if (!File.Exists(filename))
            {
                Console.WriteLine("{0} doesn't exists.", filename);
                return;
            }

            byte[] buffer = LoadBuffer(filename);

            for (int i = 0; i < 3; i++)
            {
                DoCompressDecompressCycle(buffer);
                Console.WriteLine("====================");
            }
        }

        static void DoCompressDecompressCycle(byte[] buffer)
        {
            byte[] compressed = new byte[LZ4Codec.MaximumOutputLength(buffer.Length)];

            long ini = Environment.TickCount;

            int compressedSize;

            if (IntPtr.Size == 8)
            {
                compressedSize =
                    LZ4Codec.Encode64(buffer, 0, buffer.Length, compressed, 0, compressed.Length);
            }
            else
            {
                compressedSize =
                    LZ4Codec.Encode32(buffer, 0, buffer.Length, compressed, 0, compressed.Length);
            }

            Console.WriteLine("Compressed {0} bytes to {1} bytes in {2} ms.",
                buffer.Length, compressedSize, Environment.TickCount - ini);

            ini = Environment.TickCount;

            int decompressedSize;

            if (IntPtr.Size == 8)
            {
                decompressedSize =
                    LZ4Codec.Decode64(compressed, 0, compressedSize, buffer, 0, buffer.Length, true);
            }
            else
            {
                decompressedSize =
                    LZ4Codec.Decode32(compressed, 0, compressedSize, buffer, 0, buffer.Length, true);
            }

            Console.WriteLine("Decompressed {0} bytes to {1} bytes in {2} ms.",
                compressedSize, decompressedSize, Environment.TickCount - ini);
        }

        static byte[] LoadBuffer(string filename)
        {
            byte[] buffer;

            using (FileStream input = new FileStream(filename, FileMode.Open))
            {
                buffer = new byte[input.Length];
                input.Read(buffer, 0, (int)input.Length);
            }

            return buffer;
        }
    }
}
