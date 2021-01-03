using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

//
// The code here is largely taken from https://github.com/sebastien-riou/aes-brute-force/blob/master/include/aes_ni.h
// 

namespace Benchmarking.Extension.AES
{
    public class BaseAes : Benchmark
    {
        protected int[] cipherText = null!;
        protected Vector128<int>[] loadedEncryptionKey = null!;
        protected int[] plainText = null!;
        
        private byte Shuffle(byte fp3, byte fp2, byte fp1, byte fp0)
        {
            return (byte) ((fp3 << 6) | (fp2 << 4) | (fp1 << 2) | fp0);
        }

        private Vector128<int> ExpandKey(Vector128<int> key, byte control)
        {
            var keyGened = Aes.KeygenAssist(key.As<int, byte>(), control).As<byte, int>();
            keyGened = Sse2.Shuffle(keyGened, Shuffle(3, 3, 3, 3));
            key = Sse2.Xor(key, Sse2.ShiftLeftLogical(key, 4));
            key = Sse2.Xor(key, Sse2.ShiftLeftLogical(key, 4));
            key = Sse2.Xor(key, Sse2.ShiftLeftLogical(key, 4));

            return Sse2.Xor(key, keyGened);
        }

        private Vector128<int>[] LoadKey(int[] encryptionKey)
        {
            var keySchedule = new Vector128<int>[20];

            unsafe
            {
                var keySpan = new Span<int>(encryptionKey);
                fixed (int* keyPointer = keySpan)
                {
                    // Encryption key
                    keySchedule[0] = Sse2.LoadVector128(keyPointer);
                }

                // Encryption key
                keySchedule[1] = ExpandKey(keySchedule[1], 0x01);
                keySchedule[2] = ExpandKey(keySchedule[2], 0x02);
                keySchedule[3] = ExpandKey(keySchedule[3], 0x04);
                keySchedule[4] = ExpandKey(keySchedule[4], 0x08);
                keySchedule[5] = ExpandKey(keySchedule[5], 0x10);
                keySchedule[6] = ExpandKey(keySchedule[6], 0x20);
                keySchedule[7] = ExpandKey(keySchedule[6], 0x40);
                keySchedule[8] = ExpandKey(keySchedule[6], 0x80);
                keySchedule[9] = ExpandKey(keySchedule[6], 0x1B);
                keySchedule[10] = ExpandKey(keySchedule[6], 0x36);

                // Decryption key
                keySchedule[11] = Aes.InverseMixColumns(keySchedule[9].As<int, byte>()).As<byte, int>();
                keySchedule[12] = Aes.InverseMixColumns(keySchedule[8].As<int, byte>()).As<byte, int>();
                keySchedule[13] = Aes.InverseMixColumns(keySchedule[7].As<int, byte>()).As<byte, int>();
                keySchedule[14] = Aes.InverseMixColumns(keySchedule[6].As<int, byte>()).As<byte, int>();
                keySchedule[15] = Aes.InverseMixColumns(keySchedule[5].As<int, byte>()).As<byte, int>();
                keySchedule[16] = Aes.InverseMixColumns(keySchedule[4].As<int, byte>()).As<byte, int>();
                keySchedule[17] = Aes.InverseMixColumns(keySchedule[3].As<int, byte>()).As<byte, int>();
                keySchedule[18] = Aes.InverseMixColumns(keySchedule[2].As<int, byte>()).As<byte, int>();
                keySchedule[19] = Aes.InverseMixColumns(keySchedule[1].As<int, byte>()).As<byte, int>();
            }

            return keySchedule;
        }

        public override void Initialize()
        {
            if (!Aes.IsSupported)
            {
                return;
            }

            var encryptionKey = new[]
                {0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6, 0xab, 0xf7, 0x15, 0x88, 0x09, 0xcf, 0x4f, 0x3c};
            plainText = new[]
                {0x32, 0x43, 0xf6, 0xa8, 0x88, 0x5a, 0x30, 0x8d, 0x31, 0x31, 0x98, 0xa2, 0xe0, 0x37, 0x07, 0x34};
            cipherText = new[]
                {0x39, 0x25, 0x84, 0x1d, 0x02, 0xdc, 0x09, 0xfb, 0xdc, 0x11, 0x85, 0x97, 0x19, 0x6a, 0x0b, 0x32};

            loadedEncryptionKey = LoadKey(encryptionKey);
        }

        public override double GetDataThroughput(ulong iterations)
        {
            return sizeof(int) * 4 * (double) (LENGTH * iterations);
        }
    }
}