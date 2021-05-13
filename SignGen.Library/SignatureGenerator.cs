using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SignGen.Library
{
    public class SignatureGenerator : IDisposable
    {
        #region Fields

        private readonly TextReader input;
        private readonly TextWriter output;

        private readonly string path;
        private readonly int blockSize;

        private bool disposedValue;

        #endregion

        #region Constructor
        public SignatureGenerator(TextReader input, TextWriter output) : this(input, output, 4096) { }
        public SignatureGenerator(TextReader input, TextWriter output, int blockSize)
        {
            this.input = input;
            this.output = output;
            this.blockSize = blockSize;
        }

        #endregion

        public void Start()
        {
            throw new NotImplementedException();
        }




















        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~MultithreadedSignatureGenerator()
        // {
        //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
