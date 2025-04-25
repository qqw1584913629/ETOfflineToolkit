using UnityEngine;

namespace MH
{
    [EntitySystem]
    public class AudioComponentAwakeSystem : AwakeSystem<AudioComponent>
    {
        protected override void Awake(AudioComponent self)
        {
            self.SoundEffectSource = GameObject.Find("/Global/SoundEffectSource").GetComponent<AudioSource>();
            self.MusicSource = GameObject.Find("/Global/MusicSource").GetComponent<AudioSource>();
        }
    }

    public static class AudioComponentSystem
    {
        private static void OnSoundChangeHandler(this AudioComponent self, int value)
        {
            self.SoundEffectSource.volume = value / 100f;
        }

        private static void OnMusicChangeHandler(this AudioComponent self, int value)
        {
            self.MusicSource.volume = value / 100f;
        }

        // 播放背景音乐
        public static void PlayBackgroundMusic(this AudioComponent self, ClipID id)
        {
            var resourcesComponent = self.Root.GetComponent<ResourcesComponent>();
            var clip = resourcesComponent.LoadAssetSync<AudioClip>(id.ToString());
            if (!clip)
                return;
            self.MusicSource.clip = clip;
            self.MusicSource.loop = true;
            self.MusicSource.Play();
        }

        // 播放音效
        public static void PlaySoundEffect(this AudioComponent self, ClipID id)
        {
            var resourcesComponent = self.Root.GetComponent<ResourcesComponent>();
            var clip = resourcesComponent.LoadAssetSync<AudioClip>(id.ToString());
            if (clip)
                self.SoundEffectSource.PlayOneShot(clip);
        }

        // 停止背景音乐
        public static void StopBackgroundMusic(this AudioComponent self)
        {
            self.MusicSource.Stop();
        }
    }
}