package eimu.core;

public abstract class Device {
    public abstract void Initialize();
    public abstract void Shutdown();
    public abstract void SetPauseState(boolean paused);
}
