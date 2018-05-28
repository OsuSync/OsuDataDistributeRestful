"use strict";

class Smoother{
	constructor(smoothTime,dt)
	{
		this.speed=0;
		this.smoothTime=smoothTime;
		this.dt=dt;
	}
			
	SmoothDamp(previousValue, targetValue)
	{
		let T1 = 0.36 * this.smoothTime;
		let T2 = 0.64 * this.smoothTime;
		let x = previousValue - targetValue;
		let newSpeed = this.speed + this.dt * (-1.0 / (T1 * T2) * x - (T1 + T2) / (T1 * T2) * this.speed);
		let newValue = x + this.dt * this.speed;
		this.speed = newSpeed;
		let result = targetValue + newValue;
		this.speed = isNaN(this.speed) ? 0.0 : this.speed;
		return isNaN(result)?0:result;
	}
}