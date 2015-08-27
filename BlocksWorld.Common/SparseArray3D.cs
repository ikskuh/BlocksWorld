using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BlocksWorld
{
    public sealed class Value3D<T>
    {
        public Value3D(int x, int y, int z, T value)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Value = value;
        }

        public T Value { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }
    }

    public sealed class SparseArray3D<T> : IEnumerable<Value3D<T>>
    {
        private int minX = 0, maxX = 0, minY = 0, maxY = 0, minZ = 0, maxZ = 0;
        private readonly Dictionary<int, Dictionary<int, Dictionary<int, T>>> items = new Dictionary<int, Dictionary<int, Dictionary<int, T>>>();

        void AssurePosition(int x, int y, int z)
        {
            if (items.ContainsKey(x) == false)
                items.Add(x, new Dictionary<int, Dictionary<int, T>>());
            var sliceYZ = items[x];
            if (sliceYZ.ContainsKey(y) == false)
                sliceYZ.Add(y, new Dictionary<int, T>());
            var sliceZ = sliceYZ[y];
            if (sliceZ.ContainsKey(z) == false)
                sliceZ.Add(z, default(T));
        }

        public T this[int x, int y, int z]
        {
            get
            {
                if (this.items.ContainsKey(x) == false)
                    return default(T);
                if (this.items[x].ContainsKey(y) == false)
                    return default(T);
                if (this.items[x][y].ContainsKey(z) == false)
                    return default(T);
                return this.items[x][y][z];
            }
            set
            {
                this.minX = Math.Min(this.minX, x);
                this.minY = Math.Min(this.minY, y);
                this.minZ = Math.Min(this.minZ, z);

                this.maxX = Math.Max(this.maxX, x);
                this.maxY = Math.Max(this.maxY, y);
                this.maxZ = Math.Max(this.maxZ, z);
                this.AssurePosition(x, y, z);
                this.items[x][y][z] = value;
            }
        }

        internal int GetLowerX() { return this.minX; }
        internal int GetUpperX() { return this.maxX; }

        internal int GetLowerY() { return this.minY; }
        internal int GetUpperY() { return this.maxY; }

        internal int GetLowerZ() { return this.minZ; }
        internal int GetUpperZ() { return this.maxZ; }

        public IEnumerator<Value3D<T>> GetEnumerator()
        {
            return this.items.SelectMany(x => x.Value.SelectMany(y => y.Value.Select(z => new Value3D<T>(x.Key, y.Key, z.Key, z.Value)))).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}