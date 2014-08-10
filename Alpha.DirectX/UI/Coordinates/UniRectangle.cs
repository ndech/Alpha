using System;

namespace Alpha.DirectX.UI.Coordinates
{
    struct UniRectangle : IEquatable<UniRectangle>, IEquatable<Object>
    {
        public static readonly UniRectangle Zero = new UniRectangle();
        public UniVector Position { get; set; }
        public UniVector Size { get; set; }

        public UniRectangle(UniVector position, UniVector size) : this()
        {
            Position = position;
            Size = size;
        }

        public UniRectangle(UniScalar x, UniScalar y, UniScalar width, UniScalar height) : 
            this(new UniVector(x, y), new UniVector(width, height)) { }

        public UniScalar Left { get { return Position.X; }}

        public UniScalar Top { get { return Position.Y; }}

        public UniScalar Right { get { return Position.X + Size.X; }}

        public UniScalar Bottom { get { return Position.Y + Size.Y; }}

        public override int GetHashCode() 
        {
            return Position.GetHashCode() ^ Size.GetHashCode();
        }
        public static bool operator !=(UniRectangle first, UniRectangle second) 
        {
            return !(first.Equals(second));
        }

        public static bool operator ==(UniRectangle first, UniRectangle second)
        {
            return first.Equals(second);
        }

        public override bool Equals(object other) 
        {
            if (ReferenceEquals(null, other)) return false;
            return other is UniRectangle && Equals((UniRectangle) other);
        }

        public bool Equals(UniRectangle other) 
        {
            return (Position == other.Position) && (Size == other.Size);
        }

        public override string ToString()
        {
            return string.Format("{{Position:{0}, Size:{1}}}", Position, Size);
        }
    }
}