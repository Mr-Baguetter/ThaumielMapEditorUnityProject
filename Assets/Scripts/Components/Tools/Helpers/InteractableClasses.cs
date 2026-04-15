using System;
using System.Collections.Generic;

namespace Assets.Scripts.Components.Tools.Helpers
{
    [Serializable]
    public class InteractableClasses
    {
        public List<PlayAudio> PlayAudio;
        public List<RunCommand> RunCommand;
        public List<GiveEffect> GiveEffect;
        public List<RemoveEffect> RemoveEffect;
        public List<PlayAnimation> PlayAnimation;
    }
}