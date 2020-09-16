using System;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Localization;

namespace Heyworks.PocketShooter.UI.Lobby.Presenters
{
    public class UpgradeContentCardPresenter : IDisposablePresenter
    {
        private readonly UpgradeContentCard card;
        private readonly IconsFactory sprites;

        public UpgradeContentCardPresenter(UpgradeContentCard card, IconsFactory sprites)
        {
            this.card = card;
            this.sprites = sprites;
        }

        public void Setup(IContentIdentity content, bool isGradeUp)
        {
            if (content is IHasGradeAndLevel gradeAndLevel)
            {
                card.GradeUpView.gameObject.SetActive(isGradeUp);
                card.LevelUpView.gameObject.SetActive(!isGradeUp);

                if (isGradeUp)
                {
                    card.GradeUpView.Setup(gradeAndLevel.Grade - 1, gradeAndLevel.Grade);

                }
                else
                {
                    card.LevelUpView.Setup(gradeAndLevel.Level - 1, gradeAndLevel.Level);
                }
            }
            else
            {
                throw new NotSupportedException($"Can't present content of type {content.GetType()}. Content does not have level and grade.");
            }

            var contentName = LocKeys.GetContentNameKey(content).Localized();
            card.CardNameLabel.text = contentName;
            card.ItemIcon.sprite = sprites.SpriteForItem(content);
        }

        public void Dispose()
        {
        }
    }
}
