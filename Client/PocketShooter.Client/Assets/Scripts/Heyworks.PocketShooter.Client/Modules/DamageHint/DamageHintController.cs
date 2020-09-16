using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;
using Lean.Pool;
using UnityEngine;
using Heyworks.PocketShooter.Audio;

namespace Heyworks.PocketShooter.Modules.DamageHints
{
    /// <summary>
    /// Represents an controller for displaying hint information.
    /// </summary>
    public class DamageHintController : MonoBehaviour
    {
        [SerializeField]
        private DamageHint hintPrefab;

        [SerializeField]
        private DamageHintSettings hintSettings;

        private List<DamageHint> hints = new List<DamageHint>();

        /// <summary>
        /// Show hint.
        /// </summary>
        /// <param name="position">position.</param>
        /// <param name="message">message.</param>
        /// <param name="damageType">damageType.</param>
        /// <param name="offsetDirection">offsetDirection.</param>
        public void ShowHint(Vector3 position, string message, DamageType damageType, Vector3 offsetDirection)
        {
            DamageHint hint = PrepareHint(position, message, (offsetDirection * hintSettings.DamageOffsetDistance), damageType);

            switch (damageType)
            {
                case DamageType.Critical:
                    hint.MessageLabel.color = hintSettings.TextCriticalColor;
                    break;
                case DamageType.Extra:
                    hint.MessageLabel.color = hintSettings.TextExtraColor;
                    break;
                default:
                    hint.MessageLabel.color = hintSettings.TextColor;
                    break;
            }

            hint.Show(hints.Count > 0 ? hintSettings.DefaultDelayBetweenHints : 0f);
        }

        /// <summary>
        /// Show healing hint.
        /// </summary>
        /// <param name="position">position.</param>
        /// <param name="message">message.</param>
        /// <param name="offsetDirection">offsetDirection.</param>
        public void ShowHealHint(Vector3 position, string message, Vector3 offsetDirection)
        {
            DamageHint hint = PrepareHint(position, message, (offsetDirection * hintSettings.HealOffsetDistance));
            hint.MessageLabel.color = hintSettings.TextHealColor;
            hint.Show(0f);
        }

        private DamageHint PrepareHint(Vector3 position, string message, Vector3 offset, DamageType damageType = default)
        {
            Vector3 dispersionLocalPosition = transform.InverseTransformPoint(
            position +
            offset +
            (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) *
            hintSettings.SpawnDispersionRadius));

            DamageHint hint = LeanPool.Spawn(hintPrefab, dispersionLocalPosition, Quaternion.identity, transform);

            hint.Setup(message, hintSettings, damageType);

            hint.ShowFinished += Hint_ShowFinished;

            hints.Add(hint);

            return hint;
        }

        private void Hint_ShowFinished(DamageHint hint)
        {
            hints.Remove(hint);
            hint.ShowFinished -= Hint_ShowFinished;
            LeanPool.Despawn(hint);
        }
    }
}