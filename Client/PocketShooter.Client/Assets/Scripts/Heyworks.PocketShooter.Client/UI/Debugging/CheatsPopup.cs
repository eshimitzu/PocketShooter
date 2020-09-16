using System.Collections.Generic;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Debugging
{
    public class CheatsPopup : BaseScreen
    {
        private static readonly KeyCode[] Keys =
        {
            KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O,
            KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K,
            KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M,
        };

        [SerializeField]
        private Button cheatButton;

        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private RectTransform table;

        private readonly List<Button> buttons = new List<Button>();
        private RealtimeRunBehavior grb;

        public void Setup(RealtimeRunBehavior grb)
        {
            this.grb = grb;
            closeButton.onClick.AddListener(ClosePopup);
        }

        private void Start()
        {
            var gridLayout = table.GetComponent<GridLayoutGroup>();
            var columns = gridLayout.constraintCount;
            var width = table.rect.width / columns;
            var height = width / 2;
            gridLayout.cellSize = new Vector2(width, height);

            var cheats = EnumUtils.Values<CheatType>();
            foreach (var cheat in cheats)
            {
                Button button = Instantiate(cheatButton, table);
                button.GetComponentInChildren<Text>().text = cheat.ToString();
                button.onClick.AddListener(
                    () => grb._RoomController?.LocalPlayerSimulation?.AddCommand(
                        new CheatCommandData(grb._RoomController.LocalPlayerSimulation.Game.LocalPlayerId, cheat)));
                buttons.Add(button);
            }

            if (columns != 5)
            {
                Debug.LogWarning("Keyboard hotkeys for cheats assume that cheat buttons layout is 5 columns");
            }
        }

        private void Update()
        {
            for (int i = 0; i < Keys.Length; i++)
            {
                if (Input.GetKeyDown(Keys[i]))
                {
                    if (buttons.Count > i)
                    {
                        buttons[i].onClick.Invoke();
                    }
                }
            }
        }

        private void ClosePopup()
        {
            Hide();
        }
    }
}