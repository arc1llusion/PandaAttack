using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PandaAttack.Components;
using Microsoft.Xna.Framework;

namespace PandaAttack.Utility
{
    public static class BlockUtility
    {
        public static Block GenerateBlock()
        {
            return new Block(RandomGenerator.Random.Next(0, 6));
        }

        public static Block[] GenerateRandomRow()
        {
            Block[] blocks = new Block[Field.FIELD_WIDTH];
            for (int i = 0; i < blocks.Length; i++)
                blocks[i] = GenerateBlock();

            return blocks;
        }

        public static Block[] GenerateDeterminedBlockArray(int[] input)
        {
            Block[] blocks = new Block[input.Length];
            for(int i = 0; i < input.Length; i++)
                blocks[i] = new Block((int)MathHelper.Clamp(input[i], -1, 5));

            return blocks;
        }

        public static Block[] GenerateMappedBlockArray(char[] input)
        {
            Block[] blocks = new Block[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '#') blocks[i] = GenerateBlock();
                else blocks[i] = Block.Empty;
            }
            

            return blocks;
        }

        public static Block[] GenerateEmptyBlockArray()
        {
            return new Block[] { Block.Empty, Block.Empty, Block.Empty, Block.Empty, Block.Empty, Block.Empty };
        }
    }
}
