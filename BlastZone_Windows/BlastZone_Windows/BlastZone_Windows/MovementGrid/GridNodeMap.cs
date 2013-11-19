using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BlastZone_Windows.MovementGrid
{
    class GridNodeMap
    {
        int gridSizeX;
        int gridSizeY;
        public int nodeSize;
        
        public class TileContents 
        {
            public Vector2 position;
            public bool solid;
            
            public TileContents(Vector2 pos, bool solid)
            {
                this.position = pos;
                this.solid = solid;
            }

            public TileContents(Vector2 pos)
            {
                this.position = pos;
                this.solid = false;
            }
        }

        TileContents[,] nodeArray;

        public GridNodeMap()
        {
            this.gridSizeX = GlobalGameData.gridSizeX;
            this.gridSizeY = GlobalGameData.gridSizeY;

            nodeSize = GlobalGameData.tileSize * GlobalGameData.drawRatio;
            nodeArray = new TileContents[gridSizeX, gridSizeY];

            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    Vector2 tilePos = new Vector2();
                    tilePos.X = nodeSize / 2 + nodeSize * x;
                    tilePos.Y = nodeSize / 2 + nodeSize * y;

                    nodeArray[x, y] = new TileContents(tilePos);
                }
            }
        }

        public TileContents GetNode(int x, int y)
        {
            if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY) //Requested position outside grid
            {
                return null;
            }

            return nodeArray[x, y];
        }

        public void SetSolid(bool[,] solidArray)
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    nodeArray[x, y].solid = solidArray[x, y];
                }
            }
        }

        public bool IsNodeSolid(int x, int y)
        {
            return GetNode(x, y).solid;
        }
    }
}
