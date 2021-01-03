using System;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace Benchmarking.Extension.AES
{
    public class Encryption : BaseAes
    {
        public override ulong Run(CancellationToken cancellationToken)
        {
            if (!Aes.IsSupported)
            {
                return 0uL;
            }

            var iterations = 0uL;
            var plainTextSpan = new Span<int>(plainText);
            var cipherTextSpan = new Span<int>(Enumerable.Repeat(0, cipherText.Length).ToArray());

            unsafe
            {
                fixed (int* cipherTextPointer = cipherTextSpan)
                fixed (int* plainTextPointer = plainTextSpan)
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        for (var j = 0; j < LENGTH; j++)
                        {
                            var plainTextVector = Sse2.LoadVector128(plainTextPointer);
                            var workingCopy = Sse2.Xor(plainTextVector, loadedEncryptionKey[0]).As<int, byte>();
                            workingCopy = Aes.Encrypt(workingCopy, loadedEncryptionKey[1].As<int, byte>());
                            workingCopy = Aes.Encrypt(workingCopy, loadedEncryptionKey[2].As<int, byte>());
                            workingCopy = Aes.Encrypt(workingCopy, loadedEncryptionKey[3].As<int, byte>());
                            workingCopy = Aes.Encrypt(workingCopy, loadedEncryptionKey[4].As<int, byte>());
                            workingCopy = Aes.Encrypt(workingCopy, loadedEncryptionKey[5].As<int, byte>());
                            workingCopy = Aes.Encrypt(workingCopy, loadedEncryptionKey[6].As<int, byte>());
                            workingCopy = Aes.Encrypt(workingCopy, loadedEncryptionKey[7].As<int, byte>());
                            workingCopy = Aes.Encrypt(workingCopy, loadedEncryptionKey[8].As<int, byte>());
                            workingCopy = Aes.Encrypt(workingCopy, loadedEncryptionKey[9].As<int, byte>());
                            workingCopy = Aes.EncryptLast(workingCopy, loadedEncryptionKey[10].As<int, byte>());

                            Sse2.Store(cipherTextPointer, workingCopy.As<byte, int>());
                        }

                        iterations++;
                    }
                }
            }

            return iterations;
        }

        public override string GetName()
        {
            return "aes_enc";
        }

        public override string[] GetCategories()
        {
            return new[] {"extension", "aes", "encryption", "all"};
        }

        public override string GetDescription()
        {
            return "AES-NI Benchmark of encrypting a 16 bytes/characters (128-bit) text in ECB mode (insecure)";
        }
    }
}