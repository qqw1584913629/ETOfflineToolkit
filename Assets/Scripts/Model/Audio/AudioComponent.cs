using UnityEngine;

namespace MH
{
    public class AudioComponent : Entity, IAwake
    {
        public AudioSource MusicSource; // 用于播放背景音乐的 AudioSource
        public AudioSource SoundEffectSource; // 用于播放其他音效的 AudioSource
    }
}