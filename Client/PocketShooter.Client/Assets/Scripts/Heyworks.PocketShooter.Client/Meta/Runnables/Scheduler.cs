using System.Collections.Generic;
using Heyworks.PocketShooter.Utils;

namespace Heyworks.PocketShooter.Meta.Runnables
{
    /// <summary>
    /// Represents scheduler for all scheduled items.
    /// </summary>
    public sealed class Scheduler
    {
        private readonly List<IHasRunnables> entities = new List<IHasRunnables>();
        private readonly IMetronome metronome;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="metronome"> Object which produces regular, metrical ticks .</param>
        public Scheduler(IMetronome metronome)
        {
            this.metronome = metronome;
            metronome.Tick += Metronome_Tick;
        }

        /// <summary>
        /// Remove all scheduled entities from the collection.
        /// </summary>
        public void Clear()
        {
            entities.Clear();
            metronome.Tick -= Metronome_Tick;
        }

        /// <summary>
        /// Register entity an entity which encapsulates elements, implementing <see cref="IRunnable"/> interface.
        /// </summary>
        /// <param name="entity"> Object containing <see cref="IRunnable"/> entities. </param>
        public void Register(IHasRunnables entity)
        {
            AssertUtils.NotNull(entity, "entity");

            entities.Add(entity);
        }

        private void CheckRunnables()
        {
            for (var i = 0; i < entities.Count; i++)
            {
                var hasRunnablese = entities[i];
                var runnables = hasRunnablese.GetRunnables();

                foreach (var runnable in runnables)
                {
                    if (runnable.IsFinished)
                    {
                        runnable.Finish();
                    }
                }
            }
        }

        private void Metronome_Tick()
        {
            CheckRunnables();
        }
    }
}
