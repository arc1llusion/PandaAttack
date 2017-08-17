using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PandaAttack.Components
{
    public class Chain
    {
        private List<List<BlockData>> _matches;

        public Chain()
        {
            _matches = new List<List<BlockData>>();
        }

        public Chain(List<BlockData> initialMatch) : this()
        {
            _matches.Add(initialMatch);
        }

        public void AddChain(List<BlockData> match)
        {
            _matches.Add(match);
        }

        public void Empty()
        {
            _matches.Clear();
        }

        public int ChainCount
        {
            get
            {
                return this._matches.Count;
            }
        }
    }
}
