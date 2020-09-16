using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Localization;

namespace Heyworks.PocketShooter.UI.Lobby.Presenters
{
    public class AcquireConfirmationCardPresenter : IDisposablePresenter
    {
        private readonly AcquiredContentCard card;
        private readonly IconsFactory sprites;

        public AcquireConfirmationCardPresenter(AcquiredContentCard card, IconsFactory sprites)
        {
            this.card = card;
            this.sprites = sprites;
        }

        public void Setup(IContentIdentity content)
        {
            var contentName = LocKeys.GetContentNameKey(content).Localized();
            card.CardNameLabel.text = contentName;
            card.ItemIcon.sprite = sprites.SpriteForItem(content);

            card.ResourceRoot.SetActive(false);
            card.ItemRoot.gameObject.SetActive(false);

            if (content is IHasGradeAndLevel gradeAndLevel)
            {
                card.ItemRoot.gameObject.SetActive(true);
                card.LevelAndGradeView.Setup(gradeAndLevel.Level, (int)gradeAndLevel.Grade);
            }

            if (content is ResourceIdentity resourceIdentity)
            {
                card.ResourceRoot.SetActive(true);
                card.ResourceView.Setup(resourceIdentity.Gold, resourceIdentity.Cash);
            }

            if (content is ICountable countableIdentity)
            {
                card.ResourceRoot.SetActive(true);
                card.ResourceView.Setup(countableIdentity.Count);
            }
        }

        public void Dispose()
        {
        }
    }
}
