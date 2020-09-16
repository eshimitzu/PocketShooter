public interface ISROptionsImpl
{
    void EnableInterpolator();
    void InterpolateLostTicks();
    void RandomizeWorldTick();
    void RandomizeSimulationTick();
    void AddBot();

    void ShowCheats();
    void NextMatchType();
    void NextMatchMap();
    void DebugUI();

    float SkillJumpAngle { get; set; }
    float SkillJumpSpeed { get; set; }

    void DecreaseMSAA();
    void IncreaseMSAA();
    void DecreaseQuality();
    void IncreaseQuality();
    
    void AccountLevelUp();
    void AddResources();
    void UnlockContent();
}
