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
using System.Diagnostics;

namespace TripleSTest;

public class Game1 : Game, ITripleSGame
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public Renderer _Renderer { get; private set; }
    public LevelHandler _LevelHandler { get; private set; }

    Collider line;
    Transform rect;
    Collider collider;
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

        line = new Collider(new Rectangle(50, 30, 20, 10), 0.5f, 1, false);
        rect = new Transform(new Vector2(30, 3), 16, 16, 4f, true, true);
        rect.Velocity = new Vector2(1f, 0f);
        collider = new Collider(new Rectangle(0, 30, 40, 10));

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
        _Renderer.EnableLigthing = true;
        _Renderer.Load(Content, "effects/lighting");

        DebugConsole.DebugFont = Content.Load<SpriteFont>("debug");
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

            rect.Velocity = CollisionEngine.UpdateVelocity(rect, (float)gameTime.ElapsedGameTime.TotalSeconds);

            if (CollisionEngine.CollideAll(collider, rect, out Vector2 pos, out Vector2 vel))
            {
                rect.Velocity = vel;
                rect.Position = pos;
            }

            if (CollisionEngine.CollideAll(line, rect, out pos, out vel))
            {
                rect.Velocity = vel;
                rect.Position = pos;
            }
            rect.ApplyVelocity();

            if (GameInputs.OncePress(Keys.D1))
                animator.PlayAnimation("first");
            if (GameInputs.OncePress(Keys.D2))
                animator.PlayAnimation("second");

            animator.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);

            _Renderer.View.TargetPosition = new Vector2(rect.Position.X + rect.Width / 2, rect.Position.Y + rect.Height / 2);
        }

        DebugConsole.Update();
    }

    private void Command(object sender, CommandEventArgs e)
    {
        if (e.Command == "fps")
        {
            Debug.Print(fps.ToString());
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        _Renderer.PhaseOne();
        _Renderer.BasicDraw(testTexture, collider.GetRectangle(), 0, 0, col: Color.Blue);
        _Renderer.PhaseTwo();

        Oragnizer.DrawComps(_Renderer);
        _LevelHandler.StandardDraw(_Renderer);

        _Renderer.BasicDraw(animator.OutputTexture, rect.Position, 1, 0, source: animator.OutputSource);

        _Renderer.PhaseThree();

        UIManager.Draw(_Renderer);
        DebugConsole.Draw(_Renderer);

        _Renderer.PhaseFour();
    }
}