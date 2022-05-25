using System;
using System.Drawing;

using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace RayTracing
{
    sealed class RayTracing : GameWindow
    {
        private readonly float[] Vertices = {
            -1.0f, -1.0f, 0.0f,
            1.0f, -1.0f, 0.0f,
            1.0f, 1.0f, 0.0f,
            -1.0f, 1.0f, 0.0f
        };

        private int VertexBufferObject;
        private int VertexArrayObject;

        private Shader ShaderProgram;

        private Camera camera;
        private Vector3 startCameraPos = new Vector3(0.0f, 0.0f, 7.0f);
        private const float cameraSpeed = 2.5f;
        private const float sensitivity = 0.1f;
        private bool firstMove = true;
        private Vector2 lastMousePos;

        public RayTracing(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            ClientSize = new Size(width, height);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Сборка шейдерной программы (компиляция и линковка двух шейдеров)
            ShaderProgram = new Shader(AppDomain.CurrentDomain.BaseDirectory + "shaders/raytracing.vert",
                                       AppDomain.CurrentDomain.BaseDirectory + "shaders/raytracing.frag");

            // Создание Vertex Array Object и его привязка
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            // Создание объекта буфера вершин, его привязка и заполнение
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

            // Указание OpenGL, где искать вершины в буфере вершин
            int positionLocation = ShaderProgram.GetAttribLocation("vPos");
            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 0, 0);

            // Set the clear color to gray
            GL.ClearColor(0.15f, 0.15f, 0.15f, 0.0f);

            // Установка стартового положения камеры
            camera = new Camera(startCameraPos, (float)Size.Width / Size.Height);

            // Захват и сокрытие курсора
            // CursorGrabbed = true;
            // CursorVisible = false;
        }

        protected override void OnUnload(EventArgs e)
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteBuffer(VertexBufferObject);
            ShaderProgram.Dispose();

            base.OnUnload(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Clear the color buffer and depth.
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Bind the VAO
            GL.BindVertexArray(VertexArrayObject);
            // Use/Bind the program
            ShaderProgram.Use();

            // Свойства камеры
            ShaderProgram.SetVector3("camera.Position", new Vector3(0.0f, 0.0f, -7.0f));
            ShaderProgram.SetVector3("camera.View", new Vector3(0.0f, 0.0f, 1.0f));
            ShaderProgram.SetVector3("camera.Up", new Vector3(0.0f, 1.0f, 0.0f));
            ShaderProgram.SetVector3("camera.Side", new Vector3(1.0f, 0.0f, 0.0f));
            ShaderProgram.SetVector2("camera.Scale", new Vector2(camera.AspectRatio, 1.0f));

            // This draws the square
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);

            // Swap the front/back buffers so what we just rendered to the back buffer is displayed in the window.
            Context.SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (!Focused)
            {
                return;
            }

            KeyboardState inputKey = Keyboard.GetState();

            if (inputKey.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if (inputKey.IsKeyDown(Key.W))
                camera.Position += camera.Front * cameraSpeed * (float)e.Time;
            if (inputKey.IsKeyDown(Key.S))
                camera.Position -= camera.Front * cameraSpeed * (float)e.Time;
            if (inputKey.IsKeyDown(Key.A))
                camera.Position -= camera.Right * cameraSpeed * (float)e.Time;
            if (inputKey.IsKeyDown(Key.D))
                camera.Position += camera.Right * cameraSpeed * (float)e.Time;
            if (inputKey.IsKeyDown(Key.LShift))
                camera.Position -= camera.Up * cameraSpeed * (float)e.Time;
            if (inputKey.IsKeyDown(Key.Space))
                camera.Position += camera.Up * cameraSpeed * (float)e.Time;

            MouseState inputMouse = Mouse.GetState();

            if (firstMove)
            {
                lastMousePos = new Vector2(inputMouse.X, inputMouse.Y);
                firstMove = false;
            }
            else
            {
                var deltaX = inputMouse.X - lastMousePos.X;
                var deltaY = inputMouse.Y - lastMousePos.Y;
                lastMousePos = new Vector2(inputMouse.X, inputMouse.Y);

                camera.Yaw += deltaX * sensitivity;
                camera.Pitch -= deltaY * sensitivity;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            camera.Fov -= e.DeltaPrecise;
            if (camera.Fov >= 90.0f)
            {
                camera.Fov = 90.0f;
            }
            else if (camera.Fov <= 10.0f)
            {
                camera.Fov = 10.0f;
            }

            base.OnMouseWheel(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused)
            {
                // Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.Width, Size.Height);
            camera.AspectRatio = (float)Size.Width / Size.Height;
        }
    }
}
