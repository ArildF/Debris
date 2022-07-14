using UnityEngine;
using static UnityEngine.Mathf;

namespace Player
{
    public class PlayerViewInfo
    {
        private readonly ViewAction[] _actions = 
        {
            new ForwardViewAction(),
            new BackViewAction(),
            new LeftViewAction(),
            new RightViewAction(),
            new UpViewAction(),
            new DownViewAction(),
        };
        public ViewDirection ViewDirection { get; set; }
        public Vector3 UpDirection { get; set; }
        public Transform ShipTransform { get; set; }

        public Quaternion ViewRotation => ViewDirection switch
        {
            ViewDirection.Front => Quaternion.identity,
            ViewDirection.Back => Quaternion.AngleAxis(180, Vector3.up),
            ViewDirection.Left => Quaternion.AngleAxis(90, Vector3.up),
            ViewDirection.Right => Quaternion.AngleAxis(-90, Vector3.up),
            ViewDirection.Top => Quaternion.AngleAxis(90, Vector3.right),
            ViewDirection.Bottom => Quaternion.AngleAxis(-90, Vector3.right),
            _ => Quaternion.identity,
        };

        public ViewAction CurrentView => _actions[(int)ViewDirection];

        private class ForwardViewAction : ViewAction
        {
            public override void PrimaryThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.forwardForce, control.transform.forward);
            }

            public override void PrimaryReverseThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.reverseForce, -control.transform.forward);
            }

            public override void LateralRoll(PlayerControl control, float value)
            {
                Roll(control, new (0, 0, -value * control.lateralRollForce));
            }

            public override void MedialRoll(PlayerControl control, float value)
            {
                Roll(control, new Vector3(value * control.medialRollForce, 0, 0));
            }

            public override void LateralThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.lateralThrustForce, -control.transform.right);
            }

            public override void VerticalThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.verticalThrustForce, -control.transform.up);
            }
        }
        
        private class BackViewAction : ViewAction
        {
            public override void PrimaryThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.reverseForce, -control.transform.forward);
            }

            public override void PrimaryReverseThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.forwardForce, control.transform.forward);
            }

            public override void LateralRoll(PlayerControl control, float value)
            {
                Roll(control, new (0, 0, value * control.lateralRollForce));
            }

            public override void MedialRoll(PlayerControl control, float value)
            {
                Roll(control, new Vector3(-value * control.medialRollForce, 0, 0));
            }

            public override void LateralThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.lateralThrustForce, control.transform.right);
            }

            public override void VerticalThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.verticalThrustForce, control.transform.up);
            }
        }
        
        private class LeftViewAction : ViewAction
        {
            public override void PrimaryThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.lateralThrustForce, -control.transform.right);
            }

            public override void PrimaryReverseThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.lateralThrustForce, control.transform.right);
            }

            public override void LateralRoll(PlayerControl control, float value)
            {
                Roll(control, new Vector3(value * control.medialRollForce, 0, 0));
            }

            public override void MedialRoll(PlayerControl control, float value)
            {
                Roll(control, new (0, 0, value * control.lateralRollForce));
            }

            public override void LateralThrust(PlayerControl control, float value)
            {
                var force = value * (value > 0 ? control.forwardForce : control.reverseForce);
                DirectionalThrust(control, force, control.transform.forward);
            }

            public override void VerticalThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.verticalThrustForce, -control.transform.up);
            }
        }
        
        private class RightViewAction : ViewAction
        {
            public override void PrimaryThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.lateralThrustForce, control.transform.right);
            }

            public override void PrimaryReverseThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.lateralThrustForce, -control.transform.right);
            }

            public override void LateralRoll(PlayerControl control, float value)
            {
                Roll(control, new Vector3(-value * control.medialRollForce, 0, 0));
            }

            public override void MedialRoll(PlayerControl control, float value)
            {
                Roll(control, new (0, 0, -value * control.lateralRollForce));
            }

            public override void LateralThrust(PlayerControl control, float value)
            {
                var force = value * (value > 0 ? control.reverseForce : control.forwardForce);
                DirectionalThrust(control, force, control.transform.forward);
            }

            public override void VerticalThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.verticalThrustForce, -control.transform.up);
            }
        }
        
        private class UpViewAction : ViewAction
        {
            public override void PrimaryThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.lateralThrustForce, control.transform.up);
            }

            public override void PrimaryReverseThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.lateralThrustForce, -control.transform.up);
            }

            public override void LateralRoll(PlayerControl control, float value)
            {
                Roll(control, new Vector3(0, -value * control.medialRollForce,  0));
            }

            public override void MedialRoll(PlayerControl control, float value)
            {
                Roll(control, new (value * control.lateralRollForce, 0, 0));
            }

            public override void VerticalThrust(PlayerControl control, float value)
            {
                var force = value * (value > 0 ? control.forwardForce : control.reverseForce);
                DirectionalThrust(control, force, control.transform.forward);
            }

            public override void LateralThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.lateralThrustForce, control.transform.right);
            }
        }
        
        private class DownViewAction : ViewAction
        {
            public override void PrimaryThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.lateralThrustForce, -control.transform.up);
            }

            public override void PrimaryReverseThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.lateralThrustForce, control.transform.up);
            }

            public override void LateralRoll(PlayerControl control, float value)
            {
                Roll(control, new Vector3(0, value * control.medialRollForce,  0));
            }

            public override void MedialRoll(PlayerControl control, float value)
            {
                Roll(control, new (value * control.lateralRollForce, 0, 0));
            }

            public override void VerticalThrust(PlayerControl control, float value)
            {
                var force = value * (value > 0 ? control.reverseForce : control.forwardForce);
                DirectionalThrust(control, force, -control.transform.forward);
            }

            public override void LateralThrust(PlayerControl control, float value)
            {
                DirectionalThrust(control, value * control.lateralThrustForce, -control.transform.right);
            }
        }
    }

    public abstract class ViewAction
    {
        public abstract void PrimaryThrust(PlayerControl control, float value);
        public abstract void PrimaryReverseThrust(PlayerControl control, float value);
        public abstract void LateralRoll(PlayerControl control, float value);
        public abstract void MedialRoll(PlayerControl control, float value);
        public abstract void LateralThrust(PlayerControl control, float value);
        public abstract void VerticalThrust(PlayerControl control, float value);

        protected void DirectionalThrust(PlayerControl control, float force, Vector3 direction)
        {
            control.rigidBody.AddForce(direction * force, ForceMode.Impulse);
            control.thrustInfo.CurrentDirectionalThrust = Max(control.thrustInfo.CurrentDirectionalThrust, Abs(force));
        }

        protected void Roll(PlayerControl control, Vector3 roll)
        {
            control.thrustInfo.CurrentRotationalThrust = Max(control.thrustInfo.CurrentDirectionalThrust, Abs(roll.magnitude * control.medialRollForce));
            control.rigidBody.AddRelativeTorque(roll);
        }
    }
    
    

    public enum ViewDirection
    {
        Front,
        Back,
        Left,
        Right,
        Top,
        Bottom
    }
}