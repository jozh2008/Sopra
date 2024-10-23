using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Priority_Queue;
using TheFrozenDesert.GamePlayObjects;

namespace TheFrozenDesert.Pathfinding
{
    public sealed class PathFinder
    {
        private readonly GridGraph mGraph;

        public PathFinder(int width, int height)
        {
            mGraph = new GridGraph(width, height);
        }

        public void PathBlockNodeAt(int x, int y)
        {
            mGraph.NodeFromGridPosition(x, y).BlockPath();
        }

        public void PathUnblockNodeAt(int x, int y)
        {
            mGraph.NodeFromGridPosition(x, y).UnblockPath();
        }

        public bool NodeIsBlockedAt(int x, int y, Grid grid)
        {
            return mGraph.NodeIsBlockedAt(x, y, grid);
        }

        public List<Point> FindShortestGridPath(Point start, Point destination, Grid grid, bool neighborDestination = false)
        {
            var destinationNode = mGraph.NodeFromGridPosition(destination.X, destination.Y);
            if (mGraph.NodeIsBlockedAt(destinationNode.mGridPosition.X, destinationNode.mGridPosition.Y, grid) && !neighborDestination)
            {
                return new List<Point>();
            }

            var startNode = mGraph.NodeFromGridPosition(start.X, start.Y);
            startNode.SetStart();
            var path = FindShortestPath(startNode, destinationNode, grid);
            startNode.UnSetStart();
            mGraph.ClearNodes();
            return path;
        }

        private List<Point> FindShortestPath(GridNode start, GridNode destination, Grid grid)
        {
            var currentNodes = new SimplePriorityQueue<GridNode, float>();
            var nodePath = new Dictionary<GridNode, GridNode>();
            var currentCost = new Dictionary<GridNode, float>();
            currentNodes.Enqueue(destination, 0);
            currentCost.Add(destination, 0);
            nodePath.Add(destination, null);

            while (currentNodes.Count > 0 && nodePath.Count < 400)
            {
                var currentNode = currentNodes.Dequeue();

                if (ReferenceEquals(currentNode, start))
                {
                    return PathFromPrevNodes(nodePath, start);
                }

                var nextNodes = mGraph.GetNodeNeighbors(currentNode, grid);

                foreach (var nextNode in nextNodes)
                {
                    var newCost = currentCost[currentNode] + currentNode.GetNeighborDistance(nextNode);
                    if (!currentCost.ContainsKey(nextNode))
                    {
                        currentCost.Add(nextNode, newCost);
                    }
                    else if (newCost < currentCost[nextNode])
                    {
                        currentCost[nextNode] = newCost;
                    }
                    else
                    {
                        continue;
                    }

                    var priority = newCost + SearchHeuristic(start, nextNode);
                    currentNodes.Enqueue(nextNode, priority);
                    if (!nodePath.ContainsKey(nextNode))
                    {
                        nodePath.Add(nextNode, currentNode);
                    }
                    else
                    {
                        nodePath[nextNode] = currentNode;
                    }
                }
            }

            return new List<Point>();
        }

        private float SearchHeuristic(GridNode start, GridNode pathNode)
        {
            var destX = start.mGridPosition.X;
            var destY = start.mGridPosition.Y;
            var pathX = pathNode.mGridPosition.X;
            var pathY = pathNode.mGridPosition.Y;
            var dist = Math.Pow(destX - pathX, 2) + Math.Pow(destY - pathY, 2);
            return (float) dist;
        }

        private List<Point> PathFromPrevNodes(Dictionary<GridNode, GridNode> prevNodes, GridNode start)
        {
            var path = new List<Point>();
            var currentNode = prevNodes[start];
            while (currentNode != null)
            {
                path.Add(currentNode.mGridPosition);
                currentNode = prevNodes[currentNode];
            }

            return path;
        }

        public void SetFindKeys(bool findKeys)
        {
            mGraph.mFindKeys = findKeys;
        }
    }

    public sealed class GridGraph
    {
        private readonly int mHeight;
        private readonly int mWidth;
        private readonly Dictionary<int, GridNode> mNodes;
        private readonly List<int> mNodeKeys;

        public bool mFindKeys;     // if true keys are not blocking the pathfinder

        public GridGraph(int width, int height)
        {
            mNodes = new Dictionary<int, GridNode>();
            mNodeKeys = new List<int>();
            mWidth = width;
            mHeight = height;
        }

