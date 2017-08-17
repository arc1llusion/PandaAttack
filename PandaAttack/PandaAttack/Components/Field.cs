using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PandaAttack.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PandaAttack.Components
{
    public class Field
    {
        public const int FIELD_HEIGHT = 12;
        public const int FIELD_WIDTH = 6;

        public const int STRESS_HEIGHT = 10;

        private List<Block[]> _pit;

        private Texture2D _blockTexture;

        private Point _cursor = new Point(0, 0);

        private Matcher _matcher;

        private bool[] _needsFall;

        private bool _combo;

        public Field(Texture2D blockTexture)
        {
            _pit = new List<Block[]>();
            _blockTexture = blockTexture;
            _matcher = new Matcher(this);
            _needsFall = new bool[6];
        }

        public Field(Texture2D blockTexture, int[][] field)
            : this(blockTexture)
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                _pit.Insert(0, BlockUtility.GenerateDeterminedBlockArray(field[i]));
            }
        }

        public Field(Texture2D blockTexture, char[][] mappedField)
            : this(blockTexture)
        {
            for (int i = 0; i < mappedField.GetLength(0); i++)
            {
                _pit.Insert(0, BlockUtility.GenerateMappedBlockArray(mappedField[i]));
            }
        }

        public List<Block[]> Pit
        {
            get
            {
                return _pit;
            }
        }

        #region Public Methods

        public bool IsMatchableWith(Block b1, Block b2)
        {
            return b1.Type == b2.Type;
        }

        public BlockData RightOf(Point p)
        {
            p.X = p.X + 1;
            if (!IsInBounds(p))
                return BlockData.Invalid;

            return new BlockData(p.X, p.Y, _pit[p.Y][p.X]);
        }

        public BlockData LeftOf(Point p)
        {
            p.X = p.X - 1;
            if (!IsInBounds(p))
                return BlockData.Invalid;

            return new BlockData(p.X, p.Y, _pit[p.Y][p.X]);
        }

        public BlockData Below(Point p)
        {
            p.Y = p.Y - 1;
            if (!IsInBounds(p))
                return BlockData.Invalid;

            return new BlockData(p.X, p.Y, _pit[p.Y][p.X]);
        }

        public BlockData Above(Point p)
        {
            p.Y = p.Y + 1;
            if (!IsInBounds(p))
                return BlockData.Invalid;

            return new BlockData(p.X, p.Y, _pit[p.Y][p.X]);
        }

        public BlockData At(Point p)
        {
            if (!IsInBounds(p))
                return BlockData.Invalid;

            return new BlockData(p.X, p.Y, _pit[p.Y][p.X]);
        }

        #endregion

        #region Private Methods

        private bool IsInBounds(Point p)
        {
            if (p.Y >= 0 && p.Y < _pit.Count && p.X >= 0 && p.X <= FIELD_WIDTH - 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Swaps and drops blocks to a lower level if needed.
        /// </summary>
        /// <param name="p1">The position of the first block to switch</param>
        /// <param name="p2">The position of the second block to switch</param>
        private void Swap(Point p1, Point p2)
        {
            if (IsInBounds(p1) && IsInBounds(p2))
            {
                Block temp = _pit[p1.Y][p1.X];
                _pit[p1.Y][p1.X] = _pit[p2.Y][p2.X];
                _pit[p2.Y][p2.X] = temp;
            }
        }

        /// <summary>
        /// Used when swapping with the cursor, for easy entry... Bazinga
        /// </summary>
        /// <param name="row">The row the cursor is on</param>
        /// <param name="start">The position of the cursors left reticle</param>
        private void SwapAdjacent(int row, int start)
        {
            //Assuming start = left cursor
            Point startPoint = new Point(start, row);
            Point adjacentPoint = new Point(start + 1, row);
            Swap(startPoint, adjacentPoint);
        }

        /// <summary>
        /// Drops the block at the position as well as all blocks above it.
        /// </summary>
        /// <param name="position"></param>

        bool gchain = false;
        private void DropBlock(Point position)
        {
            Block block = _pit[position.Y][position.X];
            bool areTherePoppedBlocks = false;
            while (block.Type != -2)
            {
                areTherePoppedBlocks |= block.Popped;
                Point newPosition = position;

                BlockData data = Below(newPosition);
                while (data.Block.Type == -1)
                {
                    newPosition.Y = newPosition.Y - 1;
                    data = Below(newPosition);
                }

                if (position != newPosition)
                {
                    block.BlockState = BlockState.NeedsPop;
                    Swap(position, newPosition);
                    gchain = areTherePoppedBlocks;
                }

                data = Above(position);
                position = data.Position;
                block = data.Block;
            }
        }

        private void UpdateMatches()
        {
            for (int i = 0; i < _pit.Count; i++)
            {
                Block[] row = _pit[i];
                for (int j = 0; j < FIELD_WIDTH; j++)
                {
                    Block block = row[j];
                    if (block != Block.Empty && block.BlockState == BlockState.NeedsPop)
                    {
                        _matcher.CheckForMatches(new Point(j, i)).ForEach(x => x.Block.BlockState = BlockState.Popping);
                    }
                }
            }

            for (int i = 0; i < _pit.Count; i++)
            {
                Block[] row = _pit[i];
                for (int j = 0; j < FIELD_WIDTH; j++)
                {
                    Block block = row[j];

                    //if current block is still in a needs pop state, reset it back to idle
                    if (block.BlockState == BlockState.NeedsPop)
                    {
                        block.BlockState = BlockState.Idle;
                    }
                    else if (block.BlockState == BlockState.Popping)
                    {
                        //TODO: Will need to take this out of here eventually when accounting for animation
                        row[j].Pop();
                    }
                }
            }
        }

        private void ResetPops()
        {
            for (int i = 0; i < _pit.Count; i++)
            {
                for (int j = 0; j < FIELD_WIDTH; j++)
                {
                    Block block = _pit[i][j];
                    if (block.Popped) _pit[i][j] = Block.Empty;
                }
            }
        }

        private void HandleInput(GameTime time)
        {
            if (InputHandler.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                if (_cursor.X > 0) _cursor.X--;
            }

            if (InputHandler.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                if (_cursor.X + 1 < FIELD_WIDTH - 1) _cursor.X++;
            }

            if (InputHandler.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                if (_cursor.Y < FIELD_HEIGHT) _cursor.Y++;
            }

            if (InputHandler.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                if (_cursor.Y > 0) _cursor.Y--;
            }

            if (InputHandler.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                this.SwapAdjacent(_cursor.Y, _cursor.X);
                _pit[_cursor.Y][_cursor.X].BlockState = _pit[_cursor.Y][_cursor.X + 1].BlockState = BlockState.NeedsPop;
            }

            if (InputHandler.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Z))
            {
                this._pit.Insert(0, BlockUtility.GenerateRandomRow());
            }
        }

        private void DropBlocks()
        {
            for (int i = 0; i < FIELD_WIDTH; i++)
            {
                DropBlock(new Point(i, 0));
            }
        }

        #endregion

        #region XNA methods

        public void Update(GameTime time)
        {
            HandleInput(time);
            UpdateMatches();
            DropBlocks();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color notint = Color.White;
            Color tint = Color.Red;
            Color np = Color.Green;

            Color drawColor = notint;
            spriteBatch.Begin();

            const int size = Block.BLOCK_SIZE; //Made constant so that the compiler inlines
            const int scaled_size = size * 1;
            int x = 0;
            int y = FIELD_HEIGHT * scaled_size;

            for (int i = 0; i < _pit.Count; i++)
            {
                x = 0;
                Block[] row = _pit[i];
                for (int j = 0; j < _pit[i].Length; j++)
                {
                    if (row[j].BlockState == BlockState.Popping)
                        drawColor = tint;
                    else if (row[j].BlockState == BlockState.NeedsPop)
                        drawColor = np;
                    else
                        drawColor = notint;

                    if (row[j].Type != -1)
                        spriteBatch.Draw(_blockTexture, new Vector2(x, y), new Rectangle(0, row[j].Type * size, size, size), drawColor, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);

                    x += scaled_size;
                }
                y -= scaled_size;
            }

            //TODO: Don't make sucky drawing code for blocks
            x = _cursor.X * scaled_size;
            y = FIELD_HEIGHT * scaled_size - _cursor.Y * scaled_size;

            spriteBatch.Draw(_blockTexture, new Rectangle(x, y, scaled_size, scaled_size), new Rectangle(5 * size, 10 * size, size, size), Color.White);
            spriteBatch.Draw(_blockTexture, new Rectangle(x + scaled_size, y, scaled_size, scaled_size), new Rectangle(5 * size, 10 * size, size, size), Color.White);
            spriteBatch.DrawString(Game1.FONT, "Chain: " + gchain, new Vector2(400, 20), Color.White);

            spriteBatch.End();
        }

        #endregion
    }
}
