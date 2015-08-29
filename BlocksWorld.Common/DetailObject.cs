using OpenTK;

namespace BlocksWorld
{
    public class DetailObject
    {
        public DetailObject(int id)
        {
            this.ID = id;
        }

        public int ID { get; private set; }
        public string Model { get; set; }
        public Vector3 Position { get; set; }
        public float Rotation { get; set; }
    }
}