        public GridNode NodeFromGridPosition(int x, int y)
        {
            var key = x + y * mWidth;
            if (mNodes.ContainsKey(key))
            {
                return mNodes[key];
            }

            CreateNode(key, new Point(x, y));
            return mNodes[key];
        }

        private void CreateNode(int key, Point position)
        {
            mNodes[key] = new GridNode(position);
            mNodeKeys.Add(key);
        }

        public bool NodeIsBlockedAt(int x, int y, Grid grid)
        {
            var collision = IsCollision(x, y, grid);
            var pathBlocked = NodeFromGridPosition(x, y).IsPathBlocked();
            var notStart = !NodeFromGridPosition(x, y).IsStart();
            return (collision || pathBlocked) && notStart;
        }

        private bool IsCollision(int x, int y, Grid grid)
        {
            var colObject = grid.GetAbstractGameObjectAt(new Point(x, y));
            if (colObject is EmptyObject emptyObject)
            {
                return emptyObject.IsBlocked();
            }
            if (mFindKeys)
            {
                return !(colObject is Key);
            }
            return true;
        }

        public List<GridNode> GetNodeNeighbors(GridNode node, Grid grid)
        {
            var neighbors = new List<GridNode>();
            var posX = node.mGridPosition.X;
            var posY = node.mGridPosition.Y;

            if (posX < mWidth - 1 && posY < mHeight - 1 && !NodeIsBlockedAt(posX + 1, posY + 1, grid))
            {
                neighbors.Add(NodeFromGridPosition(posX + 1, posY + 1));
            }

            if (posX < mWidth - 1 && !NodeIsBlockedAt(posX + 1, posY, grid))
            {
                neighbors.Add(NodeFromGridPosition(posX + 1, posY));
            }

            if (posX < mWidth - 1 && posY > 0 && !NodeIsBlockedAt(posX + 1, posY - 1, grid))
            {
                neighbors.Add(NodeFromGridPosition(posX + 1, posY - 1));
            }

            if (posY < mHeight - 1 && !NodeIsBlockedAt(posX, posY + 1, grid))
            {
                neighbors.Add(NodeFromGridPosition(posX, posY + 1));
            }

            if (posX > 0 && posY < mHeight - 1 && !NodeIsBlockedAt(posX - 1, posY + 1, grid))
            {
                neighbors.Add(NodeFromGridPosition(posX - 1, posY + 1));
            }

            if (posX > 0 && posY > 0 && !NodeIsBlockedAt(posX - 1, posY - 1, grid))
            {
                neighbors.Add(NodeFromGridPosition(posX - 1, posY - 1));
            }

            if (posX > 0 && !NodeIsBlockedAt(posX - 1, posY, grid))
            {
                neighbors.Add(NodeFromGridPosition(posX - 1, posY));
            }

            if (posY > 0 && !NodeIsBlockedAt(posX, posY - 1, grid))
            {
                neighbors.Add(NodeFromGridPosition(posX, posY - 1));
            }

            return neighbors;
        }

        public void ClearNodes()
        {
            for (int i = mNodeKeys.Count - 1; i >= 0; i--)
            {
                if (!mNodes[mNodeKeys[i]].IsPathBlocked())
                {
                    mNodes.Remove(mNodeKeys[i]);
                    mNodeKeys.RemoveAt(i);
                }
            }
        }
    }

    public sealed class GridNode
    {
        public readonly Point mGridPosition;
        private bool mIsStart;
        private bool mPathBlock;

        public GridNode(Point position)
        {
            mGridPosition = position;
        }

        public bool IsPathBlocked()
        {
            return mPathBlock;
        }

        public bool IsStart()
        {
            return mIsStart;
        }

        public void BlockPath()
        {
            mPathBlock = true;
        }

        public void UnblockPath()
        {
            mPathBlock = false;
        }

        public void SetStart()
        {
            mIsStart = true;
        }

        public void UnSetStart()
        {
            mIsStart = false;
        }

        public float GetNeighborDistance(GridNode neighbor)
        {
            return neighbor.mGridPosition.X == mGridPosition.X || neighbor.mGridPosition.Y == mGridPosition.Y
                ? 1.0f
                : (float) Math.Sqrt(2);
        }
    }
}