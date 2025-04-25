using System;
using UnityEngine;

namespace MH
{
    [Flags]
    public enum SceneType: long
    {
        None,
        Main = 1,
        Current = 1 << 2,
        All = long.MaxValue,
    }
    
    public class Scene : Entity, IUpdate
    {
        public SceneType SceneType;
        public string SceneName;

        public Scene(long id, SceneType sceneType, string name, Entity parent = null)
        {
            this.Id = id;
            this.SceneType = sceneType;
            this.SceneName = name;
            this.Parent = parent;
        }

        public async ETTask Update()
        {
            await ETTask.CompletedTask;
        }
    }
}