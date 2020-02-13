using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace Reproductor
{
    class EfectoFadeOut : ISampleProvider
    {
        private ISampleProvider fuente;
        private int muestrasLeidas = 0;
        private float segundosTrancurridos = 0;
        private int inicio;
        private float duracion;

        public EfectoFadeOut(ISampleProvider fuente, int inicio, float duracion)
        {
            this.fuente = fuente;
            this.inicio = inicio;
            this.duracion = duracion;
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

            //aplicar el efecto
            muestrasLeidas += read;
            segundosTrancurridos = (float)muestrasLeidas / (float)fuente.WaveFormat.SampleRate / (float)fuente.WaveFormat.Channels;

            if (segundosTrancurridos >= inicio && segundosTrancurridos <= duracion)
            {
                //aplicar efecto
                float factorEscala = segundosTrancurridos / duracion;
                for (int i = inicio; i < read; i++)
                {
                    buffer[i + offset] *= factorEscala;
                }
            }

            return read;
        }
    }
}
