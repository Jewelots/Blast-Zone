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
    class GridNodeMover
    {
        Vector2 position;
        GridNodeMap map;

        bool movingHorizontal = false;

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

       
            public static MoveEvent MakeMoveEvent(MoveEventType moveEventType, float speed = 150)
            {
                MoveEvent me = new MoveEvent();
                me.moveEvent = moveEventType;
                me.speed = speed;
                return me;
            }
        }

        Queue<MoveEvent> moveEventQueue;

        public GridNodeMover(GridNodeMap gridNodeMap, int gx = 0, int gy = 0)
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

        public void GetInput(KeyboardState k)
        {
            if (k.IsKeyDown(Keys.W) || k.IsKeyDown(Keys.Up))
            {
                moveEventQueue.Enqueue(MoveEvent.MakeMoveEvent(MoveEvent.MoveEventType.MOVE_UP));
            }

            if (k.IsKeyDown(Keys.S) || k.IsKeyDown(Keys.Down))
            {
                moveEventQueue.Enqueue(MoveEvent.MakeMoveEvent(MoveEvent.MoveEventType.MOVE_DOWN));
            }

            if (k.IsKeyDown(Keys.A) || k.IsKeyDown(Keys.Left))
            {
                moveEventQueue.Enqueue(MoveEvent.MakeMoveEvent(MoveEvent.MoveEventType.MOVE_LEFT));
            }

            if (k.IsKeyDown(Keys.D) || k.IsKeyDown(Keys.Right))
            {
                moveEventQueue.Enqueue(MoveEvent.MakeMoveEvent(MoveEvent.MoveEventType.MOVE_RIGHT));
            }
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
            int currentX = (int)Math.Floor(position.X / map.nodeSize);
            int currentY = (int)Math.Floor(position.Y / map.nodeSize);

            return map.GetNode(currentX, currentY);
        }

        private GridNodeMap.TileContents GetNodeOffset(int x, int y)
        {
            int currentX = (int)Math.Floor(position.X / map.nodeSize);
            int currentY = (int)Math.Floor(position.Y / map.nodeSize);

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

            float snap = Math.Min(Math.Abs(speed) * 5, map.nodeSize / 2);

            GridNodeMap.TileContents rightNode = GetNodeOffset(1, 0);
            GridNodeMap.TileContents leftNode = GetNodeOffset(-1, 0);

            //Check if was moving vertical, and snap to grid square before moving if close enough
            if (movingHorizontal == false)
            {
                if (Math.Abs(distToNearest) < snap)
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
                position.X += speed;
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

            float snap = Math.Min(Math.Abs(speed) * 5, map.nodeSize / 2);

            GridNodeMap.TileContents belowNode = GetNodeOffset(0, 1);
            GridNodeMap.TileContents aboveNode = GetNodeOffset(0, -1);

            //Check if was moving horizontal, and snap to grid square before moving if close enough
            if (movingHorizontal == true)
            {
                if (Math.Abs(distToNearest) < snap)
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
                position.Y += speed;
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

        private void HandleMoveEventQueue(GameTime gameTime)
        {
            //Check if event queue contains any events to move horizontally or vertically
            bool hasMoveHorizontalEvent = moveEventQueue.Any(x => x.moveEvent == MoveEvent.MoveEventType.MOVE_LEFT || x.moveEvent == MoveEvent.MoveEventType.MOVE_RIGHT);
            bool hasMoveVerticalEvent = moveEventQueue.Any(x => x.moveEvent == MoveEvent.MoveEventType.MOVE_UP || x.moveEvent == MoveEvent.MoveEventType.MOVE_DOWN);

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

        //temp below this point

        Texture2D playerTex; //replace with animation

        public void LoadContent(ContentManager Content)
        {
            playerTex = Content.Load<Texture2D>("Images/Game/bomb");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerTex, position, null, Color.White, 0f, new Vector2(GlobalGameData.tileSize / 2 - 0.5f, GlobalGameData.tileSize / 2 - 0.5f), GlobalGameData.drawRatio, SpriteEffects.None, 1f);
        }
    }
}
