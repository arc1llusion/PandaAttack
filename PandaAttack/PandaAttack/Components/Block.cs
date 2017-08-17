using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PandaAttack.Components
{
    public enum BlockState
    {
        Idle,
        Falling,
        NeedsPop,
        NeedsPopFall,
        Popping,
        Popped
    }

    public class Block : IEquatable<Block>
    {
        private int _type;
        private BlockState _state;
        private BlockState _previousState;

        public static readonly Block Empty = new Block(-1);
        public static readonly Block Invalid = new Block(-2);

        public const int BLOCK_SIZE = 16; //These should always be square... Only scale may make them different

        public Block(int type)
        {
            this._type = type;
            _state = BlockState.NeedsPop;
            _previousState = BlockState.Idle;
        }

        public int Type
        {
            get { return this._type; }
        }

        public BlockState PreviousBlockState
        {
            get
            {
                return this._previousState;
            }
        }

        public BlockState BlockState
        {
            get { return this._state; }
            set
            {
                this._previousState = this._state;
                this._state = value;
            }
        }

        public static bool operator ==(Block b1, Block b2)
        {
            return b1.Equals(b2);
        }

        public static bool operator !=(Block b1, Block b2)
        {
            return !b1.Equals(b2);
        }

        public static bool IsInvalidOrEmpty(Block block)
        {
            return (block.Type == -1 || block.Type == -2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Block)
            {
                return this.Equals(obj as Block);
            }
            return false;
        }

        public void Pop()
        {
            this._type = -1;
            this.BlockState = BlockState.Popped;
        }

        public bool Popped
        {
            get
            {
                return _state == BlockState.Popped;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(Block other)
        {
            return this.Type == other.Type;
        }
    }
}
