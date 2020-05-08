using UnityEngine;
using System.Collections;



public enum MoveType { none,forward, backward, wait, flight}
public class Ball : MonoBehaviour
{
    public event System.Action<int, Ball, Ball, float> OnBallReachedTarget;
    public string Type;

    [HideInInspector]
    public float Progress;
    [HideInInspector]
    public float CurrentSpeed;

    float FlightSpeed = 5;
    [HideInInspector]
    public float CameraOffsetForward = 5;
    // [HideInInspector]
    public Vector3 Offset;

    Vector3 _targetPos;
    float _targetProgress;
    int _targetIndex;
    Ball _targetBall;
    bool _isMoving = false;
    float deltaPos = 1f;


    public MoveType State = MoveType.none;



    public bool CatchingUp = false;
    public bool MainBackward = false;

    
    Vector3 flyDirection;

    Ray dir;
    private void OnTriggerEnter(Collider other)
    {



        if (State == MoveType.flight)
        {
            if (other.GetComponent<Ball>() != null)
            {

                Ball target = other.GetComponent<Ball>();
                Vector3 contactPosition = transform.position;
                //Debug.Log("CALLED MULTIPLE" + contactPosition);
                _isMoving = false;
                State = MoveType.none;

                OnBallReachedTarget?.Invoke(_targetIndex, this, target, target.Progress);
            }

        }
        //else
        //{
        //    if (other.GetComponent<Ball>() != null)
        //    {
        //        Ball b = other.GetComponent<Ball>();
        //        if (this != b)
        //        {
        //            Ball left;
        //            Ball right;
        //            RollingLogic.Instance.GetBallNeighbourLeft(this, out left);
        //            RollingLogic.Instance.GetBallNeighbourRight(this, out right);
        //            float distance = Mathf.Abs(Progress - b.Progress);
        //           // Debug.Log(distance + );
        //            if (  (b == left || b == right ) &&  distance < 2 * RollingLogic.Instance.ballSizePercent*Time.deltaTime)  // добавить про направление
        //                RollingLogic.Instance.Shift(this);
        //        }

        //    }
        //}
    }

    public void ClearState()
    {
        OnBallReachedTarget = null;
        CatchingUp = false;
        MainBackward = false;
        State = MoveType.none;
    }
    private void OnTriggerStay(Collider other)
    {
        //if(State!=MoveType.flight)
        //if (other.GetComponent<Ball>() != null)
        //{
        //    Ball b = other.GetComponent<Ball>();
        //    if (this != b)
        //    {
        //        Ball left;
        //        Ball right;
        //        RollingLogic.Instance.GetBallNeighbourLeft(this, out left);
        //        RollingLogic.Instance.GetBallNeighbourRight(this, out right);
        //        float distance = Mathf.Abs(Progress - b.Progress);
        //        // Debug.Log(distance + );
        //        if ((b == left || b == right) && distance < 2 * RollingLogic.Instance.ballSizePercent * Time.deltaTime)  // добавить про направление
        //            RollingLogic.Instance.Shift(this);
        //    }

        //}
    }
    private void OnTriggerExit(Collider other)
    {
        if (State == MoveType.flight && other.tag == "Bounds")
        {

            //Debug.Log(other.tag);
            State = MoveType.none;
            gameObject.SetActive(false);
        }
    }


    public void StartMove(Ray direction)
    {

        _isMoving = true;
        //_targetBall = targetBall;
        //_targetIndex = index;
        //_targetPos = targetPos;
        //_targetProgress = targetProgress;
        transform.position = Camera.main.transform.position - Camera.main.transform.forward * CameraOffsetForward;
        flyDirection = direction.direction;
        State = MoveType.flight;
        flightDistance = 1;
        dir = direction;

    }



    private void Update()
    {
        if (State == MoveType.flight)
            fly();
        else if (State == MoveType.forward)
            Move();
        else if (State == MoveType.backward)
        {
            rollBack();

        }

        if (CatchingUp)
            catchingUp();
    }


