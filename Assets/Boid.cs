﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using BGE.Geom;

namespace BGE
{
    public class Boid : MonoBehaviour
    {
        // Variables required to implement the boid
        [Header("Boid Attributes")]
        public float mass;
        public float maxSpeed;
        public float maxForce;
        public float forceMultiplier;
        [Range(0.0f, 2.0f)]
        public float timeMultiplier;
        [Range(0.0f, 1.0f)]
        public float damping;
        public enum CalculationMethods { WeightedTruncatedSum, WeightedTruncatedRunningSumWithPrioritisation, PrioritisedDithering };
        public CalculationMethods calculationMethod;
        public float radius;
        public Boolean applyBanking;

        [HideInInspector]
        public Flock flock;

        public bool enforceNonPenetrationConstraint;

        [Header("Debugging")]
        public bool drawGizmos;
        public bool drawForces;
        public bool drawVectors;
        public bool drawNeighbours;

        [Header("Seek")]
        public bool seekEnabled;
        public Vector3 seekTargetPos;
        public float seekWeight;

        [Header("Flee")]
        public bool fleeEnabled;
        public float fleeWeight;
        public GameObject fleeTarget;
        public float fleeRange;
        [HideInInspector]
        public Vector3 fleeForce;

        [Header("Arrive")]
        public bool arriveEnabled;
        public Vector3 arriveTargetPos;
        public float arriveWeight;
        public float arriveSlowingDistance;
        [Range(0.0f, 1.0f)]
        public float arriveDeceleration;

        [Header("Wander")]
        public bool wanderEnabled;
        public float wanderRadius;
        public float wanderJitter;
        public float wanderDistance;
        public float wanderWeight;
        public float wanderNoiseDelta;
        private float wanderNoiseX;
        private float wanderNoiseY;

        [Header("Flocking")]
        public float neighbourDistance;

        [Header("Separation")]
        public bool separationEnabled;
        public float separationWeight;

        [Header("Alignment")]
        public bool alignmentEnabled;
        public float alignmentWeight;

        [Header("Cohesion")]
        public bool cohesionEnabled;
        public float cohesionWeight;

        [Header("Obstacle Avoidance")]
        public bool obstacleAvoidanceEnabled;
        public float minBoxLength;
        public float obstacleAvoidanceWeight;

        [Header("Plane Avoidance")]
        public bool planeAvoidanceEnabled;
        public float planeAvoidanceWeight;

        [Header("Follow Path")]
        public bool followPathEnabled;
        public float followPathWeight;
        public Path path = new Path();

        [Header("Pursuit")]
        public bool pursuitEnabled;
        public GameObject pursuitTarget;
        public float pursuitWeight;

        [Header("Evade")]
        public bool evadeEnabled;
        public GameObject evadeTarget;
        public float evadeWeight;

        [Header("Interpose")]
        public bool interposeEnabled;
        public float interposeWeight;

        [Header("Hide")]
        public bool hideEnabled;
        public float hideWeight;

        [Header("Offset Pursuit")]
        public bool offsetPursuitEnabled;
        public GameObject offsetPursuitTarget;
        public float offsetPursuitWeight;

        [HideInInspector]
        public Vector3 offset;

        [Header("Sphere Constrain")]
        public bool sphereConstrainEnabled;
        public Vector3 sphereCentre;
        public float sphereRadius;
        public float sphereConstrainWeight;

        [Header("Random Walk")]
        public bool randomWalkEnabled;
        public Vector3 randomWalkCenter;
        public float randomWalkWait;

        public float randomWalkRadius;
        public bool randomWalkKeepY;
        public float randomWalkWeight;
        private Vector3 randomWalkForce;

        [Header("Scene Avoidance")]
        public bool sceneAvoidanceEnabled;
        public float sceneAvoidanceWeight;
        public float sceneAvoidanceFeelerDepth;

        [HideInInspector]
        public Vector3 force;

        [HideInInspector]
        public Vector3 velocity;

        [HideInInspector]
        public Vector3 acceleration;

        List<GameObject> tagged = new List<GameObject>();
        List<Vector3> PlaneAvoidanceFeelers = new List<Vector3>();
        List<Vector3> SceneAvoidanceFeelers = new List<Vector3>();

