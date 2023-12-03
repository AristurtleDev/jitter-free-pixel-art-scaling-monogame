/* ----------------------------------------------------------------------------
MIT License

Copyright (c) 2022 Christopher Whitley

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
---------------------------------------------------------------------------- */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JitterFree;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _townLeft;
    private Texture2D _townRight;
    private Camera _camera;
    private float _zoom = 1.0f;
    private readonly float _maxZoom = 1.5f;
    private float _timer = 0.0f;
    private readonly float _duration = 10.0f;
    private int direction = 1;
    private Effect _jitterFree;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1200;
        _graphics.PreferredBackBufferHeight = 700;
        _graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;


    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _townLeft = Content.Load<Texture2D>("TownLeft");
        _townRight = Content.Load<Texture2D>("TownRight");
        _jitterFree = Content.Load<Effect>("JitterFree");
        _camera = new Camera(GraphicsDevice.Viewport);
        _camera.Position = new Vector2(1200, 700) * 0.5f;
        _camera.CenterOrigin();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        LerpCamerZoom(gameTime);
        base.Update(gameTime);
    }

    //  Just zooms the camera in an out based on a timer with a lerp.
    private void LerpCamerZoom(GameTime gameTime)
    {
        if (_timer > _duration)
        {
            _timer = _duration;
            direction = -1;
        }
        else if (_timer < 0.0f)
        {
            _timer = 0.0f;
            direction = 1;
        }

        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds * direction;
        _zoom = MathHelper.Lerp(1.0f, _maxZoom, _timer / _duration);

        _camera.Zoom = new Vector2(_zoom, _zoom);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        //  Pass texture size to shader
        _jitterFree.Parameters["textureSize"].SetValue(_townLeft.Bounds.Size.ToVector2());

        //  Draw left side using shader
        _spriteBatch.Begin(transformMatrix: _camera.TransformationMatrix, effect: _jitterFree);
        _spriteBatch.Draw(_townLeft, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 5.0f, SpriteEffects.None, 0.0f);
        _spriteBatch.End();

        //  Draw right side without shader
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.TransformationMatrix);
        _spriteBatch.Draw(_townRight, new Vector2(_townLeft.Width * 5, 0), null, Color.White, 0.0f, Vector2.Zero, 5.0f, SpriteEffects.None, 0.0f);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
