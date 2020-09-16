using System;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.Popups
{
    public class PopupOptionInfo
    {
        public PopupOptionInfo(string text, Gradient buttonColor = null, Action action = null)
        {
            Text = text;
            ButtonColor = buttonColor;
            Action = action;
        }

        public Action Action { get; }

        public string Text { get; }

        public Gradient ButtonColor { get; }
    }
}