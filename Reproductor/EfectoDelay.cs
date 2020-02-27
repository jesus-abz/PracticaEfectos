using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace Reproductor
{
    class EfectoDelay : ISampleProvider
    {
        ISampleProvider fuente;

        public int offsetMiliSegundos;
        List<float> muestras = new List<float>();
        private int tamañoBuffer;

        private int cantidaMuestrasTranscurridas = 0;
        private int cantidadMuestrasBorradas = 0;

        public EfectoDelay(ISampleProvider fuente, int offsetMiliSegundo)
        {
            this.fuente = fuente;
            this.offsetMiliSegundos = offsetMiliSegundo;

            tamañoBuffer = fuente.WaveFormat.SampleRate * 20 * fuente.WaveFormat.Channels;
        }
        public WaveFormat WaveFormat
        {
            get
            {
                return fuente.WaveFormat;
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int read = fuente.Read(buffer, offset, count);



            return read;
        }
    }
}
