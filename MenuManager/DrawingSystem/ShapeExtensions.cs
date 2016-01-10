using System;



namespace MenuManager.DrawingSystem
{
    class ShapeExtensions
    {


    }


    public class ShapeObject
    {
        private Shape _collisionShape;

        public ShapeObject(Shape collisionShape)
        {
            _collisionShape = collisionShape;
        }

        public bool Intersects(ShapeObject other)
        {
            return _collisionShape.IntersectVisit(other._collisionShape);
        }
    }


    public abstract class Shape
    {
        public abstract bool IntersectVisit(Shape other);
        public abstract bool Intersect(Circle circle);
        public abstract bool Intersect(Rectangle circle);
    }

    public class Circle : Shape
    {
        public override bool IntersectVisit(Shape other)
        {
            return other.Intersect(this);
        }

        public override bool Intersect(Circle circle)
        {
            Console.WriteLine("Circle intersecting Circle");
            return false; //implement circle to circle collision detection
        }

        public override bool Intersect(Rectangle rect)
        {
            Console.WriteLine("Circle intersecting Rectangle");
            return false; //implement circle to rectangle collision detection
        }
    }

    public class Rectangle : Shape
    {
        public override bool IntersectVisit(Shape other)
        {
            return other.Intersect(this);
        }

        public override bool Intersect(Circle circle)
        {
            Console.WriteLine("Rectangle intersecting Circle");
            return true; //implement rectangle to circle collision detection
        }

        public override bool Intersect(Rectangle rect)
        {
            Console.WriteLine("Rectangle intersecting Rectangle");
            return true; //implement rectangle to rectangle collision detection
        }
    }


    /////////////////////////// EXAMPLE ///////////////////////////////


    //And example code calling it:

    //ShapeObject objectCircle = new GameObject(new Circle());
    //ShapeObject objectRect = new GameObject(new Rectangle());

    //objectCircle.Intersects(objectCircle);
    //objectCircle.Intersects(objectRect);
    //objectRect.Intersects(objectCircle);
    //objectRect.Intersects(objectRect);

    //Produces the output:

    //Circle intersecting Circle
    //Rectangle intersecting Circle
    //Circle intersecting Rectangle
    //Rectangle intersecting Rectangle


}
