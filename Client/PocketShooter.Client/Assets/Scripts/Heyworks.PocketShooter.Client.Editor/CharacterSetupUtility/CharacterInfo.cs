using Heyworks.PocketShooter.PropertyAttributesAndDrawers;
using UnityEngine;

namespace Heyworks.PocketShooter.CharacterSetupUtility
{
    public class CharacterInfo : ScriptableObject
    {
        [SerializeField]
        private TrooperClass trooperClass = TrooperClass.Rambo;

        [SerializeField]
        [GameObjectPath]
        private string fbx;

        [SerializeField]
        [MaterialPath]
        private string material;

        [SerializeField]
        [SpritePath]
        private string rosterIcon;

        [SerializeField]
        [FolderPath]
        private string saveNewCharacterTo;

        [SerializeField]
        [BoolAttribute]
        private bool createNewAnimatorOverrideController;

        [SerializeField]
        [FolderPath]
        private string saveAnimatorOverrideControllerTo;

        [SerializeField]
        [FolderPath]
        private string animationsFolder;

        [SerializeField]
        [OverrideController]
        private string animatorOverrideController;

        public TrooperClass TrooperClass => trooperClass;

        public string FBX => fbx;

        public string Material => material;

        public string RosterIcon => rosterIcon;

        public string SaveNewCharacterTo => saveNewCharacterTo;

        public bool CreateNewAnimatorOverrideController => createNewAnimatorOverrideController;

        public string SaveAnimatorOverrideControllerTo => saveAnimatorOverrideControllerTo;

        public string AnimationsFolder => animationsFolder;

        public string AnimatorOverrideController => animatorOverrideController;
    }
}