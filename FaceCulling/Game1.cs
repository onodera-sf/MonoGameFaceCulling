using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FaceCulling
{
	/// <summary>
	/// ゲームメインクラス
	/// </summary>
	public class Game1 : Game
	{
    /// <summary>
    /// グラフィックデバイス管理クラス
    /// </summary>
    private readonly GraphicsDeviceManager _graphics = null;

    /// <summary>
    /// スプライトのバッチ化クラス
    /// </summary>
    private SpriteBatch _spriteBatch = null;

    /// <summary>
    /// ポリゴン用頂点データリスト
    /// </summary>
    private VertexPositionColor[] _triangleVertives = null;

    /// <summary>
    /// 面の表側を示すラインの頂点データリスト
    /// </summary>
    private VertexPositionColor[] _lineVertices = null;

    /// <summary>
    /// 基本エフェクト
    /// </summary>
    private BasicEffect _basicEffect = null;

    /// <summary>
    /// スプライトでテキストを描画するためのフォント
    /// </summary>
    private SpriteFont _font = null;

    /// <summary>
    /// カメラの回転位置
    /// </summary>
    private float _cameraRotate = -0.5f;

    /// <summary>
    /// ポリゴンの描画を決定するためのラスタライザステート
    /// </summary>
    private RasterizerState _rasterizerState = RasterizerState.CullCounterClockwise;

    /// <summary>
    /// ボタンを押している状態かどうかを判定するためのフラグ
    /// </summary>
    private bool _isPushed = false;


    /// <summary>
    /// GameMain コンストラクタ
    /// </summary>
    public Game1()
    {
      // グラフィックデバイス管理クラスの作成
      _graphics = new GraphicsDeviceManager(this);

      // ゲームコンテンツのルートディレクトリを設定
      Content.RootDirectory = "Content";

      // マウスカーソルを表示
      IsMouseVisible = true;
    }

    /// <summary>
    /// ゲームが始まる前の初期化処理を行うメソッド
    /// グラフィック以外のデータの読み込み、コンポーネントの初期化を行う
    /// </summary>
    protected override void Initialize()
    {
      // TODO: ここに初期化ロジックを書いてください

      // コンポーネントの初期化などを行います
      base.Initialize();
    }

    /// <summary>
    /// ゲームが始まるときに一回だけ呼ばれ
    /// すべてのゲームコンテンツを読み込みます
    /// </summary>
    protected override void LoadContent()
    {
      // テクスチャーを描画するためのスプライトバッチクラスを作成します
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      // エフェクトを作成
      _basicEffect = new BasicEffect(GraphicsDevice);

      // エフェクトで頂点カラーを有効にする
      _basicEffect.VertexColorEnabled = true;

      // プロジェクションマトリックスをあらかじめ設定
      _basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(
              MathHelper.ToRadians(45.0f),
              (float)GraphicsDevice.Viewport.Width /
                  (float)GraphicsDevice.Viewport.Height,
              1.0f,
              100.0f
          );

      // ポリゴンの頂点データを作成する
      _triangleVertives = new VertexPositionColor[3];

      _triangleVertives[0] = new VertexPositionColor(new Vector3(0.0f, 3.0f, 0.0f),
                                                         Color.Red);
      _triangleVertives[1] = new VertexPositionColor(new Vector3(3.0f, -2.0f, 0.0f),
                                                         Color.Blue);
      _triangleVertives[2] = new VertexPositionColor(new Vector3(-3.0f, -2.0f, 0.0f),
                                                         Color.Green);

      // 面の表側を指すようにラインを作成
      _lineVertices = new VertexPositionColor[2];

      _lineVertices[0] = new VertexPositionColor(new Vector3(0.0f, -1.0f, 0.0f),
                                                     Color.Blue);
      _lineVertices[1] = new VertexPositionColor(new Vector3(0.0f, -1.0f, 10.0f),
                                                     Color.Blue);

      // フォントをコンテンツパイプラインから読み込む
      _font = Content.Load<SpriteFont>("Font");
    }

    /// <summary>
    /// ゲームが終了するときに一回だけ呼ばれ
    /// すべてのゲームコンテンツをアンロードします
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: ContentManager で管理されていないコンテンツを
      //       ここでアンロードしてください
    }

    /// <summary>
    /// 描画以外のデータ更新等の処理を行うメソッド
    /// 主に入力処理、衝突判定などの物理計算、オーディオの再生など
    /// </summary>
    /// <param name="gameTime">このメソッドが呼ばれたときのゲーム時間</param>
    protected override void Update(GameTime gameTime)
    {
      // キーボードの情報取得
      KeyboardState keyState = Keyboard.GetState();

      // マウスの情報取得
      MouseState mouseState = Mouse.GetState();

      // ゲームパッドの情報取得
      GamePadState padState = GamePad.GetState(PlayerIndex.One);

      // ゲームパッドの Back ボタン、またはキーボードの Esc キーを押したときにゲームを終了させます
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      {
        Exit();
      }

      ///// カリングの設定 /////
      if (keyState.IsKeyDown(Keys.A) ||
          mouseState.LeftButton == ButtonState.Pressed ||
          padState.Buttons.A == ButtonState.Pressed)
      {
        if (_isPushed == false)
        {
          // ボタンが押された瞬間

          if (_rasterizerState.CullMode == CullMode.None)
          {
            // 反時計回りをカリング
            _rasterizerState = RasterizerState.CullCounterClockwise;
          }
          else if (_rasterizerState.CullMode == CullMode.CullCounterClockwiseFace)
          {
            // 時計回りをカリング
            _rasterizerState = RasterizerState.CullClockwise;
          }
          else if (_rasterizerState.CullMode == CullMode.CullClockwiseFace)
          {
            // カリングなし
            _rasterizerState = RasterizerState.CullNone;
          }
        }

        _isPushed = true;
      }
      else
      {
        _isPushed = false;
      }

      ///// カメラの位置回転 /////
      _cameraRotate += (float)gameTime.ElapsedGameTime.TotalSeconds;

      // ビューマトリックスを設定
      _basicEffect.View = Matrix.CreateLookAt(
              Vector3.Transform(new Vector3(0.0f, 0.0f, 15.0f),
                  Matrix.CreateRotationY(_cameraRotate)),
              Vector3.Zero,
              Vector3.Up
          );

      // TODO: ここに更新処理を記述してください

      // 登録された GameComponent を更新する
      base.Update(gameTime);
    }

    /// <summary>
    /// 描画処理を行うメソッド
    /// </summary>
    /// <param name="gameTime">このメソッドが呼ばれたときのゲーム時間</param>
    protected override void Draw(GameTime gameTime)
    {
      // 画面を指定した色でクリアします
      GraphicsDevice.Clear(Color.CornflowerBlue);

      // カリングのためのラスタライザステートの設定
      GraphicsDevice.RasterizerState = _rasterizerState;

      // 深度バッファの有効化
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;

      // パスの数だけ繰り替えし描画 (といっても直接作成した BasicEffect は通常１回)
      foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
      {
        // パスの開始
        pass.Apply();

        // 三角形を描画する
        GraphicsDevice.DrawUserPrimitives(
            PrimitiveType.TriangleList,
            _triangleVertives,
            0,
            1
        );

        // 面の表側を示すラインを描画
        GraphicsDevice.DrawUserPrimitives(
            PrimitiveType.LineList,
            _lineVertices,
            0,
            1
        );
      }

      // スプライトの描画準備
      _spriteBatch.Begin();

      // カリングモードを表示
      _spriteBatch.DrawString(_font,
          "A or LeftButton:Change CullMode.",
          new Vector2(10, 30), Color.White);

      _spriteBatch.DrawString(_font,
          "CullMode:" + _rasterizerState.CullMode.ToString(),
          new Vector2(10, 60), Color.Yellow);

      // スプライトの一括描画
      _spriteBatch.End();

      // 登録された DrawableGameComponent を描画する
      base.Draw(gameTime);
    }

  }
}
