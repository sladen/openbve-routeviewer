﻿using System;

namespace OpenBve {
	internal static class FunctionScripts {

		// instruction set
		internal enum Instructions : int {
			SystemHalt, SystemConstant, SystemConstantArray, SystemValue, SystemDelta,
			StackCopy, StackSwap,
			MathPlus, MathSubtract, MathMinus, MathTimes, MathDivide, MathReciprocal, MathPower,
			MathIncrement, MathDecrement, MathFusedMultiplyAdd,
			MathQuotient, MathMod, MathFloor, MathCeiling, MathRound, MathMin, MathMax, MathAbs, MathSign,
			MathExp, MathLog, MathSqrt, MathSin, MathCos, MathTan, MathArcTan,
			CompareEqual, CompareUnequal, CompareLess, CompareGreater, CompareLessEqual, CompareGreaterEqual, CompareConditional,
			LogicalNot, LogicalAnd, LogicalOr, LogicalNand, LogicalNor, LogicalXor,
			TimeSecondsSinceMidnight, CameraDistance,
			TrainCars, TrainSpeed, TrainSpeedometer,
			TrainDistance, TrainDistanceToCar, TrainTrackDistance, TrainTrackDistanceToCar,
			Doors, DoorsIndex,
			LeftDoors, LeftDoorsIndex, RightDoors, RightDoorsIndex,
			LeftDoorsTarget, LeftDoorsTargetIndex, RightDoorsTarget, RightDoorsTargetIndex,
			ReverserNotch, PowerNotch, PowerNotches, BrakeNotch, BrakeNotches, BrakeNotchLinear, BrakeNotchesLinear, EmergencyBrake,
			HasAirBrake, HoldBrake, HasHoldBrake, ConstSpeed, HasConstSpeed,
			BrakeMainReservoir, BrakeEqualizingReservoir, BrakeBrakePipe, BrakeBrakeCylinder, StraightAirPipe,
			SecurityPluginState,
			TimetableVisible,
			SectionAspectNumber
		}

		// function script
		internal class FunctionScript {
			internal Instructions[] Instructions;
			internal double[] Stack;
			internal double[] Constants;
			internal double LastResult;
			internal double Perform(TrainManager.Train Train, World.Vector3D Position, double TrackPosition, int SectionIndex, double TimeElapsed) {
				ExecuteFunctionScript(this, Train, Position, TrackPosition, SectionIndex, TimeElapsed);
				return this.LastResult;
			}
			internal FunctionScript Clone() {
				return (FunctionScript)this.MemberwiseClone();
			}
		}

