using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using UnityEngine;

public partial class SROptions
{
    [NumberRange(0, 90)]
    [Category("Jump Skill")]
    public float JumpAngle
    {
        get => optionsImpl.SkillJumpAngle;
        set => optionsImpl.SkillJumpAngle = value;
    }

    [NumberRange(0, 100)]
    [Category("Jump Skill")]
    public float JumpSpeed
    {
        get => optionsImpl.SkillJumpSpeed;
        set => optionsImpl.SkillJumpSpeed = value;
    }  
}
