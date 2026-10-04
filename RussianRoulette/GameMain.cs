// -------------------------------------------------------------------------------------------------------------------------------------------------------------
// Author: 3dapi (https://github.com/3dapi)
// -------------------------------------------------------------------------------------------------------------------------------------------------------------

using System.Windows.Forms;
using Vortice.Mathematics;

class GameMain : G2AppBase
{
    public override System.Drawing.Size ScreenSize => GameGlobal.ScreenSize;
    public override string GameName => GameGlobal.GameName;

    private enum GameScene
    {
        Start,
        Play,
        Result
    }

    private enum GameResult
    {
        None,
        PlayerWin,
        PlayerLose
    }

    private GameScene _currentScene = GameScene.Start;
    private GameResult _gameResult = GameResult.None;

    // 게임 설정
    private const int ChamberCount = 6;
    private const double EnemyShootDelay = 1.5;
    private const double PlayerPromptDelay = 1.5;

    private const int SelfShotBonus = 100;
    private const int EnemyKillScore = 500;

    // 게임 상태
    private int _bulletChamber;
    private int _currentChamber;
    private bool _isPlayerTurn;
    private double _enemyShootTimer;

    private double _playerPromptTimer;
    private bool _isWaitingForPlayerPrompt;

    private int _score;

    // 배경
    private G2Texture _bgDarkTexture = null!;
    private G2Texture _bgTexture = null!;

    // 타이틀
    private G2Texture _titleTexture = null!;

    // 시작 버튼
    private G2Texture _startButtonNormalTexture = null!;
    private G2Texture _startButtonPressedTexture = null!;
    private bool _isMouseOnStartButton;

    // 종료 버튼
    private G2Texture _exitButtonNormalTexture = null!;
    private G2Texture _exitButtonPressedTexture = null!;
    private bool _isMouseOnExitButton;

    // 인게임 이미지
    private G2Texture _enemyTexture = null!;
    private G2Texture _enemySelectedTexture = null!;

    private G2Texture _gunTexture = null!;
    private G2Texture _gunSelectedTexture = null!;

    private G2Texture _chamberIconTexture = null!;
    private G2Texture _scoreIconTexture = null!;

    private bool _isMouseOnEnemy;
    private bool _isMouseOnGun;

    // 결과 화면
    private G2Texture _cashoutButtonNormalTexture = null!;
    private G2Texture _cashoutButtonPressedTexture = null!;
    private G2Texture _resultPanelTexture = null!;

    private bool _isMouseOnCashoutButton;

    // 텍스트
    private string _statusText = string.Empty;

    private G2Font _hudText = null!;
    private G2Font _resultTitleText = null!;
    private G2Font _resultBodyText = null!;

    // 사운드
    private G2AudioSound _bgm = null!;
    private G2AudioSound _buttonClickSound = null!;
    private G2AudioSound _cylinderSpinSound = null!;
    private G2AudioSound _dryFireSound = null!;
    private G2AudioSound _gunShotSound = null!;
    private G2AudioSound _bodyFallSound = null!;

