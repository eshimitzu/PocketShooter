using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.UI.Components;
using Heyworks.PocketShooter.Utils;
using Heyworks.PocketShooter.Utils.Unity.EditorTools.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD
{
    /// <summary>
    /// Represents character status bar view.
    /// Visualize health and armor with progress bars.
    /// </summary>
    public class CharacterStatusBarView : MonoBehaviour
    {
        private static readonly StringNumbersCache stringNumbers = new StringNumbersCache(0, 100);

        [SerializeField]
        private RectTransform rectTransform;

        [OptionalProperty]
        [SerializeField]
        private Text nicknameLabel;

        [SerializeField]
        private HealthBar healthBar;

        [SerializeField]
        private HealthBar armorBar;

        [SerializeField]
        private Vector2 eyeLashSize;

        [SerializeField]
        private GameVisualSettings gameVisualSettings;

        private Transform mainCameraTransform;
        private Vector3 localScale;

        private void Start()
        {
            mainCameraTransform = UnityEngine.Camera.main.transform;
            localScale = Vector3.one;
        }

        private void Update()
        {
            float distanceToCamera = Vector3.Distance(mainCameraTransform.position, rectTransform.position);
            distanceToCamera = Mathf.Max(distanceToCamera, 1f);

            localScale.x = distanceToCamera;
            localScale.y = distanceToCamera;
            localScale.z = distanceToCamera;
            rectTransform.localScale = localScale;
        }

        public void Setup(IPlayer player, bool isEnemy)
        {
            healthBar.SetColor(isEnemy ? gameVisualSettings.EnemyColor : gameVisualSettings.FriendColor);

            healthBar.SetEyeLashesForHP(player.Health.MaxHealth, gameVisualSettings.HealthForSegment, eyeLashSize.x, eyeLashSize.y);
            armorBar.SetEyeLashesForHP(player.Armor.MaxArmor, gameVisualSettings.ArmorForSegment, eyeLashSize.x, eyeLashSize.y);

            nicknameLabel.text = player.Nickname;
        }

        public void UpdateStatusBar(IPlayer player)
        {
            healthBar.UpdateProgress(player.Health.Health, player.Health.MaxHealth);

            armorBar.UpdateProgress(player.Armor.Armor, player.Armor.MaxArmor);
        }
    }
}
