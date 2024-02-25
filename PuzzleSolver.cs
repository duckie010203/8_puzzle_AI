using System.Collections;
using System.Collections.Generic;
using AI;
using Puzzle;
using UnityEngine;
using TMPro;

public class PuzzleSolver : MonoBehaviour
{
    public int countStep = 0;
    public string algo = "A star ";
    [SerializeField] TextMeshProUGUI countStepText;
    public PuzzleState_Viz mPuzzleStateViz;

    private PuzzleNode mCurrentState;
    private PuzzleNode mGoalState;

    // private AStarPathFinder<PuzzleState> mSolver =
    //     new AStarPathFinder<PuzzleState>();

    private PathFinder<PuzzleState> mSolver;

    private PuzzleMap mPuzzle = new PuzzleMap(3);

    void Start()
    {
        mCurrentState = new PuzzleNode(mPuzzle, new PuzzleState(3));
        mGoalState = new PuzzleNode(mPuzzle, new PuzzleState(3));
        mSolver = new GreedyPathFinder<PuzzleState>();
        mSolver.NodeTraversalCost = PuzzleMap.GetCostBetweenTwoCells;
        mSolver.HeuristicCost = PuzzleMap.GetManhattanCost;
    }

    void Update()
    {
        // Solve the puzzle and immediately show
        // the solution.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mPuzzleStateViz.SetPuzzleState(mCurrentState.Value);
            mSolver.Initialize(mCurrentState, mGoalState);

            Solve();
        }
        // if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     if (mSolver.Status == PathFinderStatus.RUNNING)
        //         mSolver.Step();
        //     if (mSolver.Status == PathFinderStatus.SUCCESS)
        //     {
        //         Debug.Log("Found solution. Displaying solution now");
        //         StartCoroutine(ShowSolution());
        //     }
        //     if (mSolver.Status == PathFinderStatus.FAILURE)
        //     {
        //         Debug.Log("Failure");
        //     }
        // }

        // Randomize the puzzle.
        if (Input.GetKeyDown(KeyCode.R))
        {
            Randomize();
        }
        countStepText.text = algo + countStep.ToString();
    }

    IEnumerator Coroutine_Solve()
    {
        // Keep calling step as long as the pathfinder's status
        // is RUNNING.
        while (mSolver.Status == PathFinderStatus.RUNNING)
        {
            mSolver.Step();
            yield return null;
        }

        // SUCCESS.
        // Show the solution in a smooth way.
        if (mSolver.Status == PathFinderStatus.SUCCESS)
        {
            Debug.Log("Found solution. Displaying solution now");
            StartCoroutine(ShowSolution());
        }

        // FAILURE
        // Failed finding path.
        if (mSolver.Status == PathFinderStatus.FAILURE)
        {
            Debug.Log("Failure");
        }
    }

    public void Solve()
    {
        StartCoroutine(Coroutine_Solve());
    }

    IEnumerator Coroutine_Randomize(int depth)
    {
        int i = 0;
        while (i < depth)
        {
            List<Node<PuzzleState>> neighbours =
                mPuzzle.GetNeighbours(mCurrentState);

            // get a random neignbour.
            int rn = Random.Range(0, neighbours.Count);
            mCurrentState.Value.SwapWithEmpty(
                neighbours[rn].Value.GetEmptyTileIndex());
            i++;
            mPuzzleStateViz.SetPuzzleState(mCurrentState.Value);
            yield return null;
        }
    }

    public void Randomize(int depth = 50)
    {
        StartCoroutine(Coroutine_Randomize(depth));
    }

    IEnumerator ShowSolution()
    {
        List<PuzzleState> reverseSolution = new List<PuzzleState>();
        PathFinder<PuzzleState>.PathFinderNode node = mSolver.CurrentNode;
        while (node != null)
        {
            reverseSolution.Add(node.Location.Value);
            node = node.Parent;
        }

        if (reverseSolution.Count > 0)
        {
            mPuzzleStateViz.SetPuzzleState(
                reverseSolution[reverseSolution.Count - 1]);

            if (reverseSolution.Count > 2)
            {
                for (int i = reverseSolution.Count - 2; i >= 0; i -= 1)
                {
                    mPuzzleStateViz.SetPuzzleState(reverseSolution[i], 0.5f);
                    countStep++;
                    yield return new WaitForSeconds(1.0f);
                }
            }
        }
    }
}
