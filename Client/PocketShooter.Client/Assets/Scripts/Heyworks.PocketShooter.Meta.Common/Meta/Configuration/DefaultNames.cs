using System;
using System.Collections.Generic;
using System.Linq;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public class DefaultNames
    {
        public static IReadOnlyList<string> PlayerNames { get; } = new HashSet<string>
        {
            { "Jango Fett" },
            { "Yoda" },
            { "Darth Vader" },
            { "Obi-Wan" },
            { "R2-D2" },
            { "C-3PO" },
            { "Chewbacca" },
            { "Palpatine" },
            { "Kate" },
            { "Padme" },
            { "Anakin" },
            { "Mace Windu" },
            { "Boba Fett" },
            { "8D8" },
            { "Mas Amedda" },
            { "Barada" },
            { "Sio Bibble" },
            { "Dengar" },
            { "Lott Dod" },
            { "Kazuda Xiono" },
            { "Eli Vanto" },
            { "Teedo" },
            { "Sana Starros" },
            { "Aurra Sing" },
            { "Rukh" },
            { "Captain Rex" },
            { "Quarrie" },
        }.ToList();
    }
}