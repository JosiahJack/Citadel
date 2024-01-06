using System.IO;
using UnityEngine;
using NAudio.Wave;

//get NAudio dll:           https://www.dllme.com/getfile.php?file=26281&id=c305a9e16a5c04eb543c6332ac4d9036
//example NAudio script:    https://www.dropbox.com/s/wks0ujanr0pm6nj/NAudioPlayer.cs?dl=0 

public static class NAudioPlayer 
{
    #region --- helper ---
    public class WAV
    {
        public float[] LeftChannel { get; internal set; }
        public float[] RightChannel { get; internal set; }
        public float[] StereoChannel { get; internal set; }
        public int ChannelCount { get; internal set; }
        public int SampleCount { get; internal set; }
        public int Frequency { get; internal set; }

        private static float bytesToFloat(byte firstByte, byte secondByte)
        {
            short s = (short)((secondByte << 8) | firstByte);
            return s / 32768.0F;
        }
        private static int bytesToInt(byte[] bytes, int offset = 0)
        {
            int value = 0;
            for (int i = 0; i < 4; i++)
            {
                value |= ((int)bytes[offset + i]) << (i * 8);
            }
            return value;
        }
        public WAV(byte[] wav)
        {
            ChannelCount = wav[22];
            Frequency = bytesToInt(wav, 24);
            int pos = 12; 
            while (!(wav[pos] == 100 && wav[pos + 1] == 97 && wav[pos + 2] == 116 && wav[pos + 3] == 97))
            {
                pos += 4;
                int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
                pos += 4 + chunkSize;
            }
            pos += 8;
            SampleCount = (wav.Length - pos) / 2;
            if (ChannelCount == 2)
                SampleCount /= 2;
            LeftChannel = new float[SampleCount];
            if (ChannelCount == 2)
                RightChannel = new float[SampleCount];
            else
                RightChannel = null;

            int i = 0;
            while (pos < wav.Length)
            {
                LeftChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
                if (ChannelCount == 2)
                {
                    RightChannel[i] = bytesToFloat(wav[pos], wav[pos + 1]);
                    pos += 2;
                }
                i++;
            }

            if (ChannelCount == 2)
            {
                StereoChannel = new float[SampleCount * 2];
                int channelPos = 0;
                short posChange = 0;
                for (int index = 0; index < (SampleCount * 2); index++)
                {
                    if (index % 2 == 0)
                    {
                        StereoChannel[index] = LeftChannel[channelPos];
                        posChange++;
                    }
                    else
                    {
                        StereoChannel[index] = RightChannel[channelPos];
                        posChange++;
                    }
                    if (posChange % 2 == 0)
                    {
                        if (channelPos < SampleCount)
                        {
                            channelPos++;
                            posChange = 0;
                        }
                    }
                }
            }
            else
            {
                StereoChannel = null;
            }
        }
        public override string ToString()
        {
            return string.Format("[WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}]", LeftChannel, RightChannel, ChannelCount, SampleCount, Frequency);
        }
    }
    #endregion

    private static MemoryStream AudioMemStream(WaveStream wavestream)
    {
        MemoryStream outputstream = new MemoryStream();
        using (WaveFileWriter wavefilewriter = new WaveFileWriter(outputstream, wavestream.WaveFormat))
        {
            byte[] bytes = new byte[wavestream.Length];
            wavestream.Position = 0;
            wavestream.Read(bytes, 0, System.Convert.ToInt32(wavestream.Length));
            wavefilewriter.Write(bytes, 0, bytes.Length);
            wavefilewriter.Flush();
        }
        return outputstream;
    }
    public static AudioClip FromMp3Data(byte[] data)
    {
        MemoryStream mp3stream = new MemoryStream(data);
        Mp3FileReader mp3audio = new Mp3FileReader(mp3stream);
        WaveStream wavestream = WaveFormatConversionStream.CreatePcmStream(mp3audio);
        WAV wav = new WAV(AudioMemStream(wavestream).ToArray());

        AudioClip audioclip = null;
        if (wav.ChannelCount == 2)
        {
            audioclip = AudioClip.Create("audio file name", wav.SampleCount, 2, wav.Frequency, false);
            audioclip.SetData(wav.StereoChannel, 0);
        }
        else
        {
            audioclip = AudioClip.Create("audio file name", wav.SampleCount, 1, wav.Frequency, false);
            audioclip.SetData(wav.LeftChannel, 0);
        }

        return audioclip;
    }
}
