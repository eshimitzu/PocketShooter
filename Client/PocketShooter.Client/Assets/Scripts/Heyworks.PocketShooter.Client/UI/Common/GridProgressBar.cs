using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Heyworks.PocketShooter.UI.Common
{
    /// <summary>
    /// Represents GridProgressBar.
    /// </summary>
    public class GridProgressBar : MonoBehaviour
    {
        [SerializeField]
        private Image[] cells;

        /// <summary>
        /// Gets progress bar value.
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// Hard setup progress value (update immediatly).
        /// </summary>
        /// <param name="value"> The progress value (0-1). </param>
        public void HardUpdateValue(float value)
        {
            Mathf.Clamp(value, 0f, 1f);
            Value = value;
            UpdateRepresentation(Value);
        }

        private void UpdateRepresentation(float fillAmount)
        {
            var requireEnabledCellsCount = Math.Round(fillAmount * cells.Length, MidpointRounding.AwayFromZero);
            for (int i = 0; i < cells.Length; i++)
            {
                if (i >= requireEnabledCellsCount)
                {
                    cells[i].gameObject.SetActive(false);
                }
                else
                {
                    cells[i].gameObject.SetActive(true);
                }
            }
        }

        [ContextMenu("Set MAX")]
        private void SetValueMax()
        {
            HardUpdateValue(1f);
        }

        [ContextMenu("Set MIN")]
        private void SetValueMin()
        {
            HardUpdateValue(0f);
        }

        [ContextMenu("Set RANDOM")]
        private void SetValueRandom()
        {
            HardUpdateValue(Random.Range(0f, 1f));
        }
    }
}