    protected override void Initialize()
    {
        //---------------------------------------
        // 리소스 경로
        //---------------------------------------

        var texBackgroundDir = "resource\\background\\";
        var texCharacterDir = "resource\\character\\";
        var texUIDir = "resource\\ui\\";
        var texWeaponDir = "resource\\weapon\\";

        var audioBgmDir = "resource\\audio\\bgm\\";
        var audioSfxDir = "resource\\audio\\sfx\\";

        //---------------------------------------
        // 배경
        //---------------------------------------

        _bgTexture = new G2Texture(
            texBackgroundDir + "room_background.png"
        );

        _bgDarkTexture = new G2Texture(
            texBackgroundDir + "room_background_dark.png"
        );

        //---------------------------------------
        // 시작 화면
        //---------------------------------------

        _titleTexture = new G2Texture(
            texUIDir + "title.png"
        );

        _startButtonNormalTexture = new G2Texture(
            texUIDir + "button_start_normal.png"
        );

        _startButtonPressedTexture = new G2Texture(
            texUIDir + "button_start_pressed.png"
        );

        _exitButtonNormalTexture = new G2Texture(
            texUIDir + "button_exit_normal.png"
        );

        _exitButtonPressedTexture = new G2Texture(
            texUIDir + "button_exit_pressed.png"
        );

        //---------------------------------------
        // 플레이 화면
        //---------------------------------------

        _enemyTexture = new G2Texture(
            texCharacterDir + "img_enemy_normal.png"
        );

        _enemySelectedTexture = new G2Texture(
            texCharacterDir + "img_enemy_selected.png"
        );

        _gunTexture = new G2Texture(
            texWeaponDir + "revolver_normal.png"
        );

        _gunSelectedTexture = new G2Texture(
            texWeaponDir + "revolver_selected.png"
        );

        _chamberIconTexture = new G2Texture(
            texUIDir + "icon_chamber.png"
        );

        _scoreIconTexture = new G2Texture(
            texUIDir + "icon_score.png"
        );

        //---------------------------------------
        // 결과 화면
        //---------------------------------------

        _cashoutButtonNormalTexture = new G2Texture(
            texUIDir + "button_cashout_normal.png"
        );

        _cashoutButtonPressedTexture = new G2Texture(
            texUIDir + "button_cashout_pressed.png"
        );

        _resultPanelTexture = new G2Texture(
            texUIDir + "panel.png"
        );

        //---------------------------------------
        // 텍스트
        //---------------------------------------

        _hudText = new G2Font("Arial", 28);

        _resultTitleText = new G2Font(
            "Arial",
            30,
            textAlignment: Vortice.DirectWrite.TextAlignment.Center,
            paragraphAlignment: Vortice.DirectWrite.ParagraphAlignment.Center
        );

        _resultBodyText = new G2Font(
            "Arial",
            18,
            textAlignment: Vortice.DirectWrite.TextAlignment.Center,
            paragraphAlignment: Vortice.DirectWrite.ParagraphAlignment.Center
        );

        //---------------------------------------
        // 사운드
        //---------------------------------------

        _bgm = new G2AudioSound(
            audioBgmDir + "longnoteone.wav"
        );

        _buttonClickSound = new G2AudioSound(
            audioSfxDir + "button_click.wav"
        );

        _cylinderSpinSound = new G2AudioSound(
            audioSfxDir + "cylinder_spin.wav"
        );

        _dryFireSound = new G2AudioSound(
            audioSfxDir + "dry_fire.wav"
        );

        _gunShotSound = new G2AudioSound(
            audioSfxDir + "gunshot.wav"
        );

        _bodyFallSound = new G2AudioSound(
            audioSfxDir + "body_fall.wav"
        );

        _bgm.Play(true);
    }

    protected override void Update()
    {
        if (_currentScene == GameScene.Start)
        {
            UpdateStartScene();
        }
        else if (_currentScene == GameScene.Play)
        {
            UpdatePlayScene();
        }
        else if (_currentScene == GameScene.Result)
        {
            UpdateResultScene();
        }
    }

    protected override void Render()
    {
        if (_currentScene == GameScene.Start)
        {
            RenderStartScene();
        }
        else if (_currentScene == GameScene.Play)
        {
            RenderPlayScene();
        }
        else if (_currentScene == GameScene.Result)
        {
            RenderResultScene();
        }
    }

    public override void Dispose()
    {
        // 배경
        _bgTexture.Dispose();
        _bgDarkTexture.Dispose();

        // 시작 화면
        _titleTexture.Dispose();

        _startButtonNormalTexture.Dispose();
        _startButtonPressedTexture.Dispose();

        _exitButtonNormalTexture.Dispose();
        _exitButtonPressedTexture.Dispose();

        // 플레이 화면
        _enemyTexture.Dispose();
        _enemySelectedTexture.Dispose();

        _gunTexture.Dispose();
        _gunSelectedTexture.Dispose();

        _chamberIconTexture.Dispose();
        _scoreIconTexture.Dispose();

        // 결과 화면
        _cashoutButtonNormalTexture.Dispose();
        _cashoutButtonPressedTexture.Dispose();
        _resultPanelTexture.Dispose();

        // 텍스트
        _hudText.Dispose();
        _resultTitleText.Dispose();
        _resultBodyText.Dispose();

        // 사운드
        _bgm.Dispose();
        _buttonClickSound.Dispose();
        _cylinderSpinSound.Dispose();
        _dryFireSound.Dispose();
        _gunShotSound.Dispose();
        _bodyFallSound.Dispose();

        base.Dispose();
    }

    private void UpdateStartScene()
    {
        var mousePos = Input.MousePosition;

        _isMouseOnStartButton =
            mousePos.X >= 240 && mousePos.X <= 407 &&
            mousePos.Y >= 415 && mousePos.Y <= 484;

        _isMouseOnExitButton =
            mousePos.X >= 540 && mousePos.X <= 707 &&
            mousePos.Y >= 415 && mousePos.Y <= 484;

        if (_isMouseOnStartButton &&
            Input.IsButtonDown(MouseButtons.Left))
        {
            _buttonClickSound.Play();

            SetupGame();

            _currentScene = GameScene.Play;
            _cylinderSpinSound.Play();
        }

        if (_isMouseOnExitButton &&
            Input.IsButtonDown(MouseButtons.Left))
        {
            _buttonClickSound.Play();
            Close();
        }
    }

