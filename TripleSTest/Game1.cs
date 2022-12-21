using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using TripleS;
using TripleS.Physics;
using TripleS.Scripting;
using TripleS.Lighting;
using TripleS.UI;
using TripleS.Animation;

using System;
using TripleSTest.Services;
using TiledCS;
using System.Linq;

namespace TripleSTest;

public class Game1 : Game, ITripleSGame
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public Renderer _Renderer { get; private set; }
    public LevelHandler _LevelHandler { get; private set; }

    Line test;
    Rectangle rect;
    Vector2 rectVel;
    Vector2 rectPos;
    Rectangle collider;
    Texture2D testTexture;
    Animator animator;

    float fps;
    bool paused = false;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.TextInput += GameInputs.TextInputHand;
    }

    protected override void Initialize()
    {
        SSS.Init(this, "TestGame", 2);
        InitOrganize();

        _LevelHandler = new LevelHandler("lev", "til", "script", 6);
        var entTypes = new Type[1] { typeof(TestEnt) };
        _LevelHandler.Initialize(0, entTypes);
        _LevelHandler.DrawLevel = false;

        test = new Line(new Vector2(50, 30), new Vector2(70, 40), SlopeInversion.Bottom);
        rectPos = new Vector2(30, 3);
        rect = new Rectangle((int)rectPos.X, (int)rectPos.Y, 16, 16);
        rectVel = new Vector2(1f, 0f);
        collider = new Rectangle(0, 30, 40, 10);

        DebugConsole.EnterInput = Keys.Enter;
        DebugConsole.ToggleInput = Keys.OemTilde;
        DebugConsole.OnCommand += Command;

        GameStateManager.CurrentSave = 0;

        base.Initialize();
    }

    protected void InitOrganize()
    {
        Oragnizer.AddComponent(new Player(this), true);
        Oragnizer.AddService(new TestService());
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _Renderer = new Renderer(_spriteBatch, SSS.DefaultInfo(), new GameView(), 1);
        _Renderer.View.Zoom = 6;

        _Renderer.Lighting.Lights.Add(new LightData(new Vector2(40,5), 1f, 1f, new Vector3(1, 1, 1)));
        _Renderer.EnableLigthing = false;
        _Renderer.Load(Content, "effects/lighting");

        Debuger.DebugFont = Content.Load<SpriteFont>("debug");
        DebugConsole.FontSize = 12;

        Oragnizer.LoadComps(Content);
        _LevelHandler.Load(Content);

        testTexture = new Texture2D(GraphicsDevice, 1, 1);
        testTexture.SetData(new Color[] { new Color(1, 1, 1, 1f) });

        Animation[] anims = new Animation[2] {
            new Animation("first", AnimationType.Strip, Content.Load<Texture2D>("one"), 4, 16, 16, 1, true), 
            new Animation("second", AnimationType.Strip, Content.Load<Texture2D>("two"), 4, 16, 16, 1.4f, false) 
        };
        animator = new Animator(anims, "first");
    }

    protected override void Update(GameTime gameTime)
    {
        fps = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
        GameInputs.UpdateState();

        if (GameInputs.OncePress(Keys.Escape) && !DebugConsole.Active)
            paused = !paused;
        paused = DebugConsole.Active;

        if (!paused)
        {
            _Renderer.View.Update(_graphics.GraphicsDevice);
            Oragnizer.UpdateMinimalComps(gameTime);
            _LevelHandler.Update();

            rect.Location = rectPos.ToPoint();
            rectVel = CollisionEngine.UpdateVelocity(rectVel, 4, (float)gameTime.ElapsedGameTime.TotalSeconds);
            rectVel = CollisionEngine.CollideRectangle(collider, rect, rectVel);
            rectVel = CollisionEngine.CollideLine(test, rect, rectVel);
            rectPos += rectVel;

            if (GameInputs.OncePress(Keys.D1))
                animator.PlayAnimation("first");
            if (GameInputs.OncePress(Keys.D2))
                animator.PlayAnimation("second");

            animator.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);

            _Renderer.View.TargetPosition = new Vector2(rectPos.X + rect.Width / 2, rectPos.Y + rect.Height / 2);
        }

        DebugConsole.Update();
    }

    private void Command(object sender, CommandEventArgs e)
    {
        if (e.Command == "fps")
        {
            Debuger.Print(fps);
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        _Renderer.PhaseOne();
        _Renderer.BasicDraw(testTexture, collider, 0, 0, col: Color.Blue);
        _Renderer.PhaseTwo();

        Oragnizer.DrawComps(_Renderer);
        _LevelHandler.StandardDraw(_Renderer);

        _Renderer.BasicDraw(animator.OutputTexture, rect.Location.ToVector2(), 1, 0, source: animator.OutputSource);
        Debuger.DrawLine(_Renderer, test, testTexture, Color.Red);

        _Renderer.PhaseThree();

        UIManager.Draw(_Renderer);
        DebugConsole.Draw(_Renderer);

        _Renderer.PhaseFour();
    }
}