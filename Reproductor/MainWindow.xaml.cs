using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using System.Windows.Threading;

namespace Reproductor
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;

        //lector de archivos
        AudioFileReader reader;
        //comunicacion con el equipo de audio
        WaveOut output;

        bool dragging = false;
        //VolumeSampleProvider volume;
        EfectoVolumen efectoVolumen;
        EfectoFadeIn efectoFadeIn;
        EfectoFadeOut efectoFadeOut;

        public MainWindow()
        {
            InitializeComponent();
            ListaDispositivosDeSalida();
            btnReproducir.IsEnabled = false;
            btnPausa.IsEnabled = false;
            btnDetener.IsEnabled = false;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (efectoFadeIn != null)
            {
                //lblMuestras.Text = efectoFadeIn.segundosTrancurridos.ToString();

            }

            lblTiempoActual.Text = reader.CurrentTime.ToString().Substring(0, 8);

            if(!dragging)
            {
                sldTiempo.Value = reader.CurrentTime.TotalSeconds;
            }
        }

        void ListaDispositivosDeSalida()
        {
            cbDispositivoSalida.Items.Clear();
            for(int i=0; i < WaveOut.DeviceCount; i++)
            {
                WaveOutCapabilities capacidades = WaveOut.GetCapabilities(i);
                cbDispositivoSalida.Items.Add(capacidades.ProductName);
            }
            cbDispositivoSalida.SelectedIndex = 0;
        }

        private void btnExaminar_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtRutaArchivo.Text = openFileDialog.FileName;
                btnReproducir.IsEnabled = true;
            }
        }

        private void cbDispositivoSalida_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnReproducir_Click(object sender, RoutedEventArgs e)
        {
            if(output != null && output.PlaybackState == PlaybackState.Paused)
            {
                //retomo la reproduccion
                output.Play();
                btnReproducir.IsEnabled = false;
                btnPausa.IsEnabled = true;
                btnDetener.IsEnabled = true;
            }
            else
            {
                if (txtRutaArchivo.Text != null && txtRutaArchivo.Text != string.Empty)
                {
                    reader = new AudioFileReader(txtRutaArchivo.Text);
                    /*volume = new VolumeSampleProvider(reader);
                    volume.Volume = (float)(sldVolumen.Value);*/

                    float duracionFadeIn = float.Parse(txtDuracionFadeIn.Text);
                    efectoFadeIn = new EfectoFadeIn(reader, duracionFadeIn);

                    efectoVolumen = new EfectoVolumen(efectoFadeIn);
                    efectoVolumen.Volumen = (float)(sldVolumen.Value);

                    int inicioFadeOut = int.Parse(txtInicioFadeOut.Text);
                    float duracionFadeOut = float.Parse(txtDuracionFadeOut.Text);
                    efectoFadeOut = new EfectoFadeOut(reader, inicioFadeOut, duracionFadeOut);

                    efectoVolumen = new EfectoVolumen(efectoFadeOut);

                    output = new WaveOut();
                    output.DeviceNumber = cbDispositivoSalida.SelectedIndex;
                    output.PlaybackStopped += Output_PlaybackStopped;
                    output.Init(efectoVolumen);
                    output.Play(); 

                    //cambiar el volumen del output
                    //output.Volume = (float)(sldVolumen.Value);

                    btnReproducir.IsEnabled = false;
                    btnPausa.IsEnabled = true;
                    btnDetener.IsEnabled = true;

                    lblTiempoTotal.Text = reader.TotalTime.ToString().Substring(0, 8);
                    lblTiempoActual.Text = reader.CurrentTime.ToString().Substring(0, 8);

                    sldTiempo.Maximum = reader.TotalTime.TotalSeconds;
                    sldTiempo.Value = reader.CurrentTime.TotalSeconds;

                    timer.Start();
                }
            }
        }

        private void Output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            timer.Stop();
            reader.Dispose();
            output.Dispose();
        }

        private void btnDetener_Click(object sender, RoutedEventArgs e)
        {
            if(output != null)
            {
                output.Stop();
                btnReproducir.IsEnabled = true;
                btnPausa.IsEnabled = false;
                btnDetener.IsEnabled = false;
            }
        }

        private void btnPausa_Click(object sender, RoutedEventArgs e)
        {
            if (output != null)
            {
                output.Pause();

                btnReproducir.IsEnabled = true;
                btnPausa.IsEnabled = false;
                btnDetener.IsEnabled = true;
            }
        }

        private void sldTiempo_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragging = true;
        }

        private void sldTiempo_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragging = false;
            if (reader !=null && output != null && output.PlaybackState != PlaybackState.Stopped)
            {
                reader.CurrentTime = TimeSpan.FromSeconds(sldTiempo.Value);
            }
        }

        private void sldVolumen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (output != null && output.PlaybackState != PlaybackState.Stopped)
            {
                efectoVolumen.Volumen = (float)(sldVolumen.Value);

                //output.Volume = (float)(sldVolumen.Value);
                //volume.Volume = (float)(sldVolumen.Value);
            }
        }
    }
}