        private Vector3 wanderTargetPos;
        private Vector3 randomWalkTarget;

        Color debugLineColour = Color.cyan;
        float timeDelta;

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            //Gizmos.DrawWireSphere(transform.position, radius);
        }



        public Boid()
        {
            TurnOffAll();

            drawGizmos = false;
            drawForces = false;
            drawVectors = false;
            drawNeighbours = false;
            applyBanking = true;
            flock = null;

            // Set default values
            force = Vector3.zero;
            velocity = Vector3.zero;
            mass = 1.0f;
            damping = 0.01f;
            radius = 5.0f;
            forceMultiplier = 1.0f;
            timeMultiplier = 1.0f;
            neighbourDistance = 10.0f;

            calculationMethod = CalculationMethods.WeightedTruncatedRunningSumWithPrioritisation;

            seekTargetPos = Vector3.zero;
            seekWeight = 1.0f;

            fleeWeight = 1.0f;
            fleeTarget = null;
            fleeRange = 100.0f;

            arriveSlowingDistance = 15.0f;
            arriveDeceleration = 0.9f;
            arriveTargetPos = Vector3.zero;
            arriveWeight = 1.0f;

            wanderRadius = 10.0f;
            wanderDistance = 15.0f;
            wanderJitter = 20.0f;
            wanderWeight = 1.0f;
            wanderNoiseX = 0;
            wanderNoiseDelta = 0.01f;

            cohesionWeight = 1.0f;
            separationWeight = 1.0f;
            alignmentWeight = 1.0f;

            obstacleAvoidanceWeight = 1.0f;
            minBoxLength = 50.0f;

            followPathWeight = 1.0f;

            pursuitWeight = 1.0f;
            pursuitTarget = null;

            evadeTarget = null;
            evadeWeight = 1.0f;

            interposeWeight = 1.0f;
            hideWeight = 1.0f;

            planeAvoidanceWeight = 1.0f;

            offsetPursuitTarget = null;
            offsetPursuitWeight = 1.0f;

            sphereCentre = Vector3.zero;
            sphereConstrainWeight = 1.0f;
            sphereRadius = 1000.0f;

            randomWalkWeight = 1.0f;
            randomWalkCenter = Vector3.zero;
            randomWalkRadius = 500.0f;
            randomWalkKeepY = false;

            randomWalkForce = Vector3.zero;
            randomWalkWait = 5.0f;

            maxSpeed = 20;
            maxForce = 10;

            sceneAvoidanceEnabled = true;
            sceneAvoidanceWeight = 1.0f;
            sceneAvoidanceFeelerDepth = 30.0f;
        }

        void Start()
        {
            wanderTargetPos = UnityEngine.Random.insideUnitSphere * wanderRadius;
            randomWalkTarget = randomWalkCenter + UnityEngine.Random.insideUnitSphere * randomWalkRadius;
            if (randomWalkKeepY)
            {
                randomWalkTarget.y = transform.position.y;
            }
            if (offsetPursuitTarget != null)
            {
                offset = offsetPursuitTarget.transform.position - transform.position;
            }
        }
        #region Flags

        public void TurnOffAll()
        {
            seekEnabled = false;
            fleeEnabled = false;
            arriveEnabled = false;
            wanderEnabled = false;
            cohesionEnabled = false;
            separationEnabled = false;
            alignmentEnabled = false;
            obstacleAvoidanceEnabled = false;
            planeAvoidanceEnabled = false;
            followPathEnabled = false;
            pursuitEnabled = false;
            evadeEnabled = false;
            interposeEnabled = false;
            hideEnabled = false;
            offsetPursuitEnabled = false;
            sphereConstrainEnabled = false;
            randomWalkEnabled = false;
        }


        #endregion

