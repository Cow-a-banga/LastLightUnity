using UnityEngine;

namespace StarterAssets.States
{
    public class MovementFunctions
    {
        public static bool IsGrounded(Vector3 position, float groundedOffset, float groundedRadius, LayerMask groundLayers)
        {
            var spherePosition = new Vector3(position.x, position.y - groundedOffset,
                position.z);
            return Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
                QueryTriggerInteraction.Ignore);
        }

        public static void Move(
            PlayerProperties properties,
            Transform transform,
            bool isSwimming = false,
            float swimmingMovementY = 0.0f)

        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = isSwimming ? properties.SwimSpeed : properties.Input.sprint ? properties.SprintSpeed : properties.MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (properties.Input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(properties.Controller.velocity.x, 0.0f, properties.Controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = properties.Input.analogMovement ? properties.Input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                properties.Speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * properties.SpeedChangeRate);

                // round speed to 3 decimal places
                properties.Speed = Mathf.Round(properties.Speed * 1000f) / 1000f;
            }
            else
            {
                properties.Speed = targetSpeed;
            }

            properties.AnimationBlend = Mathf.Lerp(properties.AnimationBlend, targetSpeed, Time.deltaTime * properties.SpeedChangeRate);
            if (properties.AnimationBlend < 0.01f) properties.AnimationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(properties.Input.move.x, 0.0f, properties.Input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (properties.Input.move != Vector2.zero)
            {
                properties.TargetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                 properties.MainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, properties.TargetRotation, ref properties.RotationVelocity,
                    properties.RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, properties.TargetRotation, 0.0f) * Vector3.forward;
            Vector3 movement = targetDirection.normalized * (properties.Speed * Time.deltaTime);

            if (isSwimming && properties.VerticalVelocity < 0.0f)
            {
                movement.y = swimmingMovementY;
            }
            else
            {
                movement.y = properties.VerticalVelocity * Time.deltaTime;
            }

            properties.Controller.Move(movement);

            properties.Animator.SetFloat(properties.AnimIDSpeed, properties.AnimationBlend);
            properties.Animator.SetFloat(properties.AnimIDMotionSpeed, inputMagnitude);
        }
    }
}