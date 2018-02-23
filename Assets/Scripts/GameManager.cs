using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Kinect = Windows.Kinect;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    Wall[] wallPrefabs;

    [SerializeField]
    GameObject envPrefab;

    [SerializeField]
    GameObject flourPrefab;

    [SerializeField]
    GameObject endPrefab;

    [SerializeField]
    GameObject truckPrefab;

    [SerializeField]
    AudioSource splatAudio;

    [SerializeField]
    AudioSource countdownAudio;

    [SerializeField]
    AudioSource passWallAudio;

    [SerializeField]
    AudioSource bgmAudio1;

    [SerializeField]
    AudioSource bgmAudio2;

    [SerializeField]
    AudioSource doorSource;

    [SerializeField]
    AudioSource truckSource;

    [SerializeField]
    Material materialL;

    [SerializeField]
    Material materialR;

    [SerializeField]
    GameObject arm;

    [SerializeField]
    GameObject leg;

    [SerializeField]
    Texture[] tex;

    [SerializeField]
    Camera camL;
    [SerializeField]
    Camera camR;

    [SerializeField]
    GameObject charL;
    [SerializeField]
    GameObject charR;

    [SerializeField]
    AvatarController avatarL;
    [SerializeField]
    AvatarController avatarR;

    [SerializeField]
    GameObject meshL;
    [SerializeField]
    GameObject meshR;

    [SerializeField]
    GameObject railL;
    [SerializeField]
    GameObject railR;

    [SerializeField]
    GameObject railPrefabL;

    [SerializeField]
    GameObject railPrefabR;

    [SerializeField]
    GameObject LshoulderL;
    [SerializeField]
    GameObject LshoulderR;
    [SerializeField]
    GameObject LhipL;
    [SerializeField]
    GameObject LhipR;

    [SerializeField]
    GameObject RshoulderL;
    [SerializeField]
    GameObject RshoulderR;
    [SerializeField]
    GameObject RhipL;
    [SerializeField]
    GameObject RhipR;

    [SerializeField]
    float beat;

    [SerializeField]
    float camMoveSpeed;
    [SerializeField]
    float wallOffset;

    [SerializeField]
    float wallAnimDistance;
    [SerializeField]
    float collisionCheckDistance;

    [SerializeField]
    GameObject envGO;

    [SerializeField]
    GameObject dancerPrefab;

    [SerializeField]
    Slider left;
    [SerializeField]
    Slider right;

    public GameObject BodySourceManager;
    private BodySourceManager _BodyManager;

    int nextWallL;
    int nextWallR;

    bool permitL;
    bool permitR;

    bool playerLdone;
    bool playerRdone;

    bool playerLNoCam = false;

    uint failStateL;
    uint failStateR;

    float endZ;

    float endPrefabZ;

    float camMoveTickL;
    float camMoveTickR;

    GameObject truck;

    //struct Sequence
    //{
    //    public Wall[] walls;

    //    public Sequence(Wall[] _walls)
    //    {
    //        walls = _walls;
    //    }
    //}

    //Sequence[] sequences;

    List<Wall> levelL;
    List<Wall> levelR;

    Vector3 armVelocity;
    Vector3 legVelocity;

    //UI:
    [SerializeField]
    ProgressController progress;
    [SerializeField]
    GameObject waitInfo;
    [SerializeField]
    Text CountText;

    [SerializeField]
    GameObject winL;
    [SerializeField]
    GameObject winR;
    [SerializeField]
    GameObject loseL;
    [SerializeField]
    GameObject loseR;

    bool pause = false;


    //define left right body
    private Kinect.Body BodyL;
    private Kinect.Body BodyR;

    GameObject goL;

    void Start()
    {
        nextWallL = 0;
        nextWallR = 0;

        permitL = false;
        permitR = false;

        playerLdone = false;
        playerRdone = false;

        camMoveTickL = camMoveSpeed;
        camMoveTickR = camMoveSpeed;

        armVelocity = new Vector3(0.0f, 8.0f, 4.0f);
        legVelocity = new Vector3(0.0f, 9.5f, 4.0f);

        materialL.mainTexture = tex[0];
        materialR.mainTexture = tex[0];

        failStateL = 0;
        failStateR = 0;

        CreateWalls();

        // PRECONDITION must come after CreateWalls()
        CreateEnv();

        {
            Vector3 pos = camL.transform.position;
            pos.x = railL.transform.position.x;
            camL.transform.position = pos;
        }

        {
            Vector3 pos = camR.transform.position;
            pos.x = railR.transform.position.x;
            camR.transform.position = pos;
        }

        {
            Vector3 pos = camL.transform.position;
            pos.z += 2.0f;
            charL.transform.position = pos;
        }

        {
            Vector3 pos = camR.transform.position;
            pos.z += 2.0f;
            charR.transform.position = pos;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();

        StartCoroutine(IntroFly());
    }

    IEnumerator IntroFly()
    {
        Vector3 endPosL = camL.transform.position;
        Vector3 endPosR = camR.transform.position;

        Quaternion endQuatL = camL.transform.rotation;
        Quaternion endQuatR = camR.transform.rotation;

        float t = 0.0f;
        //float tVel = 0.0f;

        Vector3 lookPosL = endPosL;
        lookPosL.x = -10.0f;
        lookPosL.y = 10.0f;
        lookPosL.z += 115.0f;
        camL.transform.LookAt(lookPosL);
        Quaternion startQuatL = camL.transform.rotation;

        Vector3 lookPosR = endPosR;
        lookPosR.x = 10.0f;
        lookPosR.y = 10.0f;
        lookPosR.z += 115.0f;
        camR.transform.LookAt(lookPosR);
        Quaternion startQuatR = camR.transform.rotation;

        Vector3 startPosL = endPosL;
        startPosL.z = endZ + 105.0f;
        startPosL.y = 10.0f;
        startPosL.x = -11.0f;

        Vector3 startPosR = endPosR;
        startPosR.z = endZ + 105.0f;
        startPosR.y = 10.0f;
        startPosR.x = 11.0f;

        Vector3 velL = Vector3.zero;
        Vector3 velR = Vector3.zero;

        Vector3 midPointL = (startPosL + endPosL) * 0.5f;
        Vector3 midPointR = (startPosR + endPosR) * 0.5f;

        camL.transform.position = startPosL;
        camR.transform.position = startPosR;

        float distToMidpointL = Vector3.Distance(startPosL, midPointL);
        float distToMidpointR = Vector3.Distance(startPosR, midPointR);

        Vector3 dirL = (endPosL - startPosL).normalized;
        Vector3 dirR = (endPosR - startPosR).normalized;

        yield return new WaitForSeconds(2.0f);

        while (Vector3.Distance(camL.transform.position, endPosL) > 0.02f)
        {
            float factor = Mathf.Max(0.05f, (distToMidpointL - Vector3.Distance(camL.transform.position, midPointL)));

            camL.transform.Translate(Time.deltaTime * factor * 4.0f * dirL, Space.World);
            camR.transform.Translate(Time.deltaTime * factor * 4.0f * dirR, Space.World);

            t = Mathf.Max(t, factor / distToMidpointL);
            camL.transform.rotation = Quaternion.Slerp(startQuatL, endQuatL, t);
            camR.transform.rotation = Quaternion.Slerp(startQuatR, endQuatR, t);

            yield return null;
        }

        camL.transform.position = endPosL;
        camR.transform.position = endPosR;

        bgmAudio1.Play();
        bgmAudio1.Pause();
        StartCoroutine(WaitForBodies());
        yield break;
    }

    void CreateEnv()
    {
        float envWidth = 30f;
        const float envX = 0.0f;
        const float railX = 20.0f;
        int i = 0;
        for (; i < Mathf.FloorToInt(endZ / envWidth) + 1; ++i)
        {
            GameObject envL = Instantiate(envPrefab, new Vector3(envX, -4.15f, i * envWidth), Quaternion.identity);
            GameObject railL = Instantiate(railPrefabL, new Vector3(railX, -4.15f, i * envWidth), Quaternion.identity);

            GameObject envR = Instantiate(envPrefab, new Vector3(-envX, -4.15f, i * envWidth), Quaternion.identity);
            GameObject railR = Instantiate(railPrefabR, new Vector3(-railX, -4.15f, i * envWidth), Quaternion.identity);

            {
                Vector3 scaleR = envR.transform.localScale;
                scaleR.x = -scaleR.x;
                envR.transform.localScale = scaleR;
            }
            {
                Vector3 scaleR = railR.transform.localScale;
                scaleR.x = -scaleR.x;
                railR.transform.localScale = scaleR;
            }
        }
        endPrefabZ = i * envWidth - 15.0f;
        envGO = Instantiate(endPrefab, new Vector3(envX, -4.15f, i * envWidth), Quaternion.identity);
        truck = Instantiate(truckPrefab, new Vector3(envX, -4.15f, i * envWidth + 119.0f), Quaternion.identity);
    }

    void CreateWalls()
    {
        //Sequence seq1 = new Sequence(new Wall[] { wallPrefabs[0], wallPrefabs[1], wallPrefabs[2], wallPrefabs[3], wallPrefabs[4], wallPrefabs[5], wallPrefabs[6], wallPrefabs[7], wallPrefabs[8], wallPrefabs[9]});
        //Sequence seq1 = new Sequence(new Wall[] { redWall, greenWall, blueWall });
        //Sequence seq1 = new Sequence(new Wall[] { wallPrefabs[9] });
        //Sequence seq2 = new Sequence(new Wall[] { wallPrefabs[1], wallPrefabs[2] });

        //sequences = new Sequence[] { seq1, seq2 };

        levelL = new List<Wall>();
        levelR = new List<Wall>();

        float z = wallOffset;
        //foreach (Sequence seq in sequences) {
        for (int i = 0; i < 20; ++i)
        {
            //Sequence seq = seq1;
            //foreach (Wall wall in seq.walls)
            //{
                Wall wall = wallPrefabs[Random.Range(0, 10)];

                Wall w = Instantiate(wall);
                w.transform.position = new Vector3(railL.transform.position.x, -0.15f, beat * z);
                levelL.Add(w);

                Wall rw = Instantiate(wall);
                rw.transform.position = new Vector3(railR.transform.position.x, -0.15f, beat * z);
                levelR.Add(rw);

                z += wallOffset;
            //}
        }

        endZ = Mathf.Max(levelL[levelL.Count - 1].transform.position.z, levelR[levelR.Count - 1].transform.position.z);

        for (int i = 0; i < 3; ++i)
        {
            Wall w = Instantiate(wallPrefabs[Random.Range(0, 10)]);
            w.transform.position = new Vector3(railL.transform.position.x, -0.15f, 100000.0f);
            levelL.Add(w);

            Wall rw = Instantiate(wallPrefabs[Random.Range(0, 10)]);
            rw.transform.position = new Vector3(railR.transform.position.x, -0.15f, 100000.0f);
            levelR.Add(rw);
        }
    }

    IEnumerator WaitForBodies()
    {
        if (pause)
            yield break;

        pause = true;
        yield return null;
        bgmAudio1.Pause();

        //TODO:
        // 1)appear waiting infomation(UI)

        avatarL.hidden = true;
        avatarR.hidden = true;

        waitInfo.SetActive(true);

        for (;;)
        {
            Kinect.Body[] data = _BodyManager.GetData();
            if (data == null)
            {
                yield return null;
                continue;
            }

            List<Kinect.Body> bodies = new List<Kinect.Body>();
            foreach (Kinect.Body body in data)
            {
                if (body != null && body.IsTracked) bodies.Add(body);
            }

            if (bodies.Count == 2)
            {
                // todo: define left and right body
                float b0_x = bodies[0].Joints[Kinect.JointType.SpineBase].Position.X;
                float b1_x = bodies[1].Joints[Kinect.JointType.SpineBase].Position.X;


                if (b0_x < b1_x)
                {
                    BodyL = bodies[0];
                    BodyR = bodies[1];
                }
                else
                {
                    BodyL = bodies[1];
                    BodyR = bodies[0];
                }

                //Debug.Log("b0x = " + b0_x.ToString());
                //Debug.Log("b1x = " + b1_x.ToString());

                avatarL.hidden = false;
                avatarR.hidden = false;
                break;
            }
            yield return null;
        }

        //TODO:
        //0)disappear waiting information
        waitInfo.SetActive(false);
        //1)Prepare to go( 3 ,2 ,1, GO!): UI

        countdownAudio.PlayOneShot(countdownAudio.clip);

        CountText.text = "3";

        yield return new WaitForSeconds(1);
        CountText.text = "2";

        yield return new WaitForSeconds(1);
        CountText.text = "1";

        yield return new WaitForSeconds(1);
        //CountText.text = "Start!";
        pause = false;

        for (int i = 0; i < 5; ++i)
        {
            if (charL.transform.position.z > levelL[i].transform.position.z - wallAnimDistance)
            {
                levelL[i].GetComponent<Animator>().SetBool("isPlay", true);
            }
            else
            {
                break;
            }
        }

        for (int i = 0; i < 5; ++i)
        {
            if (charR.transform.position.z > levelR[i].transform.position.z - wallAnimDistance)
            {
                levelR[i].GetComponent<Animator>().SetBool("isPlay", true);
            }
            else
            {
                break;
            }
        }

        StartCoroutine(MoveCams());
        StartCoroutine(GameUpdateR());
        StartCoroutine(GameUpdateL());

        bgmAudio1.UnPause();

        CountText.text = "";
    }

    IEnumerator BgmFade()
    {
        float vel = 0.0f;
        while (bgmAudio1.volume > 0.01f)
        {
            bgmAudio1.volume = Mathf.SmoothDamp(bgmAudio1.volume, 0.0f, ref vel, 0.4f);
            yield return null;
        }
        bgmAudio1.volume = 0.0f;

        vel = 0.0f;
        bgmAudio2.volume = 0.0f;
        bgmAudio2.Play();
        while (bgmAudio2.volume < 0.99f)
        {
            bgmAudio2.volume = Mathf.SmoothDamp(bgmAudio2.volume, 1.0f, ref vel, 0.9f);
            yield return null;
        }
        bgmAudio2.volume = 1.0f;
    }

    IEnumerator GameUpdateR()
    {
        for (;;)
        {
            if (pause)
            {
                break;
            }

            if (playerRdone)
            {
                break;
            }

            Kinect.Body[] data = _BodyManager.GetData();
            if (data == null)
            {
                yield return null;
                continue;
            }

            List<Kinect.Body> bodies = new List<Kinect.Body>();
            foreach (Kinect.Body body in data)
            {
                if (body != null && body.IsTracked) bodies.Add(body);
            }

            if (bodies.Count != 2)
            {
                //Debug.LogError("Body count not 1 or 2! Got " + bodies.Count.ToString());
                //Debug.LogError("Body count not good, Got " + bodies.Count.ToString());

                StartCoroutine(WaitForBodies());
                break;

                //if (bodies.Count == 0)
                //{
                //    yield return null;
                //    continue;
                //}
            }

            //PlayerR

            //TODO:
            //1)Same to left player

            if (charR.transform.position.z > levelR[nextWallR].transform.position.z - wallAnimDistance)
            {
                levelR[nextWallR].GetComponent<Animator>().SetBool("isPlay", true);
            }

            if (charR.transform.position.z > levelR[nextWallR + 1].transform.position.z - wallAnimDistance)
            {
                levelR[nextWallR + 1].GetComponent<Animator>().SetBool("isPlay", true);
            }

            if (charR.transform.position.z > endPrefabZ)
            {
                // We win! Script motion, land in truck...
                playerRdone = true;

                Vector3 angleDir = Quaternion.Euler(0.0f, -17.7f, 0.0f) * Vector3.forward;

                while (charR.transform.position.z < endPrefabZ + 57.0f)
                {
                    camR.transform.Translate(3 * camMoveTickR * beat * Time.deltaTime * angleDir, Space.World);
                    charR.transform.Translate(3 * camMoveTickR * beat * Time.deltaTime * angleDir, Space.World);
                    yield return null;
                }

                while (charR.transform.position.z < endPrefabZ + 117.0f)
                {
                    camR.transform.Translate(3 * camMoveTickR * beat * Time.deltaTime * Vector3.forward, Space.World);
                    charR.transform.Translate(3 * camMoveTickR * beat * Time.deltaTime * Vector3.forward, Space.World);
                    yield return null;
                }

                charR.transform.position = new Vector3(charR.transform.position.x, charR.transform.position.y, endPrefabZ + 117.0f);

                yield return null;

                GameObject goR = avatarR.gameObject;
                Destroy(avatarR);

                GameObject dancer = Instantiate(dancerPrefab);
                dancer.transform.localScale = goR.transform.localScale;
                dancer.transform.rotation = goR.transform.rotation;
                dancer.transform.position = goR.transform.position;

                Destroy(goR);
                goR = dancer;
                
                // jump in box
                float jumpVel = 3.5f;

                float startSizeFactor = 4.0f;
                float endSizeFactor = 3.5f;
                float sizeVel = 0.0f;

                float startRot = 0.0f;
                float endRot = 180.0f;
                float rotVel = 0.0f;

                float rot = startRot;
                while (rot < 175f)
                {
                    jumpVel -= 0.1f;
                    goR.transform.Translate(new Vector3(-1.0f * Time.deltaTime, Time.deltaTime * jumpVel, Mathf.Min(endPrefabZ + 122.0f, 0.87f * camMoveTickR * beat * Time.deltaTime)), Space.World);

                    if (goR.transform.position.z > endPrefabZ + 122.0f) {
                        Vector3 pos = goR.transform.position;
                        pos.z = endPrefabZ + 122.0f;
                        goR.transform.position = pos;
                    }

                    if (goR.transform.position.y < 1.0f && rot > 80.0f)
                    {
                        Vector3 pos = goR.transform.position;
                        pos.y = 1.0f;
                        goR.transform.position = pos;
                    }

                    float scale = Mathf.SmoothDamp(goR.transform.localScale.x, endSizeFactor, ref sizeVel, 0.5f);
                    goR.transform.localScale = scale * Vector3.one;

                    rot = Mathf.SmoothDamp(rot, endRot, ref rotVel, 0.8f);

                    goR.transform.eulerAngles = new Vector3(0.0f, rot, 0.0f);

                    yield return null;
                }

                /////////////////////

                while (!playerLdone)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(0.5f);

                // end the game
                truck.GetComponentInChildren<Animator>().SetBool("isLeaving", true);

                StartCoroutine(BgmFade());

                yield return new WaitForSeconds(1.0f);
                //yield return new WaitForSeconds(0.1f);

                truckSource.Play();
                yield return new WaitForSeconds(1.2f);

                goL.SetActive(false);
                goR.SetActive(false);

                yield return new WaitForSeconds(1.0f);

                doorSource.Play();

                yield return new WaitForSeconds(0.6f);

                envGO.GetComponent<Animator>().SetBool("isDoor", true);

                yield return new WaitForSeconds(2.5f);

                Vector3 cVel = Vector3.zero;
                Vector3 endCamL = new Vector3(camL.transform.position.x, camL.transform.position.y + 3.0f, camL.transform.position.z + 21.0f);
                Vector3 endCamR = new Vector3(camR.transform.position.x, camR.transform.position.y + 3.0f, camR.transform.position.z + 21.0f);
                for (; ;) {
                    camL.transform.position = Vector3.SmoothDamp(camL.transform.position, endCamL, ref cVel, 1.2f);
                    camR.transform.position = Vector3.SmoothDamp(camR.transform.position, endCamR, ref cVel, 1.2f);
                    yield return null;
                }

                break;
            }

            //if (bodies.Count == 2 && charR.transform.position.z > level[nextWallR].transform.position.z - collisionCheckDistance)
            if (charR.transform.position.z > levelR[nextWallR].transform.position.z - collisionCheckDistance)
            {
                Vector3 failVecR = Vector3.zero;
                uint newFailStateR = 0;
                if (levelR[nextWallR].Permits(BodyR, ref failVecR, failStateR, out newFailStateR))
                {
                    permitR = true;
                }

                if (charR.transform.position.z >= levelR[nextWallR].transform.position.z)
                {
                    if (permitR)
                    {
                        passWallAudio.PlayOneShot(passWallAudio.clip);
                        //Debug.Log("Player R made it past wall " + nextWallR.ToString() + "!");
                    }
                    else
                    {
                        failStateR |= newFailStateR;
                        materialR.mainTexture = tex[failStateR];

                        GameObject limb = null;
                        Vector3 limbVelocity = Vector3.zero;
                        switch ((Wall.FailStates)newFailStateR)
                        {
                            case Wall.FailStates.LeftArm:
                                {
                                    limb = Instantiate(arm);
                                    limb.transform.position = LshoulderR.transform.position;
                                    limbVelocity = armVelocity;
                                    Debug.Log("Left arm is gone!");
                                }
                                break;
                            case Wall.FailStates.RightArm:
                                {
                                    limb = Instantiate(arm);
                                    limb.transform.position = RshoulderR.transform.position;
                                    limbVelocity = armVelocity;
                                    Debug.Log("Right arm is gone!");
                                }
                                break;
                            case Wall.FailStates.LeftLeg:
                                {
                                    limb = Instantiate(leg);
                                    limb.transform.position = LhipR.transform.position;
                                    limbVelocity = legVelocity;
                                    Debug.Log("Left leg is gone!");
                                }
                                break;
                            case Wall.FailStates.RightLeg:
                                {
                                    limb = Instantiate(leg);
                                    limb.transform.position = RhipR.transform.position;
                                    limbVelocity = legVelocity;
                                    Debug.Log("Right leg is gone!");
                                }
                                break;
                        }

                        limb.transform.forward = failVecR;
                        Rigidbody rb = limb.AddComponent<Rigidbody>();
                        rb.velocity = limbVelocity + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                        rb.angularVelocity = Random.Range(2.0f, 4.0f) * Random.onUnitSphere;
                        GenerateParticle(failVecR, charR.transform.position);
                        splatAudio.PlayOneShot(splatAudio.clip);

                        Vector3 camPos = camR.transform.position;
                        Vector3 shakeVel = Vector3.zero;
                        Quaternion rot = camR.transform.rotation;

                        {
                            float moveVel = 0.0f;
                            while (camMoveTickR > 0.03f)
                            {
                                camMoveTickR = Mathf.SmoothDamp(camMoveTickR, 0.0f, ref moveVel, 0.1f);
                                Vector3 pos = Vector3.SmoothDamp(camR.transform.position, camPos + Random.Range(0.0f, 0.4f * (camMoveSpeed - camMoveTickR) / camMoveSpeed) * Random.onUnitSphere, ref shakeVel, 0.2f);
                                pos.z = camR.transform.position.z;
                                camR.transform.position = pos;
                                camR.transform.rotation = Quaternion.Euler(Random.Range(-3.0f * (camMoveSpeed - camMoveTickR) / camMoveSpeed, 3.0f * (camMoveSpeed - camMoveTickR) / camMoveSpeed), Random.Range(-3.0f * (camMoveSpeed - camMoveTickR) / camMoveSpeed, 3.0f * (camMoveSpeed - camMoveTickR) / camMoveSpeed), Random.Range(-3.0f * (camMoveSpeed - camMoveTickR) / camMoveSpeed, 3.0f * (camMoveSpeed - camMoveTickR) / camMoveSpeed)) * rot;
                                yield return null;
                            }
                            camR.transform.rotation = rot;
                            camMoveTickR = 0.03f;
                        }

                        yield return new WaitForSeconds(1.0f);
                        if ((failStateR & (uint)Wall.FailStates.LeftLeg) != 0 && (failStateR & (uint)Wall.FailStates.RightLeg) != 0)
                        {
                            failStateR = 0;
                            materialR.mainTexture = tex[failStateR];
                            yield return new WaitForSeconds(0.5f);
                            meshR.GetComponent<SkinnedMeshRenderer>().enabled = false;
                            yield return new WaitForSeconds(0.5f);
                            meshR.GetComponent<SkinnedMeshRenderer>().enabled = true;
                            yield return new WaitForSeconds(0.5f);
                            meshR.GetComponent<SkinnedMeshRenderer>().enabled = false;
                            yield return new WaitForSeconds(0.5f);
                            meshR.GetComponent<SkinnedMeshRenderer>().enabled = true;
                        }

                        {
                            float vel = 0.0f;
                            while (camMoveTickR < camMoveSpeed - 0.1f)
                            {
                                camMoveTickR = Mathf.SmoothDamp(camMoveTickR, camMoveSpeed, ref vel, 0.6f);
                                Vector3 pos = Vector3.SmoothDamp(camR.transform.position, camPos, ref shakeVel, 0.3f);
                                pos.z = camR.transform.position.z;
                                camR.transform.position = pos;
                                yield return null;
                            }
                            camMoveTickR = camMoveSpeed;
                        }

                    }

                    ++nextWallR;
                    permitR = false;
                }
            }
            progress.SetRightValue(charR.transform.position.z / endZ);

            yield return null;
        }
    }

    IEnumerator GameUpdateL()
    {
        for (;;)
        {
            if (pause)
            {
                break;
            }

            Kinect.Body[] data = _BodyManager.GetData();
            if (data == null)
            {
                yield return null;
                continue;
            }

            List<Kinect.Body> bodies = new List<Kinect.Body>();
            foreach (Kinect.Body body in data)
            {
                if (body != null && body.IsTracked) bodies.Add(body);
            }

            if (bodies.Count != 2)
            {

                //Debug.LogError("Body count not 1 or 2! Got " + bodies.Count.ToString());
                //Debug.LogError("Body count Got " + bodies.Count.ToString());

                StartCoroutine(WaitForBodies());
                break;

                //if (bodies.Count == 0)
                //{
                //    yield return null;
                //    continue;
                //}
            }

            if (charL.transform.position.z > levelL[nextWallL].transform.position.z - wallAnimDistance)
            {
                levelL[nextWallL].GetComponent<Animator>().SetBool("isPlay", true);
            }

            if (charL.transform.position.z > levelL[nextWallL + 1].transform.position.z - wallAnimDistance)
            {
                levelL[nextWallL + 1].GetComponent<Animator>().SetBool("isPlay", true);
            }

            if (charL.transform.position.z > endPrefabZ)
            {
                // We win! Script motion, land in truck...

                playerLNoCam = true;

                Vector3 angleDir = Quaternion.Euler(0.0f, 17.7f, 0.0f) * Vector3.forward;

                while (charL.transform.position.z < endPrefabZ + 57.0f)
                {
                    camL.transform.Translate(3 * camMoveTickL * beat * Time.deltaTime * angleDir, Space.World);
                    charL.transform.Translate(3 * camMoveTickL * beat * Time.deltaTime * angleDir, Space.World);
                    yield return null;
                }

                while (charL.transform.position.z < endPrefabZ + 117.0f)
                {
                    camL.transform.Translate(3 * camMoveTickL * beat * Time.deltaTime * Vector3.forward, Space.World);
                    charL.transform.Translate(3 * camMoveTickL * beat * Time.deltaTime * Vector3.forward, Space.World);
                    yield return null;
                }

                charL.transform.position = new Vector3(charL.transform.position.x, charL.transform.position.y, endPrefabZ + 117.0f);

                yield return null;

                goL = avatarL.gameObject;
                Destroy(avatarL);

                GameObject dancer = Instantiate(dancerPrefab);
                dancer.transform.localScale = goL.transform.localScale;
                dancer.transform.rotation = goL.transform.rotation;
                dancer.transform.position = goL.transform.position;

                Destroy(goL);
                goL = dancer;

                // jump in box
                float jumpVel = 3.5f;

                float startSizeFactor = 4.0f;
                float endSizeFactor = 3.5f;
                float sizeVel = 0.0f;

                float startLot = 0.0f;
                float endLot = -180.0f;
                float rotVel = 0.0f;

                float rot = startLot;
                while (rot > -175f)
                {
                    jumpVel -= 0.1f;
                    goL.transform.Translate(new Vector3(1.0f * Time.deltaTime, Time.deltaTime * jumpVel, 0.87f * camMoveTickL * beat * Time.deltaTime), Space.World);

                    if (goL.transform.position.z > endPrefabZ + 122.0f) {
                        Vector3 pos = goL.transform.position;
                        pos.z = endPrefabZ + 122.0f;
                        goL.transform.position = pos;
                    }

                    if (goL.transform.position.y < 1.0f && rot < -80.0f)
                    {
                        Vector3 pos = goL.transform.position;
                        pos.y = 1.0f;
                        goL.transform.position = pos;
                    }

                    float scale = Mathf.SmoothDamp(goL.transform.localScale.x, endSizeFactor, ref sizeVel, 0.5f);
                    goL.transform.localScale = scale * Vector3.one;

                    rot = Mathf.SmoothDamp(rot, endLot, ref rotVel, 0.8f);

                    goL.transform.eulerAngles = new Vector3(0.0f, rot, 0.0f);

                    yield return null;
                }

                playerLdone = true;

                break;
            }

            if (charL.transform.position.z > levelL[nextWallL].transform.position.z - collisionCheckDistance)
            {
                Vector3 failVecL = Vector3.zero;
                uint newFailStateL = 0;
                if (levelL[nextWallL].Permits(BodyL, ref failVecL, failStateL, out newFailStateL))
                {
                    permitL = true;
                }

                if (charL.transform.position.z >= levelL[nextWallL].transform.position.z)
                {
                    if (permitL)
                    {
                        Debug.Log("Player L made it past wall " + nextWallL.ToString() + "!");
                        passWallAudio.PlayOneShot(passWallAudio.clip);
                    }
                    else
                    {
                        failStateL |= newFailStateL;
                        materialL.mainTexture = tex[failStateL];

                        GameObject limb = null;
                        Vector3 limbVelocity = Vector3.zero;
                        switch ((Wall.FailStates)newFailStateL)
                        {
                            case Wall.FailStates.LeftArm:
                                {
                                    limb = Instantiate(arm);
                                    limb.transform.position = LshoulderL.transform.position;
                                    limbVelocity = armVelocity;
                                    //Debug.Log("Left arm is gone!");
                                }
                                break;
                            case Wall.FailStates.RightArm:
                                {
                                    limb = Instantiate(arm);
                                    limb.transform.position = LshoulderL.transform.position;
                                    limbVelocity = armVelocity;
                                    //Debug.Log("Light arm is gone!");
                                }
                                break;
                            case Wall.FailStates.LeftLeg:
                                {
                                    limb = Instantiate(leg);
                                    limb.transform.position = LhipL.transform.position;
                                    limbVelocity = legVelocity;
                                    //Debug.Log("Left leg is gone!");
                                }
                                break;
                            case Wall.FailStates.RightLeg:
                                {
                                    limb = Instantiate(leg);
                                    limb.transform.position = LhipL.transform.position;
                                    limbVelocity = legVelocity;
                                    //Debug.Log("Light leg is gone!");
                                }
                                break;
                        }

                        limb.transform.forward = failVecL;
                        Rigidbody rb = limb.AddComponent<Rigidbody>();
                        rb.velocity = limbVelocity + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                        rb.angularVelocity = Random.Range(4.0f, 7.0f) * Random.onUnitSphere;



                        GenerateParticle(failVecL, charL.transform.position);
                        splatAudio.PlayOneShot(splatAudio.clip);

                        Vector3 camPos = camL.transform.position;
                        Vector3 shakeVel = Vector3.zero;
                        Quaternion rot = camL.transform.rotation;

                        {
                            float moveVel = 0.0f;
                            while (camMoveTickL > 0.03f)
                            {
                                camMoveTickL = Mathf.SmoothDamp(camMoveTickL, 0.0f, ref moveVel, 0.1f);
                                Vector3 pos = Vector3.SmoothDamp(camL.transform.position, camPos + Random.Range(0.0f, 0.4f * (camMoveSpeed - camMoveTickL) / camMoveSpeed) * Random.onUnitSphere, ref shakeVel, 0.2f);
                                pos.z = camL.transform.position.z;
                                camL.transform.position = pos;
                                camL.transform.rotation = Quaternion.Euler(Random.Range(-3.0f * (camMoveSpeed - camMoveTickL) / camMoveSpeed, 3.0f * (camMoveSpeed - camMoveTickL) / camMoveSpeed), Random.Range(-3.0f * (camMoveSpeed - camMoveTickL) / camMoveSpeed, 3.0f * (camMoveSpeed - camMoveTickL) / camMoveSpeed), Random.Range(-3.0f * (camMoveSpeed - camMoveTickL) / camMoveSpeed, 3.0f * (camMoveSpeed - camMoveTickL) / camMoveSpeed)) * rot;
                                yield return null;
                            }
                            camL.transform.rotation = rot;
                            camMoveTickL = 0.03f;
                        }

                        yield return new WaitForSeconds(1.0f);
                        if ((failStateL & (uint)Wall.FailStates.LeftLeg) != 0 && (failStateL & (uint)Wall.FailStates.RightLeg) != 0)
                        {
                            failStateL = 0;
                            materialL.mainTexture = tex[failStateL];
                            yield return new WaitForSeconds(0.5f);
                            meshL.GetComponent<SkinnedMeshRenderer>().enabled = false;
                            yield return new WaitForSeconds(0.5f);
                            meshL.GetComponent<SkinnedMeshRenderer>().enabled = true;
                            yield return new WaitForSeconds(0.5f);
                            meshL.GetComponent<SkinnedMeshRenderer>().enabled = false;
                            yield return new WaitForSeconds(0.5f);
                            meshL.GetComponent<SkinnedMeshRenderer>().enabled = true;
                        }

                        {
                            float vel = 0.0f;
                            while (camMoveTickL < camMoveSpeed - 0.1f)
                            {
                                camMoveTickL = Mathf.SmoothDamp(camMoveTickL, camMoveSpeed, ref vel, 0.6f);
                                Vector3 pos = Vector3.SmoothDamp(camL.transform.position, camPos, ref shakeVel, 0.3f);
                                pos.z = camL.transform.position.z;
                                camL.transform.position = pos;
                                yield return null;
                            }
                            camMoveTickL = camMoveSpeed;
                        }
                    }
                    ++nextWallL;
                    permitL = false;
                }
            }
            progress.SetLeftValue(charL.transform.position.z / endZ);

            yield return null;
        }
    }


    void GenerateParticle(Vector3 forward, Vector3 pos)
    {
        GameObject flour = Instantiate(flourPrefab);
        flour.transform.forward = new Vector3(0.0f, 1.0f, 0.0f).normalized;
        flour.transform.position = pos - 1.5f * Vector3.up + 2.0f * Vector3.forward;
    }

    IEnumerator MoveCams()
    {
        for (;;)
        {

            if (pause)
            {
                break;
            }

            if (!playerLdone && !playerLNoCam)
            {
                camL.transform.Translate(camMoveTickL * beat * Time.deltaTime * Vector3.forward, Space.World);
                charL.transform.Translate(0.0f, 0.0f, (camMoveTickL * beat * Time.deltaTime * Vector3.forward).z, Space.World);
            }

            if (!playerRdone)
            {
                camR.transform.Translate(camMoveTickR * beat * Time.deltaTime * Vector3.forward, Space.World);
                charR.transform.Translate(0.0f, 0.0f, (camMoveTickR * beat * Time.deltaTime * Vector3.forward).z, Space.World);
            }

            yield return null;
        }
    }
}
