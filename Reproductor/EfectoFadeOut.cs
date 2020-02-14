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
        private float inicio;
        private float duracion;

        public EfectoFadeOut(ISampleProvider fuente, float inicio, float duracion)
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
            //proceso - modificacion de los valores de buffer
            muestrasLeidas += read;
            segundosTrancurridos = (float)muestrasLeidas / (float)fuente.WaveFormat.SampleRate / (float)fuente.WaveFormat.Channels;

            if (segundosTrancurridos >= inicio && segundosTrancurridos <= (duracion + inicio))
            {
                //aplicar efecto
                //determinar factor escala
                float factorEscala = 1-((segundosTrancurridos - inicio) / duracion);
                //escalamos muestras
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] *= factorEscala;
                }
            }else if(segundosTrancurridos >= (duracion + inicio))
            {
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] *= 0.0f;
                }
            }

            //variable buffer modificada es la salida
            return read;
        }
    }
}