        #region Utilities        
        private void makeFeelers()
        {
            PlaneAvoidanceFeelers.Clear();
            float feelerDistance = 20.0f;
            // Make the forward feeler
            Vector3 newFeeler = Vector3.forward * feelerDistance;
            newFeeler = transform.TransformPoint(newFeeler);
            PlaneAvoidanceFeelers.Add(newFeeler);

            newFeeler = Vector3.forward * feelerDistance;
            newFeeler = Quaternion.AngleAxis(45, Vector3.up) * newFeeler;
            newFeeler = transform.TransformPoint(newFeeler);
            PlaneAvoidanceFeelers.Add(newFeeler);

            newFeeler = Vector3.forward * feelerDistance;
            newFeeler = Quaternion.AngleAxis(-45, Vector3.up) * newFeeler;
            newFeeler = transform.TransformPoint(newFeeler);
            PlaneAvoidanceFeelers.Add(newFeeler);

            newFeeler = Vector3.forward * feelerDistance;
            newFeeler = Quaternion.AngleAxis(45, Vector3.right) * newFeeler;
            newFeeler = transform.TransformPoint(newFeeler);
            PlaneAvoidanceFeelers.Add(newFeeler);

            newFeeler = Vector3.forward * feelerDistance;
            newFeeler = Quaternion.AngleAxis(-45, Vector3.right) * newFeeler;
            newFeeler = transform.TransformPoint(newFeeler);
            PlaneAvoidanceFeelers.Add(newFeeler);
        }

        // These feelers are normals       

        #endregion
        #region Integration

        private bool accumulateForce(ref Vector3 runningTotal, Vector3 force)
        {
            float soFar = runningTotal.magnitude;

            float remaining = maxForce - soFar;
            if (remaining <= 0)
            {
                return false;
            }

            float toAdd = force.magnitude;


            if (toAdd < remaining)
            {
                runningTotal += force;
            }
            else
            {
                runningTotal += Vector3.Normalize(force) * remaining;
            }
            return true;
        }



        private void EnforceNonPenetrationConstraint()
        {
            GameObject[] boids;
            // Just use the tagged boids if we are flocking, otherwise get all the boids
            if (tagged != null)
            {
                boids = tagged.ToArray();
            }
            else
            {
                boids = GameObject.FindGameObjectsWithTag(gameObject.tag);
            }
            foreach (GameObject boid in boids)
            {
                if (boid == gameObject)
                {
                    continue;
                }
                Vector3 toOther = boid.transform.position - gameObject.transform.position;
                float distance = toOther.magnitude;
                float overlap = radius + boid.GetComponent<Boid>().radius - distance;
                if (overlap >= 0)
                {
                    boid.transform.position = (boid.transform.position + (toOther / distance) *
                     overlap);
                }
            }
        }

        public Vector3 Calculate()
        {
            if (calculationMethod == CalculationMethods.WeightedTruncatedRunningSumWithPrioritisation)
            {
                return CalculateWeightedPrioritised();
            }

            return Vector3.zero;
        }

