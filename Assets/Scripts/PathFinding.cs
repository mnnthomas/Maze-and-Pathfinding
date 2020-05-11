using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazeGenerator
{
    public class PathFinding : MonoBehaviour
    {
        public Vector2 _StartGridPosition;
        public MazeGenerator _MazeGenerator;

        private Vector2 mCurrentGridPos;

        private void Start()
        {
            mCurrentGridPos = _StartGridPosition;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit; 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.collider.GetComponentInParent<MazeCellData>())
                    {
                        OnMazeClicked(hit.collider.GetComponentInParent<MazeCellData>());
                    }
                }
            }
        }


        private void OnMazeClicked(MazeCellData mazeCell)
        {
            Debug.Log(" >> " + mazeCell._X + " " + mazeCell._Y);
            StartCoroutine(FindPath(mCurrentGridPos, new Vector2(mazeCell._X, mazeCell._Y)));
        }

        IEnumerator FindPath(Vector2 from, Vector2 to)
        {
            mCurrentGridPos = from;
            Vector2 endPos = to;

            while(mCurrentGridPos != to)
            {
                MazeCellData cellToMove = _MazeGenerator._MazeCells[(int)mCurrentGridPos.x, (int)mCurrentGridPos.y];
                List<Movements> possibleMovements = _MazeGenerator.FindNearestMovableCells((int)mCurrentGridPos.x, (int) mCurrentGridPos.y);
                float minimalCost = 0;
                for(int i = 0; i < possibleMovements.Count; i++)
                {
                    float currentCost = CalculateCost(new Vector2(possibleMovements[i]._Cell._X, possibleMovements[i]._Cell._Y), mCurrentGridPos, endPos);
                    if (minimalCost == 0)
                    {
                        minimalCost = currentCost;
                        cellToMove = possibleMovements[i]._Cell;
                    }
                    else
                    {
                        if (currentCost < minimalCost)
                        {
                            minimalCost = currentCost;
                            cellToMove = possibleMovements[i]._Cell;
                        }
                    }
                }
                transform.position = new Vector3(cellToMove._X, 0, cellToMove._Y);
                mCurrentGridPos = new Vector2(cellToMove._X, cellToMove._Y);

                yield return new WaitForSeconds(0.5f);
            }
        }


        private float CalculateCost(Vector2 from, Vector2 start, Vector2 to)
        {
            float FCost = default;
            //Need to find cost in-terms of grid distance
            //temp
            float GCost = Vector2.Distance(start, from);
            float HCost = Vector2.Distance(to, from);
            FCost = GCost + HCost;
            return FCost;
        }
    }
}
