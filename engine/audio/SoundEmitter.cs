using Microsoft.Xna.Framework.Audio;

using System.IO;

namespace boreal.engine
{
    public class SoundEmitter
    {
        public readonly SoundFile soundFile;

        public SoundState soundState
        {
            get { return XnaSSToASS(); }
        }

        public bool isLooped
        {
            get { return effectInstance.IsLooped; }
            set { effectInstance.IsLooped = value; }
        }

        public float volume
        {
            get { return effectInstance.Volume; }
            set { effectInstance.Volume = value; }
        }

        public float pitch
        {
            get { return effectInstance.Pitch; }
            set { effectInstance.Pitch = value; }
        }

        public float pan
        {
            get { return effectInstance.Pan; }
            set { effectInstance.Pan = value; }
        }

        private SoundEffect soundEffect;
        private SoundEffectInstance effectInstance;

        public SoundEmitter(SoundFile soundFile)
        {
            this.soundFile = soundFile;
            FileInfo fi = new FileInfo(soundFile.path);

            soundEffect = SoundEffect.FromFile(fi.FullName);
            effectInstance = soundEffect.CreateInstance();
        }

        public void Play()
        {
            if (effectInstance.State == Microsoft.Xna.Framework.Audio.SoundState.Playing) effectInstance.Stop();
            effectInstance.Play();
        }


        public void Pause()
        {
            effectInstance.Pause();
        }

        public void Resume()
        {
            effectInstance.Resume();
        }

        public void Stop()
        {
            effectInstance.Stop();
        }

        public void Dispose()
        {
            effectInstance?.Stop();
            effectInstance?.Dispose();

            soundEffect?.Dispose();
        }

        private SoundState XnaSSToASS()
        {
            switch (effectInstance.State)
            {
                case Microsoft.Xna.Framework.Audio.SoundState.Playing:
                    return SoundState.Playing;
                case Microsoft.Xna.Framework.Audio.SoundState.Stopped:
                    return SoundState.Stopped;
                case Microsoft.Xna.Framework.Audio.SoundState.Paused:
                    return SoundState.Paused;
            }
            return SoundState.Stopped;
        }

    }

    public class SoundFile
    {
        public readonly string path;

        public SoundFile(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException(path);

            this.path = path;
        }
    }

    public enum SoundState
    {
        Playing,
        Stopped,
        Paused

    }
}
