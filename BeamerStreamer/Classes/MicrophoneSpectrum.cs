using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NAudio.Wave;
using WPFSoundVisualizationLib;

namespace BeamerStreamer.Classes
{
	public class MicrophoneSpectrum : ISpectrumPlayer
	{
		private WaveIn waveIn;
		private SampleAggregator sampleAggregator;
		public MicrophoneSpectrum()
		{
            sampleAggregator = new SampleAggregator(4096);
            if (WaveIn.DeviceCount > 0)
            {
                var recordingDevices = new ObservableCollection<string>();
                for (int n = 0; n < WaveIn.DeviceCount; n++)
                {
                    recordingDevices.Add(WaveIn.GetCapabilities(n).ProductName);
                }
                
                waveIn = new WaveIn();
                waveIn.DeviceNumber = 0;
                var numberOfBuffers = waveIn.NumberOfBuffers;
                waveIn.WaveFormat = new WaveFormat(16000, 1);
                waveIn.DataAvailable += waveIn_DataAvailable;
                waveIn.StartRecording();
            }

		}

		private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
		{
			byte[] buffer = e.Buffer;

			for (int index = 0; index < e.BytesRecorded; index += 2)
			{
				short sample = (short)((buffer[index + 1] << 8) |
										buffer[index + 0]);
				float sample32 = sample / 32768f;
				sampleAggregator.Add(sample32, 0);
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;
		public bool IsPlaying { get { return true; } }

		public bool GetFFTData(float[] fftDataBuffer)
		{
			sampleAggregator.GetFFTResults(fftDataBuffer);
			return IsPlaying;
		}

		public int GetFFTFrequencyIndex(int frequency)
		{
			double maxFrequency;
			maxFrequency = 22050; // Assume a default 44.1 kHz sample rate.
			return (int)((frequency / maxFrequency) * 2048);
		}
	}
}