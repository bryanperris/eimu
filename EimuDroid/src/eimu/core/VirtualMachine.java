package eimu.core;

public abstract class VirtualMachine {

	private boolean booted = false;
	private Memory memory;
	private RunState runState;
	
	public void setMemory(Memory memory) {
		this.memory = memory;
	}
	public Memory getMemory() {
		return memory;
	}
	public void setRunningState(RunState runningState) {
		this.runningState = runningState;
	}
	public RunState getRunningState() {
		return runningState;
	}
	
	

}
