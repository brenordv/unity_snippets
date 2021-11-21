using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts
{
    /// <summary>
    ///     To hold min and max value without repeating code.
    /// </summary>
    [Serializable]
    public class MovementValueParameters
    {
        public int minimum = 1;
        public int maximum = 3;
    }
    
    /// <summary>
    ///     Segregating variables involved in the movement process.
    /// </summary>
    [Serializable]
    public class MovementConfig
    {

        public MovementValueParameters rotation;
        public MovementValueParameters walk;
        public float moveSpeed = 3f;
        public float rotateSpeed = 100f;

        public int RotationLeftOrRight => Random.Range(0, 3);
        public int RotationTime => Random.Range(rotation.minimum, rotation.maximum);
        public int RotationWait => Random.Range(rotation.minimum, rotation.maximum);
        public int WalkTime => Random.Range(walk.minimum, walk.maximum);
        public int WalkWait => Random.Range(walk.minimum, walk.maximum);        
    }
    
    
    /// <summary>
    ///     This class will make the AI wander around blindly.
    ///     It will move freely, but there's no way for it to notice an obstacle and will keep bumping on it until
    /// it's time to turn around again.
    ///
    ///     On the upside, this is a simple script that do not require any extra setup.
	///
	///		Tested on Unity 2021.2.2f1
    /// </summary>
    public class SimpleAIWanderAroundBindly : MonoBehaviour
    {
        public StrandConfig strandConfig;
        
        [Tooltip("When turned on, will make the game object move around.")]
        public bool wanderAround = true;
        
        [Tooltip("Settings for moving and turning.")]
        public MovementConfig movementConfig;
        

        private bool _isWandering;
        private bool _isWalking;
        private bool _isRotating;
        private bool _isRotatingRight;

        private void Update()
        {
            MoveOnUpdate();
        }

        /// <summary>
        ///     Movement routine.
        /// </summary>
        private void MoveOnUpdate()
        {
            if (!wanderAround) return;
            
            if (!_isWandering)
            {
                StartCoroutine(WanderAround());
            }

            if (_isRotating)
            {
                var eulers = (_isRotatingRight ? -movementConfig.rotateSpeed : movementConfig.rotateSpeed) * Time.deltaTime * transform.up;
                transform.Rotate(eulers);
            }

            if (!_isWalking) return;

            var transform1 = transform;
            transform1.position += transform1.forward * movementConfig.moveSpeed * Time.deltaTime;
        }


        /// <summary>
        ///     Controls the "state machine" for moving around.
        /// </summary>
        /// <returns>Nothing useful</returns>
        private IEnumerator WanderAround()
        {
            _isWandering = true;
            var rotateDir = movementConfig.RotationLeftOrRight;

            yield return new WaitForSeconds(movementConfig.WalkWait);
            _isWalking = true;

            yield return new WaitForSeconds(movementConfig.WalkTime);
            _isWalking = false;

            yield return new WaitForSeconds(movementConfig.RotationWait);
            if (rotateDir is 1 or 2)
            {
                _isRotatingRight = rotateDir == 2;
                _isRotating = true;
                yield return new WaitForSeconds(movementConfig.RotationTime);
                _isRotating = false;
            }

            _isWandering = false;
        }
    }
}