    private void RenderStartScene()
    {
        _bgDarkTexture.Draw();
        _titleTexture.Draw(280, 110);

        // 시작 버튼
        if (_isMouseOnStartButton)
        {
            _startButtonPressedTexture.Draw(230, 355);
        }
        else
        {
            _startButtonNormalTexture.Draw(230, 355);
        }

        // 종료 버튼
        if (_isMouseOnExitButton)
        {
            _exitButtonPressedTexture.Draw(530, 355);
        }
        else
        {
            _exitButtonNormalTexture.Draw(530, 355);
        }
    }

    private void UpdatePlayScene()
    {
        var mousePos = Input.MousePosition;

        _isMouseOnEnemy =
            mousePos.X >= 320 && mousePos.X <= 600 &&
            mousePos.Y >= 160 && mousePos.Y <= 455;

        _isMouseOnGun =
            mousePos.X >= 610 && mousePos.X <= 820 &&
            mousePos.Y >= 350 && mousePos.Y <= 530;

        //---------------------------------------
        // 결과 문구 출력 대기
        //---------------------------------------

        if (_isWaitingForPlayerPrompt)
        {
            _playerPromptTimer += DeltaTime;

            if (_playerPromptTimer >= PlayerPromptDelay)
            {
                _statusText =
                    "적 또는 리볼버를 선택하세요.";

                _isWaitingForPlayerPrompt = false;
                _isPlayerTurn = true;
            }

            return;
        }

        //---------------------------------------
        // 플레이어 차례
        //---------------------------------------

        if (_isPlayerTurn)
        {
            // 적에게 발사
            if (_isMouseOnEnemy &&
                Input.IsButtonDown(MouseButtons.Left))
            {
                ShootEnemy();
            }
            // 자신에게 발사
            else if (_isMouseOnGun &&
                     Input.IsButtonDown(MouseButtons.Left))
            {
                ShootPlayer();
            }
        }
        //---------------------------------------
        // 적의 차례
        //---------------------------------------
        else
        {
            _enemyShootTimer += DeltaTime;

            if (_enemyShootTimer >= EnemyShootDelay)
            {
                EnemyShootPlayer();
            }
        }
    }

    private void RenderPlayScene()
    {
        _bgTexture.Draw();

        // 적
        if (_isPlayerTurn &&
            !_isWaitingForPlayerPrompt &&
            _isMouseOnEnemy)
        {
            _enemySelectedTexture.Draw(280, 110);
        }
        else
        {
            _enemyTexture.Draw(280, 110);
        }

        // 리볼버
        if (_isPlayerTurn &&
            !_isWaitingForPlayerPrompt &&
            _isMouseOnGun)
        {
            _gunSelectedTexture.Draw(610, 350);
        }
        else
        {
            _gunTexture.Draw(610, 350);
        }

        int usedChamberCount =
            _currentChamber - 1;

        // 약실 정보
        _chamberIconTexture.Draw(
            new Rect(20, 30, 64, 64),
            new Rect(0, 0, 192, 192)
        );

        _hudText.DrawText(
            $"{usedChamberCount}/{ChamberCount}",
            new Rect(92, 35, 120, 50),
            new Color4(1.0f, 0.8f, 0.3f, 1.0f)
        );

        // 점수 정보
        _scoreIconTexture.Draw(
            new Rect(800, 30, 64, 64),
            new Rect(0, 0, 192, 192)
        );

        _hudText.DrawText(
            $"{_score}",
            new Rect(872, 35, 75, 50),
            new Color4(1.0f, 0.8f, 0.3f, 1.0f)
        );

        // 현재 상황
        _hudText.DrawText(
            _statusText,
            new Rect(150, 470, 700, 50),
            new Color4(1.0f, 1.0f, 1.0f, 1.0f)
        );
    }

    private void UpdateResultScene()
    {
        var mousePos = Input.MousePosition;

        _isMouseOnCashoutButton =
            mousePos.X >= 410 && mousePos.X <= 550 &&
            mousePos.Y >= 423 && mousePos.Y <= 480;

        if (_isMouseOnCashoutButton &&
            Input.IsButtonDown(MouseButtons.Left))
        {
            _buttonClickSound.Play();
            _currentScene = GameScene.Start;
        }
    }