        private Vector3 CalculateWeightedPrioritised()
        {
            Vector3 force = Vector3.zero;
            Vector3 steeringForce = Vector3.zero;

            Utilities.checkNaN(force);
            if (planeAvoidanceEnabled)
            {
                force = PlaneAvoidance() * planeAvoidanceWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (sphereConstrainEnabled)
            {
                force = SphereConstrain(sphereRadius) * sphereConstrainWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (evadeEnabled)
            {
                force = Evade() * evadeWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (fleeEnabled)
            {
                if (fleeTarget != null)
                {
                    force = Flee(fleeTarget.transform.position) * fleeWeight;
                    force *= forceMultiplier;
                    fleeForce = force;
                }
                if (flock != null)
                {
                    force = Vector3.zero;
                    foreach (GameObject enemy in flock.enemies)
                    {
                        if (enemy != null)
                        {
                            force += Flee(enemy.transform.position) * fleeWeight;
                            force *= forceMultiplier;
                        }
                    }
                }
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (seekEnabled)
            {
                force = Seek(seekTargetPos) * seekWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (arriveEnabled)
            {
                force = Arrive(arriveTargetPos) * arriveWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (wanderEnabled)
            {
                force = Wander() * wanderWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (pursuitEnabled)
            {
                force = Pursue() * pursuitWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (offsetPursuitEnabled)
            {
                force = OffsetPursuit(offset) * offsetPursuitWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (followPathEnabled)
            {
                force = FollowPath() * followPathWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            if (randomWalkEnabled)
            {
                force = RandomWalk() * randomWalkWeight;
                force *= forceMultiplier;
                if (!accumulateForce(ref steeringForce, force))
                {
                    return steeringForce;
                }
            }

            return steeringForce;
        }

        void Update()
        {
            float smoothRate;

            timeDelta = Time.deltaTime * timeMultiplier;
            if (flock != null)
            {
                timeDelta *= flock.timeMultiplier;
            }

            force = Calculate();
            if (drawForces)
            {
                Quaternion q = Quaternion.FromToRotation(Vector3.forward, force);
                LineDrawer.DrawArrowLine(transform.position, transform.position + force * 5.0f, Color.magenta, q);
            }
            Utilities.checkNaN(force);
            Vector3 newAcceleration = force / mass;
            if (drawVectors)
            {
                LineDrawer.DrawVectors(transform);
            }


            if (timeDelta > 0.0f)
            {
                smoothRate = Utilities.Clip(9.0f * timeDelta, 0.15f, 0.4f) / 2.0f;
                Utilities.BlendIntoAccumulator(smoothRate, newAcceleration, ref acceleration);
            }

            velocity += acceleration * timeDelta;

            float speed = velocity.magnitude;
            if (speed > maxSpeed)
            {
                velocity.Normalize();
                velocity *= maxSpeed;
            }
            transform.position += velocity * timeDelta;


            // the length of this global-upward-pointing vector controls the vehicle's
            // tendency to right itself as it is rolled over from turning acceleration
            Vector3 globalUp = new Vector3(0, 0.2f, 0);
            // acceleration points toward the center of local path curvature, the
            // length determines how much the vehicle will roll while turning
            Vector3 accelUp = acceleration * 0.05f;
            // combined banking, sum of UP due to turning and global UP
            Vector3 bankUp = accelUp + globalUp;
            // blend bankUp into vehicle's UP basis vector
            smoothRate = timeDelta * 3.0f;
            Vector3 tempUp = transform.up;
            Utilities.BlendIntoAccumulator(smoothRate, bankUp, ref tempUp);

            if (speed > 0.0001f)
            {
                transform.forward = velocity;
                transform.forward.Normalize();
                if (applyBanking)
                {
                    transform.LookAt(transform.position + transform.forward, tempUp);
                }
                // Apply damping
                velocity *= (1.0f - damping);
            }

            if (path != null)
            {
                path.Draw();
            }

            if (enforceNonPenetrationConstraint)
            {
                EnforceNonPenetrationConstraint();
            }
        }

        #endregion

        #region Behaviours

        Vector3 Seek(Vector3 targetPos)
        {
            Vector3 desiredVelocity;

            desiredVelocity = targetPos - transform.position;
            desiredVelocity.Normalize();
            desiredVelocity *= maxSpeed;
            if (drawGizmos)
            {
                LineDrawer.DrawTarget(targetPos, Color.red);
            }
            return (desiredVelocity - velocity);
        }

        Vector3 Evade()
        {
            float dist = (evadeTarget.transform.position - transform.position).magnitude;
            float lookAhead = maxSpeed;

            Vector3 targetPos = evadeTarget.transform.position + (lookAhead * evadeTarget.GetComponent<Boid>().velocity);
            return Flee(targetPos);
        }

        Vector3 OffsetPursuit(Vector3 offset)
        {
            Vector3 target = Vector3.zero;
            target = offsetPursuitTarget.transform.TransformPoint(offset);

            float dist = (target - transform.position).magnitude;

            float lookAhead = (dist / maxSpeed);

            target = target + (lookAhead * offsetPursuitTarget.GetComponent<Boid>().velocity);
            Utilities.checkNaN(target);
            return Arrive(target);
        }

        Vector3 Pursue()
        {
            Vector3 toTarget = pursuitTarget.transform.position - transform.position;
            float dist = toTarget.magnitude;
            float time = dist / maxSpeed;

            Vector3 targetPos = pursuitTarget.transform.position + (time * pursuitTarget.GetComponent<Boid>().velocity);
            if (drawGizmos)
            {
                LineDrawer.DrawTarget(targetPos, Color.red);
                LineDrawer.DrawLine(transform.position, targetPos, Color.cyan);
            }

            return Seek(targetPos);
        }

        Vector3 Flee(Vector3 targetPos)
        {
            Vector3 desiredVelocity;
            desiredVelocity = transform.position - targetPos;
            if (desiredVelocity.magnitude > fleeRange)
            {
                return Vector3.zero;
            }
            if (drawGizmos)
            {
                LineDrawer.DrawSphere(transform.position, fleeRange, 20, Color.yellow);
            }
            desiredVelocity.Normalize();
            desiredVelocity *= maxSpeed;
            return (desiredVelocity - velocity);
        }

        Vector3 RandomWalk()
        {
            float dist = (transform.position - randomWalkTarget).magnitude;
            if (dist < 1)
            {
                StartCoroutine("RandomWalkWait");
                randomWalkTarget = randomWalkCenter + UnityEngine.Random.insideUnitSphere * randomWalkRadius;
                if (randomWalkKeepY)
                {
                    randomWalkTarget.y = transform.position.y;
                }
            }
            return Arrive(randomWalkTarget);
        }

        IEnumerator RandomWalkWait()
        {
            randomWalkEnabled = false;
            yield return new WaitForSeconds(randomWalkWait);
            randomWalkEnabled = true;
        }

        Vector3 Wander()
        {
            float jitterTimeSlice = wanderJitter * timeDelta;

            Vector3 toAdd = UnityEngine.Random.insideUnitSphere * jitterTimeSlice;
            wanderTargetPos += toAdd;
            wanderTargetPos.Normalize();
            wanderTargetPos *= wanderRadius;

            Vector3 localTarget = wanderTargetPos + Vector3.forward * wanderDistance;
            Vector3 worldTarget = transform.TransformPoint(localTarget);
            if (drawGizmos)
            {
                LineDrawer.DrawTarget(worldTarget, Color.blue);
            }
            return (worldTarget - transform.position);
        }

        public Vector3 PlaneAvoidance()
        {
            makeFeelers();

            Plane worldPlane = new Plane(new Vector3(0, 1, 0), 0);
            Vector3 force = Vector3.zero;
            foreach (Vector3 feeler in PlaneAvoidanceFeelers)
            {
                if (!worldPlane.GetSide(feeler))
                {
                    float distance = Math.Abs(worldPlane.GetDistanceToPoint(feeler));
                    force += worldPlane.normal * distance;
                }
            }

            if (force.magnitude > 0.0)
            {
                DrawFeelers();
            }
            return force;
        }

        public void DrawFeelers()
        {
            if (drawGizmos)
            {
                foreach (Vector3 feeler in PlaneAvoidanceFeelers)
                {
                    LineDrawer.DrawLine(transform.position, feeler, Color.green);
                }
            }
        }

        public Vector3 Arrive(Vector3 target)
        {
            Vector3 desired = target - transform.position;

            float distance = desired.magnitude;
            //toTarget.Normalize();
            if (drawGizmos)
            {
                LineDrawer.DrawTarget(target, Color.red);
                LineDrawer.DrawSphere(target, arriveSlowingDistance, 20, Color.yellow);
            }

            if (distance < 1.0f)
            {
                return Vector3.zero;
            }
            float desiredSpeed = 0;
            if (distance < arriveSlowingDistance)
            {
                desiredSpeed = (distance / arriveSlowingDistance) * maxSpeed * (1.0f - arriveDeceleration);
            }
            else
            {
                desiredSpeed = maxSpeed;
            }
            desired *= desiredSpeed;

            return desired - velocity;
        }

        private Vector3 FollowPath()
        {
            float epsilon = 5.0f;
            float dist = (transform.position - path.NextWaypoint()).magnitude;
            if (dist < epsilon)
            {
                path.AdvanceToNext();
            }
            if ((!path.looped) && path.IsLast())
            {
                return Arrive(path.NextWaypoint());
            }
            else
            {
                return Seek(path.NextWaypoint());
            }
        }

        public Vector3 SphereConstrain(float radius)
        {
            Vector3 toTarget = transform.position - sphereCentre;
            Vector3 steeringForce = Vector3.zero;
            if (toTarget.magnitude > radius)
            {
                steeringForce = Vector3.Normalize(toTarget) * (radius - toTarget.magnitude);
            }
            return steeringForce;
        }

        #endregion

    }
}