#define DEBUG_COLLISION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Kinect = Windows.Kinect;

public class Wall : MonoBehaviour
{

    [SerializeField]
    bool checkAngles;

    [SerializeField]
    Vector2 shoulderElbowTargetL;
    [SerializeField]
    Vector2 shoulderElbowTargetR;

    [SerializeField]
    Vector2 elbowWristTargetL;
    [SerializeField]
    Vector2 elbowWristTargetR;

    [SerializeField]
    Vector2 hipKneeTargetL;
    [SerializeField]
    Vector2 hipKneeTargetR;

    [SerializeField]
    Vector2 kneeAnkleTargetL;
    [SerializeField]
    Vector2 kneeAnkleTargetR;

    [SerializeField]
    Vector2 spineMidSpineBaseTarget;

    [SerializeField]
    float maxCrouchHeight;
    [SerializeField]
    float minJumpHeight;

    public enum FailStates
    {
        LeftArm = 1,
        RightArm = 2,
        LeftLeg = 4,
        RightLeg = 8
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //    a*
    //     |  
    //     ------------*b
    //
    float GetAngle(Vector3 a, Vector3 b)
    {
        float dy = b.y - a.y;
        float dx = b.x - a.x;
        return Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
    }

    bool Check(Kinect.Body body, Kinect.JointType a, Kinect.JointType b, Vector2 target, ref float failAngle, ref Vector3 failVector, ref uint failState, uint curJoint)
    {
        float angle = GetAngle(body, a, b);

        float deltaAngle = Mathf.Abs(Mathf.DeltaAngle(angle, target.x));
        if (deltaAngle < target.y) return true;

        if (deltaAngle - target.y > failAngle)
        {
            failVector = GetVector3FromJoint(body.Joints[b]) - GetVector3FromJoint(body.Joints[a]);
            failAngle = deltaAngle;
            failState = curJoint;
        }

        return false;
    }