		// execute function script
		private static void ExecuteFunctionScript(FunctionScript Function, TrainManager.Train Train, World.Vector3D Position, double TrackPosition, int SectionIndex, double TimeElapsed) {
			int s = 0, c = 0;
			for (int i = 0; i < Function.Instructions.Length; i++) {
				switch (Function.Instructions[i]) {
						// system
					case Instructions.SystemHalt:
						i = Function.Instructions.Length;
						break;
					case Instructions.SystemConstant:
						Function.Stack[s] = Function.Constants[c];
						s++; c++; break;
					case Instructions.SystemConstantArray:
						{
							int n = (int)Function.Instructions[i + 1];
							for (int j = 0; j < n; j++) {
								Function.Stack[s + j] = Function.Constants[c + j];
							} s += n; c += n; i++;
						} break;
					case Instructions.SystemValue:
						Function.Stack[s] = Function.LastResult;
						s++; break;
					case Instructions.SystemDelta:
						Function.Stack[s] = TimeElapsed;
						s++; break;
						// stack
					case Instructions.StackCopy:
						Function.Stack[s] = Function.Stack[s - 1];
						s++; break;
					case Instructions.StackSwap:
						{
							double a = Function.Stack[s - 1];
							Function.Stack[s - 1] = Function.Stack[s - 2];
							Function.Stack[s - 2] = a;
						} break;
						// math
					case Instructions.MathPlus:
						Function.Stack[s - 2] += Function.Stack[s - 1];
						s--; break;
					case Instructions.MathSubtract:
						Function.Stack[s - 2] -= Function.Stack[s - 1];
						s--; break;
					case Instructions.MathMinus:
						Function.Stack[s - 1] = -Function.Stack[s - 1];
						break;
					case Instructions.MathTimes:
						Function.Stack[s - 2] *= Function.Stack[s - 1];
						s--; break;
					case Instructions.MathDivide:
						Function.Stack[s - 2] = Function.Stack[s - 1] == 0.0 ? 0.0 : Function.Stack[s - 2] / Function.Stack[s - 1];
						s--; break;
					case Instructions.MathReciprocal:
						Function.Stack[s - 1] = Function.Stack[s - 1] == 0.0 ? 0.0 : 1.0 / Function.Stack[s - 1];
						break;
					case Instructions.MathPower:
						{
							double a = Function.Stack[s - 2];
							double b = Function.Stack[s - 1];
							if (b == 2.0) {
								Function.Stack[s - 2] = a * a;
							} else if (b == 3.0) {
								Function.Stack[s - 2] = a * a * a;
							} else if (b == 4.0) {
								double t = a * a;
								Function.Stack[s - 2] = t * t;
							} else if (b == 5.0) {
								double t = a * a;
								Function.Stack[s - 2] = t * t * a;
							} else if (b == 6.0) {
								double t = a * a * a;
								Function.Stack[s - 2] = t * t;
							} else if (b == 7.0) {
								double t = a * a * a;
								Function.Stack[s - 2] = t * t * a;
							} else if (b == 8.0) {
								double t = a * a; t *= t;
								Function.Stack[s - 2] = t * t;
							} else if (a > 0.0) {
								Function.Stack[s - 2] = Math.Pow(a, b);
							} else if (a < 0.0) {
								double x = Math.Round(b);
								if (b - x >= -0.0001 & b - x <= 0.0001) {
									Function.Stack[s - 2] = Math.Pow(-a, x) * Math.Cos(Math.PI * x);
								} else {
									Function.Stack[s - 2] = 0.0;
								}
							} else {
								Function.Stack[s - 2] = 1.0;
							}
							s--; break;
						}
					case Instructions.MathIncrement:
						Function.Stack[s - 1] += 1.0;
						break;
					case Instructions.MathDecrement:
						Function.Stack[s - 1] -= 1.0;
						break;
					case Instructions.MathFusedMultiplyAdd:
						Function.Stack[s - 3] = Function.Stack[s - 3] * Function.Stack[s - 2] + Function.Stack[s - 1];
						s -= 2; break;
					case Instructions.MathQuotient:
						Function.Stack[s - 2] = Function.Stack[s - 1] == 0.0 ? 0.0 : Math.Floor(Function.Stack[s - 2] / Function.Stack[s - 1]);
						s--; break;
					case Instructions.MathMod:
						Function.Stack[s - 2] = Function.Stack[s - 1] == 0.0 ? 0.0 : Function.Stack[s - 2] - Function.Stack[s - 1] * Math.Floor(Function.Stack[s - 2] / Function.Stack[s - 1]);
						s--; break;
					case Instructions.MathFloor:
						Function.Stack[s - 1] = Math.Floor(Function.Stack[s - 1]);
						break;
					case Instructions.MathCeiling:
						Function.Stack[s - 1] = Math.Ceiling(Function.Stack[s - 1]);
						break;
					case Instructions.MathRound:
						Function.Stack[s - 1] = Math.Round(Function.Stack[s - 1]);
						break;
					case Instructions.MathMin:
						Function.Stack[s - 2] = Function.Stack[s - 2] < Function.Stack[s - 1] ? Function.Stack[s - 2] : Function.Stack[s - 1];
						s--; break;
					case Instructions.MathMax:
						Function.Stack[s - 2] = Function.Stack[s - 2] > Function.Stack[s - 1] ? Function.Stack[s - 2] : Function.Stack[s - 1];
						s--; break;
					case Instructions.MathAbs:
						Function.Stack[s - 1] = Math.Abs(Function.Stack[s - 1]);
						break;
					case Instructions.MathSign:
						Function.Stack[s - 1] = Math.Sign(Function.Stack[s - 1]);
						break;
					case Instructions.MathExp:
						Function.Stack[s - 1] = Math.Exp(Function.Stack[s - 1]);
						break;
					case Instructions.MathLog:
						Function.Stack[s - 1] = Log(Function.Stack[s - 1]);
						break;
					case Instructions.MathSqrt:
						Function.Stack[s - 1] = Sqrt(Function.Stack[s - 1]);
						break;
					case Instructions.MathSin:
						Function.Stack[s - 1] = Math.Sin(Function.Stack[s - 1]);
						break;
					case Instructions.MathCos:
						Function.Stack[s - 1] = Math.Cos(Function.Stack[s - 1]);
						break;
					case Instructions.MathTan:
						Function.Stack[s - 1] = Tan(Function.Stack[s - 1]);
						break;
					case Instructions.MathArcTan:
						Function.Stack[s - 1] = Math.Atan(Function.Stack[s - 1]);
						break;
						// comparisons
					case Instructions.CompareEqual:
						Function.Stack[s - 2] = Function.Stack[s - 2] == Function.Stack[s - 1] ? 1.0 : 0.0;
						s--; break;
					case Instructions.CompareUnequal:
						Function.Stack[s - 2] = Function.Stack[s - 2] != Function.Stack[s - 1] ? 1.0 : 0.0;
						s--; break;
					case Instructions.CompareLess:
						Function.Stack[s - 2] = Function.Stack[s - 2] < Function.Stack[s - 1] ? 1.0 : 0.0;
						s--; break;
					case Instructions.CompareGreater:
						Function.Stack[s - 2] = Function.Stack[s - 2] > Function.Stack[s - 1] ? 1.0 : 0.0;
						s--; break;
					case Instructions.CompareLessEqual:
						Function.Stack[s - 2] = Function.Stack[s - 2] <= Function.Stack[s - 1] ? 1.0 : 0.0;
						s--; break;
					case Instructions.CompareGreaterEqual:
						Function.Stack[s - 2] = Function.Stack[s - 2] >= Function.Stack[s - 1] ? 1.0 : 0.0;
						s--; break;
					case Instructions.CompareConditional:
						Function.Stack[s - 3] = Function.Stack[s - 3] != 0.0 ? Function.Stack[s - 2] : Function.Stack[s - 1];
						s -= 2; break;
						// logical
					case Instructions.LogicalNot:
						Function.Stack[s - 1] = Function.Stack[s - 1] != 0.0 ? 0.0 : 1.0;
						break;
					case Instructions.LogicalAnd:
						Function.Stack[s - 2] = Function.Stack[s - 2] != 0.0 & Function.Stack[s - 1] != 0.0 ? 1.0 : 0.0;
						s--; break;
					case Instructions.LogicalOr:
						Function.Stack[s - 2] = Function.Stack[s - 2] != 0.0 | Function.Stack[s - 1] != 0.0 ? 1.0 : 0.0;
						s--; break;
					case Instructions.LogicalNand:
						Function.Stack[s - 2] = Function.Stack[s - 2] != 0.0 & Function.Stack[s - 1] != 0.0 ? 0.0 : 1.0;
						s--; break;
					case Instructions.LogicalNor:
						Function.Stack[s - 2] = Function.Stack[s - 2] != 0.0 | Function.Stack[s - 1] != 0.0 ? 0.0 : 1.0;
						s--; break;
					case Instructions.LogicalXor:
						Function.Stack[s - 2] = Function.Stack[s - 2] != 0.0 ^ Function.Stack[s - 1] != 0.0 ? 1.0 : 0.0;
						s--; break;
						// time/camera
					case Instructions.TimeSecondsSinceMidnight:
						Function.Stack[s] = Game.SecondsSinceMidnight;
						s++; break;
					case Instructions.CameraDistance:
						{
							double dx = World.AbsoluteCameraPosition.X - Position.X;
							double dy = World.AbsoluteCameraPosition.Y - Position.Y;
							double dz = World.AbsoluteCameraPosition.Z - Position.Z;
							Function.Stack[s] = Math.Sqrt(dx * dx + dy * dy + dz * dz);
							s++;
						} break;
						// train
					case Instructions.TrainCars:
						if (Train != null) {
							Function.Stack[s] = Train.Cars.Length;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.TrainSpeed:
						if (Train != null) {
							Function.Stack[s] = Train.Specs.CurrentAverageSpeed;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.TrainSpeedometer:
						if (Train != null) {
							Function.Stack[s] = Train.Cars[Train.DriverCar].Specs.CurrentPerceivedSpeed;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.TrainDistance:
						if (Train != null) {
							double dist = double.MaxValue;
							for (int j = 0; j < Train.Cars.Length; j++) {
								double fx = Train.Cars[j].FrontAxle.Follower.WorldPosition.X - Position.X;
								double fy = Train.Cars[j].FrontAxle.Follower.WorldPosition.Y - Position.Y;
								double fz = Train.Cars[j].FrontAxle.Follower.WorldPosition.Z - Position.Z;
								double f = fx * fx + fy * fy + fz * fz;
								if (f < dist) dist = f;
								double rx = Train.Cars[j].RearAxle.Follower.WorldPosition.X - Position.X;
								double ry = Train.Cars[j].RearAxle.Follower.WorldPosition.Y - Position.Y;
								double rz = Train.Cars[j].RearAxle.Follower.WorldPosition.Z - Position.Z;
								double r = rx * rx + ry * ry + rz * rz;
								if (r < dist) dist = r;
							}
							Function.Stack[s] = Math.Sqrt(dist);
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.TrainDistanceToCar:
						if (Train != null) {
							int j = (int)Math.Round(Function.Stack[s - 1]);
							if (j < 0) j += Train.Cars.Length;
							if (j >= 0 & j < Train.Cars.Length) {
								double x = 0.5 * (Train.Cars[j].FrontAxle.Follower.WorldPosition.X + Train.Cars[j].RearAxle.Follower.WorldPosition.X) - Position.X;
								double y = 0.5 * (Train.Cars[j].FrontAxle.Follower.WorldPosition.Y + Train.Cars[j].RearAxle.Follower.WorldPosition.Y) - Position.Y;
								double z = 0.5 * (Train.Cars[j].FrontAxle.Follower.WorldPosition.Z + Train.Cars[j].RearAxle.Follower.WorldPosition.Z) - Position.Z;
								Function.Stack[s - 1] = Math.Sqrt(x * x + y * y + z * z);
							} else {
								Function.Stack[s - 1] = 0.0;
							}
						} else {
							Function.Stack[s - 1] = 0.0;
						}
						break;
					case Instructions.TrainTrackDistance:
						if (Train != null) {
							int r = Train.Cars.Length - 1;
							double t0 = Train.Cars[0].FrontAxle.Follower.TrackPosition - Train.Cars[0].FrontAxlePosition + 0.5 * Train.Cars[0].Length;
							double t1 = Train.Cars[r].RearAxle.Follower.TrackPosition - Train.Cars[r].RearAxlePosition - 0.5 * Train.Cars[r].Length;
							Function.Stack[s] = TrackPosition > t0 ? TrackPosition - t0 : TrackPosition < t1 ? TrackPosition - t1 : 0.0;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.TrainTrackDistanceToCar:
						if (Train != null) {
							int j = (int)Math.Round(Function.Stack[s - 1]);
							if (j < 0) j += Train.Cars.Length;
							if (j >= 0 & j < Train.Cars.Length) {
								double p = 0.5 * (Train.Cars[j].FrontAxle.Follower.TrackPosition + Train.Cars[j].RearAxle.Follower.TrackPosition);
								Function.Stack[s - 1] = TrackPosition - p;
							} else {
								Function.Stack[s - 1] = 0.0;
							}
						} else {
							Function.Stack[s - 1] = 0.0;
						}
						break;
						// door
					case Instructions.Doors:
						if (Train != null) {
							double a = 0.0;
							for (int j = 0; j < Train.Cars.Length; j++) {
								for (int k = 0; k < Train.Cars[j].Specs.Doors.Length; k++) {
									if (Train.Cars[j].Specs.Doors[k].State > a) {
										a = Train.Cars[j].Specs.Doors[k].State;
									}
								}
							}
							Function.Stack[s] = a;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.DoorsIndex:
						if (Train != null) {
							double a = 0.0;
							int j = (int)Math.Round(Function.Stack[s - 1]);
							if (j < 0) j += Train.Cars.Length;
							if (j >= 0 & j < Train.Cars.Length) {
								for (int k = 0; k < Train.Cars[j].Specs.Doors.Length; k++) {
									if (Train.Cars[j].Specs.Doors[k].State > a) {
										a = Train.Cars[j].Specs.Doors[k].State;
									}
								}
							}
							Function.Stack[s - 1] = a;
						} else {
							Function.Stack[s - 1] = 0.0;
						}
						break;
					case Instructions.LeftDoors:
						if (Train != null) {
							double a = 0.0;
							for (int j = 0; j < Train.Cars.Length; j++) {
								for (int k = 0; k < Train.Cars[j].Specs.Doors.Length; k++) {
									if (Train.Cars[j].Specs.Doors[k].Direction == -1 & Train.Cars[j].Specs.Doors[k].State > a) {
										a = Train.Cars[j].Specs.Doors[k].State;
									}
								}
							}
							Function.Stack[s] = a;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.LeftDoorsIndex:
						if (Train != null) {
							double a = 0.0;
							int j = (int)Math.Round(Function.Stack[s - 1]);
							if (j < 0) j += Train.Cars.Length;
							if (j >= 0 & j < Train.Cars.Length) {
								for (int k = 0; k < Train.Cars[j].Specs.Doors.Length; k++) {
									if (Train.Cars[j].Specs.Doors[k].Direction == -1 & Train.Cars[j].Specs.Doors[k].State > a) {
										a = Train.Cars[j].Specs.Doors[k].State;
									}
								}
							}
							Function.Stack[s - 1] = a;
						} else {
							Function.Stack[s - 1] = 0.0;
						}
						break;
					case Instructions.RightDoors:
						if (Train != null) {
							double a = 0.0;
							for (int j = 0; j < Train.Cars.Length; j++) {
								for (int k = 0; k < Train.Cars[j].Specs.Doors.Length; k++) {
									if (Train.Cars[j].Specs.Doors[k].Direction == 1 & Train.Cars[j].Specs.Doors[k].State > a) {
										a = Train.Cars[j].Specs.Doors[k].State;
									}
								}
							}
							Function.Stack[s] = a;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.RightDoorsIndex:
						if (Train != null) {
							double a = 0.0;
							int j = (int)Math.Round(Function.Stack[s - 1]);
							if (j < 0) j += Train.Cars.Length;
							if (j >= 0 & j < Train.Cars.Length) {
								for (int k = 0; k < Train.Cars[j].Specs.Doors.Length; k++) {
									if (Train.Cars[j].Specs.Doors[k].Direction == 1 & Train.Cars[j].Specs.Doors[k].State > a) {
										a = Train.Cars[j].Specs.Doors[k].State;
									}
								}
							}
							Function.Stack[s - 1] = a;
						} else {
							Function.Stack[s - 1] = 0.0;
						}
						break;
					case Instructions.LeftDoorsTarget:
						if (Train != null) {
							bool q = false;
							for (int j = 0; j < Train.Cars.Length; j++) {
								if (Train.Cars[j].Specs.AnticipatedLeftDoorsOpened) {
									q = true;
									break;
								}
							}
							Function.Stack[s] = q ? 1.0 : 0.0;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.LeftDoorsTargetIndex:
						if (Train != null) {
							bool q = false;
							int j = (int)Math.Round(Function.Stack[s - 1]);
							if (j < 0) j += Train.Cars.Length;
							if (j >= 0 & j < Train.Cars.Length) {
								for (int k = 0; k < Train.Cars[j].Specs.Doors.Length; k++) {
									if (Train.Cars[j].Specs.AnticipatedLeftDoorsOpened) {
										q = true;
										break;
									}
								}
							}
							Function.Stack[s] = q ? 1.0 : 0.0;
						} else {
							Function.Stack[s - 1] = 0.0;
						}
						break;
					case Instructions.RightDoorsTarget:
						if (Train != null) {
							bool q = false;
							for (int j = 0; j < Train.Cars.Length; j++) {
								if (Train.Cars[j].Specs.AnticipatedRightDoorsOpened) {
									q = true;
									break;
								}
							}
							Function.Stack[s] = q ? 1.0 : 0.0;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.RightDoorsTargetIndex:
						if (Train != null) {
							bool q = false;
							int j = (int)Math.Round(Function.Stack[s - 1]);
							if (j < 0) j += Train.Cars.Length;
							if (j >= 0 & j < Train.Cars.Length) {
								for (int k = 0; k < Train.Cars[j].Specs.Doors.Length; k++) {
									if (Train.Cars[j].Specs.AnticipatedRightDoorsOpened) {
										q = true;
										break;
									}
								}
							}
							Function.Stack[s] = q ? 1.0 : 0.0;
						} else {
							Function.Stack[s - 1] = 0.0;
						}
						break;
						// handles
					case Instructions.ReverserNotch:
						if (Train != null) {
							Function.Stack[s] = (double)Train.Specs.CurrentReverser.Driver;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.PowerNotch:
						if (Train != null) {
							Function.Stack[s] = (double)Train.Specs.CurrentPowerNotch.Driver;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.PowerNotches:
						if (Train != null) {
							Function.Stack[s] = (double)Train.Specs.MaximumPowerNotch;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.BrakeNotch:
						if (Train != null) {
							if (Train.Cars[Train.DriverCar].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake) {
								Function.Stack[s] = (double)Train.Specs.AirBrake.Handle.Driver;
							} else {
								Function.Stack[s] = (double)Train.Specs.CurrentBrakeNotch.Driver;
							}
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.BrakeNotches:
						if (Train != null) {
							if (Train.Cars[Train.DriverCar].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake) {
								Function.Stack[s] = 2.0;
							} else {
								Function.Stack[s] = (double)Train.Specs.MaximumBrakeNotch;
							}
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.BrakeNotchLinear:
						if (Train != null) {
							if (Train.Cars[Train.DriverCar].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake) {
								if (Train.Specs.CurrentEmergencyBrake.Driver) {
									Function.Stack[s] = 3.0;
								} else {
									Function.Stack[s] = (double)Train.Specs.AirBrake.Handle.Driver;
								}
							} else if (Train.Specs.HasHoldBrake) {
								if (Train.Specs.CurrentEmergencyBrake.Driver) {
									Function.Stack[s] = (double)Train.Specs.MaximumBrakeNotch + 2.0;
								} else if (Train.Specs.CurrentBrakeNotch.Driver > 0) {
									Function.Stack[s] = (double)Train.Specs.CurrentBrakeNotch.Driver + 1.0;
								} else {
									Function.Stack[s] = Train.Specs.CurrentHoldBrake.Driver ? 1.0 : 0.0;
								}
							} else {
								if (Train.Specs.CurrentEmergencyBrake.Driver) {
									Function.Stack[s] = (double)Train.Specs.MaximumBrakeNotch + 1.0;
								} else {
									Function.Stack[s] = (double)Train.Specs.CurrentBrakeNotch.Driver;
								}
							}
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.BrakeNotchesLinear:
						if (Train != null) {
							if (Train.Cars[Train.DriverCar].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake) {
								Function.Stack[s] = 3.0;
							} else if (Train.Specs.HasHoldBrake) {
								Function.Stack[s] = Train.Specs.MaximumBrakeNotch + 2.0;
							} else {
								Function.Stack[s] = Train.Specs.MaximumBrakeNotch + 1.0;
							}
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.EmergencyBrake:
						if (Train != null) {
							Function.Stack[s] = Train.Specs.CurrentEmergencyBrake.Driver ? 1.0 : 0.0;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.HasAirBrake:
						if (Train != null) {
							Function.Stack[s] = Train.Cars[Train.DriverCar].Specs.BrakeType == TrainManager.CarBrakeType.AutomaticAirBrake ? 1.0 : 0.0;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.HoldBrake:
						if (Train != null) {
							Function.Stack[s] = Train.Specs.CurrentHoldBrake.Driver ? 1.0 : 0.0;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.HasHoldBrake:
						if (Train != null) {
							Function.Stack[s] = Train.Specs.HasHoldBrake ? 1.0 : 0.0;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.ConstSpeed:
						if (Train != null) {
							Function.Stack[s] = Train.Specs.CurrentConstSpeed ? 1.0 : 0.0;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.HasConstSpeed:
						if (Train != null) {
							Function.Stack[s] = Train.Specs.HasConstSpeed ? 1.0 : 0.0;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
						// brake
					case Instructions.BrakeMainReservoir:
						if (Train != null) {
							Function.Stack[s] = Train.Cars[Train.DriverCar].Specs.AirBrake.MainReservoirCurrentPressure;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.BrakeEqualizingReservoir:
						if (Train != null) {
							Function.Stack[s] = Train.Cars[Train.DriverCar].Specs.AirBrake.EqualizingReservoirCurrentPressure;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.BrakeBrakePipe:
						if (Train != null) {
							Function.Stack[s] = Train.Cars[Train.DriverCar].Specs.AirBrake.BrakePipeCurrentPressure;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.BrakeBrakeCylinder:
						if (Train != null) {
							Function.Stack[s] = Train.Cars[Train.DriverCar].Specs.AirBrake.BrakeCylinderCurrentPressure;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
					case Instructions.StraightAirPipe:
						if (Train != null) {
							Function.Stack[s] = Train.Cars[Train.DriverCar].Specs.AirBrake.StraightAirPipeCurrentPressure;
						} else {
							Function.Stack[s] = 0.0;
						}
						s++; break;
						// security
					case Instructions.SecurityPluginState:
						if (Train == null) {
							Function.Stack[s - 1] = 0.0;
						} else {
							int n = (int)Math.Round(Function.Stack[s - 1]);
							if (Train.Specs.Security.Mode == TrainManager.SecuritySystem.Bve4Plugin) {
								if (n >= 0 & n < PluginManager.PluginPanel.Length) {
									Function.Stack[s - 1] = (double)PluginManager.PluginPanel[n];
								} else {
									Function.Stack[s - 1] = 0.0;
								}
							} else {
								Function.Stack[s - 1] = 0.0;
								switch(n) {
									case 256:
										// ATS
										if (Train.Specs.Security.Mode == TrainManager.SecuritySystem.AtsSN) {
											if (Train.Specs.Security.State == TrainManager.SecurityState.Normal | Train.Specs.Security.State == TrainManager.SecurityState.Initialization) {
												Function.Stack[s - 1] = 1.0;
											}
										} break;
									case 257:
										// ATS RUN (separated flashing)
										if (Train.Specs.Security.Mode == TrainManager.SecuritySystem.AtsSN) {
											if (Train.Specs.Security.State == TrainManager.SecurityState.Ringing) {
												Function.Stack[s - 1] = 1.0;
											} else if (Train.Specs.Security.State == TrainManager.SecurityState.Emergency | Train.Specs.Security.State == TrainManager.SecurityState.Pattern | Train.Specs.Security.State == TrainManager.SecurityState.Service) {
												Function.Stack[s - 1] = 2.0;
											}
										} break;
									case 258:
										// ATS RUN (integrated flashing)
										if (Train.Specs.Security.Mode == TrainManager.SecuritySystem.AtsSN) {
											if (Train.Specs.Security.State == TrainManager.SecurityState.Ringing) {
												Function.Stack[s - 1] = 1.0;
											} else if (Train.Specs.Security.State == TrainManager.SecurityState.Emergency | Train.Specs.Security.State == TrainManager.SecurityState.Pattern | Train.Specs.Security.State == TrainManager.SecurityState.Service) {
												if (((int)Math.Floor(2.0 * Game.SecondsSinceMidnight) & 1) == 0) {
													Function.Stack[s - 1] = 1.0;
												} else {
													Function.Stack[s - 1] = 0.0;
												}
											}
										} break;
									case 259:
										// P POWER
										if ((Train.Specs.Security.Mode == TrainManager.SecuritySystem.AtsSN | Train.Specs.Security.Mode == TrainManager.SecuritySystem.AtsP) & Train.Specs.Security.Ats.AtsPAvailable) {
											Function.Stack[s - 1] = 1.0;
										} break;
									case 260:
										// PATTERN APPROACH
										if (Train.Specs.Security.Mode == TrainManager.SecuritySystem.AtsP) {
											if (Train.Specs.Security.State == TrainManager.SecurityState.Pattern | Train.Specs.Security.State == TrainManager.SecurityState.Service) {
												Function.Stack[s - 1] = 1.0;
											}
										} break;
									case 261:
										// BRAKE RELEASE
										if (Train.Specs.Security.Mode == TrainManager.SecuritySystem.AtsP) {
											if (Train.Specs.Security.Ats.AtsPOverride) {
												Function.Stack[s - 1] = 1.0;
											}
										} break;
									case 262:
										// BRAKE OPERATION
										if (Train.Specs.Security.Mode == TrainManager.SecuritySystem.AtsP) {
											if (Train.Specs.Security.State == TrainManager.SecurityState.Service & !Train.Specs.Security.Ats.AtsPOverride) {
												Function.Stack[s - 1] = 1.0;
											}
										} break;
									case 263:
										// ATS-P
										if (Train.Specs.Security.Mode == TrainManager.SecuritySystem.AtsP) {
											Function.Stack[s - 1] = 1.0;
										} break;
									case 264:
										// FAILURE
										if (Train.Specs.Security.Mode != TrainManager.SecuritySystem.None) {
											if (Train.Specs.Security.State == TrainManager.SecurityState.Initialization) {
												Function.Stack[s - 1] = 1.0;
											} else if (Train.Specs.Security.Mode == TrainManager.SecuritySystem.AtsP) {
												if (Train.Specs.Security.State == TrainManager.SecurityState.Ringing | Train.Specs.Security.State == TrainManager.SecurityState.Emergency) {
													Function.Stack[s - 1] = 1.0;
												}
											}
										} break;
									case 265:
										// ATC
										if (Train.Specs.Security.Mode == TrainManager.SecuritySystem.Atc) {
											Function.Stack[s - 1] = 1.0;
										} break;
									case 266:
										// ATC POWER
										if ((Train.Specs.Security.Mode == TrainManager.SecuritySystem.Atc | Train.Specs.Security.Mode != TrainManager.SecuritySystem.None & Train.Specs.Security.Atc.AutomaticSwitch)) {
											Function.Stack[s - 1] = 1.0;
										} break;
									case 267:
										// ATC SERVICE
										if (Train.Specs.Security.Mode == TrainManager.SecuritySystem.Atc) {
											if (Train.Specs.Security.State == TrainManager.SecurityState.Service) {
												Function.Stack[s - 1] = 1.0;
											}
										} break;
									case 268:
										// ATC EMERGENCY
										if (Train.Specs.Security.Mode == TrainManager.SecuritySystem.Atc) {
											if (!Train.Specs.Security.Atc.Transmitting) {
												Function.Stack[s - 1] = 1.0;
											}
										} break;
									case 269:
										// EB
										if (Train.Specs.Security.Mode != TrainManager.SecuritySystem.None) {
											if (Train.Specs.Security.Eb.BellState == TrainManager.SecurityState.Ringing) {
												Function.Stack[s - 1] = 1.0;
											}
										} break;
									case 270:
										// CONST SPEED
										if (Train.Specs.HasConstSpeed) {
											if (Train.Specs.CurrentConstSpeed) {
												Function.Stack[s - 1] = 1.0;
											}
										} break;
									case 271:
										// atc speedometer state
										if (Train.Specs.Security.Mode == TrainManager.SecuritySystem.Atc) {
											if (!Train.Specs.Security.Atc.Transmitting) {
												Function.Stack[s - 1] = 0.0;
											} else {
												if (Train.Specs.Security.Atc.SpeedRestriction < 4.1666) {
													Function.Stack[s - 1] = 1.0;
												} else if (Train.Specs.Security.Atc.SpeedRestriction < 6.9443) {
													Function.Stack[s - 1] = 2.0;
												} else if (Train.Specs.Security.Atc.SpeedRestriction < 12.4999) {
													Function.Stack[s - 1] = 3.0;
												} else if (Train.Specs.Security.Atc.SpeedRestriction < 15.2777) {
													Function.Stack[s - 1] = 4.0;
												} else if (Train.Specs.Security.Atc.SpeedRestriction < 18.0555) {
													Function.Stack[s - 1] = 5.0;
												} else if (Train.Specs.Security.Atc.SpeedRestriction < 20.8333) {
													Function.Stack[s - 1] = 6.0;
												} else if (Train.Specs.Security.Atc.SpeedRestriction < 24.9999) {
													Function.Stack[s - 1] = 7.0;
												} else if (Train.Specs.Security.Atc.SpeedRestriction < 27.7777) {
													Function.Stack[s - 1] = 8.0;
												} else if (Train.Specs.Security.Atc.SpeedRestriction < 30.5555) {
													Function.Stack[s - 1] = 9.0;
												} else if (Train.Specs.Security.Atc.SpeedRestriction < 33.3333) {
													Function.Stack[s - 1] = 10.0;
												} else {
													Function.Stack[s - 1] = 11.0;
												}
											}
										} else {
											Function.Stack[s - 1] = 12.0;
										} break;

								}
							}
						} break;
						// timetable
					case Instructions.TimetableVisible:
						Function.Stack[s] = Timetable.CustomVisible ? 0.0 : -1.0;
						s++; break;
						// sections
					case Instructions.SectionAspectNumber:
						if (SectionIndex >= 0 & SectionIndex < Game.Sections.Length) {
							int a = Game.Sections[SectionIndex].CurrentAspect;
							if (a >= 0 & a < Game.Sections[SectionIndex].Aspects.Length) {
								Function.Stack[s] = (double)Game.Sections[SectionIndex].Aspects[a].Number;
							} else {
								Function.Stack[s] = 0;
							}
						} else {
							Function.Stack[s] = 0;
						}
						s++; break;
						// default
					default:
						throw new System.InvalidProgramException("The unknown instruction " + Function.Instructions[i].ToString() + " was encountered in ExecuteFunctionScript.");
				}
			}
			Function.LastResult = Function.Stack[s - 1];
		}

		// get function script from infix notation
		internal static FunctionScript GetFunctionScriptFromInfixNotation(string Expression) {
			string Function = GetFunctionNotationFromInfixNotation(Expression, true);
			string Postfix = GetPostfixNotationFromFunctionNotation(Function);
			return GetFunctionScriptFromPostfixNotation(Postfix);
		}

		// get function notation from infix notation
		private static string GetFunctionNotationFromInfixNotation(string Expression, bool Preprocessing) {
			// brackets
			if (Preprocessing) {
				int s = 0;
				while (true) {
					if (s >= Expression.Length) break;
					int i = Expression.IndexOf('[', s);
					if (i >= s) {
						int j = i + 1, t = j, m = 1;
						string[] p = new string[4]; int n = 0;
						while (j < Expression.Length) {
							bool q = false;
							switch (Expression[j]) {
								case '[':
									m++;
									break;
								case ']':
									m--;
									if (m < 0) {
										throw new System.IO.InvalidDataException("Unexpected closing bracket encountered in " + Expression);
									} else if (m == 0) {
										if (n >= p.Length) Array.Resize<string>(ref p, n << 1);
										p[n] = Expression.Substring(t, j - t);
										n++;
										string a = Expression.Substring(0, i).Trim();
										string c = Expression.Substring(j + 1).Trim();
										System.Text.StringBuilder r = new System.Text.StringBuilder();
										for (int k = 0; k < n; k++) {
											p[k] = GetFunctionNotationFromInfixNotation(p[k], true);
											if (k > 0) r.Append(',');
											r.Append(p[k]);
										}
										Expression = a + "[" + r.ToString() + "]" + c;
										s = i + r.Length + 2;
										q = true;
									} break;
								case ',':
									if (m == 1) {
										if (n >= p.Length) Array.Resize<string>(ref p, n << 1);
										p[n] = Expression.Substring(t, j - t);
										n++; t = j + 1;
									} break;
							} if (q) break;
							j++;
						}
					} else {
						break;
					}
				}
			}
			// parentheses
			{
				int i = Expression.IndexOf('(');
				if (i >= 0) {
					int j = i + 1;
					int n = 1;
					while (j < Expression.Length) {
						switch (Expression[j]) {
							case '(':
								n++;
								break;
							case ')':
								n--;
								if (n < 0) {
									throw new System.IO.InvalidDataException("Unexpected closing parenthesis encountered in " + Expression);
								} else if (n == 0) {
									string a = Expression.Substring(0, i).Trim();
									string b = Expression.Substring(i + 1, j - i - 1).Trim();
									string c = Expression.Substring(j + 1).Trim();
									return GetFunctionNotationFromInfixNotation(a + GetFunctionNotationFromInfixNotation(b, false) + c, false);
								} break;
						} j++;
					}
					throw new System.IO.InvalidDataException("No closing parenthesis found in " + Expression);
				} else {
					i = Expression.IndexOf(')');
					if (i >= 0) {
						throw new System.IO.InvalidDataException("Unexpected closing parenthesis encountered in " + Expression);
					}
				}
			}
			// operators
			{
				int i = Expression.IndexOf('|');
				if (i >= 0) {
					string a = Expression.Substring(0, i).Trim();
					string b = Expression.Substring(i + 1).Trim();
					return "Or[" + GetFunctionNotationFromInfixNotation(a, false) + "," + GetFunctionNotationFromInfixNotation(b, false) + "]";
				}
			}
			{
				int i = Expression.IndexOf('^');
				if (i >= 0) {
					string a = Expression.Substring(0, i).Trim();
					string b = Expression.Substring(i + 1).Trim();
					return "Xor[" + GetFunctionNotationFromInfixNotation(a, false) + "," + GetFunctionNotationFromInfixNotation(b, false) + "]";
				}
			}
			{
				int i = Expression.IndexOf('&');
				if (i >= 0) {
					string a = Expression.Substring(0, i).Trim();
					string b = Expression.Substring(i + 1).Trim();
					return "And[" + GetFunctionNotationFromInfixNotation(a, false) + "," + GetFunctionNotationFromInfixNotation(b, false) + "]";
				}
			}
			{
				int i = Expression.IndexOf('!');
				while (true) {
					if (i >= 0) {
						if (i < Expression.Length - 1) {
							if (Expression[i + 1] == '=') {
								int j = Expression.IndexOf('!', i + 2);
								i = j < i + 2 ? -1 : j;
							} else break;
						} else break;
					} else break;
				}
				if (i >= 0) {
					string b = Expression.Substring(i + 1).Trim();
					return "Not[" + GetFunctionNotationFromInfixNotation(b, false) + "]";
				}
			}
			{
				int[] j = new int[6];
				j[0] = Expression.LastIndexOf("==");
				j[1] = Expression.LastIndexOf("!=");
				j[2] = Expression.LastIndexOf("<=");
				j[3] = Expression.LastIndexOf(">=");
				j[4] = Expression.LastIndexOf("<");
				j[5] = Expression.LastIndexOf(">");
				int k = -1;
				for (int i = 0; i < j.Length; i++) {
					if (j[i] >= 0) {
						if (k >= 0) {
							if (j[i] > j[k]) k = i;
						} else {
							k = i;
						}
					}
				}
				if (k >= 0) {
					int l = k <= 3 ? 2 : 1;
					string a = Expression.Substring(0, j[k]).Trim();
					string b = Expression.Substring(j[k] + l).Trim();
					string f; switch (k) {
							case 0: f = "Equal"; break;
							case 1: f = "Unequal"; break;
							case 2: f = "LessEqual"; break;
							case 3: f = "GreaterEqual"; break;
							case 4: f = "Less"; break;
							case 5: f = "Greater"; break;
							default: f = "Halt"; break;
					}
					return f + "[" + GetFunctionNotationFromInfixNotation(a, false) + "," + GetFunctionNotationFromInfixNotation(b, false) + "]";
				}
			}
			{
				int i = Expression.LastIndexOf('+');
				int j = Expression.LastIndexOf('-');
				if (i >= 0 & (j == -1 | j >= 0 & i > j)) {
					string a = Expression.Substring(0, i).Trim();
					string b = Expression.Substring(i + 1).Trim();
					return "Plus[" + GetFunctionNotationFromInfixNotation(a, false) + "," + GetFunctionNotationFromInfixNotation(b, false) + "]";
				} else if (j >= 0) {
					string a = Expression.Substring(0, j).Trim();
					string b = Expression.Substring(j + 1).Trim();
					if (a.Length != 0) {
						return "Subtract[" + GetFunctionNotationFromInfixNotation(a, false) + "," + GetFunctionNotationFromInfixNotation(b, false) + "]";
					}
				}
			}
			{
				int i = Expression.IndexOf('*');
				if (i >= 0) {
					string a = Expression.Substring(0, i).Trim();
					string b = Expression.Substring(i + 1).Trim();
					return "Times[" + GetFunctionNotationFromInfixNotation(a, false) + "," + GetFunctionNotationFromInfixNotation(b, false) + "]";
				}
			}
			{
				int i = Expression.IndexOf('/');
				if (i >= 0) {
					string a = Expression.Substring(0, i).Trim();
					string b = Expression.Substring(i + 1).Trim();
					return "Divide[" + GetFunctionNotationFromInfixNotation(a, false) + "," + GetFunctionNotationFromInfixNotation(b, false) + "]";
				}
			}
			{
				int i = Expression.IndexOf('-');
				if (i >= 0) {
					string a = Expression.Substring(0, i).Trim();
					string b = Expression.Substring(i + 1).Trim();
					if (a.Length == 0) {
						return "Minus[" + GetFunctionNotationFromInfixNotation(b, false) + "]";
					}
				}
			}
			return Expression.Trim();
		}

		// get postfix notation from function notation
		private static string GetPostfixNotationFromFunctionNotation(string Expression) {
			int i = Expression.IndexOf('[');
			if (i == -1 | !Expression.EndsWith("]")) return Expression;
			string f = Expression.Substring(0, i);
			string s = Expression.Substring(i + 1, Expression.Length - i - 2);
			string[] a = new string[4]; int n = 0;
			int b = 0;
			for (i = 0; i < s.Length; i++) {
				switch (s[i]) {
					case '[':
						{
							i++; int m = 1;
							bool q = false;
							while (i < s.Length) {
								switch (s[i]) {
									case '[':
										m++;
										break;
									case ']':
										m--;
										if (m < 0) {
											throw new System.IO.InvalidDataException("Unexpected closing bracket encountered in " + Expression);
										} else if (m == 0) {
											q = true;
										} break;
								} if (q) break;
								i++;
							} if (!q) {
								throw new System.IO.InvalidDataException("No closing bracket found in " + Expression);
							}
						} break;
					case ']':
						throw new System.IO.InvalidDataException("Unexpected closing bracket encountered in " + Expression);
					case ',':
						if (n == a.Length) Array.Resize<string>(ref a, n << 1);
						a[n] = s.Substring(b, i - b).Trim();
						n++;
						b = i + 1;
						break;
				}
			}
			if (n == a.Length) Array.Resize<string>(ref a, n << 1);
			a[n] = s.Substring(b).Trim();
			n++;
			if (n == 1 & a[0].Length == 0) n = 0;
			for (i = 0; i < n; i++) {
				if (a[i].Length == 0) {
					throw new System.IO.InvalidDataException("An empty argument is invalid in " + f + " in " + Expression);
				} else if (a[i].IndexOf(' ') >= 0) {
					throw new System.IO.InvalidDataException("An argument containing a space is invalid in " + f + " in " + Expression);
				}
				a[i] = GetPostfixNotationFromFunctionNotation(a[i]).Trim();
			}
			switch (f.ToLowerInvariant()) {
					// arithmetic
				case "plus":
					if (n == 0) {
						return "0";
					} else if (n == 1) {
						return a[0];
					} else if (n == 2) {
						if (a[1].EndsWith(" *")) {
							return a[1] + " " + a[0] + " +";
						} else {
							return a[0] + " " + a[1] + " +";
						}
					} else {
						System.Text.StringBuilder t = new System.Text.StringBuilder(a[0] + " " + a[1] + " +");
						for (i = 2; i < n; i++) {
							t.Append(" " + a[i] + " +");
						} return t.ToString();
					}
				case "subtract":
					if (n == 2) {
						return a[0] + " " + a[1] + " -";
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 2 arguments in " + Expression);
					}
				case "times":
					if (n == 0) {
						return "1";
					} else if (n == 1) {
						return a[0];
					} else if (n == 2) {
						return a[0] + " " + a[1] + " *";
					} else {
						System.Text.StringBuilder t = new System.Text.StringBuilder(a[0] + " " + a[1] + " *");
						for (i = 2; i < n; i++) {
							t.Append(" " + a[i] + " *");
						} return t.ToString();
					}
				case "divide":
					if (n == 2) {
						return a[0] + " " + a[1] + " /";
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 2 arguments in " + Expression);
					}
				case "power":
					if (n == 0) {
						return "1";
					} else if (n == 1) {
						return a[0];
					} else if (n == 2) {
						return a[0] + " " + a[1] + " power";
					} else {
						System.Text.StringBuilder t = new System.Text.StringBuilder(a[0] + " " + a[1]);
						for (i = 2; i < n; i++) {
							t.Append(" " + a[i]);
						}
						for (i = 0; i < n - 1; i++) {
							t.Append(" power");
						}
						return t.ToString();
					}
					// math
				case "quotient":
				case "mod":
				case "min":
				case "max":
					if (n == 2) {
						return a[0] + " " + a[1] + " " + f;
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 2 arguments in " + Expression);
					}
				case "minus":
				case "reciprocal":
				case "floor":
				case "ceiling":
				case "round":
				case "abs":
				case "sign":
				case "exp":
				case "log":
				case "sqrt":
				case "sin":
				case "cos":
				case "tan":
				case "arctan":
					if (n == 1) {
						return a[0] + " " + f;
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 1 argument in " + Expression);
					}
					// comparisons
				case "equal":
				case "unequal":
				case "less":
				case "greater":
				case "lessequal":
				case "greaterequal":
					if (n == 2) {
						string g; switch (f.ToLowerInvariant()) {
								case "equal": g = "=="; break;
								case "unequal": g = "!="; break;
								case "less": g = "<"; break;
								case "greater": g = ">"; break;
								case "lessequal": g = "<="; break;
								case "greaterequal": g = ">="; break;
								default: g = "halt"; break;
						}
						return a[0] + " " + a[1] + " " + g;
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 2 arguments in " + Expression);
					}
				case "if":
					if (n == 3) {
						return a[0] + " " + a[1] + " " + a[2] + " ?";
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 3 arguments in " + Expression);
					}
					// logical
				case "not":
					if (n == 1) {
						return a[0] + " !";
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 1 argument in " + Expression);
					}
				case "and":
					if (n == 0) {
						return "1";
					} else if (n == 1) {
						return a[0];
					} else if (n == 2) {
						return a[0] + " " + a[1] + " &";
					} else {
						System.Text.StringBuilder t = new System.Text.StringBuilder(a[0] + " " + a[1] + " +");
						for (i = 2; i < n; i++) {
							t.Append(" " + a[i] + " &");
						} return t.ToString();
					}
				case "or":
					if (n == 0) {
						return "0";
					} else if (n == 1) {
						return a[0];
					} else if (n == 2) {
						return a[0] + " " + a[1] + " |";
					} else {
						System.Text.StringBuilder t = new System.Text.StringBuilder(a[0] + " " + a[1] + " +");
						for (i = 2; i < n; i++) {
							t.Append(" " + a[i] + " |");
						} return t.ToString();
					}
				case "xor":
					if (n == 0) {
						return "0";
					} else if (n == 1) {
						return a[0];
					} else if (n == 2) {
						return a[0] + " " + a[1] + " ^";
					} else {
						System.Text.StringBuilder t = new System.Text.StringBuilder(a[0] + " " + a[1] + " +");
						for (i = 2; i < n; i++) {
							t.Append(" " + a[i] + " ^");
						} return t.ToString();
					}
					// train
				case "distance":
					if (n == 1) {
						return a[0] + " distancetocar";
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 1 argument in " + Expression);
					}
				case "trackdistance":
					if (n == 1) {
						return a[0] + " trackdistancetocar";
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 1 argument in " + Expression);
					}
				case "doors":
					if (n == 1) {
						return a[0] + " doorsindex";
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 1 argument in " + Expression);
					}
				case "leftdoors":
					if (n == 1) {
						return a[0] + " leftdoorsindex";
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 1 argument in " + Expression);
					}
				case "rightdoorstarget":
					if (n == 1) {
						return a[0] + " rightdoorstargetindex";
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 1 argument in " + Expression);
					}
				case "leftdoorstarget":
					if (n == 1) {
						return a[0] + " leftdoorstargetindex";
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 1 argument in " + Expression);
					}
				case "rightdoors":
					if (n == 1) {
						return a[0] + " rightdoorsindex";
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 1 argument in " + Expression);
					}
				case "pluginstate":
					if (n == 1) {
						return a[0] + " pluginstate";
					} else {
						throw new System.IO.InvalidDataException(f + " is expected to have 1 argument in " + Expression);
					}
					// not supported
				default:
					throw new System.IO.InvalidDataException("The function " + f + " is not supported in " + Expression);
			}
		}

		// get optimized postfox notation
		private static string GetOptimizedPostfixNotation(string Expression) {
			Expression = " " + Expression + " ";
			Expression = Expression.Replace(" 1 1 == -- ", " 0 ");
			Expression = Expression.Replace(" 1 doors - 1 == -- ", " doors ! -- ");
			string[] Arguments = Expression.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			string[] Stack = new string[Arguments.Length];
			int StackLength = 0;
			System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;
			for (int i = 0; i < Arguments.Length; i++) {
				switch (Arguments[i].ToLowerInvariant()) {
					case "<>":
						{
							bool q = true;
							if (StackLength >= 1) {
								if (Stack[StackLength - 1] == "<>") {
									// <> <>
									// [N/A]
									StackLength--;
									q = false;
								} else if (StackLength >= 2) {
									double b;
									if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out b)) {
										double a;
										if (double.TryParse(Stack[StackLength - 2], System.Globalization.NumberStyles.Float, Culture, out a)) {
											// a b <>
											// b a
											string t = Stack[StackLength - 1];
											Stack[StackLength - 1] = Stack[StackLength - 2];
											Stack[StackLength - 2] = t;
										}
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "+":
						{
							bool q = true;
							if (StackLength >= 2) {
								double b;
								if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out b)) {
									double a;
									if (double.TryParse(Stack[StackLength - 2], System.Globalization.NumberStyles.Float, Culture, out a)) {
										// x y +
										// (x y +)
										Stack[StackLength - 2] = (a + b).ToString(Culture);
										StackLength--;
										q = false;
									} else if (StackLength >= 3 && Stack[StackLength - 2] == "+") {
										if (double.TryParse(Stack[StackLength - 3], System.Globalization.NumberStyles.Float, Culture, out a)) {
											// A x + y +
											// A (y x +) +
											Stack[StackLength - 3] = (a + b).ToString(Culture);
											StackLength--;
											q = false;
										}
									} else if (StackLength >= 3 && Stack[StackLength - 2] == "-") {
										if (double.TryParse(Stack[StackLength - 3], System.Globalization.NumberStyles.Float, Culture, out a)) {
											// A x - y +
											// A (y x -) +
											Stack[StackLength - 3] = (b - a).ToString(Culture);
											Stack[StackLength - 2] = "+";
											StackLength--;
											q = false;
										}
									} else if (Stack[StackLength - 2] == "*") {
										// A x * y +
										// A x y fma
										Stack[StackLength - 2] = Stack[StackLength - 1];
										Stack[StackLength - 1] = "fma";
										q = false;
									} else if (Stack[StackLength - 2] == "fma") {
										if (double.TryParse(Stack[StackLength - 3], System.Globalization.NumberStyles.Float, Culture, out a)) {
											// A B y fma z +
											// A B (y z +) fma
											Stack[StackLength - 3] = (a + b).ToString(Culture);
											StackLength--;
											q = false;
										}
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "-":
						{
							bool q = true;
							if (StackLength >= 2) {
								double b;
								if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out b)) {
									double a;
									if (double.TryParse(Stack[StackLength - 2], System.Globalization.NumberStyles.Float, Culture, out a)) {
										// x y -
										// (x y -)
										Stack[StackLength - 2] = (a - b).ToString(Culture);
										StackLength--;
										q = false;
									} else if (StackLength >= 3 && Stack[StackLength - 2] == "+") {
										if (double.TryParse(Stack[StackLength - 3], System.Globalization.NumberStyles.Float, Culture, out a)) {
											// A x + y -
											// A (x y -) +
											Stack[StackLength - 3] = (a - b).ToString(Culture);
											Stack[StackLength - 2] = "+";
											StackLength--;
											q = false;
										}
									} else if (StackLength >= 3 && Stack[StackLength - 2] == "-") {
										if (double.TryParse(Stack[StackLength - 3], System.Globalization.NumberStyles.Float, Culture, out a)) {
											// A x - y -
											// A (x y + minus) -
											Stack[StackLength - 3] = (-a - b).ToString(Culture);
											Stack[StackLength - 2] = "+";
											StackLength--;
											q = false;
										}
									} else if (Stack[StackLength - 2] == "*") {
										// A x * y -
										// A x (y minus) fma
										Stack[StackLength - 2] = (-b).ToString(Culture);
										Stack[StackLength - 1] = "fma";
										q = false;
									} else if (Stack[StackLength - 2] == "fma") {
										if (double.TryParse(Stack[StackLength - 3], System.Globalization.NumberStyles.Float, Culture, out a)) {
											// A B y fma z -
											// A B (y z -) fma
											Stack[StackLength - 3] = (a - b).ToString(Culture);
											StackLength--;
											q = false;
										}
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "minus":
						{
							bool q = true;
							if (StackLength >= 1) {
								if (Stack[StackLength - 1].Equals("minus", StringComparison.InvariantCultureIgnoreCase)) {
									// minus minus
									// [N/A]
									StackLength--;
									q = false;
								} else {
									double a;
									if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out a)) {
										// x minus
										// (x minus)
										Stack[StackLength - 1] = (-a).ToString(Culture);
										q = false;
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "*":
						{
							bool q = true;
							if (StackLength >= 2) {
								double b;
								if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out b)) {
									double a;
									if (double.TryParse(Stack[StackLength - 2], System.Globalization.NumberStyles.Float, Culture, out a)) {
										// x y *
										// (x y *)
										Stack[StackLength - 2] = (a * b).ToString(Culture);
										StackLength--;
										q = false;
									} else if (StackLength >= 3 && Stack[StackLength - 2] == "*") {
										if (double.TryParse(Stack[StackLength - 3], System.Globalization.NumberStyles.Float, Culture, out a)) {
											// A x * y *
											// A (x y *) *
											Stack[StackLength - 3] = (a * b).ToString(Culture);
											StackLength--;
											q = false;
										}
									} else if (StackLength >= 3 && Stack[StackLength - 2] == "+") {
										if (double.TryParse(Stack[StackLength - 3], System.Globalization.NumberStyles.Float, Culture, out a)) {
											// A x + y *
											// A y (x y *) fma
											Stack[StackLength - 3] = Stack[StackLength - 1];
											Stack[StackLength - 2] = (a * b).ToString(Culture);
											Stack[StackLength - 1] = "fma";
											q = false;
										}
									} else if (StackLength >= 3 && Stack[StackLength - 2] == "-") {
										if (double.TryParse(Stack[StackLength - 3], System.Globalization.NumberStyles.Float, Culture, out a)) {
											// A x - y *
											// A y (x y * minus) fma
											Stack[StackLength - 3] = Stack[StackLength - 1];
											Stack[StackLength - 2] = (-a * b).ToString(Culture);
											Stack[StackLength - 1] = "fma";
											q = false;
										}
									} else if (StackLength >= 4 && Stack[StackLength - 2] == "fma") {
										if (double.TryParse(Stack[StackLength - 3], System.Globalization.NumberStyles.Float, Culture, out a)) {
											double c;
											if (double.TryParse(Stack[StackLength - 4], System.Globalization.NumberStyles.Float, Culture, out c)) {
												// A x y fma z *
												// A (x z *) (y z *) fma
												Stack[StackLength - 4] = (c * b).ToString(Culture);
												Stack[StackLength - 3] = (a * b).ToString(Culture);
												StackLength--;
												q = false;
											} else {
												// A B y fma z *
												// A B * z (y z *) fma
												Stack[StackLength - 3] = "*";
												Stack[StackLength - 2] = Stack[StackLength - 1];
												Stack[StackLength - 1] = (a * b).ToString(Culture);
												Stack[StackLength] = "fma";
												StackLength++;
												q = false;
											}
										}
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "reciprocal":
						{
							bool q = true;
							if (StackLength >= 1) {
								if (Stack[StackLength - 1].Equals("reciprocal", StringComparison.InvariantCultureIgnoreCase)) {
									// reciprocal reciprocal
									// [N/A]
									StackLength--;
									q = false;
								} else {
									double a;
									if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out a)) {
										// x reciprocal
										// (x reciprocal)
										a = a == 0.0 ? 0.0 : 1.0 / a;
										Stack[StackLength - 1] = a.ToString(Culture);
										q = false;
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "/":
						{
							bool q = true;
							if (StackLength >= 2) {
								double b;
								if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out b)) {
									if (b != 0.0) {
										double a;
										if (double.TryParse(Stack[StackLength - 2], System.Globalization.NumberStyles.Float, Culture, out a)) {
											// x y /
											// (x y /)
											Stack[StackLength - 2] = (a / b).ToString(Culture);
											StackLength--;
											q = false;
										} else if (StackLength >= 3 && Stack[StackLength - 2] == "*") {
											if (double.TryParse(Stack[StackLength - 3], System.Globalization.NumberStyles.Float, Culture, out a)) {
												// A x * y /
												// A (x y /) *
												Stack[StackLength - 3] = (a / b).ToString(Culture);
												StackLength--;
												q = false;
											}
										}
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "++":
						{
							bool q = true;
							if (StackLength >= 1) {
								double a;
								if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out a)) {
									// x ++
									// (x ++)
									Stack[StackLength - 1] = (a + 1).ToString(Culture);
									q = false;
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "--":
						{
							bool q = true;
							if (StackLength >= 1) {
								double a;
								if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out a)) {
									// x --
									// (x --)
									Stack[StackLength - 1] = (a - 1).ToString(Culture);
									q = false;
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "!":
						{
							bool q = true;
							if (StackLength >= 1) {
								if (Stack[StackLength - 1] == "!") {
									StackLength--;
									q = false;
								} else if (Stack[StackLength - 1] == "==") {
									Stack[StackLength - 1] = "!=";
									q = false;
								} else if (Stack[StackLength - 1] == "!=") {
									Stack[StackLength - 1] = "==";
									q = false;
								} else if (Stack[StackLength - 1] == "<") {
									Stack[StackLength - 1] = ">=";
									q = false;
								} else if (Stack[StackLength - 1] == ">") {
									Stack[StackLength - 1] = "<=";
									q = false;
								} else if (Stack[StackLength - 1] == "<=") {
									Stack[StackLength - 1] = ">";
									q = false;
								} else if (Stack[StackLength - 1] == ">=") {
									Stack[StackLength - 1] = "<";
									q = false;
								} else {
									double a;
									if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out a)) {
										Stack[StackLength - 1] = a == 0.0 ? "1" : "0";
										q = false;
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "==":
						{
							bool q = true;
							if (StackLength >= 2) {
								double b;
								if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out b)) {
									double a;
									if (double.TryParse(Stack[StackLength - 2], System.Globalization.NumberStyles.Float, Culture, out a)) {
										Stack[StackLength - 2] = a == b ? "1" : "0";
										StackLength--;
										q = false;
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "!=":
						{
							bool q = true;
							if (StackLength >= 2) {
								double b;
								if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out b)) {
									double a;
									if (double.TryParse(Stack[StackLength - 2], System.Globalization.NumberStyles.Float, Culture, out a)) {
										Stack[StackLength - 2] = a != b ? "1" : "0";
										StackLength--;
										q = false;
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "<":
						{
							bool q = true;
							if (StackLength >= 2) {
								double b;
								if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out b)) {
									double a;
									if (double.TryParse(Stack[StackLength - 2], System.Globalization.NumberStyles.Float, Culture, out a)) {
										Stack[StackLength - 2] = a < b ? "1" : "0";
										StackLength--;
										q = false;
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case ">":
						{
							bool q = true;
							if (StackLength >= 2) {
								double b;
								if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out b)) {
									double a;
									if (double.TryParse(Stack[StackLength - 2], System.Globalization.NumberStyles.Float, Culture, out a)) {
										Stack[StackLength - 2] = a > b ? "1" : "0";
										StackLength--;
										q = false;
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "<=":
						{
							bool q = true;
							if (StackLength >= 2) {
								double b;
								if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out b)) {
									double a;
									if (double.TryParse(Stack[StackLength - 2], System.Globalization.NumberStyles.Float, Culture, out a)) {
										Stack[StackLength - 2] = a <= b ? "1" : "0";
										StackLength--;
										q = false;
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case ">=":
						{
							bool q = true;
							if (StackLength >= 2) {
								double b;
								if (double.TryParse(Stack[StackLength - 1], System.Globalization.NumberStyles.Float, Culture, out b)) {
									double a;
									if (double.TryParse(Stack[StackLength - 2], System.Globalization.NumberStyles.Float, Culture, out a)) {
										Stack[StackLength - 2] = a >= b ? "1" : "0";
										StackLength--;
										q = false;
									}
								}
							}
							if (q) {
								Stack[StackLength] = Arguments[i];
								StackLength++;
							}
						} break;
					case "floor":
						if (StackLength >= 1 && Stack[StackLength - 1] == "/") {
							Stack[StackLength - 1] = "quotient";
						} else {
							Stack[StackLength] = Arguments[i];
							StackLength++;
						} break;
					default:
						Stack[StackLength] = Arguments[i];
						StackLength++;
						break;
				}
			}
			System.Text.StringBuilder Builder = new System.Text.StringBuilder();
			for (int i = 0; i < StackLength; i++) {
				if (i != 0) Builder.Append(' ');
				Builder.Append(Stack[i]);
			}
			return Builder.ToString();
		}

		// get function script from postfix notation
		internal static FunctionScript GetFunctionScriptFromPostfixNotation(string Expression) {
			Expression = GetOptimizedPostfixNotation(Expression);
			System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;
			FunctionScript Result = new FunctionScript();
			string[] Arguments = Expression.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			Result.Instructions = new Instructions[16]; int n = 0;
			Result.Stack = new double[16]; int m = 0, s = 0;
			Result.Constants = new double[16]; int c = 0;
			for (int i = 0; i < Arguments.Length; i++) {
				double d; if (double.TryParse(Arguments[i], System.Globalization.NumberStyles.Float, Culture, out d)) {
					if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
					Result.Instructions[n] = Instructions.SystemConstant;
					if (c >= Result.Constants.Length) Array.Resize<double>(ref Result.Constants, Result.Constants.Length << 1);
					Result.Constants[c] = d;
					n++; c++; s++; if (s >= m) m = s;
				} else {
					switch (Arguments[i].ToLowerInvariant()) {
							// system
						case "halt":
							throw new System.InvalidOperationException("The halt instruction was encountered in function script " + Expression);
						case "value":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.SystemValue;
							n++; s++; if (s >= m) m = s; break;
						case "delta":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.SystemDelta;
							n++; s++; if (s >= m) m = s; break;
							// stack
						case "~":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.StackCopy;
							n++; s++; if (s >= m) m = s; break;
						case "<>":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.StackSwap;
							n++; break;
							// math
						case "+":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathPlus;
							n++; s--; break;
						case "-":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathSubtract;
							n++; s--; break;
						case "minus":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathMinus;
							n++; break;
						case "*":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathTimes;
							n++; s--; break;
						case "/":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathDivide;
							n++; s--; break;
						case "reciprocal":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathReciprocal;
							n++; break;
						case "power":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathPower;
							n++; s--; break;
						case "++":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathIncrement;
							n++; break;
						case "--":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathDecrement;
							n++; break;
						case "fma":
							if (s < 3) throw new System.InvalidOperationException(Arguments[i] + " requires at least 3 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathFusedMultiplyAdd;
							n++; s -= 2; break;
						case "quotient":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathQuotient;
							n++; s--; break;
						case "mod":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathMod;
							n++; s--; break;
						case "floor":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathFloor;
							n++; break;
						case "ceiling":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathCeiling;
							n++; break;
						case "round":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathRound;
							n++; break;
						case "min":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathMin;
							n++; s--; break;
						case "max":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathMax;
							n++; s--; break;
						case "abs":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathAbs;
							n++; break;
						case "sign":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathSign;
							n++; break;
						case "exp":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathExp;
							n++; break;
						case "log":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathLog;
							n++; break;
						case "sqrt":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathSqrt;
							n++; break;
						case "sin":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathSin;
							n++; break;
						case "cos":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathCos;
							n++; break;
						case "tan":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathTan;
							n++; break;
						case "arctan":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.MathArcTan;
							n++; break;
						case "==":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.CompareEqual;
							n++; s--; break;
						case "!=":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.CompareUnequal;
							n++; s--; break;
							// conditionals
						case "<":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.CompareLess;
							n++; s--; break;
						case ">":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.CompareGreater;
							n++; s--; break;
						case "<=":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.CompareLessEqual;
							n++; s--; break;
						case ">=":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.CompareGreaterEqual;
							n++; s--; break;
						case "?":
							if (s < 3) throw new System.InvalidOperationException(Arguments[i] + " requires at least 3 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.CompareConditional;
							n++; s -= 2; break;
							// logical
						case "!":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.LogicalNot;
							n++; break;
						case "&":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.LogicalAnd;
							n++; s--; break;
						case "|":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.LogicalOr;
							n++; s--; break;
						case "!&":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.LogicalNand;
							n++; s--; break;
						case "!|":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.LogicalNor;
							n++; s--; break;
						case "^":
							if (s < 2) throw new System.InvalidOperationException(Arguments[i] + " requires at least 2 arguments on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.LogicalXor;
							n++; s--; break;
							// time/camera
						case "time":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.TimeSecondsSinceMidnight;
							n++; s++; if (s >= m) m = s; break;
						case "cameradistance":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.CameraDistance;
							n++; s++; if (s >= m) m = s; break;
							// train
						case "cars":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.TrainCars;
							n++; s++; if (s >= m) m = s; break;
						case "speed":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.TrainSpeed;
							n++; s++; if (s >= m) m = s; break;
						case "speedometer":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.TrainSpeedometer;
							n++; s++; if (s >= m) m = s; break;
						case "distance":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.TrainDistance;
							n++; s++; if (s >= m) m = s; break;
						case "distancetocar":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.TrainDistanceToCar;
							n++; break;
						case "trackdistance":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.TrainTrackDistance;
							n++; s++; if (s >= m) m = s; break;
						case "trackdistancetocar":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.TrainTrackDistanceToCar;
							n++; break;
							// train: doors
						case "doors":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.Doors;
							n++; s++; if (s >= m) m = s; break;
						case "doorsindex":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.DoorsIndex;
							n++; break;
						case "leftdoors":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.LeftDoors;
							n++; s++; if (s >= m) m = s; break;
						case "leftdoorsindex":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.LeftDoorsIndex;
							n++; break;
						case "rightdoors":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.RightDoors;
							n++; s++; if (s >= m) m = s; break;
						case "rightdoorsindex":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.RightDoorsIndex;
							n++; break;
						case "leftdoorstarget":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.LeftDoorsTarget;
							n++; s++; if (s >= m) m = s; break;
						case "leftdoorstargetindex":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.LeftDoorsTargetIndex;
							n++; break;
						case "rightdoorstarget":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.RightDoorsTarget;
							n++; s++; if (s >= m) m = s; break;
						case "rightdoorstargetindex":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.RightDoorsTargetIndex;
							n++; break;
							// train: handles
						case "reversernotch":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.ReverserNotch;
							n++; s++; if (s >= m) m = s; break;
						case "powernotch":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.PowerNotch;
							n++; s++; if (s >= m) m = s; break;
						case "powernotches":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.PowerNotches;
							n++; s++; if (s >= m) m = s; break;
						case "brakenotch":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.BrakeNotch;
							n++; s++; if (s >= m) m = s; break;
						case "brakenotches":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.BrakeNotches;
							n++; s++; if (s >= m) m = s; break;
						case "brakenotchlinear":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.BrakeNotchLinear;
							n++; s++; if (s >= m) m = s; break;
						case "brakenotcheslinear":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.BrakeNotchesLinear;
							n++; s++; if (s >= m) m = s; break;
						case "emergencybrake":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.EmergencyBrake;
							n++; s++; if (s >= m) m = s; break;
						case "hasairbrake":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.HasAirBrake;
							n++; s++; if (s >= m) m = s; break;
						case "holdbrake":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.HoldBrake;
							n++; s++; if (s >= m) m = s; break;
						case "hasholdbrake":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.HasHoldBrake;
							n++; s++; if (s >= m) m = s; break;
						case "constspeed":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.ConstSpeed;
							n++; s++; if (s >= m) m = s; break;
						case "hasconstspeed":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.HasConstSpeed;
							n++; s++; if (s >= m) m = s; break;
							// train: brake
						case "mainreservoir":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.BrakeMainReservoir;
							n++; s++; if (s >= m) m = s; break;
						case "equalizingreservoir":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.BrakeEqualizingReservoir;
							n++; s++; if (s >= m) m = s; break;
						case "brakepipe":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.BrakeBrakePipe;
							n++; s++; if (s >= m) m = s; break;
						case "brakecylinder":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.BrakeBrakeCylinder;
							n++; s++; if (s >= m) m = s; break;
						case "straightairpipe":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.StraightAirPipe;
							n++; s++; if (s >= m) m = s; break;
							// train: security
						case "pluginstate":
							if (s < 1) throw new System.InvalidOperationException(Arguments[i] + " requires at least 1 argument on the stack in function script " + Expression);
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.SecurityPluginState;
							n++; break;
							// train: timetable
						case "timetable":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.TimetableVisible;
							n++; s++; if (s >= m) m = s; break;
							// sections
						case "section":
							if (n >= Result.Instructions.Length) Array.Resize<Instructions>(ref Result.Instructions, Result.Instructions.Length << 1);
							Result.Instructions[n] = Instructions.SectionAspectNumber;
							n++; s++; if (s >= m) m = s; break;
							// default
						default:
							throw new System.IO.InvalidDataException("Unknown command " + Arguments[i] + " encountered in function script " + Expression);
					}
				}
			}
			if (s != 1) {
				throw new System.InvalidOperationException("There must be exactly one argument left on the stack at the end in function script " + Expression);
			}
			Array.Resize<Instructions>(ref Result.Instructions, n);
			Array.Resize<double>(ref Result.Stack, m);
			Array.Resize<double>(ref Result.Constants, c);
			return Result;
		}

		// mathematical functions
		private static double Log(double X) {
			if (X <= 0.0) {
				return 0.0; /// ComplexInfinity or NonReal
			} else {
				return Math.Log(X);
			}
		}
		private static double Sqrt(double X) {
			if (X < 0.0) {
				return 0.0; /// NonReal
			} else {
				return Math.Sqrt(X);
			}
		}
		private static double Tan(double X) {
			double c = X / Math.PI;
			double d = c - Math.Floor(c) - 0.5;
			double e = Math.Floor(X >= 0.0 ? X : -X) * 1.38462643383279E-16;
			if (d >= -e & d <= e) {
				return 0.0; /// ComplexInfinity
			} else {
				return Math.Tan(X);
			}
		}

	}
}