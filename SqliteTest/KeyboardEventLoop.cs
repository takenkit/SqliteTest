using System;
using System.Threading;
using System.Threading.Tasks;

// イベント処理用のデリゲート
public delegate void KeyboardEventHandler(string eventCode);

namespace SqliteTest
{
    /// <summary>
    /// キーボードからの入力イベント待ち受けクラス
    /// </summary>
    public class KeyboardEventLoop
    {
        private KeyboardEventHandler _onKeyDown;

        public KeyboardEventLoop(KeyboardEventHandler onKeyDown)
        {
            _onKeyDown = onKeyDown;
        }

        public Task Start(CancellationToken ct)
        {
            return Task.Run(() => EventLoop(ct));
        }

        void EventLoop(CancellationToken ct)
        {
            // イベントループ
            while (!ct.IsCancellationRequested)
            {
                // 文字を読み込む
                Console.Write(">");
                string line = Console.ReadLine();
                string eventCode = (line == null || line.Length == 0) ? string.Empty : line;

                // イベント処理はデリゲートを通してほかのメソッドに任せる。
                _onKeyDown(eventCode);
            }
        }
    }
}