    // bones we probably care about:
    //
    // - shoulder to elbow
    // - elbow to wrist
    //
    // - spine mid to spine base
    //
    // - hip to knee
    // - knee to ankle
    public bool Permits(Kinect.Body body, ref Vector3 failVector, uint curFailState, out uint failState)
    {
        float failAngle = 0.0f;
        failVector = Vector3.zero;
        failState = 0;

        if (checkAngles)
        {
            if ((curFailState & (uint)FailStates.LeftArm) == 0 && !Check(body, Kinect.JointType.ShoulderLeft, Kinect.JointType.ElbowLeft, shoulderElbowTargetL, ref failAngle, ref failVector, ref failState, (uint)FailStates.LeftArm))
            {
#if DEBUG_COLLISION
                Debug.Log("Failing due to Left Shoulder->Elbow. Angle " + GetAngle(body, Kinect.JointType.ShoulderLeft, Kinect.JointType.ElbowLeft).ToString());
#endif
            }
            if ((curFailState & (uint)FailStates.RightArm) == 0 && !Check(body, Kinect.JointType.ShoulderRight, Kinect.JointType.ElbowRight, shoulderElbowTargetR, ref failAngle, ref failVector, ref failState, (uint)FailStates.RightArm))
            {
#if DEBUG_COLLISION
                Debug.Log("Failing due to Right Shoulder->Elbow. Angle " + GetAngle(body, Kinect.JointType.ShoulderRight, Kinect.JointType.ElbowRight).ToString());
#endif
            }

            if ((curFailState & (uint)FailStates.LeftArm) == 0 && !Check(body, Kinect.JointType.ElbowLeft, Kinect.JointType.WristLeft, elbowWristTargetL, ref failAngle, ref failVector, ref failState, (uint)FailStates.LeftArm))
            {
#if DEBUG_COLLISION
                Debug.Log("Failing due to Left Elbow->Wrist. Angle " + GetAngle(body, Kinect.JointType.ElbowLeft, Kinect.JointType.WristLeft).ToString());
#endif
            }
            if ((curFailState & (uint)FailStates.RightArm) == 0 && !Check(body, Kinect.JointType.ElbowRight, Kinect.JointType.WristRight, elbowWristTargetR, ref failAngle, ref failVector, ref failState, (uint)FailStates.RightArm))
            {
#if DEBUG_COLLISION
                Debug.Log("Failing due to Right Elbow->Wrist. Angle " + GetAngle(body, Kinect.JointType.ElbowRight, Kinect.JointType.WristRight).ToString());
#endif
            }

            if ((curFailState & (uint)FailStates.LeftLeg) == 0 && !Check(body, Kinect.JointType.HipLeft, Kinect.JointType.KneeLeft, hipKneeTargetL, ref failAngle, ref failVector, ref failState, (uint)FailStates.LeftLeg))
            {
#if DEBUG_COLLISION
                Debug.Log("Failing due to Left Hip->Knee. Angle " + GetAngle(body, Kinect.JointType.HipLeft, Kinect.JointType.KneeLeft).ToString());
#endif
            }
            if ((curFailState & (uint)FailStates.RightLeg) == 0 && !Check(body, Kinect.JointType.HipRight, Kinect.JointType.KneeRight, hipKneeTargetR, ref failAngle, ref failVector, ref failState, (uint)FailStates.RightLeg))
            {
#if DEBUG_COLLISION
                Debug.Log("Failing due to Right Hip->Knee. Angle " + GetAngle(body, Kinect.JointType.HipRight, Kinect.JointType.KneeRight).ToString());
#endif
            }

            if ((curFailState & (uint)FailStates.LeftLeg) == 0 && !Check(body, Kinect.JointType.KneeLeft, Kinect.JointType.AnkleLeft, kneeAnkleTargetL, ref failAngle, ref failVector, ref failState, (uint)FailStates.LeftLeg))
            {
#if DEBUG_COLLISION
                Debug.Log("Failing due to Left Knee->Ankle. Angle " + GetAngle(body, Kinect.JointType.KneeLeft, Kinect.JointType.AnkleLeft).ToString());
#endif
            }
            if ((curFailState & (uint)FailStates.RightLeg) == 0 && !Check(body, Kinect.JointType.KneeRight, Kinect.JointType.AnkleRight, kneeAnkleTargetR, ref failAngle, ref failVector, ref failState, (uint)FailStates.RightLeg))
            {
#if DEBUG_COLLISION
                Debug.Log("Failing due to Right Knee->Ankle. Angle " + GetAngle(body, Kinect.JointType.KneeRight, Kinect.JointType.AnkleRight).ToString());
#endif
            }

            float dummyAngle = 0.0f;
            if (!Check(body, Kinect.JointType.SpineMid, Kinect.JointType.SpineBase, spineMidSpineBaseTarget, ref dummyAngle, ref failVector, ref failState, (uint)(1 << Random.Range(0, 4))))
            {
#if DEBUG_COLLISION
                Debug.Log("Failing due to spine. Angle " + GetAngle(body, Kinect.JointType.SpineMid, Kinect.JointType.SpineBase).ToString());
#endif
            }
        }
        else
        {
            if (GetVector3FromJoint(body.Joints[Kinect.JointType.Head]).y > maxCrouchHeight)
            {
                failVector = GetVector3FromJoint(body.Joints[Kinect.JointType.ShoulderLeft]) - GetVector3FromJoint(body.Joints[Kinect.JointType.ShoulderRight]);
                failAngle = 1.0f;
                failState = (uint)(1 << Random.Range(0, 2));
#if DEBUG_COLLISION
            Debug.Log("Failing due to crouch. Head was at " + GetVector3FromJoint(body.Joints[Kinect.JointType.Head]).y.ToString());
#endif
            }

            if (GetVector3FromJoint(body.Joints[Kinect.JointType.FootRight]).y < minJumpHeight || GetVector3FromJoint(body.Joints[Kinect.JointType.FootLeft]).y < minJumpHeight)
            {
                failVector = GetVector3FromJoint(body.Joints[Kinect.JointType.HipLeft]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HipRight]);
                failAngle = 1.0f;
                failState = (uint)(1 << Random.Range(2, 4));
#if DEBUG_COLLISION
            Debug.Log("Failing due to jump. Foot was at " + GetVector3FromJoint(body.Joints[Kinect.JointType.FootRight]).y.ToString());
#endif
            }
        }
        
        return failAngle == 0.0f;
    }

    float GetAngle(Kinect.Body body, Kinect.JointType a, Kinect.JointType b)
    {
        return GetAngle(GetVector3FromJoint(body.Joints[a]), GetVector3FromJoint(body.Joints[b]));
    }

    static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
