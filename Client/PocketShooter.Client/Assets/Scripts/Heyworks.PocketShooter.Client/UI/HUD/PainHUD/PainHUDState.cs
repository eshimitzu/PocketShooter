using System.Collections.Generic;

namespace Heyworks.PocketShooter.UI.HUD.PainHUD
{
    /// <summary>
    /// The state of the pain HUD elements.
    /// </summary>
    public class PainHUDState
    {
        /// <summary>
        /// Represents damage indicator state.
        /// </summary>
        public struct Indicator
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Indicator"/> struct.
            /// </summary>
            /// <param name="angle">Rotation angle of the indicato.</param>
            /// <param name="scale">Scale of the indicator.</param>
            /// <param name="alpha">Color alpha of the indicator.</param>
            public Indicator(float angle, float scale, float alpha)
            {
                Angle = angle;
                Scale = scale;
                Alpha = alpha;
            }

            /// <summary>
            /// Gets the rotation angle of the damage indicator.
            /// </summary>
            public float Angle { get; }

            /// <summary>
            /// Gets the scale of the damage indicator.
            /// </summary>
            public float Scale { get; }

            /// <summary>
            /// Gets the alpha of the color of the damage indicator.
            /// </summary>
            public float Alpha { get; }
        }

        /// <summary>
        /// Gets the damage indicators.
        /// </summary>
        public List<Indicator> Indicators { get; } = new List<Indicator>();

        /// <summary>
        /// Gets or sets the damage splash color.
        /// </summary>
        public float DamageSplashColorAlpha { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets the death splash color.
        /// </summary>
        public bool DeathSplashColorEnabled { get; set; }
    }
}