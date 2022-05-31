using System;
using OpenTK;

namespace RayTracing
{
    public class Camera
    {
        //Векторы, задающие направления осей координат для камеры
        private Vector3 front = -Vector3.UnitZ;
        private Vector3 up = Vector3.UnitY;
        private Vector3 right = Vector3.UnitX;

        //Угол поворота камеры вокруг оси X и Y
        private float pitch = 0.0f;
        private float yaw = MathHelper.PiOver2;

        //Угол обзора камеры
        private float fov = MathHelper.PiOver2;

        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }

        // Позиция камеры (точка)
        public Vector3 Position { get; set; }

        //Соотношение сторон окна, используемое для расчёта projection-матрицы
        public float AspectRatio { get; set; }

        public Vector3 Front => front;
        public Vector3 Up => up;
        public Vector3 Right => right;

        // Поля для работы с уголами поворота камеры вокруг оси X и Y в градусах
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(pitch);
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f);
                pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(yaw);
            set
            {
                yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        public float Fov
        {
            get => MathHelper.RadiansToDegrees(fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                fov = MathHelper.DegreesToRadians(angle);
            }
        }

        //Получение матрицы отображения с помощью функции LookAt
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + front, Vector3.UnitY);
        }

        //Получение матрицы проекции
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(fov, AspectRatio, 0.01f, 100f);
        }

        //Обновляет вектора направления камеры
        private void UpdateVectors()
        {
            front.X = (float)(Math.Cos(pitch) * Math.Cos(yaw));
            front.Y = (float)(Math.Sin(pitch));
            front.Z = (float)(Math.Cos(pitch) * Math.Sin(yaw));

            front = Vector3.Normalize(front);
            right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
            up = Vector3.Cross(right, front);
        }
    }
}
