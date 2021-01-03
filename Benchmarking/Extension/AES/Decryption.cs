using System;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.AES
{
    public class Decryption : BaseAes
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Aes.IsSupported)
            {
                return 0uL;
            }

            var iterations = 0uL;
            var cipherTextSpan = new Span<int>(cipherText);
            var plainTextSpan = new Span<int>(Enumerable.Repeat(0, plainText.Length).ToArray());

            unsafe
            {
                fixed (int* cipherTextPointer = cipherTextSpan)
                fixed (int* plainTextPointer = plainTextSpan)
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        for (var j = 0; j < LENGTH; j++)
                        {
                            var cipherTextVector = Sse2.LoadVector128(cipherTextPointer);
                            var workingCopy = Sse2.Xor(cipherTextVector, loadedEncryptionKey[10]).As<int, byte>();
                            workingCopy = Aes.Decrypt(workingCopy, loadedEncryptionKey[11].As<int, byte>());
                            workingCopy = Aes.Decrypt(workingCopy, loadedEncryptionKey[12].As<int, byte>());
                            workingCopy = Aes.Decrypt(workingCopy, loadedEncryptionKey[13].As<int, byte>());
                            workingCopy = Aes.Decrypt(workingCopy, loadedEncryptionKey[14].As<int, byte>());
                            workingCopy = Aes.Decrypt(workingCopy, loadedEncryptionKey[15].As<int, byte>());
                            workingCopy = Aes.Decrypt(workingCopy, loadedEncryptionKey[16].As<int, byte>());
                            workingCopy = Aes.Decrypt(workingCopy, loadedEncryptionKey[17].As<int, byte>());
                            workingCopy = Aes.Decrypt(workingCopy, loadedEncryptionKey[18].As<int, byte>());
                            workingCopy = Aes.Decrypt(workingCopy, loadedEncryptionKey[19].As<int, byte>());
                            workingCopy = Aes.DecryptLast(workingCopy, loadedEncryptionKey[0].As<int, byte>());

                            Sse2.Store(plainTextPointer, workingCopy.As<byte, int>());
                        }

                        iterations++;
                    }
                }
            }

            return iterations;
        }

        public override string GetName()
        {
            return "aes_dec";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "aes", "decryption", "all"};
        }

        public override string GetDescription()
        {
            return "AES-NI Benchmark of decrypting a 16 bytes/characters (128-bit) text in ECB mode (insecure)";
        }
    }
}