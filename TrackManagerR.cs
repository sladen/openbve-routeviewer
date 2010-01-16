// ╔═════════════════════════════════════════════════════════════╗
// ║ TrackManager.cs for the Route Viewer                        ║
// ╠═════════════════════════════════════════════════════════════╣
// ║ This file cannot be used in the openBVE main program.       ║
// ║ The file from the openBVE main program cannot be used here. ║
// ╚═════════════════════════════════════════════════════════════╝

using System;

namespace OpenBve {
    internal static class TrackManager {

        // events
        internal enum EventTriggerType {
            None = 0,
            Camera = 1,
            FrontCarFrontAxle = 2,
            RearCarRearAxle = 3,
            OtherCarFrontAxle = 4,
            OtherCarRearAxle = 5
        }
        internal abstract class GeneralEvent {
            internal double TrackPositionDelta;
            internal bool DontTriggerAnymore;
            internal abstract void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex);
        }
        internal static void TryTriggerEvent(GeneralEvent Event, int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) {
            if (!Event.DontTriggerAnymore) {
                Event.Trigger(Direction, TriggerType, Train, CarIndex);
            }
        }
        // background change
        internal class BackgroundChangeEvent : GeneralEvent {
            internal World.Background PreviousBackground;
            internal World.Background NextBackground;
            internal BackgroundChangeEvent(double TrackPositionDelta, World.Background PreviousBackground, World.Background NextBackground) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
                this.PreviousBackground = PreviousBackground;
                this.NextBackground = NextBackground;
            }
            override internal void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) {
                if (TriggerType == EventTriggerType.Camera) {
                    if (Direction < 0) {
                        World.TargetBackground = this.PreviousBackground;
                        World.TargetBackgroundCountdown = World.TargetBackgroundDefaultCountdown;
                    } else if (Direction > 0) {
                        World.TargetBackground = this.NextBackground;
                        World.TargetBackgroundCountdown = World.TargetBackgroundDefaultCountdown;
                    }
                }
            }
        }
        // fog change
        internal class FogChangeEvent : GeneralEvent {
            internal Game.Fog PreviousFog;
            internal Game.Fog CurrentFog;
            internal Game.Fog NextFog;
            internal FogChangeEvent(double TrackPositionDelta, Game.Fog PreviousFog, Game.Fog CurrentFog, Game.Fog NextFog) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
                this.PreviousFog = PreviousFog;
                this.CurrentFog = CurrentFog;
                this.NextFog = NextFog;
            }
            override internal void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) {
                if (TriggerType == EventTriggerType.Camera) {
                    if (Direction < 0) {
                        Game.PreviousFog = this.PreviousFog;
                        Game.NextFog = this.CurrentFog;
                    } else if (Direction > 0) {
                        Game.PreviousFog = this.CurrentFog;
                        Game.NextFog = this.NextFog;
                    }
                }
            }
        }
        // brightness change
        internal class BrightnessChangeEvent : GeneralEvent {
            internal float CurrentBrightness;
            internal float PreviousBrightness;
            internal double PreviousDistance;
            internal float NextBrightness;
            internal double NextDistance;
            internal BrightnessChangeEvent(double TrackPositionDelta, float CurrentBrightness, float PreviousBrightness, double PreviousDistance, float NextBrightness, double NextDistance) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
                this.CurrentBrightness = CurrentBrightness;
                this.PreviousBrightness = PreviousBrightness;
                this.PreviousDistance = PreviousDistance;
                this.NextBrightness = NextBrightness;
                this.NextDistance = NextDistance;
            }
            override internal void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) { }
        }
        // marker start
        internal class MarkerStartEvent : GeneralEvent {
            internal int TextureIndex;
            internal MarkerStartEvent(double TrackPositionDelta, int TextureIndex) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
                this.TextureIndex = TextureIndex;
            }
            override internal void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) {
                if (TriggerType == EventTriggerType.Camera) {
                    if (Direction < 0) {
                        Game.RemoveMarker(this.TextureIndex);
                    } else if (Direction > 0) {
                        Game.AddMarker(this.TextureIndex);
                    }
                }
            }
        }
        // marker end
        internal class MarkerEndEvent : GeneralEvent {
            internal int TextureIndex;
            internal MarkerEndEvent(double TrackPositionDelta, int TextureIndex) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
                this.TextureIndex = TextureIndex;
            }
            override internal void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) {
                if (TriggerType == EventTriggerType.Camera) {
                    if (Direction < 0) {
                        Game.AddMarker(this.TextureIndex);
                    } else if (Direction > 0) {
                        Game.RemoveMarker(this.TextureIndex);
                    }
                }
            }
        }
        // station pass alarm
        internal class StationPassAlarmEvent : GeneralEvent {
            internal StationPassAlarmEvent(double TrackPositionDelta) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
            }
            override internal void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) { }
        }
        // station start
        internal class StationStartEvent : GeneralEvent {
            internal int StationIndex;
            internal StationStartEvent(double TrackPositionDelta, int StationIndex) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
                this.StationIndex = StationIndex;
            }
            override internal void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) {
                if (TriggerType == EventTriggerType.Camera) {
                    if (Direction < 0) {
                        if (Program.CurrentStation == this.StationIndex) {
                            Program.CurrentStation = -1;
                        }
                    } else if (Direction > 0) {
                        Program.CurrentStation = this.StationIndex;
                    }
                }
            }
        }
        // station end
        internal class StationEndEvent : GeneralEvent {
            internal int StationIndex;
            internal StationEndEvent(double TrackPositionDelta, int StationIndex) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
                this.StationIndex = StationIndex;
            }
            override internal void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) {
                if (TriggerType == EventTriggerType.Camera) {
                    if (Direction < 0) {
                        Program.CurrentStation = this.StationIndex;
                    } else if (Direction > 0) {
                        if (Program.CurrentStation == this.StationIndex) {
                            Program.CurrentStation = -1;
                        }
                    }
                }
            }
        }
        // section change
        internal class SectionChangeEvent : GeneralEvent {
            internal int PreviousSectionIndex;
            internal int NextSectionIndex;
            internal SectionChangeEvent(double TrackPositionDelta, int PreviousSectionIndex, int NextSectionIndex) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
                this.PreviousSectionIndex = PreviousSectionIndex;
                this.NextSectionIndex = NextSectionIndex;
            }
            internal override void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) { }
        }
        // transponder
        internal enum TransponderType {
            None = -1,
            S = 0,
            Sn = 1,
            AccidentalDeparture = 2,
            AtsPPatternOrigin = 3,
            AtsPImmediateStop = 4,
            AtsPTemporarySpeedRestriction = -2,
            AtsPPermanentSpeedRestriction = -3
        }
        internal enum TransponderSpecialSection : int {
            NextRedSection = -2,
        }
        internal class TransponderEvent : GeneralEvent {
            internal TransponderType Type;
            internal bool SwitchSubsystem;
            internal int OptionalInteger;
            internal double OptionalFloat;
            internal int SectionIndex;
            internal TransponderEvent(double TrackPositionDelta, TransponderType Type, bool SwitchSubsystem, int OptionalInteger, double OptionalFloat, int SectionIndex) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
                this.Type = Type;
                this.SwitchSubsystem = SwitchSubsystem;
                this.OptionalInteger = OptionalInteger;
                this.OptionalFloat = OptionalFloat;
                this.SectionIndex = SectionIndex;
            }
            internal override void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) { }
        }
        // limit change
        internal class LimitChangeEvent : GeneralEvent {
            internal double PreviousSpeedLimit;
            internal double NextSpeedLimit;
            internal LimitChangeEvent(double TrackPositionDelta, double PreviousSpeedLimit, double NextSpeedLimit) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
                this.PreviousSpeedLimit = PreviousSpeedLimit;
                this.NextSpeedLimit = NextSpeedLimit;
            }
            internal override void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) { }
        }
        // sound
        internal static bool SuppressSoundEvents = false;
        internal class SoundEvent : GeneralEvent {
            internal int SoundIndex;
            internal bool PlayerTrainOnly;
            internal bool Once;
            internal bool Dynamic;
            internal World.Vector3D Position;
            internal double Speed;
            internal SoundEvent(double TrackPositionDelta, int SoundIndex, bool PlayerTrainOnly, bool Once, bool Dynamic, World.Vector3D Position, double Speed) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
                this.SoundIndex = SoundIndex;
                this.PlayerTrainOnly = PlayerTrainOnly;
                this.Once = Once;
                this.Dynamic = Dynamic;
                this.Position = Position;
                this.Speed = Speed;
            }
            internal override void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) { }
            internal const int SoundIndexTrainPoint = -2;
        }
        // rail sounds change
        internal class RailSoundsChangeEvent : GeneralEvent {
            internal int PreviousRunIndex;
            internal int PreviousFlangeIndex;
            internal int NextRunIndex;
            internal int NextFlangeIndex;
            internal RailSoundsChangeEvent(double TrackPositionDelta, int PreviousRunIndex, int PreviousFlangeIndex, int NextRunIndex, int NextFlangeIndex) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
                this.PreviousRunIndex = PreviousRunIndex;
                this.PreviousFlangeIndex = PreviousFlangeIndex;
                this.NextRunIndex = NextRunIndex;
                this.NextFlangeIndex = NextFlangeIndex;
            }
            internal override void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) { }
        }
        // track end
        internal class TrackEndEvent : GeneralEvent {
            internal TrackEndEvent(double TrackPositionDelta) {
                this.TrackPositionDelta = TrackPositionDelta;
                this.DontTriggerAnymore = false;
            }
            internal override void Trigger(int Direction, EventTriggerType TriggerType, TrainManager.Train Train, int CarIndex) { }
        }

        // ================================

        // track element
        internal enum CantInterpolationMode {
            Linear = 0,
            BiasForward = 1,
            BiasBackward = 2
        }
        internal struct TrackElement {
            internal double StartingTrackPosition;
            internal double CurveRadius;
            internal double CurveCant;
            internal CantInterpolationMode CurveCantInterpolation;
            internal double Inaccuracy;
            internal double AdhesionMultiplier;
            internal World.Vector3D WorldPosition;
            internal World.Vector3D WorldDirection;
            internal World.Vector3D WorldUp;
            internal World.Vector3D WorldSide;
            internal GeneralEvent[] Events;
            internal TrackElement(double StartingTrackPosition) {
                this.StartingTrackPosition = StartingTrackPosition;
                this.CurveRadius = 0.0;
                this.CurveCant = 0.0;
                this.CurveCantInterpolation = CantInterpolationMode.Linear;
                this.Inaccuracy = 0.0;
                this.AdhesionMultiplier = 1.0;
                this.WorldPosition = new World.Vector3D(0.0, 0.0, 0.0);
                this.WorldDirection = new World.Vector3D(0.0, 0.0, 1.0);
                this.WorldUp = new World.Vector3D(0.0, 1.0, 0.0);
                this.WorldSide = new World.Vector3D(1.0, 0.0, 0.0);
                this.Events = new GeneralEvent[] { };
            }
        }

        // track
        internal struct Track {
            internal TrackElement[] Elements;
        }
        internal static Track CurrentTrack;

        // track follower
        internal struct TrackFollower {
            internal int LastTrackElement;
            internal double TrackPosition;
            internal World.Vector3D WorldPosition;
            internal World.Vector3D WorldDirection;
            internal World.Vector3D WorldUp;
            internal World.Vector3D WorldSide;
            internal double CurveRadius;
            internal double CurveCant;
            internal double AdhesionMultiplier;
            internal EventTriggerType TriggerType;
            internal TrainManager.Train Train;
            internal int CarIndex;
        }
        internal static void UpdateTrackFollower(ref TrackFollower Follower, double NewTrackPosition, bool UpdateWorldCoordinates, bool AddTrackInaccurary) {
        	if (CurrentTrack.Elements == null) return;
            if (CurrentTrack.Elements.Length == 0) return;
            int i = Follower.LastTrackElement;
            while (i >= 0 && NewTrackPosition < CurrentTrack.Elements[i].StartingTrackPosition) {
                double ta = Follower.TrackPosition - CurrentTrack.Elements[i].StartingTrackPosition;
                double tb = -0.01;
                CheckEvents(ref Follower, i, -1, ta, tb);
                i--;
            }
            if (i >= 0) {
                while (i < CurrentTrack.Elements.Length - 1) {
                    if (NewTrackPosition < CurrentTrack.Elements[i + 1].StartingTrackPosition) break;
                    double ta = Follower.TrackPosition - CurrentTrack.Elements[i].StartingTrackPosition;
                    double tb = CurrentTrack.Elements[i + 1].StartingTrackPosition - CurrentTrack.Elements[i].StartingTrackPosition + 0.01;
                    CheckEvents(ref Follower, i, 1, ta, tb);
                    i++;
                }
            } else i = 0;
            double da = Follower.TrackPosition - CurrentTrack.Elements[i].StartingTrackPosition;
            double db = NewTrackPosition - CurrentTrack.Elements[i].StartingTrackPosition;
            // track
            if (UpdateWorldCoordinates) {
                if (db != 0.0) {
                    if (CurrentTrack.Elements[i].CurveRadius != 0.0) {
                        // side curve
                        double r = CurrentTrack.Elements[i].CurveRadius;
                        double p = CurrentTrack.Elements[i].WorldDirection.Y / Math.Sqrt(CurrentTrack.Elements[i].WorldDirection.X * CurrentTrack.Elements[i].WorldDirection.X + CurrentTrack.Elements[i].WorldDirection.Z * CurrentTrack.Elements[i].WorldDirection.Z);
                        double s = db / Math.Sqrt(1.0 + p * p);
                        double h = s * p;
                        double b = s / Math.Abs(r);
                        double f = 2.0 * r * r * (1.0 - Math.Cos(b));
                        double c = (double)Math.Sign(db) * Math.Sqrt(f >= 0.0 ? f : 0.0);
                        double a = 0.5 * (double)Math.Sign(r) * b;
                        World.Vector3D D = new World.Vector3D(CurrentTrack.Elements[i].WorldDirection.X, 0.0, CurrentTrack.Elements[i].WorldDirection.Z);
                        World.Normalize(ref D.X, ref D.Y, ref D.Z);
                        double cosa = Math.Cos(a);
                        double sina = Math.Sin(a);
                        World.Rotate(ref D.X, ref D.Y, ref D.Z, 0.0, 1.0, 0.0, cosa, sina);
                        Follower.WorldPosition.X = CurrentTrack.Elements[i].WorldPosition.X + c * D.X;
                        Follower.WorldPosition.Y = CurrentTrack.Elements[i].WorldPosition.Y + h;
                        Follower.WorldPosition.Z = CurrentTrack.Elements[i].WorldPosition.Z + c * D.Z;
                        World.Rotate(ref D.X, ref D.Y, ref D.Z, 0.0, 1.0, 0.0, cosa, sina);
                        Follower.WorldDirection.X = D.X;
                        Follower.WorldDirection.Y = p;
                        Follower.WorldDirection.Z = D.Z;
                        World.Normalize(ref Follower.WorldDirection.X, ref Follower.WorldDirection.Y, ref Follower.WorldDirection.Z);
                        double cos2a = Math.Cos(2.0 * a);
                        double sin2a = Math.Sin(2.0 * a);
                        Follower.WorldSide = CurrentTrack.Elements[i].WorldSide;
                        World.Rotate(ref Follower.WorldSide.X, ref Follower.WorldSide.Y, ref Follower.WorldSide.Z, 0.0, 1.0, 0.0, cos2a, sin2a);
                        World.Cross(Follower.WorldDirection.X, Follower.WorldDirection.Y, Follower.WorldDirection.Z, Follower.WorldSide.X, Follower.WorldSide.Y, Follower.WorldSide.Z, out Follower.WorldUp.X, out Follower.WorldUp.Y, out Follower.WorldUp.Z);
                        Follower.CurveRadius = CurrentTrack.Elements[i].CurveRadius;
                    } else {
                        // straight
                        Follower.WorldPosition.X = CurrentTrack.Elements[i].WorldPosition.X + db * CurrentTrack.Elements[i].WorldDirection.X;
                        Follower.WorldPosition.Y = CurrentTrack.Elements[i].WorldPosition.Y + db * CurrentTrack.Elements[i].WorldDirection.Y;
                        Follower.WorldPosition.Z = CurrentTrack.Elements[i].WorldPosition.Z + db * CurrentTrack.Elements[i].WorldDirection.Z;
                        Follower.WorldDirection = CurrentTrack.Elements[i].WorldDirection;
                        Follower.WorldUp = CurrentTrack.Elements[i].WorldUp;
                        Follower.WorldSide = CurrentTrack.Elements[i].WorldSide;
                        Follower.CurveRadius = 0.0;
                    }
                    // cant
                    if (i < CurrentTrack.Elements.Length - 1) {
                        double t = db / (CurrentTrack.Elements[i + 1].StartingTrackPosition - CurrentTrack.Elements[i].StartingTrackPosition);
                        switch (CurrentTrack.Elements[i].CurveCantInterpolation) {
                            case CantInterpolationMode.BiasForward:
                                t *= t;
                                t = 1.0 - t * t;
                                t = 1.0 - t * t;
                                break;
                            case CantInterpolationMode.BiasBackward:
                                t = 1.0 - t;
                                t *= t;
                                t = 1.0 - t * t;
                                t *= t;
                                break;
                        }
                        Follower.CurveCant = (1.0 - t) * CurrentTrack.Elements[i].CurveCant + t * CurrentTrack.Elements[i + 1].CurveCant;
                    } else {
                        Follower.CurveCant = CurrentTrack.Elements[i].CurveCant;
                    }
                } else {
                    Follower.WorldPosition = CurrentTrack.Elements[i].WorldPosition;
                    Follower.WorldDirection = CurrentTrack.Elements[i].WorldDirection;
                    Follower.WorldUp = CurrentTrack.Elements[i].WorldUp;
                    Follower.WorldSide = CurrentTrack.Elements[i].WorldSide;
                    Follower.CurveRadius = CurrentTrack.Elements[i].CurveRadius;
                    Follower.CurveCant = CurrentTrack.Elements[i].CurveCant;
                }
            }
            Follower.AdhesionMultiplier = CurrentTrack.Elements[i].AdhesionMultiplier;
            if (AddTrackInaccurary) {
                double f = NewTrackPosition;
                f = 0.3121 * Math.Sin(0.9843 * f) + 1.1217 * Math.Sin(0.1874 * f) + 1.6421 * Math.Sin(0.1126 * f) + 1.8421 * Math.Sin(0.2546 * f);
                f *= 0.15 * CurrentTrack.Elements[i].Inaccuracy;
                double g = NewTrackPosition;
                g = 0.3495 * Math.Sin(0.8272 * g) + 1.6321 * Math.Sin(0.2356 * g);
                g *= 0.15 * CurrentTrack.Elements[i].Inaccuracy;
                Follower.WorldPosition.X += f * Follower.WorldSide.X + g * Follower.WorldUp.X;
                Follower.WorldPosition.Y += f * Follower.WorldSide.Y + g * Follower.WorldUp.Y;
                Follower.WorldPosition.Z += f * Follower.WorldSide.Z + g * Follower.WorldUp.Z;
            }
            // events
            CheckEvents(ref Follower, i, Math.Sign(db - da), da, db);
            // finish
            Follower.TrackPosition = NewTrackPosition;
            Follower.LastTrackElement = i;
        }

        // check events
        private static void CheckEvents(ref TrackFollower Follower, int ElementIndex, int Direction, double OldDelta, double NewDelta) {
            if (Direction < 0) {
                for (int j = 0; j < CurrentTrack.Elements[ElementIndex].Events.Length; j++) {
                    if (OldDelta > CurrentTrack.Elements[ElementIndex].Events[j].TrackPositionDelta & NewDelta <= CurrentTrack.Elements[ElementIndex].Events[j].TrackPositionDelta) {
                        TryTriggerEvent(CurrentTrack.Elements[ElementIndex].Events[j], -1, Follower.TriggerType, Follower.Train, Follower.CarIndex);
                    }
                }
            } else if (Direction > 0) {
                for (int j = 0; j < CurrentTrack.Elements[ElementIndex].Events.Length; j++) {
                    if (OldDelta < CurrentTrack.Elements[ElementIndex].Events[j].TrackPositionDelta & NewDelta >= CurrentTrack.Elements[ElementIndex].Events[j].TrackPositionDelta) {
                        TryTriggerEvent(CurrentTrack.Elements[ElementIndex].Events[j], 1, Follower.TriggerType, Follower.Train, Follower.CarIndex);
                    }
                }
            }
        }

    }
}