    private void RenderResultScene()
    {
        _bgDarkTexture.Draw();

        // 결과 패널
        _resultPanelTexture.Draw(
            new Rect(264, 15, 432, 432),
            new Rect(0, 0, 384, 384)
        );

        // 승패 제목
        if (_gameResult == GameResult.PlayerWin)
        {
            _resultTitleText.DrawText(
                "승리",
                new Rect(350, 105, 260, 50),
                new Color4(1.0f, 0.8f, 0.3f, 1.0f)
            );
        }
        else if (_gameResult == GameResult.PlayerLose)
        {
            _resultTitleText.DrawText(
                "패배",
                new Rect(350, 105, 260, 50),
                new Color4(0.9f, 0.2f, 0.2f, 1.0f)
            );
        }

        // 결과 설명
        _resultBodyText.DrawText(
            _statusText,
            new Rect(350, 165, 260, 70),
            new Color4(1.0f, 1.0f, 1.0f, 1.0f)
        );

        // 최종 점수
        _scoreIconTexture.Draw(
            new Rect(400, 250, 64, 64),
            new Rect(0, 0, 192, 192)
        );

        _hudText.DrawText(
            $"{_score}",
            new Rect(472, 258, 90, 60),
            new Color4(1.0f, 0.8f, 0.3f, 1.0f)
        );

        // 상금 수령 버튼
        if (_isMouseOnCashoutButton)
        {
            _cashoutButtonPressedTexture.Draw(
                new Rect(400, 370, 160, 160),
                new Rect(0, 0, 192, 192)
            );
        }
        else
        {
            _cashoutButtonNormalTexture.Draw(
                new Rect(400, 370, 160, 160),
                new Rect(0, 0, 192, 192)
            );
        }
    }

    private bool PullTrigger()
    {
        bool isBulletFired =
            _currentChamber == _bulletChamber;

        if (isBulletFired)
        {
            _gunShotSound.Play();
            _bodyFallSound.Play();
        }
        else
        {
            _dryFireSound.Play();
        }

        // 다음 약실로 이동
        _currentChamber++;

        return isBulletFired;
    }

    private void ShootPlayer()
    {
        bool isBulletFired = PullTrigger();

        if (isBulletFired)
        {
            _score = 0;

            _statusText =
                "실탄에 맞았습니다.\n" +
                "점수를 모두 잃었습니다.";

            _gameResult = GameResult.PlayerLose;
            _currentScene = GameScene.Result;
        }
        else
        {
            _score += SelfShotBonus;

            _statusText =
                $"공포탄입니다! 위험 보너스 {SelfShotBonus}점을 획득했습니다.";

            WaitForPlayerPrompt();
        }
    }

    private void ShootEnemy()
    {
        bool isBulletFired = PullTrigger();

        if (isBulletFired)
        {
            _score += EnemyKillScore;

            _statusText =
                "적을 처치했습니다!";

            _gameResult = GameResult.PlayerWin;
            _currentScene = GameScene.Result;
        }
        else
        {
            _statusText =
                "공포탄입니다. 적이 총을 겨누고 있습니다...";

            _isPlayerTurn = false;
            _enemyShootTimer = 0.0;
        }
    }

    private void EnemyShootPlayer()
    {
        bool isBulletFired = PullTrigger();

        if (isBulletFired)
        {
            _score = 0;

            _statusText =
                "실탄에 맞았습니다.\n" +
                "점수를 모두 잃었습니다.";

            _gameResult = GameResult.PlayerLose;
            _currentScene = GameScene.Result;
        }
        else
        {
            _statusText =
                "적이 발사했지만 공포탄이었습니다.";

            WaitForPlayerPrompt();
        }
    }

    private void WaitForPlayerPrompt()
    {
        // 결과 문구를 보여주는 동안 입력을 막음
        _isPlayerTurn = false;
        _playerPromptTimer = 0.0;
        _isWaitingForPlayerPrompt = true;
    }

    private void SetupGame()
    {
        // 1부터 6 사이에서 실탄 위치 결정
        _bulletChamber =
            Random.Shared.Next(1, ChamberCount + 1);

        _currentChamber = 1;
        _isPlayerTurn = true;
        _enemyShootTimer = 0.0;

        _playerPromptTimer = 0.0;
        _isWaitingForPlayerPrompt = false;

        _score = 0;
        _gameResult = GameResult.None;

        _statusText =
            "적 또는 리볼버를 선택하세요.";
    }
}