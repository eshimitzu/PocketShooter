using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskDescription(@"Repeat with interval")]
    [TaskIcon("{SkinColor}RepeaterIcon.png")]
    public class IntervalRepeater : Repeater
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The number of times to repeat the execution of its child task")]
        public SharedFloat interval = 1f;

        private float lastTime = 0;

        public override bool CanExecute()
        {
            bool ready = (Time.time - lastTime) > interval.Value;
            var execute = ready && base.CanExecute();

            if (execute)
            {
                lastTime = Time.time;
            }

            return execute;
        }
    }
}