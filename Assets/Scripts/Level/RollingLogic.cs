using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class RollingLogic : MonoBehaviour
{

    public event System.Action<int,int> OnScoreChanged;



     public static RollingLogic Instance;


    public EffectsPlayer EffectsPlayer;
    public Pooler Pooler;
    public LevelDataView LevelDataView;
    public BezierSolution.BezierSpline Path;
    public string[] PoolsNames;
    public List<int> TimeDeltas;
    [HideInInspector]
    public List<string> PoolNamesUsing;
    public float SpeedLimit;
    public Transform Parent;
    public Transform PortalsObject;
    public   float BallSizePercent;
    public float DeltaY = -5;
    public float Speed;
    public float LostSpeed;
    public float LerpSpeed;

    public float Radius;
    public float speedChangeSpeed = 0.05f;

    public int HitsCount
    {
        get
        {

            if (hitsCount == 0)
                return 2;
            else
                return hitsCount + 2;
        }
    }

    [HideInInspector]
    public float CurrentSpeed;

    System.Action currentMoveMethod;

    List<Ball> balls=new List<Ball>();
    bool lockMove = false;
    bool lost = false;
    bool initialized = false; 
    Queue<string> nextBallsPool = new Queue<string>();
    LevelData levelParams;
    int score;
    int startCount = 10;
    int hitsCount = 0;
    float speedIncrease = 5;
    int increaseCOunt = 0;
    float timer = 0;



    private void Start()
    {

        Instance = this;
      
        OnScoreChanged += FindObjectOfType<LevelDataView>().SetBestScore;

    }



    public void InitializeLevel(LevelData levelParams, float[] radiuses)
    {
        CurrentSpeed = Speed;

        initialized = true;    
        FindObjectOfType<PathGenerator>().CreateLevelPath(Path, levelParams, radiuses, DeltaY);
        this.levelParams = levelParams;
        BallSizePercent = 2f / Path.Length;
        Path.InitializeArcs(Path.Count);
        PortalsObject.position = Path.GetPoint(0);
        createStartBalls();
        speedIncrease =  (1-1 / (Path.Length/100))/Radius*speedChangeSpeed;
        CountScore(0);
    }



    void CountScore(int ballsCount)
    {
        
        score += ballsCount*(increaseCOunt+1) +(hitsCount+1) *(increaseCOunt+1);
        if (levelParams.BestScore < score)
        {
            levelParams.BestScore = score;
        }

        OnScoreChanged?.Invoke(score, levelParams.BestScore);
    }


    private void Update()
    {
        if (lost)
            return;

        if (!lockMove)
            currentMoveMethod?.Invoke();
        increasingDifficulty();

    }

   

    //void moveBall(Ball b)
    //{
    //    float pg = 0;
    //    Vector3 newPos = Path.GetArcParametrizedTime(b.Progress, ref pg);
    //   b.transform.position = Vector3.Lerp(b.transform.position, newPos+b.Offset, Time.deltaTime * LerpSpeed);

    //}
    void moveAlongSpline(List<Ball> balls)
    {

        if (balls.Count == 0 || balls[balls.Count - 1].Progress >= 2 * BallSizePercent )
        {
            addBallToChain();
        }
        EffectsPlayer.SetLost(balls[0].Progress);

        if (balls[0].Progress >= 1f)
        {
            CurrentSpeed = LostSpeed;
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


    //bool moveLost(List<Ball> balls)
    //{
    //    bool allDisabled = false;
       
    //    for (int i = 0; i < balls.Count; i++)
    //    {

    //        balls[i].Progress += Time.deltaTime * currentSpeed * ballSizePercent;
    //            moveBall(balls[i]);
    //            if (balls[i].Progress >= 1)
    //            {
    //            balls[i].gameObject.SetActive(false);
    //            balls.RemoveAt(i);
    //            }
            
  
    //    }
    //    if ( balls.Count==0)
    //        allDisabled = true;
    //    return allDisabled;
    //}


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
           
              
                balls[i].Progress += 2 * BallSizePercent;
                float percentProgress = 0;
                balls[i].transform.position = Path.GetArcParametrizedTime(balls[i].Progress, ref percentProgress);
                //if (_balls[i].CatchingUp && progressDistance > Radius)
                //    break;

                if(i-1>=0)
                progressDistance = Mathf.Abs(balls[i].Progress- balls[i - 1].Progress);

                if (progressDistance > 2 * BallSizePercent)
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
                balls[i].Progress += 2 * BallSizePercent;
                float percentProgress = 0;
                balls[i].transform.position = Path.GetArcParametrizedTime(balls[i].Progress, ref percentProgress);
           

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
        balls.Add(b);
        b.Progress = 0;
        b.State = MoveType.forward;
       
    }
   


    Ball getBallByProgress(float progress)
       
    {

        Ball b = null;

        b= balls.Find(x => Mathf.Abs(x.Progress -progress)<0.005f);
        return b;

    }
    
    void OnBallAdded(int index, Ball b, Ball targetBall, float progress)
    {
   

        index = balls.IndexOf(targetBall);
        //Debug.Log(index);

        if (index < 0 || index>balls.Count-1)
            return;
        lockMove = true;

        float percentProgress = 0;

        //closeer to finish
        Vector3 positionLeft = Path.GetArcParametrizedTime(targetBall.Progress+2 * BallSizePercent, ref percentProgress);
        //closeer to start
        Vector3 positionRight = Path.GetArcParametrizedTime(targetBall.Progress - 2 * BallSizePercent, ref percentProgress);

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


        balls.Insert(newIndex, b); 
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
            if (balls[i].Type == b.Type)
            {
                if (balls[i].MainBackward)
                return true;
            }
            else
                break;
        }

        for (int i = index + 1; i < balls.Count; i++)
        {
            if (balls[i].Type == b.Type)
            {
                if (balls[i].MainBackward)
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
        List<Ball> ballsToDestroy = GetBallsToDestroy(index, balls, out fromIndex, out toIndex);



        Ball ballDestroyNFrom = null;

        if (fromIndex - 1 >= 0)
            ballDestroyNFrom = balls[fromIndex - 1];
        Ball ballDestroyNTo = null;

        if (toIndex + 1 < balls.Count)
            ballDestroyNTo = balls[toIndex + 1];

        if (ballsToDestroy.Count >= 3)
        {
            EffectsPlayer.PlayEffect(ballsToDestroy);
            DestroyBalls(balls, ballsToDestroy, hitBack);
          
            fromIndex = balls.IndexOf(ballDestroyNFrom);


            toIndex = balls.IndexOf(ballDestroyNTo);

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
                    fromIndex = balls.IndexOf(ballDestroyNFrom);
                    SetBallsBack(fromIndex, ballDestroyNFrom);
                }

            }
            return true;
        }



        return false;
    }


    public void SetBallsBack(Ball ballDestroyNFrom, Ball ballDestroyNTo)
    {
        int fromIndex = balls.IndexOf(ballDestroyNFrom);
        int toIndex = balls.IndexOf(ballDestroyNTo);
        SetBallsBack(fromIndex, ballDestroyNFrom);

    }
    public void SetBallsBack(int fromIndex,  Ball gapBall)
    {

        for (int i = fromIndex; i >= 0; i--)
        {

            balls[i].State = MoveType.backward;
            if (balls[i].CatchingUp || balls[i].MainBackward)
                break;
        }
        gapBall.SetBackward();

    }


    public void SetBallsWait(Ball ballDestroyNFrom,  Ball ballDestroyNTo)
    {
       int  fromIndex = balls.IndexOf(ballDestroyNFrom);
       int   toIndex = balls.IndexOf(ballDestroyNTo);
       SetBallsWait(fromIndex, toIndex, ballDestroyNTo);

    }

    public void SetBallsWait(int fromIndex, int toIndex, Ball gapBall)
    {
        for (int i = fromIndex; i >= 0; i--)
        {
            if (balls[i].MainBackward )
                break;
            balls[i].State = MoveType.wait;

        }
        for (int i = toIndex; i < balls.Count; i++)
        {
            if (balls[i].State != MoveType.wait && !balls[i].MainBackward)
                balls[i].State = MoveType.forward;
        }
        gapBall.SetCatchingUp();

    }

    public void GetBack(Ball b, Ball targetBall)
    {
       
        int index = balls.IndexOf(b);

        for (int i = index; i >= 0; i--)
        {
            if (balls[i].MainBackward)
                break;
            if (targetBall!=null)
            balls[i].State = targetBall.State;
            else
                balls[i].State = MoveType.forward;
            if (balls[i].CatchingUp)
                break;
        }
     bool destroy=   getDestroyable(balls.IndexOf(targetBall), true);
        if(!destroy)
        EffectsPlayer.SpheresCollided();

    }
    public void CaughtUp(Ball b, Ball caughtBall)
    {
        
        int prevIndex= balls.IndexOf(b);
        int index = balls.IndexOf(caughtBall);
        for (int i = index; i>=0; i--)
        {
            if(balls[i].MainBackward)
                break;
           
            balls[i].State = MoveType.forward;
            if (balls[i].CatchingUp)
                break;
        }
        EffectsPlayer.SpheresCollided();
        //shiftBalls(index+1, prevIndex);
    }


    public void GetBallNeighbourRight(Ball b, out Ball neighbour)
    {

        int index = balls.IndexOf(b);
        int neighIndex = index - 1;

        if (neighIndex >= 0)
            neighbour = balls[neighIndex];
        else
            neighbour = null;
    }
    public void GetBallNeighbourLeft(Ball b, out Ball neighbour)
    {

        int index = balls.IndexOf(b);
        int neighIndex = index +1;

        if (neighIndex<balls.Count)
            neighbour = balls[neighIndex];
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
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].gameObject.SetActive(false);

        }
        balls.Clear();
        nextBallsPool.Clear();

        CurrentSpeed = Speed;
        createStartBalls();
        lost = false;
    }


    void increasingDifficulty()
    {
        if (increaseCOunt >= TimeDeltas.Count)
        {
            return;
        }
        if (timer > TimeDeltas[increaseCOunt])
        {
            timer = 0;

            increaseCOunt++;
            if (PoolNamesUsing.Count < PoolsNames.Length)
            {
                PoolNamesUsing.Add(PoolsNames[PoolNamesUsing.Count]);


            }

            if (CurrentSpeed < SpeedLimit)
            {
                CurrentSpeed += CurrentSpeed*speedIncrease;
            }
        }

        timer += Time.deltaTime;
    }


  
    void createStartBalls()
    {
        timer = 0;
    
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
            balls.Add(b);
            b.Progress = 2 * BallSizePercent * (startCount - i - 1);
            b.State = MoveType.forward;
        }


        shiftBalls(startCount - 1, true);
        currentMoveMethod = () => { moveAlongSpline(balls); };
        nextBallsPool.Enqueue(getRandomPrefab());
        nextBallsPool.Enqueue(getRandomPrefab());
        nextBallsPool.Enqueue(getRandomPrefab());
        LevelDataView.SetData(nextBallsPool);

       // EffectsPlayer.playRollSound(true, currentSpeed);
    }
}
