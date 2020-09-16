using System.Linq;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.UI;
using Heyworks.PocketShooter.UI.EndBattle;
using Heyworks.PocketShooter.UI.TrooperSelection;
using Heyworks.PocketShooter.UI.Popups;
using Microsoft.Extensions.Logging;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.Integration
{
    public class BasicFlowTest : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            var index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index + 1);

            await UniTask.WaitUntil(() => FindObjectOfType<AndroidTermsPopup>() != null);
            TestsLog.Log.LogTrace("basic_flow_test_pass#1: Android terms PopUp loaded.");

            PressButtonWithName("OkButton", 0.3f);

            await UniTask.WaitUntil(() => FindObjectOfType<LobbyScreen>() != null);
            TestsLog.Log.LogTrace("basic_flow_test_pass#2: lobby screen loaded.");

            PressButtonWithName("Fight", 0.3f);

            await UniTask.WaitUntil(() => FindObjectOfType<TrooperSelectionScreen>() != null);
            TestsLog.Log.LogTrace("basic_flow_test_pass#3: trooper selection screen loaded.");

            PressButtonWithName("UI_Selection_JoinBattle", 0.3f);

            await UniTask.WaitUntil(() => FindObjectsOfType<NetworkCharacter>().Length == 10);
            TestsLog.Log.LogTrace("basic_flow_test_pass#4: 10 troopers joined.");

            await UniTask.WaitUntil(() => FindObjectOfType<EndBattleScreen>() != null);
            TestsLog.Log.LogTrace("basic_flow_test_pass#5: battle ended.");
        }

        private static void PressButtonWithName(string name, float delay)
        {
            var buttons = FindObjectsOfType<Button>();
            var fightButton = buttons.Single(b => b.gameObject.name == name);
            fightButton.Invoke("Press", delay);
        }
    }
}
