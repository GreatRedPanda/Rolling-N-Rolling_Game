using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class RollingLogic : MonoBehaviour
{

    public event System.Action<int,int> OnScoreChanged;
    public Pooler Pooler;
    public LevelDataView LevelDataView;
    public BezierSolution.BezierSpline Path;
    public string[] PoolsNames;
    public List<int> TimeDeltas;
    [HideInInspector]
    public List<string> PoolNamesUsing;

    public float SpeedLimit;
    public Transform Parent;

    List<Ball> _balls=new List<Ball>();
    public Transform PortalsObject;
    public   float ballSizePercent;
    public float deltaY = -5;
    public float Speed;
    public float LostSpeed;
    public float LerpSpeed;




    [HideInInspector]
    public float currentSpeed;
    bool lockMove = false;
    bool lost = false;
    bool initialized = false;
    System.Action currentMoveMethod;
    Queue<string> nextBallsPool = new Queue<string>();
    LevelData _levelParams;
    int score;
    int startCount = 10;

    public int HitsCount { get {

            if (hitsCount == 0)
                return 2;
            else
                return hitsCount+2;} }
    int hitsCount = 0;
     float speedIncrease = 5;

    public EffectsPlayer EffectsPlayer;
    List<System.Action> actions = new List<System.Action>();


    public static RollingLogic Instance;


  
    int increaseCOunt = 0;
    float Timer = 0;
    public float speedChangeSpeed = 0.05f;
  
    private void Start()
    {

        Instance = this;
      
        OnScoreChanged += FindObjectOfType<LevelDataView>().SetBestScore;
        CountScore(0);
    }
    public void InitializeLevel(LevelData levelParams, float[] radiuses)
    {
        currentSpeed = Speed;


        initialized = true;    
        FindObjectOfType<PathGenerator>().CreateLevelPath(Path, levelParams, radiuses, deltaY);
        _levelParams = levelParams;
        ballSizePercent = 2f / Path.Length;
        Path.InitializeArcs(Path.Count);
        PortalsObject.position = Path.GetPoint(0);
        createStartBalls();
        speedIncrease =  (1-1 / (Path.Length/100))/Radius*speedChangeSpeed;

        //Debug.Log("SPEADE INCREASE " + speedIncrease);

    }

    void CountScore(int ballsCount)
    {
        
        score += ballsCount*(increaseCOunt+1) +(hitsCount+1) *(increaseCOunt+1);
        if (_levelParams.BestScore < score)
        {
            _levelParams.BestScore = score;
        }

        OnScoreChanged?.Invoke(score, _levelParams.BestScore);
    }

    private void Update()
    {
        if (lost)
            return;

        if (!lockMove)
            currentMoveMethod?.Invoke();
        increasingDifficulty();

    }

   

    void moveBall(Ball b)
    {
        float pg = 0;
        Vector3 newPos = Path.GetArcParametrizedTime(b.Progress, ref pg);
       b.transform.position = Vector3.Lerp(b.transform.position, newPos+b.Offset, Time.deltaTime * LerpSpeed);

    }
    void moveAlongSpline(List<Ball> balls)
    {

        if (balls.Count == 0 || balls[balls.Count - 1].Progress >= 2 * ballSizePercent )
        {
            addBallToChain();
        }
        EffectsPlayer.SetLost(balls[0].Progress);

        if (balls[0].Progress >= 1f)
        {
            currentSpeed = LostSpeed;
            lost = true;
            //currentMoveMethod = () =>
            {

                //moveLost(balls);
                FindObjectOfType<GameDataSaver>().Lose(score);
                FindObjectOfType<LevelDataView>().ShowLostDialog();

            };
        }
    }

    string getRandomPrefab()
    {
        int randIndex = Random.Range(0, PoolNamesUsing.Count);
        return PoolNamesUsing[randIndex];

    }


    bool moveLost(List<Ball> balls)
    {
        bool allDisabled = false;
       
        for (int i = 0; i < balls.Count; i++)
        {

            balls[i].Progress += Time.deltaTime * currentSpeed * ballSizePercent;
                moveBall(balls[i]);
                if (balls[i].Progress >= 1)
                {
                balls[i].gameObject.SetActive(false);
                balls.RemoveAt(i);
                }
            
  
        }
        if ( balls.Count==0)
            allDisabled = true;
        return allDisabled;
    }


    public void GetNewBallPlace(Ray direction)
    {
        if (lost)
            return;
        string poolName = nextBallsPool.Dequeue();
        string newPool = getRandomPrefab();
        nextBallsPool.Enqueue(newPool);
        LevelDataView.SetData(nextBallsPool);
        Ball b = Pooler.GetPooledObject(poolName).GetComponent<Ball>();
        b.gameObject.SetActive(true);
        b.ClearState();
        b.OnBallReachedTarget += OnBallAdded;
        b.StartMove(direction);
    }


    void shiftBalls(int startIndex, bool start=false)
    {

        if (!start)
        {
 
            float progressDistance = 0; // Vector3.Distance(_balls[startIndex].transform.position, _balls[prevIndex].transform.position);

            for (int i = startIndex; i >= 0; i--)
            {
           
              
                _balls[i].Progress += 2 * ballSizePercent;
                float percentProgress = 0;
                _balls[i].transform.position = Path.GetArcParametrizedTime(_balls[i].Progress, ref percentProgress);
                //if (_balls[i].CatchingUp && progressDistance > Radius)
                //    break;

                if(i-1>=0)
                progressDistance = Mathf.Abs(_balls[i].Progress- _balls[i - 1].Progress);

                if (progressDistance > 2 * ballSizePercent)
                {

                    Debug.Log(i +"    "+(i-1));
                    break;
                }
            }
        }

        else
        {
            for (int i = startIndex; i >= 0; i--)
            {
                _balls[i].Progress += 2 * ballSizePercent;
                float percentProgress = 0;
                _balls[i].transform.position = Path.GetArcParametrizedTime(_balls[i].Progress, ref percentProgress);
           

            }
        }
    }
    //void shiftBalls(int prevIndex, int startIndex)
    //{
     
    //    // float progressDistance = _balls[startIndex].Progress - _balls[prevIndex].Progress;

    //    float progressDistance = Vector3.Distance(_balls[startIndex].transform.position, _balls[prevIndex].transform.position);

    //    float startDistance = _balls[prevIndex].Progress;
      
    //        for (int i = startIndex; i >= 0; i--)
    //        {
    //                if (progressDistance < Radius)
    //                {
    //                    if (_balls[i].MainBackward)
    //                        break;
    //                    _balls[i].Progress = startDistance + 2 * ballSizePercent;
    //                    float percentProgress = 0;
    //                    startDistance = _balls[i].Progress;
    //                    _balls[i].transform.position = Path.GetArcParametrizedTime(_balls[i].Progress, ref percentProgress);
    //                    if (_balls[i].CatchingUp)
    //                        break;
    //            progressDistance = Vector3.Distance(_balls[i].transform.position, _balls[i+1].transform.position);


    //        }
    //    }
        
    //}

    void addBallToChain()
    {
       // Ball b = Instantiate(getRandomPrefab());
        string poolName = getRandomPrefab();
        Ball b = Pooler.GetPooledObject(poolName).GetComponent<Ball>();
        b.gameObject.SetActive(true);

       // b.transform.SetParent(Parent);
        b.transform.position = Path.GetPoint(0);
        _balls.Add(b);
        b.Progress = 0;
        b.State = MoveType.forward;
       
    }
   


    Ball getBallByProgress(float progress)
       
    {

        Ball b = null;

        b= _balls.Find(x => Mathf.Abs(x.Progress -progress)<0.005f);
        return b;

    }
    
    void OnBallAdded(int index, Ball b, Ball targetBall, float progress)
    {
   

        index = _balls.IndexOf(targetBall);
        //Debug.Log(index);

        if (index < 0 || index>_balls.Count-1)
            return;
        lockMove = true;

        float percentProgress = 0;

        //closeer to finish
        Vector3 positionLeft = Path.GetArcParametrizedTime(targetBall.Progress+2 * ballSizePercent, ref percentProgress);
        //closeer to start
        Vector3 positionRight = Path.GetArcParametrizedTime(targetBall.Progress - 2 * ballSizePercent, ref percentProgress);

        Vector3 contactPoint = b.transform.position;
        float dtFinish = Vector3.Distance(positionLeft, contactPoint);
        float dtStart = Vector3.Distance(positionRight, contactPoint);
        int newIndex = index;

        if (dtFinish < dtStart)
        {
            newIndex = index;
        }
        else
        {
            newIndex = index+1;
        }


        _balls.Insert(newIndex, b);  // здесь
        b.Progress = targetBall.Progress;
     
        b.transform.position = targetBall.transform.position;

        targetBall.CopyState(b);

        shiftBalls(index);
        if (!checkDestroyAfterbackwards(index, b))

        {
            bool destroyed = getDestroyable(newIndex,false);


            if (!destroyed)
            {

                EffectsPlayer.SpheresCollided();
            }

        }
         lockMove = false;


    }

    bool checkDestroyAfterbackwards(int index, Ball b)
    {
        bool result = false;

        if (b.MainBackward)
            return true;

        for (int i = index - 1; i >= 0; i--)
        {
            if (_balls[i].Type == b.Type)
            {
                if (_balls[i].MainBackward)
                return true;
            }
            else
                break;
        }

        for (int i = index + 1; i < _balls.Count; i++)
        {
            if (_balls[i].Type == b.Type)
            {
                if (_balls[i].MainBackward)
                    return true;
                //if (b.CatchingUp)
                //    return true;
            }
            else
                break;
        }


        return result; 
    }


    bool getDestroyable(int index, bool hitBack)
    {

        int fromIndex;
        int toIndex;
        List<Ball> ballsToDestroy = GetBallsToDestroy(index, _balls, out fromIndex, out toIndex);



        Ball ballDestroyNFrom = null;

        if (fromIndex - 1 >= 0)
            ballDestroyNFrom = _balls[fromIndex - 1];
        Ball ballDestroyNTo = null;

        if (toIndex + 1 < _balls.Count)
            ballDestroyNTo = _balls[toIndex + 1];

        if (ballsToDestroy.Count >= 3)
        {
            EffectsPlayer.PlayEffect(ballsToDestroy);
            DestroyBalls(_balls, ballsToDestroy, hitBack);
          
            fromIndex = _balls.IndexOf(ballDestroyNFrom);


            toIndex = _balls.IndexOf(ballDestroyNTo);

            if (ballDestroyNFrom != null && ballDestroyNTo != null)

            {
                if (ballDestroyNFrom.Type != ballDestroyNTo.Type)
                {
                    SetBallsWait(fromIndex, toIndex, ballDestroyNTo);
                }
                else
                {
                    SetBallsBack(fromIndex, ballDestroyNFrom);
                }
            }

            else
            {
                if (ballDestroyNTo == null && ballDestroyNFrom!=null)
                {
                    fromIndex = _balls.IndexOf(ballDestroyNFrom);
                    SetBallsBack(fromIndex, ballDestroyNFrom);
                }

            }
            return true;
        }



        return false;
    }


    public void SetBallsBack(Ball ballDestroyNFrom, Ball ballDestroyNTo)
    {
        int fromIndex = _balls.IndexOf(ballDestroyNFrom);
        int toIndex = _balls.IndexOf(ballDestroyNTo);
        SetBallsBack(fromIndex, ballDestroyNFrom);

    }
    public void SetBallsBack(int fromIndex,  Ball gapBall)
    {

        for (int i = fromIndex; i >= 0; i--)
        {

            _balls[i].State = MoveType.backward;
            if (_balls[i].CatchingUp || _balls[i].MainBackward)
                break;
        }
        gapBall.SetBackward();

    }


    public void SetBallsWait(Ball ballDestroyNFrom,  Ball ballDestroyNTo)
    {
       int  fromIndex = _balls.IndexOf(ballDestroyNFrom);
       int   toIndex = _balls.IndexOf(ballDestroyNTo);
       SetBallsWait(fromIndex, toIndex, ballDestroyNTo);

    }

    public void SetBallsWait(int fromIndex, int toIndex, Ball gapBall)
    {
        for (int i = fromIndex; i >= 0; i--)
        {
            if (_balls[i].MainBackward )
                break;
            _balls[i].State = MoveType.wait;

        }
        for (int i = toIndex; i < _balls.Count; i++)
        {
            if (_balls[i].State != MoveType.wait && !_balls[i].MainBackward)
                _balls[i].State = MoveType.forward;
        }
        gapBall.SetCatchingUp();

    }

    public void GetBack(Ball b, Ball targetBall)
    {
       
        int index = _balls.IndexOf(b);

        for (int i = index; i >= 0; i--)
        {
            if (_balls[i].MainBackward)
                break;
            if (targetBall!=null)
            _balls[i].State = targetBall.State;
            else
                _balls[i].State = MoveType.forward;
            if (_balls[i].CatchingUp)
                break;
        }
     bool destroy=   getDestroyable(_balls.IndexOf(targetBall), true);
        if(!destroy)
        EffectsPlayer.SpheresCollided();

    }
    public void CaughtUp(Ball b, Ball caughtBall)
    {
        //Debug.Log("CAUGHT UO");
        int prevIndex= _balls.IndexOf(b);
        int index = _balls.IndexOf(caughtBall);
        for (int i = index; i>=0; i--)
        {
            if(_balls[i].MainBackward)
                break;
           
            _balls[i].State = MoveType.forward;
            if (_balls[i].CatchingUp)
                break;
        }
        EffectsPlayer.SpheresCollided();
        //shiftBalls(index+1, prevIndex);
    }


    public void GetBallNeighbourRight(Ball b, out Ball neighbour)
    {

        int index = _balls.IndexOf(b);
        int neighIndex = index - 1;

        if (neighIndex >= 0)
            neighbour = _balls[neighIndex];
        else
            neighbour = null;
    }
    public void GetBallNeighbourLeft(Ball b, out Ball neighbour)
    {

        int index = _balls.IndexOf(b);
        int neighIndex = index +1;

        if (neighIndex<_balls.Count)
            neighbour = _balls[neighIndex];
        else
            neighbour = null;
    }

    public void DestroyBalls(List<Ball> balls, List<Ball> destroyRange, bool afterBackwards)
    {

        if (afterBackwards)
            hitsCount++;
        else
            hitsCount = 0;
        CountScore(destroyRange.Count);
        if (destroyRange.Count > 1)
        {

            for (int i = 0; i < destroyRange.Count; i++)
            {
                destroyRange[i].gameObject.SetActive(false);
                
                balls.Remove(destroyRange[i]);
            }
        }

    }

    public List<Ball> GetBallsToDestroy(int startIndex, List<Ball> balls, out int from, out int to)
    {
        Ball b = balls[startIndex];
        List<Ball> ballsToDestroy = new List<Ball>();

        ballsToDestroy.Add(b);

        from = startIndex;
        to = startIndex;
        Ball prevBall=b;
        for (int i = startIndex-1; i>=0; i--)
        {
            if (balls[i].Type == b.Type)// && Mathf.Abs(prevBall.Progress- balls[i].Progress) <=Speed* 2 *ballSizePercent)
            {

                from = i;
                ballsToDestroy.Add(balls[i]);
                prevBall = balls[i];
            }
            else
                break;
        }
        prevBall = b;
        for (int i = startIndex +1; i <balls.Count; i++)
        {
            if (balls[i].Type == b.Type)// && Mathf.Abs(prevBall.Progress - balls[i].Progress) <= Speed * 2 * ballSizePercent)
            {
                to = i;
                ballsToDestroy.Add(balls[i]);
                prevBall = balls[i];
            }
            else
                break;
        }

        // DestroyBalls(balls, ballsToDestroy);
        return ballsToDestroy;
    }




    public void Restart()
    {
        hitsCount = 0;
        score = 0;
        CountScore(0);
        currentMoveMethod = () => { };
        for (int i = 0; i < _balls.Count; i++)
        {
            _balls[i].gameObject.SetActive(false);

        }
        _balls.Clear();
        nextBallsPool.Clear();

        currentSpeed = Speed;
        createStartBalls();
        lost = false;
    }


    void increasingDifficulty()
    {
        if (increaseCOunt >= TimeDeltas.Count)
        {
            return;
        }
        if (Timer > TimeDeltas[increaseCOunt])
        {
            Timer = 0;

            increaseCOunt++;
            if (PoolNamesUsing.Count < PoolsNames.Length)
            {
                PoolNamesUsing.Add(PoolsNames[PoolNamesUsing.Count]);


            }

            if (currentSpeed < SpeedLimit)
            {
                currentSpeed += currentSpeed*speedIncrease;
            }
        }

        Timer += Time.deltaTime;
    }


    public float Radius;
    void createStartBalls()
    {
        Timer = 0;
    
        increaseCOunt = 0;

        PoolNamesUsing = new List<string>();

        for (int i = 0; i < 3; i++)
        {
            PoolNamesUsing.Add(PoolsNames[i]);
        }
        for (int i = 0; i < startCount; i++)
        {
            string poolName = getRandomPrefab();
            Ball b = Pooler.GetPooledObject(poolName).GetComponent<Ball>();

            Radius = b.GetComponent<Renderer>().bounds.size.x;

            //Debug.Log("RADIUS "+ Radius);
            b.gameObject.SetActive(true);
            _balls.Add(b);
            b.Progress = 2 * ballSizePercent * (startCount - i - 1);
            b.State = MoveType.forward;
        }


        shiftBalls(startCount - 1, true);
        currentMoveMethod = () => { moveAlongSpline(_balls); };
        nextBallsPool.Enqueue(getRandomPrefab());
        nextBallsPool.Enqueue(getRandomPrefab());
        nextBallsPool.Enqueue(getRandomPrefab());
        LevelDataView.SetData(nextBallsPool);

       // EffectsPlayer.playRollSound(true, currentSpeed);
    }
}
