using UnityEngine;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyRosterView : MonoBehaviour
    {
        [SerializeField]
        private LobbyRosterCard cardPrefab;

        [SerializeField]
        private LobbyRosterEmptyCard fakeCardPrefab;

        [SerializeField]
        private RectTransform contentRoot;

        [SerializeField]
        private RosterScroll rosterScroll;

        public LobbyRosterCard CardPrefab => cardPrefab;

        public LobbyRosterEmptyCard FakeCardPrefab => fakeCardPrefab;

        public RectTransform ContentRoot => contentRoot;

        public RosterScroll RosterScroll => rosterScroll;
    }
}