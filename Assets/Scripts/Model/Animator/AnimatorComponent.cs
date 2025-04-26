using System.Collections.Generic;
using UnityEngine;

namespace MH
{
    public enum MotionType
    {
        None,
        Idle,
        Run,
    }
    public class AnimatorComponent : Entity, IAwake, IUpdate, IDestroy
    {
        public Dictionary<string, AnimationClip> animationClips = new();
        public HashSet<string> Parameter = new();

        public MotionType MotionType;
        public float MontionSpeed;
        public bool isStop;
        public float stopSpeed;
        public Animator Animator;

        public AnimationEventBridge EventBridge { get; set; }
    }
}