    float flightDistance = 0;
    void fly()
    {

        //if (!_isMoving)
        //    return;
   
        
            //_targetPos += flyDirection;
            //transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * FlightSpeed);
        flightDistance += Time.deltaTime * FlightSpeed*20;
        transform.position = dir.GetPoint(flightDistance);
        //else
        //{

        //}
    }
    private void Move()
    {
        Progress += Time.deltaTime * RollingLogic.Instance.currentSpeed * RollingLogic.Instance.ballSizePercent;
        float pg = 0;
        Vector3 newPos = RollingLogic.Instance.Path.GetArcParametrizedTime(Progress, ref pg);
        transform.position = Vector3.Lerp(transform.position, newPos , Time.deltaTime * RollingLogic.Instance.LerpSpeed);

    }


    void getNeighbours(out Ball left, out Ball right)
    {

        left = null;
        right = null;

    }

    public void SetCatchingUp()
    {

        CatchingUp = true;


    }

    public void SetBackward()
    {
       
        MainBackward = true;
    }


    void rollBack()
    {
      Progress -= Time.deltaTime * RollingLogic.Instance.currentSpeed* RollingLogic.Instance.HitsCount*2 * RollingLogic.Instance.ballSizePercent;
        float pg = 0;
        Vector3 newPos = RollingLogic.Instance.Path.GetArcParametrizedTime(Progress, ref pg);
        transform.position = Vector3.Lerp(transform.position, newPos , Time.deltaTime * RollingLogic.Instance.LerpSpeed);


        if (MainBackward)
        {
            Ball target;

            RollingLogic.Instance.GetBallNeighbourLeft(this, out target);
            if (target != null)
            {


                if (target.Type == Type)
                {
                    if (Progress <= target.Progress + 2 * RollingLogic.Instance.ballSizePercent)
                    {

                        Progress = target.Progress + 2 * RollingLogic.Instance.ballSizePercent;
                        MainBackward = false;
                        RollingLogic.Instance.GetBack(this, target);

                    }
                }
                else
                {

                    MainBackward = false;
                    RollingLogic.Instance.SetBallsWait(this, target);

                }
            }
            else
            {
                if (Progress <= 0)
                {

                    Progress = 0;
                    MainBackward = false;
                    RollingLogic.Instance.GetBack(this, null);

                }

            }
        }


    }
    void catchingUp()
    {

        Ball target;

        RollingLogic.Instance.GetBallNeighbourRight(this, out target);
        if (target != null)
        {

            if (target.Type != Type)
            {
                if (Progress >= target.Progress - 2 * RollingLogic.Instance.ballSizePercent)
                {
                    CatchingUp = false;
                    Progress = target.Progress - 2 * RollingLogic.Instance.ballSizePercent;
                    RollingLogic.Instance.CaughtUp(this, target);
                }
            }
            else
            {
                CatchingUp = false;
                RollingLogic.Instance.SetBallsBack(   target,this);
            }
        }
        //else
        //    CatchingUp = false;
    }

    public bool DestroyAsterBackToPos()
    {

        bool result = false;

        if (State == MoveType.backward)
            result = true;

        Ball leftNeighbour;
        Ball rightNeighbour;

        RollingLogic.Instance.GetBallNeighbourRight(this, out rightNeighbour);

        RollingLogic.Instance.GetBallNeighbourLeft(this, out leftNeighbour);


        if (rightNeighbour != null)
        {

            if (rightNeighbour.State == MoveType.backward)
                result = true;
        }

        return result;

    }

    public void CopyState(Ball newBall)
    {
        newBall.State = this.State;

        Ball leftNeighbour;
        Ball rightNeighbour;

        RollingLogic.Instance.GetBallNeighbourRight(this, out rightNeighbour);

        RollingLogic.Instance.GetBallNeighbourLeft(this, out leftNeighbour);

        if (newBall==rightNeighbour && CatchingUp)
        {
            this.CatchingUp = false;
            newBall.SetCatchingUp();
        }

        if (newBall == leftNeighbour && MainBackward)
        {
            newBall.SetBackward();
            MainBackward = false;
        }
    }
}
