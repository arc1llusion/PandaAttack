using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PandaAttack.Components
{
    public struct BlockData
    {
        public readonly int X;
        public readonly int Y;
        public readonly Point Position;
        public readonly Block Block;

        public static readonly BlockData Empty = new BlockData(-1, -1, Block.Empty);
        public static readonly BlockData Invalid = new BlockData(-1, -1, Block.Invalid);

        public BlockData(int x, int y, Block block)
        {
            this.X = x;
            this.Y = y;
            this.Position = new Point(x, y);
            this.Block = block;
        }
    }

    public class Matcher
    {
        private Field _field;
        private bool[,] visited;

        public Matcher(Field field)
        {
            _field = field;
        }

        /// <summary>
        /// Checks a particular point in the field for matches. This is typically activated when a block as the "NeedsPop" status
        /// </summary>
        /// <param name="cursorPosition"></param>
        /// <returns></returns>
        public List<BlockData> CheckForMatches(Point cursorPosition)
        {
            List<BlockData> matches = new List<BlockData>();

            visited = new bool[_field.Pit.Count, Field.FIELD_WIDTH];

            BlockData block = _field.At(cursorPosition);
            visited[block.Y, block.X] = true;

            Queue<BlockData> verticals = _CheckVerticals(block);
            Queue<BlockData> horizontals = _CheckHorizontals(block);

            if (verticals.Count >= 2 || horizontals.Count >= 2)
                matches.Add(block);

            if (verticals.Count >= 2)
                matches.AddRange(verticals);
            if (horizontals.Count >= 2)
                matches.AddRange(horizontals);

            while (verticals.Count > 0 || horizontals.Count > 0)
            {
                while (verticals.Count > 0)
                {
                    BlockData data = verticals.Dequeue();
                    Queue<BlockData> tempHorizontals = _CheckHorizontals(data);

                    if (tempHorizontals.Count >= 2)
                    {
                        if (!matches.Contains(data))
                            matches.Add(data);

                        matches.AddRange(tempHorizontals);

                        while (tempHorizontals.Count > 0) {
                            BlockData temp = tempHorizontals.Dequeue();
                            if(!visited[temp.Y, temp.X])
                                horizontals.Enqueue(temp);
                        }
                    }
                }

                while (horizontals.Count > 0)
                {
                    BlockData data = horizontals.Dequeue();
                    Queue<BlockData> tempVerticals = _CheckVerticals(data);

                    if (tempVerticals.Count >= 2)
                    {
                        if (!matches.Contains(data))
                            matches.Add(data);

                        matches.AddRange(tempVerticals);

                        while (tempVerticals.Count > 0) {
                            BlockData temp = tempVerticals.Dequeue();
                            if (!visited[temp.Y, temp.X])
                                horizontals.Enqueue(tempVerticals.Dequeue());
                        }
                    }
                }
            }

            return matches;
        }

        private Queue<BlockData> _CheckVerticals(BlockData source)
        {
            BlockData data = _field.Above(source.Position);
            Queue<BlockData> verticals = new Queue<BlockData>();
            while (!Block.IsInvalidOrEmpty(data.Block) && _field.IsMatchableWith(source.Block, data.Block) && !visited[data.Y, data.X])
            {
                verticals.Enqueue(data);
                visited[data.Y, data.X] = true;
                data = _field.Above(data.Position);
            }

            data = _field.Below(source.Position);
            while (!Block.IsInvalidOrEmpty(data.Block) && _field.IsMatchableWith(source.Block, data.Block) && !visited[data.Y, data.X])
            {
                verticals.Enqueue(data);
                visited[data.Y, data.X] = true;
                data = _field.Below(data.Position);
            }

            return verticals;
        }

        private Queue<BlockData> _CheckHorizontals(BlockData source)
        {
            BlockData data = _field.RightOf(source.Position);
            Queue<BlockData> horizontals = new Queue<BlockData>();

            while (!Block.IsInvalidOrEmpty(data.Block) && _field.IsMatchableWith(source.Block, data.Block) && !visited[data.Y, data.X])
            {
                horizontals.Enqueue(data);
                visited[data.Y, data.X] = true;
                data = _field.RightOf(data.Position);
            }

            data = _field.LeftOf(source.Position);
            while (!Block.IsInvalidOrEmpty(data.Block) && _field.IsMatchableWith(source.Block, data.Block) && !visited[data.Y, data.X])
            {
                horizontals.Enqueue(data);
                visited[data.Y, data.X] = true;
                data = _field.LeftOf(data.Position);
            }

            return horizontals;
        }
    }
}
