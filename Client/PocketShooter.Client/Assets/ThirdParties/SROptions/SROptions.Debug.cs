using System;
using System.ComponentModel;
using UnityEngine;

public partial class SROptions
{
    private ISROptionsImpl optionsImpl;

    public void SetOptionsImpl(ISROptionsImpl impl)
    {
        this.optionsImpl = impl;
    }

    [Category("Bots")]
    public void AddBot()
    {
        optionsImpl.AddBot();
    }

    [Category("Utilities")]
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [Category("Utilities")]
    public void ShowCheats()
    {
        optionsImpl.ShowCheats();
    }

    [Category("Utilities")]
    public void NextMatchType()
    {
        optionsImpl.NextMatchType();
    }

    [Category("Utilities")]
    public void NextMatchMap()
    {
        optionsImpl.NextMatchMap();
    }

    [Category("Utilities")]
    public void DebugUI()
    {
        optionsImpl.DebugUI();
    }

    [Category("Interpolation")]
    public void EnableInterpolator()
    {
        optionsImpl.EnableInterpolator();
    }

    [Category("Interpolation")]
    public void InterpolateLostTicks()
    {
        optionsImpl.InterpolateLostTicks();
    }

    [Category("Time")]
    public void RandomizeWorldTick()
    {
        optionsImpl.RandomizeWorldTick();
    }

    [Category("Time")]
    public void RandomizeSimulationTick()
    {
        optionsImpl.RandomizeSimulationTick();
    }
    
    [Category("Graphics")]
    public void DecreaseMSAA()
    {
        optionsImpl.DecreaseMSAA();
    }
    
    [Category("Graphics")]
    public void IncreaseMSAA()
    {
        optionsImpl.IncreaseMSAA();
    }
    
    [Category("Graphics")]
    public void DecreaseQuality()
    {
        optionsImpl.DecreaseQuality();
    }
    
    [Category("Graphics")]
    public void IncreaseQuality()
    {
        optionsImpl.IncreaseQuality();
    }
    
    [Category("Cheats")]
    public void AccountLevelUp()
    {
        optionsImpl.AccountLevelUp();
    }
    
    [Category("Cheats")]
    public void AddResources()
    {
        optionsImpl.AddResources();
    }
    
    [Category("Cheats")]
    public void UnlockContent()
    {
        optionsImpl.UnlockContent();
    }
}
