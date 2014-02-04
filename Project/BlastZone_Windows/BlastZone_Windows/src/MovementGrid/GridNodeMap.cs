using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BlastZone_Windows.MovementGrid
{
    /// <summary>
    /// A map of gridnode's for the player to move on
    /// </summary>
    class GridNodeMap
    {
        int gridSizeX;
        int gridSizeY;
        public int nodeSize;
        
        /// <summary>
        /// Holds contents of a tile (position, and solid?)
        /// </summary>
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

        /// <summary>
        /// Array of nodes
        /// </summary>
        TileContents[,] nodeArray;

        /// <summary>
        /// Create a new GridNode map
        /// </summary>
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

        /// <summary>
        /// Get the node at (x, y)
        /// </summary>
        /// <param name="x">Tile position x</param>
        /// <param name="y">Tile position y</param>
        /// <returns>Contents of the tile</returns>
        public TileContents GetNode(int x, int y)
        {
            //Check if in bounds
            if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
            {
                return null;
            }

            return nodeArray[x, y];
        }

        /// <summary>
        /// Set the solidity of all tiles based on an array
        /// </summary>
        /// <param name="solidArray">An array of bool representing solid (true) or unsolid (false)</param>
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

        /// <summary>
        /// Returns if a node is solid at (x, y)
        /// </summary>
        /// <param name="x">Tile position x</param>
        /// <param name="y">Tile position y</param>
        /// <returns>If the node is solid orn ot</returns>
        public bool IsNodeSolid(int x, int y)
        {
            return GetNode(x, y).solid;
        }
    }
}
