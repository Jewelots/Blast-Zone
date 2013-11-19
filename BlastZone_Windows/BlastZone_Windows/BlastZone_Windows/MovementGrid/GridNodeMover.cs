using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BlastZone_Windows.MovementGrid
{
    class MoveEvent
    {
        public enum MoveEventType
        {
            MOVE_NONE,
            MOVE_UP,
            MOVE_LEFT,
            MOVE_RIGHT,
            MOVE_DOWN
        }

        public MoveEventType moveEvent;
        public float speed;


        public static MoveEvent MakeEvent(MoveEventType moveEventType, float speed)
        {
            MoveEvent me = new MoveEvent();
            me.moveEvent = moveEventType;
            me.speed = speed;
            return me;
        }

        public static MoveEvent MakeEvent(MoveEventType moveEventType)
        {
            MoveEvent me = new MoveEvent();
            me.moveEvent = moveEventType;
            me.speed = 150;
            return me;
        }
    }

    class GridNodeMover
    {
        Vector2 position;
        GridNodeMap map;

        bool movingHorizontal = false;

        float speedModifier;

        Queue<MoveEvent> moveEventQueue;

        public GridNodeMover(GridNodeMap gridNodeMap, int gx, int gy)
        {
            GridNodeMap.TileContents tile = gridNodeMap.GetNode(gx, gy);

            if (tile == null) //Tile requested doesn't exist
            {
                tile = gridNodeMap.GetNode(0, 0);
            }

            position = tile.position;
            map = gridNodeMap;

            moveEventQueue = new Queue<MoveEvent>();
        }

        public GridNodeMover(GridNodeMap gridNodeMap)
        {
            GridNodeMap.TileContents tile = gridNodeMap.GetNode(0, 0);

            if (tile == null) //Tile requested doesn't exist
            {
                tile = gridNodeMap.GetNode(0, 0);
            }

            position = tile.position;
            map = gridNodeMap;

            moveEventQueue = new Queue<MoveEvent>();
        }

        public void Reset()
        {
            speedModifier = 1;
        }

        public void QueueEvent(MoveEvent moveEvent)
        {
            moveEventQueue.Enqueue(moveEvent);
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public void Update(GameTime gameTime)
        {
            HandleMoveEventQueue(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                GetCurrentNode().solid = true;
            }

            //Console.Out.WriteLine(PercentToNearestGridSquare());
        }

        private void SnapToGridSquareX()
        {
            position.X = GetCurrentNode().position.X;
        }

        private void SnapToGridSquareY()
        {
            position.Y = GetCurrentNode().position.Y;
        }

        private void SnapToGridSquare()
        {
            SnapToGridSquareX();
            SnapToGridSquareY();
        }

        private GridNodeMap.TileContents GetCurrentNode()
        {
            int currentX, currentY;
            GetGridPosition(out currentX, out currentY);

            return map.GetNode(currentX, currentY);
        }

        private GridNodeMap.TileContents GetNodeOffset(int x, int y)
        {
            int currentX, currentY;
            GetGridPosition(out currentX, out currentY);

            return map.GetNode(currentX + x, currentY + y);
        }

        private float DistToNearestGridSquare()
        {
            GridNodeMap.TileContents gridNode = GetCurrentNode();

            float dist = 0;

            if (movingHorizontal)
            {
                dist = position.X - gridNode.position.X;
            }
            else
            {
                dist = position.Y - gridNode.position.Y;
            }

            return dist;
        }

        private float PercentToNearestGridSquare()
        {
            float dist = DistToNearestGridSquare();

            return dist / map.nodeSize;
        }

        private void MoveHorizontal(float speed)
        {
            float distToNearest = DistToNearestGridSquare();
            float percToNearest = PercentToNearestGridSquare();

            //float snap = Math.Min(Math.Abs(speed) * 5, map.nodeSize / 2);
            float snap = 0.2f;

            GridNodeMap.TileContents rightNode = GetNodeOffset(1, 0);
            GridNodeMap.TileContents leftNode = GetNodeOffset(-1, 0);

            //Check if was moving vertical, and snap to grid square before moving if close enough
            if (movingHorizontal == false)
            {
                if (Math.Abs(percToNearest) < snap)
                {
                    if (!((speed < 0 && leftNode != null && leftNode.solid) || (speed > 0 && rightNode != null && rightNode.solid)))
                    {
                        movingHorizontal = true;
                        SnapToGridSquare();
                    }
                }
            }

            //Move
            if (movingHorizontal == true)
            {
                position.X += speed * speedModifier;
            }

            distToNearest = DistToNearestGridSquare();

            if (distToNearest > 0)
            {
                if (rightNode == null || (rightNode.solid && movingHorizontal && speed > 0))
                {
                    //movingHorizontal = false;
                    SnapToGridSquareX();
                    return;
                }
            }

            if (distToNearest < 0)
            {
                if (leftNode == null || (leftNode.solid && movingHorizontal && speed < 0))
                {
                    //movingHorizontal = false;
                    SnapToGridSquareX();
                    return;
                }
            }
        }

        private void MoveVertical(float speed)
        {
            float distToNearest = DistToNearestGridSquare();
            float percToNearest = PercentToNearestGridSquare();

            //float snap = Math.Min(Math.Abs(speed) * 5, map.nodeSize / 2);
            float snap = 0.2f;

            GridNodeMap.TileContents belowNode = GetNodeOffset(0, 1);
            GridNodeMap.TileContents aboveNode = GetNodeOffset(0, -1);

            //Check if was moving horizontal, and snap to grid square before moving if close enough
            if (movingHorizontal == true)
            {
                if (Math.Abs(percToNearest) < snap)
                {
                    if (!((speed < 0 && aboveNode != null && aboveNode.solid) || (speed > 0 && belowNode != null && belowNode.solid)))
                    {
                        movingHorizontal = false;
                        SnapToGridSquare();
                    }
                }
            }

            //Move
            if (movingHorizontal == false)
            {
                position.Y += speed * speedModifier;
            }

            distToNearest = DistToNearestGridSquare();

            if (distToNearest > 0)
            {
                if (belowNode == null || (belowNode.solid && !movingHorizontal && speed > 0))
                {
                    SnapToGridSquareY();
                    return;
                }
            }

            if (distToNearest < 0)
            {
                if (aboveNode == null || (aboveNode.solid && !movingHorizontal && speed < 0))
                {
                    SnapToGridSquareY();
                    return;
                }
            }
        }

        private bool HasMoveHorizontalEvent(Queue<MoveEvent> moveEventQueue)
        {
            for (int i = 0; i < moveEventQueue.Count; ++i)
            {
                MoveEvent me = moveEventQueue.ElementAt(i);
                if (me.moveEvent == MoveEvent.MoveEventType.MOVE_LEFT || me.moveEvent == MoveEvent.MoveEventType.MOVE_RIGHT)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasMoveVerticalEvent(Queue<MoveEvent> moveEventQueue)
        {
            for (int i = 0; i < moveEventQueue.Count; ++i)
            {
                MoveEvent me = moveEventQueue.ElementAt(i);
                if (me.moveEvent == MoveEvent.MoveEventType.MOVE_UP || me.moveEvent == MoveEvent.MoveEventType.MOVE_DOWN)
                {
                    return true;
                }
            }

            return false;
        }

        private void HandleMoveEventQueue(GameTime gameTime)
        {
            //Check if event queue contains any events to move horizontally or vertically
            bool hasMoveHorizontalEvent = HasMoveHorizontalEvent(moveEventQueue);
            bool hasMoveVerticalEvent = HasMoveVerticalEvent(moveEventQueue);

            while (moveEventQueue.Count > 0)
            {
                MoveEvent inputMoveEvent = moveEventQueue.Dequeue();
                
                //If you're moving vertically, OR there's no horizontal events, check for vertical events
                if (!movingHorizontal || !hasMoveHorizontalEvent)
                {
                    if (inputMoveEvent.moveEvent == MoveEvent.MoveEventType.MOVE_UP)
                    {
                        MoveVertical(-inputMoveEvent.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        continue;
                    }

                    else if (inputMoveEvent.moveEvent ==  MoveEvent.MoveEventType.MOVE_DOWN)
                    {
                        MoveVertical(inputMoveEvent.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        continue;
                    }
                }

                //If you're moving horizontally, OR there's no vertical events, check for horizontal events
                if (movingHorizontal || !hasMoveVerticalEvent)
                {
                    if (inputMoveEvent.moveEvent == MoveEvent.MoveEventType.MOVE_LEFT)
                    {
                        MoveHorizontal(-inputMoveEvent.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        continue;
                    }

                    else if (inputMoveEvent.moveEvent == MoveEvent.MoveEventType.MOVE_RIGHT)
                    {
                        MoveHorizontal(inputMoveEvent.speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        continue;
                    }
                }
            }
        }

        public void GetGridPosition(out int gx, out int gy)
        {
            gx = (int)Math.Floor(position.X / map.nodeSize);
            gy = (int)Math.Floor(position.Y / map.nodeSize);
        }

        public void GetAllOccupiedPositions(out int[] tx, out int[] ty)
        {
            List<int> tempTilesX = new List<int>();
            List<int> tempTilesY = new List<int>();

            int curX, curY;
            GetGridPosition(out curX, out curY);

            tempTilesX.Add(curX);
            tempTilesY.Add(curY);

            float distToNearest = DistToNearestGridSquare();

            if (movingHorizontal)
            {
                if (distToNearest < 0)
                {
                    tempTilesX.Add(curX - 1);
                    tempTilesY.Add(curY);
                }
                else if (distToNearest > 0)
                {
                    tempTilesX.Add(curX + 1);
                    tempTilesY.Add(curY);
                }
            }
            else
            {
                if (distToNearest < 0)
                {
                    tempTilesX.Add(curX);
                    tempTilesY.Add(curY - 1);
                }
                else if (distToNearest > 0)
                {
                    tempTilesX.Add(curX);
                    tempTilesY.Add(curY + 1);
                }
            }

            tx = tempTilesX.ToArray();
            ty = tempTilesY.ToArray();
        }

        public void SetPosition(int gx, int gy)
        {
            position.X = gx * map.nodeSize + map.nodeSize / 2;
            position.Y = gy * map.nodeSize + map.nodeSize / 2;
        }

        public void AddSpeed()
        {
            speedModifier += 0.2f;
        }
    }
}
